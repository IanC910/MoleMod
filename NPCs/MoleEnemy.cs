using System.Net.Mime;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace MoleMod.NPCs
{
	
	public class MoleEnemy : ModNPC
	{

		private enum AIStates
		{
			Idle,
			Digging,
			Tunneling,
			Jumping,
			Attacking,
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Mole Enemy");
			Main.npcFrameCount[Type] = 4;
		}

		public override void SetDefaults() {
			NPC.width = 62;
			NPC.height = 62;
			NPC.damage = 50;
			NPC.defense = 10;
			NPC.lifeMax = 100;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;

			AIType = NPCID.Zombie;
			AnimationType = NPCID.Zombie;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			// we would like this npc to spawn in the overworld.
			return SpawnCondition.Underground.Chance * 0.3f;
		}

		

		public override void AI() {
			if(NPC.aiStyle != -1) {
				State = (float)AIStates.Attacking;
			}
			switch (NPC.ai[0]) {
				case (float)AIStates.Idle:
					Idle();
					break;
				case (float)AIStates.Digging:
					Digging();
					break;
				case (float)AIStates.Tunneling:
					Tunneling();
					break;
				case (float)AIStates.Jumping:
					Jumping();
					break;
				case (float)AIStates.Attacking:
					Attacking();
					break;
			}
		}

		public override void HitEffect(int hitDirection, double damage) {
			if (Main.netMode == NetmodeID.Server) {
				return;
			}

			if (NPC.life <= 0) {
				// These gores work by simply existing as a texture inside any folder which path contains "Gores/"
				int bottomGoreType = Mod.Find<ModGore>("MoleEnemyBottom").Type;
				int topGoreType = Mod.Find<ModGore>("MoleEnemyTop").Type;
				var entitySource = NPC.GetSource_Death();
				for (int i = 0; i < 1; i++) {
					Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), bottomGoreType);
					Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), topGoreType);
				}
				//SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
			}
		}
		
		public float State
			{
				get => NPC.ai[0]; //When getting this variable's value, instead return the vaule of npc.ai[0] 
				set => NPC.ai[0] = value; //when setting this variable to something, instead set the value of npc.ai[0]
			}
		float Timer
			{
				get => NPC.ai[1]; //When getting this variable's value, instead return the vaule of npc.ai[1] 
				set => NPC.ai[1] = value; //when setting this variable to something, instead set the value of npc.ai[1]
			}

		public override void FindFrame(int frameHeight) {
			NPC.spriteDirection = NPC.direction;
		}

		public override bool? CanFallThroughPlatforms() {
			if (State == (float)AIStates.Attacking && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y) {
				// If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
				return true;
			}
			return false;
		}

		public void Idle() {
			NPC.TargetClosest(true);
			NPC.frame.Y = 0;
			Timer++;
			State = (float)AIStates.Digging;
			Timer = 0;
		}

		public void Digging() {
			Timer++;
			NPC.TargetClosest(true);
			
			if (Timer == 1) {
				NPC.velocity = new Vector2(0, -7f);
				NPC.noTileCollide = true;
			}
			if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) {
				for (int i = 0; i < 15; i++) {
					Dust dust;
					dust = Main.dust[Terraria.Dust.NewDust(NPC.position+new Vector2(0,15), 30, 0, 0, 0f, -4.72093f, 0, new Color(255,255,255), 1.8023255f)];
					dust.fadeIn = 0;
				}
				State = (float)AIStates.Tunneling;
				Timer = 0;
			}			
		}

		public void Tunneling() {
			NPC.dontTakeDamage = true;
			Timer++;
			NPC.behindTiles = true;
			NPC.TargetClosest(true);
			// if npc is within 20 -+ of target switch to jumping
			if (Main.player[NPC.target].Distance(NPC.Center) < 150) {
				State = (float)AIStates.Jumping;
				NPC.dontTakeDamage = false;
				Timer = 0;
			}
			
			if (Timer > 1) {
				NPC.noTileCollide = true;
				if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) {
					State = (float)AIStates.Attacking;
					NPC.dontTakeDamage = false;
					Timer = 0;
				} 
				if ((!Collision.SolidCollision(NPC.position-(new Vector2(0,50)), NPC.width, NPC.height) && Math.Abs(Main.player[NPC.target].position.X-NPC.position.X) < 150)){
					State = (float)AIStates.Jumping;
					NPC.dontTakeDamage = false;
					Timer = 0;
				}
				if (Collision.SolidCollision(NPC.position-(new Vector2(0,45)), NPC.width, NPC.height) && (Main.player[NPC.target].position.Y)-NPC.position.Y < 0) {
					NPC.velocity.Y = -1f;
				}
				NPC.velocity.X += NPC.direction * 1.001f;
				if (Math.Abs(NPC.velocity.X) >= 4f){
					NPC.velocity.X = NPC.direction * 4f;
				}
				Dust dust;
				dust = Main.dust[Terraria.Dust.NewDust(NPC.position+new Vector2(0,15), NPC.width, 0, 0, 0f, -4.0f, 0, new Color(255,255,255), 1.8023255f)];
				dust.fadeIn = 0;
			}
		}

		public void Jumping() {
			Timer++;
			if (Timer == 1) {
				NPC.velocity = new Vector2(0, -10f);
			}
			else if (!Collision.SolidCollision(NPC.position, NPC.width-20, NPC.height-20)) {
				for (int i = 0; i < 15; i++) {
					Dust dust;
					dust = Main.dust[Terraria.Dust.NewDust(NPC.position+new Vector2(0,15), 30, 0, 0, 0f, -4.72093f, 0, new Color(255,255,255), 1.8023255f)];
					dust.fadeIn = 0;
				}
				NPC.noTileCollide = false;
				State = (float)AIStates.Attacking;
				Timer = 0;
			}
		}

		public void Attacking() {;
			NPC.TargetClosest(true);
			State = (float)AIStates.Attacking;
			NPC.aiStyle = 3;	
			if (Collision.SolidCollision(NPC.position, 1, 1)) {
				NPC.aiStyle = -1;
				State = (float)AIStates.Digging;
				Timer = 0;
			}
			if ((Math.Abs(Main.player[NPC.target].position.X-NPC.position.X) > 350 || (Main.player[NPC.target].position.Y)-NPC.position.Y >= 350) && Collision.SolidCollision(NPC.position+new Vector2(0,1), NPC.width, NPC.height)) {
				NPC.aiStyle = -1;
				State = (float)AIStates.Digging;
				Timer = 0;
			}
		}
	}
}

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

namespace MoleMod.NPCs
{

	public class MoleEnemy : ModNPC
	{

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Mole Enemy");
			Main.npcFrameCount[Type] = 1;
		}

		public override void SetDefaults() {
			NPC.width = 64;
			NPC.height = 64;
			NPC.damage = 50;
			NPC.defense = 10;
			NPC.lifeMax = 100;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0;
			NPC.aiStyle = 3;

			AIType = NPCID.Skeleton;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit) {
			damage = 9999;
		}
	}
}

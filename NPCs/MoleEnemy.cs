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

namespace ExampleMod.Content.NPCs.MoleEnemy
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
		}
	}
}

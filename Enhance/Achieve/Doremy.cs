using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPets.Content.Projectiles.Pets;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Doremy : BaseEnhance
    {
        public override string Text => GetText("Doremy");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<DoremyPillow>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (player.sleeping.isSleeping && Main.LocalPlayer == player && Main.rand.NextBool(10000))
            {
                PopupText.NewText(new AdvancedPopupRequest() { Velocity = -Vector2.UnitY, DurationInFrames = 90, Color = Color.White, Text = GetText("Ez") }, player.Center - Vector2.UnitY * player.height);
                ModContent.GetInstance<GoldenYuanCoupon>().Condition.Complete();
            }
        }
        public override void SystemPostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || NPC.travelNPC || Main.IsFastForwardingTime() || !Main.dayTime || Main.time >= 27000.0)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.sleeping.isSleeping || !player.EnableEnhance<DoremyPillow>())
                    return;

                if (Main.rand.NextDouble() < Main.dayRate / (27000.0 * 4))
                {
                    int num8 = 0;
                    for (int i = 0; i < 200; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].townNPC && Main.npc[i].type != NPCID.OldMan && Main.npc[i].type != NPCID.SkeletonMerchant)
                            num8++;
                    }

                    if (num8 >= 2)
                        WorldGen.SpawnTravelNPC();
                }
            }
        }
    }
}

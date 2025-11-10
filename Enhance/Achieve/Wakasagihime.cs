using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Wakasagihime : BaseEnhance
    {
        public override string Text => GetText("Wakasagihime");
        public override string[] ExperimentalText => [GetText("Wakasagihime_1"), GetText("Wakasagihime_2")];
        public override bool[] Experimental => [Config.Wakasagihime, Config.Wakasagihime_2];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<WakasagihimeFishingRod>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            float multiplier = 1f;

            if (player.accMerman)
                multiplier *= 2f;

            if (player.wet || player.dripping)
            {
                if (player.dripping && !player.wet)
                    multiplier *= 0.5f;

                player.statDefense += (int)Math.Ceiling(4 * multiplier);
                player.GetCritChance(DamageClass.Generic) += 4 * multiplier;
                player.GetAttackSpeed(DamageClass.Generic) += 0.06f * multiplier;
                player.pickSpeed -= 0.12f * multiplier;
                player.GetDamage(DamageClass.Generic) += 0.08f * multiplier;
                player.GetKnockback(DamageClass.Summon) += 0.5f * multiplier;
                player.moveSpeed += 0.28f * multiplier;
            }

            player.accMerman = true;

            if (Config.Wakasagihime)
            {
                player.arcticDivingGear = true;
                player.accFlipper = true;
                player.accDivingHelm = true;
                player.iceSkate = true;
                if (!player.wet)
                    Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.05f, 0.15f, 0.225f);

                if (player.wet)
                    Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.4f, 1.2f, 1.8f);
            }

            if (Config.Wakasagihime_2)
            {
                foreach (int type in TouhouPetsEx.OceanEnemy)
                    player.npcTypeNoAggro[type] = true;
            }
        }
        public override void PlayerModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 1.15f;
        }
        public override void ItemOnCreated(Item item, ItemCreationContext context)
        {
            if (!Main.LocalPlayer.EnableEnhance<WakasagihimeFishingRod>() || !Main.LocalPlayer.ZoneBeach || !Main.LocalPlayer.wet || Main.LocalPlayer.lavaWet || Main.LocalPlayer.honeyWet || Main.LocalPlayer.shimmerWet)
                return;

            ModContent.GetInstance<Subnautica>().Condition.Complete();
        }
    }
}

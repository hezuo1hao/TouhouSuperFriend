using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Rin : BaseEnhance
    {
        public override string Text => GetText("Rin");
        public override string[] ExperimentalText => [GetText("Rin_1"), GetText("Rin_2")];
        public override bool[] Experimental => [Config.Rin, Config.Rin_2];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<RinSkull>());
        }
        public override bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            if (Config.Satori && item.type == ModContent.ItemType<RinSkull>() && player.EnableEnhance<SatoriSlippers>())
                return false;

            return null;
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (Config.Satori && item.type == ModContent.ItemType<RinSkull>() && player.EnableEnhance<SatoriSlippers>())
            {
                if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                {
                    if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                        player.MP().ActivePassiveEnhance.Add(enhanceId);
                }
            }
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (Config.Satori && item.type == ModContent.ItemType<RinSkull>() && player.EnableEnhance<SatoriSlippers>())
            {
                if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                {
                    if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                        player.MP().ActivePassiveEnhance.Add(enhanceId);
                }
            }
        }
        public override void PlayerPostResetEffects(Player player)
        {
            if (!Config.Satori || !Config.PetInv || !player.EnableEnhance<SatoriSlippers>())
                return;

            foreach (Item item in player.miscEquips)
            {
                if (item.type == ModContent.ItemType<RinSkull>())
                {
                    if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                    {
                        if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                            player.MP().ActivePassiveEnhance.Add(enhanceId);
                    }
                    break;
                }
            }
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            if (!Config.Rin)
                return;

            if (player.MP().RinCD > 0)
            {
                player.MP().RinCD--;
                return;
            }

            if (player == Main.LocalPlayer)
            {
                List<NPC> targets = [];

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (!npc.friendly && npc.Center.Distance(player.Center) < 560)
                        targets.Add(npc);
                }

                if (targets.Count > 0)
                {
                    player.MP().RinCD = 30;

                    NPC target = Main.rand.Next(targets);
                    int dir = Main.rand.Next([-1, 1]);
                    Vector2 pos = target.position + new Vector2(target.width / 2f + Main.rand.NextFloat(40.00f, 100.00f) * -dir , -100 + Main.rand.NextFloat(-50.00f, -50.00f));
                    Vector2 vec = dir * Vector2.UnitX * 3f;
                    float randR = Main.rand.NextFloat(-3.140f, 3.140f);
                    Projectile.NewProjectile(player.GetSource_FromThis(), pos, vec, ModContent.ProjectileType<Souless>(), 0, 0, player.whoAmI, 0, randR);
                }
            }
        }
        public override void SystemPreUpdateGores()
        {
            if (!Main.LocalPlayer.EnableEnhance<RinSkull>())
                return;

            for (int i = 0; i < Main.maxGore; i++)
            {
                Gore gore = Main.gore[i];

                if (!gore.active || EnhanceSystem.GoreDamage[i] == 0)
                    continue;

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.dontTakeDamage || npc.friendly || (npc.aiStyle == 112 && !(npc.ai[2] <= 1f)) || !Main.LocalPlayer.CanNPCBeHitByPlayerOrPlayerProjectile(npc) || !Collision.CheckAABBvAABBCollision(gore.position, new Vector2(gore.Width, gore.Height), npc.position, npc.Size))
                        continue;

                    if (Config.Rin_2)
                        Projectile.NewProjectile(npc.GetSource_OnHurt(Main.LocalPlayer), gore.position + new Vector2(gore.Width, gore.Height), Vector2.Zero, ModContent.ProjectileType<CorpseExplosionTechnique>(), EnhanceSystem.GoreDamage[i], 0, Main.LocalPlayer.whoAmI);
                    else
                        Main.LocalPlayer.ApplyDamageToNPC(npc, EnhanceSystem.GoreDamage[i], 0, 1, false, damageVariation: true);

                    EnhanceSystem.GoreDamage[i] = 0;
                    gore.active = false;
                }
            }
        }
    }
}

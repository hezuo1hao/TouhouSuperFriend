using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Utsuho : BaseEnhance
    {
        public override string Text => GetText("Utsuho");
        public override string[] ExperimentalText => [GetText("Utsuho_1"), GetText("Utsuho_2"), GetText("Utsuho_3")];
        public override bool[] Experimental => [Config.Utsuho, Config.Utsuho_2, Config.Utsuho_3];
        int vaporizing = -1;
        public override void SystemPostSetupContent()
        {
            if (ModLoader.TryGetMod("Gensokyo", out var mod))
                vaporizing = mod.Find<ModBuff>("Debuff_Vaporizing").Type;
        }
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<UtsuhoEye>());
        }
        public override bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            if (Config.Satori && item.type == ModContent.ItemType<UtsuhoEye>() && player.EnableEnhance<SatoriSlippers>())
                return false;

            return null;
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (Config.Satori && item.type == ModContent.ItemType<UtsuhoEye>() && player.EnableEnhance<SatoriSlippers>())
            {
                if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                {
                    if (!player.MP().NowActivePassiveEnhance.Contains(enhanceId))
                        player.MP().NowActivePassiveEnhance.Add(enhanceId);
                }
            }
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (Config.Satori && item.type == ModContent.ItemType<UtsuhoEye>() && player.EnableEnhance<SatoriSlippers>())
            {
                if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                {
                    if (!player.MP().NowActivePassiveEnhance.Contains(enhanceId))
                        player.MP().NowActivePassiveEnhance.Add(enhanceId);
                }
            }
        }
        public override void PlayerPostUpdateBuffs(Player player)
        {
            if (vaporizing != -1)
            {
                player.buffImmune[vaporizing] = true;
                player.buffImmune[BuffID.OnFire] = true;
            }
        }
        public override void PlayerPostResetEffects(Player player)
        {
            if (!Config.Satori || !Config.PetInv || !player.EnableEnhance<SatoriSlippers>())
                return;

            foreach (Item item in player.miscEquips)
            {
                if (item.type == ModContent.ItemType<UtsuhoEye>())
                {
                    if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                    {
                        if (!player.MP().NowActivePassiveEnhance.Contains(enhanceId))
                            player.MP().NowActivePassiveEnhance.Add(enhanceId);
                    }
                    break;
                }
            }
        }
        public override void PlayerPostUpdate(Player player)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.dontTakeDamage || npc.friendly || (npc.aiStyle == 112 && !(npc.ai[2] <= 1f)) || !player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
                    continue;

                if (Config.Utsuho_3 && player == Main.LocalPlayer && TouhouPetsExModSystem.SynchronousTime % 120 == 67 && player.RollGoodLuck(6) == 0)
                    npc.AddBuff(Main.rand.Next([.. GEnhanceBuffs.FireDebuff]), 300);

                int disMax = 1000;

                if (Main.hardMode)
                    disMax = 1500;
                if (Config.Utsuho_2 && player.resistCold)
                    disMax *= 2;

                float dis = npc.Center.Distance(player.Center);

                if (Config.Utsuho)
                    dis = 0;

                if (dis < disMax)
                    npc.AddBuff(ModContent.BuffType<Melt>(), disMax - (int)dis, true);
            }
        }
    }
}

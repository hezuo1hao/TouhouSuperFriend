using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Junko : BaseEnhance
    {
        public override string Text => GetText("Junko");
        public override bool[] Experimental => [Config.Junko];
        public override string[] ExperimentalText => [GetText("Junko_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<JunkoMooncake>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += (player.statLifeMax2 - player.statLife) * .2f;
        }
        public override void PlayerModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += Math.Max(0, player.GetWeaponCrit(item) - 100);
        }
        public override void PlayerModifyHitNPCWithProjectile(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += Math.Max(0, proj.CritChance - 100) / 100f;
        }
    }
}

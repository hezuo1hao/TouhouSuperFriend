using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Meirin : BaseEnhance
    {
        public override string Text => GetText("Meirin");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override string[] ExperimentalText => [GetText("Meirin_1")];
        public override bool[] Experimental => [Config.Meirin];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MeirinPanda>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.GetJumpState<TouhouPetsExJump>().Enable();

            if (Config.Meirin)
            {
                player.MP().Earthquake = player.noFallDmg;
                player.noFallDmg = false;
            }
        }
        public override bool? PlayerFreeDodge(Player player, Player.HurtInfo info)
        {
            if (!Config.Meirin || info.DamageSource.SourceOtherIndex != 0)
                return null;

            if (player.MP().Earthquake)
            {
                // ÊÍ·ÅÒ»´Îaoe³å»÷²¨
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromAI(), player.MountedCenter, Vector2.Zero, ProjectileID.DD2OgreSmash, info.Damage, 0.1f, player.whoAmI);
                proj.friendly = true;
                proj.hostile = false;
                proj.localNPCHitCooldown = 10;
                proj.usesLocalNPCImmunity = true;
            }

            return true;
        }
    }
}

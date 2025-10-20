using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Yuyuko : BaseEnhance
    {
        public override string Text => GetText("Yuyuko");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<YuyukoFan>());
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.boss || target.life >= 5000 || Main.rand.Next(1000) >= 44 || player != Main.LocalPlayer)
                return;

            target.StrikeInstantKill();
            int rand = -1;
            if (LocalConfig.Yuyuko == YuyukoEffect.All)
                rand = Main.rand.Next(2);

            if (LocalConfig.Yuyuko == YuyukoEffect.Die || rand == 0)
                Projectile.NewProjectile(target.GetSource_Death(), target.Center, Vector2.Zero, ModContent.ProjectileType<Die>(), 0, 0, player.whoAmI, ai1: Main.rand.NextFloat(3.140f));
            else if (LocalConfig.Yuyuko == YuyukoEffect.Butterfly || rand == 1)
                Projectile.NewProjectile(target.GetSource_Death(), target.Center, Vector2.Zero, ModContent.ProjectileType<Butterfly>(), 0, 0, player.whoAmI, Main.rand.Next(2));

        }
    }
}

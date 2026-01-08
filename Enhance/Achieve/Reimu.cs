using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Reimu : BaseEnhance
    {
        public override string Text => GetText("Reimu");
        public override bool[] Experimental => [Config.Reimu];
        public override string[] ExperimentalText => [GetText("Reimu_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<ReimuYinyangOrb>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.wingTimeMax *= 2;

            if (NPC.downedEmpressOfLight)
            {
                player.empressBrooch = true;
                player.moveSpeed += 0.075f;

                if (player == Main.LocalPlayer && player.wingsLogic > 0 && player.velocity.Y < 0)
                    ModContent.GetInstance<FlyMyWings>().Condition.Complete();
            }
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (!Config.Reimu)
                return;

            if (Main.time == 0 && Main.dayTime)
                player.MP().ReimuCD = false;

            if (Main.LocalPlayer != player || player.velocity.Y == 0 || player.MP().ReimuCD)
                return;

            int type = ModContent.ProjectileType<DreamsAreBorn>();

            if (player.ownedProjectileCounts[type] > 0)
                return;

            Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, type, 0, 0, player.whoAmI);
        }
    }
}

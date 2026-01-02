using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Alice : BaseEnhance
    {
        public override string Text => GetText("Alice");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<AliceDoll>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.maxMinions += 1;
            player.maxTurrets += 1;

            if (player != Main.LocalPlayer)
                return;

            if (player.EnableEnhance<AliceOldDoll>())
                ModContent.GetInstance<ItsAlice>().Condition.Complete();
        }
        public override void PlayerKill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (player != Main.LocalPlayer)
                return;

            int i = 0;
            var doYouFeelFamiliar = ModContent.GetInstance<DoYouFeelFamiliar>();
            if (!doYouFeelFamiliar.IsCloneable && damageSource.SourceProjectileType == ProjectileID.CursedFlameHostile)
            {
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.owner == Main.myPlayer && proj.minion)
                        i++;
                }

                if (i == 3)
                    doYouFeelFamiliar.Condition.Complete();
            }
        }
    }
}

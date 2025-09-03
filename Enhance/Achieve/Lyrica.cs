using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Lyrica : BaseEnhance
    {
        public override string Text => GetText("Lyrica");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<LyricaKeyboard>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (player.MP().LyricaCD > 0)
            {
                player.MP().LyricaCD--;
                return;
            }

            if (player == Main.LocalPlayer)
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (!npc.friendly && npc.Center.Distance(player.Center) < 480)
                    {
                        player.MP().LyricaCD = 300;
                        float randY = Main.rand.NextFloat(-50.00f, -25.00f);
                        int dir = Main.rand.Next([-1, 1]);
                        for (int i = 1; i <= 5; i++)
                            Projectile.NewProjectileDirect(player.GetSource_FromThis($"Enhance_{nameof(Lyrica)}"), player.Center, Vector2.Zero, ModContent.ProjectileType<FantasyStaff>(), 1, 0.01f, player.whoAmI, ai1: i, ai2: randY).direction = dir;
                        break;
                    }
                }
        }
    }
}

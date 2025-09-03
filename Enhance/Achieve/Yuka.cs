using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Yuka : BaseEnhance
    {
        public override string Text => GetText("Yuka");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<YukaSunflower>());
        }
        public override void PlayerPreUpdate(Player player)
        {
            EnhancePlayers mp = player.MP();
            if (mp.FragrantAromaFillsTheAirCD == 0 && Framing.GetTileSafely(player.Center).TileType == TileID.Sunflower)
            {
                player.AddBuff(ModContent.BuffType<FragrantAromaFillsTheAir>(), 900);
                mp.FragrantAromaFillsTheAirCD = 1500;
                Projectile.NewProjectile(player.GetSource_Buff(player.FindBuffIndex(ModContent.BuffType<FragrantAromaFillsTheAir>())), player.Center, Vector2.Zero, ModContent.ProjectileType<YukaEffects>(), 0, 0, player.whoAmI);
            }

            if (mp.FragrantAromaFillsTheAirCD > 0)
            {
                mp.FragrantAromaFillsTheAirCD--;
            }
        }
    }
}

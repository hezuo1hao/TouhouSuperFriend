using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPets.Content.Projectiles.Pets;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Koakuma : BaseEnhance
    {
        public override string Text => GetText("Koakuma");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<KoakumaPower>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetAttackSpeed(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 5;
        }
        public override void TileDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (type == TileID.Books && Main.LocalPlayer.EnableEnhance<KoakumaPower>() && Framing.GetTileSafely(i, j).TileFrameX == 90)
            {
                if (!Main.gamePaused && Main.instance.IsActive && Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16), 16, 16, DustID.TreasureSparkle, 0f, 0f, 150, default, 0.3f);
                    dust.fadeIn = 1f;
                    dust.velocity *= 0.1f;
                    dust.noLight = true;
                }
            }
        }
    }
}

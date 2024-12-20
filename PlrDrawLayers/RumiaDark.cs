using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPetsEx.PlrDrawLayers
{
    public class RumiaDark : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => PlayerDrawLayers.AfterLastVanillaLayer;
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return !drawInfo.drawPlayer.dead && drawInfo.drawPlayer.MP().ActiveEnhance.Contains(ModContent.ItemType<RumiaRibbon>());
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                var position = drawInfo.Center.Floor() - Main.screenPosition;
                var tex = TextureAssets.Projectile[540].Value;
                drawInfo.DrawDataCache.Add(new DrawData(tex, position, null, Color.Black, 0f,
                    tex.Size() * 0.5f, 3f, SpriteEffects.None, 0));
            }
        }
    }
}

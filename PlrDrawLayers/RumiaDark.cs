using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
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
            return !Main.gameMenu && LocalConfig.Rumia && !drawInfo.drawPlayer.dead && drawInfo.drawPlayer.EnableEnhance<RumiaRibbon>();
        }
        public static Asset<Texture2D> tex = ModContent.Request<Texture2D>("TouhouPetsEx/Extra/NoBlackPointLight", AssetRequestMode.ImmediateLoad);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow == 0f)
            {
                var position = drawInfo.Center.Floor() - Main.screenPosition;
                drawInfo.DrawDataCache.Add(new DrawData(tex.Value, position, null, Color.Black, 0f,
                    tex.Size() * 0.5f, 2f, SpriteEffects.None, 0));
            }
        }
        public override void Unload()
        {
            tex = null;
        }
    }
}

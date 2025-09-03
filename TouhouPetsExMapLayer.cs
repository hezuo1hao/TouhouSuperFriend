using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPetsEx
{
    public class TouhouPetsExMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (text != "" || !Main.mapFullscreen || (Main.CurrentFrameFlags.AnyActiveBossNPC && !Config.Yukari) || Main.LocalPlayer.MP().YukariCD > 0 || !Main.LocalPlayer.EnableEnhance<YukarisItem>())
                return;

            text = GetText("Tp");
        }
    }
}
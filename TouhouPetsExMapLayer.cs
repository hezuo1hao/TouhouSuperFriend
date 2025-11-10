using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.ComponentModel;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;

namespace TouhouPetsEx
{
    public class TouhouPetsExMapLayer : ModMapLayer
    {
        private static Texture2D tex;
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            tex ??= ModContent.Request<Texture2D>("TouhouPetsEx/Extra/Point", AssetRequestMode.ImmediateLoad).Value;

            if (text == "" && Main.mapFullscreen && (!Main.CurrentFrameFlags.AnyActiveBossNPC || Config.Yukari) && Main.LocalPlayer.MP().YukariCD == 0 && Main.LocalPlayer.EnableEnhance<YukarisItem>())
                text = GetText("Tp");

            if (Main.LocalPlayer.EnableEnhance<StarSapphire>() || Main.LocalPlayer.EnableEnhance<LightsJewels>())
            {
                int i = 0;
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.friendly || NPCID.Sets.CountsAsCritter[npc.type])
                        continue;

                    context.Draw(tex, npc.Center / 16, Color.Red, new SpriteFrame(1, 1, 0, 0), 2f, 2f, Alignment.Center);

                    if (npc.Center.Distance(Main.LocalPlayer.Center) < 1200)
                        i++;
                }

                if (i >= 100)
                    ModContent.GetInstance<Nightmare>().Condition.Complete();
            }
        }
    }
}
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using Terraria.ID;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sizuha : BaseEnhance
    {
        public override string Text => GetText("Sizuha");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SizuhaBrush>());
        }
        public override void SystemPostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.EnableEnhance<SizuhaBrush>())
                {
                    Tile tile = Framing.GetTileSafely(player.Center);
                    if (TileID.Sets.IsShakeable[tile.TileType])
                        WorldGen.ShakeTree((int)player.Center.X / 16, (int)player.Center.Y / 16);
                }
            }
        }
    }
}

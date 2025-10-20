using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Kogasa : BaseEnhance
    {
        public override string Text => GetText("Kogasa");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<KogasaUmbrella>());
        }
        int time;
        public override void ItemHoldItem(Item item, Player player)
        {
            if (time == 120)
            {
                time = 0;
                item.ResetPrefix();
                item.Prefix(-2);
                item.position = Main.LocalPlayer.Center;
                PopupText.NewText(PopupTextContext.ItemReforge, item, item.stack, noStack: true);
                SoundEngine.PlaySound(SoundID.Item37);
            }

            if (time == -120)
            {
                time = 0;

                int i = 0;
                item.ResetPrefix();
                item.Prefix(-2);
                while (PrefixID.Sets.ReducedNaturalChance[item.prefix])
                {
                    if (i >= 100)
                        break;

                    item.ResetPrefix();
                    item.Prefix(-2);
                    i++;
                }
                item.position = Main.LocalPlayer.Center;
                PopupText.NewText(PopupTextContext.ItemReforge, item, item.stack, noStack: true);
                SoundEngine.PlaySound(SoundID.Item37);
            }

            if (!player.controlUseTile || !item.Prefix(-3))
            {
                time = 0;
                return;
            }

            Tile tile = Framing.GetTileSafely(Main.MouseWorld);
            if (tile.TileType is TileID.Anvils)
            {
                time++;
                return;
            }

            if (tile.TileType == TileID.MythrilAnvil)
            {
                time--;
                return;
            }
        }
        public override bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            if (!player.controlUseTile|| !item.Prefix(-3))
                return null;

            Tile tile = Framing.GetTileSafely(Main.MouseWorld);
            if (tile.TileType is TileID.Anvils or TileID.MythrilAnvil)
                return false;

            return null;
        }
    }
}

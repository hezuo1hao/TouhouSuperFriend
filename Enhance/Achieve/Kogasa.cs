using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
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
            if (item.type == ModContent.ItemType<KogasaUmbrella>())
            {
                player.fallStart = (int)(player.position.Y / 16f);
                if (player.gravDir == -1f && player.velocity.Y < -2f && !player.controlDown)
                {
                    player.velocity.Y = -2f;
                }
                else if (player.velocity.Y > 2f && !player.controlDown)
                {
                    player.velocity.Y = 2f;
                }

                if (player == Main.LocalPlayer)
                {
                    var cavesCliffs = ModContent.GetInstance<CavesCliffs>();

                    if (player.ZoneSkyHeight)
                        cavesCliffs.Condition.Value = 1;

                    if (player.velocity.Y == 0)
                        cavesCliffs.Condition.Value = 0;

                    if (player.ZoneUnderworldHeight && cavesCliffs.Condition.Value == 1)
                        cavesCliffs.Condition.Complete();
                }

            }
            else if (player == Main.LocalPlayer && !ModContent.GetInstance<CavesCliffs>().Condition.IsCompleted)
                ModContent.GetInstance<CavesCliffs>().Condition.Value = 0;


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
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<KogasaUmbrella>())
                tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, new("ExTooltip", Language.GetTextValue("ItemTooltip.TragicUmbrella")));
        }
    }
}

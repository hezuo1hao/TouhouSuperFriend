using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
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
                mp.FragrantAromaFillsTheAirCD--;

            if (mp.YukaCD > 0)
                mp.YukaCD--;
        }
        public override void ItemGrabRange(Item item, Player player, ref int grabRange)
        {
            if (Config.Yuka && item.IsACoin)
                grabRange += 144;
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (!Config.Yuka || player.MP().YukaCD > 0)
                return;

            int i = 0;
            for (int x = -9; x < 9; x++)
                for (int y = -9; y < 9; y++)
                {
                    int posX = (int)player.Center.X / 16 + x;
                    int posY = (int)player.Center.Y / 16 + y;
                    Tile tile = Framing.GetTileSafely(posX, posY);
                    if (tile.type == TileID.Sunflower && tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Item.NewItem(player.GetSource_FromThis(), new Rectangle(posX * 16, posY * 16, 32, 32), ItemID.CopperCoin, 6);

                        player.MP().YukaCD = 60;
                        i++;
                    }
                }

            if (Main.netMode != NetmodeID.Server)
            {
                var autumnGodRanch = ModContent.GetInstance<AutumnGodRanch>();

                if (autumnGodRanch.Condition.Value < i)
                    autumnGodRanch.Condition.Value = i;

                if (autumnGodRanch.Condition.Value >= AutumnGodRanch.Max)
                    autumnGodRanch.Condition.Complete();
            }
        }
    }
}

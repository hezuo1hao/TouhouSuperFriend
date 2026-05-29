using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Items
{
    [AutoloadEquip(EquipType.Wings)]
    public class AirWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 8f, 2f, true, 12f, 12f);
        }
        public override void SetDefaults()
        {
            Item.width = 1;
            Item.height = 1;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
            Item.accessory = true;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.75f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 2.5f;
            constantAscend = 0.125f;

            if (player.TryingToHoverDown && player.controlJump && player.wingTime > 0f && !player.merman)
            {
                float num71 = 0.9f;

                player.velocity.Y *= num71;
                if (player.velocity.Y > -2f && player.velocity.Y < 1f)
                    player.velocity.Y = 1E-05f;

                player.velocity.Y *= 0.92f;
                if (player.velocity.Y > -2f && player.velocity.Y < 1f)
                    player.velocity.Y = 1E-05f;
            }
            else if (player.TryingToHoverUp)
            {
                player.velocity.Y -= 0.4f * player.gravDir;
                if (player.gravDir == 1f)
                {
                    if (player.velocity.Y > 0f)
                        player.velocity.Y -= 1f;
                    else if (player.velocity.Y > 0f - Player.jumpSpeed)
                        player.velocity.Y -= 0.2f;

                    if (player.velocity.Y < (0f - Player.jumpSpeed) * 3f)
                        player.velocity.Y = (0f - Player.jumpSpeed) * 3f;
                }
                else
                {
                    if (player.velocity.Y < 0f)
                        player.velocity.Y += 1f;
                    else if (player.velocity.Y < Player.jumpSpeed)
                        player.velocity.Y += 0.2f;

                    if (player.velocity.Y > Player.jumpSpeed * 3f)
                        player.velocity.Y = Player.jumpSpeed * 3f;
                }
            }

            if (player.TryingToHoverDown && !player.controlJump && player.velocity.Y != 0f)
                player.velocity.Y += 0.4f;
        }
    }
}
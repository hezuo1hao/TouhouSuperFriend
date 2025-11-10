using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Items
{
    public class ABurntDoll : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 48;
            Item.value = 0;
            Item.rare = ItemRarityID.Yellow;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.UseSound = SoundID.Item92;
            Item.consumable = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override bool CanUseItem(Player player)
        {
            if (player.MP().ABurntDoll)
                return false;

            player.MP().ABurntDoll = true;
            EnhancePlayers.AwardPlayerSync(Mod, -1, player.whoAmI);

            return true;
        }
        public override void UpdateInventory(Player player)
        {
            if (player == Main.LocalPlayer)
            {
                var witchTrial = ModContent.GetInstance<WitchTrial>();

                witchTrial.Condition.Value += Item.stack;

                if (witchTrial.Condition.Value >= WitchTrial.Max)
                    witchTrial.Condition.Complete();
            }
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (!Collision.SolidCollision(Item.position + Vector2.UnitY * 30f, Item.width, Item.height))
                return;

            int y = 1;
            for (; y < 14; y++)
                if (!Collision.SolidCollision(Item.position + Vector2.UnitY * (30f - y), Item.width, Item.height))
                    break;

            maxFallSpeed = 7 - y / 14f * 5;

            for (; y < 30; y++)
                if (!Collision.SolidCollision(Item.position + Vector2.UnitY * (30f - y), Item.width, Item.height))
                    break;

            if (y > 15)
                gravity = -0.01f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<NewlyMadeDoll>(), 1);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 42);
            recipe.AddIngredient(ItemID.Seedling, 1);
            recipe.AddTile(TileID.Campfire);
            recipe.Register();
        }
    }
}
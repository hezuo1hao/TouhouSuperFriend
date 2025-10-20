using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Items
{
    public class NewlyMadeDoll : ModItem
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
            Item.rare = ItemRarityID.LightPurple;
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
            if (player.MP().NewlyMadeDoll)
                return false;

            player.MP().NewlyMadeDoll = true;
            EnhancePlayers.AwardPlayerSync(Mod, -1, player.whoAmI);

            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LifeCrystal, 20);
            recipe.AddIngredient(ItemID.ManaCrystal, 10);
            recipe.AddIngredient(ItemID.DirtBlock, 25);
            recipe.AddIngredient(ItemID.MudBlock, 50);
            recipe.AddIngredient(ItemID.AshBlock, 25);
            recipe.AddIngredient(ItemID.GrassSeeds, 25);
            recipe.AddIngredient(ItemID.JungleGrassSeeds, 25);
            recipe.AddIngredient(ItemID.MushroomGrassSeeds, 25);
            recipe.AddIngredient(ItemID.AshGrassSeeds, 25);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
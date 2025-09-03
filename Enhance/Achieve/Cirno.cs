using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Cirno : BaseEnhance
    {
        public override string Text => GetText("Cirno");
        public override bool[] Experimental => [Config.Cirno];
        public override string[] ExperimentalText => [GetText("Cirno_1")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<CirnoIceShard>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (TouhouPetsExModSystem.SynchronousTime % 60 == 17)
                player.QuickSpawnItem(player.GetSource_Misc(nameof(TouhouPetsEx) + "_" + nameof(Cirno)), ItemID.IceBlock);
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (item.type == ItemID.IceBlock)
            {
                if (player.EnableEnhance<CirnoIceShard>())
                {
                    item.useAnimation = item.useTime = 20;
                    item.createTile = -1;
                    item.useStyle = ItemUseStyleID.Swing;
                    item.UseSound = SoundID.Item1;
                    item.consumable = true;
                    item.noUseGraphic = true;
                    item.noMelee = true;
                    item.shoot = ModContent.ProjectileType<CirnoIce>();
                    item.shootSpeed = 9;
                    item.damage = 8;
                    item.knockBack = 5;
                    item.DamageType = DamageClass.Ranged;
                }
                else
                {
                    Item item1 = ContentSamples.ItemsByType[item.type];
                    item.useTime = item1.useTime;
                    item.useAnimation = item1.useAnimation;
                    item.createTile = item1.createTile;
                    item.useStyle = item1.useStyle;
                    item.UseSound = item1.UseSound;
                    item.consumable = item1.consumable;
                    item.noUseGraphic = item1.noUseGraphic;
                    item.noMelee = item1.noMelee;
                    item.shoot = item1.shoot;
                    item.shootSpeed = item1.shootSpeed;
                    item.damage = item1.damage;
                    item.knockBack = item1.knockBack;
                    item.DamageType = item1.DamageType;
                }
            }
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (item.type == ItemID.IceBlock)
            {
                if (player.EnableEnhance<CirnoIceShard>())
                {
                    item.useAnimation = item.useTime = 20;
                    item.createTile = -1;
                    item.useStyle = ItemUseStyleID.Swing;
                    item.UseSound = SoundID.Item1;
                    item.consumable = true;
                    item.noUseGraphic = true;
                    item.noMelee = true;
                    item.shoot = ModContent.ProjectileType<CirnoIce>();
                    item.shootSpeed = 9;
                    item.damage = 8;
                    item.knockBack = 5;
                    item.DamageType = DamageClass.Ranged;
                }
                else
                {
                    Item item1 = ContentSamples.ItemsByType[item.type];
                    item.useTime = item1.useTime;
                    item.useAnimation = item1.useAnimation;
                    item.createTile = item1.createTile;
                    item.useStyle = item1.useStyle;
                    item.UseSound = item1.UseSound;
                    item.consumable = item1.consumable;
                    item.noUseGraphic = item1.noUseGraphic;
                    item.noMelee = item1.noMelee;
                    item.shoot = item1.shoot;
                    item.shootSpeed = item1.shootSpeed;
                    item.damage = item1.damage;
                    item.knockBack = item1.knockBack;
                    item.DamageType = item1.DamageType;
                }
            }
        }
    }
}

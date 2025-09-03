using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
    public class Letty : BaseEnhance
    {
        public override string Text => GetText("Letty");
        public override bool[] Experimental => [Config.Letty, Config.Letty_2];
        public override string[] ExperimentalText => [GetText("Letty_1"), GetText("Letty_2")];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<LettyGlobe>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            var mp = player.MP();
            
            if (mp.LettyCD > 0)
                mp.LettyCD--;
        }
        public override bool? ItemUseItem(Item item, Player player)
        {
            if (player == Main.LocalPlayer && item.damage > 0 && player.MP().LettyCD == 0)
            {
                List<int> blackList = [];

                foreach (string fullName in Config.Letty_2_1 ?? [])
                {
                    string[] parts = fullName.Split('/');
                    if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModProjectile proj))
                        blackList.Add(proj.Type);
                }

                int projType = Main.rand.Next(Config.Letty_2 ? [.. TouhouPetsEx.ColdProjAll.Where(type => type < ProjectileID.Count || blackList.Contains(type) != true)] : TouhouPetsEx.ColdProjVanilla);

                for (int i = -1; i <= 1; i++)
                    Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(item, 0), player.MountedCenter + Vector2.UnitY,
                        Vector2.Normalize(Main.MouseWorld - player.MountedCenter).RotatedBy(i * MathHelper.TwoPi / 36f) * (item.shootSpeed > 1 ? item.shootSpeed : 11f),
                        projType, player.GetWeaponDamage(item) / 10, 0.1f, player.whoAmI, ai2: (projType == ModContent.ProjectileType<CirnoIce>() ? 20 : 0));

                player.MP().LettyCD = 120;
            }

            return null;
        }
        public override void PlayerModifyHitNPCWithProjectile(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Config.Letty && proj.coldDamage)
                modifiers.FinalDamage *= 1.1f;
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Raiko : BaseEnhance
    {
        public override string Text => GetText("Raiko");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<RaikoDrum>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (player != Main.LocalPlayer || !Main.IsItStorming)
                return;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.dontTakeDamage || npc.friendly || (npc.aiStyle == 112 && !(npc.ai[2] <= 1f)) || !Main.LocalPlayer.CanNPCBeHitByPlayerOrPlayerProjectile(npc) || !Main.rand.NextBool(2400))
                    continue;

                int damage = 100;

                if (!npc.boss)
                    damage += (int)Math.Ceiling(npc.lifeMax / 10f);

                Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Lightning>(), damage, 0, player.whoAmI);

                if (LocalConfig.Raiko)
                {
                    PunchCameraModifier modifier4 = new(player.MountedCenter, new(0, Main.rand.NextFloat(-1.00f, 1.00f)), 12f, 15f, 15, 200f, "Ãû×Ö");
                    Main.instance.CameraModifiers.Add(modifier4);
                }
            }
        }
    }
}

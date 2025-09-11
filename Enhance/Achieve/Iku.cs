using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Iku : BaseEnhance
    {
        public override string Text => GetText("Iku");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<IkuOarfish>());
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && Main.rand.NextBool(5))
            {
                float maxDistance = 304;
                NPC target2 = null;
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc != target)

                    if (npc.CanBeChasedBy(player, false) && Collision.CanHit(target.position, target.width, target.height, npc.position, npc.width, npc.height))
                    {
                        float distance = Vector2.Distance(npc.Center, target.Center);
                        if (distance <= maxDistance)
                        {
                            maxDistance = distance;
                            target2 = npc;
                        }
                    }
                }

                if (target2 != null)
                    Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<LightningStormProj>(), (int)Math.Max(damageDone * 0.19f, 1), 0.5f, player.whoAmI, target2.whoAmI, target.whoAmI, -1);
            }
        }
    }
}

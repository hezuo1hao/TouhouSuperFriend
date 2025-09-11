using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPetsEx.Buffs;
using static Terraria.ModLoader.BackupIO;

namespace TouhouPetsEx.Projectiles
{
    public class LightningStormProj : ModProjectile
    {
        public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }
        List<Vector4> imitate = [];
        List<Vector4> imitate2 = [];
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];

            for (int i = 0; i < 100; i++)
            {

                if (i == 0)
                {
                    Vector2 v = Vector2.Normalize(npc.Center - Projectile.Center).RotatedByRandom(MathHelper.Lerp(1.57f, 0, i / 100f));
                    imitate.Add(new(Projectile.Center, Projectile.Center.X + v.X * 50, Projectile.Center.Y + v.Y * 50));
                }
                else
                {
                    Vector2 v = Vector2.Normalize(npc.Center - imitate[i - 1].ZW()).RotatedByRandom(MathHelper.Lerp(1.57f, 0, i / 100f));
                    imitate.Add(new(imitate[i - 1].ZW(), imitate[i - 1].Z + v.X * 50, imitate[i - 1].W + v.Y * 50));
                }

                if (Main.rand.NextBool(2))
                {
                    Vector2 pos = Vector2.Lerp(imitate[i].XY(), imitate[i].ZW(), Main.rand.NextFloat(1.00f));
                    Vector2 v = Vector2.Normalize(imitate[i].ZW() - imitate[i].XY()).RotatedByRandom(1.57f);
                    imitate2.Add(new(pos, pos.X + v.X * 50, pos.Y + v.Y * 50));
                }

                if (Collision.CheckAABBvLineCollision(npc.position, npc.Size, imitate[i].XY(), imitate[i].ZW()) || i == 99)
                {
                    imitate[i] = new(imitate[i].XY(), npc.Center.X, npc.Center.Y);
                    break;
                }
            }

            foreach (Vector4 vector4 in imitate2)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(Vector2.Lerp(vector4.XY(), vector4.ZW(), i / 30f), 1, 1, DustID.GoldFlame, Scale: 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0.25f;
                }
            }

            foreach (Vector4 vector4 in imitate)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(Vector2.Lerp(vector4.XY(), vector4.ZW(), i / 30f), 1, 1, DustID.GoldFlame, Scale: 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0.25f;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            bool canHit = false;
            foreach (Vector4 vector4 in imitate)
            {
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), vector4.XY(), vector4.ZW()))
                    canHit = true;
            }

            foreach (Vector4 vector4 in imitate2)
            {
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), vector4.XY(), vector4.ZW()))
                    canHit = true;
            }
            return canHit;
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            foreach (Vector4 vector4 in imitate)
            {
                Utils.PlotTileLine(vector4.XY(), vector4.ZW(), (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
            }

            foreach (Vector4 vector4 in imitate2)
            {
                Utils.PlotTileLine(vector4.XY(), vector4.ZW(), (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<LeiZhe>(), 60);

            if (Projectile.ai[2] != -1)
                return;

            float maxDistance = 304;
            NPC target2 = null;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc != target && npc.whoAmI != Projectile.ai[1])

                    if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(target.position, target.width, target.height, npc.position, npc.width, npc.height))
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
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<LightningStormProj>(), Projectile.damage, 0.5f, Projectile.owner, target2.whoAmI, target.whoAmI, Projectile.ai[1]);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return (target.whoAmI == Projectile.ai[1] || target.whoAmI == Projectile.ai[2]) ? false : null;
        }
    }
}
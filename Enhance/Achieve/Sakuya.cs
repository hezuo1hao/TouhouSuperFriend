using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sakuya : BaseEnhance
    {
        public override string Text => GetText("Sakuya");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SakuyaWatch>());
        }
        public override void PlayerResetEffectsAlways(Player player)
        {
            if ((!player.EnableEnhance<SakuyaWatch>() || !LocalConfig.Sakuya) && player.MP().frameSkipMode != null)
            {
                Main.FrameSkipMode = (Terraria.Enums.FrameSkipMode)player.MP().frameSkipMode;
                player.MP().frameSkipMode = null;
            }
        }
        public override void PlayerPreUpdate(Player player)
        {
            if (!LocalConfig.Sakuya)
                return;

            if (player.MP().frameSkipMode == null)
                player.MP().frameSkipMode = Main.FrameSkipMode;

            Main.FrameSkipMode = Terraria.Enums.FrameSkipMode.Off;
            Main.UpdateTimeAccumulator -= Main.gameTimeCache.ElapsedGameTime.TotalSeconds / 1.67f;
        }
        public override void PlayerModifyHurt(Player player, ref Player.HurtModifiers modifiers)
        {
            if (Config.Sakuya && modifiers.DamageSource.TryGetCausingEntity(out var entity) && player.MP().SakuyaCD == 0)
                modifiers.ModifyHurtInfo += Modifiers_ModifyHurtInfo;
        }

        private void Modifiers_ModifyHurtInfo(ref Player.HurtInfo info)
        {
            info.Dodgeable = true;
        }

        public override bool? PlayerFreeDodge(Player player, Player.HurtInfo info)
        {
            if (Config.Sakuya && info.DamageSource.TryGetCausingEntity(out var entity) && player.MP().SakuyaCD == 0)
            {
                if (entity is NPC)
                    player.MP().SakuyaCD = 9600;
                else
                    player.MP().SakuyaCD = 4800;

                Projectile.NewProjectile(player.GetSource_OnHurt(info.DamageSource), player.Center, Vector2.Zero, ModContent.ProjectileType<PerfectMaid>(), 0, 0, player.whoAmI, ai1: info.DamageSource.SourceNPCIndex, ai2: (int)Math.Ceiling(info.ReflectionDamage() / 5f));

                float[] samples = new float[2];
                Vector2 vec = entity.velocity == Vector2.Zero ? Vector2.One * (player.velocity.X > 0 ? 1 : -1) : Vector2.Normalize(entity.velocity);
                vec *= -1;
                Collision.LaserScan(player.Center, vec, player.height, entity.Size.Length() + 250, samples);
                float maxDis = (samples[0] + samples[1]) * 0.5f;
                int time = player.longInvince ? 360 : 240;

                player.Center += vec * maxDis;
                player.immune = true;
                player.immuneTime += time;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += time;
                }

                return true;
            }

            return null;
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (Config.Sakuya)
            {
                if (player.MP().SakuyaCD > 0)
                    player.MP().SakuyaCD--;

                if (player.MP().SakuyaCD == 1)
                    SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_cardget"), player.Center);
            }
        }
        public override void PlayerKill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Config.Sakuya)
                player.MP().SakuyaCD = 0;
        }
        public override bool? NPCPreAI(NPC npc)
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<PerfectMaid>() && proj.ai[1] == npc.whoAmI)
                {
                    npc.velocity = Vector2.Zero;
                    return false;
                }
            }

            return null;
        }
    }
}

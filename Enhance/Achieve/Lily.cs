using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Lily : BaseEnhance
    {
        public override string Text => GetText("Lily");
        public override string[] ExperimentalText => [GetText("Lily_1")];
        public override bool[] Experimental => [Config.Lily];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<LilyOneUp>());
        }
        public override void PlayerPreUpdateBuffsAlways(Player player)
        {
            if (!player.HasBuff<LilyCD>())
                return;

            if (player.buffTime[player.FindBuffIndex(ModContent.BuffType<LilyCD>())] <= 5)
                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_extend") with { SoundLimitBehavior = SoundLimitBehavior.IgnoreNew}, player.Center);
        }
        public override void PlayerUpdateEquips(Player player)
        {
            if (Config.Lily)
                player.statLifeMax2 += 80;
        }
        public override bool? PlayerPreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (!player.HasBuff<LilyCD>())
            {
                int time = player.longInvince ? 600 : 360;

                if (player.EnableEnhance<MokuMatch>())
                    player.statLife = 100;
                else
                    player.statLife = 1;

                foreach (int buffType in player.buffType)
                {
                    if ((Main.debuff[buffType] && !BuffID.Sets.NurseCannotRemoveDebuff[buffType]) || buffType == ModContent.BuffType<DaiyouseiCD>())
                        player.ClearBuff(buffType);
                }
                player.breath = player.breathMax;
                player.lifeRegenTime += 3600;
                player.immune = true;
                player.immuneTime += time;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += time;
                }
                player.AddBuff(ModContent.BuffType<LilyCD>(), 10800);
                Projectile.NewProjectile(player.GetSource_Death(), player.Center, Vector2.Zero, ModContent.ProjectileType<LilyDodgeEffects>(), 0, 0, player.whoAmI);

                if (player.EnableAllYousei() && player == Main.LocalPlayer)
                {
                    var touhouFairyKnockout = ModContent.GetInstance<TouhouFairyKnockout>();

                    if (touhouFairyKnockout.Condition.Value == 0)
                        touhouFairyKnockout.Condition.Value = 2;
                    else if (touhouFairyKnockout.Condition.Value == 1)
                        touhouFairyKnockout.Condition.Complete();
                }
                return false;
            }

            return null;
        }
    }
}

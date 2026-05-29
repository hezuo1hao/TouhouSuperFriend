using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class YukariAndRanAndChen : BaseEnhance
    {
        public override string Text => GetText("YukariAndRanAndChen");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override string[] ExperimentalText => [GetText("YukariAndRanAndChen_1")];
        public override bool[] Experimental => [Config.Yukari];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<YukarisItem>());
        }

        public override void PlayerResetEffects(Player player)
        {
            if (player.MP().YukariCD > 0)
                player.MP().YukariCD--;
        }
        public override void PlayerPostUpdateAlways(Player player)
        {
            if (player.immune || !player.HasBuff<ChenCD>())
                return;

            player.Kaguya(player.FindBuffIndex(ModContent.BuffType<ChenCD>()));
        }
        public override void PlayerPostUpdateBuffs(Player player)
        {
            player.statManaMax2 += 20;
        }
        public override void PlayerModifyHurt(Player player, ref Player.HurtModifiers modifiers)
        {
            player.MP().ChenDodge = false;

            if ((!player.HasBuff<ChenCD>() || player.buffTime[player.FindBuffIndex(ModContent.BuffType<ChenCD>())] < 1500) && player.RollGoodLuck(100) < 10)
            {
                player.MP().ChenDodge = true;
                modifiers.ModifyHurtInfo += Modifiers_ModifyHurtInfo;
            }
        }

        private void Modifiers_ModifyHurtInfo(ref Player.HurtInfo info)
        {
            info.Dodgeable = true;
        }

        public override bool? PlayerFreeDodge(Player player, Player.HurtInfo info)
        {
            if (player.MP().ChenDodge)
            {
                player.MP().ChenDodge = false;
                int time = (1800 - (player.HasBuff<ChenCD>() ? player.buffTime[player.FindBuffIndex(ModContent.BuffType<ChenCD>())] : 0)) / 5;
                player.AddBuff(ModContent.BuffType<ChenCD>(), player.longInvince ? 1800 : 1440);
                player.immune = true;
                player.immuneTime += time;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] += time;
                }

                Projectile.NewProjectile(player.GetSource_Misc("Dodge"), player.Center, Vector2.Zero, ModContent.ProjectileType<Sandevistan>(), 0, 0, player.whoAmI, time);
                return true;
            }

            return null;
        }
        public static int count;
        public static void DrawPlayer(Camera camera, Player player)
        {
            var advancedShadows = player.MP().OldPlayer;
            var a = player.immuneAlpha;
            var b = player.balloon;
            var c = player.bodyFrame.Y;
            var d = player.carpetFrame;
            var e = player.legFrame.Y;
            var f = player.headFrame.Y;
            var g = player.wings;
            var z = player.direction;
            player.immuneAlpha = 0;
            player.balloon = -1;

            for (int i = advancedShadows.Count - 1; i > 0; i--)
            {
                count = i;
                NewEntityShadowInfo advancedShadow = advancedShadows[i];
                player.bodyFrame.Y = player.bodyFrame.Height * advancedShadow.BodyFrameIndex;
                player.carpetFrame = advancedShadow.CarpetFrame;
                player.legFrame.Y = advancedShadow.LegFrameY;
                player.headFrame.Y = advancedShadow.HeadFrameY;
                player.wings = 0;
                player.direction = advancedShadow.Direction;
                Main.PlayerRenderer.DrawPlayer(camera, player, advancedShadow.Position, advancedShadow.Rotation, advancedShadow.Origin, 0.1f);
            }
            count = 0;
            player.immuneAlpha = a;
            player.balloon = b;
            player.bodyFrame.Y = c;
            player.carpetFrame = d;
            player.legFrame.Y = e;
            player.headFrame.Y = f;
            player.wings = g;
            player.direction = z;
        }
    }
}

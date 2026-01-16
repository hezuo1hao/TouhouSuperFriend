using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPets.Content.Projectiles.Pets;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Koishi : BaseEnhance
    {
        public override string Text => GetText("Koishi");
        public override string[] ExperimentalText => [GetText("Koishi_1"), GetText("Koishi_2"), GetText("Koishi_3")];
        public override bool[] Experimental => [Config.Koishi, Config.Koishi_2, Config.Koishi_3];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<KoishiTelephone>());
        }
        public override bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            if (Config.Satori && item.type == ModContent.ItemType<KoishiTelephone>() && player.EnableEnhance<SatoriSlippers>())
                return false;

            return null;
        }
        public override void ItemHoldItem(Item item, Player player)
        {
            if (Config.Satori && item.type == ModContent.ItemType<KoishiTelephone>() && player.EnableEnhance<SatoriSlippers>())
            {
                if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                {
                    if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                        player.MP().ActivePassiveEnhance.Add(enhanceId);
                }
            }
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (Config.Satori && item.type == ModContent.ItemType<KoishiTelephone>() && player.EnableEnhance<SatoriSlippers>())
            {
                if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                {
                    if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                        player.MP().ActivePassiveEnhance.Add(enhanceId);
                }
            }
        }
        public override void PlayerPostResetEffects(Player player)
        {
            if (player.MP().Popularity > 0)
                player.MP().Popularity -= 0.005f;

            if (!Config.Satori || !Config.PetInv || !player.EnableEnhance<SatoriSlippers>())
                return;

            foreach (Item item in player.miscEquips)
            {
                if (item.type == ModContent.ItemType<KoishiTelephone>())
                {
                    if (EnhanceRegistry.TryGetEnhanceId(item.type, out EnhancementId enhanceId))
                    {
                        if (!player.MP().ActivePassiveEnhance.Contains(enhanceId))
                            player.MP().ActivePassiveEnhance.Add(enhanceId);
                    }
                    break;
                }
            }
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.aggro -= 10000;

            if (Config.Koishi_2 && player.armor[0].defense > 0)
            {
                player.statDefense += 10;
                player.endurance += 0.16f;

                if (player == Main.LocalPlayer && player.armor[0].type == ItemID.HallowedHood)
                    ModContent.GetInstance<TheBlueGuy>().Condition.Complete();
            }

            if (Config.Koishi_3 && player == Main.LocalPlayer && TouhouPetsExModSystem.SynchronousTime % 60 == 41)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.Distance(player.Center) < 1000)
                        player.MP().Popularity += npc.lifeMax / 2500f;
                }
            }

            if (player.MP().Popularity >= 100)
            {
                if (Config.Koishi)
                    player.MP().Popularity = 100;
                else if (!player.HasBuff(ModContent.BuffType<PopularityExplosion>()) || player.buffTime[player.FindBuffIndex(ModContent.BuffType<PopularityExplosion>())] < 180)
                {
                    player.MP().Popularity = 0;
                    player.AddBuff(ModContent.BuffType<PopularityExplosion>(), 900);
                    Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<PopularityExplosionEffect>(), 0, 0, player.whoAmI);
                }
            }
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.MP().Popularity += damageDone / 10f;
        }
        public override void PlayerPostHurt(Player player, Player.HurtInfo info)
        {
            int time = 30;
            if (player.aggro < -10000)
                time += (int)Math.Ceiling((Math.Abs(player.aggro) - 10000) / 33f);

            player.immuneTime += time;
            for (int i = 0; i < player.hurtCooldowns.Length; i++)
            {
                if (player.hurtCooldowns[0] > 0)
                player.hurtCooldowns[i] += time;
            }
        }
        public override bool? ItemPreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Config.Koishi_2 || item.defense == 0 || item.headSlot == -1 || !Main.LocalPlayer.EnableEnhance<KoishiTelephone>())
                return null;

            SpriteBatch sb = spriteBatch;
            Effect effect = TouhouPetsEx.TransformShader;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.CurrentTechnique.Passes["EnchantedPass"].Apply();
            Main.instance.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("TouhouPetsEx/Extra/Enchanted", AssetRequestMode.ImmediateLoad).Value; // 传入调色板

            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
            sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, effect, Main.UIScaleMatrix);

            return null;
        }
        public override void ItemPostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Config.Koishi_2 || item.defense == 0 || item.headSlot == -1 || !Main.LocalPlayer.EnableEnhance<KoishiTelephone>())
                return;

            SpriteBatch sb = spriteBatch;
            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
            sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }
        public override void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!Config.Koishi_2 || item.defense == 0 || item.headSlot == -1 || !Main.LocalPlayer.EnableEnhance<KoishiTelephone>())
                return;

            tooltips[0].Text = $"{Language.GetTextValue("Prefix.Warding")} {Language.GetTextValue("Prefix.Warding")} {Language.GetTextValue("Prefix.Guarding")} " + tooltips[0].Text;

            int index = tooltips.FindIndex(tip => tip.Name == "Defense" && tip.Mod == "Terraria");

            if (index != -1)
                tooltips.Insert(index + 1, new TooltipLine(TouhouPetsEx.Instance, "Protect", GetText("Protect")));
        }
    }
}

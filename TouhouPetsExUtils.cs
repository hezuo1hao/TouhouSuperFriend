global using static TouhouPetsEx.TouhouPetsExUtils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;
using Terraria.ID;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPetsEx
{
	public static class TouhouPetsExUtils
	{
        public static TouhouPetsExConfigs Config;
        public static TouhouPetsExLocalConfigs LocalConfig;
        public static Dictionary<int, int> EnhanceCount;
        public static EnhancePlayers MP(this Player player) => player.TryGetModPlayer<EnhancePlayers>(out var mp) ? mp : null;
        public static EnhanceBuffPlayers MBP(this Player player) => player.TryGetModPlayer<EnhanceBuffPlayers>(out var mbp) ? mbp : null;
        public static bool HasTouhouPetsBuff(this Player player) => player.buffType.Any(type => (Main.vanityPet[type] || Main.lightPet[type]) && BuffLoader.GetBuff(type)?.FullName.StartsWith("TouhouPets/") == true);
        public static bool HasEnhance<T>(this Player player) where T : ModItem => player.MP()?.ActiveEnhance.Concat(player.MP()?.ActivePassiveEnhance).Contains(ModContent.ItemType<T>()) == true;
        public static bool HasEnhance(this Player player, int type) => player.MP()?.ActiveEnhance.Concat(player.MP()?.ActivePassiveEnhance).Contains(type) == true;
        public static bool EnableEnhance<T>(this Player player) where T : ModItem => player.HasEnhance<T>() && player.HasTouhouPetsBuff();
        public static bool EnableEnhance(this Player player, int type) => player.HasEnhance(type) && player.HasTouhouPetsBuff();
        public static bool EnableAllYousei(this Player player) => player.EnableEnhance<CirnoIceShard>() && player.EnableEnhance<DaiyouseiBomb>() && player.EnableEnhance<LilyOneUp>() && player.EnableEnhance<HecatiaPlanet>() && (player.EnableEnhance<LightsJewels>() || (player.EnableEnhance<SunnyMilk>() && player.EnableEnhance<LunaMoon>() && player.EnableEnhance<StarSapphire>()));
        public static bool WorldEnableEnhance<T>() where T : ModItem => EnhanceCount.TryGetValue(ModContent.ItemType<T>(), out int value) && value > 0;
        public static int ReflectionDamage(this Player.HurtInfo info)
        {
            int damage = info.Damage;
            info.DamageSource.TryGetCausingEntity(out var npcOrProj);

            if (damage < info.SourceDamage)
                damage = info.SourceDamage;

            if (npcOrProj is NPC npc && damage < npc.damage)
                damage = npc.damage;

            if (npcOrProj is Projectile proj && damage < proj.damage)
                damage = proj.damage;

            return damage;
        }
        public static int GetTooltipsLastIndex(this List<TooltipLine> tooltips)
        {
            return tooltips
                .Select((t, index) => new { t.Name, Index = index }) // 保留索引
                .Where(x => x.Name.StartsWith("Tooltip") && int.TryParse(x.Name.Substring(7), out _)) // 筛选有效字符串
                .OrderByDescending(x => int.Parse(x.Name.Substring(7))) // 按数字降序排序
                .Select(x => x.Index) // 取索引
                .FirstOrDefault(); // 获取第一个结果
        }
        /// <summary>
        /// 可以让文本优先在其它文本之上
        /// </summary>
        public static int NewText(Rectangle location, Color color, int amount, bool dramatic = false, bool dot = false) => NewText(location, color, amount.ToString(), dramatic, dot);
        /// <summary>
        /// 可以让文本优先在其它文本之上
        /// </summary>
        public static int NewText(Rectangle location, Color color, string text, bool dramatic = false, bool dot = false)
        {
            if (Main.netMode == NetmodeID.Server)
                return 100;

            for (int i = Main.combatText.Length - 1; i >= 0; i--)
            {
                if (Main.combatText[i].active)
                    continue;

                int num = 0;
                if (dramatic)
                    num = 1;

                Vector2 vector = FontAssets.CombatText[num].Value.MeasureString(text);
                Main.combatText[i].alpha = 1f;
                Main.combatText[i].alphaDir = -1;
                Main.combatText[i].active = true;
                Main.combatText[i].scale = 0f;
                Main.combatText[i].rotation = 0f;
                Main.combatText[i].position.X = (float)location.X + (float)location.Width * 0.5f - vector.X * 0.5f;
                Main.combatText[i].position.Y = (float)location.Y + (float)location.Height * 0.25f - vector.Y * 0.5f;
                Main.combatText[i].position.X += Main.rand.Next(-(int)((double)location.Width * 0.5), (int)((double)location.Width * 0.5) + 1);
                Main.combatText[i].position.Y += Main.rand.Next(-(int)((double)location.Height * 0.5), (int)((double)location.Height * 0.5) + 1);
                Main.combatText[i].color = color;
                Main.combatText[i].text = text;
                Main.combatText[i].velocity.Y = -7f;
                if (Main.player[Main.myPlayer].gravDir == -1f)
                {
                    Main.combatText[i].velocity.Y *= -1f;
                    Main.combatText[i].position.Y = (float)location.Y + (float)location.Height * 0.75f + vector.Y * 0.5f;
                }

                Main.combatText[i].lifeTime = 60;
                Main.combatText[i].crit = dramatic;
                Main.combatText[i].dot = dot;
                if (dramatic)
                {
                    Main.combatText[i].text = text;
                    Main.combatText[i].lifeTime *= 2;
                    Main.combatText[i].velocity.Y *= 2f;
                    Main.combatText[i].velocity.X = (float)Main.rand.Next(-25, 26) * 0.05f;
                    Main.combatText[i].rotation = (float)(Main.combatText[i].lifeTime / 2) * 0.002f;
                    if (Main.combatText[i].velocity.X < 0f)
                        Main.combatText[i].rotation *= -1f;
                }

                if (dot)
                {
                    Main.combatText[i].velocity.Y = -4f;
                    Main.combatText[i].lifeTime = 40;
                }

                return i;
            }

            return 100;
        }
        public static string GetText(string name) => Language.GetTextValue("Mods.TouhouPetsEx." + name);
        public static string GetText(string name, object arg0) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0);
        public static string GetText(string name, object arg0, object arg1) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1);
        public static string GetText(string name, object arg0, object arg1, object arg2) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1, arg2);
        public static string GetText(string name, params object[] args) => Language.GetTextValue("Mods.TouhouPetsEx." + name, args);
    }
}

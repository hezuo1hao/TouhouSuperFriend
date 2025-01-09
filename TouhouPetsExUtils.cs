using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx
{
	public static class TouhouPetsExUtils
	{
        public static EnhancePlayers MP(this Player player) => player.GetModPlayer<EnhancePlayers>();
        public static int GetTooltipsLastIndex(this List<TooltipLine> tooltips)
        {
            return tooltips
                .Select((t, index) => new { t.Name, Index = index }) // 保留索引
                .Where(x => x.Name.StartsWith("Tooltip") && int.TryParse(x.Name.Substring(7), out _)) // 筛选有效字符串
                .OrderByDescending(x => int.Parse(x.Name.Substring(7))) // 按数字降序排序
                .Select(x => x.Index) // 取索引
                .FirstOrDefault(); // 获取第一个结果
        }
        public static string GetText(string name) => Language.GetTextValue("Mods.TouhouPetsEx." + name);
        public static string GetText(string name, object arg0) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0);
        public static string GetText(string name, object arg0, object arg1) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1);
        public static string GetText(string name, object arg0, object arg1, object arg2) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1, arg2);
        public static string GetText(string name, params object[] args) => Language.GetTextValue("Mods.TouhouPetsEx." + name, args);
    }
}

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
        public static List<BaseEnhance> GEnhanceInstances = new List<BaseEnhance>();
        public static EnhancePlayers MP(this Player player) => player.GetModPlayer<EnhancePlayers>();
        public static string GetText(string name) => Language.GetTextValue("Mods.TouhouPetsEx." + name);
        public static string GetText(string name, int arg0) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0);
        public static string GetText(string name, int arg0, int arg1) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1);
        public static string GetText(string name, int arg0, int arg1, int arg2) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1, arg2);
        public static string GetText(string name, params int[] args) => Language.GetTextValue("Mods.TouhouPetsEx." + name, args);
    }
}

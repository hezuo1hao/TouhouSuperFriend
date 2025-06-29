global using static TouhouPetsEx.TouhouPetsExUtils;
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
        public static TouhouPetsExConfigs Config;
        public static EnhancePlayers MP(this Player player) => player.GetModPlayer<EnhancePlayers>();
        public static bool HasTouhouPetsBuff(this Player player) => player.buffType.Any(type => (Main.vanityPet[type] || Main.lightPet[type]) && BuffLoader.GetBuff(type)?.FullName.StartsWith("TouhouPets/") == true);
        public static int GetTooltipsLastIndex(this List<TooltipLine> tooltips)
        {
            return tooltips
                .Select((t, index) => new { t.Name, Index = index }) // ��������
                .Where(x => x.Name.StartsWith("Tooltip") && int.TryParse(x.Name.Substring(7), out _)) // ɸѡ��Ч�ַ���
                .OrderByDescending(x => int.Parse(x.Name.Substring(7))) // �����ֽ�������
                .Select(x => x.Index) // ȡ����
                .FirstOrDefault(); // ��ȡ��һ�����
        }
        public static string GetText(string name) => Language.GetTextValue("Mods.TouhouPetsEx." + name);
        public static string GetText(string name, object arg0) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0);
        public static string GetText(string name, object arg0, object arg1) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1);
        public static string GetText(string name, object arg0, object arg1, object arg2) => Language.GetTextValue("Mods.TouhouPetsEx." + name, arg0, arg1, arg2);
        public static string GetText(string name, params object[] args) => Language.GetTextValue("Mods.TouhouPetsEx." + name, args);
    }
    public class BoolWrapper
    {
        public bool Value { get; set; }

        public BoolWrapper(bool initialValue)
        {
            Value = initialValue;
        }
    }
}

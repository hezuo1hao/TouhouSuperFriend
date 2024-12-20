using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class GEnhanceItems : GlobalItem
    {
        private static void ProcessDemonismAction(Action<BaseEnhance> action)
        {
            foreach (var enhance in TouhouPetsEx.GEnhanceInstances)
            {
                action(enhance.Value);
            }
        }
        private static void ProcessDemonismAction(Item item, Action<BaseEnhance> action)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance))
            {
                action(enhance);
            }
        }
        private static void ProcessDemonismAction(Player player, Action<BaseEnhance> action)
        {
            foreach (int id in player.MP().ActiveEnhance)
            {
                action(TouhouPetsEx.GEnhanceInstances[id]);
            }
        }
        private static bool? ProcessDemonismAction(Item item, Func<BaseEnhance, bool?> action)
        {
            bool? @return = null;
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance))
            {
                @return = action(enhance);
            }
            return @return;
        }
        private static bool? ProcessDemonismAction(Player player, Func<BaseEnhance, bool?> action)
        {
            bool? @return = null;
            foreach (int id in player.MP().ActiveEnhance)
            {
                @return = action(TouhouPetsEx.GEnhanceInstances[id]);
            }
            return @return;
        }
        public override void SetDefaults(Item entity)
        {
            ProcessDemonismAction(entity, (enhance) => enhance.ItemSD(entity));
        }
        public override void HoldItem(Item item, Player player)
        {
            ProcessDemonismAction((enhance) => enhance.ItemHoldItem(item, player));
        }
        public override void UpdateInventory(Item item, Player player)
        {
            ProcessDemonismAction((enhance) => enhance.ItemUpdateInventory(item, player));
        }
        public override bool? UseItem(Item item, Player player)
        {
            return ProcessDemonismAction(player, (enhance) => enhance.ItemUseItem(item, player));
        }
        public override void SetStaticDefaults()
        {
            List<Type> subclasses = new List<Type>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] allTypes = assembly.GetTypes();

            foreach (Type type in allTypes)
            {
                if (type.IsClass && !type.IsAbstract && typeof(BaseEnhance).IsAssignableFrom(type))
                {
                    subclasses.Add(type);
                }
            }

            foreach (Type types in subclasses)
            {
                object enhance = Activator.CreateInstance(types);
                BaseEnhance thisEnhance = enhance as BaseEnhance;
                thisEnhance.ItemSSD();
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enh))
            {
                tooltips.Insert(tooltips.GetTooltipsLastIndex() + 1, new TooltipLine(Mod, "EnhanceTooltip", TouhouPetsExUtils.GetText("Common") + "\n" + enh.Text));
            }

            ProcessDemonismAction((enhance) => enhance.ItemModifyTooltips(item, tooltips));
        }
        public override bool AltFunctionUse(Item item, Player player)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enh))
                return enh.EnableRightClick;

            return false;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && player.altFunctionUse == 2
                && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enh) && enh.EnableRightClick)
            {
                if (player.MP().ActiveEnhance.Contains(item.type))
                {
                    player.MP().ActiveEnhance.Remove(item.type);
                    CombatText.NewText(player.getRect(), Color.Cyan, TouhouPetsExUtils.GetText("Disable"));
                }
                else
                {
                    if (player.MP().ActiveEnhance.Count == player.MP().ActiveEnhanceCount)
                        player.MP().ActiveEnhance.RemoveAt(0);

                    player.MP().ActiveEnhance.Add(item.type);
                    CombatText.NewText(player.getRect(), Color.Cyan, TouhouPetsExUtils.GetText("Enable"));
                }

                return false;
            }

            return true;
        }
    }
}

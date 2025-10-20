using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            if (!player.HasTouhouPetsBuff())
                return;

            foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
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
        /// <param name="priority">填写需要优先返回的bool结果，如：执行三次，俩false一true，需求true，则返回true结果
        /// <br>特别的，如果填null则会返回最后一个非null的结果</br>
        /// </param>
        private static bool? ProcessDemonismAction(Player player, bool? priority, Func<BaseEnhance, bool?> action)
        {
            if (!player.HasTouhouPetsBuff())
                return null;

            if (priority == null)
            {
                bool? @return = null;
                foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    bool? a = action(TouhouPetsEx.GEnhanceInstances[id]);
                    if (a != null) @return = a;
                }
                return @return;
            }
            else
            {
                bool? @return = null;
                foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    bool? a = action(TouhouPetsEx.GEnhanceInstances[id]);
                    if (a == priority) return a;
                    else if (a != null) @return = a;
                }
                return @return;
            }
        }
        /// <param name="priority">填写需要优先返回的bool结果，如：执行三次，俩false一true，需求true，则返回true结果
        /// <br>特别的，如果填null则会返回最后一个非null的结果</br>
        /// </param>
        private static bool? ProcessDemonismAction(bool? priority, Func<BaseEnhance, bool?> action)
        {
            if (priority == null)
            {
                bool? @return = null;
                foreach (var enhance in TouhouPetsEx.GEnhanceInstances)
                {
                    bool? a = action(enhance.Value);
                    if (a != null) @return = a;
                }
                return @return;
            }
            else
            {
                bool? @return = null;
                foreach (var enhance in TouhouPetsEx.GEnhanceInstances)
                {
                    bool? a = action(enhance.Value);
                    if (a == priority) return a;
                    else if (a != null) @return = a;
                }
                return @return;
            }
        }
        public override void SetDefaults(Item entity)
        {
            ProcessDemonismAction(entity, (enhance) => enhance.ItemSD(entity));
        }
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (player.MBP().Throw)
                grabRange += 1600;
        }
        public override void HoldItem(Item item, Player player)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance) && enhance.Passive && !player.EnableEnhance(item.type))
                player.MP().ActivePassiveEnhance.Add(item.type);

            ProcessDemonismAction((enhance) => enhance.ItemHoldItem(item, player));
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance) && enhance.Passive && !player.EnableEnhance(item.type))
                player.MP().ActivePassiveEnhance.Add(item.type);

            ProcessDemonismAction((enhance) => enhance.ItemUpdateInventory(item, player));
        }
        public override bool? UseItem(Item item, Player player)
        {
            return ProcessDemonismAction(player, false, (enhance) => enhance.ItemUseItem(item, player));
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
                int index = tooltips.GetTooltipsLastIndex();
                int experimentalTooltip = 0;
                tooltips.Insert(index + 1, new TooltipLine(Mod, "EnhanceTooltip", GetText("Common") + "\n" + (enh.Passive ? GetText("Passive") + "\n" : "") + enh.Text));

                for (int i = 0; i < enh.Experimental.Length; i++)
                {
                    if (enh.Experimental[i])
                    {
                        if (experimentalTooltip == 0)
                        {
                            tooltips.Insert(index + 2, new TooltipLine(Mod, "EnhanceTooltip_Experimental", GetText("Experimental")));
                            experimentalTooltip++;
                        }

                        tooltips.Insert(index + 2 + experimentalTooltip, new TooltipLine(Mod, "EnhanceTooltip_Experimental", enh.ExperimentalText[i]));
                        experimentalTooltip++;
                    }
                }
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
            bool def = true;
            bool? reesult = ProcessDemonismAction(player, false, (enhance) => enhance.ItemCanUseItem(item, player, ref def));

            if (reesult.HasValue)
                return reesult.Value;

            if (def && item.ModItem?.Mod.Name == "TouhouPets" && player.altFunctionUse == 2
                && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enh) && enh.EnableRightClick)
            {
                if (player.MP().ActiveEnhance.Contains(item.type))
                {
                    player.MP().ActiveEnhance.Remove(item.type);
                    CombatText.NewText(player.getRect(), Color.Cyan, GetText("Disable"));
                }
                else
                {
                    if (player.MP().ActiveEnhance.Count >= player.MP().ActiveEnhanceCount)
                    {
                        player.MP().ActiveEnhance.RemoveAt(0);
                        player.MP().ActiveEnhance.Add(item.type);
                    }
                    else
                        player.MP().ActiveEnhance.Add(item.type);

                    CombatText.NewText(player.getRect(), Color.Cyan, GetText("Enable"));
                }

                EnhancePlayers.AwardPlayerSync(Mod, -1, player.whoAmI);

                return false;
            }

            return true;
        }
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            bool? reesult = ProcessDemonismAction(Main.LocalPlayer, false, (enhance) => enhance.ItemPreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale));

            return reesult ?? base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            ProcessDemonismAction((enhance) => enhance.ItemPostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale));
        }
    }
}

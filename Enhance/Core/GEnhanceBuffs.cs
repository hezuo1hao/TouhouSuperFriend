using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class GEnhanceBuffs : GlobalBuff
    {
        private static void ProcessDemonismAction(Player player, Action<BaseEnhance> action)
        {
            if (!player.HasTouhouPetsBuff())
                return;

            foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
            {
                action(TouhouPetsEx.GEnhanceInstances[id]);
            }
        }
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            string buffName2 = buffName;
            string tip2 = tip;
            int rare2 = rare;
            ProcessDemonismAction(Main.LocalPlayer, (enhance) => enhance.ModifyBuffText(Main.LocalPlayer, type, ref buffName2, ref tip2, ref rare2));
            buffName = buffName2;
            tip = tip2;
            rare = rare2;

            if (BuffLoader.GetBuff(type)?.Mod.Name == "TouhouPets")
            {
                if (Main.LocalPlayer.MP().ActiveEnhance.Count > 0)
                    tip += "\n" + GetText("Common");

                foreach (int id in Main.LocalPlayer.MP().ActiveEnhance.Concat(Main.LocalPlayer.MP().ActivePassiveEnhance))
                {
                    var enh = TouhouPetsEx.GEnhanceInstances[id];
                    tip += "\n" + enh.Text;

                    for (int i = 0; i < enh.Experimental.Length; i++)
                    {
                        if (enh.Experimental[i] && enh.ExperimentalText[i] != "")
                        {
                            tip += "\n" + enh.ExperimentalText[i];
                        }
                    }
                }
            }
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            int buffIndex2 = buffIndex;
            ProcessDemonismAction(player, (enhance) => enhance.BuffUpdate(type, player, ref buffIndex2));
            buffIndex = buffIndex2;
        }
    }
}

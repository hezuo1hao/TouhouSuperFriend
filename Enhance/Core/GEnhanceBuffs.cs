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
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (BuffLoader.GetBuff(type)?.Mod.Name == "TouhouPets")
            {
                if (Main.LocalPlayer.MP().ActiveEnhance.Count > 0)
                    tip += "\n" + TouhouPetsExUtils.GetText("Common");

                foreach (int id in Main.LocalPlayer.MP().ActiveEnhance)
                {
                    tip += "\n" + TouhouPetsEx.GEnhanceInstances[id].Text;
                }
            }
        }
    }
}

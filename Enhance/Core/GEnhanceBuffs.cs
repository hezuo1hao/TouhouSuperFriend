using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class GEnhanceBuffs : GlobalBuff
    {
        private static bool alreadyDrawn;
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
            Player player = Main.LocalPlayer;

            string buffName2 = buffName;
            string tip2 = tip;
            int rare2 = rare;
            ProcessDemonismAction(player, (enhance) => enhance.ModifyBuffText(player, type, ref buffName2, ref tip2, ref rare2));
            buffName = buffName2;
            tip = tip2;
            rare = rare2;

            if (!LocalConfig.Tooltip_1 && !LocalConfig.Tooltip_2)
                return;

            if (BuffLoader.GetBuff(type)?.Mod.Name == "TouhouPets")
            {
                List<int> allActiveEnhance = [];

                if (LocalConfig.Tooltip_1)
                    allActiveEnhance.AddRange(player.MP().ActiveEnhance.Where(id => TouhouPetsEx.GEnhanceInstances[id].EnableBuffText));

                if (LocalConfig.Tooltip_2)
                    allActiveEnhance.AddRange(player.MP().ActivePassiveEnhance.Where(id => TouhouPetsEx.GEnhanceInstances[id].EnableBuffText));

                HashSet<int> allBanTootips = [];
                allActiveEnhance.ForEach(id => allBanTootips.UnionWith(TouhouPetsEx.GEnhanceInstances[id].BanTootips));
                allActiveEnhance.RemoveAll(allBanTootips.Contains);

                if (allActiveEnhance.Count != 0)
                    tip += "\n" + GetText("Common");
                else return;

                foreach (int id in allActiveEnhance)
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
        public override void PostDraw(SpriteBatch spriteBatch, int type, int buffIndex, BuffDrawParams drawParams)
        {
            if (!alreadyDrawn && LocalConfig.Tooltip_3 && BuffLoader.GetBuff(type)?.Mod.Name == "TouhouPets")
            {
                alreadyDrawn = true;
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, Main.LocalPlayer.MP().ActiveEnhance.Count + " / " + Main.LocalPlayer.MP().ActiveEnhanceCount, drawParams.Position.X, drawParams.Position.Y + 32, Color.AliceBlue, Color.Black, Vector2.Zero, 0.8f);
            }
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            alreadyDrawn = false;

            int buffIndex2 = buffIndex;
            ProcessDemonismAction(player, (enhance) => enhance.BuffUpdate(type, player, ref buffIndex2));
            buffIndex = buffIndex2;
        }
    }
}

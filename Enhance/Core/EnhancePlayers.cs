using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TouhouPetsEx.Enhance.Core
{
	public class EnhancePlayers : ModPlayer
    {
        public List<int> ActiveEnhance = [];
        public int ActiveEnhanceCount = 1;
        /// <summary>
        /// ¬‹¿ÚÀø”√
        /// </summary>
        public int EatBook = 0;
        public int DaiyouseiCD = 0;
        private static void ProcessDemonismAction(Player player, Action<BaseEnhance> action)
        {
            foreach (int id in player.MP().ActiveEnhance)
            {
                action(TouhouPetsEx.GEnhanceInstances[id]);
            }
        }
        public override void ResetEffects()
        {
            ActiveEnhanceCount = 1;
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add("EatBook", EatBook);
        }
        public override void LoadData(TagCompound tag)
        {
            EatBook = tag.GetInt("EatBook");
        }
        public override void PostUpdateEquips()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdateEquips(Player));
        }
        public override void PostUpdate()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdate(Player));
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            float r2 = r;
            float g2 = g;
            float b2 = b;
            float a2 = a;
            bool fullBright2 = fullBright;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerDrawEffects(drawInfo, ref r2, ref g2, ref b2, ref a2, ref fullBright2));
            r = r2;
            g = g2;
            b = b2;
            a2 = a;
            fullBright2 = fullBright;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            Player.HurtModifiers modifiers2 = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHurt(Player, ref modifiers2));
            modifiers2 = modifiers;
        }
    }
}

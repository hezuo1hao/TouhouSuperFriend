using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class EnhancePlayers : ModPlayer
    {
        public List<int> ActiveEnhance = [];
        public int ActiveEnhanceCount = 1;
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
        public override void PostUpdate()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdate(Player));
        }
    }
}

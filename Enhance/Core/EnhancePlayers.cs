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
        /// 萝莉丝用
        /// </summary>
        public int EatBook = 0;
        /// <summary>
        /// 大妖精用
        /// </summary>
        public int DaiyouseiCD = 0;
        /// <summary>
        /// 姬虫百百世用
        /// <para>索引决定对应的加成：0—移动速度、1—挖矿速度、2—最大氧气值、3—最大生命值、4—岩浆免疫时间、5—伤害减免、6—暴击伤害、7—运气、8—百分比穿甲</para>
        /// </summary>
        public float[] ExtraAddition = [];
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
            tag.Add("ExtraAddition", ExtraAddition);
        }
        public override void LoadData(TagCompound tag)
        {
            EatBook = tag.GetInt("EatBook");
            ExtraAddition = tag.Get<float[]>("ExtraAddition");
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

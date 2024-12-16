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
	public class BaseEnhance
	{
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Text => "";
        /// <summary>
        /// 是否启用右键，默认启用
        /// </summary>
        public virtual bool EnableRightClick => true;
        /// <summary>
        /// 添加本增强与对应物品之间的联系
        /// </summary>
        /// <param name="type">对应物品的type</param>
        public void AddEnhance(int type)
        {
            TouhouPetsEx.GEnhanceInstances[type] = this;
        }
        public virtual void ItemSSD()
        {

        }
        public virtual void PlayerPostUpdate(Player player)
        {

        }
    }
}

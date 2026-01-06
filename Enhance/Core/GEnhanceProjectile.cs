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
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Core
{
	/// <summary>
	/// 增强相关的全局投射物数据（按实体实例化）。
	/// </summary>
	public class GEnhanceProjectile : GlobalProjectile
    {
        /// <summary>
        /// 只在本地进行判定
        /// </summary>
        public bool Bullet;
        /// <summary>
        /// 每个投射物实例拥有独立的 <see cref="GlobalProjectile"/> 数据。
        /// </summary>
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // 只对“由子弹类弹药发射”的投射物打标记，用于后续本地逻辑（如 Reisen 的转向）。
            if (source is EntitySource_ItemUse_WithAmmo s && s.AmmoItemIdUsed > 0 && ContentSamples.ItemsByType[s.AmmoItemIdUsed].ammo == ItemID.MusketBall)
                Bullet = true;
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class GEnhanceProjectile : GlobalProjectile
    {
        /// <summary>
        /// ֻ�ڱ��ؽ����ж�
        /// </summary>
        public bool Bullet;
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo s && s.AmmoItemIdUsed > 0 && ContentSamples.ItemsByType[s.AmmoItemIdUsed].ammo == ItemID.MusketBall)
                Bullet = true;
        }
    }
}

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
	/// <summary>
	/// 增强相关的全局 Tile 钩子分发。
	/// <para>
	/// 通过 <see cref="EnhanceHookRegistry"/> 只调用真正覆写了 Tile 钩子的增强，避免每个 Tile 都遍历全部增强。
	/// </para>
	/// </summary>
	public class GEnhanceTile : GlobalTile
    {
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // TileDrawInfo 是 struct，这里用临时变量承接多增强的链式修改。
            TileDrawInfo drawData2 = drawData;
            foreach (BaseEnhance enhance in EnhanceHookRegistry.TileDrawEffects)
                enhance.TileDrawEffects(i, j, type, spriteBatch, ref drawData2);
            drawData = drawData2;
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            // RandomUpdate 只分发给真正实现该钩子的增强。
            foreach (BaseEnhance enhance in EnhanceHookRegistry.TileRandomUpdate)
                enhance.TileRandomUpdate(i, j, type);
        }
    }
}

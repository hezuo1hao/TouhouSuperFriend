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
	public class GEnhanceTile : GlobalTile
    {
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            TileDrawInfo drawData2 = drawData;
            foreach (BaseEnhance enhance in EnhanceHookRegistry.TileDrawEffects)
                enhance.TileDrawEffects(i, j, type, spriteBatch, ref drawData2);
            drawData = drawData2;
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            foreach (BaseEnhance enhance in EnhanceHookRegistry.TileRandomUpdate)
                enhance.TileRandomUpdate(i, j, type);
        }
    }
}

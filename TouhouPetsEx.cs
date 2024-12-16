using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx
{
	public class TouhouPetsEx : Mod
	{
        public static Dictionary<int, BaseEnhance> GEnhanceInstances = [];
    }
}

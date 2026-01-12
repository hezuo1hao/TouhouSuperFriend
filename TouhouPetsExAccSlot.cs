using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPetsEx
{
	public class HecatiaAccSlot_1 : ModAccessorySlot
	{
        public override bool IsHidden()
        {
            return !(Config.Hecatia && Player.EnableEnhance<HecatiaPlanet>()) && IsEmpty;
        }
        public override bool IsEnabled()
        {
            return Config.Hecatia && Player.EnableEnhance<HecatiaPlanet>();
        }
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            return base.CanAcceptItem(checkItem, context) && Config.Hecatia && Player.EnableEnhance<HecatiaPlanet>();
        }
    }
    public class HecatiaAccSlot_2 : ModAccessorySlot
    {
        public override bool IsHidden()
        {
            return !(Config.Hecatia && Main.hardMode && Player.EnableEnhance<HecatiaPlanet>()) && IsEmpty;
        }
        public override bool IsEnabled()
        {
            return Config.Hecatia && Main.hardMode && Player.EnableEnhance<HecatiaPlanet>();
        }
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            return base.CanAcceptItem(checkItem, context) && Config.Hecatia && Main.hardMode && Player.EnableEnhance<HecatiaPlanet>();
        }
    }
    public class HecatiaAccSlot_3 : ModAccessorySlot
    {
        public override bool IsHidden()
        {
            return !(Config.Hecatia && NPC.downedMoonlord && Player.EnableEnhance<HecatiaPlanet>()) && IsEmpty;
        }
        public override bool IsEnabled()
        {
            return Config.Hecatia && NPC.downedMoonlord && Player.EnableEnhance<HecatiaPlanet>();
        }
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            return base.CanAcceptItem(checkItem, context) && Config.Hecatia && NPC.downedMoonlord && Player.EnableEnhance<HecatiaPlanet>();
        }
    }
}

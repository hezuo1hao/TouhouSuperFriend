using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Luna : BaseEnhance
    {
        public override string Text => GetText("Luna");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<LunaMoon>());
        }
        public override bool? NPCCanHitNPC(NPC npc, NPC target)
        {
            if (target.type != NPCID.DD2EterniaCrystal && (target.friendly || NPCID.Sets.CountsAsCritter[target.type]) && (WorldEnableEnhance<LunaMoon>() || WorldEnableEnhance<LightsJewels>()))
                return false;

            return null;
        }
    }
}

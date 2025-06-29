using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Rumia : BaseEnhance
    {
        public override string Text => GetText("Rumia");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<RumiaRibbon>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (!npc.friendly && npc.Center.Distance(player.Center) < 120)
                    npc.AddBuff(BuffID.Confused, 60);
            }
        }
    }
}

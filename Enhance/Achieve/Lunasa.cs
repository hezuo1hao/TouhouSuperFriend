using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Lunasa : BaseEnhance
    {
        public override string Text => GetText("Lunasa");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<LunasaViolin>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.friendly && npc.Center.Distance(player.Center) < 240)
                    npc.AddBuff(ModContent.BuffType<Depression>(), 60);
            }
        }
    }
}

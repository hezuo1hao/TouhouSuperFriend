using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Hina : BaseEnhance
    {
        public override string Text => GetText("Hina");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<HinaDoll>());
        }
        public override void PlayerModifyLuck(Player player, ref float luck)
        {
            if (luck < 0)
                luck = 0;

            luck += 0.001f;
        }
        public override void ItemUpdateInventory(Item item, Player player)
        {
            if (!player.EnableEnhance<HinaDoll>())
                return;

            int i = 0;
            while (PrefixID.Sets.ReducedNaturalChance[item.prefix])
            {
                if (i >= 100)
                {
                    item.prefix = 0;
                    return;
                }

                item.ResetPrefix();
                item.Prefix(-2);
                i++;
            }
        }
        float[] rots = new float[200];
        public override void PlayerPostUpdate(Player player)
        {
            if (DateTime.Now.Month != 4 || DateTime.Now.Day != 1)
                return;

            player.fullRotation -= 0.07f;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if ((npc.Distance(player.Center) < 500 || npc.target == player.whoAmI))
                {
                    rots[npc.whoAmI] += 0.07f;
                    npc.rotation = rots[npc.whoAmI];

                    if (npc.boss && player == Main.LocalPlayer)
                        ModContent.GetInstance<SpinSpinSpin>().Condition.Complete();
                }
            }
        }
    }
}

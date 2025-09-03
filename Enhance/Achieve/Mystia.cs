using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Mystia : BaseEnhance
    {
        public override string Text => GetText("Mystia");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MystiaFeather>());
        }
        private static int[] Buffs = [ModContent.BuffType<Glutton>(), ModContent.BuffType<Patience>(), ModContent.BuffType<Throw>()];
        public override void PlayerResetEffects(Player player)
        {
            if (player.MP().MystiaCD > 0)
                player.MP().MystiaCD--;
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && player.MP().MystiaCD == 0 && !player.HasBuff(ModContent.BuffType<MystiasSong>()) && Main.rand.NextBool(4))
            {
                player.MP().MystiaCD = 60;

                List<int> buffs = [.. Buffs.Where(buffId => !player.buffType.Contains(buffId))];

                if (buffs.Count > 1)
                    player.AddBuff(Main.rand.Next(buffs), 600);
                else
                {
                    foreach (int buffId in Buffs)
                        if (player.HasBuff(buffId))
                            player.ClearBuff(buffId);

                    player.AddBuff(ModContent.BuffType<MystiasSong>(), 900);
                }
            }
        }
    }
}

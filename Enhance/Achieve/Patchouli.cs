using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Patchouli : BaseEnhance
    {
        public override string Text => GetText("Patchouli");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<PatchouliMoon>());
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hit.DamageType.CountsAsClass(DamageClass.Magic))
                return;

            List<int> buffs = [BuffID.Midas, BuffID.Wet, BuffID.Poisoned, BuffID.OnFire, ModContent.BuffType<Earth>()];
            if (NPC.downedPlantBoss)
            {
                if (!target.HasBuff(BuffID.Daybreak))
                    buffs.Add(BuffID.Daybreak);

                buffs.Add(ModContent.BuffType<MoonMist>());
            }

            target.AddBuff(Main.rand.Next(buffs), Main.rand.Next(180, 421));
        }
    }
}

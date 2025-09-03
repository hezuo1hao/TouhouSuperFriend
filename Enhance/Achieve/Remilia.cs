using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Remilia : BaseEnhance
    {
        public override string Text => GetText("Remilia");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<RemiliaRedTea>());
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (player.lifeSteal > -60)
                player.lifeSteal = -60;

            player.lifeSteal += 0.2f;
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hit.DamageType.CountsAsClass(DamageClass.Melee) || !Main.rand.NextBool(20))
                return;

            player.Heal(1);
        }
    }
}

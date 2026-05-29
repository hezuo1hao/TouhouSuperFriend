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
        public override void PlayerPostUpdateBuffs(Player player)
        {
            player.statLifeMax2 += player.statLifeMax / 5 / 20 * 20;
        }
        public override void PlayerPostUpdate(Player player)
        {
            if (player.lifeSteal < -60)
                player.lifeSteal = -60;

            player.lifeSteal += 0.2f;

            if (player.MP().RemiliaCD > 0)
                player.MP().RemiliaCD--;
        }
        public override void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.MP().RemiliaCD > 0)
                return;

            float probability = player.statLifeMax2 / 20f;
            if (hit.DamageType.CountsAsClass(DamageClass.Melee))
                probability *= 2;

            if (player.RollGoodLuck(100) >= probability)
                return;

            player.Heal(10);
            player.MP().RemiliaCD = 60;
        }
    }
}

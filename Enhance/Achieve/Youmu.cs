using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Youmu : BaseEnhance
    {
        public override string Text => GetText("Youmu");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<YoumuKatana>());
        }
        public override void PlayerPostUpdateEquips(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
        }
        public override void PlayerModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 1.15f;
        }
    }
}

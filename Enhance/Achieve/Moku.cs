using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Moku : BaseEnhance
    {
        public override string Text => GetText("Moku");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<MokuMatch>());
        }
        public override void PlayerUpdateLifeRegen(Player player)
        {
            if ((TouhouPetsExModSystem.SynchronousTime % 30 == 0 && !player.pStone) || (TouhouPetsExModSystem.SynchronousTime % 24 == 0 && player.pStone))
                player.lifeRegenCount += player.lifeRegenCount < 0 ? 125 - player.lifeRegenCount : 125;
        }
        public override void PlayerKill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (player.respawnTimer > 180)
                player.respawnTimer = 180;
        }
    }
}

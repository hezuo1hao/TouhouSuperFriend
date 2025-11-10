using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Kaguya : BaseEnhance
    {
        public override string Text => GetText("Kaguya");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<KaguyaBranch>());
        }
        public override void PlayerPreUpdateBuffsAlways(Player player)
        {
            player.MP().OldBuff = (int[])player.buffTime.Clone();
        }
        public override void BuffUpdate(int type, Player player, ref int buffIndex)
        {
            if (player.MP().OldBuff[buffIndex] == player.buffTime[buffIndex] || BuffID.Sets.TimeLeftDoesNotDecrease[type] || BuffID.Sets.NurseCannotRemoveDebuff[type])
                return;

            if (Main.debuff[type])
                player.buffTime[buffIndex]--;
            else if (TouhouPetsExModSystem.SynchronousTime % 2 == 0)
                player.buffTime[buffIndex]++;

            if (player == Main.LocalPlayer)
            {
                var tt2 = ModContent.GetInstance<TT2>();
                tt2.Condition.Value++;

                if (Main.debuff[type])
                    tt2.Condition.Value++;

                if (tt2.Condition.Value > TT2.Max)
                    tt2.Condition.Complete();
            }    
        }
    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sakuya : BaseEnhance
    {
        public override string Text => GetText("Sakuya");
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SakuyaWatch>());
        }
        public override void PlayerResetEffectsAlways(Player player)
        {
            if (!player.EnableEnhance<SakuyaWatch>() && player.MP().frameSkipMode != null)
            {
                Main.FrameSkipMode = (Terraria.Enums.FrameSkipMode)player.MP().frameSkipMode;
                player.MP().frameSkipMode = null;
            }
        }
        public override void PlayerPreUpdate(Player player)
        {
            if (player.MP().frameSkipMode == null)
                player.MP().frameSkipMode = Main.FrameSkipMode;

            Main.FrameSkipMode = Terraria.Enums.FrameSkipMode.Off;
            Main.UpdateTimeAccumulator -= Main.gameTimeCache.ElapsedGameTime.TotalSeconds / 1.67f;
        }
    }
}

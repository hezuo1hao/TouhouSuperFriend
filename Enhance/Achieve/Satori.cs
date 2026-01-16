using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Satori : BaseEnhance
    {
        public override string Text => GetText("Satori");
        public override string[] ExperimentalText => [GetText("Satori_1"), GetText("Satori_2"), GetText("Satori_3")];
        public override bool[] Experimental => [Config.Satori, Config.Satori_2, Config.Satori_3];
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SatoriSlippers>());
        }
        public override bool? NPCPreAI(NPC npc)
        {
            if (Main.netMode == NetmodeID.Server || npc.dontTakeDamage || npc.friendly || (npc.aiStyle == 112 && !(npc.ai[2] <= 1f)) || !Main.LocalPlayer.CanNPCBeHitByPlayerOrPlayerProjectile(npc) || !Main.LocalPlayer.EnableEnhance<SatoriSlippers>())
                return null;

            if (Config.Satori_2)
            {
                if (Collision.CheckAABBvAABBCollision(npc.position, npc.Size, Main.LocalPlayer.MountedCenter - Vector2.UnitY * 136 - Vector2.UnitX * 1200, new Vector2(2400, 320)))
                    npc.AddBuff(BuffID.Confused, 60);
            }
            else if (Collision.CheckAABBvAABBCollision(npc.position, npc.Size, Main.LocalPlayer.MountedCenter - Vector2.UnitY * 136 - Vector2.UnitX * (Main.LocalPlayer.direction == 1 ? 0 : 1200), new Vector2(1200, 320)))
                npc.AddBuff(BuffID.Confused, 60);

            return null;
        }
        public override void SystemPostUpdateEverything()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (!Config.Satori || !player.EnableEnhance<SatoriSlippers>())
                    return;

                EnhanceRegistry.TryGetEnhanceId(ModContent.ItemType<RinSkull>(), out EnhancementId rinId);
                EnhanceRegistry.TryGetEnhanceId(ModContent.ItemType<UtsuhoEye>(), out EnhancementId utsuhoId);
                EnhanceRegistry.TryGetEnhanceId(ModContent.ItemType<KoishiTelephone>(), out EnhancementId koishiId);

                player.MP().ActiveEnhance.RemoveAll(id => id == rinId || id == utsuhoId || id == koishiId);

            }
        }
    }
}

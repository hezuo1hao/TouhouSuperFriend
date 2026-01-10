using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class HecatiaAndPiece : BaseEnhance
    {
        private const int TorchScanIntervalTicks = 25;
        private const int TorchScanRadiusTiles = 15; // 方形范围：左右/上下各 15 格

        public override string Text => GetText("HecatiaAndPiece");
        public override string[] ExperimentalText => [GetText("HecatiaAndPiece_1")];
        public override bool[] Experimental => [Config.Hecatia];
        public override bool EnableRightClick => false;
        public override bool Passive => true;

        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<HecatiaPlanet>());
        }

        public override void PlayerResetEffects(Player player)
        {
            if (Main.hardMode)
                player.MP().ActiveEnhanceCount++;

            if (NPC.downedMoonlord)
                player.MP().ActiveEnhanceCount++;
        }

        public override void NPCAI(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || npc.dontTakeDamage || npc.friendly ||
                NPCID.Sets.CountsAsCritter[npc.type])
                return;

            GEnhanceNPCs gNpc = npc.GetGlobalNPC<GEnhanceNPCs>();

            if (EnhanceCount == null ||
                !EnhanceRegistry.TryGetEnhanceId(ModContent.ItemType<HecatiaPlanet>(), out EnhancementId enhanceId) ||
                !EnhanceCount.TryGetValue(enhanceId, out int magnification) ||
                magnification <= 0)
                return;

            _touchDmg(npc, gNpc, magnification);

            if (Main.GameUpdateCount % 30 != 17 || gNpc.TorchDamage <= 0)
                return;

            npc.SimpleStrikeNPC(gNpc.TorchDamage, 0);
            gNpc.TorchDamage = 0;
        }

        private static void _touchDmg(NPC npc, GEnhanceNPCs gNpc, int magnification)
        {
            // 每隔 15 tick 进行一次“完整扫描”，并用 whoAmI 分桶，把不同 NPC 的扫描摊到不同 tick 上，允许部分 NPC 的扫描延后。
            if ((Main.GameUpdateCount % (ulong)TorchScanIntervalTicks) !=
                (ulong)(npc.whoAmI % TorchScanIntervalTicks))
                return;

            int centerTileX = (int)(npc.Center.X / 16f);
            int centerTileY = (int)(npc.Center.Y / 16f);

            int minTileX = Math.Max(0, centerTileX - TorchScanRadiusTiles);
            int maxTileX = Math.Min(Main.maxTilesX - 1, centerTileX + TorchScanRadiusTiles);
            int minTileY = Math.Max(0, centerTileY - TorchScanRadiusTiles);
            int maxTileY = Math.Min(Main.maxTilesY - 1, centerTileY + TorchScanRadiusTiles);

            int torchTiles = 0;
            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int y = minTileY; y <= maxTileY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && TileID.Sets.Torch[tile.TileType])
                        torchTiles++;
                }
            }

            int torchBuckets = (torchTiles + 9) / 10;
            if (torchBuckets > 0)
                gNpc.TorchDamage += torchBuckets * magnification;
        }
    }
}

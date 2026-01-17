using System;
using System.Security.Principal;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx.Enhance.Achieve
{
    public class Sizuha : BaseEnhance
    {
        public override string Text => GetText("Sizuha");
        public override bool Passive => true;
        public override bool EnableRightClick => false;
        public override void ItemSSD()
        {
            AddEnhance(ModContent.ItemType<SizuhaBrush>());
        }
        public override void SystemPostUpdateEverything()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.EnableEnhance<SizuhaBrush>())
                {
                    Tile tile = Framing.GetTileSafely(player.Center);
                    if (TileID.Sets.IsShakeable[tile.TileType])
                        WorldGen.ShakeTree((int)player.Center.X / 16, (int)player.Center.Y / 16);
                }
            }
        }
        public override void ProjectileOnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ShakeTree && projectile.type is ProjectileID.BeeHive && WorldEnableEnhance<SizuhaBrush>())
            {
                projectile.active = false;
                projectile.type = ProjectileID.None;
                Main.projectileIdentity[projectile.owner, projectile.identity] = -1;
                projectile.timeLeft = 0;

                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
            }
        }
        public override void NPCOnSpawn(NPC npc, IEntitySource source)
        {
            if (source is EntitySource_ShakeTree && npc.type is NPCID.Bee or NPCID.BeeSmall or NPCID.JungleBat or NPCID.EaterofSouls or NPCID.Crimera && WorldEnableEnhance<SizuhaBrush>())
                npc.active = false;
        }
    }
}

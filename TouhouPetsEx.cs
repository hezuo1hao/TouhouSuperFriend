using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx
{
	public class TouhouPetsEx : Mod
	{
        public static int[] WhitelistBlock = [2, 3, 3086, 3081, 169, 3271, 3272, 133, 176, 172, 593, 664, 9, 620, 619, 911, 2503, 2504, 1727, 4564, 586, 591, 1872];
        public static int[] ColdProjVanilla = [118, 119, 120, 166, 172, 253, 309, 337, 344, 359, 520, 979];
        public static List<int> ColdProjAll = [];
        public static int[] VanillaBug = [355, 356, 357, 358, 359, 360, 377, 484, 485, 486, 487, 595, 596, 597, 598, 599, 600, 604, 606, 612, 653, 654, 655, 669];
        public static int[] VanillaGoldBug = [444, 446, 448, 601, 605, 613];
        public static HashSet<int> OceanEnemy = [];
        public static Dictionary<int, BaseEnhance> GEnhanceInstances = [];
        public static TouhouPetsEx Instance;
        public static Effect RingShader;
        public static Effect DistortShader;
        public static Effect TransformShader;
        public static Effect GrayishWhiteShader;
        public static BlendState InverseColor;
        public static int TransparentHead = -1;
        public override void Load()
        {
            Instance = this;
            RingShader = ModContent.Request<Effect>("TouhouPetsEx/Effects/Ring", AssetRequestMode.ImmediateLoad).Value;
            DistortShader = ModContent.Request<Effect>("TouhouPetsEx/Effects/Distort", AssetRequestMode.ImmediateLoad).Value;
            TransformShader = ModContent.Request<Effect>("TouhouPetsEx/Effects/Transform", AssetRequestMode.ImmediateLoad).Value;
            GrayishWhiteShader = ModContent.Request<Effect>("TouhouPetsEx/Effects/GrayishWhite", AssetRequestMode.ImmediateLoad).Value;
            TransparentHead = EquipLoader.AddEquipTexture(this, "TouhouPetsEx/Projectiles/DaiyouseiBoom", EquipType.Head, name: "TouhouPetsEx.TransparentHead");
            InverseColor = new BlendState()
            {
                Name = "BlendState.InverseColor",
                ColorDestinationBlend = Blend.InverseSourceColor,
                ColorSourceBlend = Blend.InverseDestinationColor,
                AlphaDestinationBlend = Blend.One,
                AlphaSourceBlend = Blend.Zero
            };
        }
        public override void PostSetupContent()
        {
            // 方便我排查有没有破坏性模组出现
            if (ModLoader.TryGetMod("TouhouPetsExOptimization", out Mod mod))
                Logger.Info($"侦测到 {mod.DisplayName} ，请不要将任何问题报告提交给 {DisplayName}");

			ColdProjAll.AddRange(ColdProjVanilla);
            ColdProjAll.AddRange(ContentSamples.ProjectilesByType.Where(kv => kv.Value.coldDamage && !kv.Value.hostile && kv.Value.friendly && kv.Key >= ProjectileID.Count).Select(kv => kv.Key));

            foreach (var kvp in ContentSamples.NpcsByNetId)
            {
                var entry = BestiaryDatabaseNPCsPopulator.FindEntryByNPCID(kvp.Key);
                if (kvp.Key >= 0 && !kvp.Value.boss && !kvp.Value.friendly && entry?.Info?.Contains(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean) == true)
                {
                    OceanEnemy.Add(kvp.Key);
                }
            }

            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(25))
                Main.instance.Window.Title = GetText("KoishiNo1");

            ArmorIDs.Head.Sets.DrawHead[TransparentHead] = false;
        }
        public override void Unload()
        {
            Instance = null;
            Config = null;
            WhitelistBlock = null;
            ColdProjVanilla = null;
            ColdProjAll = null;
            VanillaBug = null;
            VanillaGoldBug = null;
            RingShader = null;
            DistortShader = null;
            TransformShader = null;
            GrayishWhiteShader = null;
            InverseColor?.Dispose();
        }
        internal enum MessageType : byte
        {
            StatIncreasePlayerSync,
            Tp,
            SuperCrit,
            Weather
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            // 如果写的没问题的话，这里应该只有客户端在触发收包，服务器不参与

            MessageType msgType = (MessageType)reader.ReadByte();

            switch (msgType)
            {
                case MessageType.StatIncreasePlayerSync:
                    Player player = Main.player[reader.ReadByte()];
                    bool award = reader.ReadBoolean();

                    EnhancePlayers.ReceivePlayerSync(reader, player.whoAmI, award);

                    if (Main.netMode == NetmodeID.Server)
                        EnhancePlayers.AwardPlayerSync(this, -1, player.whoAmI, award);
                    break;

                case MessageType.Tp:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = GetPacket();
                        byte plr = reader.ReadByte();

                        packet.Write((byte)MessageType.Tp);
                        packet.Write(plr);
                        packet.WriteVector2(reader.ReadVector2());
                        packet.Send(-1, plr);
                        break;
                    }
                    else
                    {
                        EnhancePlayers.YukariTp(Main.player[reader.ReadByte()], reader.ReadVector2());
                    }
                    break;

                case MessageType.SuperCrit:
                    byte npc = reader.ReadByte();
                    Main.npc[npc].GetGlobalNPC<GEnhanceNPCs>().SuperCrit = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = GetPacket();

                        packet.Write((byte)MessageType.SuperCrit);
                        packet.Write(npc);
                        packet.Send(-1, whoAmI);
                    }
                    break;

                case MessageType.Weather:
                    Main.rainTime = reader.ReadDouble();
                    Main.maxRaining = reader.ReadSingle();
                    Main.raining = reader.ReadBoolean();
                    Main.windSpeedTarget = reader.ReadSingle();

                    if (Main.maxRaining != Main.oldMaxRaining)
                        Main.oldMaxRaining = Main.maxRaining;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = GetPacket();

                        packet.Write((byte)MessageType.Weather);
                        packet.Write(Main.rainTime);
                        packet.Write(Main.maxRaining);
                        packet.Write(Main.raining);
                        packet.Write(Main.windSpeedTarget);
                        packet.Send(-1, whoAmI);
                    }
                    break;
            }
        }
    }
}

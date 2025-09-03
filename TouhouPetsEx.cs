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
        public static Dictionary<int, BaseEnhance> GEnhanceInstances = [];
        public static TouhouPetsEx Instance;
        public static Effect RingShader;
        public static Effect DistortShader;
        public static BlendState InverseColor;
        public override void Load()
        {
            Instance = this;
            RingShader = ModContent.Request<Effect>("TouhouPetsEx/Effects/Ring", AssetRequestMode.ImmediateLoad).Value;
            DistortShader = ModContent.Request<Effect>("TouhouPetsEx/Effects/Distort", AssetRequestMode.ImmediateLoad).Value;
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
            ColdProjAll.AddRange(ColdProjVanilla);
            ColdProjAll.AddRange(ContentSamples.ProjectilesByType.Where(kv => kv.Value.coldDamage && !kv.Value.hostile && kv.Value.friendly && kv.Key >= ProjectileID.Count).Select(kv => kv.Key));

            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(25))
                Main.instance.Window.Title = GetText("KoishiNo1");
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
            InverseColor.Dispose();
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
            // ���д��û����Ļ�������Ӧ��ֻ�пͻ����ڴ����հ���������������

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

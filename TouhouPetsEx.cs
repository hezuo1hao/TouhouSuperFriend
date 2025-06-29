using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;

namespace TouhouPetsEx
{
	public class TouhouPetsEx : Mod
	{
        public static int[] WhitelistBlock = [ 2, 3, 3086, 3081, 169, 3271, 3272, 133, 176, 172, 593, 664, 9, 620, 619, 911, 2503, 2504, 1727, 4564, 586, 591, 1872];
        public static Dictionary<int, BaseEnhance> GEnhanceInstances = [];
        public static TouhouPetsEx Instance;
        public override void Load()
        {
            Instance = this;
        }
        public override void Unload()
        {
            Instance = null;
            Config = null;
            WhitelistBlock = null;
        }
        internal enum MessageType : byte
        {
            StatIncreasePlayerSync,
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
            }
        }
    }
}

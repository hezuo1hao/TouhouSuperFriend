using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using TouhouPetsEx.Enhance.Core;
using TouhouPets.Content.Items.PetItems;
using Terraria.ID;

namespace TouhouPetsEx
{
    public class TouhouPetsExModCommand : ModCommand
    {
        public override CommandType Type
            => CommandType.Chat;

        public override string Command
            => "give";

        public override string Usage => GetText("Give.Usage");

        public override string Description => GetText("Give.Description");

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // 不开能力就想用指令，报错！
            if (!caller.Player.HasEnhance<ShinkiHeart>())
                throw new UsageException(GetText("Give.Error_1"));

            // 一个参数都没有，报错！
            if (args.Length == 0)
                throw new UsageException(GetText("Give.Error_2"));

            // 尝试获取指令输入里有没有数字，没有意味着填写的是名称或是瞎几把填的
            if (!int.TryParse(args[0], out int type))
            {
                // 将填写的名称中的下划线替换成空格
                string name = args[0].Replace("_", " ");

                // 遍历所有物品，查询是否有符合当前名称的物品（不区分大小写）
                for (int k = 1; k < ItemLoader.ItemCount; k++)
                {
                    if (name.ToLower() == Lang.GetItemNameValue(k).ToLower())
                    {
                        type = k;
                        break;
                    }
                }
            }

            // 找不到对应的物品，报错！
            if (type <= 0 || type >= ItemLoader.ItemCount)
                throw new UsageException(GetText("Give.Error_3", ItemLoader.ItemCount));

            // 试图在月前给不在表里的东西，报错！
            if (!NPC.downedMoonlord && !TouhouPetsEx.WhitelistBlock.Contains(type))
                throw new UsageException(GetText("Give.Error_4"));

            // 试图给予怪东西，报错！(月后）
            Item item = ContentSamples.ItemsByType[type];
            if (item.createTile == -1 || !Main.tileSolid[item.createTile])
                throw new UsageException(GetText("Give.Error_5"));

            // 如果填了物品数量就获取，没填则默认为1
            int stack = 1;
            if (args.Length >= 2)
            {
                // 参数瞎几把填，报错！
                if (!int.TryParse(args[1], out stack))
                    throw new UsageException(GetText("Give.Error_6") + args[1]);
            }

            // 在输入命令的玩家身上生成物品
            caller.Player.QuickSpawnItem(new EntitySource_DebugCommand($"{nameof(TouhouPetsEx)}_{nameof(TouhouPetsExModCommand)}"), type, stack);
        }
    }
}

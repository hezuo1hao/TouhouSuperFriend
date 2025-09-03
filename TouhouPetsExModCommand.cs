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
            // ��������������ָ�����
            if (!caller.Player.HasEnhance<ShinkiHeart>())
                throw new UsageException(GetText("Give.Error_1"));

            // һ��������û�У�����
            if (args.Length == 0)
                throw new UsageException(GetText("Give.Error_2"));

            // ���Ի�ȡָ����������û�����֣�û����ζ����д�������ƻ���Ϲ�������
            if (!int.TryParse(args[0], out int type))
            {
                // ����д�������е��»����滻�ɿո�
                string name = args[0].Replace("_", " ");

                // ����������Ʒ����ѯ�Ƿ��з��ϵ�ǰ���Ƶ���Ʒ�������ִ�Сд��
                for (int k = 1; k < ItemLoader.ItemCount; k++)
                {
                    if (name.ToLower() == Lang.GetItemNameValue(k).ToLower())
                    {
                        type = k;
                        break;
                    }
                }
            }

            // �Ҳ�����Ӧ����Ʒ������
            if (type <= 0 || type >= ItemLoader.ItemCount)
                throw new UsageException(GetText("Give.Error_3", ItemLoader.ItemCount));

            // ��ͼ����ǰ�����ڱ���Ķ���������
            if (!NPC.downedMoonlord && !TouhouPetsEx.WhitelistBlock.Contains(type))
                throw new UsageException(GetText("Give.Error_4"));

            // ��ͼ����ֶ���������(�º�
            Item item = ContentSamples.ItemsByType[type];
            if (item.createTile == -1 || !Main.tileSolid[item.createTile])
                throw new UsageException(GetText("Give.Error_5"));

            // ���������Ʒ�����ͻ�ȡ��û����Ĭ��Ϊ1
            int stack = 1;
            if (args.Length >= 2)
            {
                // ����Ϲ���������
                if (!int.TryParse(args[1], out stack))
                    throw new UsageException(GetText("Give.Error_6") + args[1]);
            }

            // ������������������������Ʒ
            caller.Player.QuickSpawnItem(new EntitySource_DebugCommand($"{nameof(TouhouPetsEx)}_{nameof(TouhouPetsExModCommand)}"), type, stack);
        }
    }
}

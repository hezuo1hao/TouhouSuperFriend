using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using Terraria.ID;

namespace TouhouPetsEx.Enhance.Core
{
	public class EnhancePlayers : ModPlayer
    {
        public List<int> ActiveEnhance = [];
        public List<int> ActivePassiveEnhance = [];
        public int ActiveEnhanceCount = 11037;
        /// <summary>
        /// 萝莉丝用
        /// </summary>
        public int EatBook = 0;
        /// <summary>
        /// 大妖精用
        /// </summary>
        public int DaiyouseiCD = 0;
        /// <summary>
        /// 莉莉白用
        /// </summary>
        public int LilyCD = 0;
        /// <summary>
        /// 姬虫百百世用
        /// <para>索引决定对应的加成：0—移动速度、1—挖矿速度、2—最大氧气值、3—最大生命值、4—岩浆免疫时间、5—伤害减免、6—暴击伤害、7/8/9—运气、10—百分比穿甲</para>
        /// </summary>
        public int[] ExtraAddition = new int[11];
        /// <summary>
        /// 姬虫百百世用
        /// <para>索引决定对应的加成上限：0—移动速度、1—挖矿速度、2—最大氧气值、3—最大生命值、4—岩浆免疫时间、5—伤害减免、6—暴击伤害、7/8/9—运气、10—百分比穿甲</para>
        /// </summary>
        public static int[] ExtraAdditionMax = [50, 50, int.MaxValue, 100, int.MaxValue, 50, 200, 10, 4, 1, 150];
        private static void ProcessDemonismAction(Player player, Action<BaseEnhance> action)
        {
            if (!player.HasTouhouPetsBuff())
                return;

            foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
            {
                action(TouhouPetsEx.GEnhanceInstances[id]);
            }
        }
        private static bool? ProcessDemonismAction(Player player, bool? priority, Func<BaseEnhance, bool?> action)
        {
            if (priority == null)
            {
                bool? @return = null;
                foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    bool? a = action(TouhouPetsEx.GEnhanceInstances[id]);
                    if (a != null) @return = a;
                }
                return @return;
            }
            else
            {
                bool? @return = null;
                foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
                {
                    bool? a = action(TouhouPetsEx.GEnhanceInstances[id]);
                    if (a == priority) return a;
                    else if (a != null) @return = a;
                }
                return @return;
            }
        }
        public override void ResetEffects()
        {
            ActiveEnhanceCount = 1;

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerResetEffects(Player));

            ActivePassiveEnhance = [];
        }
        public override void SaveData(TagCompound tag)
        {
            List<string> strings = [];
            foreach (int type in ActiveEnhance)
            {
                strings.Add(ItemLoader.GetItem(type).Name);
            }
            tag.Add("ActiveEnhanceName", strings);
            tag.Add("EatBook", EatBook);
            tag.Add("ExtraAddition", ExtraAddition);
        }
        public override void LoadData(TagCompound tag)
        {
            List<int> ints = [];
            foreach (string name in tag.GetList<string>("ActiveEnhanceName"))
            {
                if (ModContent.TryFind("TouhouPets", name, out ModItem item))
                    ints.Add(item.Type);
            }
            ActiveEnhance = ints;
            EatBook = tag.GetInt("EatBook");
            if (tag.GetIntArray("ExtraAddition").Length != 0) ExtraAddition = tag.GetIntArray("ExtraAddition");
        }
        public override void ModifyLuck(ref float luck)
        {
            float luck2 = luck;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyLuck(Player, ref luck2));
            luck = luck2;
        }
        public override void PreUpdate()
        {
            foreach (Item item in Player.miscEquips)
            {
                if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance) && enhance.Passive)
                    ActivePassiveEnhance.Add(item.type);
            }

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPreUpdate(Player));
        }
        public override void PostUpdateEquips()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdateEquips(Player));
        }
        public override void PostUpdate()
        {
            if (Main.GameUpdateCount % 18000 == 12000 && Player == Main.LocalPlayer)
                AwardPlayerSync(Mod, -1, Main.myPlayer);

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdate(Player));
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            float r2 = r;
            float g2 = g;
            float b2 = b;
            float a2 = a;
            bool fullBright2 = fullBright;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerDrawEffects(drawInfo, ref r2, ref g2, ref b2, ref a2, ref fullBright2));
            r = r2;
            g = g2;
            b = b2;
            a = a2;
            fullBright = fullBright2;
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            bool? reesult = ProcessDemonismAction(Player, true, (enhance) => enhance.PlayerFreeDodge(Player, info));

            return reesult ?? base.FreeDodge(info);
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            Player.HurtModifiers modifiers2 = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHurt(Player, ref modifiers2));
            modifiers = modifiers2;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            NPC.HitModifiers modifiers2 = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHitNPC(Player, target, ref modifiers2));
            modifiers = modifiers2;
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            bool playSound2 = playSound;
            bool genDust2 = genDust;
            PlayerDeathReason damageSource2 = damageSource;
            bool? result = ProcessDemonismAction(Player, false, (enhance) => enhance.PlayerPreKill(Player, damage, hitDirection, pvp, ref playSound2, ref genDust2, ref damageSource2));
            playSound = playSound2;
            genDust = genDust2;
            damageSource = damageSource2;

            return result ?? base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerKill(Player, damage, hitDirection, pvp, damageSource));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerOnHitNPC(Player, target, hit, damageDone));
        }
        public override void OnEnterWorld()
        {
            if (Player == Main.LocalPlayer)
                AwardPlayerSync(Mod, -1, Main.myPlayer, true);
        }

        public static void AwardPlayerSync(Mod mod, int toWho, int fromWho, bool rebate = false)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            Player plr = Main.player[fromWho];
            EnhancePlayers player = plr.MP();
            ModPacket packet = mod.GetPacket();

            packet.Write((byte)TouhouPetsEx.MessageType.StatIncreasePlayerSync);
            packet.Write((byte)plr.whoAmI);
            packet.Write(rebate);
            packet.Write(player.EatBook);
            packet.Write(player.ActiveEnhance.Count);
            for (int i = 0; i < player.ActiveEnhance.Count; i++)
                packet.Write(player.ActiveEnhance[i]);
            packet.Write(player.ExtraAddition.Length);
            for (int i = 0; i < player.ExtraAddition.Length; i++)
                packet.Write(player.ExtraAddition[i]);
            packet.Send(toWho, fromWho);

            //Main.NewText("发");
        }
        public static void ReceivePlayerSync(BinaryReader reader, int whoAmI, bool award)
        {
            if (whoAmI == Main.myPlayer)
                return;

            Player plr = Main.player[whoAmI];
            EnhancePlayers player = plr.MP();

            player.EatBook = reader.ReadInt32();

            int activeEnhanceCount = reader.ReadInt32();
            List<int> activeEnhance = [];
            for (int i = 0;i < activeEnhanceCount;i++)
                activeEnhance.Add(reader.ReadInt32());
            player.ActiveEnhance = activeEnhance;

            int extraAdditionLength = reader.ReadInt32();
            int[] extraAddition = new int[extraAdditionLength];
            for (int i = 0;i < extraAdditionLength;i++)
                extraAddition[i] = reader.ReadInt32();
            player.ExtraAddition = extraAddition;

            if (award)
                AwardPlayerSync(TouhouPetsEx.Instance, whoAmI, Main.myPlayer);

            //Main.NewText("收");
        }
    }
}

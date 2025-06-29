using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TouhouPetsEx.Enhance.Core
{
	public class BaseEnhance
	{
        /// <summary>
        /// ����
        /// </summary>
        public virtual string Text => "";
        /// <summary>
        /// ʵ�����淨����
        /// </summary>
        public virtual string ExperimentalText => "";
        /// <summary>
        /// �Ƿ������Ҽ���Ĭ������
        /// </summary>
        public virtual bool EnableRightClick => true;
        /// <summary>
        /// �Ƿ�Ϊ����������Я���ڱ��������/��������������Ч����Ĭ��Ϊ��
        /// </summary>
        public virtual bool Passive => false;
        /// <summary>
        /// �Ƿ������˶�Ӧ������ʵ�����淨
        /// </summary>
        public virtual bool Experimental
        {
            get { return false; }
        }
        /// <summary>
        /// ��ӱ���ǿ���Ӧ��Ʒ֮�����ϵ
        /// </summary>
        /// <param name="type">��Ӧ��Ʒ��type</param>
        public void AddEnhance(int type)
        {
            TouhouPetsEx.GEnhanceInstances[type] = this;
        }
        public virtual void ItemSSD()
        {

        }
        public virtual void ItemSD(Item item)
        {

        }
        public virtual void ItemHoldItem(Item item, Player player)
        {

        }
        public virtual void ItemUpdateInventory(Item item, Player player)
        {

        }
        public virtual void ItemModifyTooltips(Item item, List<TooltipLine> tooltips)
        {

        }
        public virtual bool? ItemUseItem(Item item, Player player)
        {
            return null;
        }
        public virtual void PlayerResetEffects(Player player)
        {

        }
        public virtual void PlayerPreUpdate(Player player)
        {

        }
        public virtual void PlayerPostUpdate(Player player)
        {

        }
        public virtual void PlayerPostUpdateEquips(Player player)
        {

        }
        public virtual void PlayerDrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {

        }
        public virtual bool? PlayerFreeDodge(Player player, Player.HurtInfo info)
        {
            return null;
        }
        public virtual void PlayerModifyHurt(Player player, ref Player.HurtModifiers modifiers)
        {

        }
        public virtual void PlayerModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public virtual void PlayerModifyLuck(Player player, ref float luck)
        {

        }
        public virtual void PlayerOnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public virtual bool? PlayerPreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            return null;
        }
        public virtual void PlayerKill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            
        }
        public virtual void NPCAI(NPC npc)
        {

        }
    }
}

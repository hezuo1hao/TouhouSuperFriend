using Microsoft.Xna.Framework.Graphics;
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
        /// ʵ�����淨��������ȷ�������� Experimental һ��
        /// <br>���ĳһ����Ϊ""�򵱶�Ӧʵ�����淨����ʱ��������ʾ������</br>
        /// </summary>
        public virtual string[] ExperimentalText => [];
        /// <summary>
        /// �Ƿ������Ҽ���Ĭ������
        /// </summary>
        public virtual bool EnableRightClick => true;
        /// <summary>
        /// �Ƿ�Ϊ����������Я���ڱ��������/��������������Ч����Ĭ��Ϊ��
        /// </summary>
        public virtual bool Passive => false;
        /// <summary>
        /// �Ƿ������˶�Ӧ������ʵ�����淨����ȷ�������� ExperimentalText һ��
        /// </summary>
        public virtual bool[] Experimental => [];
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
        public virtual bool? ItemCanUseItem(Item item, Player player, ref bool def)
        {
            return null;
        }
        public virtual bool? ItemUseItem(Item item, Player player)
        {
            return null;
        }
        public virtual void PlayerInitialize(Player player)
        {

        }
        public virtual void PlayerResetEffects(Player player)
        {

        }
        public virtual void PlayerResetEffectsAlways(Player player)
        {

        }
        public virtual void PlayerPreUpdate(Player player)
        {

        }
        public virtual void PlayerPreUpdateBuffs(Player player)
        {

        }
        public virtual void PlayerPreUpdateBuffsAlways(Player player)
        {

        }
        public virtual void PlayerPostUpdateBuffs(Player player)
        {

        }
        public virtual void PlayerPostUpdateEquips(Player player)
        {

        }
        public virtual void PlayerPostUpdate(Player player)
        {

        }
        public virtual void PlayerModifyItemScale(Player player, Item item, ref float scale)
        {

        }
        public virtual float? PlayerUseTimeMultiplier(Player player, Item item)
        {
            return null;
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
        public virtual void PlayerModifyHitNPCWithItem(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public virtual void PlayerModifyHitNPCWithProjectile(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public virtual void PlayerModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public virtual void PlayerModifyLuck(Player player, ref float luck)
        {

        }
        public virtual void PlayerUpdateLifeRegen(Player player)
        {
        
        }
        public virtual void PlayerGetHealLife(Player player, Item item, bool quickHeal, ref int healValue)
        {

        }
        public virtual void PlayerGetHealMana(Player player, Item item, bool quickHeal, ref int healValue)
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
        public virtual void TileDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {

        }
        public virtual void TileRandomUpdate(int i, int j, int type)
        {

        }
        public virtual void ModifyBuffText(Player player, int type, ref string buffName, ref string tip, ref int rare)
        {

        }
        public virtual void BuffUpdate(int type, Player player, ref int buffIndex)
        {

        }
        public virtual void SystemPostSetupContent()
        {

        }
        public virtual void SystemPostUpdateNPCs()
        {

        }
        public virtual void SystemPostAddRecipes()
        {

        }
        public virtual void SystemPostUpdateEverything()
        {

        }
    }
}

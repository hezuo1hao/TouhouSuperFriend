using Microsoft.Xna.Framework;
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
        /// 描述
        /// </summary>
        public virtual string Text => "";
        /// <summary>
        /// 实验性玩法描述，需确保长度与 <see cref="Experimental"/> 一致
        /// <br>如果某一索引为""则当对应实验性玩法启动时并不会显示新描述</br>
        /// </summary>
        public virtual string[] ExperimentalText => [];
        /// <summary>
        /// 是否启用右键，默认启用
        /// </summary>
        public virtual bool EnableRightClick => true;
        /// <summary>
        /// 是否为被动能力（携带在背包或宠物/照明宠物栏即生效），默认为否
        /// </summary>
        public virtual bool Passive => false;
        /// <summary>
        /// 是否启用了对应能力的实验性玩法，需确保长度与 <see cref="ExperimentalText"/> 一致
        /// </summary>
        public virtual bool[] Experimental => [];
        /// <summary>
        /// 关闭所给物品的增益能力描述
        /// </summary>
        public HashSet<int> BanTootips = [];
        /// <summary>
        /// 添加本增强与对应物品之间的联系
        /// </summary>
        /// <param name="type">对应物品的type</param>
        public void AddEnhance(int type)
        {
            TouhouPetsEx.GEnhanceInstances[type] = this;
        }
        public void AddBanTootips(HashSet<int> ints)
        {
            BanTootips = ints;
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
        public virtual bool? ItemPreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return null;
        }
        public virtual void ItemPostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {

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
        public virtual void PlayerPostResetEffects(Player player)
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
        public virtual void PlayerPostHurt(Player player, Player.HurtInfo info)
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
        public virtual bool? NPCPreAI(NPC npc)
        {
            return null;
        }
        public virtual void NPCAI(NPC npc)
        {

        }
        public virtual bool? NPCCanHitNPC(NPC npc, NPC target)
        {
            return null;
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
        public virtual void SystemModifyLightingBrightness(ref float scale)
        {

        }
        public virtual void SystemPostSetupContent()
        {

        }
        public virtual void SystemPreUpdateGores()
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

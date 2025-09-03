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
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static TouhouPetsEx.TouhouPetsEx;

namespace TouhouPetsEx.Enhance.Core
{
	public class EnhanceBuffPlayers : ModPlayer
    {
        /// <summary>
        /// ����Buff�ã���˹���������أ�
        /// </summary>
        public bool Glutton = false;
        /// <summary>
        /// ����Buff�ã���˹���������أ�
        /// </summary>
        public bool Patience = false;
        /// <summary>
        /// Ͷ��Buff�ã���˹���������أ�
        /// </summary>
        public bool Throw = false;
        /// <summary>
        /// ��˹�����
        /// </summary>
        public float? OldLifeRegenTime = null;
        public override void ResetEffects()
        {
            if (!Patience)
                OldLifeRegenTime = null;

            Glutton = false;
            Patience = false;
            Throw = false;
        }
        public override void PreUpdate()
        {
            if (Patience)
            {
                if (OldLifeRegenTime != null && OldLifeRegenTime > Player.lifeRegenTime)
                    Player.lifeRegenTime = OldLifeRegenTime.Value;

                OldLifeRegenTime = Player.lifeRegenTime;
            }
        }
        public override void UpdateLifeRegen()
        {
            if (Patience && OldLifeRegenTime != null && OldLifeRegenTime > Player.lifeRegenTime)
            {
                Player.lifeRegenTime = OldLifeRegenTime.Value;
                Player.bleed = false;
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ModifyHitInfo += (ref NPC.HitInfo a) => SuperCrit(ref a, target);
        }

        private void SuperCrit(ref NPC.HitInfo info, NPC npc)
        {
            if (info.Crit && Player.MBP().Glutton && Main.rand.NextBool(25))
            {
                info.Damage *= 2;

                if (!info.HideCombatText && !info.InstantKill && npc.lifeMax > 1 && !npc.HideStrikeDamage)
                {
                    npc.GetGlobalNPC<GEnhanceNPCs>().SuperCrit = true;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return;

                    ModPacket packet = Mod.GetPacket();

                    packet.Write((byte)MessageType.SuperCrit);
                    packet.Write(npc.whoAmI);
                    packet.Send(-1, Player.whoAmI);
                }
            }
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using TouhouPetsEx.Enhance.Core;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx
{
    public class TouhouPetsExModSystem : ModSystem
    {
        public static int SynchronousTime;
        public static ModKeybind ReisenKeyBind { get; private set; }
        public static ModKeybind KoishiKeyBind { get; private set; }
        public static bool[] SakuyaStoppedNPC { get; private set; }
        private static List<EnhancementId> EnhanceCountKeysCache;

        public override void Load()
        {
            ReisenKeyBind = KeybindLoader.RegisterKeybind(Mod, "ReisenKeyBind", "V");
            KoishiKeyBind = KeybindLoader.RegisterKeybind(Mod, "KoishiKeyBind", "C");
            EnhanceCount = [];
            SakuyaStoppedNPC = new bool[Main.maxNPCs];
            EnhanceCountKeysCache = null;
        }

        public override void Unload()
        {
            ReisenKeyBind = null;
            KoishiKeyBind = null;
            EnhanceCount = null;
            SakuyaStoppedNPC = null;
            EnhanceCountKeysCache = null;
            EnhanceHookRegistry.Clear();
            EnhanceRegistry.Clear();
        }
        public override void PreUpdateTime()
        {
            SynchronousTime++;

            foreach (CombatText text in Main.combatText)
            {
                if (!text.active ||  text.lifeTime % 10 != 0)
                    continue;

                if (text.color == new Color(133, 0, 133))
                {
                    text.rotation *= -1;
                    text.scale += 0.1f;
                    text.color = new Color(166, 0, 166);
                    continue;
                }

                if (text.color == new Color(166, 0, 166))
                {
                    text.rotation *= -1;
                    text.scale -= 0.1f;
                    text.color = new Color(133, 0, 133);
                    continue;
                }
            }
        }
        public override void PreUpdatePlayers()
        {
            if (EnhanceCountKeysCache == null || EnhanceCountKeysCache.Count != EnhanceCount.Count)
                EnhanceCountKeysCache = new List<EnhancementId>(EnhanceCount.Keys);

            for (int i = 0; i < EnhanceCountKeysCache.Count; i++)
                EnhanceCount[EnhanceCountKeysCache[i]] = 0;

            foreach (Player player in Main.ActivePlayers)
            {
                EnhancePlayers mp = player.MP();
                if (mp == null)
                    continue;

                for (int i = 0; i < mp.ActiveEnhance.Count; i++)
                {
                    EnhancementId enhanceId = mp.ActiveEnhance[i];
                    EnhanceCount[enhanceId] = EnhanceCount.TryGetValue(enhanceId, out int value) ? value + 1 : 1;
                }

                for (int i = 0; i < mp.ActivePassiveEnhance.Count; i++)
                {
                    EnhancementId enhanceId = mp.ActivePassiveEnhance[i];
                    EnhanceCount[enhanceId] = EnhanceCount.TryGetValue(enhanceId, out int value) ? value + 1 : 1;
                }
            }
        }

        public override void PreUpdateNPCs()
        {
            if (SakuyaStoppedNPC == null || SakuyaStoppedNPC.Length != Main.maxNPCs)
                SakuyaStoppedNPC = new bool[Main.maxNPCs];

            Array.Clear(SakuyaStoppedNPC, 0, SakuyaStoppedNPC.Length);

            int perfectMaidType = ModContent.ProjectileType<PerfectMaid>();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != perfectMaidType)
                    continue;

                int npcIndex = (int)proj.ai[1];
                if ((uint)npcIndex < (uint)SakuyaStoppedNPC.Length)
                    SakuyaStoppedNPC[npcIndex] = true;
            }
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(SynchronousTime);
        }
        public override void NetReceive(BinaryReader reader)
        {
            SynchronousTime = reader.ReadInt32();
        }
    }
}

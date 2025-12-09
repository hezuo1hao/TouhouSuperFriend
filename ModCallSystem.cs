using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using ReLogic.Content;
using System.Linq;
using Terraria.GameContent;
using Terraria.Utilities;
using TouhouPets.Content.Projectiles.Pets;
using TouhouPets;
using Terraria.ID;

namespace TouhouPetsEx
{

    public class ModCallSystem : ModSystem
    {
        private static bool LookStar => ((!Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight && Main.cloudAlpha <= 0) || Main.LocalPlayer.ZoneSkyHeight) && !Main.bloodMoon;
        public override void PostSetupContent()
        {
            #region 东方小伙伴
            if (ModLoader.TryGetMod("TouhouPets", out Mod TouhouPets))
            {
                // 常规对话及聊天室
                int[] keyCount = new int[62];
                int koishi = ModContent.ProjectileType<Koishi>();
                int satori = ModContent.ProjectileType<Satori>();
                int rin = ModContent.ProjectileType<Rin>();
                int utsuho = ModContent.ProjectileType<Utsuho>();
                int kokoro = ModContent.ProjectileType<Kokoro>();
                int marisa = ModContent.ProjectileType<Marisa>();
                int alice = ModContent.ProjectileType<Alice>();
                int reimu = ModContent.ProjectileType<Reimu>();

                // 特别的，给魔理沙加了一句通用的，恋恋存在时所说的对boss对话
                foreach (NPC npc in ContentSamples.NpcsByNetId.Values)
                {
                    if (npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type])
                        TouhouPets.Call("PetsReactionToBoss", Mod, npc.type, 25, Language.GetText("Mods.TouhouPetsEx.TouhouPets.Response.Marisa_12"), () => HasPets(koishi) && LocalConfig.MarisaKoishi && Main.rand.NextBool(3), 1);
                }

                for (int i = 1; i < keyCount.Length; i++)
                    keyCount[i] = 1;

                // 地灵殿全员 或 只有恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => ((HasPets(satori, rin, utsuho) && NotHasPets(kokoro, marisa)) || NotHasPets(satori, rin, utsuho, kokoro, marisa)) && LookStar);
                // 恋恋、小五
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori) && NotHasPets(rin, utsuho, kokoro, marisa) && LookStar);
                // 恋恋、阿燐（虽然不可能触发，但是我已经写完了，留着）
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(rin) && NotHasPets(satori, utsuho, kokoro, marisa) && LookStar && !LocalConfig.Box5);
                // 新地灵殿一家：恋恋、阿燐（虽然不可能触发，但是我已经写完了，留着）
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(rin) && NotHasPets(satori, utsuho, kokoro, marisa) && LookStar && LocalConfig.Box5);
                // 恋恋、阿空
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(utsuho) && NotHasPets(satori, rin, kokoro, marisa) && LookStar && !LocalConfig.Box5);
                // 新地灵殿一家：恋恋、阿空
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(utsuho) && NotHasPets(satori, rin, kokoro, marisa) && LookStar && LocalConfig.Box5);
                // 恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(kokoro) && NotHasPets(satori, rin, utsuho, marisa) && LookStar && !LocalConfig.Box5);
                // 新地灵殿一家：恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(kokoro) && NotHasPets(satori, rin, utsuho, marisa) && LookStar && LocalConfig.Box5);
                // 新地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori, rin, utsuho, kokoro) && LookStar && LocalConfig.Box5);
                // 恋恋、魔理沙
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(marisa) && NotHasPets(satori, rin, utsuho, kokoro) && LookStar && LocalConfig.MarisaKoishi);
                // 秦心、恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Kokoro, "Kokoro", ref keyCount, () => HasPets(koishi));
                // 魔理沙、恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Marisa, "Marisa", ref keyCount, () => HasPets(koishi) && NotHasPets(satori, rin, utsuho, kokoro) && LocalConfig.MarisaKoishi);
                // 地灵殿全员 或 只有恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => ((HasPets(satori, rin, utsuho) && NotHasPets(kokoro, marisa)) || NotHasPets(satori, rin, utsuho, kokoro, marisa)));
                // 恋恋、小五
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori) && NotHasPets(rin, utsuho, kokoro, marisa));
                // 恋恋、阿空
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(utsuho) && NotHasPets(satori, rin, kokoro, marisa));
                // 恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(kokoro) && NotHasPets(satori, rin, utsuho, marisa) && !LocalConfig.Box5);
                // 新地灵殿一家：恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(kokoro) && NotHasPets(satori, rin, utsuho, marisa) && LocalConfig.Box5);
                // 新地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori, rin, utsuho, kokoro) && LocalConfig.Box5);
                // 恋恋、魔理沙
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(marisa) && NotHasPets(satori, rin, utsuho, kokoro) && LocalConfig.MarisaKoishi);
                // 小五、魔理沙（不可能触发，乐）
                TouhouPetsAdd(TouhouPets, TouhouPetID.Satori, "Satori", ref keyCount, () => HasPets(marisa) && NotHasPets(koishi) && LocalConfig.MarisaKoishi);
                // 小五、魔理沙、恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Satori, "Satori", ref keyCount, () => HasPets(marisa, koishi) && LocalConfig.MarisaKoishi);
                // 魔理沙
                TouhouPetsAdd(TouhouPets, TouhouPetID.Marisa, "Marisa", ref keyCount, () => (!LocalConfig.MarisaKoishi || NotHasPets(koishi)) && NotHasPets(alice, reimu));
                // 魔理沙、爱丽丝
                TouhouPetsAdd(TouhouPets, TouhouPetID.Marisa, "Marisa", ref keyCount, () => HasPets(alice));
                // 魔理沙、灵梦
                TouhouPetsAdd(TouhouPets, TouhouPetID.Marisa, "Marisa", ref keyCount, () => HasPets(reimu));
                // 魔理沙、恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Marisa, "Marisa", ref keyCount, () => HasPets(koishi) && NotHasPets(satori) && LocalConfig.MarisaKoishi);
                // 魔理沙、恋恋、小五
                TouhouPetsAdd(TouhouPets, TouhouPetID.Marisa, "Marisa", ref keyCount, () => HasPets(koishi, satori) && LocalConfig.MarisaKoishi);
                keyCount[(int)TouhouPetID.Koishi] += 3;
                // 恋恋、魔理沙
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(marisa) && LocalConfig.MarisaKoishi);
                // 地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => ((HasPets(satori, rin, utsuho) && NotHasPets(kokoro, marisa) && !LocalConfig.Box5 && !LocalConfig.MarisaKoishi)));
                // 新地灵殿一家：地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => ((HasPets(satori, rin, utsuho) && NotHasPets(kokoro, marisa) && LocalConfig.Box5 && !LocalConfig.MarisaKoishi)));
                // 纯粹享乐的二人：地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => ((HasPets(satori, rin, utsuho) && NotHasPets(kokoro, marisa) && !LocalConfig.Box5 && LocalConfig.MarisaKoishi)));
                // 新地灵殿一家、纯粹享乐的二人：地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => ((HasPets(satori, rin, utsuho) && NotHasPets(kokoro, marisa) && LocalConfig.Box5 && LocalConfig.MarisaKoishi)));
                // 恋恋、小五
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori) && NotHasPets(rin, utsuho, kokoro, marisa) && !LocalConfig.Box5);
                // 新地灵殿一家：恋恋、小五
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori) && NotHasPets(rin, utsuho, kokoro, marisa) && LocalConfig.Box5);
                // 恋恋、阿空
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(utsuho) && NotHasPets(satori, rin, kokoro, marisa));
                // 恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(kokoro) && NotHasPets(satori, rin, utsuho, marisa) && !LocalConfig.Box5);
                // 新地灵殿一家：恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(kokoro) && NotHasPets(satori, rin, utsuho, marisa) && LocalConfig.Box5);
                // 新地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori, rin, utsuho, kokoro) && LocalConfig.Box5);
                // 恋恋、魔理沙
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(marisa) && NotHasPets(satori, rin, utsuho, kokoro) && LocalConfig.MarisaKoishi);
                // 地灵殿全员 或 只有恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => ((HasPets(satori, rin, utsuho) && NotHasPets(kokoro, marisa)) || NotHasPets(satori, rin, utsuho, kokoro, marisa)));
                // 恋恋、小五
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori) && NotHasPets(rin, utsuho, kokoro, marisa));
                // 恋恋、阿空
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(utsuho) && NotHasPets(satori, rin, kokoro, marisa));
                // 恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(kokoro) && NotHasPets(satori, rin, utsuho, marisa));
                // 新地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori, rin, utsuho, kokoro) && LocalConfig.Box5);
                // 恋恋、魔理沙
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(marisa) && NotHasPets(satori, rin, utsuho, kokoro) && LocalConfig.MarisaKoishi);
                // 小五、恋恋
                TouhouPetsAdd(TouhouPets, TouhouPetID.Satori, "Satori", ref keyCount, () => HasPets(koishi) && (!LocalConfig.Box5 || NotHasPets(kokoro)));
                // 新地灵殿一家：小五、恋恋、秦心
                TouhouPetsAdd(TouhouPets, TouhouPetID.Satori, "Satori", ref keyCount, () => HasPets(koishi, kokoro) && LocalConfig.Box5);
                keyCount[(int)TouhouPetID.Koishi] += 11;
                // 恋恋、小五
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori) && NotHasPets(rin, utsuho, kokoro, marisa));
                // 新地灵殿全员
                TouhouPetsAdd(TouhouPets, TouhouPetID.Koishi, "Koishi", ref keyCount, () => HasPets(satori, rin, utsuho, kokoro) && LocalConfig.Box5);

                List<(TouhouPetID, string, int)> chatRoom1 =
                [
                    (TouhouPetID.Koishi, "Koishi_1", -1),
                    (TouhouPetID.Satori, "Satori_1", 0),
                    (TouhouPetID.Rin, "Rin_1", 1),
                    (TouhouPetID.Koishi, "Koishi_1", 2),
                    (TouhouPetID.Rin, "Rin_2", 3),
                    (TouhouPetID.Satori, "Satori_2", 4),
                    (TouhouPetID.Satori, "Satori_3", 5),
                    (TouhouPetID.Utsuho, "Utsuho_1", 6),
                    (TouhouPetID.Koishi, "Koishi_17", 7),
                    (TouhouPetID.Satori, "Satori_4", 8),
                    (TouhouPetID.Utsuho, "Utsuho_2", 9),
                    (TouhouPetID.Rin, "Rin_3", 10),
                    (TouhouPetID.Rin, "Rin_4", 11),
                    (TouhouPetID.Koishi, "Koishi_18", 12),
                    (TouhouPetID.Satori, "Satori_5", 13),
                    (TouhouPetID.Koishi, "Koishi_19", 14),
                    (TouhouPetID.Koishi, "Koishi_20", 15),
                    (TouhouPetID.Utsuho, "Utsuho_3", 16),
                    (TouhouPetID.Rin, "Rin_5", 17),
                    (TouhouPetID.Utsuho, "Utsuho_4", 18),
                    (TouhouPetID.Satori, "Satori_6", 19),
                    (TouhouPetID.Satori, "Satori_7", 20),
                    (TouhouPetID.Koishi, "Koishi_21", 21),
                    (TouhouPetID.Rin, "Rin_6", 21),
                    (TouhouPetID.Utsuho, "Utsuho_5", 21),
                    (TouhouPetID.Satori, "Satori_8", 22),
                ];

                List<(TouhouPetID, string, int)> chatRoom2 =
                [
                    (TouhouPetID.Koishi, "Koishi_2", -1),
                    (TouhouPetID.Koishi, "Koishi_1", 0),
                    (TouhouPetID.Satori, "Satori_10", 1),
                    (TouhouPetID.Koishi, "Koishi_22", 2),
                    (TouhouPetID.Satori, "Satori_9", 3),
                    (TouhouPetID.Koishi, "Koishi_21", 4),
                ];

                List<(TouhouPetID, string, int)> chatRoom3 =
                [
                    (TouhouPetID.Koishi, "Koishi_3", -1),
                    (TouhouPetID.Rin, "Rin_1", 0),
                    (TouhouPetID.Koishi, "Koishi_1", 1),
                    (TouhouPetID.Rin, "Rin_7", 2),
                    (TouhouPetID.Koishi, "Koishi_23", 3),
                    (TouhouPetID.Koishi, "Koishi_24", 4),
                    (TouhouPetID.Rin, "Rin_8", 5),
                ];

                List<(TouhouPetID, string, int)> chatRoom3_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_4", -1),
                    (TouhouPetID.Rin, "Rin_1", 0),
                    (TouhouPetID.Koishi, "Koishi_1", 1),
                    (TouhouPetID.Rin, "Rin_7", 2),
                    (TouhouPetID.Koishi, "Koishi_23", 3),
                    (TouhouPetID.Koishi, "Koishi_24_1", 4),
                    (TouhouPetID.Rin, "Rin_8", 5),
                ];

                List<(TouhouPetID, string, int)> chatRoom4 =
                [
                    (TouhouPetID.Koishi, "Koishi_5", -1),
                    (TouhouPetID.Koishi, "Koishi_1", 0),
                    (TouhouPetID.Utsuho, "Utsuho_6", 1),
                    (TouhouPetID.Utsuho, "Utsuho_7", 2),
                    (TouhouPetID.Koishi, "Koishi_25", 3),
                    (TouhouPetID.Koishi, "Koishi_26", 4),
                    (TouhouPetID.Koishi, "Koishi_27", 5),
                    (TouhouPetID.Koishi, "Koishi_28", 1),
                    (TouhouPetID.Utsuho, "Utsuho_8", 1),
                ];

                List<(TouhouPetID, string, int)> chatRoom4_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_6", -1),
                    (TouhouPetID.Koishi, "Koishi_1", 0),
                    (TouhouPetID.Utsuho, "Utsuho_6", 1),
                    (TouhouPetID.Utsuho, "Utsuho_7", 2),
                    (TouhouPetID.Koishi, "Koishi_25", 3),
                    (TouhouPetID.Koishi, "Koishi_26", 4),
                    (TouhouPetID.Koishi, "Koishi_27_1", 5),
                    (TouhouPetID.Koishi, "Koishi_28", 1),
                    (TouhouPetID.Utsuho, "Utsuho_8", 1),
                ];

                List<(TouhouPetID, string, int)> chatRoom5 =
                [
                    (TouhouPetID.Koishi, "Koishi_7", -1),
                    (TouhouPetID.Kokoro, "Kokoro_9", 0),
                    (TouhouPetID.Koishi, "Koishi_1", 1),
                    (TouhouPetID.Kokoro, "Kokoro_10", 2),
                    (TouhouPetID.Koishi, "Koishi_29", 3),
                    (TouhouPetID.Kokoro, "Kokoro_11", 4),
                    (TouhouPetID.Koishi, "Koishi_30", 5),
                ];

                List<(TouhouPetID, string, int)> chatRoom5_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_8", -1),
                    (TouhouPetID.Kokoro, "Kokoro_9", 0),
                    (TouhouPetID.Koishi, "Koishi_1", 1),
                    (TouhouPetID.Kokoro, "Kokoro_10", 2),
                    (TouhouPetID.Koishi, "Koishi_29", 3),
                    (TouhouPetID.Kokoro, "Kokoro_11_1", 4),
                    (TouhouPetID.Koishi, "Koishi_30", 5),
                ];

                List<(TouhouPetID, string, int)> chatRoom6 =
                [
                    (TouhouPetID.Koishi, "Koishi_9", -1),
                    (TouhouPetID.Satori, "Satori_1", 0),
                    (TouhouPetID.Rin, "Rin_1", 1),
                    (TouhouPetID.Kokoro, "Kokoro_9", 2),
                    (TouhouPetID.Koishi, "Koishi_1", 3),
                    (TouhouPetID.Utsuho, "Utsuho_9", 4),
                    (TouhouPetID.Utsuho, "Utsuho_10", 5),
                    (TouhouPetID.Kokoro, "Kokoro_12", 6),
                    (TouhouPetID.Utsuho, "Utsuho_11", 7),
                    (TouhouPetID.Rin, "Rin_9", 7),
                    (TouhouPetID.Rin, "Rin_10", 8),
                    (TouhouPetID.Satori, "Satori_11", 9),
                    (TouhouPetID.Kokoro, "Kokoro_13", 10),
                    (TouhouPetID.Koishi, "Koishi_31", 11),
                    (TouhouPetID.Koishi, "Koishi_32", 12),
                    (TouhouPetID.Kokoro, "Kokoro_14", 13),
                    (TouhouPetID.Utsuho, "Utsuho_12", 14),
                    (TouhouPetID.Rin, "Rin_11", 15),
                    (TouhouPetID.Koishi, "Koishi_33", 15),
                    (TouhouPetID.Satori, "Satori_12", 15),
                    (TouhouPetID.Satori, "Satori_13", 16),
                    (TouhouPetID.Koishi, "Koishi_21", 17),
                    (TouhouPetID.Rin, "Rin_12", 17),
                    (TouhouPetID.Utsuho, "Utsuho_5", 17),
                    (TouhouPetID.Kokoro, "Kokoro_15", 17),
                    (TouhouPetID.Rin, "Rin_13", 18),
                    (TouhouPetID.Utsuho, "Utsuho_13", 19),
                    (TouhouPetID.Koishi, "Koishi_34", 20),
                    (TouhouPetID.Satori, "Satori_14", 21),
                    (TouhouPetID.Kokoro, "Kokoro_16", 22),
                    (TouhouPetID.Rin, "Rin_14", 23),
                    (TouhouPetID.Kokoro, "Kokoro_17", 24),
                ];

                List<(TouhouPetID, string, int)> chatRoom7 =
                [
                    (TouhouPetID.Koishi, "Koishi_10", -1),
                    (TouhouPetID.Koishi, "Koishi_1", 0),
                    (TouhouPetID.Marisa, "Marisa_1", 1),
                    (TouhouPetID.Koishi, "Koishi_2", 2),
                    (TouhouPetID.Marisa, "Marisa_2", 3),
                    (TouhouPetID.Koishi, "Koishi_3", 4),
                    (TouhouPetID.Marisa, "Marisa_3", 5),
                    (TouhouPetID.Koishi, "Koishi_4", 6),
                    (TouhouPetID.Koishi, "Koishi_5", 7),
                    (TouhouPetID.Marisa, "Marisa_4", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom8 =
                [
                    (TouhouPetID.Kokoro, "Kokoro_1", -1),
                    (TouhouPetID.Kokoro, "Kokoro_1", 0),
                    (TouhouPetID.Koishi, "Koishi_6", 1),
                    (TouhouPetID.Koishi, "Koishi_7", 2),
                    (TouhouPetID.Kokoro, "Kokoro_2", 3),
                    (TouhouPetID.Kokoro, "Kokoro_3", 4),
                    (TouhouPetID.Koishi, "Koishi_8", 5),
                    (TouhouPetID.Kokoro, "Kokoro_4", 6),
                    (TouhouPetID.Koishi, "Koishi_9", 7),
                    (TouhouPetID.Kokoro, "Kokoro_5", 8),
                    (TouhouPetID.Koishi, "Koishi_10", 9),
                    (TouhouPetID.Kokoro, "Kokoro_6", 10),
                    (TouhouPetID.Kokoro, "Kokoro_7", 11),
                    (TouhouPetID.Koishi, "Koishi_11", 12),
                    (TouhouPetID.Kokoro, "Kokoro_8", 13),
                ];

                List<(TouhouPetID, string, int)> chatRoom9 =
                [
                    (TouhouPetID.Marisa, "Marisa_1", -1),
                    (TouhouPetID.Marisa, "Marisa_5", 0),
                    (TouhouPetID.Koishi, "Koishi_12", 1),
                    (TouhouPetID.Marisa, "Marisa_6", 2),
                    (TouhouPetID.Koishi, "Koishi_13", 3),
                    (TouhouPetID.Marisa, "Marisa_7", 4),
                    (TouhouPetID.Marisa, "Marisa_8", 5),
                    (TouhouPetID.Koishi, "Koishi_14", 5),
                    (TouhouPetID.Marisa, "Marisa_9", 6),
                    (TouhouPetID.Marisa, "Marisa_10", 7),
                    (TouhouPetID.Koishi, "Koishi_15", 8),
                    (TouhouPetID.Marisa, "Marisa_11", 9),
                ];

                List<(TouhouPetID, string, int)> chatRoom10 =
                [
                    (TouhouPetID.Marisa, "Marisa_12", -1),
                    (TouhouPetID.Koishi, "Koishi_16", 0),
                ];

                List<(TouhouPetID, string, int)> chatRoom11 =
                [
                    (TouhouPetID.Koishi, "Koishi_11", -1),
                    (TouhouPetID.Satori, "Satori_23", 0),
                    (TouhouPetID.Satori, "Satori_24", 1),
                    (TouhouPetID.Rin, "Rin_15", 2),
                    (TouhouPetID.Utsuho, "Utsuho_17", 3),
                    (TouhouPetID.Satori, "Satori_25", 4),
                    (TouhouPetID.Rin, "Rin_16", 5),
                    (TouhouPetID.Koishi, "Koishi_50", 6),
                    (TouhouPetID.Utsuho, "Utsuho_18", 7),
                    (TouhouPetID.Koishi, "Koishi_51", 8),
                    (TouhouPetID.Rin, "Rin_17", 9),
                    (TouhouPetID.Satori, "Satori_26", 10),
                    (TouhouPetID.Utsuho, "Utsuho_19", 11),
                    (TouhouPetID.Koishi, "Koishi_52", 12),
                    (TouhouPetID.Satori, "Satori_27", 13),
                    (TouhouPetID.Utsuho, "Utsuho_20", 13),
                    (TouhouPetID.Rin, "Rin_18", 13),
                ];

                List<(TouhouPetID, string, int)> chatRoom12 =
                [
                    (TouhouPetID.Koishi, "Koishi_12", -1),
                    (TouhouPetID.Satori, "Satori_15", 0),
                    (TouhouPetID.Koishi, "Koishi_38", 1),
                    (TouhouPetID.Satori, "Satori_16", 2),
                    (TouhouPetID.Satori, "Satori_17", 3),
                    (TouhouPetID.Koishi, "Koishi_39", 4),
                    (TouhouPetID.Satori, "Satori_18", 5),
                    (TouhouPetID.Koishi, "Koishi_40", 6),
                    (TouhouPetID.Satori, "Satori_19", 7),
                    (TouhouPetID.Satori, "Satori_20", 8),
                    (TouhouPetID.Koishi, "Koishi_41", 9),
                    (TouhouPetID.Koishi, "Koishi_42", 10),
                    (TouhouPetID.Satori, "Satori_21", 11),
                    (TouhouPetID.Koishi, "Koishi_43", 12),
                    (TouhouPetID.Satori, "Satori_22", 13),
                    (TouhouPetID.Koishi, "Koishi_44", 14),
                ];

                List<(TouhouPetID, string, int)> chatRoom13 =
                [
                    (TouhouPetID.Koishi, "Koishi_13", -1),
                    (TouhouPetID.Utsuho, "Utsuho_14", 0),
                    (TouhouPetID.Koishi, "Koishi_45", 1),
                    (TouhouPetID.Utsuho, "Utsuho_15", 2),
                    (TouhouPetID.Koishi, "Koishi_46", 3),
                    (TouhouPetID.Utsuho, "Utsuho_16", 4),
                ];

                List<(TouhouPetID, string, int)> chatRoom14 =
                [
                    (TouhouPetID.Koishi, "Koishi_14", -1),
                    (TouhouPetID.Kokoro, "Kokoro_18", 0),
                    (TouhouPetID.Koishi, "Koishi_47", 1),
                    (TouhouPetID.Kokoro, "Kokoro_19", 2),
                    (TouhouPetID.Koishi, "Koishi_48", 3),
                    (TouhouPetID.Koishi, "Koishi_49", 4),
                    (TouhouPetID.Kokoro, "Kokoro_20", 5),
                ];

                List<(TouhouPetID, string, int)> chatRoom14_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_15", -1),
                    (TouhouPetID.Kokoro, "Kokoro_18", 0),
                    (TouhouPetID.Koishi, "Koishi_47", 1),
                    (TouhouPetID.Kokoro, "Kokoro_19", 2),
                    (TouhouPetID.Koishi, "Koishi_48", 3),
                    (TouhouPetID.Koishi, "Koishi_49_1", 4),
                    (TouhouPetID.Kokoro, "Kokoro_20", 5),
                ];

                List<(TouhouPetID, string, int)> chatRoom15 =
                [
                    (TouhouPetID.Koishi, "Koishi_16", -1),
                    (TouhouPetID.Kokoro, "Kokoro_21", 0),
                    (TouhouPetID.Kokoro, "Kokoro_22", 1),
                    (TouhouPetID.Kokoro, "Kokoro_23", 2),
                    (TouhouPetID.Koishi, "Koishi_53", 3),
                    (TouhouPetID.Rin, "Rin_19", 4),
                    (TouhouPetID.Satori, "Satori_28", 5),
                    (TouhouPetID.Kokoro, "Kokoro_24", 6),
                    (TouhouPetID.Utsuho, "Utsuho_21", 7),
                    (TouhouPetID.Koishi, "Koishi_54", 8),
                    (TouhouPetID.Satori, "Satori_29", 9),
                    (TouhouPetID.Rin, "Rin_20", 10),
                    (TouhouPetID.Satori, "Satori_30", 11),
                    (TouhouPetID.Rin, "Rin_21", 12),
                    (TouhouPetID.Satori, "Satori_31", 13),
                    (TouhouPetID.Satori, "Satori_32", 14),
                    (TouhouPetID.Kokoro, "Kokoro_25", 15),
                    (TouhouPetID.Satori, "Satori_33", 16),
                    (TouhouPetID.Utsuho, "Utsuho_22", 17),
                    (TouhouPetID.Kokoro, "Kokoro_26", 18),
                    (TouhouPetID.Kokoro, "Kokoro_27", 19),
                    (TouhouPetID.Koishi, "Koishi_55", 20),
                    (TouhouPetID.Kokoro, "Kokoro_28", 21),
                    (TouhouPetID.Kokoro, "Kokoro_29", 22),
                    (TouhouPetID.Kokoro, "Kokoro_30", 23),
                    (TouhouPetID.Satori, "Satori_34", 24),
                    (TouhouPetID.Rin, "Rin_23", 25),
                    (TouhouPetID.Utsuho, "Utsuho_23", 26),
                    (TouhouPetID.Utsuho, "Utsuho_24", 27),
                    (TouhouPetID.Koishi, "Koishi_57", 28),
                    (TouhouPetID.Satori, "Satori_35", 28),
                    (TouhouPetID.Rin, "Rin_24", 28),
                    (TouhouPetID.Kokoro, "Kokoro_31", 28),
                    (TouhouPetID.Kokoro, "Kokoro_32", 29),
                ];

                List<(TouhouPetID, string, int)> chatRoom16 =
                [
                    (TouhouPetID.Koishi, "Koishi_17", -1),
                    (TouhouPetID.Marisa, "Marisa_13", 0),
                    (TouhouPetID.Koishi, "Koishi_35", 1),
                    (TouhouPetID.Koishi, "Koishi_36", 2),
                    (TouhouPetID.Marisa, "Marisa_14", 3),
                    (TouhouPetID.Marisa, "Marisa_15", 4),
                    (TouhouPetID.Koishi, "Koishi_37", 5),
                    (TouhouPetID.Marisa, "Marisa_16", 6),
                ];

                List<(TouhouPetID, string, int)> chatRoom17 =
                [
                    (TouhouPetID.Satori, "Satori_1", -1),
                    (TouhouPetID.Satori, "Satori_36", 0),
                    (TouhouPetID.Satori, "Satori_37", 1),
                    (TouhouPetID.Marisa, "Marisa_17", 2),
                    (TouhouPetID.Satori, "Satori_38", 3),
                    (TouhouPetID.Satori, "Satori_39", 4),
                    (TouhouPetID.Marisa, "Marisa_18", 5),
                    (TouhouPetID.Satori, "Satori_40", 6),
                    (TouhouPetID.Marisa, "Marisa_19", 7),
                    (TouhouPetID.Satori, "Satori_41", 8),
                    (TouhouPetID.Marisa, "Marisa_20", 9),
                ];

                List<(TouhouPetID, string, int)> chatRoom18 =
                [
                    (TouhouPetID.Satori, "Satori_2", -1),
                    (TouhouPetID.Satori, "Satori_36", 0),
                    (TouhouPetID.Satori, "Satori_37", 1),
                    (TouhouPetID.Marisa, "Marisa_17", 2),
                    (TouhouPetID.Satori, "Satori_38", 3),
                    (TouhouPetID.Satori, "Satori_39", 4),
                    (TouhouPetID.Marisa, "Marisa_18", 5),
                    (TouhouPetID.Satori, "Satori_40", 6),
                    (TouhouPetID.Marisa, "Marisa_19", 7),
                    (TouhouPetID.Satori, "Satori_41", 8),
                    (TouhouPetID.Marisa, "Marisa_20", 9),
                    (TouhouPetID.Marisa, "Marisa_21", 10),
                    (TouhouPetID.Marisa, "Marisa_22", 11),
                    (TouhouPetID.Koishi, "Koishi_58", 11),
                    (TouhouPetID.Satori, "Satori_42", 12),
                ];

                List<(TouhouPetID, string, int)> chatRoom19 =
                [
                    (TouhouPetID.Marisa, "Marisa_2", -1),
                    (TouhouPetID.Marisa, "Marisa_23", 0),
                ];

                List<(TouhouPetID, string, int)> chatRoom20 =
                [
                    (TouhouPetID.Marisa, "Marisa_3", -1),
                    (TouhouPetID.Alice, "Alice_1", 0),
                    (TouhouPetID.Marisa, "Marisa_24", 1),
                    (TouhouPetID.Marisa, "Marisa_25", 2),
                    (TouhouPetID.Alice, "Alice_2", 3),
                    (TouhouPetID.Alice, "Alice_3", 4),
                    (TouhouPetID.Marisa, "Marisa_26", 5),
                    (TouhouPetID.Marisa, "Marisa_27", 6),
                    (TouhouPetID.Alice, "Alice_4", 7),
                    (TouhouPetID.Alice, "Alice_5", 8),
                    (TouhouPetID.Marisa, "Marisa_28", 9),
                    (TouhouPetID.Alice, "Alice_6", 10),
                ];

                List<(TouhouPetID, string, int)> chatRoom21 =
                [
                    (TouhouPetID.Marisa, "Marisa_4", -1),
                    (TouhouPetID.Reimu, "Reimu_1", 0),
                    (TouhouPetID.Marisa, "Marisa_29", 1),
                    (TouhouPetID.Reimu, "Reimu_2", 2),
                    (TouhouPetID.Reimu, "Reimu_3", 3),
                    (TouhouPetID.Marisa, "Marisa_30", 4),
                    (TouhouPetID.Marisa, "Marisa_31", 5),
                    (TouhouPetID.Reimu, "Reimu_4", 6),
                    (TouhouPetID.Reimu, "Reimu_5", 7),
                    (TouhouPetID.Marisa, "Marisa_32", 8),
                    (TouhouPetID.Reimu, "Reimu_6", 9),
                    (TouhouPetID.Marisa, "Marisa_33", 10),
                ];

                List<(TouhouPetID, string, int)> chatRoom22 =
                [
                    (TouhouPetID.Marisa, "Marisa_5", -1),
                    (TouhouPetID.Marisa, "Marisa_34", 0),
                    (TouhouPetID.Koishi, "Koishi_59", 1),
                    (TouhouPetID.Marisa, "Marisa_35", 2),
                    (TouhouPetID.Koishi, "Koishi_60", 3),
                    (TouhouPetID.Marisa, "Marisa_36", 4),
                    (TouhouPetID.Marisa, "Marisa_37", 5),
                    (TouhouPetID.Koishi, "Koishi_61", 6),
                ];

                List<(TouhouPetID, string, int)> chatRoom23 =
                [
                    (TouhouPetID.Marisa, "Marisa_6", -1),
                    (TouhouPetID.Marisa, "Marisa_34", 0),
                    (TouhouPetID.Koishi, "Koishi_59", 1),
                    (TouhouPetID.Marisa, "Marisa_35", 2),
                    (TouhouPetID.Koishi, "Koishi_60", 3),
                    (TouhouPetID.Marisa, "Marisa_36", 4),
                    (TouhouPetID.Marisa, "Marisa_37", 5),
                    (TouhouPetID.Koishi, "Koishi_61", 6),
                    (TouhouPetID.Satori, "Satori_43", 7),
                    (TouhouPetID.Marisa, "Marisa_38", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom24 =
                [
                    (TouhouPetID.Koishi, "Koishi_18", -1),
                    (TouhouPetID.Satori, "Satori_44", 0),
                    (TouhouPetID.Koishi, "Koishi_62", 1),
                    (TouhouPetID.Satori, "Satori_45", 2),
                    (TouhouPetID.Koishi, "Koishi_63", 3),
                    (TouhouPetID.Satori, "Satori_46", 4),
                    (TouhouPetID.Satori, "Satori_47", 5),
                    (TouhouPetID.Satori, "Satori_48", 6),
                    (TouhouPetID.Koishi, "Koishi_64", 7),
                    (TouhouPetID.Satori, "Satori_49", 8),
                    (TouhouPetID.Koishi, "Koishi_65", 9),
                    (TouhouPetID.Satori, "Satori_50", 10),
                ];

                List<(TouhouPetID, string, int)> chatRoom25 =
                [
                    (TouhouPetID.Koishi, "Koishi_19", -1),
                    (TouhouPetID.Satori, "Satori_44", 0),
                    (TouhouPetID.Koishi, "Koishi_66", 1),
                    (TouhouPetID.Satori, "Satori_51", 2),
                    (TouhouPetID.Koishi, "Koishi_67", 3),
                    (TouhouPetID.Koishi, "Koishi_68", 4),
                    (TouhouPetID.Satori, "Satori_52", 5),
                    (TouhouPetID.Koishi, "Koishi_69", 6),
                    (TouhouPetID.Satori, "Satori_53", 7),
                    (TouhouPetID.Koishi, "Koishi_70", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom26 =
                [
                    (TouhouPetID.Koishi, "Koishi_20", -1),
                    (TouhouPetID.Satori, "Satori_44", 0),
                    (TouhouPetID.Koishi, "Koishi_71", 1),
                    (TouhouPetID.Koishi, "Koishi_72", 2),
                    (TouhouPetID.Satori, "Satori_54", 3),
                    (TouhouPetID.Koishi, "Koishi_73", 3),
                    (TouhouPetID.Satori, "Satori_55", 4),
                    (TouhouPetID.Satori, "Satori_56", 5),
                    (TouhouPetID.Koishi, "Koishi_74", 6),
                    (TouhouPetID.Koishi, "Koishi_75", 7),
                    (TouhouPetID.Koishi, "Koishi_76", 8),
                    (TouhouPetID.Koishi, "Koishi_77", 9),
                    (TouhouPetID.Satori, "Satori_57", 10),
                    (TouhouPetID.Satori, "Satori_58", 11),
                    (TouhouPetID.Koishi, "Koishi_78", 12),
                    (TouhouPetID.Satori, "Satori_59", 13),
                    (TouhouPetID.Koishi, "Koishi_79", 14),
                    (TouhouPetID.Koishi, "Koishi_80", 15),
                    (TouhouPetID.Satori, "Satori_60", 16),
                ];

                List<(TouhouPetID, string, int)> chatRoom27 =
                [
                    (TouhouPetID.Koishi, "Koishi_21", -1),
                    (TouhouPetID.Marisa, "Marisa_39", 0),
                    (TouhouPetID.Koishi, "Koishi_81", 1),
                    (TouhouPetID.Marisa, "Marisa_40", 2),
                    (TouhouPetID.Koishi, "Koishi_82", 3),
                ];

                List<(TouhouPetID, string, int)> chatRoom28 =
                [
                    (TouhouPetID.Koishi, "Koishi_22", -1),
                    (TouhouPetID.Rin, "Rin_25", 0),
                    (TouhouPetID.Satori, "Satori_61", 1),
                    (TouhouPetID.Utsuho, "Utsuho_27", 2),
                    (TouhouPetID.Koishi, "Koishi_84", 3),
                    (TouhouPetID.Koishi, "Koishi_85", 4),
                    (TouhouPetID.Koishi, "Koishi_86", 5),
                    (TouhouPetID.Satori, "Satori_62", 6),
                    (TouhouPetID.Rin, "Rin_26", 7),
                    (TouhouPetID.Utsuho, "Utsuho_28", 8),
                    (TouhouPetID.Satori, "Satori_63", 9),
                    (TouhouPetID.Koishi, "Koishi_87", 10),
                    (TouhouPetID.Rin, "Rin_27", 10),
                    (TouhouPetID.Utsuho, "Utsuho_29", 10),
                    (TouhouPetID.Satori, "Satori_64", 11),
                ];

                List<(TouhouPetID, string, int)> chatRoom28_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_23", -1),
                    (TouhouPetID.Rin, "Rin_25", 0),
                    (TouhouPetID.Satori, "Satori_61", 1),
                    (TouhouPetID.Utsuho, "Utsuho_27", 2),
                    (TouhouPetID.Koishi, "Koishi_84", 3),
                    (TouhouPetID.Koishi, "Koishi_85_1", 4),
                    (TouhouPetID.Koishi, "Koishi_86", 5),
                    (TouhouPetID.Satori, "Satori_62", 6),
                    (TouhouPetID.Rin, "Rin_26", 7),
                    (TouhouPetID.Utsuho, "Utsuho_28", 8),
                    (TouhouPetID.Satori, "Satori_63", 9),
                    (TouhouPetID.Koishi, "Koishi_87", 10),
                    (TouhouPetID.Rin, "Rin_27", 10),
                    (TouhouPetID.Utsuho, "Utsuho_29", 10),
                    (TouhouPetID.Satori, "Satori_64", 11),
                ];

                List<(TouhouPetID, string, int)> chatRoom28_2 =
                [
                    (TouhouPetID.Koishi, "Koishi_24", -1),
                    (TouhouPetID.Rin, "Rin_25", 0),
                    (TouhouPetID.Satori, "Satori_61", 1),
                    (TouhouPetID.Utsuho, "Utsuho_27", 2),
                    (TouhouPetID.Koishi, "Koishi_84", 3),
                    (TouhouPetID.Koishi, "Koishi_85_2", 4),
                    (TouhouPetID.Koishi, "Koishi_86", 5),
                    (TouhouPetID.Satori, "Satori_62", 6),
                    (TouhouPetID.Rin, "Rin_26", 7),
                    (TouhouPetID.Utsuho, "Utsuho_28", 8),
                    (TouhouPetID.Satori, "Satori_63", 9),
                    (TouhouPetID.Koishi, "Koishi_87", 10),
                    (TouhouPetID.Rin, "Rin_27", 10),
                    (TouhouPetID.Utsuho, "Utsuho_29", 10),
                    (TouhouPetID.Satori, "Satori_64", 11),
                ];

                List<(TouhouPetID, string, int)> chatRoom28_3 =
                [
                    (TouhouPetID.Koishi, "Koishi_25", -1),
                    (TouhouPetID.Rin, "Rin_25", 0),
                    (TouhouPetID.Satori, "Satori_61", 1),
                    (TouhouPetID.Utsuho, "Utsuho_27", 2),
                    (TouhouPetID.Koishi, "Koishi_84", 3),
                    (TouhouPetID.Koishi, "Koishi_85_3", 4),
                    (TouhouPetID.Koishi, "Koishi_86", 5),
                    (TouhouPetID.Satori, "Satori_62", 6),
                    (TouhouPetID.Rin, "Rin_26", 7),
                    (TouhouPetID.Utsuho, "Utsuho_28", 8),
                    (TouhouPetID.Satori, "Satori_63", 9),
                    (TouhouPetID.Koishi, "Koishi_87", 10),
                    (TouhouPetID.Rin, "Rin_27", 10),
                    (TouhouPetID.Utsuho, "Utsuho_29", 10),
                    (TouhouPetID.Satori, "Satori_64", 11),
                ];

                List<(TouhouPetID, string, int)> chatRoom29 =
                [
                    (TouhouPetID.Koishi, "Koishi_26", -1),
                    (TouhouPetID.Satori, "Satori_65", 0),
                    (TouhouPetID.Satori, "Satori_66", 1),
                    (TouhouPetID.Koishi, "Koishi_88", 2),
                    (TouhouPetID.Koishi, "Koishi_84", 3),
                    (TouhouPetID.Satori, "Satori_67", 4),
                    (TouhouPetID.Koishi, "Koishi_89", 5),
                    (TouhouPetID.Satori, "Satori_68", 6),
                    (TouhouPetID.Koishi, "Koishi_90", 7),
                    (TouhouPetID.Satori, "Satori_69", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom29_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_27", -1),
                    (TouhouPetID.Satori, "Satori_65", 0),
                    (TouhouPetID.Satori, "Satori_66", 1),
                    (TouhouPetID.Koishi, "Koishi_88", 2),
                    (TouhouPetID.Koishi, "Koishi_84", 3),
                    (TouhouPetID.Satori, "Satori_67_1", 4),
                    (TouhouPetID.Koishi, "Koishi_89", 5),
                    (TouhouPetID.Satori, "Satori_68", 6),
                    (TouhouPetID.Koishi, "Koishi_90", 7),
                    (TouhouPetID.Satori, "Satori_69", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom30 =
                [
                    (TouhouPetID.Koishi, "Koishi_28", -1),
                    (TouhouPetID.Utsuho, "Utsuho_25", 0),
                    (TouhouPetID.Utsuho, "Utsuho_26", 1),
                    (TouhouPetID.Koishi, "Koishi_83", 2),
                ];

                List<(TouhouPetID, string, int)> chatRoom31 =
                [
                    (TouhouPetID.Koishi, "Koishi_29", -1),
                    (TouhouPetID.Kokoro, "Kokoro_33", 0),
                    (TouhouPetID.Koishi, "Koishi_91", 1),
                    (TouhouPetID.Koishi, "Koishi_92", 2),
                    (TouhouPetID.Kokoro, "Kokoro_34", 3),
                    (TouhouPetID.Kokoro, "Kokoro_35", 4),
                    (TouhouPetID.Koishi, "Koishi_93", 5),
                    (TouhouPetID.Kokoro, "Kokoro_36", 6),
                    (TouhouPetID.Kokoro, "Kokoro_37", 7),
                    (TouhouPetID.Koishi, "Koishi_94", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom31_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_30", -1),
                    (TouhouPetID.Kokoro, "Kokoro_33", 0),
                    (TouhouPetID.Koishi, "Koishi_91_1", 1),
                    (TouhouPetID.Koishi, "Koishi_92", 2),
                    (TouhouPetID.Kokoro, "Kokoro_34", 3),
                    (TouhouPetID.Kokoro, "Kokoro_35", 4),
                    (TouhouPetID.Koishi, "Koishi_93", 5),
                    (TouhouPetID.Kokoro, "Kokoro_36", 6),
                    (TouhouPetID.Kokoro, "Kokoro_37", 7),
                    (TouhouPetID.Koishi, "Koishi_94", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom32 =
                [
                    (TouhouPetID.Koishi, "Koishi_31", -1),
                    (TouhouPetID.Utsuho, "Utsuho_30", 0),
                    (TouhouPetID.Koishi, "Koishi_95", 1),
                    (TouhouPetID.Koishi, "Koishi_96", 2),
                    (TouhouPetID.Koishi, "Koishi_97", 3),
                    (TouhouPetID.Kokoro, "Kokoro_38", 4),
                    (TouhouPetID.Kokoro, "Kokoro_39", 5),
                    (TouhouPetID.Satori, "Satori_70", 6),
                    (TouhouPetID.Rin, "Rin_28", 7),
                    (TouhouPetID.Rin, "Rin_29", 8),
                    (TouhouPetID.Utsuho, "Utsuho_31", 9),
                    (TouhouPetID.Utsuho, "Utsuho_32", 10),
                    (TouhouPetID.Rin, "Rin_30", 11),
                    (TouhouPetID.Utsuho, "Utsuho_33", 12),
                    (TouhouPetID.Satori, "Satori_71", 13),
                    (TouhouPetID.Koishi, "Koishi_98", 14),
                    (TouhouPetID.Kokoro, "Kokoro_40", 15),
                    (TouhouPetID.Satori, "Satori_72", 16),
                    (TouhouPetID.Satori, "Satori_73", 17),
                    (TouhouPetID.Koishi, "Koishi_99", 18),
                    (TouhouPetID.Koishi, "Koishi_100", 19),
                    (TouhouPetID.Utsuho, "Utsuho_34", 20),
                    (TouhouPetID.Rin, "Rin_31", 20),
                    (TouhouPetID.Satori, "Satori_74", 20),
                    (TouhouPetID.Kokoro, "Kokoro_41", 20),
                ];

                List<(TouhouPetID, string, int)> chatRoom33 =
                [
                    (TouhouPetID.Koishi, "Koishi_32", -1),
                    (TouhouPetID.Marisa, "Marisa_41", 0),
                    (TouhouPetID.Koishi, "Koishi_101", 1),
                    (TouhouPetID.Koishi, "Koishi_102", 2),
                    (TouhouPetID.Koishi, "Koishi_103", 3),
                    (TouhouPetID.Marisa, "Marisa_42", 4),
                    (TouhouPetID.Koishi, "Koishi_104", 5),
                    (TouhouPetID.Marisa, "Marisa_43", 6),
                    (TouhouPetID.Marisa, "Marisa_44", 7),
                    (TouhouPetID.Koishi, "Koishi_105", 8),
                    (TouhouPetID.Marisa, "Marisa_45", 9),
                    (TouhouPetID.Koishi, "Marisa_45", 9),
                ];

                List<(TouhouPetID, string, int)> chatRoom34 =
                [
                    (TouhouPetID.Koishi, "Koishi_33", -1),
                    (TouhouPetID.Rin, "Rin_32", 0),
                    (TouhouPetID.Koishi, "Koishi_106", 1),
                    (TouhouPetID.Rin, "Rin_33", 2),
                    (TouhouPetID.Utsuho, "Utsuho_35", 3),
                    (TouhouPetID.Utsuho, "Utsuho_36", 4),
                    (TouhouPetID.Utsuho, "Utsuho_37", 5),
                    (TouhouPetID.Rin, "Rin_34", 6),
                    (TouhouPetID.Satori, "Satori_75", 7),
                    (TouhouPetID.Rin, "Rin_35", 8),
                    (TouhouPetID.Satori, "Satori_76", 9),
                    (TouhouPetID.Rin, "Rin_36", 10),
                    (TouhouPetID.Utsuho, "Utsuho_38", 10),
                    (TouhouPetID.Koishi, "Koishi_107", 10),
                    (TouhouPetID.Satori, "Satori_77", 11),
                    (TouhouPetID.Koishi, "Koishi_108", 12),
                    (TouhouPetID.Satori, "Satori_78", 13),
                    (TouhouPetID.Satori, "Satori_79", 14),
                    (TouhouPetID.Utsuho, "Utsuho_39", 15),
                    (TouhouPetID.Rin, "Rin_37", 16),
                    (TouhouPetID.Satori, "Satori_80", 17),
                    (TouhouPetID.Satori, "Satori_81", 18),
                    (TouhouPetID.Koishi, "Koishi_109", 19),
                    (TouhouPetID.Utsuho, "Utsuho_40", 20),
                    (TouhouPetID.Rin, "Rin_38", 20),
                    (TouhouPetID.Satori, "Satori_82", 21),
                    (TouhouPetID.Koishi, "Koishi_110", 22),
                    (TouhouPetID.Satori, "Satori_83", 23),
                    (TouhouPetID.Utsuho, "Utsuho_41", 24),
                    (TouhouPetID.Rin, "Rin_39", 24),
                    (TouhouPetID.Satori, "Satori_84", 25),
                    (TouhouPetID.Satori, "Satori_85", 26),
                    (TouhouPetID.Utsuho, "Utsuho_42", 27),
                    (TouhouPetID.Rin, "Rin_40", 27),
                    (TouhouPetID.Koishi, "Koishi_111", 27),
                ];

                List<(TouhouPetID, string, int)> chatRoom35 =
                [
                    (TouhouPetID.Koishi, "Koishi_34", -1),
                    (TouhouPetID.Satori, "Satori_86", 0),
                    (TouhouPetID.Koishi, "Koishi_116", 1),
                    (TouhouPetID.Koishi, "Koishi_117", 2),
                    (TouhouPetID.Satori, "Satori_87", 3),
                    (TouhouPetID.Koishi, "Koishi_118", 5),
                    (TouhouPetID.Satori, "Satori_88", 6),
                    (TouhouPetID.Koishi, "Koishi_119", 7),
                    (TouhouPetID.Koishi, "Koishi_120", 8),
                    (TouhouPetID.Satori, "Satori_89", 9),
                    (TouhouPetID.Satori, "Satori_90", 10),
                ];

                List<(TouhouPetID, string, int)> chatRoom36 =
                [
                    (TouhouPetID.Koishi, "Koishi_35", -1),
                    (TouhouPetID.Utsuho, "Utsuho_43", 0),
                    (TouhouPetID.Koishi, "Koishi_112", 1),
                    (TouhouPetID.Utsuho, "Utsuho_44", 2),
                    (TouhouPetID.Utsuho, "Utsuho_45", 3),
                    (TouhouPetID.Koishi, "Koishi_113", 4),
                    (TouhouPetID.Utsuho, "Utsuho_46", 5),
                    (TouhouPetID.Utsuho, "Utsuho_47", 6),
                    (TouhouPetID.Utsuho, "Utsuho_48", 7),
                    (TouhouPetID.Utsuho, "Utsuho_49", 8),
                    (TouhouPetID.Utsuho, "Utsuho_50", 9),
                    (TouhouPetID.Koishi, "Koishi_114", 10),
                    (TouhouPetID.Koishi, "Koishi_115", 11),
                ];

                List<(TouhouPetID, string, int)> chatRoom37 =
                [
                    (TouhouPetID.Koishi, "Koishi_36", -1),
                    (TouhouPetID.Kokoro, "Kokoro_42", 0),
                    (TouhouPetID.Koishi, "Koishi_121", 1),
                    (TouhouPetID.Kokoro, "Kokoro_43", 2),
                    (TouhouPetID.Koishi, "Koishi_122", 3),
                    (TouhouPetID.Koishi, "Koishi_123", 4),
                    (TouhouPetID.Kokoro, "Kokoro_44", 5),
                    (TouhouPetID.Kokoro, "Kokoro_45", 6),
                    (TouhouPetID.Koishi, "Koishi_124", 7),
                    (TouhouPetID.Kokoro, "Kokoro_46", 8),
                    (TouhouPetID.Koishi, "Koishi_125", 9),
                    (TouhouPetID.Kokoro, "Kokoro_47", 10),
                    (TouhouPetID.Koishi, "Koishi_126", 11),
                    (TouhouPetID.Kokoro, "Kokoro_48", 12),
                ];

                List<(TouhouPetID, string, int)> chatRoom38 =
                [
                    (TouhouPetID.Koishi, "Koishi_37", -1),
                    (TouhouPetID.Kokoro, "Kokoro_49", 0),
                    (TouhouPetID.Kokoro, "Kokoro_50", 1),
                    (TouhouPetID.Utsuho, "Utsuho_51", 2),
                    (TouhouPetID.Utsuho, "Utsuho_52", 3),
                    (TouhouPetID.Rin, "Rin_41", 4),
                    (TouhouPetID.Satori, "Satori_91", 5),
                    (TouhouPetID.Satori, "Satori_92", 6),
                    (TouhouPetID.Koishi, "Koishi_132", 7),
                    (TouhouPetID.Koishi, "Koishi_133", 8),
                ];

                List<(TouhouPetID, string, int)> chatRoom39 =
                [
                    (TouhouPetID.Koishi, "Koishi_38", -1),
                    (TouhouPetID.Marisa, "Marisa_46", 0),
                    (TouhouPetID.Koishi, "Koishi_127", 1),
                    (TouhouPetID.Marisa, "Marisa_47", 2),
                    (TouhouPetID.Koishi, "Koishi_128", 3),
                    (TouhouPetID.Marisa, "Marisa_48", 4),
                    (TouhouPetID.Koishi, "Koishi_129", 5),
                    (TouhouPetID.Marisa, "Marisa_49", 6),
                    (TouhouPetID.Koishi, "Koishi_130", 7),
                    (TouhouPetID.Marisa, "Marisa_50", 8),
                    (TouhouPetID.Koishi, "Koishi_131", 9),
                ];

                List<(TouhouPetID, string, int)> chatRoom40 =
                [
                    (TouhouPetID.Satori, "Satori_3", -1),
                    (TouhouPetID.Koishi, "Koishi_134", 0),
                    (TouhouPetID.Satori, "Satori_93", 1),
                    (TouhouPetID.Satori, "Satori_94", 2),
                    (TouhouPetID.Koishi, "Koishi_135", 3),
                    (TouhouPetID.Satori, "Satori_95", 4),
                    (TouhouPetID.Koishi, "Koishi_136", 5),
                    (TouhouPetID.Koishi, "Koishi_137", 6),
                    (TouhouPetID.Satori, "Satori_96", 7),
                    (TouhouPetID.Satori, "Satori_97", 8),
                    (TouhouPetID.Satori, "Satori_98", 9),
                    (TouhouPetID.Koishi, "Koishi_138", 10),
                    (TouhouPetID.Koishi, "Koishi_139", 11),
                ];

                List<(TouhouPetID, string, int)> chatRoom41 =
                [
                    (TouhouPetID.Satori, "Satori_4", -1),
                    (TouhouPetID.Satori, "Satori_99", 0),
                    (TouhouPetID.Koishi, "Koishi_134", 1),
                    (TouhouPetID.Kokoro, "Kokoro_51", 1),
                    (TouhouPetID.Satori, "Satori_93", 2),
                    (TouhouPetID.Satori, "Satori_94", 3),
                    (TouhouPetID.Koishi, "Koishi_135", 4),
                    (TouhouPetID.Satori, "Satori_100", 5),
                    (TouhouPetID.Satori, "Satori_101", 6),
                    (TouhouPetID.Koishi, "Koishi_136", 7),
                    (TouhouPetID.Kokoro, "Kokoro_52", 7),
                    (TouhouPetID.Kokoro, "Kokoro_53", 8),
                    (TouhouPetID.Satori, "Satori_96", 9),
                    (TouhouPetID.Satori, "Satori_97", 10),
                    (TouhouPetID.Satori, "Satori_98", 11),
                    (TouhouPetID.Koishi, "Koishi_138", 12),
                    (TouhouPetID.Koishi, "Koishi_139", 13),
                    (TouhouPetID.Kokoro, "Kokoro_54", 14),
                ];

                List<(TouhouPetID, string, int)> chatRoom42 =
                [
                    (TouhouPetID.Koishi, "Koishi_39", -1),
                    (TouhouPetID.Kokoro, "Kokoro_55", 0),
                    (TouhouPetID.Koishi, "Koishi_140", 1),
                    (TouhouPetID.Kokoro, "Kokoro_56", 2),
                    (TouhouPetID.Koishi, "Koishi_141", 3),
                    (TouhouPetID.Kokoro, "Kokoro_57", 4),
                    (TouhouPetID.Koishi, "Koishi_142", 5),
                    (TouhouPetID.Koishi, "Koishi_143", 6),
                    (TouhouPetID.Koishi, "Kokoro_58", 7),
                    (TouhouPetID.Kokoro, "Kokoro_58", 7),
                    (TouhouPetID.Koishi, "Koishi_144", 8),
                    (TouhouPetID.Kokoro, "Kokoro_59", 9),
                    (TouhouPetID.Koishi, "Koishi_145", 10),
                    (TouhouPetID.Kokoro, "Kokoro_60", 11),
                    (TouhouPetID.Koishi, "Koishi_146", 12),
                    (TouhouPetID.Kokoro, "Kokoro_61", 13),
                    (TouhouPetID.Kokoro, "Kokoro_62", 14),
                    (TouhouPetID.Koishi, "Koishi_147", 15),
                    (TouhouPetID.Kokoro, "Kokoro_63", 16),
                    (TouhouPetID.Koishi, "Koishi_148", 17),
                    (TouhouPetID.Kokoro, "Kokoro_64", 18),
                    (TouhouPetID.Koishi, "Koishi_149", 19),
                    (TouhouPetID.Kokoro, "Kokoro_65", 20),
                    (TouhouPetID.Koishi, "Kokoro_65", 20),
                ];

                List<(TouhouPetID, string, int)> chatRoom43 =
                [
                    (TouhouPetID.Koishi, "Koishi_40", -1),
                    (TouhouPetID.Koishi, "Koishi_150", 0),
                    (TouhouPetID.Kokoro, "Kokoro_66", 1),
                    (TouhouPetID.Koishi, "Koishi_151", 2),
                    (TouhouPetID.Koishi, "Koishi_152", 3),
                    (TouhouPetID.Kokoro, "Kokoro_67", 4),
                    (TouhouPetID.Koishi, "Koishi_153", 5),
                    (TouhouPetID.Kokoro, "Kokoro_68", 6),
                    (TouhouPetID.Koishi, "Koishi_154", 7),
                    (TouhouPetID.Kokoro, "Kokoro_69", 8),
                    (TouhouPetID.Kokoro, "Kokoro_70", 9),
                    (TouhouPetID.Koishi, "Koishi_155", 10),
                    (TouhouPetID.Koishi, "Koishi_156", 11),
                    (TouhouPetID.Kokoro, "Kokoro_71", 12),
                    (TouhouPetID.Koishi, "Koishi_157", 13),
                    (TouhouPetID.Kokoro, "Kokoro_72", 14),
                    (TouhouPetID.Koishi, "Koishi_158", 15),
                    (TouhouPetID.Koishi, "Koishi_159", 16),
                    (TouhouPetID.Kokoro, "Kokoro_73", 17),
                    (TouhouPetID.Koishi, "Koishi_160", 18),
                ];

                List<(TouhouPetID, string, int)> chatRoom43_1 =
                [
                    (TouhouPetID.Koishi, "Koishi_41", -1),
                    (TouhouPetID.Koishi, "Koishi_150", 0),
                    (TouhouPetID.Kokoro, "Kokoro_66_1", 1),
                    (TouhouPetID.Koishi, "Koishi_151", 2),
                    (TouhouPetID.Koishi, "Koishi_152", 3),
                    (TouhouPetID.Kokoro, "Kokoro_67", 4),
                    (TouhouPetID.Koishi, "Koishi_153", 5),
                    (TouhouPetID.Kokoro, "Kokoro_68", 6),
                    (TouhouPetID.Koishi, "Koishi_154", 7),
                    (TouhouPetID.Kokoro, "Kokoro_69", 8),
                    (TouhouPetID.Kokoro, "Kokoro_70", 9),
                    (TouhouPetID.Koishi, "Koishi_155", 10),
                    (TouhouPetID.Koishi, "Koishi_156", 11),
                    (TouhouPetID.Kokoro, "Kokoro_71", 12),
                    (TouhouPetID.Koishi, "Koishi_157", 13),
                    (TouhouPetID.Kokoro, "Kokoro_72", 14),
                    (TouhouPetID.Koishi, "Koishi_158", 15),
                    (TouhouPetID.Koishi, "Koishi_159", 16),
                    (TouhouPetID.Kokoro, "Kokoro_73", 17),
                    (TouhouPetID.Koishi, "Koishi_160", 18),
                ];

                List<(TouhouPetID, string, int)> chatRoom44 =
                [
                    (TouhouPetID.Koishi, "Koishi_42", -1),
                    (TouhouPetID.Marisa, "Marisa_51", 0),
                    (TouhouPetID.Koishi, "Koishi_161", 1),
                    (TouhouPetID.Marisa, "Marisa_52", 2),
                    (TouhouPetID.Marisa, "Marisa_53", 3),
                    (TouhouPetID.Koishi, "Koishi_162", 4),
                    (TouhouPetID.Marisa, "Marisa_54", 5),
                    (TouhouPetID.Marisa, "Marisa_55", 6),
                    (TouhouPetID.Marisa, "Marisa_56", 7),
                    (TouhouPetID.Koishi, "Koishi_163", 8),
                    (TouhouPetID.Marisa, "Marisa_57", 9),
                    (TouhouPetID.Marisa, "Marisa_58", 10),
                    (TouhouPetID.Koishi, "Koishi_164", 11),
                    (TouhouPetID.Marisa, "Marisa_59", 12),
                ];

                List<(TouhouPetID, string, int)> chatRoom45 =
                [
                    (TouhouPetID.Koishi, "Koishi_43", -1),
                    (TouhouPetID.Marisa, "Marisa_51", 0),
                    (TouhouPetID.Koishi, "Koishi_161", 1),
                    (TouhouPetID.Marisa, "Marisa_52", 2),
                    (TouhouPetID.Marisa, "Marisa_53", 3),
                    (TouhouPetID.Koishi, "Koishi_162", 4),
                    (TouhouPetID.Marisa, "Marisa_54", 5),
                    (TouhouPetID.Marisa, "Marisa_55", 6),
                    (TouhouPetID.Marisa, "Marisa_56", 7),
                    (TouhouPetID.Koishi, "Koishi_163", 8),
                    (TouhouPetID.Marisa, "Marisa_57", 9),
                    (TouhouPetID.Marisa, "Marisa_58", 10),
                    (TouhouPetID.Koishi, "Koishi_165", 11),
                    (TouhouPetID.Marisa, "Marisa_60", 12),
                    (TouhouPetID.Marisa, "Marisa_61", 13),
                    (TouhouPetID.Koishi, "Koishi_166", 14),
                    (TouhouPetID.Marisa, "Marisa_62", 15),
                ];

                List<(TouhouPetID, string, int)> chatRoom46 =
                [
                    (TouhouPetID.Koishi, "Koishi_44", -1),
                    (TouhouPetID.Marisa, "Marisa_63", 0),
                    (TouhouPetID.Koishi, "Koishi_167", 1),
                    (TouhouPetID.Marisa, "Marisa_64", 2),
                    (TouhouPetID.Marisa, "Marisa_65", 3),
                    (TouhouPetID.Koishi, "Koishi_168", 4),
                    (TouhouPetID.Marisa, "Marisa_66", 5),
                    (TouhouPetID.Marisa, "Marisa_67", 6),
                    (TouhouPetID.Koishi, "Koishi_169", 7),
                    (TouhouPetID.Marisa, "Marisa_68", 8),
                    (TouhouPetID.Koishi, "Koishi_170", 9),
                    (TouhouPetID.Marisa, "Marisa_69", 10),
                    (TouhouPetID.Marisa, "Marisa_70", 11),
                    (TouhouPetID.Koishi, "Koishi_171", 12),
                ];

                List<(TouhouPetID, string, int)> chatRoom47 =
                [
                    (TouhouPetID.Koishi, "Koishi_45", -1),
                    (TouhouPetID.Utsuho, "Utsuho_53", 0),
                    (TouhouPetID.Koishi, "Koishi_172", 1),
                    (TouhouPetID.Koishi, "Koishi_173", 2),
                    (TouhouPetID.Utsuho, "Utsuho_54", 3),
                    (TouhouPetID.Koishi, "Koishi_174", 4),
                    (TouhouPetID.Utsuho, "Utsuho_55", 5),
                    (TouhouPetID.Utsuho, "Utsuho_56", 6),
                    (TouhouPetID.Utsuho, "Utsuho_57", 7),
                    (TouhouPetID.Utsuho, "Utsuho_58", 8),
                    (TouhouPetID.Utsuho, "Utsuho_59", 9),
                    (TouhouPetID.Koishi, "Koishi_175", 10),
                    (TouhouPetID.Utsuho, "Utsuho_60", 11),
                    (TouhouPetID.Koishi, "Koishi_176", 12),
                    (TouhouPetID.Koishi, "Koishi_177", 13),
                    (TouhouPetID.Utsuho, "Utsuho_61", 14),
                    (TouhouPetID.Koishi, "Koishi_178", 15),
                    (TouhouPetID.Utsuho, "Utsuho_62", 16),
                ];

                List<(TouhouPetID, string, int)> chatRoom48 =
                [
                    (TouhouPetID.Koishi, "Koishi_46", -1),
                    (TouhouPetID.Utsuho, "Utsuho_53", 0),
                    (TouhouPetID.Koishi, "Koishi_172", 1),
                    (TouhouPetID.Koishi, "Koishi_173", 2),
                    (TouhouPetID.Utsuho, "Utsuho_54", 3),
                    (TouhouPetID.Koishi, "Koishi_174", 4),
                    (TouhouPetID.Utsuho, "Utsuho_55", 5),
                    (TouhouPetID.Utsuho, "Utsuho_56", 6),
                    (TouhouPetID.Utsuho, "Utsuho_57", 7),
                    (TouhouPetID.Utsuho, "Utsuho_58_1", 8),
                    (TouhouPetID.Utsuho, "Utsuho_59", 9),
                    (TouhouPetID.Koishi, "Koishi_179", 10),
                    (TouhouPetID.Koishi, "Koishi_180", 11),
                    (TouhouPetID.Utsuho, "Utsuho_63", 12),
                    (TouhouPetID.Koishi, "Koishi_181", 13),
                    (TouhouPetID.Utsuho, "Utsuho_64", 14),
                    (TouhouPetID.Koishi, "Koishi_182", 15),
                    (TouhouPetID.Koishi, "Koishi_183", 16),
                    (TouhouPetID.Utsuho, "Utsuho_65", 17),
                    (TouhouPetID.Koishi, "Koishi_184", 18),
                    (TouhouPetID.Utsuho, "Utsuho_66", 19),
                    (TouhouPetID.Koishi, "Koishi_185", 20),
                    (TouhouPetID.Utsuho, "Utsuho_67", 21),
                    (TouhouPetID.Koishi, "Koishi_186", 22),
                    (TouhouPetID.Utsuho, "Utsuho_68", 23),
                    (TouhouPetID.Koishi, "Koishi_187", 24),
                ];

                List<(TouhouPetID, string, int)> chatRoom49 =
                [
                    (TouhouPetID.Koishi, "Koishi_47", -1),
                    (TouhouPetID.Utsuho, "Utsuho_69", 0),
                    (TouhouPetID.Rin, "Rin_42", 1),
                    (TouhouPetID.Koishi, "Koishi_188", 2),
                    (TouhouPetID.Satori, "Satori_102", 3),
                    (TouhouPetID.Rin, "Rin_43", 4),
                    (TouhouPetID.Satori, "Satori_103", 5),
                    (TouhouPetID.Utsuho, "Utsuho_70", 6),
                    (TouhouPetID.Koishi, "Koishi_189", 6),
                    (TouhouPetID.Rin, "Rin_44", 7),
                    (TouhouPetID.Rin, "Rin_45", 8),
                    (TouhouPetID.Rin, "Rin_46", 9),
                    (TouhouPetID.Rin, "Rin_47", 10),
                    (TouhouPetID.Rin, "Rin_48", 11),
                    (TouhouPetID.Koishi, "Koishi_190", 12),
                    (TouhouPetID.Utsuho, "Utsuho_71", 13),
                    (TouhouPetID.Satori, "Satori_104", 14),
                    (TouhouPetID.Satori, "Satori_105", 15),
                    (TouhouPetID.Koishi, "Koishi_191", 16),
                    (TouhouPetID.Utsuho, "Utsuho_72", 16),
                    (TouhouPetID.Rin, "Rin_49", 17),
                    (TouhouPetID.Satori, "Satori_106", 18),
                    (TouhouPetID.Satori, "Satori_107", 19),
                    (TouhouPetID.Rin, "Rin_50", 20),
                ];

                List<(TouhouPetID, string, int)> chatRoom50 =
                [
                    (TouhouPetID.Koishi, "Koishi_48", -1),
                    (TouhouPetID.Utsuho, "Utsuho_73", 0),
                    (TouhouPetID.Rin, "Rin_51", 1),
                    (TouhouPetID.Satori, "Satori_108", 2),
                    (TouhouPetID.Koishi, "Koishi_192", 3),
                    (TouhouPetID.Satori, "Satori_109", 4),
                    (TouhouPetID.Koishi, "Koishi_193", 5),
                    (TouhouPetID.Utsuho, "Utsuho_74", 5),
                    (TouhouPetID.Rin, "Rin_52", 5),
                    (TouhouPetID.Satori, "Satori_110", 6),
                    (TouhouPetID.Satori, "Satori_111", 7),
                    (TouhouPetID.Satori, "Satori_112", 8),
                    (TouhouPetID.Utsuho, "Utsuho_75", 9),
                    (TouhouPetID.Rin, "Rin_53", 9),
                    (TouhouPetID.Koishi, "Koishi_194", 10),
                    (TouhouPetID.Satori, "Satori_113", 11),
                    (TouhouPetID.Koishi, "Koishi_195", 12),
                    (TouhouPetID.Satori, "Satori_114", 13),
                    (TouhouPetID.Satori, "Satori_115", 14),
                    (TouhouPetID.Utsuho, "Utsuho_76", 15),
                    (TouhouPetID.Rin, "Rin_54", 16),
                    (TouhouPetID.Utsuho, "Utsuho_77", 17),
                    (TouhouPetID.Satori, "Satori_116", 18),
                    (TouhouPetID.Koishi, "Koishi_196", 19),
                    (TouhouPetID.Satori, "Satori_117", 20),
                    (TouhouPetID.Koishi, "Koishi_197", 21),
                    (TouhouPetID.Utsuho, "Utsuho_78", 21),
                    (TouhouPetID.Rin, "Rin_55", 21),
                ];

                List<(TouhouPetID, string, int)> chatRoom51 =
                [
                    (TouhouPetID.Koishi, "Koishi_49", -1),
                    (TouhouPetID.Koishi, "Koishi_198", 0),
                    (TouhouPetID.Satori, "Satori_118", 1),
                    (TouhouPetID.Koishi, "Koishi_199", 2),
                    (TouhouPetID.Satori, "Satori_119", 3),
                    (TouhouPetID.Satori, "Satori_120", 4),
                    (TouhouPetID.Koishi, "Koishi_200", 5),
                    (TouhouPetID.Koishi, "Koishi_201", 6),
                    (TouhouPetID.Satori, "Satori_121", 7),
                    (TouhouPetID.Koishi, "Koishi_202", 8),
                    (TouhouPetID.Satori, "Satori_122", 9),
                    (TouhouPetID.Satori, "Satori_123", 10),
                    (TouhouPetID.Satori, "Satori_124", 11),
                    (TouhouPetID.Satori, "Satori_125", 12),
                    (TouhouPetID.Satori, "Satori_126", 13),
                    (TouhouPetID.Satori, "Satori_127", 14),
                    (TouhouPetID.Koishi, "Koishi_203", 15),
                    (TouhouPetID.Satori, "Satori_128", 16),
                    (TouhouPetID.Koishi, "Koishi_204", 17),
                    (TouhouPetID.Satori, "Satori_129", 18),
                    (TouhouPetID.Koishi, "Koishi_205", 19),
                    (TouhouPetID.Satori, "Satori_130", 20),
                    (TouhouPetID.Koishi, "Koishi_206", 21),
                    (TouhouPetID.Satori, "Satori_131", 22),
                    (TouhouPetID.Koishi, "Koishi_207", 23),
                    (TouhouPetID.Satori, "Satori_132", 24),
                ];

                List<(TouhouPetID, string, int)> chatRoom52 =
                [
                    (TouhouPetID.Koishi, "Koishi_50", -1),
                    (TouhouPetID.Koishi, "Koishi_208", 0),
                    (TouhouPetID.Satori, "Satori_133", 1),
                    (TouhouPetID.Koishi, "Koishi_209", 2),
                    (TouhouPetID.Satori, "Satori_134", 3),
                    (TouhouPetID.Satori, "Satori_135", 4),
                    (TouhouPetID.Koishi, "Koishi_210", 5),
                    (TouhouPetID.Koishi, "Koishi_211", 6),
                    (TouhouPetID.Satori, "Satori_136", 7),
                    (TouhouPetID.Koishi, "Koishi_212", 8),
                    (TouhouPetID.Satori, "Satori_137", 9),
                    (TouhouPetID.Koishi, "Koishi_213", 10),
                    (TouhouPetID.Satori, "Satori_138", 11),
                    (TouhouPetID.Koishi, "Koishi_214", 12),
                    (TouhouPetID.Koishi, "Koishi_215", 13),
                    (TouhouPetID.Satori, "Satori_139", 14),
                    (TouhouPetID.Koishi, "Koishi_216", 15),
                    (TouhouPetID.Koishi, "Koishi_217", 16),
                    (TouhouPetID.Satori, "Satori_140", 17),
                    (TouhouPetID.Koishi, "Koishi_218", 18),
                    (TouhouPetID.Satori, "Satori_141", 19),
                    (TouhouPetID.Koishi, "Koishi_219", 20),
                    (TouhouPetID.Satori, "Satori_142", 21),
                ];

                List<(TouhouPetID, string, int)> chatRoom53 =
                [
                    (TouhouPetID.Koishi, "Koishi_51", -1),
                    (TouhouPetID.Kokoro, "Kokoro_74", 0),
                    (TouhouPetID.Rin, "Rin_56", 1),
                    (TouhouPetID.Utsuho, "Utsuho_79", 2),
                    (TouhouPetID.Satori, "Satori_143", 3),
                    (TouhouPetID.Koishi, "Koishi_220", 4),
                    (TouhouPetID.Satori, "Satori_144", 5),
                    (TouhouPetID.Koishi, "Koishi_221", 6),
                    (TouhouPetID.Kokoro, "Kokoro_75", 6),
                    (TouhouPetID.Rin, "Rin_57", 6),
                    (TouhouPetID.Utsuho, "Utsuho_80", 6),
                    (TouhouPetID.Koishi, "Koishi_222", 7),
                    (TouhouPetID.Kokoro, "Kokoro_76", 7),
                    (TouhouPetID.Rin, "Rin_58", 7),
                    (TouhouPetID.Utsuho, "Utsuho_81", 7),
                    (TouhouPetID.Satori, "Satori_145", 8),
                    (TouhouPetID.Satori, "Satori_146", 9),
                    (TouhouPetID.Satori, "Satori_147", 10),
                    (TouhouPetID.Satori, "Satori_148", 11),
                    (TouhouPetID.Utsuho, "Utsuho_82", 12),
                    (TouhouPetID.Rin, "Rin_59", 13),
                    (TouhouPetID.Kokoro, "Kokoro_77", 14),
                    (TouhouPetID.Koishi, "Koishi_223", 15),
                    (TouhouPetID.Satori, "Satori_149", 16),
                    (TouhouPetID.Satori, "Satori_150", 17),
                    (TouhouPetID.Utsuho, "Utsuho_83", 18),
                    (TouhouPetID.Koishi, "Koishi_224", 19),
                    (TouhouPetID.Kokoro, "Kokoro_78", 20),
                    (TouhouPetID.Rin, "Rin_60", 21),
                    (TouhouPetID.Satori, "Satori_151", 22),
                    (TouhouPetID.Satori, "Satori_152", 23),
                    (TouhouPetID.Koishi, "Koishi_225", 24),
                    (TouhouPetID.Kokoro, "Kokoro_79", 24),
                    (TouhouPetID.Rin, "Rin_61", 24),
                    (TouhouPetID.Utsuho, "Utsuho_84", 24),
                    (TouhouPetID.Satori, "Satori_153", 25),
                    (TouhouPetID.Koishi, "Koishi_226", 26),
                    (TouhouPetID.Kokoro, "Koishi_226", 26),
                    (TouhouPetID.Rin, "Koishi_226", 26),
                    (TouhouPetID.Utsuho, "Koishi_226", 26),
                    (TouhouPetID.Satori, "Koishi_226", 26),
                    (TouhouPetID.Utsuho, "Utsuho_85", 27),
                    (TouhouPetID.Rin, "Rin_62", 28),
                    (TouhouPetID.Kokoro, "Kokoro_80", 29),
                    (TouhouPetID.Koishi, "Koishi_227", 30),
                    (TouhouPetID.Satori, "Satori_154", 31),
                    (TouhouPetID.Koishi, "Koishi_228", 32),
                    (TouhouPetID.Satori, "Satori_155", 33),
                    (TouhouPetID.Utsuho, "Utsuho_86", 34),
                    (TouhouPetID.Satori, "Satori_156", 35),
                    (TouhouPetID.Koishi, "Koishi_229", 36),
                    (TouhouPetID.Kokoro, "Kokoro_81", 36),
                    (TouhouPetID.Rin, "Rin_63", 36),
                    (TouhouPetID.Satori, "Satori_157", 37),
                    (TouhouPetID.Koishi, "Koishi_230", 38),
                    (TouhouPetID.Koishi, "Koishi_231", 39),
                    (TouhouPetID.Kokoro, "Kokoro_82", 39),
                    (TouhouPetID.Utsuho, "Utsuho_87", 39),
                    (TouhouPetID.Rin, "Rin_64", 40),
                    (TouhouPetID.Satori, "Satori_158", 41),
                ];

                TouhouPetsAdd(TouhouPets, chatRoom1);
                TouhouPetsAdd(TouhouPets, chatRoom2);
                TouhouPetsAdd(TouhouPets, chatRoom3);
                TouhouPetsAdd(TouhouPets, chatRoom3_1);
                TouhouPetsAdd(TouhouPets, chatRoom4);
                TouhouPetsAdd(TouhouPets, chatRoom4_1);
                TouhouPetsAdd(TouhouPets, chatRoom5);
                TouhouPetsAdd(TouhouPets, chatRoom5_1);
                TouhouPetsAdd(TouhouPets, chatRoom6);
                TouhouPetsAdd(TouhouPets, chatRoom7);
                TouhouPetsAdd(TouhouPets, chatRoom8);
                TouhouPetsAdd(TouhouPets, chatRoom9);
                TouhouPetsAdd(TouhouPets, chatRoom10, true);
                TouhouPetsAdd(TouhouPets, chatRoom11);
                TouhouPetsAdd(TouhouPets, chatRoom12);
                TouhouPetsAdd(TouhouPets, chatRoom13);
                TouhouPetsAdd(TouhouPets, chatRoom14);
                TouhouPetsAdd(TouhouPets, chatRoom14_1);
                TouhouPetsAdd(TouhouPets, chatRoom15);
                TouhouPetsAdd(TouhouPets, chatRoom16);
                TouhouPetsAdd(TouhouPets, chatRoom17);
                TouhouPetsAdd(TouhouPets, chatRoom18);
                TouhouPetsAdd(TouhouPets, chatRoom19);
                TouhouPetsAdd(TouhouPets, chatRoom20);
                TouhouPetsAdd(TouhouPets, chatRoom21);
                TouhouPetsAdd(TouhouPets, chatRoom22);
                TouhouPetsAdd(TouhouPets, chatRoom23);
                TouhouPetsAdd(TouhouPets, chatRoom24);
                TouhouPetsAdd(TouhouPets, chatRoom25);
                TouhouPetsAdd(TouhouPets, chatRoom26);
                TouhouPetsAdd(TouhouPets, chatRoom27);
                TouhouPetsAdd(TouhouPets, chatRoom28);
                TouhouPetsAdd(TouhouPets, chatRoom28_1);
                TouhouPetsAdd(TouhouPets, chatRoom28_2);
                TouhouPetsAdd(TouhouPets, chatRoom28_3);
                TouhouPetsAdd(TouhouPets, chatRoom29);
                TouhouPetsAdd(TouhouPets, chatRoom29_1);
                TouhouPetsAdd(TouhouPets, chatRoom30);
                TouhouPetsAdd(TouhouPets, chatRoom31);
                TouhouPetsAdd(TouhouPets, chatRoom31_1);
                TouhouPetsAdd(TouhouPets, chatRoom32);
                TouhouPetsAdd(TouhouPets, chatRoom33);
                TouhouPetsAdd(TouhouPets, chatRoom34);
                TouhouPetsAdd(TouhouPets, chatRoom35);
                TouhouPetsAdd(TouhouPets, chatRoom36);
                TouhouPetsAdd(TouhouPets, chatRoom37);
                TouhouPetsAdd(TouhouPets, chatRoom38);
                TouhouPetsAdd(TouhouPets, chatRoom39);
                TouhouPetsAdd(TouhouPets, chatRoom40);
                TouhouPetsAdd(TouhouPets, chatRoom41);
                TouhouPetsAdd(TouhouPets, chatRoom42);
                TouhouPetsAdd(TouhouPets, chatRoom43);
                TouhouPetsAdd(TouhouPets, chatRoom43_1);
                TouhouPetsAdd(TouhouPets, chatRoom44);
                TouhouPetsAdd(TouhouPets, chatRoom45);
                TouhouPetsAdd(TouhouPets, chatRoom46);
                TouhouPetsAdd(TouhouPets, chatRoom47);
                TouhouPetsAdd(TouhouPets, chatRoom48);
                TouhouPetsAdd(TouhouPets, chatRoom49);
                TouhouPetsAdd(TouhouPets, chatRoom50);
                TouhouPetsAdd(TouhouPets, chatRoom51);
                TouhouPetsAdd(TouhouPets, chatRoom52);
                TouhouPetsAdd(TouhouPets, chatRoom53);
                //TODO: 做阿燐和恋恋、小五和阿空的召唤物
            }
            #endregion
        }
        #region Utils
        /// <summary>
        /// 为宠物添加平常随机或在特定条件下可能会说出的话。
        /// </summary>
        /// <param name="touhouPets">东方小伙伴的Mod实例</param>
        /// <param name="uniquePetID">宠物索引值，可参考 https://github.com/MineGame223/TouhouLittleFriend/blob/master/Common/TouhouPetUniqueID.cs</param>
        /// <param name="chatTextKey">与之相关话语文本的Key</param>
        /// <param name="condition">允许说出相关话语的条件</param>
        /// <param name="weight">相关话语的出现权重，值越大则出现几率越高</param>
        private void TouhouPetsAdd(Mod touhouPets, TouhouPetID uniquePetID, string chatTextKey, ref int[] keyCount, Func<bool> condition = null, int weight = 1)
        {
            touhouPets.Call("PetDialog", Mod, (int)uniquePetID, Language.GetText($"Mods.TouhouPetsEx.TouhouPets.{chatTextKey}_{keyCount[(int)uniquePetID]}"), condition, weight);

            keyCount[(int)uniquePetID]++;
        }
        /// <summary>
        /// 为宠物添加包含单个或多个宠物之间进行互动的聊天室，必须与 PetDialog 配合使用！
        /// </summary>
        /// <param name="touhouPets">东方小伙伴的Mod实例</param>
        /// <param name="chatRoomInfoList">列表中元组内的三个参数分别代表 宠物索引 ，文本Key 和 回合数</param>
        private void TouhouPetsAdd(Mod touhouPets, List<(TouhouPetID, string, int)> chatRoomInfoList, bool firstUsePrefix = false, string prefixKey = ".Response")
        {
            List<(int, LocalizedText, int)> chatRoom = [];
            bool first = true;

            foreach (var chatRoomInfo in chatRoomInfoList)
            {
                if (first && !firstUsePrefix)
                {
                    first = false;
                    chatRoom.Add(new((int)chatRoomInfo.Item1, Language.GetText($"Mods.TouhouPetsEx.TouhouPets.{chatRoomInfo.Item2}"), chatRoomInfo.Item3));
                }
                else
                    chatRoom.Add(new((int)chatRoomInfo.Item1, Language.GetText($"Mods.TouhouPetsEx.TouhouPets{prefixKey}.{chatRoomInfo.Item2}"), chatRoomInfo.Item3));
            }

            touhouPets.Call("PetChatRoom", Mod, chatRoom);
        }
        /// <summary>
        /// 为宠物添加包含单个或多个宠物之间进行互动的聊天室，必须与 PetDialog 配合使用！
        /// <br>非常规样式版本，可用于为原版或可能存在的其它模组的对话续写聊天室</br>
        /// </summary>
        /// <param name="touhouPets">东方小伙伴的Mod实例</param>
        /// <param name="chatRoomInfoList">列表中元组内的三个参数分别代表 宠物索引 ，文本Key 和 回合数</param>
        private void TouhouPetsAdd(Mod touhouPets, List<(TouhouPetID, string, int)> chatRoomInfoList, string firstKey, string prefixKey = ".Response")
        {
            List<(int, LocalizedText, int)> chatRoom = [];
            bool first = true;

            foreach (var chatRoomInfo in chatRoomInfoList)
            {
                if (first)
                {
                    first = false;
                    chatRoom.Add(new((int)chatRoomInfo.Item1, Language.GetText(firstKey), -1));
                }
                else
                    chatRoom.Add(new((int)chatRoomInfo.Item1, Language.GetText($"Mods.TouhouPetsEx.TouhouPets{prefixKey}.{chatRoomInfo.Item2}"), chatRoomInfo.Item3));
            }

            touhouPets.Call("PetChatRoom", Mod, chatRoom);
        }
        public static bool HasPets(params int[] petTypes)
        {
            foreach (int type in petTypes)
            {
                if (Main.LocalPlayer.ownedProjectileCounts[type] == 0)
                    return false;
            }

            return true;
        }
        public static bool NotHasPets(params int[] petTypes)
        {
            foreach (int type in petTypes)
            {
                if (Main.LocalPlayer.ownedProjectileCounts[type] > 0)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
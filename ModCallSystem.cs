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
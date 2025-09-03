using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Buffs;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Core
{
    public class EnhancePlayers : ModPlayer
    {
        public List<int> ActiveEnhance = [];
        public List<int> ActivePassiveEnhance = [];
        public int ActiveEnhanceCount = 11037;
        /// <summary>
        /// ����˿��
        /// </summary>
        public int EatBook = 0;
        /// <summary>
        /// ��������
        /// </summary>
        public int DaiyouseiCD = 0;
        /// <summary>
        /// �������
        /// </summary>
        public int LilyCD = 0;
        /// <summary>
        /// ��������Buff�ã��������������أ�
        /// </summary>
        public bool FragrantAromaFillsTheAir = false;
        /// <summary>
        /// ���������
        /// </summary>
        public int FragrantAromaFillsTheAirCD = 0;
        /// <summary>
        /// �ٵ���
        /// </summary>
        public int LettyCD = 0;
        /// <summary>
        /// ������
        /// </summary>
        public int LyricaCD = 0;
        /// <summary>
        /// ��������
        /// </summary>
        public int YukariCD = 0;
        /// <summary>
        /// ���¶��
        /// </summary>
        public int WriggleCD = 0;
        /// <summary>
        /// ��˹�����
        /// </summary>
        public int MystiaCD = 0;
        /// <summary>
        /// �ϰ�������ã�����1��¼������һ��buff��type
        /// </summary>
        public int[] KeineCD = [0, -1];
        /// <summary>
        /// ����ɽ��ҹ��
        /// </summary>
        public int[] OldBuff;
        /// <summary>
        /// �ӳǺ�ȡ�ã���¼ԭ��ϳ�վ
        /// </summary>
        public bool[] adjTileVanilla;
        /// <summary>
        /// �ӳǺ�ȡ�ã���¼ģ��ϳ�վ
        /// </summary>
        public List<string> adjTileMod;
        /// <summary>
        /// �ӳǺ�ȡ�ã���¼ȫ���ϳ�վ
        /// </summary>
        public bool[] adjTile;
        /// <summary>
        /// �ӳǺ�ȡ�ã���¼ˮ�����ۡ��ҽ���΢�⡢��ҩ��
        /// </summary>
        public bool[] adjOther;
        /// <summary>
        /// �����������
        /// </summary>
        public int SanaeCD;
        /// <summary>
        /// ����ٰ�����
        /// <para>����������Ӧ�ļӳɣ�0���ƶ��ٶȡ�1���ڿ��ٶȡ�2���������ֵ��3���������ֵ��4���ҽ�����ʱ�䡢5���˺����⡢6�������˺���7/8/9��������10���ٷֱȴ���</para>
        /// </summary>
        public int[] ExtraAddition = new int[11];
        /// <summary>
        /// ����ٰ�����
        /// <para>����������Ӧ�ļӳ����ޣ�0���ƶ��ٶȡ�1���ڿ��ٶȡ�2���������ֵ��3���������ֵ��4���ҽ�����ʱ�䡢5���˺����⡢6�������˺���7/8/9��������10���ٷֱȴ���</para>
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
        private static void ProcessDemonismAction(Action<BaseEnhance> action)
        {
            foreach (BaseEnhance enhance in TouhouPetsEx.GEnhanceInstances.Values)
            {
                action(enhance);
            }
        }
        private static bool? ProcessDemonismAction(Player player, bool? priority, Func<BaseEnhance, bool?> action)
        {
            if (!player.HasTouhouPetsBuff())
                return null;

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
        private static float ProcessDemonismAction(Player player, Func<BaseEnhance, float?> action)
        {
            float multiplier = 1f;

            if (!player.HasTouhouPetsBuff())
                return multiplier;

            foreach (int id in player.MP().ActiveEnhance.Concat(player.MP().ActivePassiveEnhance))
            {
                multiplier *= action(TouhouPetsEx.GEnhanceInstances[id]) ?? 1f;
            }

            return multiplier;
        }
        public override void Initialize()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerInitialize(Player));
        }
        public override void ResetEffects()
        {
            ActiveEnhanceCount = 1;

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerResetEffects(Player));
            ProcessDemonismAction((enhance) => enhance.PlayerResetEffectsAlways(Player));

            ActivePassiveEnhance = [];

            FragrantAromaFillsTheAir = false;

            foreach (Item item in Player.miscEquips)
            {
                if (item.ModItem?.Mod.Name == "TouhouPets" && TouhouPetsEx.GEnhanceInstances.TryGetValue(item.type, out var enhance) && enhance.Passive)
                    ActivePassiveEnhance.Add(item.type);
            }
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
            tag.Add("adjTileVanilla", adjTileVanilla);
            tag.Add("adjTileMod", adjTileMod);
            tag.Add("adjOther", adjOther);
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
            if (tag.TryGet<bool[]>("adjTileVanilla", out var adjtileVanilla))
                adjTileVanilla = adjtileVanilla;
            else
                adjTileVanilla = new bool[TileID.Count];
            if (tag.TryGet<List<string>>("adjTileMod", out var adjtileMod))
                adjTileMod = adjtileMod;
            else
                adjTileMod = [];
            if (tag.TryGet<bool[]>("adjOther", out var adjother))
                adjOther = adjother;
            else
            adjOther = new bool[5];
            if (tag.GetIntArray("ExtraAddition").Length != 0) ExtraAddition = tag.GetIntArray("ExtraAddition");

            adjTile = (bool[])Player.adjTile.Clone();

            for (int i = 0; i < adjTileVanilla.Length; i++)
            {
                adjTile[i] = adjTileVanilla[i];
            }

            foreach (string fullName in adjTileMod)
            {
                string[] parts = fullName.Split('/');
                if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModTile tile))
                    adjTile[tile.Type] = true;
            }
        }
        public override void ModifyLuck(ref float luck)
        {
            float luck2 = luck;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyLuck(Player, ref luck2));
            luck = luck2;
        }
        public override void UpdateLifeRegen()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerUpdateLifeRegen(Player));
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            int healValue2 = healValue;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerGetHealLife(Player, item, quickHeal, ref healValue2));
            healValue = healValue2;
        }
        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            int healValue2 = healValue;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerGetHealMana(Player, item, quickHeal, ref healValue2));
            healValue = healValue2;
        }
        public override void PreUpdate()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPreUpdate(Player));
        }
        public override void PreUpdateBuffs()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPreUpdateBuffs(Player));
            ProcessDemonismAction((enhance) => enhance.PlayerPreUpdateBuffsAlways(Player));
        }
        public override void PostUpdateBuffs()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdateBuffs(Player));
        }
        public override void PostUpdateEquips()
        {
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdateEquips(Player));
        }
        public override void PostUpdate()
        {
            if (FragrantAromaFillsTheAir && TouhouPetsExModSystem.SynchronousTime % 60 == 37)
                Player.statLife += Math.Clamp(Player.statLifeMax2 / 100, 0, Player.statLifeMax2 - Player.statLife);

            if (Main.GameUpdateCount % 18000 == 12000 && Player == Main.LocalPlayer)
                AwardPlayerSync(Mod, -1, Main.myPlayer);

            ProcessDemonismAction(Player, (enhance) => enhance.PlayerPostUpdate(Player));
        }
        public override void ModifyItemScale(Item item, ref float scale)
        {
            float scale2 = scale;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyItemScale(Player, item, ref scale2));
            scale = scale2;
        }
        public override float UseTimeMultiplier(Item item)
        {
            return ProcessDemonismAction(Player, (enhance) => enhance.PlayerUseTimeMultiplier(Player, item));
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
            if (info.DamageSource.SourceNPCIndex > -1)
            {
                NPC npc = Main.npc[info.DamageSource.SourceNPCIndex];
                if (npc.GetGlobalNPC<GEnhanceNPCs>().MoonMist && Main.rand.NextBool(10))
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TownSlimeTransform, new ParticleOrchestraSettings
                    {
                        UniqueInfoPiece = 1,
                        PositionInWorld = Player.Center + (npc.Center - Player.Center) / 4f,
                        MovementVector = Vector2.Zero
                    });

                    Player.immune = true;
                    Player.immuneTime += Player.longInvince ? 60 : 20;
                    for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                    {
                        Player.hurtCooldowns[i] += Player.longInvince ? 60 : 20;
                    }

                    return true;
                }
            }

            bool? reesult = ProcessDemonismAction(Player, true, (enhance) => enhance.PlayerFreeDodge(Player, info));

            return reesult ?? base.FreeDodge(info);
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            Player.HurtModifiers modifiers2 = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHurt(Player, ref modifiers2));
            modifiers = modifiers2;
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            NPC.HitModifiers modifiers2 = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHitNPCWithItem(Player, item, target, ref modifiers2));
            modifiers = modifiers2;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            NPC.HitModifiers modifiers2 = modifiers;
            ProcessDemonismAction(Player, (enhance) => enhance.PlayerModifyHitNPCWithProjectile(Player, proj, target, ref modifiers2));
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
            if (Player == Main.LocalPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                AwardPlayerSync(Mod, -1, Main.myPlayer, true);
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TouhouPetsExModSystem.ReisenKeyBind.JustPressed && Config.Reisen && Main.LocalPlayer.EnableEnhance<ReisenGun>())
            {
                List<int> blackList = [];
                foreach (string fullName in Config.Letty_2_1 ?? [])
                {
                    string[] parts = fullName.Split('/');
                    if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) && ModLoader.TryGetMod(parts[0], out Mod mod) && mod.TryFind(parts[1], out ModProjectile proj))
                        blackList.Add(proj.Type);
                }

                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.owner != Main.LocalPlayer.whoAmI || proj.type == ProjectileID.ChlorophyteBullet || (proj.type >= ProjectileID.Count && blackList.Contains(proj.type)) || !proj.GetGlobalProjectile<GEnhanceProjectile>().Bullet)
                        continue;

                    int target = proj.FindTargetWithLineOfSight();

                    if (target == -1)
                        continue;

                    proj.velocity = proj.DirectionTo(Main.npc[target].Center).SafeNormalize(-Vector2.UnitY) * proj.velocity.Length();
                    proj.GetGlobalProjectile<GEnhanceProjectile>().Bullet = false;
                }

                Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<ReisenEffect>(), 0, 0, Main.LocalPlayer.whoAmI, ai1: 1);
            }
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

            //Main.NewText("��");
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
            for (int i = 0; i < activeEnhanceCount; i++)
                activeEnhance.Add(reader.ReadInt32());
            player.ActiveEnhance = activeEnhance;

            int extraAdditionLength = reader.ReadInt32();
            int[] extraAddition = new int[extraAdditionLength];
            for (int i = 0; i < extraAdditionLength; i++)
                extraAddition[i] = reader.ReadInt32();
            player.ExtraAddition = extraAddition;

            if (award)
                AwardPlayerSync(TouhouPetsEx.Instance, whoAmI, Main.myPlayer);

            //Main.NewText("��");
        }
        public static void YukariTp(Player player, Vector2 newPos)
        {
            player.RemoveAllGrapplingHooks();
            player.StopVanityActions();

            if (player.shimmering || player.shimmerWet)
            {
                player.shimmering = false;
                player.shimmerWet = false;
                player.wet = false;
                player.ClearBuff(353);
            }

            SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_warpl") with { MaxInstances = 114514 }, player.Center);
            for (int i = player.width * player.height / 5; i >= 0; i--)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.TeleportationPotion);
                dust.scale = Main.rand.Next(20, 70) * 0.01f;

                if (i < 10)
                    dust.scale += 0.25f;

                if (i < 5)
                    dust.scale += 0.25f;
            }

            PressurePlateHelper.UpdatePlayerPosition(player);
            player.position = newPos;
            player.fallStart = (int)player.position.Y / 16;
            PressurePlateHelper.UpdatePlayerPosition(player);
            player.ResetAdvancedShadows();
            for (int i = 0; i < 3; i++)
            {
                player.UpdateSocialShadow();
            }

            player.oldPosition = player.position + player.BlehOldPositionFixer;

            player.MP().YukariCD = 60;

            SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/se_warpl") with { MaxInstances = 114514 }, player == Main.LocalPlayer ? null : player.Center);
            for (int i = player.width * player.height / 5; i >= 0; i--)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.TeleportationPotion);
                dust.scale = Main.rand.Next(20, 70) * 0.01f;

                if (i < 10)
                    dust.scale += 0.25f;

                if (i < 5)
                    dust.scale += 0.25f;
            }
        }
    }
}

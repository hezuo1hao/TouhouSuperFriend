using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.Chat;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using TouhouPets;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Items.PetItems;
using TouhouPets.Content.Projectiles.Pets;
using TouhouPetsEx.Achievements;
using TouhouPetsEx.Projectiles;

namespace TouhouPetsEx.Enhance.Core
{
    /// <summary>
    /// 使用 tML 的 On/IL/MonoModHooks 方式注入增强逻辑（<see cref="ModSystem"/>）。
    /// <para>
    /// 这个类主要负责把部分“无法通过常规 Global/ModPlayer 钩子覆盖”的行为，挂到 Terraria 的原方法上。
    /// </para>
    /// </summary>
	public class EnhanceOn : ModSystem
    {
        static IDictionary<string, Mod> modsByName = new Dictionary<string, Mod>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 用于绘制特定弹幕特效
        /// </summary>
        static RenderTarget2D Render;
        public override void Unload()
        {
            Main.RunOnMainThread(() => Render?.Dispose());
        }
        /// <summary>
        /// 注册所有 On/IL/MonoModHooks 钩子。
        /// </summary>
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                Main.RunOnMainThread(() =>
                {
                    Render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    Main.OnResolutionChanged += Main_OnResolutionChanged;
                });
            On_FilterManager.EndCapture += On_FilterManager_EndCapture;
            // 玩家属性相关（伤害/攻速/暴击/穿甲等）。
            On_Player.GetDamage += On_Player_GetDamage;
            On_Player.GetCritChance += On_Player_GetCritChance;
            On_Player.GetAttackSpeed += On_Player_GetAttackSpeed;
            On_Player.GetKnockback += On_Player_GetKnockback;
            On_Player.GetArmorPenetration += On_Player_GetArmorPenetration;
            On_Player.VanillaBaseDefenseEffectiveness += On_Player_VanillaBaseDefenseEffectiveness;
            On_Player.GetPickaxeDamage += On_Player_GetPickaxeDamage;
            // 运气相关：多个来源会用到同一套加成逻辑。
            On_Player.RollLuck += LuckUp;
            On_NPC.HitModifiers.GetDamage += ExCrit;
            On_NPC.getGoodAdjustments += On_NPC_getGoodAdjustments;
            On_NPC.StrikeNPC_HitInfo_bool_bool += SuperCrit;
            On_NPC.NPCLoot_DropMoney += LuckUp;
            // 地图与世界事件。
            Main.OnPostFullscreenMapDraw += TeleportFromMap;
            On_Main.DamageVar_float_int_float += LuckUp;
            On_WorldGen.ShakeTree += On_WorldGen_ShakeTree;
            On_BirthdayParty.NaturalAttempt += On_BirthdayParty_NaturalAttempt;
            On_Main.UpdateTime_StartNight += On_Main_UpdateTime_StartNight;
            On_WorldGen.UpdateWorld_GrassGrowth += On_WorldGen_UpdateWorld_GrassGrowth;
            On_ShopHelper.LimitAndRoundMultiplier += On_ShopHelper_LimitAndRoundMultiplier;
            On_ShopHelper.ProcessMood += On_ShopHelper_ProcessMood;
            On_Gore.NewGore_IEntitySource_Vector2_Vector2_int_float += On_Gore_NewGore_IEntitySource_Vector2_Vector2_int_float;
            // 叠叠叠叠叠叠到厌倦~
            if (Main.netMode != NetmodeID.Server && ModLoader.TryGetMod("TouhouPetsExOptimization", out Mod m))
            {
                On_RemadeChatMonitor.AddNewMessage += On_RemadeChatMonitor_AddNewMessage;
                MonoModHooks.Add(typeof(ModLoader).GetMethod("get_Mods", BindingFlags.Static | BindingFlags.Public), On_SetMods);
                MonoModHooks.Add(typeof(ModLoader).GetMethod("GetMod", BindingFlags.Static | BindingFlags.Public), On_GetMod);
                MonoModHooks.Add(typeof(ModLoader).GetMethod("TryGetMod", BindingFlags.Static | BindingFlags.Public), On_TryGetMod);
                MonoModHooks.Add(typeof(ModLoader).GetMethod("HasMod", BindingFlags.Static | BindingFlags.Public), On_HasMod);
            }
            // IL 注入：对原版 AdjTiles 做扩展（河城荷取相关）。
            IL_Player.AdjTiles += IL_Player_AdjTiles;
            // 对 TouhouPets 的部分行为做补丁。
            MonoModHooks.Add(typeof(Koishi).GetMethod("ShouldKillPlayer", BindingFlags.Instance | BindingFlags.NonPublic), On_ShouldKillPlayer);
            MonoModHooks.Add(typeof(BasicTouhouPet).GetMethod("MoveToPoint", BindingFlags.Instance | BindingFlags.NonPublic), On_MoveToPoint);
            MonoModHooks.Add(typeof(BasicTouhouPet).GetMethod("ChangeDir", BindingFlags.Instance | BindingFlags.NonPublic), On_ChangeDir);
            MonoModHooks.Add(typeof(Koishi).GetMethod("RegularDialogText", BindingFlags.Instance | BindingFlags.Public), On_RegularDialogText);
            MonoModHooks.Add(typeof(ChatRoomHelper).GetMethod("SetChat_Inner", BindingFlags.Static | BindingFlags.NonPublic), On_SetChat_Inner);

            // 锤子敲背景墙有特殊处理，要用On才能应用工具速度提升
            On_Player.ItemCheck_UseMiningTools_TryHittingWall += (orig, player, item, x, y) =>
            {
                orig.Invoke(player, item, x, y);

                // 检测那一堆if判断成没成
                if (player.itemTime == item.useTime / 2 && player.EnableEnhance<FlandrePudding>())
                    player.itemTime = (int)Math.Max(1, item.useTime / 4f);
            };
        }
        (Vector2, float)[] speedLine = new(Vector2, float)[30];
        private void On_FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            var spriteBatch = Main.spriteBatch;
            var player = Main.LocalPlayer;

            if (Config.Aya && player.EnableEnhance<AyaCamera>())
            {
                var tex = TextureAssets.MagicPixel.Value;
                bool left = player.velocity.X < 0;
                bool newLine = !Main.gamePaused && Math.Abs(player.velocity.X) > 3 && player.velocity.Y != 0 && Main.GameUpdateCount % 3 == 0 && Main.rand.NextBool(2);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                for (int i = 0; i < speedLine.Length; i++)
                {
                    ref var line = ref speedLine[i];
                    float length = line.Item2 * 5;
                    Vector2 pos = line.Item1 - Main.screenPosition;

                    if (!Main.gamePaused)
                        line.Item1.X += line.Item2;

                    if ((pos.X < length && !left) || (pos.X > Main.screenWidth + length && left) || pos.Y < 0 || pos.Y > Main.screenHeight)
                    {
                        line.Item1 = Vector2.Zero;
                        line.Item2 = 0;
                    }

                    if (newLine && line.Item1 == Vector2.Zero)
                    {
                        line.Item2 = Main.rand.NextFloat(16.00f, 32.00f) * (left ? 1 : -1);
                        line.Item1 = Main.screenPosition + (new Vector2(left ? 0 : Main.screenWidth, 50 + Main.rand.NextFloat(Main.screenHeight - 100.00f))) + Vector2.UnitX * line.Item2 * -3;
                        newLine = false;
                    }

                    if (line.Item1 != Vector2.Zero)
                        spriteBatch.Draw(tex, pos, null, Color.White, 1.57f, tex.Size() / 2f, new Vector2(4, line.Item2 / 200f), SpriteEffects.None, 0);
                }

                spriteBatch.End();
            }

            bool perfectMaid = false;
            bool reisenEffect = false;
            int perfectMaidType = ModContent.ProjectileType<PerfectMaid>();
            int reisenEffectType = ModContent.ProjectileType<ReisenEffect>();
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == perfectMaidType)
                    perfectMaid = true;

                if (proj.type == reisenEffectType && proj.ai[1] != 1)
                    reisenEffect = true;
            }

            bool draw = perfectMaid || reisenEffect;

            if (!draw)
            {
                orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
                return;
            }
            var graphicsDevice = Main.instance.GraphicsDevice;
            var screenTarget = screenTarget1;
            var screenTargetSwap = screenTarget2;

            if (perfectMaid)
            {
                var shader = TouhouPetsEx.GrayishWhiteShader;

                graphicsDevice.SetRenderTarget(screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
                spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                graphicsDevice.SetRenderTarget(Render);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                TouhouPetsEx.RingShader.Parameters["width"].SetValue(0);
                TouhouPetsEx.RingShader.CurrentTechnique.Passes[0].Apply();

                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.type == perfectMaidType)
                    {
                        var dPosition = proj.Center - Main.screenPosition;

                        spriteBatch.Draw(TextureAssets.MagicPixel.Value, dPosition, null, Color.White * ((255 - proj.alpha) / 255f), 0, TextureAssets.MagicPixel.Size() / 2f, new Vector2(proj.width, proj.width * 0.001f), SpriteEffects.None, 0);
                    }
                }

                spriteBatch.End();
                graphicsDevice.SetRenderTarget(screenTarget);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
                Main.instance.GraphicsDevice.Textures[1] = Render;
                shader.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(screenTargetSwap, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            if (reisenEffect)
            {
                var shader = TouhouPetsEx.DistortShader;
                var tex = TextureAssets.Projectile[reisenEffectType].Value;

                graphicsDevice.SetRenderTarget(screenTargetSwap);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
                spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                graphicsDevice.SetRenderTarget(Render);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.type == reisenEffectType)
                    {
                        var dPosition = proj.Center - Main.screenPosition;

                        spriteBatch.Draw(tex, dPosition, null, Color.White * ((255 - proj.alpha) / 255f), 0, tex.Size() / 2f, proj.ai[0] * proj.ai[0] / 300f, SpriteEffects.None, 0);
                    }
                }

                spriteBatch.End();
                graphicsDevice.SetRenderTarget(screenTarget);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
                shader.Parameters["tex0"].SetValue(Render);
                shader.Parameters["mult"].SetValue(0.02f);
                shader.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(screenTargetSwap, Vector2.Zero, Color.White);

                spriteBatch.End();
            }

            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        private void Main_OnResolutionChanged(Vector2 obj)
        {
            Render?.Dispose();
            Render = new(Main.graphics.GraphicsDevice, (int)obj.X, (int)obj.Y);
        }
        bool gggggg;
        private void On_RemadeChatMonitor_AddNewMessage(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
        {
            StackTrace stackTrace = new StackTrace();
            bool isCalledByTargetMod = false;
            string callingMethodInfo = "";

            // 遍历调用堆栈，检查是否来自目标模组
            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame?.GetMethod();

                if (method == null) continue;

                // 跳过当前方法和On框架方法
                if (method.Name.Contains("On_Main_NewText") ||
                    method.DeclaringType?.FullName?.Contains("On.Terraria") == true)
                    continue;

                string typeName = method.DeclaringType?.FullName ?? "";
                string methodName = method.Name;

                // 检查是否来自目标模组
                if (typeName.Contains("TouhouPetsExOptimization"))
                {
                    isCalledByTargetMod = true;
                    callingMethodInfo = $"{typeName}.{methodName}";
                    break;
                }
            }

            if (isCalledByTargetMod)
            {
                if (!gggggg)
                {
                    string a = ModLoader.GetMod("TouhouPetsExOptimization").DisplayName;
                    text = $"侦测到开启 {a}，请注意在此情况下产生的任何问题/报错/BUG均有可能是因为该模组导致（由于该模组使用了大量破坏/不兼容性代码），请不要在 {Mod.DisplayName} 处反馈\n" +
                        $"注：由于 {Mod.DisplayName} 的底层代码变动与底层优化，实质上 {a} 所提供的优化已失效，但由于对面刻意编写了恶意代码——旧版模拟，所以可能在同时加载两个模组时游戏体验被劣化\n" +
                        $"注：由于 {a} 使用了头痛砍头的优化方案与编写的恶意代码共同影响下，本模组的不少功能会受到其影响无法使用";
                    color = Color.Red;
                    gggggg = true;
                }
                else return;
            }

            orig(self, text, color, widthLimitInPixels);
        }

        private delegate Mod[] SetModsDelegate();
        private Mod[] On_SetMods(SetModsDelegate orig)
        {
            StackTrace stackTrace = new StackTrace();
            bool isCalledByTargetMod = false;
            string callingMethodInfo = "";

            // 遍历调用堆栈，检查是否来自目标模组
            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame?.GetMethod();

                if (method == null) continue;

                // 跳过当前方法和On框架方法
                if (method.Name.Contains("On_Main_NewText") ||
                    method.DeclaringType?.FullName?.Contains("On.Terraria") == true)
                    continue;

                string typeName = method.DeclaringType?.FullName ?? "";
                string methodName = method.Name;

                // 检查是否来自目标模组
                if (typeName.Contains("TouhouPetsExOptimization"))
                {
                    isCalledByTargetMod = true;
                    callingMethodInfo = $"{typeName}.{methodName}";
                    break;
                }
            }

            if (isCalledByTargetMod)
                return [];

            return orig();
        }
        private delegate bool HasModDelegate(string name);
        private bool On_HasMod(HasModDelegate orig, string name)
        {
            if (modsByName.Count == 0)
            {
                modsByName = new Dictionary<string, Mod>(ModLoader.modsByName);
                ModLoader.modsByName.Clear();
            }

            StackTrace stackTrace = new StackTrace();
            bool isCalledByTargetMod = false;
            string callingMethodInfo = "";

            // 遍历调用堆栈，检查是否来自目标模组
            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame?.GetMethod();

                if (method == null) continue;

                // 跳过当前方法和On框架方法
                if (method.Name.Contains("On_Main_NewText") ||
                    method.DeclaringType?.FullName?.Contains("On.Terraria") == true)
                    continue;

                string typeName = method.DeclaringType?.FullName ?? "";
                string methodName = method.Name;

                // 检查是否来自目标模组
                if (typeName.Contains("TouhouPetsExOptimization"))
                {
                    isCalledByTargetMod = true;
                    callingMethodInfo = $"{typeName}.{methodName}";
                    break;
                }
            }

            if (isCalledByTargetMod && name == Mod.Name)
                return false;

            return modsByName.ContainsKey(name);
        }
        private delegate Mod GetModDelegate(string name);
        private Mod On_GetMod(GetModDelegate orig, string name)
        {
            if (modsByName.Count == 0)
            {
                modsByName = new Dictionary<string, Mod>(ModLoader.modsByName);
                ModLoader.modsByName.Clear();
            }

            StackTrace stackTrace = new StackTrace();
            bool isCalledByTargetMod = false;
            string callingMethodInfo = "";

            // 遍历调用堆栈，检查是否来自目标模组
            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame?.GetMethod();

                if (method == null) continue;

                // 跳过当前方法和On框架方法
                if (method.Name.Contains("On_Main_NewText") ||
                    method.DeclaringType?.FullName?.Contains("On.Terraria") == true)
                    continue;

                string typeName = method.DeclaringType?.FullName ?? "";
                string methodName = method.Name;

                // 检查是否来自目标模组
                if (typeName.Contains("TouhouPetsExOptimization"))
                {
                    isCalledByTargetMod = true;
                    callingMethodInfo = $"{typeName}.{methodName}";
                    break;
                }
            }

            if (isCalledByTargetMod && name == Mod.Name)
                return null;

            return modsByName[name];
        }
        private delegate bool TryGetModDelegate(string name, out Mod result);
        private bool On_TryGetMod(TryGetModDelegate orig, string name, out Mod result)
        {
            if (modsByName.Count == 0)
            {
                modsByName = new Dictionary<string, Mod>(ModLoader.modsByName);
                ModLoader.modsByName.Clear();
            }

            StackTrace stackTrace = new StackTrace();
            bool isCalledByTargetMod = false;
            string callingMethodInfo = "";

            // 遍历调用堆栈，检查是否来自目标模组
            for (int i = 2; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame?.GetMethod();

                if (method == null) continue;

                // 跳过当前方法和On框架方法
                if (method.Name.Contains("On_Main_NewText") ||
                    method.DeclaringType?.FullName?.Contains("On.Terraria") == true)
                    continue;

                string typeName = method.DeclaringType?.FullName ?? "";
                string methodName = method.Name;

                // 检查是否来自目标模组
                if (typeName.Contains("TouhouPetsExOptimization"))
                {
                    isCalledByTargetMod = true;
                    callingMethodInfo = $"{typeName}.{methodName}";
                    break;
                }
            }

            if (isCalledByTargetMod && name == Mod.Name)
            {
                result = null;
                return false;
            }

            return modsByName.TryGetValue(name, out result);
        }
        static bool delicacy;
        private delegate void SetChat_InnerDelegate(Projectile projectile, ChatSettingConfig config, int lag, LocalizedText text, bool forcely);
        /// <summary>
        /// 聊天室文本设置钩子：用于对特定台词触发额外音效/战斗文字（恋/魔理沙等彩蛋）。
        /// </summary>
        private static void On_SetChat_Inner(SetChat_InnerDelegate orig, Projectile projectile, ChatSettingConfig config, int lag, LocalizedText text, bool forcely)
        {
            // 先执行原逻辑，确保聊天文本先被写入。
            orig(projectile, config, lag, text, forcely);

            //若被设置对象并非东方宠物，则不再执行后续
            if (!projectile.IsATouhouPet())
                return;

            BasicTouhouPet pet = projectile.AsTouhouPet();

            //若宠物非玩家本人召唤，则不再执行后续
            if (projectile.owner != Main.myPlayer)
                return;

            if (pet.chatText == Language.GetText("Mods.TouhouPetsEx.TouhouPets.Response.Kokoro_58") || pet.chatText == Language.GetText("Mods.TouhouPetsEx.TouhouPets.Response.Marisa_56") ||
                pet.chatText == Language.GetText("Mods.TouhouPetsEx.TouhouPets.Response.Utsuho_58_1") || pet.chatText == Language.GetText("Mods.TouhouPetsEx.TouhouPets.Response.Rin_47"))
            {
                if (delicacy)
                    return;

                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/delicacy"), Main.player[projectile.owner].Center);
                delicacy = true;
            }
            else if (pet.chatText == Language.GetText("Mods.TouhouPetsEx.TouhouPets.Response.Marisa_59") && pet.chatTimeLeft < 10)
            {
                if (delicacy)
                    return;

                Main.combatText[CombatText.NewText(Main.LocalPlayer.getRect(), Color.Pink, GetText("TouhouPets.Response.Extra_1"))].lifeTime *= 3;
                delicacy = true;
            }
            else if (pet.chatText == Language.GetText("Mods.TouhouPetsEx.TouhouPets.Response.Utsuho_58"))
            {
                if (delicacy)
                    return;

                SoundEngine.PlaySound(new SoundStyle("TouhouPetsEx/Sound/delicacyFail"), Main.player[projectile.owner].Center);
                delicacy = true;
            }
            else delicacy = false;
        }
        private delegate WeightedRandom<LocalizedText> RegularDialogTextDelegate(Koishi self);
        private WeightedRandom<LocalizedText> On_RegularDialogText(RegularDialogTextDelegate orig, Koishi self)
        {
            var rand = orig(self);

            // 小五
            if (ModCallSystem.HasPets(ModContent.ProjectileType<Satori>()))
            {
                double weight = 0.333;

                // 没有魔理沙
                if (!LocalConfig.MarisaKoishi || ModCallSystem.NotHasPets(ModContent.ProjectileType<Marisa>()))
                {
                    weight = 0.25;
                    rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_18"), weight);
                }

                rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_19"), weight);
                rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_20"), weight);
                rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_49"), weight);

                // 地灵殿一家
                if (ModCallSystem.HasPets(ModContent.ProjectileType<Utsuho>(), ModContent.ProjectileType<Rin>()))
                {
                    // 只有地灵殿一家
                    if (ModCallSystem.NotHasPets(ModContent.ProjectileType<Marisa>(), ModContent.ProjectileType<Kokoro>()))
                    {
                        weight = 0.5;
                        rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_47"), weight);
                        rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_48"), weight);
                    }
                }
            }

            // 没有小五
            if (ModCallSystem.NotHasPets(ModContent.ProjectileType<Satori>()))
            {
                // 秦心
                if (ModCallSystem.HasPets(ModContent.ProjectileType<Kokoro>()))
                {
                    double weight = 0.5;

                    if (LocalConfig.Box5)
                        rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_41"), weight);
                    else
                        rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_40"), weight);

                    rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_39"), weight);
                }

                // 魔理沙
                if (LocalConfig.MarisaKoishi && ModCallSystem.HasPets(ModContent.ProjectileType<Marisa>()))
                {
                    double weight = 0.333;
                    rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_42"), weight);
                    rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_43"), weight);
                    rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_44"), weight);
                }

                // 阿空
                if (ModCallSystem.HasPets(ModContent.ProjectileType<Utsuho>()))
                {
                    double weight = 0.5;
                    rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_45"), weight);
                    rand.Add(Language.GetText("Mods.TouhouPetsEx.TouhouPets.Koishi_46"), weight);
                }
            }

            return rand;
        }
        private delegate void ChangeDirDelegate(BasicTouhouPet self, float dist);
        private void On_ChangeDir(ChangeDirDelegate orig, BasicTouhouPet self, float dist)
        {
            if (self.Type == ModContent.ProjectileType<Marisa>() && self.Owner.HasBuff(ModContent.BuffType<KomeijiBuff>()))
                dist = 150;

            orig(self, dist);
        }
        private delegate void MoveToPointDelegate(BasicTouhouPet self, Vector2 point, float speed, Vector2 center);
        private void On_MoveToPoint(MoveToPointDelegate orig, BasicTouhouPet self, Vector2 point, float speed, Vector2 center)
        {
            if (self.Type == ModContent.ProjectileType<Marisa>() && self.Owner.HasBuff(ModContent.BuffType<KomeijiBuff>()))
                point = new Vector2(-90 * self.Owner.direction, -60 + self.Owner.gfxOffY);

            orig(self, point, speed, center);
        }
        private delegate bool ShouldKillPlayerDelegate(Koishi koishi);
        private bool On_ShouldKillPlayer(ShouldKillPlayerDelegate orig, Koishi koishi)
        {
            if (koishi.Owner.HasBuff(ModContent.BuffType<MarisaBuff>()))
                return false;

            return orig(koishi);
        }

        private int On_Gore_NewGore_IEntitySource_Vector2_Vector2_int_float(On_Gore.orig_NewGore_IEntitySource_Vector2_Vector2_int_float orig, IEntitySource source, Vector2 Position, Vector2 Velocity, int Type, float Scale)
        {
            int index = orig(source, Position, Velocity, Type, Scale);
            if (index < 600 && Main.LocalPlayer.EnableEnhance<RinSkull>())
            {
                NPC targetNpc = source switch
                {
                    EntitySource_Parent { Entity: NPC npc } when npc.life <= 0 => npc,
                    EntitySource_Death { Entity: NPC npc } => npc,
                    _ => null
                };

                if (targetNpc != null)
                {
                    if (ChildSafety.Disabled)
                        EnhanceSystem.GoreDamage[index] = targetNpc.damage / 4;
                    else
                        Projectile.NewProjectile(source, Position, Velocity, ProjectileID.SpiritHeal, 0, 0, Main.LocalPlayer.whoAmI, Main.LocalPlayer.whoAmI, 1);
                }
            }
            return index;
        }

        private void On_ShopHelper_ProcessMood(On_ShopHelper.orig_ProcessMood orig, ShopHelper self, Player player, NPC npc)
        {
            if (player.EnableEnhance<KokoroMask>())
            {
                bool a = Main.remixWorld;
                Main.remixWorld = false;
                orig(self, player, npc);
                Main.remixWorld = a;
            }
            else orig(self, player, npc);
        }

        private float On_ShopHelper_LimitAndRoundMultiplier(On_ShopHelper.orig_LimitAndRoundMultiplier orig, ShopHelper self, float priceAdjustment)
        {
            if (Main.LocalPlayer.EnableEnhance<KokoroMask>())
            {
                if (self._currentPriceAdjustment >= 1.5f)
                    ModContent.GetInstance<BurningWithRage>().Condition.Complete();

                return 0.75f;
            }

            return orig(self, priceAdjustment);
        }

        private void On_WorldGen_UpdateWorld_GrassGrowth(On_WorldGen.orig_UpdateWorld_GrassGrowth orig, int i, int j, int minI, int maxI, int minJ, int maxJ, bool underground)
        {
            orig(i, j, minI, maxI, minJ, maxJ, underground);

            if (!underground || !WorldGen.InWorld(i, j, 10) || Main.tile[i, j].type != TileID.JungleGrass)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.sleeping.isSleeping || !player.EnableEnhance<DoremyPillow>())
                    return;

                orig(i, j, minI, maxI, minJ, maxJ, underground);
            }
        }

        private void On_Main_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents)
        {
            orig(ref stopEvents);

            if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.downedBoss2)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.sleeping.isSleeping && player.EnableEnhance<DoremyPillow>() && Main.rand.NextBool(50))
                    WorldGen.spawnMeteor = true;
            }
        }

        private void On_BirthdayParty_NaturalAttempt(On_BirthdayParty.orig_NaturalAttempt orig)
        {
            orig();

            if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.AnyNPCs(208) || BirthdayParty.PartyDaysOnCooldown > 0)
                return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.sleeping.isSleeping && player.EnableEnhance<DoremyPillow>())
                    orig();
            }
        }

        private static void IL_Player_AdjTiles(ILContext il)
        {
            var c = new ILCursor(il);

            // 查找具体的模式：if (!Main.playerInventory) return;
            // 对应的 IL 可能是：
            // IL_XXXX: ldsfld     bool Terraria.Main::playerInventory
            // IL_XXXX: brtrue.s   IL_XXXX  // 如果为 true，跳转到 if 块之后
            // IL_XXXX: ret                  // return 语句

            // 查找 ldsfld + brtrue + ret 的模式
            if (!c.TryGotoNext(MoveType.Before,
                i => i.MatchLdsfld(typeof(Main), "playerInventory"),
                i => i.MatchBrtrue(out _),      // 匹配 brtrue 或 brtrue.s
                i => i.MatchRet()))             // 匹配 ret
            {
                // 尝试其他可能的模式
                c.Index = 0;
                if (!c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdsfld<Main>("playerInventory")))
                {
                    throw new Exception("无法定位到 if (!Main.playerInventory) return; 语句");
                }
            }

            // 现在 c 指向 ldsfld Main.playerInventory
            // 我们要在这个语句之前插入代码

            // 插入我们的自定义逻辑
            c.Emit(OpCodes.Ldarg_0);  // 加载 this (Player)
            c.EmitDelegate<Action<Player>>(player =>
            {
                if (player.EnableEnhance<NitoriCucumber>())
                {
                    EnhancePlayers mp = player.MP();

                    if (mp.adjTile is null || mp.adjOther is null)
                        return;

                    for (int i = 0; i < player.adjTile.Length; i++)
                    {
                        if (!player.adjTile[i])
                            continue;

                        mp.adjTile[i] = true;

                        if (i < TileID.Count)
                            mp.adjTileVanilla[i] = true;
                        else if (!mp.adjTileMod.Contains(TileLoader.GetTile(i).FullName))
                            mp.adjTileMod.Add(TileLoader.GetTile(i).FullName);
                    }

                    if (player.adjWater)
                        mp.adjOther[0] = true;

                    if (player.adjHoney)
                        mp.adjOther[1] = true;

                    if (player.adjLava)
                        mp.adjOther[2] = true;

                    if (player.adjShimmer)
                        mp.adjOther[3] = true;

                    if (player.alchemyTable)
                        mp.adjOther[4] = true;

                    // 成就的触发
                    int count = mp.adjTile.Concat(mp.adjOther).Where(a => a).Count();
                    var improveGame = ModContent.GetInstance<ImproveGame>();

                    if (improveGame.Condition.Value < count)
                        improveGame.Condition.Value = count;

                    if (improveGame.Condition.Value >= ImproveGame.Max)
                        improveGame.Condition.Complete();

                    player.adjWater = mp.adjOther[0];
                    player.adjHoney = mp.adjOther[1];
                    player.adjLava = mp.adjOther[2];
                    player.adjShimmer = mp.adjOther[3];
                    player.alchemyTable = mp.adjOther[4];
                    player.adjTile = (bool[])mp.adjTile.Clone();
                }
            });
        }

        private void On_WorldGen_ShakeTree(On_WorldGen.orig_ShakeTree orig, int i, int j)
        {
            if (!WorldEnableEnhance<SizuhaBrush>())
            {
                orig(i, j);
                return;
            }

            for (int l = 0; ;l++)
            {
                // 在orig前获取树是否被摇过，因为orig会修改WorldGen.treeShakeX,Y的值，标记为被摇过
                bool treeShaken = false;

                WorldGen.GetTreeBottom(i, j, out var x, out var y);
                for (int k = 0; k < WorldGen.numTreeShakes; k++)
                {
                    if (WorldGen.treeShakeX[k] == x && WorldGen.treeShakeY[k] == y)
                    {
                        treeShaken = true;
                        break;
                    }
                }

                bool a = Main.getGoodWorld;
                Main.getGoodWorld = false;

                orig(i, j);

                Main.getGoodWorld = a;

                if (WorldGen.numTreeShakes == WorldGen.maxTreeShakes || treeShaken || l >= 20)
                    return;
                else
                {
                    for (int k = 0; k < WorldGen.numTreeShakes; k++)
                    {
                        if (WorldGen.treeShakeX[k] == x && WorldGen.treeShakeY[k] == y)
                        {
                            if (k < WorldGen.numTreeShakes - 1)
                            {
                                Array.Copy(WorldGen.treeShakeX, k + 1, WorldGen.treeShakeX, k, WorldGen.numTreeShakes - k - 1);
                                Array.Copy(WorldGen.treeShakeY, k + 1, WorldGen.treeShakeY, k, WorldGen.numTreeShakes - k - 1);
                            }

                            WorldGen.treeShakeX[WorldGen.numTreeShakes - 1] = 0;
                            WorldGen.treeShakeY[WorldGen.numTreeShakes - 1] = 0;

                            WorldGen.numTreeShakes--;
                            break;
                        }
                    }
                }
            }
        }

        private int LuckUp(On_Main.orig_DamageVar_float_int_float orig, float dmg, int percent, float luck)
        {
            int damage = orig(dmg, percent, luck);

            if (luck > 1f && Main.LocalPlayer.EnableEnhance<TewiCarrot>() && Main.rand.NextFloat() < luck - 1)
            {
                float damage2 = dmg * (1f + Main.rand.Next(-percent, percent * 2 + 1) * 0.01f);
                if (damage2 > damage)
                    damage = (int)Math.Round(damage2);
            }

            if (luck < -1f && Main.LocalPlayer.EnableEnhance<TewiCarrot>() && Main.rand.NextFloat() < -1 - luck)
            {
                float damage2 = dmg * (1f + Main.rand.Next(-percent * 2, percent + 1) * 0.01f);
                if (damage2 < damage)
                    damage = (int)Math.Round(damage2);
            }

            if (luck > 0f && damage < dmg && Main.LocalPlayer.EnableEnhance<HinaDoll>())
                damage = (int)Math.Round(dmg * (1f + Main.rand.Next(percent + 1) * 0.01f));

            if (luck < 0f && damage > dmg && Main.LocalPlayer.EnableEnhance<HinaDoll>())
                damage = (int)Math.Round(dmg * (1f + Main.rand.Next(-percent, 1) * 0.01f));

            return damage;
        }

        private void LuckUp(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
        {
            float value = self.value;

            if (closestPlayer.luck > 1f && closestPlayer.EnableEnhance<TewiCarrot>())
                self.value *= 1 + MathHelper.Lerp(0, 0.05f + Main.rand.NextFloat(0.20f), closestPlayer.luck - 1);

            orig(self, closestPlayer);

            self.value = value;
        }

        private int LuckUp(On_Player.orig_RollLuck orig, Player self, int range)
        {
            if (self.luck > 1f && self.EnableEnhance<TewiCarrot>() && Main.rand.NextFloat() < self.luck - 1)
                return Main.rand.Next(Main.rand.Next(range / 4, range));

            return orig(self, range);
        }

        private int SuperCrit(On_NPC.orig_StrikeNPC_HitInfo_bool_bool orig, NPC self, NPC.HitInfo hit, bool fromNet, bool noPlayerInteraction)
        {
            if (self.TryGetGlobalNPC(out GEnhanceNPCs gnpc) && gnpc.SuperCrit)
            {
                if (self.type == NPCID.MossHornet)
                    ModContent.GetInstance<SniperDuel>().Condition.Complete();

                int index = NewText(new Rectangle((int)self.position.X, (int)self.position.Y, self.width, self.height), new Color(133, 0, 133), hit.Damage, true);

                if (index <= Main.combatText.Length - 1)
                {
                    var text = Main.combatText[index];
                    text.lifeTime *= 2;
                    text.rotation *= 2;
                    hit.HideCombatText = true;
                }
                gnpc.SuperCrit = false;
            }

            return orig(self, hit, fromNet, noPlayerInteraction);
        }

        private void On_NPC_getGoodAdjustments(On_NPC.orig_getGoodAdjustments orig, NPC self)
        {
            orig(self);

            if (!WorldEnableEnhance<SuikaGourd>())
                return;

            bool a = Main.tenthAnniversaryWorld;
            NPC npc = new();
            Main.getGoodWorld = false;
            Main.tenthAnniversaryWorld = false;
            npc.SetDefaults(self.type);
            self.scale = npc.scale;
            Main.getGoodWorld = true;
            Main.tenthAnniversaryWorld = a;
        }

        private void TeleportFromMap(Vector2 arg1, float arg2)
        {
            if (!Main.mouseRight || (Main.CurrentFrameFlags.AnyActiveBossNPC && !Config.Yukari) || Main.LocalPlayer.MP()?.YukariCD is null or > 0 || !Main.LocalPlayer.EnableEnhance<YukarisItem>())
                return;

            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;
            Vector2 target = ((Main.MouseScreen - screenSize / 2) / 16 * (16 / Main.mapFullscreenScale) + Main.mapFullscreenPos) * 16;

            if (WorldGen.InWorld((int)target.X / 16, (int)target.Y / 16) && Main.LocalPlayer.BuyItem(10000))
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    EnhancePlayers.YukariTp(Main.LocalPlayer, target);
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    EnhancePlayers.YukariTp(Main.LocalPlayer, target);

                    ModPacket packet = Mod.GetPacket();

                    packet.Write((byte)TouhouPetsEx.MessageType.Tp);
                    packet.Write((byte)Main.LocalPlayer.whoAmI);
                    packet.WriteVector2(target);
                    packet.Send(-1, Main.LocalPlayer.whoAmI);
                }
            }
        }

        private int ExCrit(On_NPC.HitModifiers.orig_GetDamage orig, ref NPC.HitModifiers self, float baseDamage, bool crit, bool damageVariation, float luck)
        {
            if (!crit && self.DamageType?.CountsAsClass(DamageClass.Summon) == true && Main.LocalPlayer.EnableEnhance<YukarisItem>() && Main.rand.NextBool(100))
            {
                self.SetCrit();
            }

            if (Main.LocalPlayer.EnableEnhance<TenshiKeyStone>())
            {
                self.ScalingArmorPenetration += baseDamage / 100f;

                if (Config.Tenshi)
                {
                    float defense = self.Defense.ApplyTo(0);
                    float effectiveScaling = Math.Max(self.ScalingArmorPenetration.Value, 0);
                    float totalPenetration = effectiveScaling * defense + self.ArmorPenetration.Value;

                    if (effectiveScaling > 1)
                        totalPenetration = effectiveScaling * self.ArmorPenetration.Value;

                    baseDamage += Math.Max(totalPenetration - defense, 0);
                }
            }
            return orig(ref self, baseDamage, crit, damageVariation, luck);
        }
        private int On_Player_GetPickaxeDamage(On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            int damage = orig(self, x, y, pickPower, hitBufferIndex, tileTarget);

            if (damage > 0 && self.EnableEnhance<FlandrePudding>())
            {
                damage *= 495;
            }

            return damage;
        }

        private float On_Player_VanillaBaseDefenseEffectiveness(On_Player.orig_VanillaBaseDefenseEffectiveness orig)
        {
            float effectiveness = orig();
            EnhancePlayers mp = Main.LocalPlayer.MP();

            if (mp == null)
                return effectiveness;

            if (mp.FragrantAromaFillsTheAir == true)
                effectiveness += 0.25f;

            if (Main.LocalPlayer.EnableEnhance<MomoyoPickaxe>())
                effectiveness += Math.Clamp(mp.ExtraAddition[11], 0, 514) / 5140f;

            return effectiveness;
        }

        private ref float On_Player_GetArmorPenetration(On_Player.orig_GetArmorPenetration orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].armorPen;

            return ref orig(self, damageClass);
        }

        private ref StatModifier On_Player_GetKnockback(On_Player.orig_GetKnockback orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].knockback;

            return ref orig(self, damageClass);
        }

        private ref float On_Player_GetAttackSpeed(On_Player.orig_GetAttackSpeed orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].attackSpeed;

            return ref orig(self, damageClass);
        }

        private ref float On_Player_GetCritChance(On_Player.orig_GetCritChance orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].critChance;

            return ref orig(self, damageClass);
        }

        private ref StatModifier On_Player_GetDamage(On_Player.orig_GetDamage orig, Player self, DamageClass damageClass)
        {
            if (Config.Junko && self.EnableEnhance<JunkoMooncake>())
                return ref self.damageData[DamageClass.Generic.Type].damage;

            return ref orig(self, damageClass);
        }
    }
}

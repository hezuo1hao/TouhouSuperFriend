using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
using TouhouPetsEx.Achievements;

namespace TouhouPetsEx.Projectiles;

public class PlayerHead : ModProjectile
{
    // QOT伟大，无需多言
    private class PlayerControlLock : ModPlayer
    {
        public override void UpdateEquips()
        {
            // 原版的 isOperatingAnotherEntity 在 UpdateEquips 执行前赋值
            // 所以我们在这写我们的代码
            Player.isOperatingAnotherEntity |=
                Player.ownedProjectileCounts[ModContent.ProjectileType<PlayerHead>()] > 0;
        }
    }

    public override string Texture => "TouhouPetsEx/Projectiles/DaiyouseiBoom";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 960;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.JimsDrone);
        Projectile.aiStyle = -1;
        DrawOffsetX = -10;
        DrawOriginOffsetY = -6;
    }
    public int homing = 0;
    int time = 0;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(homing);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        homing = reader.ReadInt32();
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        bool shouldBeKilled = player.dead;
        float dir = Projectile.Distance(player.Center);

        if (Projectile.owner == Main.myPlayer)
        {
            // 高处爆炸
            if (Projectile.position.Y - Projectile.height <= 16 * Main.offScreenRange / 2)
            {
                Projectile.Kill();
                return;
            }

            if (player.HeldItem.type != ModContent.ItemType<SekibankiBow>())
                homing++;

            if (dir < 6 && time > 60)
                shouldBeKilled = true;

            bool gravityFlipped = player.gravDir == -1f;
            var direction = Vector2.Zero;

            if (PlayerInput.UsingGamepad)
            {
                direction = PlayerInput.GamepadThumbstickLeft;
            }
            else
            {
                Player.DirectionalInputSyncCache localInputCache = player.LocalInputCache;
                direction.X -= (localInputCache.controlLeft ^ gravityFlipped).ToInt();
                direction.X += (localInputCache.controlRight ^ gravityFlipped).ToInt();
                direction.Y -= localInputCache.controlUp.ToInt();
                direction.Y += localInputCache.controlDown.ToInt();
                direction = direction.SafeNormalize(Vector2.Zero);
            }

            if (homing > 0)
            {
                Projectile.tileCollide = false;
                direction = Vector2.Normalize(player.Center - Projectile.Center) * (dir / 300f + homing / 240f);
                homing++;
            }

            if (new Vector2(Projectile.ai[0], Projectile.ai[1]) != direction)
            {
                Projectile.ai[0] = direction.X;
                Projectile.ai[1] = direction.Y;
                Projectile.netUpdate = true;
            }

            Main.DroneCameraTracker.Track(Projectile);
        }

        if (shouldBeKilled)
        {
            Projectile.Kill();
            return;
        }

        var speedDirection = new Vector2(Projectile.ai[0], Projectile.ai[1]);
        const float speed = 0.12f;
        bool onLand = Projectile.velocity.Y == 0f;
        Projectile.velocity += speedDirection * speed;

        // float gravity = 0.05f; // 重力系数
        // Projectile.velocity.Y += gravity;

        if (Projectile.velocity.Length() > 20f)
            Projectile.velocity *= 20f / Projectile.velocity.Length();

        Projectile.velocity *= 0.98f;

        UpdateSound(onLand, speedDirection);

        Lighting.AddLight(Projectile.Center, Color.White.ToVector3());
        Projectile.timeLeft = 2;

        Projectile.rotation = Projectile.velocity.X * 0.1f;

        if (Main.netMode is NetmodeID.Server && Main.player.IndexInRange(Projectile.owner) && player.active)
            RemoteClient.CheckSection(Projectile.owner, Projectile.position);

        if (Main.LocalPlayer == player && ((player.Center.X < 6400 && Projectile.Center.X > Main.maxTilesX * 16 - 6400) || (Projectile.Center.X < 6400 && player.Center.X > Main.maxTilesX * 16 - 6400)))
            ModContent.GetInstance<Teleportation>().Condition.Complete();

        time++;
    }

    public override bool OnTileCollide(Vector2 lastVelocity)
    {
        if (Projectile.velocity.X != lastVelocity.X && Math.Abs(lastVelocity.X) > 1f)
            Projectile.velocity.X = -lastVelocity.X * 0.3f;

        if (Projectile.velocity.Y != lastVelocity.Y && Math.Abs(lastVelocity.Y) > 1f)
            Projectile.velocity.Y = -lastVelocity.Y * 0.3f;

        return false;
    }

    public override bool? CanCutTiles() => false;

    /// <summary>
    /// 声音控制，从原版抄的
    /// </summary>
    private void UpdateSound(bool onLand, Vector2 speedDirection)
    {
        Projectile.localAI[0] += 1f;

        float pitchFactor = 0; // 声音的声调程度
        if (!onLand)
            pitchFactor = 50;

        if (speedDirection.Length() > 0.5f)
            pitchFactor = MathHelper.Lerp(0f, 100f, speedDirection.Length());

        Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], pitchFactor, 0.2f);
        float num3 = Utils.Remap(Projectile.localAI[0], 0f, 5f, 0f, 1f) *
                     Utils.Remap(Projectile.localAI[0], 5f, 15f, 1f, 0f);
        float volume = Utils.Clamp(MathHelper.Max(Utils.Remap(Projectile.localAI[1], 0f, 100f, 0f, 25f), num3 * 12f),
            0f, 100f) * 2f;
        SoundEngine.TryGetActiveSound(SlotId.FromFloat(Projectile.localAI[2]), out ActiveSound activeSound);
        if (activeSound == null && volume != 0f)
        {
            Projectile.localAI[2] = SoundEngine.PlayTrackedLoopedSound(SoundID.JimsDrone, Projectile.Center,
                new ProjectileAudioTracker(Projectile).IsActiveAndInGame).ToFloat();
            SoundEngine.TryGetActiveSound(SlotId.FromFloat(Projectile.localAI[2]), out activeSound);
        }

        if (activeSound != null)
        {
            activeSound.Volume = volume;
            activeSound.Position = Projectile.Center;
            activeSound.Pitch = Utils.Clamp(Utils.Remap(Projectile.localAI[1], 0f, 100f, -1f, 1f) + num3, -1f, 1f);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Main.player[Projectile.owner];
        if (Projectile.ai[2] == -2)
            Projectile.ai[2] = player.head;
        int dir = player.direction;
        player.head = (int)Projectile.ai[2];
        player.headRotation = Projectile.rotation;
        player.direction = Projectile.direction;
        Main.PlayerRenderer.DrawPlayerHead(Main.Camera, Main.player[Projectile.owner], Projectile.position - Main.screenPosition);

        if (player.direction == -1)
            Main.DrawTrail(Projectile, new Vector2(10f, 2f), new Color(59, 217, 180, 100));
        else
            Main.DrawTrail(Projectile, new Vector2(-10f, 2f), new Color(59, 217, 180, 100));

        player.direction = dir;
        player.head = TouhouPetsEx.TransparentHead;
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        if (SoundEngine.TryGetActiveSound(SlotId.FromFloat(Projectile.localAI[2]), out var sound))
            sound.Stop();
    }
}
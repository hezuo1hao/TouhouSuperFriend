using Microsoft.Xna.Framework;
using Terraria;

namespace TouhouPetsEx.Enhance.Core;
public struct NewEntityShadowInfo
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Origin;
    public int Direction;
    public int GravityDirection;
    public int BodyFrameIndex;
    public int CarpetFrame;
    public int LegFrameY;
    public int HeadFrameY;
    public int WingFrame;

    public Vector2 HeadgearOffset => Main.OffsetsPlayerHeadgear[BodyFrameIndex];

    public void CopyPlayer(Player player)
    {
        Position = player.position;
        Rotation = player.fullRotation;
        Origin = player.fullRotationOrigin;
        Direction = player.direction;
        GravityDirection = (int)player.gravDir;
        BodyFrameIndex = player.bodyFrame.Y / player.bodyFrame.Height;
        CarpetFrame = player.carpetFrame;
        LegFrameY = player.legFrame.Y;
        HeadFrameY = player.headFrame.Y;
        WingFrame = player.wingFrame;
    }
}

using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace TouhouPetsEx
{
    public class TouhouPetsExModSystem : ModSystem
    {
        public static int SynchronousTime;
        public static ModKeybind ReisenKeyBind { get; private set; }

        public override void Load()
        {
            ReisenKeyBind = KeybindLoader.RegisterKeybind(Mod, "ReisenKeyBind", "V");
        }

        public override void Unload()
        {
            ReisenKeyBind = null;
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
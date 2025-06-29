using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPetsEx
{
    public class TouhouPetsExConfigs : ModConfig
    {
        public override void OnLoaded() => Config = this;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool Lily;

        [DefaultValue(false)]
        public bool Hecatia;

        [DefaultValue(false)]
        public bool Cirno;
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPetsEx
{
    public class TouhouPetsExConfigs : ModConfig
    {
        public override void OnLoaded() => Config = this;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool AliceOld;

        [DefaultValue(false)]
        public bool Rumia;

        [DefaultValue(false)]
        public bool Lily;

        [DefaultValue(false)]
        public bool Cirno;

        [DefaultValue(false)]
        public bool Letty;

        [DefaultValue(false)]
        public bool Letty_2;

        public List<string> Letty_2_1;

        [DefaultValue(false)]
        public bool Yukari;

        [DefaultValue(false)]
        public bool Marisa;

        [DefaultValue(false)]
        public bool Reisen;

        public List<string> Reisen_1;

        [DefaultValue(false)]
        public bool Eirin;

        [DefaultValue(false)]
        public bool Sanae;

        [DefaultValue(false)]
        public bool Tenshi;

        [DefaultValue(false)]
        public bool Junko;

        [DefaultValue(false)]
        public bool Hecatia;
    }
    public class TouhouPetsExLocalConfigs : ModConfig
    {
        public override void OnLoaded() => LocalConfig = this;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool Rumia;

        [DefaultValue(YuyukoEffectType.All)]
        [DrawTicks]
        public YuyukoEffectType Yuyuko;
    }
    public enum YuyukoEffectType
    {
        Disabled,
        Butterfly,
        Die,
        All
    }
}
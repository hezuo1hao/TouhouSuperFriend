using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TouhouPetsEx
{
    public class TouhouPetsExConfigs : ModConfig
    {
        public override void OnLoaded() => Config = this;
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Vanilla")]
        [DefaultValue(true)]
        public bool PetInv;

        [DefaultValue(true)]
        public bool Piece_V;

        [Header("Experimental")]
        [DefaultValue(false)]
        public bool AliceOld;

        [DefaultValue(false)]
        public bool Rumia;

        [DefaultValue(false)]
        public bool Cirno;

        [DefaultValue(false)]
        public bool Meirin;

        [DefaultValue(false)]
        public bool Sakuya;

        [DefaultValue(false)]
        public bool Lily;

        [DefaultValue(false)]
        public bool Letty;

        [DefaultValue(false)]
        public bool Letty_2;

        public List<string> Letty_2_1;

        [DefaultValue(false)]
        public bool Yukari;

        [DefaultValue(false)]
        public bool Reimu;

        [DefaultValue(false)]
        public bool Marisa;

        [DefaultValue(false)]
        public bool Reisen;

        public List<string> Reisen_1;

        [DefaultValue(false)]
        public bool Eirin;

        [DefaultValue(false)]
        public bool Aya;

        [DefaultValue(false)]
        public bool Yuka;

        [DefaultValue(false)]
        public bool Yuka_2;

        [DefaultValue(false)]
        public bool Sanae;

        [DefaultValue(false)]
        public bool Tenshi;

        [DefaultValue(false)]
        public bool Satori;

        [DefaultValue(false)]
        public bool Satori_2;

        [DefaultValue(false)]
        public bool Rin;

        [DefaultValue(false)]
        public bool Rin_2;

        [DefaultValue(false)]
        public bool Utsuho;

        [DefaultValue(false)]
        public bool Utsuho_2;

        [DefaultValue(false)]
        public bool Koishi;

        [DefaultValue(false)]
        public bool Koishi_2;

        [DefaultValue(false)]
        public bool Wakasagihime;

        [DefaultValue(false)]
        public bool Wakasagihime_2;

        [DefaultValue(false)]
        public bool Junko;

        [DefaultValue(false)]
        public bool Hecatia;

        [DefaultValue(false)]
        public bool Piece;
    }
    public class TouhouPetsExLocalConfigs : ModConfig
    {
        public override void OnLoaded() => LocalConfig = this;
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool MarisaKoishi;

        [DefaultValue(true)]
        public bool Box5;

        [DefaultValue(true)]
        public bool Tooltip_1;

        [DefaultValue(true)]
        public bool Tooltip_2;

        [DefaultValue(true)]
        public bool Tooltip_3;

        [DefaultValue(true)]
        public bool Rumia;

        [DefaultValue(true)]
        public bool Sakuya;

        [DefaultValue(YuyukoEffect.All)]
        [DrawTicks]
        public YuyukoEffect Yuyuko;

        [DefaultValue(YukaEffect.All)]
        [DrawTicks]
        public YukaEffect Yuka;

        [DefaultValue(true)]
        public bool Raiko;
    }
    public enum YuyukoEffect
    {
        Disabled,
        Butterfly,
        Die,
        All
    }
    public enum YukaEffect
    {
        Disabled,
        NoSound,
        NoVisual,
        All
    }
}
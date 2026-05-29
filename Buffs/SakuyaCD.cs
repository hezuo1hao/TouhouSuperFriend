using Terraria.ModLoader;
namespace TouhouPetsEx.Buffs
{
    public class SakuyaCD : CDBuff
    {
        public override string BuffName => "SakuyaBuff";
        public override bool IsLoadingEnabled(Mod mod) => true;
    }
}
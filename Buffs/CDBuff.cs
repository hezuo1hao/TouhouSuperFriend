using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TouhouPets.Content.Items.PetItems;
namespace TouhouPetsEx.Buffs
{
    // TODO: 四种cd改为：cd最低为原先的十分之一，根据伤害与血上限50%之比线性提升至cd的100%
    public abstract class CDBuff : ModBuff
    {
        public abstract string BuffName
        {
            get;
        }
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override string Texture => $"TouhouPets/Content/Buffs/PetBuffs/{BuffName}";
        public override LocalizedText DisplayName => Language.GetText("Mods.TouhouPetsEx.Buffs.CD.DisplayName");
        public override LocalizedText Description => Language.GetText("Mods.TouhouPetsEx.Buffs.CD.Description");
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] = time;
            return false;
        }
    }
}
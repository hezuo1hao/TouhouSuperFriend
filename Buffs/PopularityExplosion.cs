using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace TouhouPetsEx.Buffs
{
    public class PopularityExplosion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
            Main.persistentBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            float multiplier = 1;

            if (Config.Koishi)
                multiplier += MathHelper.Lerp(0, 1, player.MP().Popularity / 100f);

            player.aggro += 10000;
            player.GetArmorPenetration(DamageClass.Generic) += 10 * multiplier;
            player.GetDamage(DamageClass.Generic) += 0.05f * multiplier;
            player.GetCritChance(DamageClass.Generic) += 4f * multiplier;
            player.statDefense += (int)Math.Ceiling(8 * multiplier);
            player.endurance += 0.06f * multiplier;

            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = Config.Koishi;
            Main.buffNoTimeDisplay[Type] = Config.Koishi;
        }
    }
}
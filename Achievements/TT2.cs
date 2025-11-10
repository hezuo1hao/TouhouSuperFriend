using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class TT2 : ModAchievement
{
	public const int Max = 216000;

    public CustomIntCondition Condition { get; private set; }

	public override bool Hidden => false;

	public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Challenger);

		Condition = AddIntCondition(Max);
    }
}

using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class WitchTrial : ModAchievement
{
	public const int Max = 13;

    public CustomIntCondition Condition { get; private set; }

	public override bool Hidden => Condition.Value < Max;

	public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Challenger);

		Condition = AddIntCondition(Max);
    }
}

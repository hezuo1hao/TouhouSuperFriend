using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class AutumnGodRanch : ModAchievement
{
	public const int Max = 32;

    public CustomIntCondition Condition { get; private set; }

	public override bool Hidden => false;

	public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Challenger);

		Condition = AddIntCondition(Max);
    }
}

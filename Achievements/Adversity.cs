using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class Adversity : ModAchievement
{
	public const float Max = 100f;

    public CustomFloatCondition Condition { get; private set; }

	public override bool Hidden => false;

	public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Challenger);

		Condition = AddFloatCondition(Max);
    }
}

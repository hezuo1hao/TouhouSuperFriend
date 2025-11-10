using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class LikesToRead : ModAchievement
{
	public const int Max = 100;

    public CustomIntCondition Condition { get; private set; }

	public override bool Hidden => false;

	public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Collector);

		Condition = AddIntCondition(Max);
    }
}

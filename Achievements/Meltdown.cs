using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class Meltdown : ModAchievement
{
	public const int Max = 1000;

    public CustomIntCondition Condition { get; private set; }

	public override bool Hidden => false;

	public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Slayer);

		Condition = AddIntCondition(Max);
    }
}

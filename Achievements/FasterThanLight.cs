using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class FasterThanLight : ModAchievement
{
	public const float LightSpeed = 310.391419704094488f;

    public CustomFloatCondition Condition { get; private set; }

	public override bool Hidden => false;

	public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Challenger);

		// TODO: 这个key没搞明白是啥，注意一下
		Condition = AddFloatCondition(LightSpeed);
    }
}

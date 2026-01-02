using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class FlyMyWings : ModAchievement
{
    public CustomFlagCondition Condition { get; private set; }

	public override bool Hidden => false;
    public override Position GetDefaultPosition()
    {
        return new After("DEFEAT_EMPRESS_OF_LIGHT");
    }
    public override void SetStaticDefaults() {
		Achievement.SetCategory(AchievementCategory.Explorer);

        Condition = AddCondition();
    }
}

using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace TouhouPetsEx.Achievements;
public class IntMaxValue : ModAchievement
{

    public CustomFlagCondition Condition { get; private set; }

    public override bool Hidden => !Condition.IsCompleted;

    public override void SetStaticDefaults()
    {
        Achievement.SetCategory(AchievementCategory.Challenger);

        Condition = AddCondition();
    }
}

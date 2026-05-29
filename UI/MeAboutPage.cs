using ImproveGame.UI.ModernConfig;
using ImproveGame.UI.ModernConfig.FakeCategories;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TouhouPetsEx.UI;

[JITWhenModsEnabled(nameof(ImproveGame))]
[ExtendsFromMod(nameof(ImproveGame))]
public sealed class MeAboutPage : Category
{
    public override int ItemIconId => ItemID.GPS;
    public override string Label => GetText($"ModernConfig.{LocalizationKey}.Label");
    public override string Tooltip => GetText($"ModernConfig.{LocalizationKey}.Tooltip");
    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.ShouldHideSearchBar = true;

        var text = new SUIText
        {
            TextOrKey = "Mods.TouhouPetsEx.ModernConfig.MeAboutPage.About",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = { Precent = 1f },
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(text);
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));

        AddLinksToPanel(panel);

        panel.Recalculate();
    }

    public static void AddLinksToPanel(ConfigOptionsPanel panel)
    {
        var gapProvider = new View
        {
            Width = StyleDimension.Fill,
            Height = new(30, 0f),
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(gapProvider);

        AboutPage.GenerateLinkElement(panel, "Mods.TouhouPetsEx.ModernConfig.MeAboutPage.LinkGitHub", "https://github.com/hezuo1hao/TouhouSuperFriend");

        if (Language.ActiveCulture.Name is not "zh-Hans")
            return;

        AboutPage.GenerateLinkElement(panel, "Mods.TouhouPetsEx.ModernConfig.MeAboutPage.LinkQQ", "https://qm.qq.com/q/DfMN49cJHO");
    }
}
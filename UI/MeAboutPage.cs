using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TouhouPetsEx.UI;

[JITWhenModsEnabled("ImproveGame")]
public sealed class MeAboutPage : ImproveGame.UI.ModernConfig.Category
{
    [JITWhenModsEnabled("ImproveGame")]
    public override int ItemIconId => ItemID.GPS;
    [JITWhenModsEnabled("ImproveGame")]
    public override string Label => GetText($"ModernConfig.{LocalizationKey}.Label");
    [JITWhenModsEnabled("ImproveGame")]
    public override string Tooltip => GetText($"ModernConfig.{LocalizationKey}.Tooltip");
    [JITWhenModsEnabled("ImproveGame")]
    public override void AddOptions(ImproveGame.UI.ModernConfig.ConfigOptionsPanel panel)
    {
        panel.ShouldHideSearchBar = true;

        var text = new ImproveGame.UIFramework.SUIElements.SUIText
        {
            TextOrKey = "Mods.TouhouPetsEx.ModernConfig.MeAboutPage.About",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = { Precent = 1f },
            TextScale = 1.1f,
            RelativeMode = ImproveGame.UIFramework.BaseViews.RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(text);
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));

        AddLinksToPanel(panel);

        panel.Recalculate();
    }
    [JITWhenModsEnabled("ImproveGame")]
    public static void AddLinksToPanel(ImproveGame.UI.ModernConfig.ConfigOptionsPanel panel)
    {
        var gapProvider = new ImproveGame.UIFramework.BaseViews.View
        {
            Width = StyleDimension.Fill,
            Height = new(30, 0f),
            RelativeMode = ImproveGame.UIFramework.BaseViews.RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(gapProvider);

        ImproveGame.UI.ModernConfig.FakeCategories.AboutPage.GenerateLinkElement(panel, "Mods.TouhouPetsEx.ModernConfig.MeAboutPage.LinkGitHub", "https://github.com/hezuo1hao/TouhouSuperFriend");

        if (Language.ActiveCulture.Name is not "zh-Hans")
            return;

        ImproveGame.UI.ModernConfig.FakeCategories.AboutPage.GenerateLinkElement(panel, "Mods.TouhouPetsEx.ModernConfig.MeAboutPage.LinkQQ", "https://qm.qq.com/q/DfMN49cJHO");
    }
}
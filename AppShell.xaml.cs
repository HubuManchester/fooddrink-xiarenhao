using BiteRecord.Views;

namespace BiteRecord;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // 注册路由
        Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(HardwarePage), typeof(HardwarePage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }
}
using BiteRecord.Services;
using BiteRecord.Views;

namespace BiteRecord;

public partial class App : Application
{
    public static DatabaseService Database { get; private set; }

    public App()
    {
        InitializeComponent();

        // Initialize database
        Database = new DatabaseService();
        Database.InitializeDatabase();

        MainPage = new AppShell();
    }
}
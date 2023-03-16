using REMod.Views.Pages;
using REModman.Configuration.Enums;
using REModman.Logger;
using Wpf.Ui.Appearance;

namespace REMod
{
    public partial class MainWindow
    {
        public GameType SelectedGameType = GameType.None;

        public MainWindow()
        {
            DataContext = this;

            Watcher.Watch(this);

            InitializeComponent();

            Loaded += (_, _) => RootNavigation.Navigate(typeof(CollectionPage));

            ILogger logger = new NativeLogger();
            LogBase.Add(logger);
            LogBase.Info("REMod stdout has been initialized.");
            AppVersion_TextBlock.Text = GetAssemblyVersion();
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
        }
    }
}
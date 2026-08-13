using System.Windows;
using System.Windows.Controls;
using FIRSTMOD.Views;

namespace FIRSTMOD
{
    public partial class MainWindow : Window
    {
        private readonly HomeView _homeView = new();
        private readonly CefOperationsView _cefView = new();
        private readonly GtaMtaView _gtaMtaView = new();
        private readonly SystemView _systemView = new();
        private readonly SettingsView _settingsView = new();

        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = _homeView;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var btn in new[] { BtnHome, BtnCef, BtnGtaMta, BtnSystem, BtnSettings })
                btn.Tag = null;

            var clicked = (Button)sender;
            clicked.Tag = "Active";

            MainContent.Content = clicked.Name switch
            {
                nameof(BtnHome) => _homeView,
                nameof(BtnCef) => _cefView,
                nameof(BtnGtaMta) => _gtaMtaView,
                nameof(BtnSystem) => _systemView,
                nameof(BtnSettings) => _settingsView,
                _ => _homeView,
            };
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _homeView.StopTimers();
            _cefView.StopTimers();
            _systemView.StopTimers();
            base.OnClosed(e);
        }
    }
}

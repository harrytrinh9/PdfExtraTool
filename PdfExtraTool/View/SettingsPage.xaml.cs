using ModernWpf;
using PdfExtraTool.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PdfExtraTool.View
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.SettingsViewModel();
            CbChangeTheme.Text = "Sáng";
        }

        public bool IsLightTheme { get; private set; }
        public bool IsDarkTheme { get; private set; }
        public bool IsDefaultTheme { get; private set; }

        private void SetTheme(string theme)
        {
            switch (theme)
            {
                case "Sáng":
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    IsLightTheme = true;
                    IsDarkTheme = false;
                    IsDefaultTheme = false;
                    break;
                case "Tối":
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    IsLightTheme = false;
                    IsDarkTheme = true;
                    IsDefaultTheme = false;
                    break;
                case "Hệ thống":
                    IsLightTheme = false;
                    IsDarkTheme = false;
                    IsDefaultTheme = true;
                    break;
                default:
                    IsLightTheme = false;
                    IsDarkTheme = false;
                    IsDefaultTheme = true;
                    break;
            }
            //Settings.Default.Theme = theme;
            //Settings.Default.Save();
        }

        private async void CbChangeTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(100).ConfigureAwait(true);
            SetTheme(CbChangeTheme.Text);
        }
    }
}

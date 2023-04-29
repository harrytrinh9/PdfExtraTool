using ModernWpf;
using MVVMHelper;
using PdfExtraTool.Common;
using PdfExtraTool.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PdfExtraTool.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            //var currentLanguage = CultureInfo.CurrentUICulture.Name;
            var currentLanguage = PdfExtraTool.Properties.Resources.Culture.Name;

            _supportedLanguage = new List<Language>
            {
                new Language
                {
                    Code = "en-US",
                    Display = "English"
                },
                new Language
                {
                    Code="vi-VN",
                    Display = "Tiếng Việt"
                }
            };
            _themes = new List<AppTheme>
            {
                new AppTheme
                {
                    DisplayText = Resources.Default,
                    Theme = ThemeManager.Current.ActualApplicationTheme
                },
                new AppTheme
                {
                    DisplayText = Resources.Light,
                    Theme = ApplicationTheme.Light
                },
                new AppTheme
                {
                    DisplayText = Resources.Dark,
                    Theme = ApplicationTheme.Dark
                }
            };
            _selectedLanguage = SupportedLanguage.FirstOrDefault(x => x.Code == currentLanguage);

            var useDefaultTheme = Settings.Default.UseDefaultTheme;
            if (useDefaultTheme)
            {
                SelectedTheme = Themes[0];
            }
            else
            {
                var settingTheme = Settings.Default.Theme;
                if (settingTheme != null)
                {
                    if(settingTheme == "Light")
                    {
                        SelectedTheme = Themes[1];
                    }
                    else if (settingTheme == "Dark")
                    {
                        SelectedTheme = Themes[2];
                    }
                }
            }
        }

        private List<Language> _supportedLanguage;
        private Language _selectedLanguage;
        private List<AppTheme> _themes;
        private AppTheme _selectedTheme;
        private ICommand _restartAppCommand;

        public List<Language> SupportedLanguage { get => _supportedLanguage; set => SetProperty(ref _supportedLanguage, value); }
        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                SetProperty(ref _selectedLanguage, value);
                Settings.Default.Language = SelectedLanguage.Code;
                Settings.Default.Save();
            }
        }

        public List<AppTheme> Themes { get => _themes; set => Set(ref _themes, value); }
        public AppTheme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                Set(ref _selectedTheme, value);
                ThemeManager.Current.ApplicationTheme = SelectedTheme.Theme;
                if (SelectedTheme.DisplayText == Resources.Default)
                {
                    Settings.Default.UseDefaultTheme = true;
                }
                else
                {
                    Settings.Default.UseDefaultTheme = false;
                    Settings.Default.Theme = SelectedTheme.Theme.ToString();
                }

                Settings.Default.Save();
            }
        }

        public ICommand RestartAppCommand
        {
            get 
            {
                if (_restartAppCommand == null)
                {
                    _restartAppCommand = new RelayCommand(_ => RestartApp());
                }
                return _restartAppCommand; 
            }
            set => _restartAppCommand = value;
        }

        private void RestartApp()
        {
            throw new NotImplementedException();
        }

        public class Language
        {
            public string Display { get; set; }
            public string Code { get; set; }
        }

        public class AppTheme
        {
            public ApplicationTheme Theme { get; set; }
            public string DisplayText { get; set; }
        }
    }
}

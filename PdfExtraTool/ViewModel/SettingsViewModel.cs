using MVVMHelper;
using PdfExtraTool.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PdfExtraTool.ViewModel
{
    public class SettingsViewModel: ViewModelBase
    {
        public SettingsViewModel()
        {
            var currentLanguage = CultureInfo.CurrentUICulture.Name;
            
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

            SelectedLanguage = SupportedLanguage.FirstOrDefault(x => x.Code == currentLanguage);
        }

        private List<Language> _supportedLanguage;
        private Language _selectedLanguage;


        public List<Language> SupportedLanguage { get => _supportedLanguage; set => SetProperty(ref _supportedLanguage, value); }
        public Language SelectedLanguage 
        { 
            get => _selectedLanguage;
            set 
            {
                SetProperty(ref _selectedLanguage, value);
                //PdfExtraTool.Properties.Resources.Culture = new CultureInfo(SelectedLanguage.Code);

                //Thread.CurrentThread.CurrentUICulture = new CultureInfo(SelectedLanguage.Code);
                //Resources.Culture = Thread.CurrentThread.CurrentUICulture;
            }
        }
    }


    public class Language
    {
        public string Display { get; set; }
        public string Code { get; set; }
    }
}

using ModernWpf;
using PdfExtraTool.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfExtraTool.Common
{
    public static class ThemeHelper
    {
        public static void SetTheme()
        {
            var useDefaultTheme = Settings.Default.UseDefaultTheme;
            if (!useDefaultTheme)
            {
                var settingTheme = Settings.Default.Theme;
                if (settingTheme != null)
                {
                    
                    var theme = settingTheme == "Light" ? ApplicationTheme.Light: ApplicationTheme.Dark;
                    ThemeManager.Current.ApplicationTheme = theme;
                }
            }
        }
    }
}

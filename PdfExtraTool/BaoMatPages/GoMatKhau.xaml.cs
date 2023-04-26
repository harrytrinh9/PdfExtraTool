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

namespace PdfExtraTool.BaoMatPages
{
    /// <summary>
    /// Interaction logic for GoMatKhau.xaml
    /// </summary>
    public partial class GoMatKhau
    {
        public GoMatKhau()
        {
            InitializeComponent();
            DataContext = new ViewModel.GoMatKhauViewModel();
        }
    }
}

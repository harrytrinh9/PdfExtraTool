using MVVMHelper;
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
using PdfRenderByHarryTrinhWpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Collections.ObjectModel;

namespace PdfExtraTool.CustomControls
{
    /// <summary>
    /// Interaction logic for PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : UserControl
    {
        public PdfViewer()
        {
            InitializeComponent();
            ListViewPages.ItemsSource = ViewSource;
        }

        public static readonly DependencyProperty ViewSourceProperty =
        DependencyProperty.Register("ViewSource", typeof(ObservableCollection<PdfPageView>), typeof(PdfViewer));

        public ObservableCollection<PdfPageView> ViewSource
        {
            get { return (ObservableCollection<PdfPageView>)GetValue(ViewSourceProperty); }
            set { SetValue(ViewSourceProperty, value); }
        }

        //public BindablePasswordBox()
        //{
        //    InitializeComponent();
        //    txtPassword.PasswordChanged += OnPasswordChanged;

        //}

        //private void OnPasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    Password = txtPassword.SecurePassword;
        //}

        //private List<PdfPageView> _viewSource;
        //private PdfPageView _selectedPage;
        //private bool _isLoading;

        //public List<PdfPageView> ViewSource { get => _viewSource; set => SetProperty(ref _viewSource, value); }
        //public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        //public PdfPageView SelectedPage { get => _selectedPage; set => SetProperty(ref _selectedPage, value); }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(storage, value))
        //        return false;
        //    storage = value;
        //    this.OnPropertyChanged(propertyName);
        //    return true;
        //}
    }
}

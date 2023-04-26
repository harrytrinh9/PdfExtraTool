using iText.Kernel.Pdf;
using MVVMHelper;
using PdfExtraTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PdfExtraTool.ViewModel
{
    public class LatTrangViewModel: ViewModelBase
    {
        private string _selectedFile;
        private ICommand _selectFileCommand;
        private bool _isLoading;
        private string _openPassword;
        private string _protectPassword;
        private int _totalPage;
        private ICommand _saveFileCommand;

        public string SelectedFile { get => _selectedFile; set => SetProperty(ref _selectedFile, value); }
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        public ICommand SelectFileCommand
        {
            get
            {
                if (_selectFileCommand == null)
                {
                    _selectFileCommand = new RelayCommand(_ => SelectFile(), _ => !IsLoading);
                }
                return _selectFileCommand;
            }
            set => _selectFileCommand = value;
        }
        public string ProtectPassword { get => _protectPassword; set => SetProperty(ref _protectPassword, value); }
        public int TotalPage { get => _totalPage; set => SetProperty(ref _totalPage, value); }
        public ICommand SaveFileCommand { get => _saveFileCommand; set => _saveFileCommand = value; }

        private async void SelectFile()
        {
            var openPdf = new OpenPdf();
            IsLoading = openPdf.IsLoading;
            await openPdf.Open().ConfigureAwait(true);
            IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.SelectedFile;
            _openPassword = openPdf.OpenPdfPassword;
            TotalPage = openPdf.TotalPage;
            GetPage();
        }

        private void GetPage()
        {
            var reader = new PdfReader(SelectedFile);
            var doc = new PdfDocument(reader);
            var pages = new List<PdfPageProps>();
            for (int i = 1; i <= TotalPage; i++)
            {
                pages.Add(new PdfPageProps
                {
                    PageNo = i,
                    Rotation = doc.GetPage(i).GetRotation()
                });

            }
        }
    }

    public class PdfPageProps
    {
        public int PageNo { get; set; }
        public int Rotation { get; set; }
    }
}

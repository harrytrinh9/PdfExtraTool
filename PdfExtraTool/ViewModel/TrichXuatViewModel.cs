using iText.Kernel.Pdf;
using Microsoft.Win32;
using ModernWpf.Controls;
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
    public class TrichXuatViewModel: ViewModelBase
    {
        private string _selectedFile;
        private ICommand _selectFileCommand;
        private bool _isLoading;
        private string _openPassword;
        private string _protectPassword;
        private int _totalPage;
        private int _extractFromPage = 1;
        private int _extractToPage = 1;
        private int _minimumSliderToPage = 1;
        private ICommand _extractPdfPageCommand;

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
        public int MinimumSliderToPage { get => _minimumSliderToPage; set => SetProperty(ref _minimumSliderToPage, value); }

        public int ExtractFromPage
        {  get => _extractFromPage;
            set
            {
                SetProperty(ref _extractFromPage, value);
                MinimumSliderToPage = ExtractFromPage;
                ExtractToPage = ExtractFromPage;
            }
        }

        public int ExtractToPage { get => _extractToPage; set => SetProperty(ref _extractToPage, value); }

        public ICommand ExtractPdfPageCommand
        {
            get
            {
                if (_extractPdfPageCommand == null)
                {
                    _extractPdfPageCommand = new RelayCommand(_ => ExtractPdfPage(), _ => SelectedFile?.Length > 0);
                }
                return _extractPdfPageCommand;
            }
            set => _extractPdfPageCommand = value;
        }

        private async void SelectFile()
        {
            var openPdf = new OpenPdf();
            IsLoading = openPdf.IsLoading;
            await openPdf.Open().ConfigureAwait(true);
            IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.SelectedFile;
            _openPassword = openPdf.OpenPdfPassword;
            TotalPage = openPdf.TotalPage;
        }

        private async void ExtractPdfPage()
        {
            string saveFileName = $"{System.IO.Path.GetFileNameWithoutExtension(SelectedFile)}";
            if (ExtractFromPage != ExtractToPage)
            {
                saveFileName += $"_page_{ExtractFromPage}-{ExtractToPage}.pdf";
            }
            else if (ExtractFromPage == ExtractToPage)
            {
                saveFileName += $"_page_{ExtractFromPage}.pdf";
            }
            var s = new SaveFileDialog()
            {
                Filter = "Pdf file|*.pdf",
                FileName = saveFileName
            };
            if (s.ShowDialog() == false)
            {
                return;
            }

            //input doc
            PdfReader pdfReader;
            if (string.IsNullOrEmpty(_openPassword))
            {
                pdfReader = new PdfReader(SelectedFile);
            }
            else
            {
                pdfReader = new PdfReader(SelectedFile,
                new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(_openPassword)));
                pdfReader.SetUnethicalReading(true);
            }
            var pdfDoc = new PdfDocument(pdfReader);
            // output doc
            var pdfWriter = new PdfWriter(s.FileName);
            var pdfDocOutput = new PdfDocument(pdfWriter);

            pdfDoc.CopyPagesTo(ExtractFromPage, ExtractToPage, pdfDocOutput);

            pdfDoc.Close();
            pdfDocOutput.Close();
            void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
                System.Diagnostics.Process.Start(s.FileName);
            }
            await MsgBox.Show($"Đã trích xuất thành công từ trang: {ExtractFromPage} - {ExtractToPage}", "PDF Extra Tool", "Xem kết quả", Dlg_PrimaryButtonClick)
                .ConfigureAwait(true);
        }
    }
}

using iText.Kernel.Pdf;
using Microsoft.Win32;
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
    public class GoMatKhauViewModel: ViewModelBase
    {
        private string _selectedFile;
        private ICommand _selectFileCommand;
        private ICommand _unprotectPdfCommand;
        private bool _isLoading;
        private string _openPassword;

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

        public ICommand UnprotectPdfCommand
        {
            get
            {
                if (_unprotectPdfCommand == null)
                {
                    _unprotectPdfCommand = new RelayCommand(_ => RemovePassword(), _ => SelectedFile?.Length > 0);
                }
                return _unprotectPdfCommand;
            }
            set => _unprotectPdfCommand = value;
        }

        private async void SelectFile()
        {
            var openPdf = new OpenPdf();
            IsLoading = openPdf.IsLoading;
            await openPdf.Open().ConfigureAwait(true);
            IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.SelectedFile;
            _openPassword = openPdf.OpenPdfPassword;
        }

        private async void RemovePassword()
        {
            if (string.IsNullOrEmpty(_openPassword))
            {
                await MsgBox.Show("File bạn đã chọn không có mật khẩu.", "PDF Extra tool").ConfigureAwait(true);
                return;
            }
            var sf = new SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"{System.IO.Path.GetFileNameWithoutExtension(SelectedFile)}_no_Password.pdf"
            };
            sf.FileName = sf.FileName.Replace("[PW]_", "");
            if (sf.ShowDialog() == false)
            {
                return;
            }
            var pdfWriter = new PdfWriter(sf.FileName);
            var pdfReader = new PdfReader(SelectedFile,
            new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(_openPassword)));
            pdfReader.SetUnethicalReading(true);
            var pdfDoc = new PdfDocument(pdfReader, pdfWriter);
            pdfDoc.Close();
            await MsgBox.Show("Đã gỡ mật khẩu thành công.", "PDF Extra tool").ConfigureAwait(true);
        }
    }
}

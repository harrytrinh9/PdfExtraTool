using iText.Kernel.Pdf;
using Microsoft.Win32;
using ModernWpf.Controls;
using MVVMHelper;
using PdfExtraTool.Common;
using PdfExtraTool.Properties;
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
            IsLoading = true;
            await openPdf.Open().ConfigureAwait(true);
            IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.SelectedFile;
            _openPassword = openPdf.OpenPdfPassword;
            IsLoading = false;
        }

        private async void RemovePassword()
        {
            if (string.IsNullOrEmpty(_openPassword))
            {
                await MsgBox.Show(Resources.FileHaveNoPassword, "PDF Extra tool").ConfigureAwait(true);
                return;
            }
            var s = new SaveFileDialog
            {
                Filter = "PDF|*.pdf",
                FileName = $"{System.IO.Path.GetFileNameWithoutExtension(SelectedFile)}_no_Password.pdf"
            };
            s.FileName = s.FileName.Replace("[PW]_", "");
            if (s.ShowDialog() == false)
            {
                return;
            }
            var pdfWriter = new PdfWriter(s.FileName);
            var pdfReader = new PdfReader(SelectedFile,
            new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(_openPassword)));
            pdfReader.SetUnethicalReading(true);
            var pdfDoc = new PdfDocument(pdfReader, pdfWriter);
            pdfDoc.Close();
            void open_click(Object sender, ContentDialogButtonClickEventArgs args)
            {
                System.Diagnostics.Process.Start(s.FileName);
            }
            await MsgBox.Show(Resources.UnProtectSuccess, "PDF Extra tool", Resources.OpenNow, open_click).ConfigureAwait(true);
        }

    }
}

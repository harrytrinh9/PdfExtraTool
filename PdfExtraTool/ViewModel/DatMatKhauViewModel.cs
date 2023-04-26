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
    public class DatMatKhauViewModel: ViewModelBase
    {
        private string _selectedFile;
        private ICommand _selectFileCommand;
        private bool _isLoading;
        private string _openPassword;
        private string _protectPassword;
        private ICommand _setPasswordPdfCommand;
        private bool _allowPrint = true;
        private bool _allowCopy = false;

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

        public ICommand SetPasswordPdfCommand
        {
            get
            {
                if (_setPasswordPdfCommand == null)
                {
                    _setPasswordPdfCommand = new RelayCommand(_ => SetPasswordPdf(), _ => SelectedFile?.Length > 0);
                }
                return _setPasswordPdfCommand;
            }
            set => _setPasswordPdfCommand = value;
        }
        public bool AllowPrint { get => _allowPrint; set => SetProperty(ref _allowPrint, value); }
        public bool AllowCopy { get => _allowCopy; set => SetProperty(ref _allowCopy, value); }



        private async void SelectFile()
        {
            var openPdf = new OpenPdf();
            IsLoading = openPdf.IsLoading;
            await openPdf.Open().ConfigureAwait(true);
            IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.SelectedFile;
            _openPassword = openPdf.OpenPdfPassword;
        }

        private async void SetPasswordPdf()
        {
            if (ProtectPassword?.Length == 0)
            {
                await MsgBox.Show("Không được để trống mật khẩu").ConfigureAwait(true);
                return;
            }
            var s = new SaveFileDialog()
            {
                FileName = $"[PW]_{System.IO.Path.GetFileNameWithoutExtension(SelectedFile)}",
                Filter = "PDF file|*.pdf"
            };
            if (s.ShowDialog() == false)
            {
                return;
            }
            var userPass = Encoding.UTF8.GetBytes(ProtectPassword);
            var ownerPass = Encoding.UTF8.GetBytes("1111");
            PdfReader reader;
            if (_openPassword?.Length > 0)
            {
                reader = new PdfReader(SelectedFile,
                                new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(_openPassword)));
                reader.SetUnethicalReading(true);
            }
            else
            {
                reader = new PdfReader(SelectedFile);
            }


            int permisions = EncryptionConstants.ALLOW_FILL_IN;
            if (AllowCopy)
            {
                permisions = permisions | EncryptionConstants.ALLOW_COPY;
            }
            if (AllowPrint)
            {
                permisions = permisions | EncryptionConstants.ALLOW_PRINTING;
            }

            var props = new WriterProperties()
                .SetStandardEncryption(userPass, ownerPass, permisions, EncryptionConstants.ENCRYPTION_AES_128 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA);

            var writer = new PdfWriter(s.FileName, props);
            var doc = new PdfDocument(reader, writer);

            doc.Close();
            writer.Close();
            await MsgBox.Show("Đã bảo vệ thành công pdf với mật khẩu", "PDF Extra tool").ConfigureAwait(true);

        }
    }
}

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
    public class DatMatKhauViewModel: ViewModelBase
    {
        private string _selectedFile;
        private ICommand _selectFileCommand;
        private bool _isLoading;
        private string _openPassword;
        private string _protectPassword;
        private ICommand _setPasswordPdfCommand;
        private bool _allowPrint;
        private bool _allowCopy;
        private bool _allowFillIn = true;
        private bool _allowModifyAnnotations;
        private bool _allowModifyContents;
        private bool _allowScreenReaders;
        private bool _allowAssembly;
        private bool _allowDegradedPrint;

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
        public bool AllowFillIn { get => _allowFillIn; set => Set(ref _allowFillIn, value); }
        public bool AllowModifyAnnotations { get => _allowModifyAnnotations; set => Set(ref _allowModifyAnnotations, value); }
        public bool AllowModifyContents { get => _allowModifyContents; set => Set(ref _allowModifyContents, value); }
        public bool AllowScreenReaders { get => _allowScreenReaders; set => Set(ref _allowScreenReaders, value); }
        public bool AllowAssembly { get => _allowAssembly; set => Set(ref _allowAssembly, value); }
        public bool AllowDegradedPrint { get => _allowDegradedPrint; set => Set(ref _allowDegradedPrint, value); }

        private async void SelectFile()
        {
            var openPdf = new OpenPdf();
            IsLoading = true;
            await openPdf.Open().ConfigureAwait(true);
            IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.FileName;
            _openPassword = openPdf.OpenPdfPassword;
            IsLoading = false;
        }

        private async void SetPasswordPdf()
        {
            if (ProtectPassword?.Length == 0)
            {
                await MsgBox.Show(Resources.DontLeaveBlankPassword, Resources.SetPassword).ConfigureAwait(true);
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


            int permisions = 0;
            if (AllowFillIn)
            {
                permisions = permisions | EncryptionConstants.ALLOW_FILL_IN;
            }

            if (AllowCopy)
            {
                permisions = permisions | EncryptionConstants.ALLOW_COPY;
            }

            if (AllowPrint)
            {
                permisions = permisions | EncryptionConstants.ALLOW_PRINTING;
            }

            if (AllowModifyAnnotations)
            {
                permisions = permisions | EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS;
            }

            if (AllowModifyContents)
            {
                permisions = permisions | EncryptionConstants.ALLOW_MODIFY_CONTENTS;
            }

            if (AllowScreenReaders)
            {
                permisions = permisions | EncryptionConstants.ALLOW_SCREENREADERS;
            }

            if (AllowAssembly)
            {
                permisions = permisions | EncryptionConstants.ALLOW_ASSEMBLY;
            }

            if (AllowDegradedPrint)
            {
                permisions = permisions | EncryptionConstants.ALLOW_DEGRADED_PRINTING;
            }

            var props = new WriterProperties()
                .SetStandardEncryption(userPass, ownerPass, permisions, EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA);

            var writer = new PdfWriter(s.FileName, props);
            var doc = new PdfDocument(reader, writer);

            doc.Close();
            writer.Close();
            void open_click(Object sender, ContentDialogButtonClickEventArgs args)
            {
                System.Diagnostics.Process.Start(s.FileName);
            }
            await MsgBox.Show(Resources.ProtectedSuccess, "PDF Extra tool", Resources.OpenNow, open_click).ConfigureAwait(true);

        }
    }
}

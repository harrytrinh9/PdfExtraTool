using iText.Kernel.Pdf;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.Win32;
using ModernWpf.Controls;
using MVVMHelper;
using Org.BouncyCastle.Bcpg.OpenPgp;
using PdfExtraTool.Common;
using PdfExtraTool.Properties;
using PdfRenderByHarryTrinhWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PdfExtraTool.ViewModel
{
    public class LatTrangViewModel: ViewModelBase
    {
        private string _selectedFile;
        private ICommand _selectFileCommand;
        private bool _isLoading;
        private string openPdfPassword;
        private string _protectPassword;
        private int _totalPage;
        private ICommand _saveFileCommand;
        //private List<PdfPageView> _previewPdf;
        private ObservableCollection<PdfPreview> _previewPage = new ObservableCollection<PdfPreview>();

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
        public ICommand SaveFileCommand
        {
            get
            {
                if (_saveFileCommand == null)
                {
                    _saveFileCommand = new RelayCommand(_ => SaveFile());
                }
                return _saveFileCommand; 
            }

            set => _saveFileCommand = value;
        }
        //public List<PdfPageView> PreviewPdf { get => _previewPdf; set => Set(ref _previewPdf, value); }
        public ObservableCollection<PdfPreview> PreviewPage { get => _previewPage; set => Set(ref _previewPage, value); }

        private async void SelectFile()
        {
            PreviewPage.Clear();
            var openPdf = new OpenPdf();
            IsLoading = true;
            await openPdf.Open().ConfigureAwait(true);
            SelectedFile = openPdf.SelectedFile;
            if (string.IsNullOrEmpty(SelectedFile))
            {
                return;
            }

            openPdfPassword = openPdf.OpenPdfPassword;
            TotalPage = openPdf.TotalPage;


            RenderPdf render = new RenderPdf
            {
                FilePath = SelectedFile,
                Password = openPdfPassword
            };
            var previewPdf = await render.Render().ConfigureAwait(false);
            foreach (var item in previewPdf)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    PreviewPage.Add(new PdfPreview
                    {
                        Image = item.Image,
                        Page = item.Page,
                        TotalPage = item.TotalPage,
                        Orientation = 0

                    });
                });

            }
            IsLoading = false;

        }

        private void SaveFile()
        {
            var s = new SaveFileDialog
            {
                Title = Resources.SaveAs,
                Filter = "PDF|*.pdf|All file|*.*",
                FileName = $"{System.IO.Path.GetFileNameWithoutExtension(SelectedFile)}_rotated.pdf"
            };
            if (s.ShowDialog() == false)
            {
                return;
            }
            var pdfWriter = new PdfWriter(s.FileName);
            var outputDoc = new PdfDocument(pdfWriter);
            PdfReader pdfReader;
            if (openPdfPassword?.Length > 0)
            {
                pdfReader = new PdfReader(SelectedFile,
                new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(openPdfPassword)));
                pdfReader.SetUnethicalReading(true);
            }
            else
            {
                pdfReader = new PdfReader(SelectedFile);
            }
            var inputDoc = new PdfDocument(pdfReader);
            foreach (var item in PreviewPage)
            {
                var page = inputDoc.GetPage(item.Page);
                page.SetRotation(item.Orientation);
                inputDoc.CopyPagesTo(item.Page, item.Page, outputDoc);
            }
            outputDoc.Close();
            inputDoc.Close();

            var msg = string.Format(Resources.SaveSucess, s.FileName);

            void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
                System.Diagnostics.Process.Start(s.FileName);
            }
            void Later_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
                
            }
            _ = MsgBox.ShowYesNo(msg, Resources.Saved, Resources.OpenNow, Resources.Later, Dlg_PrimaryButtonClick, Later_Click);
        }

    }

    public class PdfPreview : ViewModelBase
    {
        private int page;
        private int totalPage;
        private BitmapImage image;
        private int orientation;
        private bool selected;

        public int Page { get => page; set => Set(ref page, value); }
        public int TotalPage { get => totalPage; set => Set(ref totalPage, value); }
        public BitmapImage Image { get => image; set => Set(ref image, value); }
        public int Orientation { get => orientation; set => Set(ref orientation, value); }
        public bool Selected { get => selected; set => Set(ref selected, value); }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PdfRenderByHarryTrinhWpf
{
    public class RenderPdf
    {
        private PdfDocument pdfDocument;
        public string FilePath { get; set; }
        public string Password { get; set; }
        //const int WrongPassword = unchecked((int)0x8007052b); // HRESULT_FROM_WIN32(ERROR_WRONG_PASSWORD)
        //const int GenericFail = unchecked((int)0x80004005);   // E_FAIL
        public async Task<List<PdfPageView>> Render()
        {
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(FilePath);
            if (!string.IsNullOrEmpty(Password))
            {
                pdfDocument = await PdfDocument.LoadFromFileAsync(file, Password);
            }
            else
            {
                pdfDocument = await PdfDocument.LoadFromFileAsync(file);
            }
            var totalPage = pdfDocument.PageCount;

            List<PdfPageView> listPageView = new List<PdfPageView>();

            for (uint i = 0; i < pdfDocument.PageCount; i++)
            {
                using (PdfPage page = pdfDocument.GetPage(i))
                {
                    PdfPageRenderOptions options = new PdfPageRenderOptions
                    {
                        DestinationWidth = (uint)(page.Size.Width),
                        DestinationHeight = (uint)(page.Size.Height),
                    };

                    var stream = new InMemoryRandomAccessStream();
                    await page.RenderToStreamAsync(stream, options);
                    // Chuyển InMemoryRandomAccessStream sang stream để đọc được với WPF
                    var imageSource = new System.Windows.Media.Imaging.BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = stream.AsStream(); // convert to stream of .NET
                    imageSource.EndInit();
                    listPageView.Add(new PdfPageView
                    {
                        Page = (int)i+1,
                        Image = imageSource,
                        TotalPage = (int)totalPage,
                        PageWidth = (float)imageSource.Width,
                        PageHeight = (float)imageSource.Height,
                    });
                }
            }
            return listPageView;

        }

    }

    public class PdfPageView : INotifyPropertyChanged
    {
        private int page;
        private int totalPage;
        private BitmapImage image;
        private float pageWidth;
        private float pageHeight;
        private int orientation;

        public int Page { get => page; set => Set(ref page, value); }
        public int TotalPage { get => totalPage; set => Set(ref totalPage, value); }
        public BitmapImage Image { get => image; set => Set(ref image, value); }
        public float PageWidth { get => pageWidth; set => Set(ref pageWidth, value); }
        public float PageHeight { get => pageHeight; set => Set(ref pageHeight, value); }
        public bool IsPortrait { get => PageHeight > PageWidth; }
        public int Orientation { get => orientation; set => Set(ref orientation, value); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

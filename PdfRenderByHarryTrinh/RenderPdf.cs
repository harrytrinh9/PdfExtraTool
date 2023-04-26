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

namespace PdfRenderByHarryTrinhWpf
{
    public class RenderPdf
    {
        private PdfDocument pdfDocument;
        public string FilePath { get; set; }
        public string Password { get; set; }
        const int WrongPassword = unchecked((int)0x8007052b); // HRESULT_FROM_WIN32(ERROR_WRONG_PASSWORD)
        const int GenericFail = unchecked((int)0x80004005);   // E_FAIL
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
                        TotalPage = (int)totalPage
                    });
                }
            }
            return listPageView;

        }

    }

    public class PdfPageView
    {
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public BitmapImage Image { get; set; }
    }
}

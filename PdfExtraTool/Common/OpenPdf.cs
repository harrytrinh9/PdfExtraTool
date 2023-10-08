using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using Microsoft.Win32;
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using PdfExtraTool.Properties;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PdfExtraTool.Common
{
    public class OpenPdf
    {
        public string FileName { get; set; }
        public bool IsLoading { get; set; }
        public int TotalPage { get; set; }
        public string OpenPdfPassword { get; set; }

        public async Task Open()
        {

            var o = new OpenFileDialog
            {
                Title = Resources.SelectPdfFile,
                Filter = "PDF file|*.pdf"
            };



            if (o.ShowDialog() == true)
            {

                //IsOpeningFile = true;
                IsLoading = true;
                FileName = o.FileName;

                MemoryStream stream = new MemoryStream();
                FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                file.CopyTo(stream);
                file.Close();

                IRandomAccessSource source = new RandomAccessSourceFactory().CreateSource(stream.ToArray());

                try
                {
                    await Task.Run(() =>
                    {
                        //var pdfReader = new PdfReader(FileName);
                        var pdfReader = new PdfReader(source, new ReaderProperties());
                        pdfReader.SetUnethicalReading(true);
                        var pdfDoc = new PdfDocument(pdfReader);
                        TotalPage = pdfDoc.GetNumberOfPages();
                        pdfDoc.Close();
                    }).ConfigureAwait(true);

                }
                catch (PdfException ex)
                {
                    if (ex.Message.Contains("password"))
                    {
                        OpenPdfPassword = await MsgBox.ShowInputPassword(Resources.PasswordRequirement).ConfigureAwait(true);
                        if (OpenPdfPassword.Length == 0)
                        {
                            FileName = null;
                            IsLoading = false;
                            return;
                        }

                        try
                        {
                            await Task.Run(() =>
                            {
                                var pdfReader = new PdfReader(FileName,
                                new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(OpenPdfPassword)));
                                pdfReader.SetUnethicalReading(true);
                                PdfDocument pdfDoc = new PdfDocument(pdfReader);
                                TotalPage = pdfDoc.GetNumberOfPages();
                                pdfDoc.Close();
                            }).ConfigureAwait(true);


                        }
                        catch
                        {
                            await MsgBox.Show(Resources.WrongPassword, Resources.UnableToOpenFile).ConfigureAwait(true);
                            FileName = null;
                        }

                    }
                }
                finally
                {
                    source.Close();
                    stream.Dispose();
                }
                IsLoading = false;
#if DEBUG
                Debug.WriteLine("File loaded, total Page {0}", TotalPage);
#endif

            }

        }

        public async Task Open(string fileName)
        {
            IsLoading = true;
            FileName = fileName;
            try
            {
                await Task.Run(() =>
                {
                    var pdfReader = new PdfReader(FileName);
                    pdfReader.SetUnethicalReading(true);
                    var pdfDoc = new PdfDocument(pdfReader);
                    TotalPage = pdfDoc.GetNumberOfPages();
                    pdfDoc.Close();
                }).ConfigureAwait(true);

            }
            catch (PdfException ex)
            {
                if (ex.Message.Contains("password"))
                {
                    OpenPdfPassword = await MsgBox.ShowInputPassword(Resources.PasswordRequirement).ConfigureAwait(true);
                    if (OpenPdfPassword.Length == 0)
                    {
                        FileName = null;
                        IsLoading = false;
                        return;
                    }

                    try
                    {
                        await Task.Run(() =>
                        {
                            var pdfReader = new PdfReader(FileName,
                            new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(OpenPdfPassword)));
                            pdfReader.SetUnethicalReading(true);
                            PdfDocument pdfDoc = new PdfDocument(pdfReader);
                            TotalPage = pdfDoc.GetNumberOfPages();
                            pdfDoc.Close();
                        }).ConfigureAwait(true);


                    }
                    catch
                    {
                        await MsgBox.Show(Resources.WrongPassword, Resources.UnableToOpenFile).ConfigureAwait(true);
                        FileName = null;
                    }

                }
            }

            IsLoading = false;

        }

    }
}

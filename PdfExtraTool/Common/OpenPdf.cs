using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using Microsoft.Win32;
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using PdfExtraTool.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PdfExtraTool.Common
{
    public class OpenPdf
    {
        public string SelectedFile { get; set; }
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
                SelectedFile = o.FileName;
                try
                {
                    await Task.Run(() =>
                    {
                        var pdfReader = new PdfReader(SelectedFile);
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
                            SelectedFile = null;
                            IsLoading = false;
                            return;
                        }

                        try
                        {
                            await Task.Run(() =>
                            {
                                var pdfReader = new PdfReader(SelectedFile,
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
                            SelectedFile = null;
                        }

                    }
                }

                IsLoading = false;
            }
        }

        public async Task Open(string fileName)
        {
            IsLoading = true;
            SelectedFile = fileName;
            try
            {
                await Task.Run(() =>
                {
                    var pdfReader = new PdfReader(SelectedFile);
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
                        SelectedFile = null;
                        IsLoading = false;
                        return;
                    }

                    try
                    {
                        await Task.Run(() =>
                        {
                            var pdfReader = new PdfReader(SelectedFile,
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
                        SelectedFile = null;
                    }

                }
            }

            IsLoading = false;

        }

    }
}

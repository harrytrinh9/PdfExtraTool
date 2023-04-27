using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using Microsoft.Win32;
using ModernWpf.Controls;
using MVVMHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ModernWpf.Controls.Primitives;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas;
using PdfExtraTool.Common;
using PdfRenderByHarryTrinhWpf;
using System.Resources;
using System.Globalization;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Windows;
using System.Reflection;

namespace PdfExtraTool.ViewModel
{
    public class DanhSoTrangViewModel: ViewModelBase
    {
        public DanhSoTrangViewModel()
        {

        }
        private string _selectedFile;
        private bool _isOpeningFile;
        private int _totalPage;
        private bool _isWorking;
        private float _fontSize = 11;
        private float _margin = 20;
        private ICommand _selectFileCommand;
        private ICommand _startPagingCommand;
        private string _startBtnContent = "Bắt đầu";
        private bool _isLoading;
        private string _pagingContent = "Page";
        private double _progressPercentage;
        private bool _isTopLeft;
        private bool _isTopCenter;
        private bool _isTopRight;
        private bool _isBottomLeft;
        private bool _isBottomCenter;
        private bool _isBottomRight;
        private string[] _listFont = new string[]{ "HELVETICA", "HELVETICA_OBLIQUE", "HELVETICA_BOLD", "HELVETICA_BOLDOBLIQUE", "COURIER", "COURIER_OBLIQUE", "COURIER_BOLD", "COURIER_BOLDOBLIQUE", "TIMES_ROMAN", "TIMES_ITALIC", "TIMES_BOLD" };
        private string _selectedFont = "HELVETICA";

        private string openPdfPassword;
        private List<PdfPageView> _previewPdf;


        public int TotalPage { get => _totalPage; set => SetProperty(ref _totalPage, value);}
        public bool IsOpeningFile{ get => _isOpeningFile; set => SetProperty(ref _isOpeningFile, value);}
        public string SelectedFile { get => _selectedFile; set => SetProperty(ref _selectedFile, value);}
        public bool IsWorking{ get => _isWorking;  set => SetProperty(ref _isWorking, value);}
        public float FontSize { get => _fontSize; set => SetProperty(ref _fontSize, value); }
        public float Margin { get => _margin; set => SetProperty(ref _margin, value);}
        public string StartBtnContent{ get => _startBtnContent; set => SetProperty(ref _startBtnContent, value); }

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
        public ICommand StartPagingCommand
        {
            get
            {
                if (_startPagingCommand == null)
                {
                    _startPagingCommand = new RelayCommand(_ => StartPaging(),
                        _ => CanStart());
                }
                return _startPagingCommand;
            }
            set => _startPagingCommand = value;
        }

        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        public string PagingContent
        {
            get => _pagingContent;
            set => SetProperty(ref _pagingContent, value);
        }

        public double ProgressPercentage
        {
            get => _progressPercentage;
            set => SetProperty(ref _progressPercentage, value);
        }

        public bool IsTopLeft
        {
            get => _isTopLeft;
            set => SetProperty(ref _isTopLeft, value);
        }

        public bool IsTopCenter
        {
            get => _isTopCenter;
            set => SetProperty(ref _isTopCenter, value);
        }

        public bool IsTopRight
        {
            get => _isTopRight;
            set => SetProperty(ref _isTopRight, value);
        }

        public bool IsBottomLeft
        {
            get => _isBottomLeft;
            set => SetProperty(ref _isBottomLeft, value);
        }

        public bool IsBottomCenter
        {
            get => _isBottomCenter;
            set => SetProperty(ref _isBottomCenter, value);
        }

        public bool IsBottomRight
        {
            get => _isBottomRight;
            set => SetProperty(ref _isBottomRight, value);
        }
        public string[] ListFont { get => _listFont; set => SetProperty(ref _listFont, value); }
        public string SelectedFont { get => _selectedFont; set => SetProperty(ref _selectedFont, value); }
        public List<PdfPageView> PreviewPdf { get => _previewPdf; set => SetProperty(ref _previewPdf, value); }

        private bool CanStart()
        {
            return SelectedFile?.Length > 0 &&
                 (IsBottomLeft || IsBottomCenter || IsBottomRight ||
                 IsTopLeft || IsTopCenter || IsTopRight) && !IsLoading
                 && !IsWorking;
        }

        private async void SelectFile()
        {
            IsLoading = true;
            var openPdf = new OpenPdf();
            //IsLoading = openPdf.IsLoading;
            await openPdf.Open().ConfigureAwait(true);
            //IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.SelectedFile;
            openPdfPassword = openPdf.OpenPdfPassword;
            TotalPage = openPdf.TotalPage;

            if (!string.IsNullOrEmpty(SelectedFile))
            {
                var render = new PdfRenderByHarryTrinhWpf.RenderPdf();
                render.FilePath = SelectedFile;
                render.Password = openPdfPassword;
                PreviewPdf = await render.Render().ConfigureAwait(false);
            }

            IsLoading = false;
        }


        private async void StartPaging()
        {
            IsWorking = true;
            StartBtnContent = "Đang đánh số trang...";

            var s = new SaveFileDialog
            {
                Title = "Lưu file ...",
                Filter = "PDF|*.pdf",
                FileName = $"{System.IO.Path.GetFileNameWithoutExtension(SelectedFile)}_paged.pdf"
            };
            if (s.ShowDialog() == false)
            {
                return;
            }
            var pdfWriter = new PdfWriter(s.FileName);
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
            #region Fonts
            var pdfDoc = new PdfDocument(pdfReader, pdfWriter);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            if (SelectedFont == "HELVETICA")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            }
            else if (SelectedFont == "HELVETICA_OBLIQUE")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);
            }
            else if (SelectedFont == "HELVETICA_BOLD")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            }
            else if (SelectedFont == "HELVETICA_BOLDOBLIQUE")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLDOBLIQUE);
            }
            else if (SelectedFont == "COURIER")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            }
            else if (SelectedFont == "COURIER_OBLIQUE")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.COURIER_OBLIQUE);
            }
            else if (SelectedFont == "COURIER_BOLD")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD);
            }
            else if (SelectedFont == "COURIER_BOLDOBLIQUE")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLDOBLIQUE);
            }
            else if (SelectedFont == "TIMES_ROMAN")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            }
            else if (SelectedFont == "TIMES_ITALIC")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ITALIC);
            }
            else if (SelectedFont == "TIMES_BOLD")
            {
                font = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
            }
            #endregion

            for (int i = 1; i <= TotalPage; i++)
            {
                PdfPage page = pdfDoc.GetPage(i);
                PdfCanvas canvas = new PdfCanvas(page);

                //var pageSize = page.GetPageSize();
                //var left = pageSize.GetLeft();
                //var top = pageSize.GetTop();
                //var bottom = pageSize.GetBottom();
                //var right = pageSize.GetRight();
                //var width = pageSize.GetWidth();
                //var height = pageSize.GetHeight();


                float posX;
                float posY;
                string _pagingStr = string.Format("{0} {1} / {2}", PagingContent, i, TotalPage);
                if (IsTopLeft)
                {
                    posX = page.GetPageSize().GetLeft() + Margin;
                    posY = page.GetPageSize().GetTop() - Margin;
                    canvas.BeginText().SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsTopCenter)
                {
                    posX = page.GetPageSize().GetWidth() / 2;
                    posY = page.GetPageSize().GetTop() - Margin;
                    canvas.BeginText().SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsTopRight)
                {
                    posX = page.GetPageSize().GetRight() - Margin - FontSize - _pagingStr.Length - 20;
                    posY = page.GetPageSize().GetTop() - Margin;
                    canvas.BeginText().SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsBottomLeft)
                {
                    posX = page.GetPageSize().GetLeft() + Margin;
                    posY = page.GetPageSize().GetBottom() + Margin;
                    canvas.BeginText().SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsBottomCenter)
                {
                    posX = page.GetPageSize().GetWidth() / 2;
                    posY = page.GetPageSize().GetBottom() + Margin;
                    canvas.BeginText().SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsBottomRight)
                {
                    posX = page.GetPageSize().GetWidth() - Margin - FontSize - _pagingStr.Length - 20;
                    posY = page.GetPageSize().GetBottom() + Margin;
                    canvas.BeginText().SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }


            }
            pdfDoc.Close();
            //pdfDoc.GetPage(1).SetRotation(90);
            const string msg = "File của bạn đã được đánh số trang thành công.\n(nếu file có mật khẩu, phần mềm sẽ tự bỏ mật khẩu sau khi đánh số trang)";
            void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
                System.Diagnostics.Process.Start(s.FileName);
            }
            await MsgBox.Show(msg, "PDF Extra tool", Properties.Resources.OpenNow, Dlg_PrimaryButtonClick).ConfigureAwait(true);

            IsWorking = false;
            StartBtnContent = "Bắt đầu";
            SelectedFile = null;
            TotalPage = 0;

        }



    }
}

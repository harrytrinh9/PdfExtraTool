using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;
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
using PdfExtraTool.Common;
using PdfRenderByHarryTrinhWpf;
using System.Resources;
using System.Globalization;
using System.Windows;
using System.Reflection;
using PdfExtraTool.Properties;
using System.Windows.Media;
using System.Collections.ObjectModel;

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
        private iText.Kernel.Colors.Color _fontColor = ColorConstants.BLACK;
        private ICommand _selectFileCommand;
        private ICommand _startPagingCommand;
        private ICommand _changeFontColorCommand;
        private string _startBtnContent = Resources.Start;
        private bool _isLoading;
        private string _pagingContent = "Page";
        private bool _isTopLeft;
        private bool _isTopCenter;
        private bool _isTopRight;
        private bool _isBottomLeft;
        private bool _isBottomCenter = true;
        private bool _isBottomRight;
        private string[] _listFont = new string[]{ "HELVETICA", "HELVETICA_OBLIQUE", "HELVETICA_BOLD", "HELVETICA_BOLDOBLIQUE", "COURIER", "COURIER_OBLIQUE", "COURIER_BOLD", "COURIER_BOLDOBLIQUE", "TIMES_ROMAN", "TIMES_ITALIC", "TIMES_BOLD" };
        private string _selectedFont = "HELVETICA";
        private double _progress;

        private string openPdfPassword;
        private ObservableCollection<PdfPageView> _previewPdf = new ObservableCollection<PdfPageView>();
        private float _topMargin = 15;
        private float _leftMargin = 20;
        private float _rightMargin = 20;
        private float _bottomMargin = 15;

        public int TotalPage { get => _totalPage; set => Set(ref _totalPage, value);}
        public bool IsOpeningFile{ get => _isOpeningFile; set => Set(ref _isOpeningFile, value);}
        public string SelectedFile { get => _selectedFile; set => Set(ref _selectedFile, value);}
        public bool IsWorking{ get => _isWorking;  set => Set(ref _isWorking, value);}
        public float FontSize { get => _fontSize; set => Set(ref _fontSize, value); }
        public string StartBtnContent{ get => _startBtnContent; set => Set(ref _startBtnContent, value); }

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

        public bool IsLoading { get => _isLoading; set => Set(ref _isLoading, value); }

        public string PagingContent { get => _pagingContent; set => Set(ref _pagingContent, value);}

        public bool IsTopLeft { get => _isTopLeft; set => Set(ref _isTopLeft, value);}

        public bool IsTopCenter{ get => _isTopCenter;set => Set(ref _isTopCenter, value);}

        public bool IsTopRight { get => _isTopRight;set => Set(ref _isTopRight, value); }

        public bool IsBottomLeft { get => _isBottomLeft; set => Set(ref _isBottomLeft, value);}

        public bool IsBottomCenter { get => _isBottomCenter;  set => Set(ref _isBottomCenter, value);}

        public bool IsBottomRight { get => _isBottomRight; set => Set(ref _isBottomRight, value);}

        public string[] ListFont { get => _listFont; set => Set(ref _listFont, value); }
        public string SelectedFont { get => _selectedFont; set => Set(ref _selectedFont, value); }
        public ObservableCollection<PdfPageView> PreviewPdf
        { 
            get => _previewPdf; 
            set => Set(ref _previewPdf, value); 
        }
        public double Progress { get => _progress; set => Set(ref _progress, value); }
        public float TopMargin { get => _topMargin; set => Set(ref _topMargin, value); }
        public float LeftMargin { get => _leftMargin; set => Set(ref _leftMargin, value); }
        public float RightMargin { get => _rightMargin; set => Set(ref _rightMargin, value); }
        public float BottomMargin { get => _bottomMargin; set => Set(ref _bottomMargin, value); }
        public ICommand ChangeFontColorCommand
        {
            get
            {
                if (_changeFontColorCommand == null)
                {
                    _changeFontColorCommand = new RelayCommand(color => ChangeFontColor(color));
                }
                return _changeFontColorCommand;
            }
            set => _changeFontColorCommand = value;
        }

        public iText.Kernel.Colors.Color FontColor { get => _fontColor; set => Set(ref _fontColor, value); }

        private void ChangeFontColor(object content)
        {
            var rectangle = (System.Windows.Shapes.Rectangle)content;
            var color = ((SolidColorBrush)rectangle.Fill).Color;
            FontColor = new DeviceRgb(color.R, color.G, color.B);
        }

        private bool CanStart()
        {
            return SelectedFile?.Length > 0 &&
                 (IsBottomLeft || IsBottomCenter || IsBottomRight ||
                 IsTopLeft || IsTopCenter || IsTopRight) && !IsLoading
                 && !IsWorking;
        }

        private async void SelectFile()
        {
            PreviewPdf.Clear();

            var openPdf = new OpenPdf();
            IsLoading = true;
            //IsLoading = openPdf.IsLoading;
            await openPdf.Open().ConfigureAwait(true);
            //IsLoading = openPdf.IsLoading;
            SelectedFile = openPdf.SelectedFile;
            openPdfPassword = openPdf.OpenPdfPassword;
            TotalPage = openPdf.TotalPage;

            if (!string.IsNullOrEmpty(SelectedFile))
            {
                RenderPdf render = new RenderPdf
                {
                    FilePath = SelectedFile,
                    Password = openPdfPassword
                };
                var pages = await render.Render().ConfigureAwait(false);
                foreach (var item in pages)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        PreviewPdf.Add(item);
                    });
    
                };
            }
            IsLoading = false;
        }


        private async void StartPaging()
        {
            IsWorking = true;
            StartBtnContent = Resources.Working;

            var s = new SaveFileDialog
            {
                Title = Resources.SaveAs,
                Filter = "PDF|*.pdf|All file|*.*",
                FileName = $"{System.IO.Path.GetFileNameWithoutExtension(SelectedFile)}_paged.pdf"
            };
            if (s.ShowDialog() == false)
            {
                StartBtnContent = Resources.Start;
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
                float posX;
                float posY;
                string _pagingStr = string.Format("{0} {1} / {2}", PagingContent, i, TotalPage);
                PdfPage page = pdfDoc.GetPage(i);
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SetColor(FontColor, true);
                canvas.SetFontAndSize(font, FontSize);

                //canvas.BeginText().ShowText(_pagingContent);
                //var pageSize = page.GetPageSize();
                //var left = pageSize.GetLeft();
                //var top = pageSize.GetTop();
                //var bottom = pageSize.GetBottom();
                //var right = pageSize.GetRight();
                //var width = pageSize.GetWidth();
                //var height = pageSize.GetHeight();


                if (IsTopLeft)
                {
                    posX = page.GetPageSize().GetLeft() + LeftMargin;
                    posY = page.GetPageSize().GetTop() - TopMargin;
                    canvas.BeginText()
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsTopCenter)
                {
                    posX = page.GetPageSize().GetWidth() / 2;
                    posY = page.GetPageSize().GetTop() - TopMargin;
                    canvas.BeginText() //.SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsTopRight)
                {
                    posX = page.GetPageSize().GetRight() - RightMargin - FontSize - _pagingStr.Length - 20;
                    posY = page.GetPageSize().GetTop() - TopMargin;
                    canvas.BeginText() //.SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsBottomLeft)
                {
                    posX = page.GetPageSize().GetLeft() + LeftMargin;
                    posY = page.GetPageSize().GetBottom() + BottomMargin;
                    canvas.BeginText() //.SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsBottomCenter)
                {
                    posX = page.GetPageSize().GetWidth() / 2;
                    posY = page.GetPageSize().GetBottom() + BottomMargin;
                    canvas.BeginText() //.SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }
                if (IsBottomRight)
                {
                    posX = page.GetPageSize().GetWidth() - RightMargin - FontSize - _pagingStr.Length - 20;
                    posY = page.GetPageSize().GetBottom() + BottomMargin;
                    canvas.BeginText() //.SetFontAndSize(font, FontSize)
                        .MoveText(posX, posY)
                        .ShowText(_pagingStr)
                        .EndText();
                }

                Progress = (double)i / TotalPage * 100;
                await Task.Delay(1).ConfigureAwait(true);
            }
            pdfDoc.Close();
            //pdfDoc.GetPage(1).SetRotation(90);
            string msg = Resources.PagingComplete;
            void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
                System.Diagnostics.Process.Start(s.FileName);
            }
            await MsgBox.Show(msg, "PDF Extra tool", Properties.Resources.OpenNow, Dlg_PrimaryButtonClick).ConfigureAwait(true);

            IsWorking = false;
            StartBtnContent = Resources.Start;
            SelectedFile = null;
            TotalPage = 0;

        }

        


    }
}

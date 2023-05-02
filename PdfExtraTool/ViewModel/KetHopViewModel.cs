using iText.Kernel.Pdf;
using Microsoft.Win32;
using ModernWpf.Controls;
using MVVMHelper;
using PdfExtraTool.Common;
using PdfExtraTool.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PdfExtraTool.ViewModel
{
    public class KetHopViewModel: ViewModelBase
    {
        private ICommand _addFileToListCommand;

        private ObservableCollection<InputPdf> _listInputPdf = new ObservableCollection<InputPdf>();
        private ICommand _mergePdfCommand;
        private ICommand _removeFileCommand;
        private int _totalPage;

        public ICommand AddFileToListCommand
        {
            get
            {
                if (_addFileToListCommand == null)
                {
                    _addFileToListCommand = new RelayCommand(_ => AddFileToList());
                }
                return _addFileToListCommand;
            }
            set => _addFileToListCommand = value;
        }

        public ObservableCollection<InputPdf> ListInputPdf
        {
            get => _listInputPdf;
            set
            {
                SetProperty(ref _listInputPdf, value);
            }
        }

        public ICommand MergePdfCommand
        {
            get
            {
                if (_mergePdfCommand == null)
                {
                    _mergePdfCommand = new RelayCommand(_ => MergePdf(), _ => ListInputPdf.Count > 0);
                }
                return _mergePdfCommand;
            }
            set => _mergePdfCommand = value;
        }

        public ICommand RemoveFileCommand
        {
            get
            {
                if (_removeFileCommand == null)
                {
                    _removeFileCommand = new RelayCommand(RemoveFile);
                }
                return _removeFileCommand;
            }
            set => _removeFileCommand = value;
        }

        public int TotalPage
        {
            get => _totalPage;
            set
            {
                SetProperty(ref _totalPage, value);
            }
        }

        private void RemoveFile(object param)
        {
            var file = (InputPdf)param;
            ListInputPdf.Remove(file);
            int dem = 1;
            foreach (var item in ListInputPdf)
            {
                item.No = dem;
                dem++;
            }
            TotalPage = ListInputPdf.Sum(x => x.PageCount);
        }

        private async void AddFileToList()
        {
            var o = new OpenFileDialog
            {
                Filter = "Pdf file|*.pdf",
                Multiselect = true
            };
            if (o.ShowDialog() == false)
            {
                return;
            }

            var openPdf = new OpenPdf();

            foreach (var item in o.FileNames)
            {
                var fi = new FileInfo(item);
                await openPdf.Open(fi.FullName).ConfigureAwait(true);
                ListInputPdf.Add(new InputPdf
                {
                    FileName = fi.Name,
                    Size = fi.Length,
                    FullPath = fi.FullName,
                    PageCount = openPdf.TotalPage,
                    OpenPassword = openPdf.OpenPdfPassword,
                    ModifiedDate = fi.LastWriteTime
                });
            }
            int dem = 1;
            foreach (var item in ListInputPdf)
            {
                item.No = dem;
                dem++;
            }
            TotalPage = ListInputPdf.Sum(x => x.PageCount);
        }


        private async void MergePdf()
        {
            var s = new SaveFileDialog()
            {
                Filter = "Pdf file|*.pdf"
            };
            if (s.ShowDialog() == false)
            {
                return;
            }
            var writer = new PdfWriter(s.FileName);
            var outputDoc = new PdfDocument(writer);
            foreach (var item in ListInputPdf)
            {
                if (string.IsNullOrEmpty(item.OpenPassword))
                {
                    var reader = new PdfReader(item.FullPath);
                    var doc = new PdfDocument(reader);
                    doc.CopyPagesTo(1, item.PageCount, outputDoc);
                    doc.Close();
                    reader.Close();
                }
                else
                {
                    var reader = new PdfReader(item.FullPath,
                    new ReaderProperties().SetPassword(Encoding.UTF8.GetBytes(item.OpenPassword)));
                    reader.SetUnethicalReading(true);
                    var doc = new PdfDocument(reader);
                    doc.CopyPagesTo(1, item.PageCount, outputDoc);
                    doc.Close();
                    reader.Close();
                }

            }
            outputDoc.Close();
            writer.Close();
            void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
                System.Diagnostics.Process.Start(s.FileName);
            }
            string msg = string.Format(Resources.MergedSuccess, ListInputPdf.Count, ListInputPdf.Sum(x => x.PageCount));
            await MsgBox.Show(msg, "PDF Extra tool", Resources.OpenNow, Dlg_PrimaryButtonClick)
                .ConfigureAwait(true);
        }
    }

    public class InputPdf : ViewModelBase
    {
        private int _no;
        private string _fileName;
        private string _fullPath;
        private long _size;
        private int _pageCount;
        private string _openPassword;
        private DateTime _modifiedDate;

        public int No { get => _no; set => SetProperty(ref _no, value); }
        public string FileName { get => _fileName; set => SetProperty(ref _fileName, value); }
        public string FullPath { get => _fullPath; set => SetProperty(ref _fullPath, value); }
        public long Size { get => _size; set => SetProperty(ref _size, value); }
        public int PageCount { get => _pageCount; set => SetProperty(ref _pageCount, value); }
        public string OpenPassword { get => _openPassword; set => SetProperty(ref _openPassword, value); }
        public DateTime ModifiedDate { get => _modifiedDate; set => SetProperty(ref _modifiedDate,value); }
    }
}

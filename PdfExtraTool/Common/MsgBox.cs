﻿using ModernWpf;
using ModernWpf.Controls;
using ModernWpf.Controls.Primitives;
using PdfExtraTool.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PdfExtraTool.Common
{
    public static class MsgBox
    {
        public async static Task Show(string message, string title = "")
        {
            var scrollViewer = new ScrollViewer
            {
                CanContentScroll = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap
            };
            scrollViewer.Content = textBlock;
            var dlg = new ContentDialog
            {
                Title = title,
                Content = scrollViewer,
                CloseButtonText = Resources.Close
            };
            await dlg.ShowAsync().ConfigureAwait(true);
        }

        public async static Task<string> ShowInputPassword(string title)
        {
            string Password = "";
            var dlg = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = "OK",
                CloseButtonText = Resources.Close,
            };
            var pwdBox = new PasswordBox();
            ControlHelper.SetHeader(pwdBox, Resources.InputPassword);
            dlg.Content = pwdBox;
            dlg.PrimaryButtonClick += (_, __) => Password = pwdBox.Password;

            await dlg.ShowAsync().ConfigureAwait(true);
            return Password;
        }

        public async static Task Show(string message, string title, string primaryButtonText, TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> primaryButtonClick)
        {
            var accentBtnStyle = Application.Current.FindResource("AccentButtonStyle") as Style;
            var scrollViewer = new ScrollViewer
            {
                CanContentScroll = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap
            };
            scrollViewer.Content = textBlock;
            var dlg = new ContentDialog
            {
                Title = title,
                Content = scrollViewer,
                PrimaryButtonText = primaryButtonText,
                PrimaryButtonStyle = accentBtnStyle,
                CloseButtonText = Resources.Close
            };
            dlg.PrimaryButtonClick += primaryButtonClick;
            await dlg.ShowAsync().ConfigureAwait(true);
        }

        public async static Task ShowYesNo(string message, string title, string yesBtnText, string noBtnText,
            TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> yesBtnClick,
            TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs> noBtnClick)
        {
            var accentBtnStyle = Application.Current.FindResource("AccentButtonStyle") as Style;
            var dlg = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = yesBtnText,
                PrimaryButtonStyle = accentBtnStyle,
                SecondaryButtonText = noBtnText,
            };
            dlg.PrimaryButtonClick += yesBtnClick;
            dlg.SecondaryButtonClick += noBtnClick;
            await dlg.ShowAsync().ConfigureAwait(true);
        }

    }
}

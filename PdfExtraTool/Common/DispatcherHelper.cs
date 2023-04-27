using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PdfExtraTool.Common
{
    internal static class DispatcherHelper
    {
        public static void DoEvents(DispatcherPriority priority = DispatcherPriority.Background)
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                priority,
                new DispatcherOperationCallback(ExitFrame),
                frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        public static void RunOnMainThread(Action action)
        {
            RunOnUIThread(Application.Current, action);
        }

        public static void RunOnUIThread(this DispatcherObject d, Action action)
        {
            var dispatcher = d.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }
    }

}

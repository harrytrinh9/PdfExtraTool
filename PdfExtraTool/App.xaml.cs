using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace PdfExtraTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }


        /// <summary>
        /// Resolve Assembly (*.dll) from embedded resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            if (!args.Name.Contains("resources"))
            {
                var a = executingAssembly.GetManifestResourceNames();
                if (args.Name.Contains("EntityFramework.SqlServer"))
                {
                    string ns = typeof(App).Namespace;
                    using (Stream stream = executingAssembly.GetManifestResourceStream($"{ns}.EmbeddedResources.EntityFramework.SqlServer.dll"))
                    {
                        byte[] block = new byte[stream.Length];
                        stream.Read(block, 0, block.Length);
                        return Assembly.Load(block);
                    }
                }
                var name = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";

                var resourceName = executingAssembly.GetManifestResourceNames().First(s => s.EndsWith(name));
                using (Stream stream = executingAssembly.GetManifestResourceStream(resourceName))
                {
                    byte[] block = new byte[stream.Length];
                    stream.Read(block, 0, block.Length);
                    return Assembly.Load(block);
                }
            }
            return null;
        }


        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Dispatcher Unhandled Exception");
#if DEBUG
            Console.WriteLine(e.Exception.ToString());
            Console.WriteLine(e.Exception.StackTrace);
#endif
            e.Handled = true;
        }
    }
}

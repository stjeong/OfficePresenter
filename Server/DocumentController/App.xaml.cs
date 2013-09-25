using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DocumentController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Dispatcher_UnhandledException);
            this.Dispatcher.UnhandledExceptionFilter += new System.Windows.Threading.DispatcherUnhandledExceptionFilterEventHandler(Dispatcher_UnhandledExceptionFilter);
        }

        void Dispatcher_UnhandledExceptionFilter(object sender, System.Windows.Threading.DispatcherUnhandledExceptionFilterEventArgs e)
        {
#if !DEBUG
            e.RequestCatch = true;
#endif
        }

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG
            MessageBox.Show(e.Exception.ToString());
#endif
            e.Handled = true;
        }
    }
}

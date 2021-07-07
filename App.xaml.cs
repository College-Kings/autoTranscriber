using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Auto_Transcriber
{
    public partial class App : Application
    {
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oscar Six", "AutoTranscriber");
            string logFile = Path.Combine(rootPath, "log.txt");

            using (StreamWriter sw = File.AppendText(logFile))
            {
                sw.WriteLine(e.Exception);
            }
        }
    }
}

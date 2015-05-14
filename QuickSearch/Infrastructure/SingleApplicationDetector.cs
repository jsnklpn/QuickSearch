using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;

namespace QuickSearch.Infrastructure
{
    public static class SingleApplicationDetector
    {
        private static Semaphore __semaphore;

        static SingleApplicationDetector()
        {
            Application.ApplicationExit += Application_ApplicationExit;
        }

        public static bool IsRunning()
        {
            string guid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
            var semaphoreName = @"Global\" + guid;
            try
            {
                __semaphore = Semaphore.OpenExisting(semaphoreName, SemaphoreRights.Synchronize);

                Close();
                return true;
            }
            catch
            {
                __semaphore = new Semaphore(0, 1, semaphoreName);
                return false;
            }
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try { Close(); }
            catch { }
        }

        public static void Close()
        {
            if (__semaphore != null)
            {
                __semaphore.Close();
                __semaphore = null;
            }
        }
    }
}
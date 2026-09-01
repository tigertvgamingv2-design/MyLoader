using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Loader.Core.Injection
{
    class Functions
    {
        // Webclient used for downloading dll bytes.
        private static WebClient wc = new WebClient();

        /// <summary>
        /// This method will return the process id of the provided process name
        /// </summary>
        /// <param name="processName">Name of the process</param>
        /// <returns>The process id of the provided process name</returns>
        private static int getProcessId(string processName)
        {
            Debug.WriteLine($"[Functions] Looking for process with name: {processName}");

            Process[] allMatches = Process.GetProcessesByName(processName);
            Debug.WriteLine($"[Functions] Found {allMatches.Length} matching process(es).");

            Process targetProcess = allMatches.FirstOrDefault();

            if (targetProcess == null)
            {
                Debug.WriteLine($"[Functions] No process found named \"{processName}\".");
                throw new InvalidOperationException($"No running process found with the name \"{processName}\".");
            }

            Debug.WriteLine($"[Functions] Found process id: {targetProcess.Id}");

            return targetProcess.Id;
        }

        /// <summary>
        /// Method for doing an example injection.
        /// </summary>
        public static void preformInjection()
        {
            Debug.WriteLine("[Functions] preformInjection called.");

            byte[] dllByteArray = null;
            Bleak.Injector injector = null;

            try
            {
                Debug.WriteLine($"[Functions] Downloading dll from: {Constants.dllUrl}");

                using (var downloadTask = Task.Run(() => wc.DownloadData(Constants.dllUrl)))
                {
                    if (!downloadTask.Wait(TimeSpan.FromSeconds(10)))
                    {
                        Debug.WriteLine("[Functions] Download timed out after 10 seconds.");
                        MessageBox.Show("The dll download timed out. Is the server at the configured URL reachable?");
                        return;
                    }

                    dllByteArray = downloadTask.Result;
                }

                Debug.WriteLine($"[Functions] Download succeeded, byte count: {dllByteArray?.Length ?? 0}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Functions] Download failed: {ex}");
                MessageBox.Show("There was an error while downloading the dll: " + ex.Message);
                return;
            }

            try
            {
                Debug.WriteLine("[Functions] Attempting injection...");
                injector = new Bleak.Injector(Bleak.InjectionMethod.ManualMap, getProcessId("NMRiH2-Win64-Shipping"), dllByteArray);

                IntPtr moduleBaseAddress = injector.InjectDll();
                Debug.WriteLine($"[Functions] Injection completed. Module base address: {moduleBaseAddress}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Functions] Injection failed: {ex}");
                MessageBox.Show("There was an error during the injection process: " + ex.Message);
                return;
            }

            Debug.WriteLine("[Functions] Exiting application after successful injection.");
            injector?.Dispose();
            Application.Exit();
        }
    }
}

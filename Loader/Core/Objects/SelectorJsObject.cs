using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;

namespace Loader.Core.Objects
{
    class SelectorJsObject
    {
        private static ChromiumWebBrowser _instanceBrowser = null;
        private static UI.Main _instanceMainForm = null;
     
        public SelectorJsObject(ChromiumWebBrowser originalBrowser, UI.Main mainForm)
        {
            _instanceBrowser = originalBrowser;
            _instanceMainForm = mainForm;
        }

        /// <summary>
        /// A method for handling a launch.
        /// </summary>
        /// <param name="selectedOption">The option that was selected to be launched.</param>
        /// <param name="checkState">The user's checkstate.</param>
        public void handleLaunch(string selectedOption, bool checkState)
        {
            Debug.WriteLine($"[SelectorJsObject] handleLaunch called. selectedOption=\"{selectedOption}\", checkState={checkState}");

            // Check if the user checked the remember selection box.
            if (checkState)
            {
                // Store the selected option
                Properties.Settings.Default.selectorOption = selectedOption;

                // If the checkstate was not stored as true before, set it to true.
                if (!Properties.Settings.Default.selectorCheckState)
                {
                    Properties.Settings.Default.selectorCheckState = true;
                }

                // Save all the changes to the application settings.
                Properties.Settings.Default.Save();
            }
            // If the user did not check the remember me box.
            else
            {
                // Store the checkstate & the selector option.
                Properties.Settings.Default.selectorCheckState = false;
                Properties.Settings.Default.selectorOption = string.Empty;

                // Save all the changes to the application settings.
                Properties.Settings.Default.Save();
            }

            Debug.WriteLine($"[SelectorJsObject] Entering switch statement with selectedOption=\"{selectedOption}\"");

            // Check what option the user selected.
            switch (selectedOption)
            {
                // If the selected option was nmrih2.
                case "nmrih2":
                    Debug.WriteLine("[SelectorJsObject] Matched case \"nmrih2\", calling preformInjection() on a background thread.");
                    // Run on a background thread so the blocking download/injection work doesn't freeze the CEF UI thread.
                    Task.Run(() => Core.Injection.Functions.preformInjection());
                    break;

                // If the selected option was option2.
                case "option2":
                    MessageBox.Show("Example option 2 was selected");
                    break;

                // If the selected option was option3.
                case "option3":
                    MessageBox.Show("Example option 3 was selected");
                    break;

                // If the option didn't match anything we expect.
                default:
                    Debug.WriteLine($"[SelectorJsObject] No case matched selectedOption=\"{selectedOption}\"");
                    MessageBox.Show($"Unrecognized option selected: \"{selectedOption}\"");
                    break;
            }
        }

        /// <summary>
        /// A method for easily passing remember selection information to our javascript.
        /// By converting the information to a JSON object.
        /// </summary>
        /// <returns>
        /// Returns a JSON object containing selection information information.
        /// </returns>
        public string handleRememberSelection()
        {
            Debug.WriteLine($"[SelectorJsObject] handleRememberSelection called. selectorCheckState={Properties.Settings.Default.selectorCheckState}, selectorOption=\"{Properties.Settings.Default.selectorOption}\"");

            // Check the stored checkstate.
            if (Properties.Settings.Default.selectorCheckState)
            {
                // Create a new instance of the selection object.
                JSON.Selection currentSelection = new JSON.Selection();

                // Set the values
                currentSelection.selectedOption = Properties.Settings.Default.selectorOption;
                currentSelection.checkState = Properties.Settings.Default.selectorCheckState;

                // Convert the object to a JSON object & return it.
                return JsonConvert.SerializeObject(currentSelection).ToString();
            }

            // Default return value: still valid JSON so JSON.parse in selector.js doesn't throw.
            return JsonConvert.SerializeObject(new JSON.Selection
            {
                selectedOption = string.Empty,
                checkState = false
            });
        }
    }
}

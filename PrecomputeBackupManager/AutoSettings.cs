using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace PrecomputeBackupManager
{
    class AutoSettings
    {
        public Dictionary<string, string> allSettings = new Dictionary<string, string>();
        private const string prefix = "setting_";

        // Use this class to hold all information regarding a setting
        static JavaScriptSerializer json = new JavaScriptSerializer();
        internal class ControlInformation
        {
            public object Value;
            public bool Enabled;
        }

        // Get data from control and serialize it to string
        private string GetDataFromControl(Control ctrl)
        {
            ControlInformation ci = new ControlInformation();
            ci.Enabled = ctrl.Enabled;

            if (ctrl is TextBox)
                ci.Value = ((TextBox)ctrl).Text;

            else if (ctrl is CheckBox)
                ci.Value = ((CheckBox)ctrl).Checked;

            else if (ctrl is NumericUpDown)
                ci.Value = ((NumericUpDown)ctrl).Value;

            else
                return null; // Didnt find knwon control;


            return json.Serialize(ci);
        }

        // Deserialize data and put in control.
        private bool SetDataFromControl(Control ctrl, string settingValue)
        {
            ControlInformation ci = json.Deserialize<ControlInformation>(settingValue);
            ctrl.Enabled = ci.Enabled;

            if (ctrl is TextBox)
                ((TextBox)ctrl).Text = (String)ci.Value;

            else if (ctrl is CheckBox)
                ((CheckBox)ctrl).Checked = (bool)ci.Value;

            else if (ctrl is NumericUpDown)
                ((NumericUpDown)ctrl).Value = (int)ci.Value;

            else
                return false; // Didnt find knwon control;

            return true; // Sucess
        }

        // Save all data from control to the main dictionary
        public void ReadDataFromControl(Control root)
        {
            string controlKey = (string)root.Tag;

            if (controlKey != null && controlKey.ToLower().StartsWith(prefix)) // ingore non settings keys
            {

                if (allSettings.ContainsKey(controlKey))
                {
                    Console.WriteLine("[ERR] Found 2 controls with same key! current control: "
                        + root.Name + "\n Duplicate key: " + controlKey
                        );
                }
                else
                {
                    string controlData = GetDataFromControl(root);
                    if (controlData != null)
                    {
                        allSettings.Add(controlKey, controlData);
                        Console.WriteLine("* Added key: " + controlKey + " from control: " + root.Name);
                    }
                    else
                    {
                        Console.WriteLine("[ERR] Couldn't get data from control! current control: " + root.Name);
                    }
                }

            }

            // Do this recursivly because controls has hierarchy
            foreach (Control child in root.Controls)
            {
                ReadDataFromControl(child);
            }
        }

        // Load all data from main dictionary
        public void WriteDataFromDictionary(Control root) // ingore non settings keys
        {
            string controlKey = (string)root.Tag;

            if (controlKey != null && controlKey.ToLower().StartsWith(prefix))
            {

                if (allSettings.ContainsKey(controlKey))
                {

                    if (SetDataFromControl(root, allSettings[controlKey]))
                    {
                        Console.WriteLine("* Read key: " + controlKey + " from control: " + root.Name);
                    }
                    else
                    {
                        Console.WriteLine("[ERR] Couldn't set data to control! current control: "
                            + root.Name + "\nData: \n" + allSettings[controlKey]
                        );
                    }
                    
                }
                else
                {
                    Console.WriteLine("[ERR] Didn't found key! current control: " 
                        + root.Name + "\n Control key: " + controlKey);
                }

            }

            // Do this recursivly because controls has hierarchy
            foreach (Control child in root.Controls)
            {
                WriteDataFromDictionary(child);
            }
        }
    }
}

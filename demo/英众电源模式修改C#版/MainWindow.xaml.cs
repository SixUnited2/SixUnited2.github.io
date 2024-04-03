using System;
using System.Management;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace WmiPowerModeSwitcher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetModeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (powerModeComboBox.SelectedItem is ComboBoxItem selectedMode)
                {
                    byte modeValue = Convert.ToByte(selectedMode.Tag.ToString(), 16);
                    Log($"Attempting to set power mode to {selectedMode.Content}...");
                    SetPowerMode(modeValue);
                }
                else
                {
                    MessageBox.Show("Please select a power mode first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred SetModeButton_Click: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetPowerMode(byte modeValue)
        {
            try
            {
                ManagementObject classInstance =
                    new ManagementObject("root\\WMI",
                    @"PowerSwitchInterface.InstanceName='ACPI\PNP0C14\IP3POWERSWITCH_0'",
                    null);

                // Obtain in-parameters for the method
                ManagementBaseObject inParams =
                    classInstance.GetMethodParameters("SetPowerMode");

                // Add the input parameters.
                inParams["PowerMode"] = modeValue;

                // Execute the method and obtain the return values.
                ManagementBaseObject outParams =
                    classInstance.InvokeMethod("SetPowerMode", inParams, null);

                // List outParams
                Log("Out parameters:");
                Log("ResultStatus: " + outParams["ResultStatus"]);
            }
            catch (ManagementException err)
            {
                MessageBox.Show("An error occurred while trying to execute the WMI method: " + err.Message);
            }
        }

        private void GetModeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte mode = GetPowerMode();
                string modeString = mode switch
                {
                    0x00 => "Balance mode",
                    0x01 => "Performance mode",
                    0x02 => "Quiet mode",
                    0x03 => "Super mode",
                    _ => "Unknown"
                };

                Log($"Current power mode: {modeString}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetPowerMode");
                MessageBox.Show($"An error occurred GetModeButton_Click: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private byte GetPowerMode()
        {
            try
            {
                ManagementObject classInstance =
                    new ManagementObject("root\\WMI",
                    @"PowerSwitchInterface.InstanceName='ACPI\PNP0C14\IP3POWERSWITCH_0'",
                    null);

                // no method in-parameters to define


                // Execute the method and obtain the return values.
                ManagementBaseObject outParams =
                    classInstance.InvokeMethod("GetPowerMode", null, null);

                // List outParams
                Log("Out parameters:");
                Log("CurrentPowerMode: " + outParams["CurrentPowerMode"]);
                return byte.Parse(outParams["CurrentPowerMode"].ToString());
            }
            catch (ManagementException err)
            {
                MessageBox.Show("An error occurred while trying to execute the WMI method: " + err.Message);
            }

            return 0;
        }

        private void Log(string message)
        {
            logTextBox.AppendText($"{DateTime.Now}: {message}\n");
            logTextBox.ScrollToEnd();
        }

    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Ports;
using Modbus.Device;
using System.Threading.Tasks;
using Modbus.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using Modbus.Utility;

namespace F28027TempTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort portObj;

        bool modbusLock = false;

        string textSendFormat;

        //private Modbus master;

        public MainWindow()
        {
            InitializeComponent();
        }

        //private void clearTestTextLogeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    testTextLog.Clear();
        //}

        //private void portTestReceive_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    if (!modbusLock)
        //    {
        //        SerialPort sp = (SerialPort)sender;
        //        string indata = null;

        //        try
        //        {
        //            indata = sp.ReadExisting();
        //        }
        //        catch
        //        {
        //            indata = "[Error data received]";
        //        }

        //        if (this.textSendFormat == "HEX")
        //        {
        //            WriteLog($"<{DateTime.Now.ToString()}> <-- {BitConverter.ToString(Encoding.ASCII.GetBytes(indata))} \n");
        //        }
        //        else
        //        {
        //            WriteLog($"<{DateTime.Now.ToString()}> <-- {Regex.Replace(indata, @"[\n]", "\\n")} \n");
        //        }
        //    }
        //}

        //private void RadioButton_sendfilter_Checked(object sender, RoutedEventArgs e)
        //{
        //    this.textSendFormat = ((RadioButton)sender).Tag.ToString();
        //    if (this.textSendFormat == "HEX")
        //    {
        //        transmitPlusCrcCheckBox.IsEnabled = true;
        //    }
        //    else
        //    {
        //        transmitPlusCrcCheckBox.IsEnabled = false;
        //        transmitPlusCrcCheckBox.IsChecked = false;
        //    }
        //}

        //private void receiveTextLog_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if ((bool)receiveAutoscrollCheckBox.IsChecked)
        //    {
        //        ((TextBox)sender).ScrollToEnd();
        //    }
        //}

        //private void sendTransmitButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (portObj != null)
        //        {
        //            string t = transmitTextBox.Text;
        //            byte[] bytes = null;

        //            if (textSendFormat == "HEX")
        //            {
        //                bytes = ConvertHexStringToByteArray(t);

        //            }
        //            else if (textSendFormat == "ASCII")
        //            {
        //                bytes = Encoding.ASCII.GetBytes(t);
        //            }

        //            if ((bool)transmitPlusCrCheckBox.IsChecked)
        //            {
        //                bytes = bytes.Concat(Encoding.ASCII.GetBytes(portObj.NewLine)).ToArray();
        //            }

        //            if ((bool)transmitPlusCrcCheckBox.IsChecked)
        //            {
        //                bytes = bytes.Concat(ModbusUtility.CalculateCrc(bytes)).ToArray();
        //            }

        //            portObj.Write(bytes, 0, bytes.Length);

        //            if(this.textSendFormat == "HEX")
        //            {
        //                WriteLog($"<{DateTime.Now.ToString()}> --> {BitConverter.ToString(bytes)} \n");
        //            }
        //            else
        //            {
        //                WriteLog($"<{DateTime.Now.ToString()}> --> {Regex.Replace(System.Text.Encoding.UTF8.GetString(bytes), @"[\n]", "\\n")} \n");
        //            }
                    
        //        }
        //        else
        //        {
        //            throw new Exception("Please open a COM port before sending data");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "COM Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void ModbusTransmitEvent(byte[] str)
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        testTextLog.Text += $"<info>TX: {BitConverter.ToString(str)} \n";
        //    });
        //}
        //private void ModbusReceiveEvent(byte[] str)
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        testTextLog.Text += $"<info>RX: {BitConverter.ToString(str)} \n";
        //    });
        //}

        //private void WriteLog(string str)
        //{
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        testTextLog.Text += str;
        //    });
        //}

        //private async void testModbusButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (portObj != null)
        //    {
        //        ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(portObj);
        //        try
        //        {
        //            modbusLock = true;
        //            ((ModbusSerialTransport)(master.Transport)).DataTransmited += ModbusTransmitEvent;
        //            ((ModbusSerialTransport)(master.Transport)).DataReceived += ModbusReceiveEvent;

        //            byte slaveID = 0x0f;
        //            ushort startAddress = 0;
        //            ushort numOfPoints = 1;
        //            ushort[] hr = await Task.Run(() => master.ReadHoldingRegisters(slaveID, startAddress, numOfPoints));
        //            WriteLog("<info>DATA:" + string.Join(",", hr) + "\n");
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.ToString(), "COM Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        finally
        //        {

        //            ((ModbusSerialTransport)(master.Transport)).DataTransmited -= ModbusTransmitEvent;
        //            ((ModbusSerialTransport)(master.Transport)).DataReceived -= ModbusReceiveEvent;
        //            modbusLock = false;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please open a COM port before sending data", "COM Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            hexString = Regex.Replace(hexString, @"[ \-]", "");
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}

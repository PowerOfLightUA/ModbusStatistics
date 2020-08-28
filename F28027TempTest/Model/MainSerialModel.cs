using F28027TempTest.Model;
using Modbus.Device;
using Modbus.IO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace F28027TempTest
{
    class MainSerialModel
    {
        private static MainSerialModel instance;

        private static object syncRoot = new Object();

        public SerialPort SerialObj { get; private set; }

        public bool IsConnected { private set; get; }

        public string ComPort { get; set; }

        public uint BaudRate { get; set; }

        public Parity Parity { get; set; }

        public StopBits StopBits { get; set; }

        public int ReceiveSplitTimeMs { get; set; } = 100;

        public ObservableCollection<SerialLogEntity> SerialLogs { get; private set; } = new ObservableCollection<SerialLogEntity>();


        protected MainSerialModel()
        {
        }

        public static MainSerialModel getInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new MainSerialModel();
                }
            }
            return instance;
        }

        public void SendBytes(byte[] bytes)
        {
            SerialObj.Write(bytes, 0, bytes.Length);
            LogTransmitEvent(bytes);
        }

        public bool Connect()
        {
            SerialObj = new SerialPort();
            try
            {
                // настройки порта
                SerialObj.PortName = ComPort;
                SerialObj.BaudRate = (int)BaudRate;
                SerialObj.DataBits = 8;
                SerialObj.Parity = Parity;
                SerialObj.StopBits = StopBits;
                SerialObj.ReadTimeout = 1000;
                SerialObj.WriteTimeout = 1000;

                SerialObj.Open();
                IsConnected = true;

                SerialObj.DataReceived += SerialObj_DataReceived;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "COM Open Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }

        private void SerialObj_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int c = sp.BytesToRead;
            byte[] indata = new byte[c];
            sp.Read(indata, 0, c);
            LogReceiveEvent(indata);
        }

        public bool Disconnect()
        {
            if (SerialObj != null && SerialObj.IsOpen)
            {
                SerialObj.Close();
                SerialObj = null;

                IsConnected = false;
                return true;
            }
            return false;
        }

        private void LogTransmitEvent(byte[] bytes)
        {
            SerialLogs.Add(new SerialLogEntity(DateTime.Now, "TX", new List<byte>(bytes)));
        }
        private void LogReceiveEvent(byte[] bytes)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (SerialLogs.Count > 0 && ReceiveSplitTimeMs != 0 && (DateTime.Now - SerialLogs[SerialLogs.Count - 1].Time).TotalMilliseconds < ReceiveSplitTimeMs)
                {
                    SerialLogs[SerialLogs.Count - 1] = new SerialLogEntity(DateTime.Now, "RX", SerialLogs[SerialLogs.Count - 1].Bytes.Concat(bytes).ToList());
                }
                else
                {
                    SerialLogs.Add(new SerialLogEntity(DateTime.Now, "RX", new List<byte>(bytes)));
                }
            });
        }
    }
}

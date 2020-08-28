using F28027TempTest.Command;
using F28027TempTest.Model;
using F28027TempTest.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Modbus.Utility;

namespace F28027TempTest.ViewModel
{
    class MainSerialViewModel : ViewModelBase
    {
        private MainSerialModel msm;

        #region Variables
        public ObservableCollection<string> ComPortsList
        {
            get;
            private set;
        }

        public string ComPort
        {
            get => msm.ComPort;
            set
            {
                msm.ComPort = value;
                RaisePropertyChanged();
            }
        }

        public bool IsConnected
        {
            get => msm.IsConnected;
        }

        public uint BaudRate
        {
            get => msm.BaudRate;
            set
            {
                msm.BaudRate = value;
                RaisePropertyChanged();
            }

        }

        public string Parity
        {
            get => msm.Parity.ToString();
            set
            {
                msm.Parity = (Parity)Enum.Parse(typeof(Parity), value);
                RaisePropertyChanged("Parity");
            }

        }

        public string StopBits
        {
            get => msm.StopBits.ToString();
            set
            {
                msm.StopBits = (StopBits)Enum.Parse(typeof(StopBits), value);
                RaisePropertyChanged("StopBits");
            }

        }

        public int ReceiveSplitTimeMs
        {
            get => msm.ReceiveSplitTimeMs;
            set => msm.ReceiveSplitTimeMs = value;
        }

        public ObservableCollection<SerialLogEntity> SerialLogs
        {
            get => msm.SerialLogs;
        }

        public bool SendPlus_CRC { get; set; } = false;

        public bool SendPlus_CR { get; set; } = false;

        #region TestingText
        private string _TestingSendText;
        public string TestingSendText
        {
            get => _TestingSendText;
            set
            {
                if (TestingTextIsHex)
                {
                    string last = _TestingSendText;
                    string n = StringHexFormatter.Prettify(value);
                    if (StringHexFormatter.IsGood(n) || n == "")
                    {
                        _TestingSendText = n;
                    }
                    else
                    {
                        _TestingSendText = last;
                    }
                }
                else
                {
                    _TestingSendText = value;
                }
            }
        }

        public bool TestingTextIsHex { get; set; } = false;
        public bool TestingTextIsAscii { get; set; } = true;

        
        #endregion

        #endregion

        #region Commands

        #region RefreshComPortsListCommand
        public ICommand RefreshComPortsListCommand { get; }
        private bool CanRefreshComPortsListCommandExecute(object p) => true;
        private void OnRefreshComPortsListCommandExecuted(object p)
        {
            RefreshComPortsList();
            RaisePropertyChanged("ComPortsList");
            ComPort = ComPortsList.Count > 0 ? ComPortsList[0] : null;
        }
        #endregion

        #region ConnectDisconnectSerialCommand

        public ICommand ConnectDisconnectSerialCommand { get; }
        private bool CanConnectDisconnectSerialCommandExecute(object p) => ((msm.ComPort != null && msm.BaudRate != 0) || msm.IsConnected);
        private void OnConnectDisconnectSerialCommandExecuted(object p)
        {
            if (!msm.IsConnected)
            {
                msm.Connect();
            }
            else
            {
                msm.Disconnect();
            }
            RaisePropertyChanged("IsConnected");
        }

        #endregion

        #region TestingSendSerialCommand
        public ICommand TestingSendSerialCommand { get; }
        private bool CanTestingSendSerialCommandExecute(object p) => (msm.IsConnected);
        private void OnTestingSendSerialCommandExecuted(object p)
        {
            if (msm.IsConnected)
            {
                byte[] bytes = null;
                try
                {
                    if (TestingTextIsAscii)
                    {
                        bytes = ASCIIEncoding.ASCII.GetBytes(TestingSendText);
                    }
                    else if (TestingTextIsHex)
                    {
                        bytes = StringHexFormatter.ConvertHexStringToByteArray(TestingSendText);
                    }

                    if(SendPlus_CR) bytes = bytes.Concat(Encoding.ASCII.GetBytes(msm.SerialObj.NewLine)).ToArray();
                    if(SendPlus_CRC) bytes = bytes.Concat(ModbusUtility.CalculateCrc(bytes)).ToArray();
                    msm.SendBytes(bytes);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion

        #region ClearTestingLog

        public ICommand ClearTestingLog { get; }
        private bool CanClearTestingLogExecute(object p) => (msm.SerialLogs.Count >0);
        private void OnClearTestingLogExecuted(object p)
        {
            msm.SerialLogs.Clear();
        }

        #endregion

        #endregion

        public MainSerialViewModel()
        {            
            SettingsInit();

            RefreshComPortsListCommand = new LambdaCommand(OnRefreshComPortsListCommandExecuted, CanRefreshComPortsListCommandExecute);

            ConnectDisconnectSerialCommand = new LambdaCommand(OnConnectDisconnectSerialCommandExecuted, CanConnectDisconnectSerialCommandExecute);

            TestingSendSerialCommand = new LambdaCommand(OnTestingSendSerialCommandExecuted, CanTestingSendSerialCommandExecute);

            ClearTestingLog = new LambdaCommand(OnClearTestingLogExecuted, CanClearTestingLogExecute);
        }

        private void SettingsInit()
        {
            msm = MainSerialModel.getInstance();

            BaudRate = 9600;
            Parity = "None";
            StopBits = "One";

            RefreshComPortsList();
            ComPort = ComPortsList.Count > 0 ? ComPortsList[0] : null;
        }

        private void RefreshComPortsList()
        {
            ComPortsList = new ObservableCollection<string>(SerialPort.GetPortNames());
        }
    }
}
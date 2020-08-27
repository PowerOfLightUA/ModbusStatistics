using F28027TempTest.Command;
using F28027TempTest.Model;
using F28027TempTest.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        public string TestingSendText { get; set; } = "Test";

        public ObservableCollection<SerialLogEntity> SerialLogs
        {
            get => msm.SerialLogs;
        }

        public bool TestingTextIsHex { get; set; } = false;
        public bool TestingTextIsAscii { get; set; } = true;

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

        public ICommand TestingSendSerialCommand { get; }
        private bool CanTestingSendSerialCommandExecute(object p) => (msm.IsConnected);
        private void OnTestingSendSerialCommandExecuted(object p)
        {
            if (msm.IsConnected)
            {
                msm.SendBytes(ASCIIEncoding.ASCII.GetBytes(TestingSendText));
            }
        }

        #endregion

        public MainSerialViewModel()
        {
            SettingsInit();

            RefreshComPortsListCommand = new LambdaCommand(OnRefreshComPortsListCommandExecuted, CanRefreshComPortsListCommandExecute);

            ConnectDisconnectSerialCommand = new LambdaCommand(OnConnectDisconnectSerialCommandExecuted, CanConnectDisconnectSerialCommandExecute);

            TestingSendSerialCommand = new LambdaCommand(OnTestingSendSerialCommandExecuted, CanTestingSendSerialCommandExecute);
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
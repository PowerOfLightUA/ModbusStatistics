using F28027TempTest.Command;
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

namespace F28027TempTest.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        #region Variables
        public ObservableCollection<string> ComPortsList
        {
            get;
            private set;
        }

        public string ComPort
        {
            get => SettingsModel.getInstance().ComPort;
            set
            {
                SettingsModel.getInstance().ComPort = value;
                RaisePropertyChanged();
            }
        }

        public bool IsConnected
        {
            get => SettingsModel.getInstance().IsConnected;
        }

        public uint BaudRate
        {
            get => SettingsModel.getInstance().BaudRate;
            set
            {
                SettingsModel.getInstance().BaudRate = value;
                RaisePropertyChanged();
            }

        }

        public string Parity
        {
            get => SettingsModel.getInstance().Parity.ToString();
            set
            {
                SettingsModel.getInstance().Parity = (Parity)Enum.Parse(typeof(Parity), value);
                RaisePropertyChanged("Parity");
            }

        }

        public string StopBits
        {
            get => SettingsModel.getInstance().StopBits.ToString();
            set
            {
                SettingsModel.getInstance().StopBits = (StopBits)Enum.Parse(typeof(StopBits), value);
                RaisePropertyChanged("StopBits");
            }

        }

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
        private bool CanConnectDisconnectSerialCommandExecute(object p) => ((SettingsModel.getInstance().ComPort != null && SettingsModel.getInstance().BaudRate != 0) || SettingsModel.getInstance().IsConnected);
        private void OnConnectDisconnectSerialCommandExecuted(object p)
        {
            if (!SettingsModel.getInstance().IsConnected)
            {
                SettingsModel.getInstance().Connect();
            }
            else
            {
                SettingsModel.getInstance().Disconnect();
            }
            RaisePropertyChanged("IsConnected");
        }

        #endregion

        #endregion

        public SettingsViewModel()
        {
            RefreshComPortsListCommand = new LambdaCommand(OnRefreshComPortsListCommandExecuted, CanRefreshComPortsListCommandExecute);

            ConnectDisconnectSerialCommand = new LambdaCommand(OnConnectDisconnectSerialCommandExecuted, CanConnectDisconnectSerialCommandExecute);

            SettingsInit();
        }

        private void SettingsInit()
        {
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
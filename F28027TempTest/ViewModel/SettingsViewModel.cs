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
            set {
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

        #endregion

        #region Commands

        #region RefreshComPortsListCommand
        public ICommand RefreshComPortsListCommand { get; }
        private bool CanRefreshComPortsListCommandExecute(object p) => true;
        private void OnRefreshComPortsListCommandExecuted(object p)
        {
            ComPortsList = new ObservableCollection<string>(SerialPort.GetPortNames());
            RaisePropertyChanged("ComPortsList");
        }
        #endregion

        #region SelectBaudRateCommand
        public ICommand SelectBaudRateCommand { get; }
        private bool CanSelectBaudRateCommandExecute(object p) => true;
        private void OnSelectBaudRateCommandExecuted(object p)
        {
            if (int.TryParse((string)p, out int i))
            {
                SettingsModel.getInstance().BaudRate = (uint)i;
                RaisePropertyChanged("BaudRate");
            }
        }
        #endregion

        #region SelectParityCommand

        public ICommand SelectParityCommand { get; }
        private bool CanSelectParityCommandExecute(object p) => true;
        private void OnSelectParityCommandExecuted(object p)
        {
            SettingsModel.getInstance().Parity = (Parity)Enum.Parse(typeof(Parity), (string)p);
            RaisePropertyChanged("Parity");
        }

        #endregion

        #region SelectStopBitsCommand

        public ICommand SelectStopBitsCommand { get; }
        private bool CanSelectStopBitsCommandExecute(object p) => true;
        private void OnSelectStopBitsCommandExecuted(object p)
        {
            SettingsModel.getInstance().StopBits = (StopBits)Enum.Parse(typeof(StopBits), (string)p);
            RaisePropertyChanged("StopBits");
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
            SelectBaudRateCommand = new LambdaCommand(OnSelectBaudRateCommandExecuted, CanSelectBaudRateCommandExecute);
            SelectParityCommand = new LambdaCommand(OnSelectParityCommandExecuted, CanSelectParityCommandExecute);
            SelectStopBitsCommand = new LambdaCommand(OnSelectStopBitsCommandExecuted, CanSelectStopBitsCommandExecute);



            ConnectDisconnectSerialCommand = new LambdaCommand(OnConnectDisconnectSerialCommandExecuted, CanConnectDisconnectSerialCommandExecute);
        }
    }
}
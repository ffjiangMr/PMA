using Neusoft.Reach.CANComponent.Handle;
using Neusoft.Reach.DBCAnalysis.Handle;
using log4net;
using PMA_Project.Models;
using PMA_Project.Models.Configuration;

using System;
using System.Windows;
using System.Windows.Controls;

namespace PMA_Project.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        #region Logger

        private static ILog Logger = LogManager.GetLogger(typeof(Settings));

        #endregion

        public Settings()
        {
            InitializeComponent();

        }
        public delegate void LoadDbcDelegate();
        public event LoadDbcDelegate LoadDbcEvent;

        //保存通道配置
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DBCHandler dbc;
                WorkSpace.This.VirtualNode.ObjChannelCfgDevCfg = new ChannelCfgDevCfg();
                WorkSpace.This.VirtualNode.ObjChannelCfgDevCfg.DevName = this.cbxDevType.Text;
                WorkSpace.This.VirtualNode.ObjChannelCfgDevCfg.DevIndex = this.cbxDevType.SelectedIndex;
                WorkSpace.This.VirtualNode.ObjChannelCfgDevCfg.DevID = 0;
                WorkSpace.This.VirtualNode.Lst_RcvDBCHandler.Clear();
                if (true == this.chxChannel01.IsChecked)
                {
                    dbc = new DBCHandler(this.tbDBCFilePath01.Text);
                    dbc.DBCChannel.BaudRate = Convert.ToInt32(this.tbxBauRat01.Text);
                    dbc.DBCChannel.ChannelName = "通道1";
                    dbc.LoadDBC();
                    WorkSpace.This.VirtualNode.Lst_RcvDBCHandler.Add(dbc);
                    WorkSpace.This.VirtualNode.ObjChannelCfgDevCfg.DevCha = 0;
                }
                if (true == this.chxChannel02.IsChecked)
                {
                    dbc = new DBCHandler(this.tbDBCFilePath02.Text);
                    dbc.DBCChannel.BaudRate = Convert.ToInt32(this.tbxBauRat02.Text);
                    dbc.DBCChannel.ChannelName = "通道2";
                    dbc.LoadDBC();
                    WorkSpace.This.VirtualNode.Lst_RcvDBCHandler.Add(dbc);
                    WorkSpace.This.VirtualNode.ObjChannelCfgDevCfg.DevCha = 1;
                }
                this.OnRaiseLoadDbcEvent();
                this.Close();
                this.LoadDbcEvent = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void OnRaiseLoadDbcEvent()
        {
            var temp = this.LoadDbcEvent;
            if (temp != null)
            {
                temp();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();// Create OpenFileDialog
            dlg.DefaultExt = ".dbc";// Set filter for file extension and default file extension 
            dlg.Filter = "Text documents (.dbc)|*.dbc";
            Nullable<bool> result = dlg.ShowDialog();// Display OpenFileDialog by calling ShowDialog method  
            if (result == true)// Get the selected file name and display in a TextBox 
            {
                if ("btnOpen01" == ((Button)sender).Name)
                {
                    this.tbDBCFilePath01.Text = dlg.FileName;
                }
                else if ("btnOpen02" == ((Button)sender).Name)
                {
                    this.tbDBCFilePath02.Text = dlg.FileName;
                }
            }
        }

        private void Channel_Checked(object sender, RoutedEventArgs e)
        {
            if (this.chxChannel01 == (CheckBox)sender)
            {
                if (true == this.chxChannel01.IsChecked)
                {
                    this.dpChannel01.IsEnabled = true;
                }
                else { this.dpChannel01.IsEnabled = false; }

            }
            if (this.chxChannel02 == (CheckBox)sender)
            {
                if (true == this.chxChannel02.IsChecked)
                {
                    this.dpChannel02.IsEnabled = true;
                }
                else { this.dpChannel02.IsEnabled = false; }

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.cbxDevType.Items.Clear();
            this.cbxDevType.ItemsSource = CanInteractionHandler.GetSupportedDeviceNames();
            this.cbxDevType.SelectedIndex = WorkSpace.This.VirtualNode.ObjChannelCfgDevCfg.DevIndex;
            foreach (var dbc in WorkSpace.This.VirtualNode.Lst_RcvDBCHandler)
            {
                if (dbc.DBCChannel.ChannelName == "通道1")
                {
                    this.chxChannel01.IsChecked = true;
                    this.tbxBauRat01.Text = dbc.DBCChannel.BaudRate.ToString();
                    this.tbDBCFilePath01.Text = dbc.DBCFilePath;
                }
                else if (dbc.DBCChannel.ChannelName == "通道2")
                {
                    this.chxChannel02.IsChecked = true;
                    this.tbxBauRat02.Text = dbc.DBCChannel.BaudRate.ToString();
                    this.tbDBCFilePath02.Text = dbc.DBCFilePath;
                }
            }
        }

        private void cbxDevType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string devName = cbxDevType.SelectedItem.ToString();
            int x = Neusoft.Reach.CANComponent.Handle.CanInteractionHandler.CANGetDeviceSupportedChannelCount(ref devName);

            if (x == 1)
            {
                this.chxChannel02.IsChecked = false;
                this.chxChannel02.IsEnabled = false;
            }
            else
            {
                this.chxChannel02.IsEnabled = true;
            }
        }
    }
}

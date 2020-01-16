using Neusoft.Reach.CANComponent.Handle;
using Neusoft.Reach.CANComponent.Infrastructure;
using Neusoft.Reach.DBCAnalysis.Handle;
using Neusoft.Reach.DBCAnalysis.Infrastructure;
using Neusoft.Reach.DBCAnalysis.Model;
using log4net;
using PMA_Project.common;
using PMA_Project.Common;
using PMA_Project.Models;
using PMA_Project.Models.Configuration;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static PMA_Project.MainWindow;
using FrameType = Neusoft.Reach.CANComponent.Infrastructure.FrameType;

namespace PMA_Project.View
{
    /// <summary>
    /// Interaction logic for SndDataWin.xaml
    /// </summary>
    /// 
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct sRawData_Big
    {
        [FieldOffset(0)]
        public byte Bit08_RawData07;
        [FieldOffset(1)]
        public byte Bit08_RawData06;
        [FieldOffset(2)]
        public byte Bit08_RawData05;
        [FieldOffset(3)]
        public byte Bit08_RawData04;
        [FieldOffset(4)]
        public byte Bit08_RawData03;
        [FieldOffset(5)]
        public byte Bit08_RawData02;
        [FieldOffset(6)]
        public byte Bit08_RawData01;
        [FieldOffset(7)]
        public byte Bit08_RawData00;
        [FieldOffset(0)]
        public UInt64 Bit64_RawAll;
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct sRawData_Lit
    {
        [FieldOffset(0)]
        public byte Bit08_RawData00;
        [FieldOffset(1)]
        public byte Bit08_RawData01;
        [FieldOffset(2)]
        public byte Bit08_RawData02;
        [FieldOffset(3)]
        public byte Bit08_RawData03;
        [FieldOffset(4)]
        public byte Bit08_RawData04;
        [FieldOffset(5)]
        public byte Bit08_RawData05;
        [FieldOffset(6)]
        public byte Bit08_RawData06;
        [FieldOffset(7)]
        public byte Bit08_RawData07;
        [FieldOffset(0)]
        public UInt64 Bit64_RawAll;
    }

    /// <summary>
    /// Interaction logic for Send.xaml
    /// </summary>
    public partial class Send : Window
    {

        #region Logger

        private static ILog Logger = LogManager.GetLogger(typeof(Send));

        #endregion

        public Send()
        {
            Logger.Info("Func in.");
            InitializeComponent();
            Logger.Info("Func out.");
        }

        private bool isMultiSignal = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            if ((Button)sender == this.btnAdd)
            {
                SendAdd window = SendAdd.Instance();
                window.getMessage = getMessage;
                window.Show();
            }
            else if ((Button)sender == this.btnDel)
            {
                if (dgList.SelectedIndex < 0)
                {
                    MessageBox.Show("请选择删除的报文！");
                    return;
                }
                WorkSpace.This.ObjSendFrameData.LstSendFrameData.RemoveAt(dgList.SelectedIndex);
                if (WorkSpace.This.ObjSendFrameData.LstSendFrameData.Count <= 0) { this.btnDel.IsEnabled = false; }
                int tmpIdx = 0;
                foreach (var MsgNode in WorkSpace.This.ObjSendFrameData.LstSendFrameData)
                {
                    MsgNode.objFrameDataModel.Index = ++tmpIdx;
                }
            }
            Logger.Info("Func out.");
        }

        private void getMessage(DBCHandler dbc, DBCMessage message)
        {
            Logger.Info("Func in.");
            if (message.MessageSize > 8)
            {
                MessageBox.Show("发送数据功能暂时不支持报文长度>8");
            }
            else
            {
                FrameData data = new FrameData();
                data.Id = Convert.ToString(message.MessageID, 16);
                data.Index = this.dgList.Items.Count + 1;
                data.IsEnabled = false;
                data.MsgNam = message.MessageName;
                data.ChannelName = dbc.DBCChannel.ChannelName;
                data.TriggerType = "手动";
                data.DLC = "8";
                data.FrameType = "标准帧";
                SendFramData tmpLstSignalDataModel = new SendFramData();
                tmpLstSignalDataModel.objFrameDataModel = data;
                tmpLstSignalDataModel.LstFrmChannelSel.Add(dbc.DBCChannel.ChannelName);
                rawData(message, tmpLstSignalDataModel);
                signalData(message, tmpLstSignalDataModel);
                WorkSpace.This.ObjSendFrameData.LstSendFrameData.Add(tmpLstSignalDataModel);
                this.btnDel.IsEnabled = true;
            }
            // this.dgList.DataContext = WorkSpace.This.ObjSendFrameData.LstSendFrameData;
            Logger.Info("Func out.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            WorkSpace.This.ObjSendFrameData = new SendFramData();
            WorkSpace.This.IsRunSndThd = true;
            this.dgList.DataContext = WorkSpace.This.ObjSendFrameData.LstSendFrameData;
            Thread thread = new Thread(new ThreadStart(SendThread));
            thread.Start();
            WorkSpace.This.objUltraHighAccurateTimer_SndData.Start();
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            //if (source != null) source.AddHook(WndProc);
            Logger.Info("Func out.");
        }

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Logger.Info("Func in.");
            if (dgList.SelectedIndex >= 0)
            {
                ObservableCollection<SignalData> temp = new ObservableCollection<SignalData>();
                foreach (var s in WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex].LstSignalDataModel)
                {
                    if (s.MultiplexerIndicator != MultiplexerIndicator.NormalSignal && s.MultiplexerSwitchValue == 0)
                    {
                        temp.Add(s);
                        isMultiSignal = true;
                    }
                }
                if (temp.Count <= 0)
                {
                    temp = WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex].LstSignalDataModel;
                    isMultiSignal = false;
                }
                dgSignal.DataContext = temp;
                ObservableCollection<RawData> lsttemp = WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex].LstRawDataModel;
                dgRawData.DataContext = lsttemp;
            }
            Logger.Info("Func out.");
        }

        private void DataGridColumn_Init()
        {
            Logger.Info("Func in.");
            ObservableCollection<String> source = new ObservableCollection<string>();
            source.Add("标准帧");
            source.Add("扩展帧");
            frametypeColumn.ItemsSource = source;

            ObservableCollection<String> triggerSource = new ObservableCollection<string>();
            triggerSource.Add("手动");
            triggerSource.Add("自动");
            triggerTypeColumn.ItemsSource = triggerSource;

            ObservableCollection<String> DLCSource = new ObservableCollection<string>();
            for (int i = 1; i <= 8; i++)
            {
                DLCSource.Add(i.ToString());
            }
            DLCColumn.ItemsSource = DLCSource;

            ObservableCollection<String> ChannelSource = new ObservableCollection<string>();
            ChannelSource.Add("通道1");
            ChannelSource.Add("通道2");
            channelColumn.ItemsSource = ChannelSource;
            Logger.Info("Func out.");
        }

        private void signalData(DBCMessage message, SendFramData SendFramData)
        {
            Logger.Info("Func in.");
            foreach (var signal in message.Signals)
            {
                SignalData tmpSignalDataModel = new SignalData();
                tmpSignalDataModel.Name = signal.SignalName;
                tmpSignalDataModel.nOldRawVal = 0;
                tmpSignalDataModel.RawData = 0;
                tmpSignalDataModel.PhysicalData = (Single)(tmpSignalDataModel.RawData * signal.Factor + signal.Offset);
                tmpSignalDataModel.StrUnit = signal.Unit;
                tmpSignalDataModel.BeginBit = signal.StartPosition;
                tmpSignalDataModel.Length = signal.SignalSize;
                tmpSignalDataModel.fFact = (Single)signal.Factor;
                tmpSignalDataModel.fOffSet = (Single)signal.Offset;
                tmpSignalDataModel.eEndi = signal.ByteOrder;
                tmpSignalDataModel.ValDesc = "";
                tmpSignalDataModel.MultiplexerIndicator = signal.MultiplexerIndicator;
                tmpSignalDataModel.MultiplexerSwitchValue = signal.MultiplexerSwitchValue;
                foreach (var item in signal.Values)
                {
                    if (tmpSignalDataModel.RawData == item.Value)
                    {
                        tmpSignalDataModel.ValDesc = item.Key;
                        break;
                    }
                }
                foreach (var item in signal.Values)
                {
                    tmpSignalDataModel.LstValDesc.Add(item.Key);
                    tmpSignalDataModel.LstSigVal.Add(item.Value);
                }
                SendFramData.LstSignalDataModel.Add(tmpSignalDataModel);
            }
            Logger.Info("Func out.");
        }

        private void rawData(DBCMessage message, SendFramData SendFramData)
        {
            Logger.Info("Func in.");
            RawData objRawDataModel = new RawData();
            objRawDataModel.ZeroByte = "00";
            objRawDataModel.FirstByte = "00";
            objRawDataModel.SecondByte = "00";
            objRawDataModel.ThreeByte = "00";
            objRawDataModel.FourByte = "00";
            objRawDataModel.FiveByte = "00";
            objRawDataModel.SixByte = "00";
            objRawDataModel.SevenByte = "00";
            if (message.MessageSize == 0)
            {
                objRawDataModel.ZeroByte = "-";
                objRawDataModel.FirstByte = "-";
                objRawDataModel.SecondByte = "-";
                objRawDataModel.ThreeByte = "-";
                objRawDataModel.FourByte = "-";
                objRawDataModel.FiveByte = "-";
                objRawDataModel.SixByte = "-";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 1)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "-";
                objRawDataModel.SecondByte = "-";
                objRawDataModel.ThreeByte = "-";
                objRawDataModel.FourByte = "-";
                objRawDataModel.FiveByte = "-";
                objRawDataModel.SixByte = "-";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 2)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "00";
                objRawDataModel.SecondByte = "-";
                objRawDataModel.ThreeByte = "-";
                objRawDataModel.FourByte = "-";
                objRawDataModel.FiveByte = "-";
                objRawDataModel.SixByte = "-";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 3)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "00";
                objRawDataModel.SecondByte = "00";
                objRawDataModel.ThreeByte = "-";
                objRawDataModel.FourByte = "-";
                objRawDataModel.FiveByte = "-";
                objRawDataModel.SixByte = "-";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 4)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "00";
                objRawDataModel.SecondByte = "00";
                objRawDataModel.ThreeByte = "00";
                objRawDataModel.FourByte = "-";
                objRawDataModel.FiveByte = "-";
                objRawDataModel.SixByte = "-";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 5)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "00";
                objRawDataModel.SecondByte = "00";
                objRawDataModel.ThreeByte = "00";
                objRawDataModel.FourByte = "00";
                objRawDataModel.FiveByte = "-";
                objRawDataModel.SixByte = "-";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 6)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "00";
                objRawDataModel.SecondByte = "00";
                objRawDataModel.ThreeByte = "00";
                objRawDataModel.FourByte = "00";
                objRawDataModel.FiveByte = "00";
                objRawDataModel.SixByte = "-";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 7)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "00";
                objRawDataModel.SecondByte = "00";
                objRawDataModel.ThreeByte = "00";
                objRawDataModel.FourByte = "00";
                objRawDataModel.FiveByte = "00";
                objRawDataModel.SixByte = "00";
                objRawDataModel.SevenByte = "-";
            }
            else if (message.MessageSize == 8)
            {
                objRawDataModel.ZeroByte = "00";
                objRawDataModel.FirstByte = "00";
                objRawDataModel.SecondByte = "00";
                objRawDataModel.ThreeByte = "00";
                objRawDataModel.FourByte = "00";
                objRawDataModel.FiveByte = "00";
                objRawDataModel.SixByte = "00";
                objRawDataModel.SevenByte = "00";
            }
            SendFramData.LstRawDataModel.Add(objRawDataModel);
            Logger.Info("Func out.");
        }

        private void dgSignal_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            if (e.Column == this.colRawData)
            {
                SigRawData_LostFocus(sender, e);
            }
            else if (e.Column == this.colPhysical)
            {
                SigPhyData_LostFocus(sender, e);
            }
            Logger.Info("Func out.");
        }
        private void dgRawData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            TextBox_TextChanged_RawData(sender, e);
            TextBox_LostFocus_RawData(sender, e);
            Logger.Info("Func out.");
        }

        private void SigRawData_LostFocus(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            var objTextBox = e.EditingElement as TextBox;
            var c = objTextBox.DataContext as SignalData;

            if (objTextBox.Text == "")
            {
                c.RawData = c.nOldRawVal;
            }
            else
            {
                //判断输入值是否超出信号的位长度
                UInt64 lnRawMask = 0;
                UInt64 tmpRawdata;
                for (UInt32 i = 0; i < c.Length; i++)
                {
                    lnRawMask = (lnRawMask << 1) | 1;
                }
                if ((c.RawData & (~lnRawMask)) > 0)
                {
                    MessageBox.Show("输入值超出信号的位长度！");
                    tmpRawdata = (c.RawData & lnRawMask);
                    c.RawData = (UInt32)tmpRawdata;
                    c.PhysicalData = c.RawData * c.fFact + c.fOffSet;
                    c.nOldRawVal = c.RawData;
                }
                else
                {
                    c.PhysicalData = c.RawData * c.fFact + c.fOffSet;
                    c.nOldRawVal = c.RawData;
                    tmpRawdata = c.RawData;
                }
                if (c.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignal)
                {
                    ObservableCollection<SignalData> temp = new ObservableCollection<SignalData>();
                    foreach (var s in WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex].LstSignalDataModel)
                    {
                        if (s.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignals && s.MultiplexerSwitchValue == c.RawData)
                        {
                            temp.Add(s);
                        }
                    }
                    temp.Add(c);
                    dgSignal.DataContext = null;
                    dgSignal.DataContext = temp;
                }
                signalToRawData(c, lnRawMask, tmpRawdata);
            }
            Logger.Info("Func out.");
        }
        private void SigPhyData_LostFocus(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            var objTextBox = e.EditingElement as TextBox;

            string TextBox_Name = objTextBox.Name;
            var c = objTextBox.DataContext as SignalData;
            if (objTextBox.Text == "")
            {
                c.PhysicalData = c.RawData * c.fFact + c.fOffSet;
                c.nOldRawVal = c.RawData;
            }
            else
            {
                if (c.fFact == 0)
                {
                    MessageBox.Show("此信号的比例因子=0，请检查dbc文件！");
                }
                else
                {
                    double temp = Math.Round((c.PhysicalData - c.fOffSet) / c.fFact, 3);

                    c.RawData = (UInt32)(temp);
                    c.nOldRawVal = c.RawData;

                    UInt64 lnRawMask = 0;
                    UInt64 tmpRawdata;
                    for (UInt32 i = 0; i < c.Length; i++)
                    {
                        lnRawMask = (lnRawMask << 1) | 1;
                    }
                    if ((c.RawData & (~lnRawMask)) > 0)
                    {
                        MessageBox.Show("输入值超出信号的位长度！");
                        tmpRawdata = (c.RawData & lnRawMask);
                        c.RawData = (UInt32)tmpRawdata;
                        c.PhysicalData = c.RawData * c.fFact + c.fOffSet;
                        c.nOldRawVal = c.RawData;
                    }
                    else
                    {
                        c.PhysicalData = c.RawData * c.fFact + c.fOffSet;
                        c.nOldRawVal = c.RawData;
                        tmpRawdata = c.RawData;

                        string strResult = null;
                        List<UInt32> valueList = new List<UInt32>();
                        valueList = c.LstSigVal;
                        for (int i = 0; i < c.LstSigVal.Count; i++)
                        {
                            if (c.LstSigVal[i] == c.RawData)
                            {
                                strResult = c.LstValDesc[i];
                                break;
                            }
                        }
                        c.ValDesc = strResult;
                    }
                    if (c.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignal)
                    {
                        ObservableCollection<SignalData> sTemp = new ObservableCollection<SignalData>();
                        foreach (var s in WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex].LstSignalDataModel)
                        {
                            if (s.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignals && s.MultiplexerSwitchValue == c.RawData)
                            {
                                sTemp.Add(s);
                            }
                        }
                        sTemp.Add(c);
                        dgSignal.DataContext = null;
                        dgSignal.DataContext = sTemp;
                    }
                    signalToRawData(c, lnRawMask, tmpRawdata);
                }
            }
            Logger.Info("Func out.");
        }
        /// <summary>
        /// 判断输入是否合法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private unsafe void TextBox_TextChanged_RawData(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            var objTextBox = e.EditingElement as TextBox;
            string TextBox_Name = objTextBox.Name;
            var c = objTextBox.DataContext as RawData;
            if (objTextBox.Text == "")
                return;
            if (e.Column == this.colRawData0) { ReCalculateRawData(objTextBox, 0, c); }
            else if (e.Column == this.colRawData1) { ReCalculateRawData(objTextBox, 1, c); }
            else if (e.Column == this.colRawData2) { ReCalculateRawData(objTextBox, 2, c); }
            else if (e.Column == this.colRawData3) { ReCalculateRawData(objTextBox, 3, c); }
            else if (e.Column == this.colRawData4) { ReCalculateRawData(objTextBox, 4, c); }
            else if (e.Column == this.colRawData5) { ReCalculateRawData(objTextBox, 5, c); }
            else if (e.Column == this.colRawData6) { ReCalculateRawData(objTextBox, 6, c); }
            else if (e.Column == this.colRawData7) { ReCalculateRawData(objTextBox, 7, c); }
            Logger.Info("Func out.");
        }
        private void TextBox_LostFocus_RawData(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            var objTextBox = e.EditingElement as TextBox;
            var c = objTextBox.DataContext as RawData;
            if (e.Column == this.colRawData0)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.ZeroByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.ZeroByte = "00";
                }
            }
            else if (e.Column == this.colRawData1)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.FirstByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.FirstByte = "00";
                }
            }
            else if (e.Column == this.colRawData2)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.SecondByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.SecondByte = "00";
                }
            }
            else if (e.Column == this.colRawData3)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.ThreeByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.ThreeByte = "00";
                }
            }
            else if (e.Column == this.colRawData4)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.FourByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.FourByte = "00";
                }
            }
            else if (e.Column == this.colRawData5)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.FiveByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.FiveByte = "00";
                }
            }
            else if (e.Column == this.colRawData6)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.SixByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.SixByte = "00";
                }
            }
            else if (e.Column == this.colRawData7)
            {
                if (objTextBox.Text.Count() == 1)
                {
                    c.SevenByte = "0" + objTextBox.Text;
                }
                else if (objTextBox.Text.Count() == 0)
                {
                    c.SevenByte = "00";
                }
            }

            try
            {
                SendFramData MsgLin = WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex];
                MsgLin.objFrameDataModel.RawData[0] = Convert.ToByte(c.ZeroByte.Contains("-") ? "0" : c.ZeroByte, 16);
                MsgLin.objFrameDataModel.RawData[1] = Convert.ToByte(c.FirstByte.Contains("-") ? "0" : c.FirstByte, 16);
                MsgLin.objFrameDataModel.RawData[2] = Convert.ToByte(c.SecondByte.Contains("-") ? "0" : c.SecondByte, 16);
                MsgLin.objFrameDataModel.RawData[3] = Convert.ToByte(c.ThreeByte.Contains("-") ? "0" : c.ThreeByte, 16);
                MsgLin.objFrameDataModel.RawData[4] = Convert.ToByte(c.FourByte.Contains("-") ? "0" : c.FourByte, 16);
                MsgLin.objFrameDataModel.RawData[5] = Convert.ToByte(c.FiveByte.Contains("-") ? "0" : c.FiveByte, 16);
                MsgLin.objFrameDataModel.RawData[6] = Convert.ToByte(c.SixByte.Contains("-") ? "0" : c.SixByte, 16);
                MsgLin.objFrameDataModel.RawData[7] = Convert.ToByte(c.SevenByte.Contains("-") ? "0" : c.SevenByte, 16);
                foreach (var tmpsig in MsgLin.LstSignalDataModel)
                {
                    if (tmpsig.eEndi == ByteOrder.Motorola)
                    {
                        sRawData_Big tmpRawData = new sRawData_Big();
                        tmpRawData.Bit08_RawData00 = MsgLin.objFrameDataModel.RawData[0];
                        tmpRawData.Bit08_RawData01 = MsgLin.objFrameDataModel.RawData[1];
                        tmpRawData.Bit08_RawData02 = MsgLin.objFrameDataModel.RawData[2];
                        tmpRawData.Bit08_RawData03 = MsgLin.objFrameDataModel.RawData[3];
                        tmpRawData.Bit08_RawData04 = MsgLin.objFrameDataModel.RawData[4];
                        tmpRawData.Bit08_RawData05 = MsgLin.objFrameDataModel.RawData[5];
                        tmpRawData.Bit08_RawData06 = MsgLin.objFrameDataModel.RawData[6];
                        tmpRawData.Bit08_RawData07 = MsgLin.objFrameDataModel.RawData[7];

                        UInt32 usStart;
                        usStart = 63 - (7 - tmpsig.BeginBit % 8) - tmpsig.BeginBit / 8 * 8;

                        UInt64 Temp64 = 0;
                        Temp64 = tmpRawData.Bit64_RawAll << (Int32)(63 - usStart);
                        Temp64 = Temp64 >> (Int32)(63 - tmpsig.Length + 1);
                        tmpsig.RawData = (UInt32)Temp64;
                        tmpsig.PhysicalData = tmpsig.fFact * tmpsig.RawData + tmpsig.fOffSet;
                        string strResult = null;
                        for (int i = 0; i < tmpsig.LstSigVal.Count; i++)
                        {
                            if (tmpsig.LstSigVal[i] == tmpsig.RawData)
                            {
                                strResult = tmpsig.LstValDesc[i];
                                break;
                            }
                        }
                        tmpsig.ValDesc = strResult;
                    }
                    else if (tmpsig.eEndi == ByteOrder.Intel)
                    {
                        sRawData_Lit tmpRawData = new sRawData_Lit();
                        tmpRawData.Bit08_RawData00 = MsgLin.objFrameDataModel.RawData[0];
                        tmpRawData.Bit08_RawData01 = MsgLin.objFrameDataModel.RawData[1];
                        tmpRawData.Bit08_RawData02 = MsgLin.objFrameDataModel.RawData[2];
                        tmpRawData.Bit08_RawData03 = MsgLin.objFrameDataModel.RawData[3];
                        tmpRawData.Bit08_RawData04 = MsgLin.objFrameDataModel.RawData[4];
                        tmpRawData.Bit08_RawData05 = MsgLin.objFrameDataModel.RawData[5];
                        tmpRawData.Bit08_RawData06 = MsgLin.objFrameDataModel.RawData[6];
                        tmpRawData.Bit08_RawData07 = MsgLin.objFrameDataModel.RawData[7];
                        UInt32 usStart;
                        usStart = tmpsig.BeginBit;
                        UInt64 Temp64 = 0;
                        Temp64 = tmpRawData.Bit64_RawAll << (Int32)(63 - usStart - tmpsig.Length + 1);
                        Temp64 = Temp64 >> (Int32)(63 - tmpsig.Length + 1);
                        tmpsig.RawData = (UInt32)Temp64;
                        tmpsig.PhysicalData = tmpsig.fFact * tmpsig.RawData + tmpsig.fOffSet;
                    }
                }
                if (isMultiSignal)
                {
                    ObservableCollection<SignalData> temp = new ObservableCollection<SignalData>();
                    SignalData M = new SignalData();
                    foreach (var s in WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex].LstSignalDataModel)
                    {
                        if (s.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignal)
                        {
                            M = s;
                            break;
                        }
                    }
                    foreach (var s in WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex].LstSignalDataModel)
                    {
                        if (s.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignals && s.MultiplexerSwitchValue == M.RawData)
                        {
                            temp.Add(s);
                        }
                    }
                    temp.Add(M);
                    dgSignal.DataContext = null;
                    dgSignal.DataContext = temp;
                }
            }
            catch
            {
                MessageBox.Show("输入异常字符，请重新输入！");
            }
            Logger.Info("Func out.");
        }
        /// <summary>
        /// 如果输出超出2位，仅读前两位
        /// </summary>
        /// <param name="objTextBox"></param>
        /// <param name="index"></param>
        /// <param name="c"></param>
        private void ReCalculateRawData(TextBox objTextBox, int index, RawData c)
        {
            Logger.Info("Func in.");
            SendFramData MsgNode = WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex];
            if (objTextBox.Text.Count() > 2)
            {
                switch (index)
                {
                    case 0: c.ZeroByte = objTextBox.Text.Substring(0, 2); break;
                    case 1: c.FirstByte = objTextBox.Text.Substring(0, 2); break;
                    case 2: c.SecondByte = objTextBox.Text.Substring(0, 2); break;
                    case 3: c.ThreeByte = objTextBox.Text.Substring(0, 2); break;
                    case 4: c.FourByte = objTextBox.Text.Substring(0, 2); break;
                    case 5: c.FiveByte = objTextBox.Text.Substring(0, 2); break;
                    case 6: c.SixByte = objTextBox.Text.Substring(0, 2); break;
                    case 7: c.SevenByte = objTextBox.Text.Substring(0, 2); break;
                }
            }
            try
            {
                MsgNode.objFrameDataModel.RawData[index] = Convert.ToByte(objTextBox.Text, 16);
            }
            catch
            {
                MsgNode.objFrameDataModel.RawData[index] = 0;
            }
            Logger.Info("Func out.");
        }
        private void RawToPhysical(string[] values, SendFramData MsgLin)
        {
            Logger.Info("Func in.");
            MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = values[0];
            MsgLin.LstRawDataModel.ElementAt(0).FirstByte = values[1];
            MsgLin.LstRawDataModel.ElementAt(0).SecondByte = values[2];
            MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = values[3];
            MsgLin.LstRawDataModel.ElementAt(0).FourByte = values[4];
            MsgLin.LstRawDataModel.ElementAt(0).FiveByte = values[5];
            MsgLin.LstRawDataModel.ElementAt(0).SixByte = values[6];
            MsgLin.LstRawDataModel.ElementAt(0).SevenByte = values[7];
            Logger.Info("Func out.");
        }

        private void dgList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            var column = e.Column;
            if (column == this.triggerTypeColumn) { TriTyp_DropDownClosed(sender, e); }
            Logger.Info("Func out.");
        }
        /// <summary>
        /// 列表中【触发】事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TriTyp_DropDownClosed(object sender, DataGridCellEditEndingEventArgs e)
        {
            Logger.Info("Func in.");
            var btn = e.EditingElement as ComboBox;
            var c = btn.DataContext as SendFramData;
            lock (WorkSpace.This.SendDataCycleLock)
            {
                if (c.objFrameDataModel.TriggerType == "手动")
                {
                    c.RecordCurTim = 0;
                    c.IsCycSndData = false;
                    c.objFrameDataModel.SndNam = "发送";
                    c.objFrameDataModel.IsEnabled = false;
                }
                else if (c.objFrameDataModel.TriggerType == "周期")
                {
                    c.RecordCurTim = 0;
                    c.IsCycSndData = true;
                    c.objFrameDataModel.IsEnabled = true;
                }
            }
            Logger.Info("Func out.");
        }

        /// <summary>
        /// 执行发送
        /// </summary>
        private void SendThread()
        {
            Logger.Info("Func in.");
            while (WorkSpace.This.IsRunSndThd)
            {
                if (false == WorkSpace.This.bSysSts_StartStop)
                {
                    Thread.Sleep(1);
                    continue;
                }

                UInt64 nDifTim = (UInt64)WorkSpace.This.objUltraHighAccurateTimer_SndData.GetDifTimer_us();

                foreach (var MsgLin in WorkSpace.This.ObjSendFrameData.LstSendFrameData)
                {
                    MsgLin.RecordCurTim += nDifTim; ;
                    if ((MsgLin.IsCycSndData == false) || (MsgLin.objFrameDataModel.SndNam == "发送"))
                        continue;
                    if ((MsgLin.RecordCurTim + 1000) > ((Convert.ToUInt64(MsgLin.objFrameDataModel.CycleTime)) * 1000))
                    {
                        MsgLin.RecordCurTim = 0;

                        int nChanNum = 0;
                        foreach (var tmpChanNam in MsgLin.LstFrmChannelSel)
                        {
                            if (tmpChanNam == MsgLin.objFrameDataModel.ChannelName)
                            {

                                Neusoft.Reach.CANComponent.Infrastructure.SendObject mTmp = new Neusoft.Reach.CANComponent.Infrastructure.SendObject();
                                if (MsgLin.objFrameDataModel.FrameType == "标准帧")
                                    mTmp.FrameType = Neusoft.Reach.CANComponent.Infrastructure.FrameType.StandardFrame;
                                else if (MsgLin.objFrameDataModel.FrameType == "扩展帧")
                                    mTmp.FrameType = Neusoft.Reach.CANComponent.Infrastructure.FrameType.ExtendFrame;

                                mTmp.SendID = Convert.ToUInt32(MsgLin.objFrameDataModel.Id, 16);
                                mTmp.Length = Convert.ToUInt16(MsgLin.objFrameDataModel.DLC);
                                mTmp.SendContent = new byte[8];

                                for (int i = 0; i < mTmp.Length; i++)
                                {
                                    mTmp.SendContent[i] = MsgLin.objFrameDataModel.RawData[i];
                                }

                                for (int i = mTmp.Length; i < 8; i++)
                                {
                                    mTmp.SendContent[i] = 0;
                                }
                                Logger.Info("Send message, message id : " + mTmp.SendID);
                                Logger.Debug("Message length : " + mTmp.Length);
                                Logger.Debug("Message frame type : " + mTmp.FrameType);
                                Logger.Debug("Message data[0] : " + mTmp.SendContent[0]);
                                Logger.Debug("Message data[1] : " + mTmp.SendContent[1]);
                                Logger.Debug("Message data[2] : " + mTmp.SendContent[2]);
                                Logger.Debug("Message data[3] : " + mTmp.SendContent[3]);
                                Logger.Debug("Message data[4] : " + mTmp.SendContent[4]);
                                Logger.Debug("Message data[5] : " + mTmp.SendContent[5]);
                                Logger.Debug("Message data[6] : " + mTmp.SendContent[6]);
                                Logger.Debug("Message data[7] : " + mTmp.SendContent[7]);
                                var sendResult = CanInteractionHandler.SendChannelMessage(ref nChanNum, ref mTmp);
                                String alertMessage = String.Empty;
                                if (sendResult != CANError.ErrNon)
                                {
                                    alertMessage = "Send message fault, detail : " + sendResult;
                                    Logger.Error(alertMessage);
                                }                                
                                var dispatch = new UpdateWindowDelegate(() =>
                                  {
                                      this.sendErrorMessage.Text = alertMessage;
                                  });
                                this.Dispatcher.Invoke(dispatch, null);
                                break;
                            }
                            nChanNum++;
                        }
                    }
                }
                WorkSpace.This.objUltraHighAccurateTimer_SndData.startTimer();
                Thread.Sleep(10);
            }
            Logger.Info("Func out.");
        }

        /// <summary>
        /// 列表中的【发送】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            if (false == WorkSpace.This.bSysSts_StartStop)
            {
                MessageBox.Show("系统未启动，请先启动系统");
            }
            else
            {
                var btn = sender as TextBlock;
                var c = btn.DataContext as SendFramData;
                if (c.objFrameDataModel.TriggerType == "手动")
                {
                    int nChanNum = 0;
                    foreach (var tmpChanNam in c.LstFrmChannelSel)
                    {
                        if (tmpChanNam == c.objFrameDataModel.ChannelName)
                        {
                            SendObject mTmp = new SendObject();
                            if (c.objFrameDataModel.FrameType == "标准帧")
                                mTmp.FrameType = FrameType.StandardFrame;
                            else if (c.objFrameDataModel.FrameType == "扩展帧")
                                mTmp.FrameType = FrameType.ExtendFrame;
                            mTmp.SendID = Convert.ToUInt32(c.objFrameDataModel.Id, 16);
                            mTmp.Length = Convert.ToUInt16(c.objFrameDataModel.DLC);
                            mTmp.SendContent = new byte[8];

                            for (int i = 0; i < mTmp.Length; i++)
                            {
                                mTmp.SendContent[i] = c.objFrameDataModel.RawData[i];
                            }

                            for (int i = mTmp.Length; i < 8; i++)
                            {
                                mTmp.SendContent[i] = 0;
                            }
                            Logger.Info("Send message, message id : " + mTmp.SendID);
                            Logger.Debug("Message length : " + mTmp.Length);
                            Logger.Debug("Message frame type : " + mTmp.FrameType);                            
                            Logger.Debug("Message data[0] : " + mTmp.SendContent[0]);
                            Logger.Debug("Message data[1] : " + mTmp.SendContent[1]);
                            Logger.Debug("Message data[2] : " + mTmp.SendContent[2]);
                            Logger.Debug("Message data[3] : " + mTmp.SendContent[3]);
                            Logger.Debug("Message data[4] : " + mTmp.SendContent[4]);
                            Logger.Debug("Message data[5] : " + mTmp.SendContent[5]);
                            Logger.Debug("Message data[6] : " + mTmp.SendContent[6]);
                            Logger.Debug("Message data[7] : " + mTmp.SendContent[7]);

                            var sendResult = CanInteractionHandler.SendChannelMessage(ref nChanNum, ref mTmp);
                            String alertMessage = String.Empty;
                            if (sendResult != CANError.ErrNon)
                            {
                                alertMessage = "Send message fault, detail : " + sendResult;
                                Logger.Error(alertMessage);
                            }
                            var dispatch = new UpdateWindowDelegate(() =>
                            {
                                this.sendErrorMessage.Text = alertMessage;
                            });
                            this.Dispatcher.Invoke(dispatch, null);
                            break;
                        }
                        nChanNum++;
                    }
                }
                else if (c.objFrameDataModel.TriggerType == "周期")
                {
                    lock (WorkSpace.This.SendDataCycleLock)
                    {
                        if (c.objFrameDataModel.SndNam == "停止")
                        {
                            c.RecordCurTim = 0;
                            c.IsCycSndData = false;
                            c.objFrameDataModel.SndNam = "发送";
                        }
                        else if (c.objFrameDataModel.SndNam == "发送")
                        {
                            c.RecordCurTim = 0;
                            c.IsCycSndData = true;
                            c.objFrameDataModel.SndNam = "停止";
                        }

                    }
                }
            }
            Logger.Info("Func out.");
        }
        /// <summary>
        /// 修改signalData后，更改其RawData
        /// </summary>
        /// <param name="c"></param>
        /// <param name="lnRawMask"></param>
        /// <param name="tmpRawdata"></param>
        private void signalToRawData(SignalData c, UInt64 lnRawMask, UInt64 tmpRawdata)
        {
            Logger.Info("Func in.");
            SendFramData MsgLin = WorkSpace.This.ObjSendFrameData.LstSendFrameData[this.dgList.SelectedIndex];
            if (c.eEndi == ByteOrder.Motorola)
            {
                sRawData_Big tmpRawData = new sRawData_Big();
                tmpRawData.Bit08_RawData00 = MsgLin.objFrameDataModel.RawData[0];
                tmpRawData.Bit08_RawData01 = MsgLin.objFrameDataModel.RawData[1];
                tmpRawData.Bit08_RawData02 = MsgLin.objFrameDataModel.RawData[2];
                tmpRawData.Bit08_RawData03 = MsgLin.objFrameDataModel.RawData[3];
                tmpRawData.Bit08_RawData04 = MsgLin.objFrameDataModel.RawData[4];
                tmpRawData.Bit08_RawData05 = MsgLin.objFrameDataModel.RawData[5];
                tmpRawData.Bit08_RawData06 = MsgLin.objFrameDataModel.RawData[6];
                tmpRawData.Bit08_RawData07 = MsgLin.objFrameDataModel.RawData[7];

                UInt64 RawAll = tmpRawData.Bit64_RawAll;

                UInt32 usStart;
                usStart = 63 - (7 - c.BeginBit % 8) - c.BeginBit / 8 * 8;

                UInt64 lnRawMask_64 = lnRawMask << (Int32)(usStart - c.Length + 1);
                UInt64 lnRawData_64 = tmpRawdata << (Int32)(usStart - c.Length + 1);
                lnRawMask_64 = ~lnRawMask_64;
                RawAll = (RawAll & lnRawMask_64) | lnRawData_64;
                tmpRawData.Bit64_RawAll = RawAll;
                MsgLin.objFrameDataModel.RawData[0] = tmpRawData.Bit08_RawData00;
                MsgLin.objFrameDataModel.RawData[1] = tmpRawData.Bit08_RawData01;
                MsgLin.objFrameDataModel.RawData[2] = tmpRawData.Bit08_RawData02;
                MsgLin.objFrameDataModel.RawData[3] = tmpRawData.Bit08_RawData03;
                MsgLin.objFrameDataModel.RawData[4] = tmpRawData.Bit08_RawData04;
                MsgLin.objFrameDataModel.RawData[5] = tmpRawData.Bit08_RawData05;
                MsgLin.objFrameDataModel.RawData[6] = tmpRawData.Bit08_RawData06;
                MsgLin.objFrameDataModel.RawData[7] = tmpRawData.Bit08_RawData07;
                Int16 tmpDlc;
                Int16.TryParse(MsgLin.objFrameDataModel.DLC, out tmpDlc);

                #region tmpDlc
                if (tmpDlc == 0)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 1)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 2)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData01);
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 3)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData01);
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData02);
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 4)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData01);
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData02);
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData03);
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 5)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData01);
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData02);
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData03);
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData04);
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 6)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData01);
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData02);
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData03);
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData04);
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData05);
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = "-";
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 7)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData01);
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData02);
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData03);
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData04);
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData05);
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData06);
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = "-";
                }
                else if (tmpDlc == 8)
                {
                    MsgLin.LstRawDataModel.ElementAt(0).ZeroByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData00);
                    MsgLin.LstRawDataModel.ElementAt(0).FirstByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData01);
                    MsgLin.LstRawDataModel.ElementAt(0).SecondByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData02);
                    MsgLin.LstRawDataModel.ElementAt(0).ThreeByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData03);
                    MsgLin.LstRawDataModel.ElementAt(0).FourByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData04);
                    MsgLin.LstRawDataModel.ElementAt(0).FiveByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData05);
                    MsgLin.LstRawDataModel.ElementAt(0).SixByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData06);
                    MsgLin.LstRawDataModel.ElementAt(0).SevenByte = string.Format("{0:X2}", tmpRawData.Bit08_RawData07);
                }
                #endregion
            }
            else if (c.eEndi == ByteOrder.Intel)
            {
                sRawData_Lit tmpRawData = new sRawData_Lit();
                tmpRawData.Bit08_RawData00 = MsgLin.objFrameDataModel.RawData[0];
                tmpRawData.Bit08_RawData01 = MsgLin.objFrameDataModel.RawData[1];
                tmpRawData.Bit08_RawData02 = MsgLin.objFrameDataModel.RawData[2];
                tmpRawData.Bit08_RawData03 = MsgLin.objFrameDataModel.RawData[3];
                tmpRawData.Bit08_RawData04 = MsgLin.objFrameDataModel.RawData[4];
                tmpRawData.Bit08_RawData05 = MsgLin.objFrameDataModel.RawData[5];
                tmpRawData.Bit08_RawData06 = MsgLin.objFrameDataModel.RawData[6];
                tmpRawData.Bit08_RawData07 = MsgLin.objFrameDataModel.RawData[7];

                UInt64 RawAll = tmpRawData.Bit64_RawAll;

                UInt32 usStart;
                usStart = c.BeginBit;

                UInt64 lnRawMask_64 = lnRawMask << (Int32)usStart;
                UInt64 lnRawData_64 = tmpRawdata << (Int32)usStart;
                lnRawMask_64 = ~lnRawMask_64;
                RawAll = (RawAll & lnRawMask_64) | lnRawData_64;
                tmpRawData.Bit64_RawAll = RawAll;
                MsgLin.objFrameDataModel.RawData[0] = tmpRawData.Bit08_RawData00;
                MsgLin.objFrameDataModel.RawData[1] = tmpRawData.Bit08_RawData01;
                MsgLin.objFrameDataModel.RawData[2] = tmpRawData.Bit08_RawData02;
                MsgLin.objFrameDataModel.RawData[3] = tmpRawData.Bit08_RawData03;
                MsgLin.objFrameDataModel.RawData[4] = tmpRawData.Bit08_RawData04;
                MsgLin.objFrameDataModel.RawData[5] = tmpRawData.Bit08_RawData05;
                MsgLin.objFrameDataModel.RawData[6] = tmpRawData.Bit08_RawData06;
                MsgLin.objFrameDataModel.RawData[7] = tmpRawData.Bit08_RawData07;
                Int16 tmpDlc = Convert.ToInt16(MsgLin.objFrameDataModel.DLC);
                switch (tmpDlc)
                {
                    case 0:
                        RawToPhysical(new String[] {
                                    "-", "-", "-", "-", "-", "-", "-", "-" }, MsgLin); break;
                    case 1:
                        RawToPhysical(new String[] {
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    "-", "-", "-", "-", "-", "-", "-" }, MsgLin); break;
                    case 2:
                        RawToPhysical(new String[] {
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData01),
                                    "-", "-", "-", "-", "-", "-" }, MsgLin); break;
                    case 3:
                        RawToPhysical(new String[] {
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData01),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData02),
                                    "-", "-", "-", "-", "-" }, MsgLin); break;
                    case 4:
                        RawToPhysical(new String[] {
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData01),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData02),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData03),
                                     "-", "-", "-", "-" }, MsgLin); break;
                    case 5:
                        RawToPhysical(new String[] {
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData01),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData02),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData03),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData04),
                                    "-", "-", "-" }, MsgLin); break;
                    case 6:
                        RawToPhysical(new String[] {
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData01),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData02),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData03),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData04),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData05),
                                    "-", "-" }, MsgLin); break;
                    case 7:
                        RawToPhysical(new String[] {
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData01),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData02),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData03),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData04),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData05),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData06),
                                    "-" }, MsgLin); break;
                    case 8:
                        RawToPhysical(new String[] {
                                   string.Format("{0:X2}", tmpRawData.Bit08_RawData00),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData01),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData02),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData03),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData04),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData05),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData06),
                                    string.Format("{0:X2}", tmpRawData.Bit08_RawData07),
                                    }, MsgLin); break;
                }
            }
            Logger.Info("Func out.");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.Info("Func in.");
            WorkSpace.This.IsRunSndThd = false;
            WorkSpace.This.objUltraHighAccurateTimer_SndData.Stop();
            Logger.Info("Func out.");
        }
    }


}

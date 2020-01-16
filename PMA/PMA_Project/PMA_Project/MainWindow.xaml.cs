
#region using directive

using Neusoft.Reach.CANComponent.Handle;
using Neusoft.Reach.CANComponent.Infrastructure;
using Neusoft.Reach.CANComponent.Model;
using Neusoft.Reach.DBCAnalysis.Infrastructure;
using Neusoft.Reach.DBCAnalysis.Model;
using Neusoft.Reach.SaveComponent.Handler;
using Neusoft.Reach.SaveComponent.Infrastructure;

using log4net;

using PMA_Project.Models;
using PMA_Project.View;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using MessageBox = System.Windows.MessageBox;

#endregion

namespace PMA_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Logger

        private static ILog Logger = LogManager.GetLogger(typeof(MainWindow));

        #endregion

        #region private field
        private static int TempStartScale = -50;

        private Path MaxTempPath;
        private Path MinTempPath;
        private DBHandler dbHandler;
        private FileHandler fileHandler;
        private WorkSpace ObjWorkspace;
        private double old_INHW_Ve_CellMaxVolt = 0;
        private double old_INHW_Ve_CellMinVolt = 0;
        private double old_INHW_Ve_BattCurr = 0;
        private double old_INHW_Ve_BattPatVolt = 0;

        #endregion

        #region Receive message thread

        private Boolean isReceiveMessageThreadStarted = false;
        private Thread receiveMessageThread;
        public delegate void UpdateWindowDelegate();

        #endregion

        public MainWindow()
        {
            Logger.Info(Environment.NewLine + "Func in.");
            try
            {
                InitializeComponent();
                this.InitInstruments();
                //this.dbHandler = DBHandler.Instance();
                this.fileHandler = FileHandler.Instance();
                ObjWorkspace = WorkSpace.This;
                ObjWorkspace.MainWindow = this;
                this.DataContext = this.ObjWorkspace;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        /// <summary>
        /// 仪表初始化
        /// </summary>
        private void InitInstruments()
        {
            Logger.Info("Func in.");
            try
            {
                drawGaugeScale(0, 0.5M, this.gaugeCanvas);
                drawGaugeScale(0, 0.5M, this.gaugeCanvas00);
                drawGaugeScale(-500, 100M, this.gaugeCanvas01);
                drawGaugeScale(0, 60M, this.gaugeCanvas02);
                DrawThermometer(Colors.Black, true, 16, TempStartScale, this.canvasMaxTemp);
                DrawThermometer(Colors.Black, true, 16, TempStartScale, this.canvasMinTemp);
                MaxTempPath = DrawThermometer(Colors.White, false, 16, TempStartScale, this.canvasMaxTemp);
                MinTempPath = DrawThermometer(Colors.White, false, 16, TempStartScale, this.canvasMinTemp);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        /// <summary>
        /// 图形绘制
        /// 仪表值
        /// </summary>
        /// <param name="oldValue">表针移动前数值</param>
        /// <param name="NewValue">表针移动后数值</param>
        /// <param name="MaxScale">表盘最大刻度值,有负数取绝对值（如：-500至500刻度，则赋值1000）</param>
        /// <param name="path">移动的指针名</param>
        /// <param name="text">显示指针值的文本框</param>
        private void setAngelGauge(double oldValue, double NewValue, double MaxScale, Path path)
        {
            Logger.Info("Func in.");
            if (NewValue > MaxScale) NewValue = MaxScale;
            try
            {
                RotateTransform rt = new RotateTransform();
                rt.CenterX = 100;
                rt.CenterY = 100;
                int angelOld = Convert.ToInt32(oldValue / MaxScale * 180);
                int angelNew = Convert.ToInt32(NewValue / MaxScale * 180);
                path.RenderTransform = rt;
                double timeAnimation = Math.Abs(angelOld - angelNew) * 0.001;
                DoubleAnimation da = new DoubleAnimation(angelOld, angelNew, new Duration(TimeSpan.FromMilliseconds(timeAnimation)));
                da.AccelerationRatio = 1;
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        /// <summary>
        /// 绘制仪表盘和刻度，表盘共10个刻度
        /// </summary>
        /// <param name="startValue">仪表最小刻度</param>
        /// <param name="diff">仪表刻度间距</param>
        /// <param name="canvas">面板名称</param>
        private void drawGaugeScale(decimal startValue, decimal diff, Canvas canvas)
        {
            Logger.Info("Func in.");
            try
            {
                decimal count = startValue;
                int offsetL = 92;
                int offsetR = 88;
                if (startValue < 0)
                {
                    offsetL = 90;
                    offsetR = 88;
                }
                for (int i = 0; i <= 180; i += 18 / 5)
                {
                    //添加刻度线
                    Line lineScale = new Line();

                    if (i % 18 == 0)
                    {
                        lineScale.X1 = 100 - 80 * Math.Cos(i * Math.PI / 180);
                        lineScale.Y1 = 100 - 80 * Math.Sin(i * Math.PI / 180);
                        lineScale.Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0));
                        lineScale.StrokeThickness = 2;

                        //添加刻度值
                        TextBlock txtScale = new TextBlock();
                        txtScale.Text = (count).ToString();
                        count += diff;
                        txtScale.FontSize = 10;
                        if (i < 90)//对坐标值进行一定的修正
                        {
                            Canvas.SetLeft(txtScale, offsetL - 77.5 * Math.Cos(i * Math.PI / 180));
                        }
                        else if (i == 90)
                        {
                            Canvas.SetLeft(txtScale, 93 - 77.5 * Math.Cos(i * Math.PI / 180));
                        }
                        else
                        {
                            Canvas.SetLeft(txtScale, offsetR - 77.5 * Math.Cos(i * Math.PI / 180));
                        }
                        Canvas.SetTop(txtScale, 100 - 77.5 * Math.Sin(i * Math.PI / 180));
                        canvas.Children.Add(txtScale);
                    }
                    else
                    {
                        lineScale.X1 = 100 - 85 * Math.Cos(i * Math.PI / 180);
                        lineScale.Y1 = 100 - 85 * Math.Sin(i * Math.PI / 180);
                        lineScale.Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0));
                        lineScale.StrokeThickness = 1;
                    }

                    lineScale.X2 = 100 - 90 * Math.Cos(i * Math.PI / 180);
                    lineScale.Y2 = 100 - 90 * Math.Sin(i * Math.PI / 180);

                    canvas.Children.Add(lineScale);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private Path DrawThermometer(Color StrokeColor, bool isDrawKedu, int ScaleCount, int startValue, Canvas canvas)
        {
            Logger.Info("Func in.");
            Path result = null;
            try
            {
                int BottomCircleX = 50;
                int BottomCircleY = 210;
                int BottomRadius = 10;
                int LineLength = 200;
                Path p = new Path();
                //设置path的线条颜色
                p.Stroke = new SolidColorBrush(StrokeColor);
                //设置path的线条粗线
                p.StrokeThickness = 1;
                //path用来绘制几何形状的类
                PathGeometry geometry = new PathGeometry();
                //因为不止有一条线段
                PathFigureCollection pfc = new PathFigureCollection();
                //path用来绘制线段的类
                PathFigure pf = new PathFigure();
                //这是温度计下面的大圆弧的左边点的X坐标                 
                float LeftPointX = BottomCircleX - (BottomRadius / 5 * 2);
                //这是温度计下面的大圆弧的左边点的Y坐标      
                float LeftPointY = BottomCircleY - (BottomRadius / 5 * 3);
                //这是温度计下面的大圆弧的右边点的X坐标               
                float RightPointX = BottomCircleX + (BottomRadius / 5 * 2);
                //跟左边一样的       
                float RightPointY = LeftPointY;
                //上面小圆的半径
                float TopSmallRoundRadius = BottomRadius / 5 * 2;
                //起始点
                pf.StartPoint = new Point(LeftPointX, LeftPointY);
                //最后闭合
                pf.IsClosed = true;
                //最下面的圆弧
                ArcSegment arc1 = new ArcSegment();
                arc1.Size = new Size(BottomRadius, BottomRadius);
                arc1.IsLargeArc = true;
                arc1.Point = new Point(RightPointX, RightPointY);
                //连接线到上面的小圆
                LineSegment l1 = new LineSegment();
                l1.Point = new Point(RightPointX, RightPointY - LineLength);
                //最上面的圆弧
                ArcSegment arc2 = new ArcSegment();
                arc2.Size = new Size(TopSmallRoundRadius, TopSmallRoundRadius);
                arc2.Point = new Point(LeftPointX, LeftPointY - LineLength);
                //连接线到下面的小圆
                LineSegment l2 = new LineSegment();
                l2.Point = new Point(LeftPointX, LeftPointY);
                pf.Segments = new PathSegmentCollection() { arc1, l1, arc2, l2 };
                pfc.Add(pf);
                //绘制刻度
                if (isDrawKedu)
                {
                    float startPointX = LeftPointX;
                    float startPointY = LeftPointY + BottomRadius * 2;
                    float endPointX = LeftPointX;
                    float endPointY = LeftPointY - LineLength - TopSmallRoundRadius;
                    float eachLength = (startPointY - endPointY) / (ScaleCount * 3);
                    TextBlock txtScale = new TextBlock();

                    //绘制16条
                    for (int i = 0; i < ScaleCount * 3; i++)
                    {
                        PathFigure pf2 = new PathFigure();
                        pf2.StartPoint = new Point(startPointX + BottomRadius, startPointY - i * eachLength);
                        LineSegment l = new LineSegment();
                        if (i % 3 == 0)
                        {
                            l.Point = new Point(startPointX + BottomRadius + 10, startPointY - i * eachLength);
                            txtScale = new TextBlock();
                            txtScale.Text = (startValue + i / 3 * 10).ToString();
                            txtScale.FontSize = 10;
                            Canvas.SetLeft(txtScale, startPointX + BottomRadius + 17);
                            Canvas.SetTop(txtScale, startPointY - i * eachLength - 6);
                            canvas.Children.Add(txtScale);
                        }
                        else
                        {
                            l.Point = new Point(startPointX + BottomRadius + 5, startPointY - i * eachLength);
                        }
                        pf2.Segments = new PathSegmentCollection() { l };
                        pfc.Add(pf2);

                    }
                }
                //设置图形
                geometry.Figures = pfc;
                //设置数据
                p.Data = geometry;
                canvas.Children.Add(p);
                result = p;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
            return result;
        }

        private void setTempPercent(Double v, Path tempPath, int startValue)
        {
            Logger.Info("Func in.");
            try
            {
                LinearGradientBrush brush = new LinearGradientBrush();
                brush.StartPoint = new Point(1, 1);//1,1是path右下角
                brush.EndPoint = new Point(1, 0);//1,0是path右上角  从下而上
                GradientStop stop1 = new GradientStop();
                stop1.Color = Colors.Green;
                stop1.Offset = (v - startValue) / 160;
                GradientStop stop2 = new GradientStop();
                stop2.Color = Colors.AliceBlue;
                stop2.Offset = (v - startValue) / 160;
                //offset的作用:
                //假设上面的offset是0.3,这里的offset是1,就是0.3到1 从紫色渐变到黄色 即0.3是紫色,1是黄色 0.3-1之间是这两种颜色渐变的过程
                brush.GradientStops = new GradientStopCollection() { stop1, stop2 };
                tempPath.Fill = brush;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                Send send = new Send();
                send.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                Settings settings = new Settings();
                settings.LoadDbcEvent += initMainView;
                settings.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void btnSysCtrlStart_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                //this.dbHandler.DeleteAllRecord();
                if (this.CfgVirtualHardwareChannel())
                {
                    this.btnSysCtrlStart.IsEnabled = false;
                    isReceiveMessageThreadStarted = true;
                    receiveMessageThread = new Thread(new ThreadStart(RcvDealFun));
                    receiveMessageThread.Priority = ThreadPriority.Highest;
                    receiveMessageThread.Start();
                    WorkSpace.This.bSysSts_StartStop = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void btnSysCtrlStop_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                this.btnSysCtrlStart.IsEnabled = true;

                SysCtrlStop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Logger.Info("Func in.");
            this.SysCtrlStop();
            if (this.dbHandler != null)
            {
                this.dbHandler.Dispose();
            }
            if (this.fileHandler != null)
            {
                this.fileHandler.Dispose();
            }
            Logger.Info("Func out." + Environment.NewLine);
        }

        private void SysCtrlStop()
        {
            Logger.Info("Func in.");
            try
            {
                WorkSpace.This.bSysSts_StartStop = false;
                isReceiveMessageThreadStarted = false;
                ObjWorkspace.ObjRecordOp_Write.Close();
                Int32 nChanNum = ObjWorkspace.VirtualNode.Lst_RcvDBCHandler.Count;
                for (int i = 0; i < nChanNum; i++)
                {
                    CanInteractionHandler.CloseCANChannel(ref i);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
            initSignalData();
        }

        /// <summary>
        /// configuration virtual hardware channel
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        private bool CfgVirtualHardwareChannel()
        {
            Logger.Info("Func in.");
            var result = true;
            try
            {
                if (ObjWorkspace.VirtualNode.Lst_RcvDBCHandler.Count < 1)
                {
                    Logger.Info("Channel not configured.");
                    MessageBox.Show("请配置通道", "提示", MessageBoxButton.OK);
                    result = false;
                }
                else
                {
                    List<DeviceInfoModel> deviceInfoes = new List<DeviceInfoModel>();
                    foreach (var item in ObjWorkspace.VirtualNode.Lst_RcvDBCHandler)
                    {
                        DeviceInfoModel deviceTemp = new DeviceInfoModel();
                        deviceTemp.DeviceName = ObjWorkspace.VirtualNode.ObjChannelCfgDevCfg.DevName.PadLeft(32, '0');
                        deviceTemp.HardwareIndex = ObjWorkspace.VirtualNode.ObjChannelCfgDevCfg.DevID;
                        deviceTemp.DevicePort = item.DBCChannel.ChannelName == "通道1" ? 0 : 1;
                        deviceTemp.BaudRate = item.DBCChannel.BaudRate;
                        Logger.Info("Channel info: " +
                                    "DeviceName : " + deviceTemp.DeviceName +
                                    "HardwareIndex : " + deviceTemp.HardwareIndex +
                                    "DevicePort : " + deviceTemp.DevicePort +
                                    "BaudRate : " + deviceTemp.BaudRate);
                        deviceInfoes.Add(deviceTemp);
                    }
                    CanInteractionHandler.CANSetDeviceInfo(ref deviceInfoes);
                    for (int j = 0; j < deviceInfoes.Count; j++)
                    {
                        CANError temp = CanInteractionHandler.OpenCANChannelDevice(ref j);
                        if (temp != CANError.ErrNon)
                        {
                            Logger.Error("Open can channel device fault,detail : " + temp.ToString());
                            CanInteractionHandler.ShowErrorDialog(ref j, ref temp);
                            result = false;
                        }
                        else
                        {
                            temp = CanInteractionHandler.StartCANChannel(ref j);
                            if (temp != CANError.ErrNon)
                            {
                                Logger.Error("Start can channel fault,detail : " + temp.ToString());
                                CanInteractionHandler.ShowErrorDialog(ref j, ref temp);
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
            return result;
        }

        /// <summary>
        /// realtime to acquire the messages
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        private void RcvDealFun()
        {
            Logger.Info("Func in.");
            ReceiveObject receiveMessage;
            var lastReceiveTime = DateTime.Now;
            var receiveTime = DateTime.Now;
            var dbcs = WorkSpace.This.VirtualNode.Lst_RcvDBCHandler;
            while (isReceiveMessageThreadStarted)
            {
                try
                {
                    for (Int32 flag = 0; flag < dbcs.Count; flag++)
                    {
                        var channelName = dbcs[flag].DBCChannel.ChannelName;
                        var errorCode = CanInteractionHandler.GetSingleChannelMessage(ref flag, ref channelName, out receiveMessage);
                        receiveTime = DateTime.Now;
                        if ((receiveMessage.ReceiveID == 0X701) && (receiveMessage.IsSelfSend == false))
                        {
                            lastReceiveTime = DateTime.Now;
                            this.ObjWorkspace.BMSSts.ValDesc = "正常";
                            Logger.Info("Receive 0x701 message, communication normal.");
                        }
                        else if ((receiveTime > lastReceiveTime.AddSeconds(2)) && (receiveMessage.IsSelfSend == false))
                        {
                            this.ObjWorkspace.BMSSts.ValDesc = "丢失";
                            Logger.Info("Over 2s not receive 0x701 message, communication abnormal.");
                        }
                        if ((errorCode == CANError.ErrNon) && (receiveMessage.Length > 0) && ((receiveMessage.IsSelfSend == false) || (receiveMessage.ReceiveID == 0x720)))
                        {
                            DBCMessage msg = new DBCMessage();
                            dbcs[flag].DBCChannel.SearchDBCMessageByID(receiveMessage.ReceiveID, out msg);
                            if (msg != null)
                            {
                                Logger.Info("Receive a meesage from Channel : " + channelName + " ,message id : " + receiveMessage.ReceiveID + ", message name : " + msg.MessageName);
                                Logger.Debug("Receive Data: ");
                                Logger.Debug("Data[0]: " + receiveMessage.ReceiveData[0]);
                                Logger.Debug("Data[1]: " + receiveMessage.ReceiveData[1]);
                                Logger.Debug("Data[2]: " + receiveMessage.ReceiveData[2]);
                                Logger.Debug("Data[3]: " + receiveMessage.ReceiveData[3]);
                                Logger.Debug("Data[4]: " + receiveMessage.ReceiveData[4]);
                                Logger.Debug("Data[5]: " + receiveMessage.ReceiveData[5]);
                                Logger.Debug("Data[6]: " + receiveMessage.ReceiveData[6]);
                                Logger.Debug("Data[7]: " + receiveMessage.ReceiveData[7]);
                                foreach (var signal in msg.Signals)
                                {
                                    var tempSignal = signal;
                                    String dataValue;
                                    DBCSignal.GetDbcData(ref tempSignal, receiveMessage.ReceiveData, out dataValue);
                                }
                                this.drawCellGauge(msg);
                                this.drawTotalGauge(msg);
                                this.drawThermometer(msg);
                                this.calFalInfo(msg);
                                Logger.Info("Handle message done!");
                            }
                            else
                            {
                                Logger.Error("Receive error message,id : " + receiveMessage.ReceiveID);
                            }
                        }
                        else
                        {
                            //TODO
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
            Logger.Info("Func out.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.ObjWorkspace.FilePath = folderDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                if (this.fileHandler == null)
                {
                    this.fileHandler = FileHandler.Instance();
                }
                if (this.ChangeFileRecoedInfo(this.fileHandler.IsStart) == true)
                {
                    MessageBox.Show("数据存储配置应用成功！", "提示", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out");
        }

        private Boolean ChangeFileRecoedInfo(Boolean isStartRecord)
        {
            Logger.Info("Func in");
            var result = false;
            try
            {
                if (isStartRecord)
                {
                    this.fileHandler.Stop();
                }
                this.fileHandler.FileName = this.ObjWorkspace.FileName;
                this.fileHandler.LineCountLimit = this.ObjWorkspace.LineLimit;
                this.fileHandler.TimeSpan = this.ObjWorkspace.Interval;
                this.fileHandler.Directory = this.ObjWorkspace.FilePath;
                this.fileHandler.Mode = LimitMode.LineMode;
                if (isStartRecord)
                {
                    this.fileHandler.Start();
                }
                result = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
            return result;
        }

        private void StartRecord(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                if (this.fileHandler == null)
                {
                    this.fileHandler = FileHandler.Instance();
                }
                this.fileHandler.Start();
                this.btnStartRecord.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void StopRecord(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                if (this.fileHandler == null)
                {
                    this.fileHandler = FileHandler.Instance();
                }
                this.fileHandler.Stop();
                this.btnStartRecord.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void initMainView()
        {
            Logger.Info("Func in.");
            try
            {
                Type type = typeof(MainWindow);
                foreach (var handler in ObjWorkspace.VirtualNode.Lst_RcvDBCHandler)
                {
                    foreach (var node in handler.DBCChannel.Nodes)
                    {
                        foreach (var message in node.Messages)
                        {
                            foreach (var signal in message.Signals)
                            {
                                foreach (var item in signal.Values)
                                {
                                    if (WorkSpace.SignalDefaultValueDescription.Contains(item.Key.ToLower()))
                                    {
                                        signal.Data = item.Value;
                                        break;
                                    }
                                }
                                var fieldInfo = type.GetField(signal.SignalName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                if (fieldInfo != null)
                                {
                                    var field = (StackPanel)fieldInfo.GetValue(WorkSpace.This.MainWindow);
                                    field.DataContext = signal;
                                }
                            }
                        }
                    }
                }
                this.BMSSts.DataContext = this.ObjWorkspace.BMSSts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void drawThermometer(DBCMessage msg)
        {
            Logger.Info("Func in.");
            if (msg.MessageID == 0x705)
            {
                var drawDelegate = new UpdateWindowDelegate(() =>
                {
                    Logger.Info("Receive BMS_CellTempExtremum message ,id : 0x705");
                    DBCSignal signal;
                    msg.SearchSignalByName("INHW_Ve_CellMaxTemp", out signal);
                    setTempPercent(signal.Data, MaxTempPath, TempStartScale);
                    msg.SearchSignalByName("INHW_Ve_CellMinTemp", out signal);
                    setTempPercent(signal.Data, MinTempPath, TempStartScale);
                });
                this.Dispatcher.Invoke(drawDelegate, null);
            }
            Logger.Info("Func out.");
        }

        private void drawCellGauge(DBCMessage msg)
        {
            Logger.Info("Func in.");
            if (msg.MessageID == 0x707)
            {
                var drawDelegate = new UpdateWindowDelegate(() =>
                {
                    Logger.Info("Receive " + msg.MessageName + " message");
                    DBCSignal signal;
                    msg.SearchSignalByName("INHW_Ve_CellMaxVolt", out signal);
                    setAngelGauge(old_INHW_Ve_CellMaxVolt, signal.Data / 1000, 5, this.pinMaxCellVolt);
                    old_INHW_Ve_CellMaxVolt = signal.Data / 1000;
                    msg.SearchSignalByName("INHW_Ve_CellMinVolt", out signal);
                    setAngelGauge(old_INHW_Ve_CellMinVolt, signal.Data / 1000, 5, this.pinMinCellVolt);
                    old_INHW_Ve_CellMinVolt = signal.Data / 1000;
                });
                this.Dispatcher.Invoke(drawDelegate, null);
            }
            Logger.Info("Func out.");
        }

        private void drawTotalGauge(DBCMessage msg)
        {
            Logger.Info("Func in.");
            if (msg.MessageID == 0x70F)
            {
                var drawDelegate = new UpdateWindowDelegate(() =>
                {
                    Logger.Info("Receive " + msg.MessageName + " message");
                    DBCSignal signal;
                    msg.SearchSignalByName("INHW_Ve_BattCurr", out signal);
                    setAngelGauge(old_INHW_Ve_BattCurr, signal.Data, 1000, this.pinCurr);
                    old_INHW_Ve_BattCurr = signal.Data;
                    msg.SearchSignalByName("INHW_Ve_BattPackVolt", out signal);
                    setAngelGauge(old_INHW_Ve_BattPatVolt, signal.Data, 600, this.pinVolt);
                    old_INHW_Ve_BattPatVolt = signal.Data;
                });
                this.Dispatcher.Invoke(drawDelegate, null);
            }
            Logger.Info("Func out.");
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                if (((String)btnLogin.Content) == "登陆")
                {
                    LoginWindow login = new LoginWindow();
                    login.ShowDialog();
                    if (login.isLoginSuccess)
                    {
                        Logger.Info("Log in success.");
                        this.btnSend.IsEnabled = true;
                        btnLogin.Content = "注销";
                    }
                }
                else
                {
                    btnLogin.Content = "登陆";
                    btnSend.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
        }

        private void calFalInfo(DBCMessage msg)
        {
            Logger.Info("Func in.");
            if (msg.MessageID == 0x70A)
            {
                var drawDelegate = new UpdateWindowDelegate(() =>
                {
                    Logger.Info("Receive " + msg.MessageName + " message");
                    StringBuilder appender = new StringBuilder();
                    foreach (var signal in msg.Signals)
                    {
                        if ((signal.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignals) &&
                            (signal.Data != 0))
                        {
                            appender.Append(signal.SignalName + ";");
                        }
                    }
                    this.FaultCodeInfo.Text = appender.ToString();
                });
                this.Dispatcher.Invoke(drawDelegate, null);
            }
            Logger.Info("Func out.");
        }
        /// <summary>
        /// 初始化界面信号数据
        /// </summary>
        private void initSignalData()
        {
            this.ObjWorkspace.BMSSts.ValDesc = "丢失";
        }
    }
}

using Neusoft.Reach.DBCAnalysis.Handle;
using Neusoft.Reach.DBCAnalysis.Model;

using PMA_Project.Models;

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PMA_Project.View
{
    /// <summary>
    /// Interaction logic for SendNew.xaml
    /// </summary>

    public partial class SendAdd : Window
    {

        #region 单例

        /// <summary>
        /// 单例实体
        /// </summary>
        private static SendAdd entity;


        /// <summary>
        /// 获取单例实体
        /// </summary>
        /// <returns></returns>
        public static SendAdd Instance()
        {
            if (entity == null)
            {
                entity = new SendAdd();
            }
            return entity;
        }

        #endregion


        private SendAdd()
        {
            InitializeComponent();
        }

        public delegate void GetMessage(DBCHandler dbc, DBCMessage message);
        public GetMessage getMessage;
        private ObservableCollection<treeViewCfg> treeInfo = new ObservableCollection<treeViewCfg>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (WorkSpace.This.VirtualNode.Lst_RcvDBCHandler.Count < 0)
            {
                MessageBox.Show("请先配置DBC!");
            }
            else
            {
                var dbcs = WorkSpace.This.VirtualNode.Lst_RcvDBCHandler;

                foreach (var dbc in dbcs) //通道名
                {
                    treeViewCfg ChannelTemp = new treeViewCfg();
                    ChannelTemp.Parent = null;
                    ChannelTemp.Child = new ObservableCollection<treeViewCfg>();
                    ChannelTemp.NodesName = dbc.DBCChannel.ChannelName;
                    treeViewCfg DBCTemp = new treeViewCfg();
                    DBCTemp.Parent = ChannelTemp;
                    DBCTemp.Child = new ObservableCollection<treeViewCfg>();
                    DBCTemp.NodesName = System.IO.Path.GetFileName(dbc.DBCFilePath);
                    foreach (var name in dbc.DBCChannel.Nodes)//BMU
                    {
                        treeViewCfg nodeTemp = new treeViewCfg();
                        nodeTemp.Parent = DBCTemp;
                        nodeTemp.Child = new ObservableCollection<treeViewCfg>();
                        nodeTemp.NodesName = name.NodeName;
                        foreach (var msg in name.Messages)//报文
                        {
                            treeViewCfg msgTemp = new treeViewCfg();
                            msgTemp.Parent = nodeTemp;
                            msgTemp.NodesName = "(0x" + Convert.ToString(msg.MessageID, 16) + ")" + msg.MessageName;
                            nodeTemp.Child.Add(msgTemp);
                        }
                        DBCTemp.Child.Add(nodeTemp);
                    }
                    ChannelTemp.Child.Add(DBCTemp);
                    treeInfo.Add(ChannelTemp);
                }
                this.treeView.ItemsSource = treeInfo;
            }
        }

        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    this.Hide();
        //    e.Cancel = true;
        //}

        int i;
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            i += 1;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += (s, e1) => { timer.IsEnabled = false; i = 0; };
            timer.IsEnabled = true;
            if (i % 2 == 0)
            {
                timer.IsEnabled = false;
                i = 0;
                treeViewCfg selectItem = (treeViewCfg)this.treeView.SelectedItem;
                //Add double mouse click events
                if (getMessage != null)
                {
                    treeViewCfg NodeItem = selectItem.Parent as treeViewCfg;
                    if (NodeItem == null)
                        return;
                    treeViewCfg FileItem = NodeItem.Parent as treeViewCfg;
                    if (FileItem == null)
                        return;
                    treeViewCfg ChannelItem = FileItem.Parent as treeViewCfg;
                    if (ChannelItem == null)
                        return;

                    string strMessageIDName = selectItem.NodesName.ToString();
                    string strOutID = strMessageIDName.Substring(3, strMessageIDName.LastIndexOf(")") - 3);
                    UInt32 nOutID = Convert.ToUInt32(strOutID, 16);
                    DBCMessage objCMsg = new DBCMessage();
                    foreach (var dbc in WorkSpace.This.VirtualNode.Lst_RcvDBCHandler)
                    {
                        if (dbc.DBCChannel.ChannelName != ChannelItem.NodesName.ToString())
                        {
                            continue;
                        }

                        string dbcfilename = dbc.DBCFilePath.Substring(dbc.DBCFilePath.LastIndexOf("\\") + 1);
                        if (dbcfilename != FileItem.NodesName.ToString())
                        {
                            continue;
                        }
                        if (dbc.DBCChannel.SearchDBCMessageByID(nOutID, out objCMsg))
                        {
                            getMessage(dbc, objCMsg);
                        }
                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SendAdd.entity = null;
        }
    }
    public class treeViewCfg : ViewModelBase
    {
        private string _nodesName;
        private ObservableCollection<treeViewCfg> _child;
        private treeViewCfg _parent;
        public string NodesName
        {
            get { return _nodesName; }
            set
            {
                _nodesName = value;
                RaisePropertyChanged("NodesName");
            }
        }
        public ObservableCollection<treeViewCfg> Child
        {
            get { return _child; }
            set
            {
                _child = value;
                RaisePropertyChanged("Child");
            }
        }
        public treeViewCfg Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                RaisePropertyChanged("Parent");
            }
        }
    }
}

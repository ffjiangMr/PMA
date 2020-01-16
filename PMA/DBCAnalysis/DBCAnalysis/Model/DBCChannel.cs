
/*
******************************************************************************                                                                                                                                         
*  File name:        DBCChannel.cs                                                                                                                                                                                                                            
*  Copyright         ReachAuto Corporation. All rights reserved.                                                                                                                              
*  Notes:                                                                                                                              
*  History:                                                                                                                              
*    Revision        Date           Name              Comment                                                              
*    ------------------------------------------------------------------                
*    1.0          2019.04.10        JiangFei           Initial                                                                      
*                                                                                                                                          
******************************************************************************
*/

#region using directive

using Neusoft.Reach.DBCAnalysis.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel;

#endregion


namespace Neusoft.Reach.DBCAnalysis.Model
{
    /// <summary>
    /// 与下位机通信信道
    /// </summary>
    public sealed class DBCChannel : ViewModel
    {
        #region property define

        /// <summary>
        /// 通道Name
        /// </summary>
        public String ChannelName
        {
            get
            {
                return this.channelName;
            }
            set
            {
                this.channelName = value;
                RaisePropertyChanged("ChannelName");
            }
        }

        /// <summary>
        /// 波特率
        /// 单位Kbps
        /// </summary>
        public Int32 BaudRate
        {
            get
            {
                return this.baudRate;
            }
            set
            {
                this.baudRate = value;
                RaisePropertyChanged("BaudRate");
            }
        }

        /// <summary>
        /// DBC类型
        /// </summary>
        public DBCType DBCType
        {
            get
            {
                return this.dbcType;

            }
            set
            {
                this.dbcType = value;
                RaisePropertyChanged("DBCType");
            }
        }

        /// <summary>
        /// 通道包含的Node 列表
        /// </summary>
        public List<DBCNode> Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        #endregion

        #region private field

        private String channelName;

        private DBCType dbcType;

        private List<DBCNode> nodes = new List<DBCNode>() { new DBCNode() { NodeName = ConstDefine.DefaultNodeName } };

        private Int32 baudRate;

        #endregion

        #region public method

        /// <summary>
        /// 清除节点列表
        /// </summary>
        public void ClearNodes()
        {
            this.Nodes.Clear();
            this.RaisePropertyChanged("Nodes");
        }

        /// <summary>
        /// 根据Message ID 从当前Channel中搜索指定Message
        /// 如果存在多个匹配则返回第一个匹配项
        /// </summary>
        /// <param name="id">message ID</param>
        /// <param name="message">搜索结果</param>
        /// <returns>搜索是否成功</returns>
        public Boolean SearchDBCMessageByID(UInt32 id, out DBCMessage message)
        {
            var result = false;
            message = null;
            foreach (var node in this.Nodes)
            {
                foreach (var msg in node.Messages)
                {
                    if (msg.MessageID == id)
                    {
                        message = msg;
                        result = true;
                        break;
                    }
                }
                if (result == true)
                {
                    break;
                }
            }
            return result;
        }

        #endregion

    }
}

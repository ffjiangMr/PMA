
/*
******************************************************************************                                                                                                                                         
*  File name:        DBCHandler.cs                                                                                                                                                                                                                            
*  Copyright         ReachAuto Corporation. All rights reserved.                                                                                                                              
*  Notes:                                                                                                                              
*  History:                                                                                                                              
*    Revision        Date           Name              Comment                                                              
*    ------------------------------------------------------------------                
*    1.0          2019.04.10        JiangFei           Initial                                                                      
*                                                                                                                                          
******************************************************************************
*/

#region  using directive

using Neusoft.Reach.DBCAnalysis.Infrastructure;
using Neusoft.Reach.DBCAnalysis.Model;

using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Neusoft.Reach.DBCAnalysis.Handle
{
    /// <summary>
    /// 解析并保存DBC文件数据
    /// </summary>
    public sealed class DBCHandler : ViewModel
    {
        #region property define
        /// <summary>
        /// DBC channel
        /// </summary>
        public DBCChannel DBCChannel { get; } = new DBCChannel();

        /// <summary>
        /// DBC 文件路径
        /// </summary>
        public String DBCFilePath
        {
            get
            {
                return this.dbcFilePath;
            }
            set
            {
                this.dbcFilePath = value;
                this.RaisePropertyChanged("DBCFilePath");
            }
        }

        #endregion

        #region private field

        private String dbcFilePath;

        #endregion

        #region public method

        /// <summary>
        /// 加载DBC文件
        /// 使用初始化路径
        /// </summary>
        /// <returns>是否成功</returns>
        public Boolean LoadDBC()
        {
            return this.LoadDBC(this.DBCFilePath);
        }

        /// <summary>
        /// 加载DBC文件
        /// </summary>
        /// <param name="dbcPath">DBC文件路径</param>
        /// <returns>是否成功</returns>
        public Boolean LoadDBC(String dbcPath)
        {
            Boolean result = false;
            var tempPath = dbcPath;
            if (!File.Exists(dbcPath))
            {
                throw new FileNotFoundException("DBC文件不存在,Path : " + dbcPath);
            }
            String dbcHead, dbcContent;
            this.DBCChannel.DBCType = DBCType.StandardDBC;
            using (var reader = new StreamReader(tempPath, Encoding.Default))
            {
                dbcContent = reader.ReadLine();
                while (dbcContent != null)
                {
                    if (dbcContent.Length > 4)
                    {
                        dbcHead = dbcContent.Substring(0, 4);
                        switch (dbcHead)
                        {
                            case ConstDefine.NetworkNodeDefinition:
                                this.LoadNode(dbcContent);
                                break;
                            case ConstDefine.Message:
                                this.LoadMessage(dbcContent, reader);
                                break;
                            case ConstDefine.ValueEncodings:
                                if (dbcContent.StartsWith(ConstDefine.ValueEncodings + " "))
                                {
                                    this.LoadSignalValue(dbcContent);
                                }
                                break;
                            case ConstDefine.AttributeDefinition:
                                if (dbcContent.StartsWith(ConstDefine.AttributeDefault + ConstDefine.ProtocolType))
                                {
                                    this.LoadDBCType(dbcContent);
                                }
                                break;
                            case ConstDefine.AttributeValues:
                                if (Regex.IsMatch(dbcContent, ConstDefine.AttributeRegexPattern))
                                {
                                    this.LoadAttribute(dbcContent);
                                }
                                break;
                        }
                    }
                    dbcContent = reader.ReadLine();
                }
                result = true;
                this.DBCFilePath = dbcPath;
                this.RaisePropertyChanged("DBCChannel");
            }
            return result;
        }

        #endregion

        #region private method

        /// <summary>
        /// 加载DBC节点
        /// </summary>
        /// <param name="content">当前行内容</param>
        private void LoadNode(String content)
        {
            String tempContent = content;

            //找到分隔符" "的位置,开始解析Node Name
            var index = tempContent.IndexOf(" ", 0);
            if (index != -1)
            {
                tempContent = content.Substring(index, content.Length - index);

                //根据" "分割字符串，获取NodeName 列表,数组中不包含空串
                var nodeNames = tempContent.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //遍历NodeName 列表
                //创建Node切初始化name
                //将Node添加至Channle Node 列表
                foreach (var item in nodeNames)
                {
                    var node = new DBCNode()
                    {
                        NodeName = item
                    };
                    this.DBCChannel.Nodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 加载DBC Message
        /// </summary>
        /// <param name="content">当前行内容</param>
        /// <param name="reader">当前文件流</param>
        private void LoadMessage(String content, StreamReader reader)
        {
            DBCMessage message = new DBCMessage();

            //起始索引
            Int32 preIndex = 0;

            //终止索引

            //获取分隔符位置，开始解析            
            preIndex = content.IndexOf(" ", preIndex) + 1;
            var index = content.IndexOf(" ", preIndex);

            #region 获取Message ID

            String id = content.Substring(preIndex, index - preIndex);
            UInt32 messageId;
            if (!UInt32.TryParse(id, out messageId))
            {
                messageId = 0;
            }
            messageId &= 0x9FFFFFFF;
            message.MessageID = messageId;

            #endregion

            #region 判断消息帧类型

            UInt32 frameType = messageId >> 31;
            if (frameType == 0)
            {
                message.FrameType = FrameType.StandardFrame;
            }
            else
            {
                message.FrameType = FrameType.ExtendFrame;
            }
            #endregion

            #region 获取消息Name

            preIndex = index + 1;
            index = content.IndexOf(":", preIndex);
            message.MessageName = content.Substring(preIndex, index - preIndex);

            #endregion

            #region 获取消息 Size (dlc)

            preIndex = index + 2;
            index = content.IndexOf(" ", preIndex);
            UInt32 size;
            if (!UInt32.TryParse(content.Substring(preIndex, index - preIndex), out size))
            {
                size = 0;
            }
            message.MessageSize = size;

            #endregion

            #region 获取发送节点Name

            preIndex = index + 1;
            index = content.IndexOf("", preIndex);
            message.Transmitter = content.Substring(index, content.Length - index);

            #endregion

            #region 将消息添加至节点

            foreach (var node in this.DBCChannel.Nodes)
            {
                if (message.Transmitter == node.NodeName)
                {
                    var nextContent = reader.ReadLine();
                    while (nextContent != null)
                    {
                        if (String.IsNullOrEmpty(nextContent))
                        {
                            break;
                        }
                        LoadSignal(nextContent, ref message);
                        nextContent = reader.ReadLine();
                    }
                    node.Messages.Add(message);
                    break;
                }
            }

            #endregion           
        }

        /// <summary>
        /// 加载DBC Signal
        /// </summary>
        /// <param name="content">当前行内容</param>
        /// <param name="message">当前消息</param>
        private void LoadSignal(String content, ref DBCMessage message)
        {
            var match = Regex.Match(content.Trim(), ConstDefine.SignalRegexPattern);
            if (!match.Success)
            {
                //TODO 应该添加Log
                return;
            }
            UInt16 tempUInt16;
            Double tempDouble;
            DBCSignal signal = new DBCSignal();
            ByteOrder byteOrder;
            if (!Enum.TryParse<ByteOrder>(match.Groups[9].Value, out byteOrder))
            {
                byteOrder = ByteOrder.None;
            }
            signal.ByteOrder = byteOrder;
            if (!Double.TryParse(match.Groups[11].Value, out tempDouble))
            {
                tempDouble = 0D;
            }
            signal.Factor = tempDouble;
            if (!Double.TryParse(match.Groups[17].Value, out tempDouble))
            {
                tempDouble = 0D;
            }
            signal.Maximum = tempDouble;
            signal.Message = message;
            signal.MessageID = message.MessageID;
            signal.MessageName = message.MessageName;
            if (!Double.TryParse(match.Groups[15].Value, out tempDouble))
            {
                tempDouble = 0D;
            }
            signal.Minimum = tempDouble;           
            if (String.IsNullOrEmpty(match.Groups[2].Value))
            {
                signal.MultiplexerIndicator = MultiplexerIndicator.NormalSignal;
            }
            else if (match.Groups[6].Value == "M ")
            {
                signal.MultiplexerIndicator = MultiplexerIndicator.MultiplexedSignal;
            }
            else if (match.Groups[4].Value == "m")
            {
                signal.MultiplexerIndicator = MultiplexerIndicator.MultiplexedSignals;
                if (!UInt16.TryParse(match.Groups[5].Value, out tempUInt16))
                {
                    tempUInt16 = 0;
                }
                signal.MultiplexerSwitchValue = tempUInt16;
            }
            else
            {
                signal.MultiplexerIndicator = MultiplexerIndicator.NormalSignal;
            }
            if (!Double.TryParse(match.Groups[13].Value, out tempDouble))
            {
                tempDouble = 0D;
            }
            signal.Offset = tempDouble;
            signal.SignalName = match.Groups[1].Value;
            if (!UInt16.TryParse(match.Groups[8].Value, out tempUInt16))
            {
                tempUInt16 = 0;
            }
            signal.SignalSize = tempUInt16;
            if (!UInt16.TryParse(match.Groups[7].Value, out tempUInt16))
            {
                tempUInt16 = 0;
            }
            signal.StartPosition = tempUInt16;
            signal.Unit = match.Groups[19].Value;
            if (match.Groups[10].Value == "+")
            {
                signal.ValueType = Infrastructure.ValueType.Unsigned;
            }
            else if (match.Groups[10].Value == "-")
            {
                signal.ValueType = Infrastructure.ValueType.Signed;
            }
            else
            {
                signal.ValueType = Infrastructure.ValueType.Unsigned;
            }
            message.Signals.Add(signal);
        }

        /// <summary>
        /// 加载DBC 文件中 Signal value descriptions define encodings for specific signal raw values.
        /// </summary>
        /// <param name="content">当前行内容</param>
        private void LoadSignalValue(String content)
        {
            String tempString = String.Empty;
            UInt32 tempUint32 = 0;
            Int32 preIndex = 0;
            Int32 index = -1;
            preIndex = content.IndexOf(" ", preIndex) + 1;
            index = content.IndexOf(" ", preIndex);
            String messageId = content.Substring(preIndex, index - preIndex);
            UInt32 id;
            if (!UInt32.TryParse(messageId, out id))
            {
                id = 0;
            }

            DBCMessage message;
            if (this.DBCChannel.SearchDBCMessageByID(id, out message) == false)
            {
                return;
            }
            preIndex = index + 1;
            index = content.IndexOf(" ", preIndex);
            tempString = content.Substring(preIndex, index - preIndex);
            DBCSignal signal;
            if (message.SearchSignalByName(tempString, out signal) == false)
            {
                return;
            }
            preIndex = index + 1;
            index = content.IndexOf(" ", preIndex);

            while (index != -1)
            {
                tempString = content.Substring(preIndex, index - preIndex);
                if (!UInt32.TryParse(tempString, out tempUint32))
                {
                    tempUint32 = 0;
                }
                preIndex = index + 2;
                index = content.IndexOf("\"", preIndex);
                tempString = content.Substring(preIndex, index - preIndex);
                signal.Values[tempString] = tempUint32;
                preIndex = index + 2;
                index = content.IndexOf(" ", preIndex);
            }
        }

        /// <summary>
        /// 加载DBC Type
        /// </summary>
        /// <param name="content">当前行内容</param>
        private void LoadDBCType(String content)
        {
            Int32 preIndex = 27;
            Int32 index = 0;

            preIndex = content.IndexOf("\"", preIndex) + 1;
            index = content.IndexOf("\"", preIndex);

            if (content.Substring(preIndex, index - preIndex) == "J1939")
            {
                this.DBCChannel.DBCType = DBCType.J1939DBC;
            }
        }

        /// <summary>
        /// 加载DBC Attribute
        /// 当前只加载 Node的NmStationAddress
        /// </summary>
        /// <param name="content">当前行内容</param>
        private void LoadAttribute(String content)
        {
            var match = Regex.Match(content, ConstDefine.AttributeRegexPattern);
            if (match.Success == false)
            {
                return;
            }
            //目前只解析Node指定属性"NmStationAddress"
            if ((match.Groups[11].Value == ConstDefine.NetworkNode) &&
                (match.Groups[1].Value == ConstDefine.NmStationAddress))
            {
                foreach (var node in this.DBCChannel.Nodes)
                {
                    if (node.NodeName == match.Groups[12].Value)
                    {
                        UInt32 tempValue;
                        if (UInt32.TryParse(match.Groups[13].Value, out tempValue))
                        {
                            tempValue = 0;
                        }
                        node.NodeAddress = tempValue;
                        break;
                    }
                }
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DBCHandler()
            : this(String.Empty)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbcPath">DBC 文件路径</param>
        public DBCHandler(String dbcPath)
        {
            this.DBCFilePath = dbcPath;
        }

        #endregion 

    }
}

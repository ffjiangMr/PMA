
/*
******************************************************************************                                                                                                                                         
*  File name:        DBCMessage.cs                                                                                                                                                                                                                            
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

#endregion 

namespace Neusoft.Reach.DBCAnalysis.Model
{
    /// <summary>
    /// The message section defines the names of all frames in the cluster as well as their
    /// properties and the signals transferred on the frames.
    /// </summary>
    public sealed class DBCMessage : ViewModel
    {

        #region property define

        /// <summary>
        /// 原始数据
        /// </summary>
        public UInt64 RawData
        {
            get
            {
                UInt64 temp = 0;
                foreach (var signal in this.Signals)
                {
                    temp |= signal.RawData;
                }
                return temp;
            }
        }

        /// <summary>
        /// The names defined in this section have to be unique within the set of messages.
        /// </summary>
        public String MessageName
        {
            get
            {
                return this.messageName;
            }
            set
            {
                this.messageName = value;
                RaisePropertyChanged("MessageName");
            }
        }

        /// <summary>
        /// The message's CAN-ID. The CAN-ID has to be unique within the DBC file. 
        /// If the most significant bit of the CAN-ID is set, the ID is an extended CAN ID.
        /// The extended CAN ID can be determined by masking out the most significant bit with the mask 0xCFFFFFFF.
        /// </summary>
        public UInt32 MessageID
        {
            get
            {
                return this.messageID;
            }
            set
            {
                this.messageID = value;
                RaisePropertyChanged("MessageID");
            }
        }

        /// <summary>
        /// The transmitter name specifies the name of the node transmitting the message.
        /// The sender name has to be defined in the set of node names in the node section.
        /// If the massage shall have no sender, the string 'Vector__XXX' has to be given here.
        /// </summary>
        public String Transmitter
        {
            get
            {
                return this.transmitter;
            }
            set
            {
                this.transmitter = value;
                RaisePropertyChanged("Transmitter");
            }
        }

        /// <summary>
        /// The message_size specifies the size of the message in bytes.
        /// </summary>
        public UInt32 MessageSize { get
            {
                return this.messageSize;

            } set
            {
                this.messageSize = value;
                RaisePropertyChanged("MessageSize");
            } }

        /// <summary>
        /// 帧类型
        /// </summary>
        public FrameType FrameType { get
            {
                return this.frameType;
            } set
            {
                this.frameType = value;
                RaisePropertyChanged("FrameType");
            } }

        /// <summary>
        /// The message's signal section lists all signals placed on the message.
        /// </summary>
        public List<DBCSignal> Signals { get; } = new List<DBCSignal>();

        #endregion

        #region  private field

        private String messageName;
        private UInt32 messageID;
        private String transmitter;
        private UInt32 messageSize;
        private FrameType frameType;
        #endregion

        #region public method

        /// <summary>
        /// 从message中检索MultiplexedSignal
        /// 如果存在多个，则只返回第一个
        /// </summary>
        /// <param name="signal">搜索结果</param>
        /// <returns>搜索是否成功</returns>
        public Boolean SearchMultiplexerSignalFormMessage(out DBCSignal signal)
        {
            var result = false;
            signal = null;
            foreach (var item in this.Signals)
            {
                if (item.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignal)
                {
                    signal = item;
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据Name从Message检索Signal
        /// 如果存在多个匹配则返回第一个匹配
        /// </summary>
        /// <param name="name">Signal name</param>
        /// <param name="signal">检索结果</param>
        /// <returns>是否成功</returns>
        public Boolean SearchSignalByName(String name, out DBCSignal signal)
        {
            var result = false;
            signal = null;
            foreach (var item in this.Signals)
            {
                if (item.SignalName == name)
                {
                    signal = item;
                    result = true;
                    break;
                }
            }
            return result;
        }

        #endregion

    }
}

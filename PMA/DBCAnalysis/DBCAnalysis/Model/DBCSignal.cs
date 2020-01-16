
/*
******************************************************************************                                                                                                                                         
*  File name:        DBCSignal.cs                                                                                                                                                                                                                            
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
    #region using directive

    using ValueType = Infrastructure.ValueType;

    #endregion

    /// <summary>
    /// The message's signal section lists all signals placed on the message, 
    /// their position in the message's data field and their properties.
    /// </summary>
    public sealed class DBCSignal : ViewModel
    {
        #region  property Define

        /// <summary>
        /// Signal原始数据        
        /// </summary>
        public UInt64 RawData
        {
            get
            {
                UInt64 result = 0;
                if (this.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignals)
                {
                    DBCSignal signal;
                    this.Message.SearchMultiplexerSignalFormMessage(out signal);
                    if (signal != null)
                    {
                        if (this.MultiplexerSwitchValue == signal.Data)
                        {
                            result = this.rawData;
                        }
                    }
                }
                return result;
            }
            set
            {
                UInt64 mask = 0;
                for (Int32 flag = 0; flag < this.SignalSize; flag++)
                {
                    mask <<= 1;
                    mask |= 1;
                }
                if (this.ByteOrder == ByteOrder.Motorola)
                {
                    UInt64 temp = value & mask;
                    Int32 offset = ((8 * 8) - (7 - this.StartPosition % 8) - this.SignalSize - (this.StartPosition / 8 * 8));
                    temp <<= offset;
                    mask = 0xFF;
                    for (Int32 flag = 0; flag < 8; flag++)
                    {
                        this.rawData |= (temp & mask);
                        if (flag < 7)
                        {
                            this.rawData <<= 8;
                            temp >>= 8;
                        }
                    }
                }
                else if (this.ByteOrder == ByteOrder.Intel)
                {
                    this.rawData = value & mask;
                    Int32 startPosition = this.StartPosition + this.SignalSize - 1;
                    Int32 offset = ((8 * 8) - (7 - startPosition % 8) - this.SignalSize - (startPosition / 8 * 8));
                    this.rawData <<= offset;
                }
                RaisePropertyChanged("RawData");
            }
        }

        /// <summary>
        /// The names defined here have to be unique for the signals of a single message.
        /// </summary>
        public String SignalName { get; set; }

        /// <summary>
        /// The message's CAN-ID. The CAN-ID has to be unique within the DBC file.        
        /// </summary>
        public UInt32 MessageID { get; set; }

        /// <summary>
        /// The names defined in this section have to be unique within the set of messages.
        /// </summary>
        public String MessageName { get; set; }

        /// <summary>
        /// The start bit value specifies the position of the signal within the data field of the frame.
        /// The startbit has to be in the range of 0 to (8 * message_size - 1).
        /// </summary>
        public UInt16 StartPosition { get; set; }

        /// <summary>
        /// The signal_size specifies the size of the signal in bits.
        /// </summary>
        public UInt16 SignalSize { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType IsData
        {
            get
            {
                if ((this.Values != null) && (this.Values.Count > 0))
                {
                    return DataType.SenseType;
                }
                return DataType.DataType;
            }
        }

        /// <summary>
        /// The factor and offset define the linear conversion rule to convert the signals raw
        /// value into the signal's physical value and vice versa:
        /// physical_value = raw_value* factor + offset
        /// raw_value = (physical_value – offset) / factor
        /// As can be seen in the conversion rule formulas the factor must not be 0.
        /// </summary>
        public Double Factor { get; set; }

        /// <summary>
        /// The factor and offset define the linear conversion rule to convert the signals raw
        /// value into the signal's physical value and vice versa:
        /// physical_value = raw_value* factor + offset
        /// raw_value = (physical_value – offset) / factor
        /// As can be seen in the conversion rule formulas the factor must not be 0.
        /// </summary>
        public Double Offset { get; set; }

        /// <summary>
        /// The minimum and maximum define the range of valid physical values of the signal.
        /// </summary>
        public Double Minimum { get; set; }

        /// <summary>
        /// The minimum and maximum define the range of valid physical values of the signal.
        /// </summary>
        public Double Maximum { get; set; }

        /// <summary>
        /// 数据单位
        /// </summary>
        public String Unit { get; set; }

        /// <summary>
        /// The byte_format is 0 if the signal's byte order is Motorola(big endian) or
        /// 1 if the byte  order is  Intel (little endian).
        /// </summary>
        public ByteOrder ByteOrder { get; set; }

        /// <summary>
        /// The value_type defines the signal as being of type unsigned (+) or signed (-).
        /// </summary>
        public ValueType ValueType { get; set; }

        /// <summary>
        /// The multiplexer indicator defines whether the signal is a normal signal,
        /// a multiplexer switch for multiplexed signals, or a multiplexed signal.
        /// 针对359信号
        /// </summary>
        public MultiplexerIndicator MultiplexerIndicator { get; set; }

        /// <summary>
        /// The multiplexed signal is transferred in the message if the switch value of
        /// the multiplexer signal is equal to its multiplexer_switch_value.
        /// </summary>
        public UInt16 MultiplexerSwitchValue { get; set; }

        /// <summary>
        /// 包含signal 的  message
        /// </summary>
        public DBCMessage Message { get; set; } = null;

        /// <summary>
        /// Signal value descriptions define encodings for specific signal raw values.
        /// </summary>
        public Dictionary<String, UInt32> Values { get; } = new Dictionary<String, UInt32>();

        /// <summary>
        /// 信号的真实数据
        /// </summary>
        public Double Data
        {
            get
            {
                return this.data;
            }
            set
            {
                if (value > this.Maximum)
                {
                    this.data = this.Maximum;
                }
                else if (value < this.Minimum)
                {
                    this.data = this.Minimum;
                }
                else
                {
                    this.data = value;
                }                
                RaisePropertyChanged("Data");
            }
        }

        #endregion

        #region private field

        private UInt64 rawData = 0;
        private Double data = 0D;

        #endregion

        #region public method 

        /// <summary>
        /// 根据原始数据解析Signal数据
        /// </summary>
        /// <param name="signal">需要解析的signal</param>
        /// <param name="data">原始数据</param>
        /// <param name="valueString">解析值</param>
        /// <returns>是否成功</returns>
        public static Boolean GetDbcData(ref DBCSignal signal, Byte[] data, out String valueString)
        {
            valueString = "";
            UInt16 start;
            UInt64 tempUInt64;
            if (signal.MultiplexerIndicator == MultiplexerIndicator.MultiplexedSignals)
            {
                DBCSignal tempSignal;
                signal.Message.SearchMultiplexerSignalFormMessage(out tempSignal);
                if (tempSignal != null)
                {
                    String tempResult;
                    GetDbcData(ref tempSignal, data, out tempResult);
                    if (tempSignal.Data != signal.MultiplexerSwitchValue)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (signal.ByteOrder == ByteOrder.Motorola)
            {
                start = (UInt16)(63 - (7 - signal.StartPosition % 8) - signal.StartPosition / 8 * 8);
                UInt64 temp = 0;
                for (Int32 i = 0; i < 8; i++)
                {
                    temp <<= 8;
                    temp |= data[i];
                }
                tempUInt64 = temp << (63 - start);
                tempUInt64 = tempUInt64 >> (63 - signal.SignalSize + 1);
            }
            else
            {
                start = signal.StartPosition;
                UInt64 temp = 0;
                for (Int32 i = 8; i > 0; i--)
                {
                    temp <<= 8;
                    temp |= data[i];
                }
                tempUInt64 = temp << (63 - start - signal.SignalSize + 1);
                tempUInt64 = tempUInt64 >> (63 - signal.SignalSize + 1);
            }
            Double value = signal.Factor * tempUInt64 + signal.Offset;
            String tempString = value.ToString();
            Boolean isDecimals = tempString.Contains(".");
            if (!isDecimals)
            {
                UInt32 tempValue = (UInt32)value;
                Int32 size = signal.Values.Count;
                if (size > 0)
                {
                    for (Int32 i = 0; i < size; i++)
                    {
                        foreach (var item in signal.Values)
                        {
                            if (item.Value == tempValue)
                            {
                                valueString = item.Key;
                                signal.Data = item.Value;
                                return true;
                            }
                        }
                    }
                }
            }
            String factorString = signal.Factor.ToString();
            String offsetString = signal.Offset.ToString();
            var precision = ((factorString.Length - factorString.LastIndexOf(".") - 1) >
                            (offsetString.Length - offsetString.LastIndexOf(".") - 1)) ?
                            (factorString.Length - factorString.LastIndexOf(".") - 1) :
                            (offsetString.Length - offsetString.LastIndexOf(".") - 1);
            if (precision == -1)
            {
                precision = 0;
            }
            String formatString = "0.".PadRight(2 + precision, '0');
            valueString = String.Format("{0:" + formatString + "}", value);
            signal.Data = value;
            return true;
        }

        #endregion

    }

}


/*
******************************************************************************                                                                                                                                         
*  File name:        EnumDefine.cs                                                                                                                                                                                                                            
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

using System;

#endregion 

namespace Neusoft.Reach.DBCAnalysis.Infrastructure
{
    /// <summary>
    /// For signals with byte order Intel (little endian) the position of the leastsignificant bit is given. 
    /// For signals with byte order Motorola (big endian) the position of the most significant bit is given.
    /// </summary>
    public enum ByteOrder : SByte
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = -1,

        /// <summary>
        /// big endian
        /// </summary>
        Motorola = 0,

        /// <summary>
        /// little endian
        /// </summary>
        Intel = 1,
    }

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataType : Byte
    {
        /// <summary>
        /// 数据
        /// </summary>
        DataType = 1,

        /// <summary>
        /// 意义
        /// </summary>
        SenseType = 2,
    }

    /// <summary>
    /// The value_type defines the signal as being of type unsigned (-) or signed (-).
    /// </summary>
    public enum ValueType : SByte
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None = -1,

        /// <summary>
        /// 无符号(+)
        /// </summary>
        Unsigned = 0,

        /// <summary>
        /// 有符号(-)
        /// </summary>
        Signed = 1,

    }

    /// <summary>
    /// The multiplexer indicator defines whether the signal is a normal signal,
    /// a multiplexer switch for multiplexed signals, or a multiplexed signal.
    /// </summary>
    public enum MultiplexerIndicator : Byte
    {
        /// <summary>
        ///  normal signal
        /// </summary>
        NormalSignal = 0,

        /// <summary>
        /// A 'M' (uppercase) character defines the signal as the multiplexer switch.
        /// Only one signal within a single message can be the multiplexer switch.
        /// </summary>
        MultiplexedSignal,

        /// <summary>
        /// A 'm' (lowercase) character followed by an unsigned integer
        /// defines the signal as being multiplexed by the multiplexer switch. 
        /// </summary>
        MultiplexedSignals,
    }

    /// <summary>
    /// 帧类型
    /// </summary>
    public enum FrameType : Int32
    {
        /// <summary>
        /// 标准帧
        /// </summary>
        StandardFrame = 0,

        /// <summary>
        /// 扩展帧
        /// </summary>
        ExtendFrame = 1,
    }

    /// <summary>
    /// DBC 文件类型
    /// </summary>
    public enum DBCType : Int32
    {
        /// <summary>
        /// 标准DBC
        /// </summary>
        StandardDBC = 0,

        /// <summary>
        /// eStdDBC
        /// </summary>
        J1939DBC = 1,
    }
}

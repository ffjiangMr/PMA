
/*
******************************************************************************                                                                                                                                         
*  File name:        StructDefine.cs                                                                                                                                                                                                                            
*  Copyright         ReachAuto Corporation. All rights reserved.                                                                                                                              
*  Notes:                                                                                                                              
*  History:                                                                                                                              
*    Revision        Date           Name              Comment                                                              
*    ------------------------------------------------------------------                
*    1.0          2019.04.12        JiangFei           Initial                                                                      
*                                                                                                                                          
******************************************************************************
*/

#region  using directive

using System;
using System.Runtime.InteropServices;

#endregion

namespace Neusoft.Reach.CANComponent.Infrastructure
{
    /// <summary>
    /// 硬件设置
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DeviceSetting
    {
        /// <summary>
        /// 硬件设备名字
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Byte[] DeviceName;

        /// <summary>
        /// 硬件端口号
        /// </summary>
        public CANPort DevicePort;

        /// <summary>
        /// 硬件设备索引号
        /// 在当前上位机所连接设备中的索引
        /// 以 0 开始
        /// </summary>
        public Int32 HardwareIndex;

        /// <summary>
        /// 波特率
        /// 单位 Band
        /// </summary>
        public Int32 BaudRate;
    };

    /// <summary>
    /// 发送报文结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SendObject
    {
        /// <summary>
        /// 发送ID
        /// </summary>
        public UInt32 SendID;

        /// <summary>
        /// 长度
        /// </summary>
        public UInt16 Length;

        /// <summary>
        /// 发送实体
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] SendContent;

        /// <summary>
        /// 帧类型
        /// </summary>
        public FrameType FrameType;
    };

    /// <summary>
    /// 接收报文结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ReceiveObject
    {
        /// <summary>
        /// 帧类型
        /// </summary>
        public FrameType FrameType;

        /// <summary>
        /// 接收ID
        /// </summary>
        public UInt32 ReceiveID;

        /// <summary>
        /// 时间戳
        /// 0.1ms级
        /// </summary>
        public UInt64 TimeStamp;

        /// <summary>
        /// 消息长度
        /// </summary>
        public UInt16 Length;

        /// <summary>
        /// 消息实体
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] ReceiveData;

        /// <summary>
        /// 是否为自己发送的消息
        /// </summary>
        public Boolean IsSelfSend;

    };

    /// <summary>
    /// 过滤器结构体
    /// </summary>
    public struct Filter
    {
        /// <summary>
        /// 帧类型
        /// </summary>
        public FrameType FrameType;

        /// <summary>
        /// ID
        /// </summary>
        public UInt32 ID;
    };
}

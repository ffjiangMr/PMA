
/*
******************************************************************************                                                                                                                                         
*  File name:        EnumDefine.cs                                                                                                                                                                                                                            
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

using Neusoft.Reach.CANComponent.CANInterface;
using Neusoft.Reach.CANComponent.Infrastructure;
using Neusoft.Reach.CANComponent.Model;

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Neusoft.Reach.CANComponent.Handle
{
    /// <summary>
    /// 与CAN通信相关实现
    /// </summary>
    public sealed class CanInteractionHandler
    {
        /// <summary>
        /// 接收Message事件Delegate
        /// </summary>
        /// <param name="message">接收的消息</param>
        /// <param name="channelName">通道Name</param>
        public delegate void ReceiveMessageDelegate(String channelName, ReceiveObject message);

        /// <summary>
        /// 接收Message触发事件
        /// </summary>
        public static event ReceiveMessageDelegate receiveMessageEvent;

        /// <summary>
        /// 触发接收Message事件
        /// </summary>
        /// <param name="message">接收到的Message</param>
        /// <param name="channelName">通道Name</param>
        private static void OnReceiveMessageEvent(String channelName, ReceiveObject message)
        {
            var tempEvent = receiveMessageEvent;
            if (tempEvent != null)
            {
                tempEvent(channelName, message);
            }
        }


        /// <summary>
        /// 获取驱动库所支持的CAN Device设备数量
        /// </summary>
        /// <returns>支持的设备数量</returns>
        public static Int32 GetSupportedDeviceTypeCount()
        {
            return CanInteraction.CANGetDeviceNumSupported();
        }

        /// <summary>
        /// 获取驱动库所支持的硬件设备的具体名称
        /// </summary>
        /// <returns>设备名称列表</returns>
        public static List<String> GetSupportedDeviceNames()
        {
            List<String> result = new List<String>();
            unsafe
            {
                //与C++组件交互固定内存
                //防止垃圾回收移动数据
                fixed (Byte* buffer = new Byte[32, 64])
                {
                    Byte*[] nameArray = new Byte*[32];
                    for (Int32 flag = 0; flag < nameArray.Length; flag++)
                    {
                        nameArray[flag] = buffer + flag * 64;
                    }
                    var deviceCount = GetSupportedDeviceTypeCount();
                    CanInteraction.CANGetDeviceNameSupported(nameArray);
                    Byte[][] deviceNames = new Byte[deviceCount][];
                    for (Int32 type = 0; type < deviceCount; type++)
                    {
                        deviceNames[type] = new Byte[64];
                        for (int flag = 0; flag < 64; flag++)
                        {
                            deviceNames[type][flag] = *(nameArray[type]);
                            if (deviceNames[type][flag] == 0)
                            {
                                break;
                            }
                            nameArray[type]++;
                        }
                        String name = Encoding.ASCII.GetString(deviceNames[type]);
                        name = name.TrimEnd(new Char[] { '\0' });
                        result.Add(name);
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 为最先调用函数
        /// </summary>
        /// <param name="settings">通道设置列表</param>
        public static void CANSetDeviceInfo(ref List<DeviceInfoModel> settings)
        {
            var infos = new List<DeviceSetting>();
            foreach (var setting in settings)
            {
                var info = new DeviceSetting();
                info.BaudRate = setting.BaudRate * 1000;
                info.HardwareIndex = setting.HardwareIndex;
                info.DeviceName = Encoding.Default.GetBytes(setting.DeviceName.PadLeft(32, '0'));
                info.DevicePort = (CANPort)setting.DevicePort;
                infos.Add(info);
            }
            CanInteraction.CANSetDeviceInfo(infos.Count, infos.ToArray());
        }

        /// <summary>
        /// 获取驱动所支持指定设备的通道个数
        /// 不支持的返回0
        /// </summary>
        /// <param name="name">device name</param>
        /// <returns>支持数量</returns>
        public static Int32 CANGetDeviceSupportedChannelCount(ref String name)
        {
            return CanInteraction.CANGetDeviceChannelNun(name); ;
        }

        /// <summary>
        /// 显示错误信息对话框
        /// </summary>
        /// <param name="index">通道索引(从 0 开始)</param>
        /// <param name="error">错误码</param>
        public static void ShowErrorDialog(ref Int32 index, ref CANError error)
        {
            CanInteraction.CANError(index, error);
        }

        /// <summary>
        /// 关闭通道
        /// </summary>
        /// <param name="index">通道索引(基于0)</param>
        /// <returns>返回关闭码</returns>
        public static CANError CloseCANChannel(ref Int32 index)
        {
            return CanInteraction.CANClose(index);
        }

        /// <summary>
        /// 根据通道索引号打开设备
        /// 打开前需要配置通道
        /// </summary>
        /// <param name="index">通道索引(基于0)</param>
        /// <returns>状态码</returns>
        public static CANError OpenCANChannelDevice(ref Int32 index)
        {
            return CanInteraction.OpenChan(index);
        }

        /// <summary>
        /// 根据通道索引号打开通道
        /// 打开前需要先开启设备
        /// </summary>
        /// <param name="index">通道索引(基于0)</param>
        /// <returns>状态码</returns>
        public static CANError StartCANChannel(ref Int32 index)
        {
            return CanInteraction.StartChan(index);
        }

        /// <summary>
        /// 根据给定通道索引号
        /// 获取通道message
        /// </summary>
        /// <param name="index">通道索引值(基于0)</param>
        /// <param name="channelName">通道Name</param>
        /// <param name="message">接收到的消息</param>
        /// <returns>状态码</returns>
        public static CANError GetSingleChannelMessage(ref Int32 index, ref String channelName, out ReceiveObject message)
        {
            message = new ReceiveObject();
            var result = CanInteraction.CANRcvMsg(index, ref message);
            if (result == CANError.ErrNon)
            {
                OnReceiveMessageEvent(channelName, message);
            }
            return result;
        }

        /// <summary>
        /// 获取多个通道的Message
        /// 如出现错误则接收Message 为 null
        /// </summary>
        /// <param name="channelNames">通道Name列表</param>
        /// <param name="message">接收到的信息列表</param>
        /// <returns>状态码</returns>
        public static CANError GetMultiChannelMessage(ref List<String> channelNames, out List<ReceiveObject> message)
        {
            var result = CANError.ErrNon;
            message = new List<ReceiveObject>();
            for (Int32 flag = 0; flag < channelNames.Count; flag++)
            {
                ReceiveObject receive = new ReceiveObject();
                result = CanInteraction.CANRcvMsg(flag, ref receive);
                if (result == CANError.ErrNon)
                {
                    message.Add(receive);
                    OnReceiveMessageEvent(channelNames[flag], receive);
                }
                else if (result != CANError.Err_RcvEmpty)
                {
                    message = null;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据通道索引发送Message
        /// </summary>
        /// <param name="Index">通道索(基于0)</param>
        /// <param name="message">要发送的消息结构体</param>
        /// <returns>状态码</returns>
        public static CANError SendChannelMessage(ref Int32 Index, ref SendObject message)
        {
            return CanInteraction.CANSndMsg(Index, message);
        }
    }
}

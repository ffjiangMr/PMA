
/*
******************************************************************************                                                                                                                                         
*  File name:        DeviceInfoModel.cs                                                                                                                                                                                                                            
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

using Neusoft.Reach.CANComponent.Infrastructure;

using System;

#endregion

namespace Neusoft.Reach.CANComponent.Model
{
    /// <summary>
    /// 设置Device 信息model
    /// </summary>
    public sealed class DeviceInfoModel
    {
        /// <summary>
        /// 硬件设备名字
        /// </summary>        
        public String DeviceName;

        /// <summary>
        /// 硬件端口号
        /// </summary>
        public Int32 DevicePort;

        /// <summary>
        /// 硬件设备索引号
        /// 在当前上位机所连接设备中的索引
        /// 以 0 开始
        /// </summary>
        public Int32 HardwareIndex;

        /// <summary>
        /// 波特率
        /// 单位 KBand
        /// </summary>
        public Int32 BaudRate;
    }
}

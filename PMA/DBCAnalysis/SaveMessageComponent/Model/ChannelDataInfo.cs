
/*
******************************************************************************                                                                                                                                         
*  File name:        ChannelDataInfo.cs                                                                                                                                                                                                                            
*  Copyright         ReachAuto Corporation. All rights reserved.                                                                                                                              
*  Notes:                                                                                                                              
*  History:                                                                                                                              
*    Revision        Date           Name              Comment                                                              
*    ------------------------------------------------------------------                
*    1.0          2019.04.18        JiangFei           Initial                                                                      
*                                                                                                                                          
******************************************************************************
*/

#region  using directive

using Neusoft.Reach.CANComponent.Infrastructure;

using System;

#endregion

namespace Neusoft.Reach.SaveComponent.Model
{
    /// <summary>
    /// 数据库实体信息
    /// </summary>
    public sealed class ChannelDataInfo
    {
        #region property

        /// <summary>
        /// 通道名称
        /// </summary>
        public String ChannelName { get; set; }

        /// <summary>
        /// 接收的消息
        /// </summary>
        public ReceiveObject RecevieMessage { get; set; } = new ReceiveObject();

        #endregion 
    }
}

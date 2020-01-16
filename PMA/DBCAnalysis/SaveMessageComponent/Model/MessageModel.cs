
/*
******************************************************************************                                                                                                                                         
*  File name:        MessageModel.cs                                                                                                                                                                                                                            
*  Copyright         ReachAuto Corporation. All rights reserved.                                                                                                                              
*  Notes:                                                                                                                              
*  History:                                                                                                                              
*    Revision        Date           Name              Comment                                                              
*    ------------------------------------------------------------------                
*    1.0          2019.04.28        JiangFei           Initial                                                                      
*                                                                                                                                          
******************************************************************************
*/

#region  using directive

using Neusoft.Reach.CANComponent.Infrastructure;

using System;

#endregion

namespace Neusoft.Reach.SaveComponent.Model
{
    internal class MessageModel
    {
        public ReceiveObject Message { get; set; }
        public String ChannelName { get; set; }
    }
}

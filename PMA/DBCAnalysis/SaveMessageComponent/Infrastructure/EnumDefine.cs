
/*
******************************************************************************                                                                                                                                         
*  File name:        EnumDefine.cs                                                                                                                                                                                                                            
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

using System;

#endregion

namespace Neusoft.Reach.SaveComponent.Infrastructure
{
    /// <summary>
    /// 保存限制模式
    /// </summary>
    public enum LimitMode : Byte
    {
        /// <summary>
        /// 行数限制
        /// </summary>
        LineMode = 0,

        /// <summary>
        /// 大小限制
        /// </summary>
        SizeMode = 1,
    }
}

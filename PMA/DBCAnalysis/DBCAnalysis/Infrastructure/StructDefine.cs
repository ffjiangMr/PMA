
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

using System;
using System.Runtime.InteropServices;

#endregion

namespace Neusoft.Reach.DBCAnalysis.Infrastructure
{
    /// <summary>
    /// 用于解析报文原始数据
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ByteArrayToInt64
    {
        /// <summary>
        /// 报文原始数据
        /// </summary>
        [FieldOffset(0)]

        public fixed Byte Data[8];

        /// <summary>
        /// 原始数据64位表示形式
        /// </summary>
        [FieldOffset(0)]
        public UInt64 Data64;
    }
}

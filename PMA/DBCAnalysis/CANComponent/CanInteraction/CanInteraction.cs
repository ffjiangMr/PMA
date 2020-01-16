
/*
******************************************************************************                                                                                                                                         
*  File name:        CanInterface.cs                                                                                                                                                                                                                            
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
using System.Runtime.InteropServices;

#endregion

namespace Neusoft.Reach.CANComponent.CANInterface
{
    internal sealed class CanInteraction
    {
        #region dll import

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "OpenChan", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError OpenChan(Int32 index);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "StartChan", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError StartChan(Int32 index);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANRcvMsg", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANRcvMsg(Int32 index, ref ReceiveObject message);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANSndMsg", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANSndMsg(Int32 nIndex, SendObject message);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANClose", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANClose(Int32 index);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANSetFilter", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANSetFilter(Int32 index, Filter[] FilterArray, Int32 FilterNumber);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANGetTotleFilterNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANGetTotleFilterNum(Int32 index);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANReOpenFilter", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANReOpenFilter(Int32 index);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANClearReceiveBuf", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANClearReceiveBuf(Int32 index);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANGetDeviceNameSupported", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void CANGetDeviceNameSupported(Byte*[] nameArray);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANGetDeviceNumSupported", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 CANGetDeviceNumSupported();

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANResetDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern CANError CANResetDevice(Int32 index);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANGetDeviceChannelNun", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 CANGetDeviceChannelNun(String name);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANGetReceiveQueueLevel", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 CANGetReceiveQueueLevel(Int32 index);
        
        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANSetDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CANSetDeviceInfo(Int32 size, DeviceSetting[] setting);

        [DllImport(@".\CAN drive library\CanInterface.dll", EntryPoint = "CANError", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CANError(Int32 index, CANError error);       

        #endregion
    }
}

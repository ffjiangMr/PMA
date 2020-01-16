
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

using System;

#endregion

namespace Neusoft.Reach.CANComponent.Infrastructure
{
    /// <summary>
    /// 定义CAN通信错误码
    /// </summary>
    public enum CANError : Int32
    {
        /// <summary>
        /// 无异常
        /// </summary>
        ErrNon,

        /// <summary>
        /// 接收信息为空
        /// </summary>
        Err_RcvEmpty,

        /// <summary>
        /// connect the hard device of ZLG fail
        /// </summary>
        Err_ZLGCon,

        /// <summary>
        /// connect the hard device of Vector fail
        /// </summary>
        Err_VecCon,

        /// <summary>
        /// connect the hard device of PCan fail
        /// </summary>
        Err_PCaCon,

        /// <summary>
        /// connect the hard device of ValueCAN fail
        /// </summary>
        Err_ValCon,

        /// <summary>
        /// select the device fail
        /// </summary>
        Err_DevSel,

        /// <summary>
        /// select the filter fail
        /// </summary>
        Err_FltSel,

        /// <summary>
        /// baud setting fail
        /// </summary>
        Err_ZLG_BauSet,

        /// <summary>
        /// ZLG CAN start fail
        /// </summary>
        Err_ZLG_CanSta,

        /// <summary>
        /// ZLG send data fail
        /// </summary>
        Err_ZLG_SndDat,

        /// <summary>
        /// ZLG recieve data fail
        /// </summary>
        Err_ZLG_RcvDat,

        /// <summary>
        /// init ZLG fail
        /// </summary>
        Err_ZLGIni,

        /// <summary>
        /// init Vector
        /// </summary>
        Err_VecIni,

        /// <summary>
        /// vector applycation setting fail
        /// </summary>
        Err_Vec_AppSet,

        /// <summary>
        /// vector open port fail
        /// </summary>
        Err_Ver_OpePor,

        /// <summary>
        /// vector request Chip State fail
        /// </summary>
        Err_Ver_ReqSta,

        /// <summary>
        /// vector baud rate setting fail
        /// </summary>
        Err_Ver_BauSet,

        /// <summary>
        /// vector set filter fail
        /// </summary>
        Err_Ver_FltSet,

        /// <summary>
        /// vector activate channel fail
        /// </summary>
        Err_Ver_ActCha,

        /// <summary>
        /// vector set notification fail
        /// </summary>
        Err_Ver_SetNot,

        /// <summary>
        /// vector reset clock fail
        /// </summary>
        Err_Ver_ResClo,

        /// <summary>
        /// vector send data fail
        /// </summary>
        Err_Ver_SndDat,

        /// <summary>
        /// vector recieve data fail(Error Frame)
        /// </summary>
        Err_Ver_ErrFrm,

        /// <summary>
        /// vector recieve data fail
        /// </summary>
        Err_Ver_RcvDat,

        /// <summary>
        /// PCAN recieve data fail
        /// </summary>
        Err_PCAN_RcvDat,

        /// <summary>
        /// PCAN send data fail
        /// </summary>
        Err_PCAN_SndDat,

        /// <summary>
        /// valueCAN recieve data fail
        /// </summary>
        Err_ValCAN_RcvDat,

        /// <summary>
        /// valueCAN send data fail
        /// </summary>
        Err_ValCAN_SndDat,
    };

    /// <summary>
    /// 硬件端口
    /// </summary>
    public enum CANPort : Int32
    {
        /// <summary>
        /// 端口一
        /// </summary>
        PortOne = 1,

        /// <summary>
        /// 端口二
        /// </summary>
        PortTwo,

        /// <summary>
        /// 端口三
        /// </summary>
        PortThree,

        /// <summary>
        /// 端口四
        /// </summary>
        PortFour
    };

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
}


/*
******************************************************************************                                                                                                                                         
*  File name:        ViewModel.cs                                                                                                                                                                                                                            
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
using System.ComponentModel;

#endregion

namespace Neusoft.Reach.DBCAnalysis.Model
{
    /// <summary>
    /// 实现INotifyPropertyChanged    
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {

        #region 实现 INotifyPropertyChanged

        /// <summary>
        /// 属性值发生变化时发起通知
        /// </summary>
        /// <param name="propertyName"></param>
        public void RaisePropertyChanged(String propertyName)
        {
            var temp = PropertyChanged;
            if (temp != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

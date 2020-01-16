
/*
******************************************************************************                                                                                                                                         
*  File name:        DBCNode.cs                                                                                                                                                                                                                            
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
using System.Collections.Generic;

#endregion 

namespace Neusoft.Reach.DBCAnalysis.Model
{
    /// <summary>
    /// The node section defines the names of all participating nodes 
    /// The names definedin this section have to be unique within this section.
    /// </summary>
    public sealed class DBCNode : ViewModel
    {
        #region property define

        /// <summary>
        /// The names definedin this section have to be unique within this section.
        /// </summary>
        public String NodeName
        {
            get
            {
                return this.nodeName;
            }
            set
            {
                this.nodeName = value;
                RaisePropertyChanged("NodeName");
            }
        }

        /// <summary>
        /// 节点地址
        /// </summary>
        public UInt32 NodeAddress
        {
            get
            {
                return this.nodeAddress;
            }
            set
            {
                this.nodeAddress = value;
                RaisePropertyChanged("NodeAddress");
            }
        }

        /// <summary>
        /// 节点包含的message 列表
        /// </summary>
        public List<DBCMessage> Messages { get; } = new List<DBCMessage>();

        #endregion

        #region private field

        private String nodeName;
        private UInt32 nodeAddress;

        #endregion 
    }
}

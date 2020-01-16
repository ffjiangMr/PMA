
/*
******************************************************************************                                                                                                                                         
*  File name:        ConstDefine.cs                                                                                                                                                                                                                            
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
    /// 定义常量
    /// </summary>
    public sealed class ConstDefine
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public static readonly String TableName = "message";

        /// <summary>
        /// 检查数据库表是否存在
        /// </summary>
        public static readonly String TableExistQuery = "SELECT COUNT(*) FROM sqlite_master where type='table' and name= @name;";

        /// <summary>
        /// 创建表
        /// </summary>
        public static readonly String CreateTable = $"CREATE TABLE IF NOT EXISTS {TableName}(id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,FrameType INT8, ReceiveID INTEGER, TimeStamp INT64, Length SMALLINT, data1 INT8, data2 INT8, data3 INT8, data4 INT8, data5 INT8, data6 INT8, data7 INT8, data8 INT8, IsSelfSend BOOLEAN, ChannelName VARCHAR(50));";

        /// <summary>
        /// 插入数据
        /// </summary>
        public static readonly String InsertData = $"INSERT INTO {TableName}(FrameType, ReceiveID, TimeStamp, Length, data1, data2, data3, data4, data5, data6, data7, data8, IsSelfSend, ChannelName)values(@FrameType,@ReceiveID,@TimeStamp,@Length,@data1,@data2,@data3,@data4,@data5,@data6,@data7,@data8,@IsSelfSend,@ChannelName);";

        /// <summary>
        /// 查询记录数量
        /// </summary>
        public static readonly String QueryRecordCount = $"SELECT COUNT(*) FROM {TableName};";

        /// <summary>
        /// 删除全部数据库记录
        /// </summary>
        public static readonly String DeleteAllRecord = $"DELETE FROM {TableName};VACUUM;";

        /// <summary>
        /// 删除表
        /// </summary>
        public static readonly String DeleteTable = $"DROP TABLE IF EXISTS {TableName};VACUUM;";

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public static readonly String FileExtension = ".csv";

        /// <summary>
        /// 文件头
        /// </summary>
        public static readonly String FileHeader = "ChannelName,FrameType,ReceiveID,TimeStamp,DLC,data1(H),data2(H),data3(H),data4(H),data5(H),data6(H),data7(H),data8(H),bTx";

       /// <summary>
       ///  文件大小单位M
       /// </summary>
        public static readonly Int64 M = 1024 * 1024;


    }
}

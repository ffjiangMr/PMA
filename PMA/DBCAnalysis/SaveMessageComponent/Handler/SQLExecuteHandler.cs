
/*
******************************************************************************                                                                                                                                         
*  File name:        SQLExecuteHandler.cs                                                                                                                                                                                                                            
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
using Neusoft.Reach.SaveComponent.Model;

using System;
using System.Collections.Generic;
using System.Data.SQLite;

#endregion

namespace Neusoft.Reach.SaveComponent.Handler
{
    /// <summary>
    /// 执行SQL 语句
    /// </summary>
    public sealed class SQLExecuteHandler
    {
        /// <summary>
        /// 操作数据库时加锁
        /// </summary>
        private static Object dbLock = new object();

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="connect">数据库连接实体</param>
        /// <param name="sql">查询语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>查询结果（数据量）</returns>
        public static Int32 QueryCount(ref SQLiteConnection connect, ref String sql, SQLiteParameter[] parameters = null)
        {
            Int32 result = 0;
            lock (dbLock)
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    using (var command = connect.CreateCommand())
                    {
                        command.CommandText = sql;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read() == true)
                            {
                                result = reader.GetInt32(0);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 执行SQL语句 
        /// 返回受影响行数
        /// </summary>
        /// <param name="connect">数据库连接</param>
        /// <param name="sql">SQL 语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>受影响行数</returns>
        public static Int32 ExecuteNonQuery(ref SQLiteConnection connect, ref String sql, SQLiteParameter[] parameters = null)
        {
            var result = 0;
            lock (dbLock)
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    using (var command = connect.CreateCommand())
                    {
                        command.CommandText = sql;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        result = command.ExecuteNonQuery();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="connect">数据库连接实体</param>
        /// <param name="sql">查询语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>查询结果列表</returns>
        public static List<ChannelDataInfo> QueryData(ref SQLiteConnection connect, ref String sql, SQLiteParameter[] parameters = null)
        {
            var result = new List<ChannelDataInfo>();
            lock (dbLock)
            {
                if (connect.State == System.Data.ConnectionState.Open)
                {
                    using (var command = connect.CreateCommand())
                    {
                        command.CommandText = sql;
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var entity = new ChannelDataInfo();
                                entity.ChannelName = (String)reader["ChannelName"];
                                ReceiveObject message = new ReceiveObject();
                                message.FrameType = (FrameType)reader["FrameType"];
                                message.ReceiveID = (UInt32)reader["ReceiveID"];
                                message.TimeStamp = (UInt64)reader["TimeStamp"];
                                message.Length = (UInt16)reader["Length"];
                                message.IsSelfSend = (Boolean)reader["IsSelfSend"];
                                message.ReceiveData[0] = (Byte)reader["data1"];
                                message.ReceiveData[1] = (Byte)reader["data2"];
                                message.ReceiveData[2] = (Byte)reader["data3"];
                                message.ReceiveData[3] = (Byte)reader["data4"];
                                message.ReceiveData[4] = (Byte)reader["data5"];
                                message.ReceiveData[5] = (Byte)reader["data6"];
                                message.ReceiveData[6] = (Byte)reader["data7"];
                                message.ReceiveData[7] = (Byte)reader["data8"];
                                entity.RecevieMessage = message;
                                result.Add(entity);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}


/*
******************************************************************************                                                                                                                                         
*  File name:        DBHandler.cs                                                                                                                                                                                                                            
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

using Neusoft.Reach.CANComponent.Handle;
using Neusoft.Reach.CANComponent.Infrastructure;
using Neusoft.Reach.SaveComponent.Infrastructure;
using Neusoft.Reach.SaveComponent.Model;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;

#endregion

namespace Neusoft.Reach.SaveComponent.Handler
{
    /// <summary>
    /// DB操作类
    /// </summary>
    public sealed class DBHandler : IDisposable
    {
        #region 单例

        /// <summary>
        /// 单例实体
        /// </summary>
        private static DBHandler entity;

        private static Object queueLock = new Object();

        /// <summary>
        /// 获取单例实体
        /// </summary>
        /// <returns></returns>
        public static DBHandler Instance()
        {
            if (entity == null)
            {
                entity = new DBHandler();
            }
            return entity;
        }

        /// <summary>
        /// 私有构造函数
        /// 以实现单例
        /// </summary>
        private DBHandler()
        {
            String directorySetting = ConfigurationManager.AppSettings["DBDirectory"];
            String nameSetting = ConfigurationManager.AppSettings["DBName"];
            this.DBPath = directorySetting == null ? @".\DB" : directorySetting;
            this.DBName = nameSetting == null ? "MsgData.db" : nameSetting;
            this.Init();
        }

        #endregion

        #region private field

        private SQLiteConnection connection;
        private Queue<MessageModel> messageQueue = new Queue<MessageModel>();
        private Boolean isDisposed = false;
        private Dictionary<String, DateTime> lastInsertTime = new Dictionary<String, DateTime>();

        #endregion

        #region property 

        /// <summary>
        /// 数据库文件路径
        /// </summary>
        public String DBPath { get; }

        /// <summary>
        /// 数据库文件名称
        /// </summary>
        public String DBName { get; }

        /// <summary>
        /// 数据记录条数
        /// </summary>
        public Int32 DBRecordCount
        {
            get
            {
                var sql = ConstDefine.QueryRecordCount;
                return SQLExecuteHandler.QueryCount(ref this.connection, ref sql);
            }
        }

        #endregion

        #region public method

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="message">待插入消息</param>
        /// <returns>是否成功</returns>
        public Boolean InsertMessageData(String channelName, ReceiveObject message)
        {
            lock (queueLock)
            {
                Boolean result = true;
                if (this.lastInsertTime.ContainsKey(message.ReceiveID.ToString()) == false)
                {
                    this.lastInsertTime[message.ReceiveID.ToString()] = DateTime.MinValue;
                }
                if (DateTime.Now < lastInsertTime[message.ReceiveID.ToString()].AddMilliseconds(100))
                {
                    result = false;
                }
                else
                {
                    if (this.isDisposed == false)
                    {
                        messageQueue.Enqueue(new MessageModel()
                        {
                            Message = message,
                            ChannelName = channelName,
                        });
                    }
                    if ((messageQueue.Count > 100) || this.isDisposed)
                    {
                        var tempBuffer = messageQueue;
                        messageQueue = new Queue<MessageModel>();
                        foreach (var item in tempBuffer)
                        {
                            var sql = ConstDefine.InsertData;
                            SQLiteParameter[] parameters = new SQLiteParameter[]
                            {
                    new SQLiteParameter("@FrameType",item.Message.FrameType),
                    new SQLiteParameter("@ReceiveID",item.Message.ReceiveID),
                    new SQLiteParameter("@TimeStamp",item.Message.TimeStamp),
                    new SQLiteParameter("@Length",item.Message.Length),
                    new SQLiteParameter("@data1",item.Message.ReceiveData[0]),
                    new SQLiteParameter("@data2",item.Message.ReceiveData[1]),
                    new SQLiteParameter("@data3",item.Message.ReceiveData[2]),
                    new SQLiteParameter("@data4",item.Message.ReceiveData[3]),
                    new SQLiteParameter("@data5",item.Message.ReceiveData[4]),
                    new SQLiteParameter("@data6",item.Message.ReceiveData[5]),
                    new SQLiteParameter("@data7",item.Message.ReceiveData[6]),
                    new SQLiteParameter("@data8",item.Message.ReceiveData[7]),
                    new SQLiteParameter("@IsSelfSend",item.Message.IsSelfSend),
                    new SQLiteParameter("@ChannelName",item.ChannelName)};
                            var effectLines = SQLExecuteHandler.ExecuteNonQuery(ref this.connection, ref sql, parameters);
                            if (effectLines < 0)
                            {
                                result = false;
                            }
                        }
                        tempBuffer.Clear();
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 删除全部记录        
        /// </summary>
        /// <returns>返回受影响行数</returns>
        public Int32 DeleteAllRecord()
        {
            Int32 result = 0;
            var sql = ConstDefine.DeleteAllRecord;
            result = SQLExecuteHandler.ExecuteNonQuery(ref this.connection, ref sql);
            return result;
        }

        /// <summary>
        /// 删除表
        /// </summary>
        public void DeleteTable()
        {
            var sql = ConstDefine.DeleteTable;
            SQLExecuteHandler.ExecuteNonQuery(ref this.connection, ref sql);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="page">要显示的页数</param>
        /// <param name="size">每页需要显示的数据量</param>
        /// <returns>查询结果列表</returns>
        public List<ChannelDataInfo> QueryDataList(Int32 page, Int32 size = 1000)
        {
            var result = new List<ChannelDataInfo>();
            var sql = $"SELECT * FROM {ConstDefine.TableName} order by id limit {size} offset {page}*{size}";
            result = SQLExecuteHandler.QueryData(ref this.connection, ref sql);
            return result;
        }

        /// <summary>
        /// 释放数据库资源
        /// </summary>
        public void Dispose()
        {
            this.isDisposed = true;
            this.InsertMessageData("", new ReceiveObject());
            if ((this.connection != null) && (this.connection.State != System.Data.ConnectionState.Closed))
            {
                this.connection.Dispose();
            }
            CanInteractionHandler.receiveMessageEvent -= this.OnReceiveEvent;
        }

        #endregion

        #region private method

        /// <summary>
        /// 初始化数据库连接
        /// </summary>
        private void Init()
        {
            SQLiteConnectionStringBuilder connectStringBuilder = new SQLiteConnectionStringBuilder();
            connectStringBuilder.DataSource = Path.Combine(this.DBPath, this.DBName);
            if (!Directory.Exists(this.DBPath))
            {
                Directory.CreateDirectory(this.DBPath);
            }
            this.connection = new SQLiteConnection(connectStringBuilder.ToString());
            this.connection.Open();
            if (this.TableExist() == false)
            {
                this.CreateTable();
            }
            CanInteractionHandler.receiveMessageEvent += this.OnReceiveEvent;
        }

        /// <summary>
        /// 处理接收数据事件
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="message">收到的消息</param>
        private void OnReceiveEvent(String channelName, ReceiveObject message)
        {
            this.InsertMessageData(channelName, message);
        }

        /// <summary>
        /// 创建表
        /// </summary>
        private void CreateTable()
        {
            var sql = ConstDefine.CreateTable;
            SQLExecuteHandler.ExecuteNonQuery(ref this.connection, ref sql);
        }

        /// <summary>
        /// 判断指定表是否存在
        /// </summary>
        /// <returns>判断结果</returns>
        private Boolean TableExist()
        {
            Boolean result = false;
            var sql = ConstDefine.TableExistQuery;
            var parameterArray = new SQLiteParameter[]
            {
                 new SQLiteParameter("name", ConstDefine.TableName)
            };
            if (SQLExecuteHandler.QueryCount(ref this.connection, ref sql, parameterArray) > 0)
            {
                result = true;
            }
            return result;
        }

        #endregion

    }
}

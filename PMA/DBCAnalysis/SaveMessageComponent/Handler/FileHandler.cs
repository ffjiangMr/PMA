
/*
******************************************************************************                                                                                                                                         
*  File name:        FileHandler.cs                                                                                                                                                                                                                            
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
using System.IO;
using System.Text;

#endregion

namespace Neusoft.Reach.SaveComponent.Handler
{
    /// <summary>
    /// 处理文件
    /// </summary>
    public sealed class FileHandler : IDisposable
    {
        #region 单例

        private static FileHandler instance = null;
        private static Object recordLock = new Object();

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        public static FileHandler Instance()
        {
            if (instance == null)
            {
                instance = new FileHandler();
            }
            return instance;
        }

        private FileHandler()
        {
            this.directory = @".\MainData";
            this.fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.Init();
        }

        #endregion

        #region public method      

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.isDisposed = true;
            CanInteractionHandler.receiveMessageEvent -= this.RecordMessage;
            this.RecordMessage("", new ReceiveObject());
        }

        /// <summary>
        /// 开始记录
        /// </summary>
        public void Start()
        {
            lock (recordLock)
            {
                this.lastRecordTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 停止记录
        /// </summary>
        public void Stop()
        {
            lock (recordLock)
            {
                this.lastRecordTime = DateTime.MaxValue;
            }
        }

        #endregion

        #region property

        /// <summary>
        /// 文件保存路径
        /// </summary>
        public String Directory
        {
            get
            {
                return this.directory;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    this.directory = value;
                }
            }
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public String FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    this.fileName = value;
                }
            }
        }

        /// <summary>
        /// 保存行数阈值
        /// </summary>
        public Int32 LineCountLimit
        {
            get
            {
                return this.lineCountLimit;
            }
            set
            {
                if (value > 1)
                {
                    this.lineCountLimit = value;
                }
            }
        }

        /// <summary>
        /// 文件大小阈值 M
        /// </summary>
        public UInt32 FileSizeLimit { get; set; } = 10;

        /// <summary>
        /// 设置限制模式
        /// </summary>
        public LimitMode Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.mode = value;
            }
        }

        /// <summary>
        /// 周期间隔ms
        /// </summary>
        public Int32 TimeSpan { get; set; } = 100;

        /// <summary>
        /// 是否开始记录
        /// </summary>
        public Boolean IsStart { get { return this.lastRecordTime != DateTime.MaxValue; } }

        #endregion

        #region private field

        private DateTime lastRecordTime = DateTime.MaxValue;
        private LimitMode mode = LimitMode.SizeMode;
        private Int32 recordLines = 0;
        private Queue<MessageModel> messageQueue = new Queue<MessageModel>();
        private Boolean isDisposed = false;
        private String directory;
        private String fileName;
        private Int32 lineCountLimit = 1000;

        #endregion

        #region private method

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (System.IO.Directory.Exists(this.Directory) == false)
            {
                System.IO.Directory.CreateDirectory(this.Directory);
            }
            CanInteractionHandler.receiveMessageEvent += this.RecordMessage;
        }

        /// <summary>
        /// 格式化接收的Message
        /// </summary>
        /// <param name="channel">通道名</param>
        /// <param name="message">接收的消息</param>
        /// <returns>格式化字符串</returns>
        private String FormateData(String channel, ReceiveObject message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(channel + ",");
            builder.Append(message.FrameType.ToString() + ",");
            builder.Append(message.ReceiveID.ToString() + ",");
            builder.Append((message.TimeStamp * 1.0 / 10000).ToString() + ",");
            builder.Append(message.Length.ToString() + ",");
            builder.Append(message.ReceiveData[0].ToString("X2") + ",");
            builder.Append(message.ReceiveData[1].ToString("X2") + ",");
            builder.Append(message.ReceiveData[2].ToString("X2") + ",");
            builder.Append(message.ReceiveData[3].ToString("X2") + ",");
            builder.Append(message.ReceiveData[4].ToString("X2") + ",");
            builder.Append(message.ReceiveData[5].ToString("X2") + ",");
            builder.Append(message.ReceiveData[6].ToString("X2") + ",");
            builder.Append(message.ReceiveData[7].ToString("X2") + ",");
            if (message.IsSelfSend)
            {
                builder.Append("Tx,");
            }
            else
            {
                builder.Append("Rx,");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        private void RenameFile(ref String filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var index = filePath.LastIndexOf(".");
            filePath = filePath.Insert(index, "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff"));
            fileInfo.MoveTo(filePath);
            filePath = Path.Combine(this.Directory, DateTime.Now.ToString("yyyyMMddHHmmssffff") + ConstDefine.FileExtension);
        }

        /// <summary>
        /// 记录接收的数据
        /// </summary>
        /// <param name="channelName">通道名</param>
        /// <param name="message">接收的消息</param>
        private void RecordMessage(String channelName, ReceiveObject message)
        {
            lock (recordLock)
            {
                if (((DateTime.Now - lastRecordTime).Milliseconds >= this.TimeSpan) || this.isDisposed)
                {
                    var newFile = false;
                    var filePath = Path.Combine(this.Directory, this.FileName + ConstDefine.FileExtension);
                    if (System.IO.Directory.Exists(this.Directory) == false)
                    {
                        System.IO.Directory.CreateDirectory(this.Directory);
                    }
                    if (File.Exists(Path.Combine(this.Directory, this.FileName + ConstDefine.FileExtension)) == false)
                    {
                        newFile = true;
                    }
                    if (this.isDisposed == false)
                    {
                        this.messageQueue.Enqueue(new MessageModel()
                        {
                            Message = message,
                            ChannelName = channelName,
                        });
                        this.recordLines++;
                    }
                    if ((this.messageQueue.Count > 50) || newFile || this.isDisposed || (this.recordLines >= this.LineCountLimit))
                    {
                        var tempBuffer = this.messageQueue;
                        this.messageQueue = new Queue<MessageModel>();
                        using (var writer = new StreamWriter(filePath, true, Encoding.Default))
                        {
                            foreach (var item in tempBuffer)
                            {
                                if (newFile)
                                {
                                    writer.WriteLine(ConstDefine.FileHeader);
                                    newFile = false;
                                }
                                writer.WriteLine(this.FormateData(item.ChannelName, item.Message));
                            }
                        }
                    }
                    this.MoveFile(ref filePath);
                    lastRecordTime = DateTime.Now;
                }
            }
        }

        private Boolean MoveFile(ref String filePath)
        {
            var result = false;

            if (this.Mode == LimitMode.SizeMode)
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > (FileSizeLimit * ConstDefine.M))
                {
                    this.RenameFile(ref filePath);
                    this.recordLines = 0;
                    result = true;
                }
            }
            else
            {
                if (this.recordLines >= this.LineCountLimit)
                {
                    this.RenameFile(ref filePath);
                    this.recordLines = 0;
                    result = true;
                }
            }

            return result;
        }

        #endregion
    }
}


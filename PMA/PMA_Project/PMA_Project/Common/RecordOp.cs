using Neusoft.Reach.CANComponent.Infrastructure;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace PMA_Project.common
{

    struct sRcvObj_Exp
    {
        public ReceiveObject rcvDataObj;
        public int nChan;
    };

    class RecordOp
    {
        public const int MIN_MSGNUM = 1;
        public const string strName_W = "\\MsgData";
        public string strHead0 = "date ";
        public const string strHead1 = "base hex timestamps absolute	";

        FileStream[] fs;                //文件流数组(用于读)
        StreamReader[] sr;              //流读取器数组

        //读
        string[] strLines;              //单个文件存储的所有记录
        string[] strFileName;           //记录文件名字
        UInt32[] nLineSize;             //单个记录文件总行数
        UInt32 nFileNum;                // 文件数量
        UInt32 nAllLines;               //总行数

        UInt32 nBegL;                   //当前一个回放文件的读取位置
        UInt32 nAllBegPos;
        int nCurFileInd;                //当前使用的文件序号
        int nSpeed;                     //回放速度  不大于16倍
        int nMsgsOne;                   //一次返回帧数量 
        bool bReadOver = false;         //false:No over    true: Over

        //写
        string strPath;                 //保存路径
        UInt32 nLineNum_W;              //为每个文件设置保存的行数
        List<FileStream> fs_W;          //文件流数组(用于写)
        List<StreamWriter> sw_W;        //流写入器数组

        public RecordOp()
        {
            nBegL = 0;
            nSpeed = 1;
            nMsgsOne = MIN_MSGNUM * nSpeed;
            nCurFileInd = 0;
            nAllLines = 0;
            nAllBegPos = 0;
        }
        private void Init_forA()
        {
            nBegL = 0;
            nSpeed = 1;
            nMsgsOne = MIN_MSGNUM * nSpeed;
            nCurFileInd = 0;
            nAllLines = 0;
            nAllBegPos = 0;
            strHead0 = "date ";
            bReadOver = false;
        }
        /// <summary>
        /// 初始化写文件
        /// </summary>
        /// <param name="strP">运行路径</param>
        public string Init_forW(string strP)
        {
            Init_forA();
            strPath = strP + strName_W;
            DateTime TimeT = System.DateTime.Now;
            string strDay = TimeT.ToString("ddd MMM dd hh:mm:ss t午 yyyy ");
            strHead0 += strDay;
            string strTime = TimeT.ToString("yyyyMMdd_hhmmss");
            strPath += strTime;
            fs_W = new List<FileStream>();
            sw_W = new List<StreamWriter>();

            // 使用字符串接收返回结果 成功返回空 失败返回错误信息
            // Add by hanxn 2018-12-26
            var result = OpenFile_ForW();

            return result;
        }

        /// <summary>
        /// 打开记录文件
        /// </summary>
        public string OpenFile_ForW()
        {
            // 使用字符串接收返回结果 成功返回空 失败返回错误信息
            // Add by hanxn 2018-12-26
            try
            {
                string strFile = strPath + "_" + nCurFileInd.ToString() + ".asc";
                FileStream fsT = new FileStream(strFile, FileMode.Create, FileAccess.Write);
                fsT.SetLength(0);
                StreamWriter swT = new StreamWriter(fsT, Encoding.Default);
                swT.WriteLine(strHead0);
                swT.WriteLine(strHead1);
                fs_W.Add(fsT);
                sw_W.Add(swT);

                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /// <summary>
        /// 初始化读文件，读所有行保存
        /// </summary>
        /// <param name="strFile">使用的多个读文件</param>
        public void Init_forR(string[] strFile)
        {
            Init_forA();
            Close();
            strFileName = strFile;
            nFileNum = (UInt32)strFileName.Length;
            fs = new FileStream[nFileNum];
            sr = new StreamReader[nFileNum];
            nLineSize = new UInt32[nFileNum];
            for (int i = 0; i < nFileNum; i++)
            {
                try
                {
                    fs[i] = new FileStream(strFileName[i], FileMode.Open);
                    sr[i] = new StreamReader(fs[i], Encoding.Default);
                }
                catch (System.Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                strLines = sr[i].ReadToEnd().Split('\n');
                nLineSize[i] = (UInt32)strLines.Length;
                nAllLines += nLineSize[i];
                sr[i].BaseStream.Seek(0, SeekOrigin.Begin);
            }
        }
        void OpenFile_ForR(int nindex)
        {
            strLines = sr[nindex].ReadToEnd().Split('\n');
        }
        /// <summary>
        /// 向文件写数据
        /// </summary>
        /// <param name="nWriteNum">要写数据量</param>
        /// <param name="lstrec">数据内存</param>
        public string WriteData(UInt32 nWriteNum, List<sRcvObj_Exp> lstrec)
        {
            // 使用字符串接收返回结果 成功返回空 失败返回错误信息
            // Add by hanxn 2018-12-26
            var result = string.Empty;

            for (int i = 0; i < nWriteNum; i++)
            {
                if (i < lstrec.Count)
                {
                    string str = FormatDataIn(lstrec[i]);
                    sw_W[nCurFileInd].WriteLine(str);
                    nBegL++;
                    if (nBegL == nLineNum_W)
                    {
                        sw_W[nCurFileInd].Close();
                        nCurFileInd++;

                        // 使用字符串接收返回结果 成功返回空 失败返回错误信息
                        // Add by hanxn 2018-12-26
                        result += OpenFile_ForW();
                        nBegL = 0;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 格式化记录数据
        /// </summary>
        /// <param name="rcvDataT">数据对象</param>
        /// <returns>格式化的字符串</returns>
        string FormatDataIn(sRcvObj_Exp rcvDataT)
        {
            string str = "";
            string str1;
            double dTime = rcvDataT.rcvDataObj.TimeStamp * 1.0 / 10000;
            //  str1 = rcvDataT.rcvDataObj.TimSmp.ToString()+"  ";//时间
            //  str1 = Math.Round(dTime, 4).ToString() + "  ";
            str1 = dTime.ToString() + "  ";
            str += str1;
            str1 = rcvDataT.nChan.ToString() + "  ";//通道
            str += str1;
            str1 = Convert.ToString(rcvDataT.rcvDataObj.ReceiveID, 16);//id
            if (rcvDataT.rcvDataObj.FrameType == FrameType.ExtendFrame)
            {
                str1 += "X";
            }
            str1 += "  ";
            str += str1;
            if (rcvDataT.rcvDataObj.IsSelfSend == true)//传输方向
            {
                str1 = "Tx" + "  ";
            }
            else
                str1 = "Rx" + "  ";
            str += str1;
            if (true)//数据帧
            {
                str1 = "d" + "  ";
            }
            else
            {
            }
            str += str1;
            str1 = rcvDataT.rcvDataObj.Length.ToString() + "  ";
            str += str1;

            for (int i = 0; i < rcvDataT.rcvDataObj.Length; i++)
            {
                str1 = rcvDataT.rcvDataObj.ReceiveData[i].ToString("X2") + " ";
                //   str1 = Convert.ToString(rcvDataT.rcvDataObj.RcvDat[i], 16) + " ";
                str += str1;
            }

            return str;
        }
        /// <summary>
        /// 格式化数据结构
        /// </summary>
        /// <param name="strOut">数据字符串</param>
        /// <returns>数据结构对象</returns>
        sRcvObj_Exp FormatDataOut(string strOut)
        {
            string str;
            Regex phoneExp = new Regex(@"\S+");
            MatchCollection matches = phoneExp.Matches(strOut);
            sRcvObj_Exp rcvTmp = new sRcvObj_Exp();
            ReceiveObject temp = new ReceiveObject();
            temp.ReceiveData = new byte[8];
            if (matches.Count > 6)
            {
                string str0 = matches[3].Value;
                string str1 = matches[4].Value;
                if ((str0 == "Rx" || str0 == "Tx") && str1 == "d")
                {
                    Double dTmp = Convert.ToDouble(matches[0].Value);
                    temp.TimeStamp = (UInt64)(Math.Round(10000 * dTmp, 0));
                    rcvTmp.nChan = Convert.ToInt32(matches[1].Value);
                    str = matches[2].Value;
                    if (str.IndexOf('X') != -1)//存在X
                    {
                        temp.FrameType = FrameType.ExtendFrame;
                        str = matches[2].Value.Substring(0, matches[2].Value.Length - 1);
                    }
                    else
                    {
                        temp.FrameType = FrameType.StandardFrame;
                        str = matches[2].Value;
                    }
                    temp.ReceiveID = UInt32.Parse(str, System.Globalization.NumberStyles.HexNumber);
                    temp.Length = Convert.ToUInt16(matches[5].Value);
                    for (int j = 0; j < temp.Length; j++)
                    {
                        temp.ReceiveData[j] = byte.Parse(matches[6 + j].Value, System.Globalization.NumberStyles.HexNumber);
                    }
                    if (str0 == "Tx")
                    {
                        temp.IsSelfSend = true;
                    }
                    else if (str0 == "Rx")
                    {
                        temp.IsSelfSend = false;
                    }
                }
            }
            rcvTmp.rcvDataObj = temp;
            return rcvTmp;
        }
        /// <summary>
        /// 获取数据行数
        /// </summary>
        /// <returns>行数</returns>
        public int GetRows()
        {
            return strLines.Length;
        }

        //public UInt32
        /// <summary>
        /// 获取回放速度
        /// </summary>
        /// <returns>速度</returns>
        public int GetSpeed()
        {
            return nSpeed;
        }
        /// <summary>
        /// 设置回放速度
        /// </summary>
        /// <param name="nTmpS">速度倍数</param>
        public void SetSpeed(int nTmpS)
        {
            if (nTmpS >= 1 && nTmpS <= 16)
            {
                nSpeed = nTmpS;
                nMsgsOne = MIN_MSGNUM * nSpeed;
            }
            else
            {
                MessageBox.Show("回放速度设置错误");
            }
        }
        /// <summary>
        /// 设置记录文件行数
        /// </summary>
        /// <param name="nLines">行数</param>
        public void SetFileSize(UInt32 nLines)
        {
            nLineNum_W = nLines;
        }

        /// <summary>
        /// 设置回放起始行
        /// </summary>
        /// <param name="nline">行序号</param>
        public void SetBeg(UInt32 nline)
        {
            UInt32 nSize = 0;
            for (int i = 0; i < nFileNum; i++)
            {
                nSize += nLineSize[i];
                if (nSize > nline)
                {
                    nCurFileInd = i;
                    break;
                }
            }
            nSize -= nLineSize[nCurFileInd];
            nBegL = nline - nSize;
            sr[nCurFileInd].BaseStream.Seek(0, SeekOrigin.Begin);
            strLines = sr[nCurFileInd].ReadToEnd().Split('\n');
        }

        /// <summary>
        /// 获取进度百分比
        /// </summary>
        /// <returns></returns>        
        public UInt32 GetPosPer()
        {
            UInt32 nPer = nAllBegPos / nAllLines;
            return nPer;
        }
        /// <summary>
        ///  读取文件，到结尾返回false，否则返回true，
        /// </summary>
        /// <param name="nLeg"></param>  输出 读取长度
        /// <param name="lstrec"></param> 读取内容存放内存
        /// <returns></returns>
        public bool ReadData(ref int nLeg, ref List<sRcvObj_Exp> lstrec)
        {
            if (bReadOver == true)
                return false;

            int nsize = 0;
            if (nFileNum != 0)
            {

                if (nBegL == 0 && nCurFileInd < nFileNum + 1)
                {
                    sr[nCurFileInd].BaseStream.Seek(0, SeekOrigin.Begin);
                    strLines = sr[nCurFileInd].ReadToEnd().Split('\n');
                    nBegL = 0;
                    nCurFileInd++;
                }
                do
                {
                    sRcvObj_Exp rcv = new sRcvObj_Exp();
                    nAllBegPos++;
                    rcv = FormatDataOut(strLines[nBegL]);
                    if (rcv.rcvDataObj.TimeStamp != 0)
                    {
                        lstrec[nsize] = rcv;
                        nsize++;
                    }
                    nBegL++;
                    if (nBegL == nLineSize[nCurFileInd - 1])
                    {
                        nLeg = nsize;
                        nBegL = 0;
                        if (nCurFileInd == nFileNum)
                        {
                            bReadOver = true;
                        }
                        return true;
                    }

                } while (nsize != nMsgsOne);


                nLeg = nMsgsOne;
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close()
        {
            for (int i = 0; i < nFileNum; i++)
            {
                if (sr != null)
                {

                    if (sr[i] != null)
                    {
                        sr[i].Close();
                    }
                }
                if (fs != null)
                {
                    if (fs[i] != null)
                    {
                        fs[i].Close();
                    }
                }
            }
            if (fs_W != null)
            {
                for (int i = 0; i < fs_W.Count; i++)
                {
                    sw_W[i].Close();
                    fs_W[i].Close();
                }
            }
        }
    }

}


/*
******************************************************************************                                                                                                                                         
*  File name:        ConstDefine.cs                                                                                                                                                                                                                            
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

#endregion 

namespace Neusoft.Reach.DBCAnalysis.Infrastructure
{
    /// <summary>
    /// 定义常量
    /// </summary>
    public sealed class ConstDefine
    {
        /// <summary>
        /// Node Definitions
        /// </summary>
        public const String NetworkNodeDefinition = "BU_:";

        /// <summary>
        /// Message Definitions
        /// </summary>
        public const String Message = "BO_ ";

        /// <summary>
        /// Signal Value Descriptions
        /// </summary>
        public const String ValueEncodings = "VAL_";

        /// <summary>
        /// Attribute definition
        /// </summary>
        public const String AttributeDefinition = "BA_D";

        /// <summary>
        /// Node  definition
        /// </summary>
        public const String NetworkNode = "BU_ ";


        /// <summary>
        /// Environment Variable definition
        /// </summary>
        public const String EnvironmentVariable = "EV_ ";

        /// <summary>
        /// Attribute Values
        /// </summary>
        public const String AttributeValues = "BA_ ";

        /// <summary>
        /// Attribute Default
        /// </summary>
        public const String AttributeDefault = "BA_DEF_DEF_  ";

        /// <summary>
        /// Protocol Type
        /// </summary>
        public const String ProtocolType = "\"ProtocolType\"";

        /// <summary>
        /// Attribute Name
        /// </summary>
        public const String NmStationAddress = "NmStationAddress";

        /// <summary>
        /// Default Node name
        /// </summary>
        public const String DefaultNodeName = "Vector__XXX";

        /// <summary>
        /// 匹配Signal的正则表达式
        ///*********************************************************************************************************************
        ///* 
        ///*      输入 : "SG_ INHW_Ve_VoltDiffMax m0 : 55|16@0+ (0.001,0.002) [-10.01|-65.535] \"V\" Vector__XXX"
        ///*      模式 : @"^SG_ (\S+) (((m)(\d+) )|(M ))?: (\d+)\|(\d+)@([0|1])([+|-]) \((\d+(\.\d+)?),(\d+(\.\d+)?)\) \[(-?\d+(\.\d+)?)\|(-?\d+(\.\d+)?)\] " + "\"" + @"(\S*)" + "\"" + @" ((Vector__XXX)|(\S+))$";
        ///*    
        ///*********************************************************************************************************************
        ///*   
        ///*     分组号                                  匹配值                                                            含义
        ///*
        ///* *******************************************************************************************************************  
        ///*    Group[0]    "SG_ INHW_Ve_VoltDiffMax m0 : 55|16@0+ (0.001,0.002) [-10.01|-65.535] "V" Vector__XXX"       全串
        ///*    Group[1]    "INHW_Ve_VoltDiffMax"                                                                         SignalName
        ///*    Group[2]    "m0 "                                                                                        multiplexer_indicator(""|"M"|"m\d")             
        ///*    Group[3]    "m0 "                                                                                        m multiplexer_switch_value            
        ///*    Group[4]    "m"                                                                                          multiplexed signals          
        ///*    Group[5]    "0"                                                                                          multiplexer_switch_value          
        ///*    Group[6]    ""                                                                                           M(当前未匹配)         
        ///*    Group[7]    "55"                                                                                         start_bit           
        ///*    Group[8]    "16"                                                                                         signal_size           
        ///*    Group[9]    "0"                                                                                          byte_order          
        ///*    Group[10]   "+"                                                                                          value_type          
        ///*    Group[11]   "0.001"                                                                                      factor              
        ///*    Group[12]   ".001"                                                                                       factor小数部分             
        ///*    Group[13]   "0.002"                                                                                      offset              
        ///*    Group[14]   ".002"                                                                                       offset小数部分             
        ///*    Group[15]   "-10.01"                                                                                     minimum               
        ///*    Group[16]   ".01"                                                                                        minimum小数部分            
        ///*    Group[17]   "-65.535"                                                                                    maximum                
        ///*    Group[18]   ".535"                                                                                       maximum小数部分             
        ///*    Group[19]   "V"                                                                                          unit          
        ///*    Group[20]   "Vector__XXX"                                                                                receiver                    
        ///*    Group[21]   "Vector__XXX"                                                                                receiver默认值                    
        ///*    Group[22]   ""                                                                                           设置值(当前未匹配)                                                                                                               
        ///*********************************************************************************************************************      
        /// </summary>
        public static readonly String SignalRegexPattern = @"^SG_ (\S+) (((m)(\d+) )|(M ))?: (\d+)\|(\d+)@([0|1])([+|-]) \((-?\d+(\.\d+)?),(-?\d+(\.\d+)?)\) \[(-?\d+(\.\d+)?)\|(-?\d+(\.\d+)?)\] " + "\"" + @"(\S*)" + "\"" + @"\s+((Vector__XXX)|(\S+))$";

        /// <summary>
        /// 匹配Attribute的正则表达式
        /// attribute_value_for_object = 'BA_' attribute_name (attribute_value |
        /// 'BU_' node_name attribute_value |
        /// 'BO_' message_id attribute_value |
        /// 'SG_' message_id signal_name attribute_value |
        /// 'EV_' env_var_name attribute_value)';' ;
        /// Groups[1] attribute_name
        /// Groups[3] attribute_value
        /// Groups[6] "BO_ "
        /// Groups[7] message_id
        /// Groups[8] attribute_value
        /// Groups[11] "BU_ "
        /// Groups[12] node_name
        /// Groups[13] attribute_value
        /// Groups[16] "SG_ "
        /// Groups[17] message_id
        /// Groups[18] signal_name
        /// Groups[19] attribute_value
        /// Groups[22] "EV_ "
        /// Groups[23] env_var_name
        /// Groups[24] attribute_value
        /// </summary>
        public static readonly String AttributeRegexPattern = @"^BA_ " + "\"" + @"(\S+)" + "\" " + @"((\d+(\.\d+)?)|((BO_ )(\d+ )(\d+(\.\d+)?))|((BU_ )(\S+ )(\d+(\.\d+)?))|((SG_ )(\d+ )(\S+ )(\d+(\.\d+)?))|((EV_ )(\S+ )(\d+(\.\d+)?)));$";

        #region dbc正则解析

        public static readonly String MessagePattern = "^BO_[ ]+(\\d+)[ ]+(\\w+):[ ]+(\\d+)[ ]+(\\w+)$";
        public static readonly String SignalPattern = "^SG_[ ]+(\\w+)[ ]+(((m)(\\d+[ ]+))|(M[ ]+))?:[ ]+(\\d+)\\|(\\d+)@([0|1])([+|-])[ ]+\\((-?\\d+(\\.\\d+)?),(-?\\d+(\\.\\d+)?)\\)[ ]+\\[(-?\\d+(\\.\\d+)?)\\|(-?\\d+(\\.\\d+)?)\\][ ]+\"((([^\"\\s])|([\\s\\u4e00-\\u9fa5]))*)\"[ ]+(\\w+(,[ ]*\\w+)*)$";
        public static readonly String AttributeValuePattern = "^BA_[ ]+\"(\\w+)\"[ ]+(((BU_)[ ]+(\\w+)[ ]+)|((BO_)[ ]+(\\d+)[ ]+)|((SG_)[ ]+(\\d+)[ ]+(\\w+)[ ]+)|((EV_)[ ]+(\\w+)[ ]+))?\"?(([+|-]?\\d*.?\\d*)|((([^\"\\s])|([\\s\\u4e00-\\u9fa5]))*))\"?;$";
        public static readonly String AttributeDefinitionPattern = "^BA_DEF_[ ]+((BU_)|(BO_)|(SG_)|(EV_))?[ ]+\"(\\w+)\"[ ]+(((INT)[ ]+([+|-]?\\d+)[ ]+([+|-]?\\d+))|((HEX)[ ]+([+|-]?\\d+)[ ]+([+|-]?\\d+))|((FLOAT)[ ]+([+|-]?\\d+.?\\d*)[ ]+([+|-]?\\d+.?\\d*))|(STRING)|((ENUM)[ ]+(\"((([^\"\\s])|([\\s\\u4e00-\\u9fa5]))*)\"([ ]*,\"((([^\"\\s])|([\\s\\u4e00-\\u9fa5]))*)\")*)))[ ]*;$";
        public static readonly String AttributeDefaultPattern = "^BA_DEF_DEF_[ ]+\"(\\w+)\"[ ]+\"?(([+|-]?\\d*.?\\d*)|((([^\"\\s])|([\\s\\u4e00-\\u9fa5]))*))\"?;$";
        public static readonly String SignalValueDescriptionPattern = "^VAL_[ ]+(\\d+)[ ]+(\\w+)[ ]+(((\\d+.?\\d*)[ ]+\"((([^\"\\s])|([\\s\\u4e00-\\u9fa5]))*)\"[ ]*)*)+;$";
        public static readonly String ValueTableDefinitionPattern = "^VAL_TABLE_[ ]+(\\w+)[ ]+(((\\d+.?\\d*)[ ]+\"((([^\"\\s])|([\\s\\u4e00-\\u9fa5]))*)\"[ ]*)*)+;$";

        #endregion

    }
}

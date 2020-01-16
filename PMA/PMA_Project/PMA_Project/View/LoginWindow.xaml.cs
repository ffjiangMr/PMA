
/*
******************************************************************************                                                                                                                                         
*  File name:        LoginWindow.xaml.cs                                                                                                                                                                                                                            
*  Copyright         ReachAuto Corporation. All rights reserved.                                                                                                                              
*  Notes:                                                                                                                              
*  History:                                                                                                                              
*    Revision        Date           Name              Comment                                                              
*    ------------------------------------------------------------------                
*    1.0          2019.04.18        JiangFei           Initial                                                                      
*                                                                                                                                          
******************************************************************************
*/

#region using directive

using log4net;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

#endregion 

namespace PMA_Project.View
{
    /// <summary>
    /// Interaction logic for LoginWin.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Logger

        private static ILog Logger = LogManager.GetLogger(typeof(MainWindow));

        #endregion

        #region security 

        private readonly static String account = "admin";
        private readonly static String security = "+XHD3GTZS5PbHAtFxlBvEw==";
        private readonly static String key = "!@#$%^&*";
        private readonly static String iv = "P!M@A#E$O%S^e&V*i(e)w_e+r~";

        #endregion

        public Boolean isLoginSuccess = false;

        public LoginWindow()
        {
            Logger.Info("Func in.");
            InitializeComponent();
            Logger.Info("Func out.");
        }

        private static String Encrypt(String password)
        {
            Logger.Info("Func in.");
            String result = String.Empty;
            try
            {
                byte[] rgbKkey = Encoding.UTF8.GetBytes(key);
                byte[] rgbIV = Encoding.UTF8.GetBytes(iv);
                byte[] data = Encoding.UTF8.GetBytes(password);
                Logger.Debug("Input passward is : " + password);
                using (var descsp = new DESCryptoServiceProvider())
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var crypto = new CryptoStream(stream, descsp.CreateEncryptor(rgbKkey, rgbIV), CryptoStreamMode.Write))
                        {
                            crypto.Write(data, 0, data.Length);
                        }
                        result = Convert.ToBase64String(stream.ToArray());
                        Logger.Debug("Output security result is : " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
            return result;
        }

        private static String Decrypt(String securityString)
        {
            Logger.Info("Func in.");
            String result = String.Empty;
            try
            {
                Byte[] rgbKey = Encoding.UTF8.GetBytes(key);
                Byte[] rgbIV = Encoding.UTF8.GetBytes(iv);
                Byte[] data = Encoding.UTF8.GetBytes(securityString);
                Byte[] buffer = Convert.FromBase64String(securityString);
                using (var descsp = new DESCryptoServiceProvider())
                {
                    using (var stream = new MemoryStream(buffer))
                    {
                        using (var crypto = new CryptoStream(stream, descsp.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Read))
                        {
                            using (var reader = new StreamReader(crypto))
                            {
                                result = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            Logger.Info("Func out.");
            return result;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Func in.");
            try
            {
                var securityString = Encrypt(this.password.Password);
                if ((this.textBox1.Text == account) && (securityString == security))
                {
                    isLoginSuccess = true;
                }
                else
                {
                    Logger.Error("Password error.");
                    MessageBox.Show("\tAccount or Password incorret ,\t" + Environment.NewLine + "\t  please check your input.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            finally
            {
                this.Close();
            }
            Logger.Info("Func out.");
        }
    }
}

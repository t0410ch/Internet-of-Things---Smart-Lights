using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 物联网
{
    /// <summary>
    /// Show_say.xaml 的交互逻辑
    /// </summary>
    public partial class Show_say : Window
    {
        public Show_say(Socket socketSend, IPAddress ip, int port)
        {
            InitializeComponent();
            this.socketSend = socketSend;
            try
            {
                socketSend.Connect(ip, port);
                Thread threadReceive;
                threadReceive = new Thread(new ThreadStart(Receive));
                threadReceive.IsBackground = true;
                threadReceive.Start();
                //登陆
                string strMsg = "32 00 F1 80 11 53 A5 60 fe af 27 05 61 64 6d 69 6e 20 32 31 32 33 32 66 32 39 37 61 35 37 61 35 61 37 34 33 38 39 34 61 30 65 34 61 38 30 31 66 63 33";
                byte[] buffer = new byte[2048];
                buffer = GetByteArray(strMsg);
                int receive = socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                lb.Dispatcher.Invoke(() =>
                {
                    lb.Items.Add(ex);
                });
            }
        }
        private int H1;
        private int S1;
        private int L1;
        //创建连接的Socket
        private Socket socketSend;
        //创建接收客户端发送消息的线程
        
        Task C;
        int trad1_1 = 0;
        int trad1_2 = 0;
        int trad1_3 = 0;
        public static void ShowAnimation(object control)
        {
            Type type = control.GetType();
            switch (type.Name)
            {
                case "Border":
                    {
                        Border newBorder = (Border)control;
                        #region 高、宽变化动画

                        DoubleAnimation widthAnimation = new DoubleAnimation(0, newBorder.Width, new Duration(TimeSpan.FromSeconds(0.5)));
                        newBorder.BeginAnimation(Border.WidthProperty, widthAnimation, HandoffBehavior.Compose);

                        DoubleAnimation heightAnimation = new DoubleAnimation(0, newBorder.Height, new Duration(TimeSpan.FromSeconds(0.5)));
                        newBorder.BeginAnimation(Border.HeightProperty, heightAnimation, HandoffBehavior.Compose);
                        #endregion
                    }
                    break;

                default:
                    break;
            }
        }

        #region 智能插座操作
        /// <summary>
        /// 智能插座的开启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[2048];
                buffer = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 C1 A6 00 00 00 00 00 00 08 00 00 01");
                socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }

        }
        /// <summary>
        /// 智能插座关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[2048];
                buffer = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 C1 A6 00 00 00 00 00 00 08 00 00 00");
                socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }

        #endregion

        #region 网关操作
        ///// <summary>
        ///// 修改按键使用
        ///// </summary>
        //private void set()
        //{
        //    inname.IsEnabled = true;
        //    inpassword.IsEnabled = true;
        //    insn.IsEnabled = true;
        //    btu2.IsEnabled = true;
        //    inputip.IsEnabled = false;
        //    inputname.IsEnabled = false;
        //    btu1.IsEnabled = false;
        //}

        ///// <summary>
        ///// 连接网关
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        set();
        //        //Task.Run(() =>
        //        //{
        //        //    this.inputip.Dispatcher.Invoke(() =>
        //        //    {
        //        socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //        IPAddress ip = IPAddress.Parse(this.inputip.Text.Trim());
        //        socketSend.Connect(ip, Convert.ToInt32(this.inputname.Text.Trim()));
        //        //    });

        //        //});
        //        //Task.Run(() =>
        //        //{
        //        //    while (i)
        //        //    {
        //        threadReceive = new Thread(new ThreadStart(Receive));
        //        threadReceive.IsBackground = true;
        //        threadReceive.Start();
        //        //    }
        //        //});

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("连接服务端出错:" + ex.ToString());
        //    }


        //}

        /// <summary>
        /// 接收网关发送的消息
        /// </summary>
        private void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];
                    //实际接收到的字节数
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    else
                    {
                        string str = ByteToHexStr(buffer, r);
                        lb.Dispatcher.Invoke(() =>
                        {
                            if (str == "40 01 00")
                            {
                                //btu2.IsEnabled = false;
                                lb.Items.Add("登陆成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 BC 2E 08 01")
                            {
                                lb.Items.Add("小灯打开成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 BC 2E 08 00")
                            {
                                lb.Items.Add("小灯关闭成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 52 23 0A 00")
                            {
                                lb.Items.Add("开关关闭成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 52 23 0A 01")
                            {
                                lb.Items.Add("开关打开成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 52 23 08 00")
                            {
                                lb.Items.Add("开关关闭成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 52 23 08 01")
                            {
                                lb.Items.Add("开关打开成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 C1 A6 08 01")
                            {
                                lb.Items.Add("插座打开成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                            if (str == "07 04 C1 A6 08 00")
                            {
                                lb.Items.Add("插座关闭成功!");
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                lb.Dispatcher.Invoke(() =>
                {
                    lb.Items.Add("接收服务端发送的消息出错: " + ex.ToString());
                });
            }
        }

        #endregion
        #region 小灯的操作

        /// <summary>
        /// 改变灯的颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            RgbToHsv();
            string color_h = Convert.ToString(H1, 16);
            string color_s = Convert.ToString(S1, 16);
            string color_l = Convert.ToString(L1, 16);
            string s = "1B 00 F1 80 11 53 A5 60 FE 84 10 02 BC 2E 00 00 00 00 00 00 08 00 00 ";//头参数
            s += color_h + " ";
            s += color_s + " ";
            s += "00 00";
            string str = "1B 00 F1 80 11 53 A5 60 FE 83 0f 02 BC 2E 00 00 00 00 00 00 08 00 00 ";//头参数 "
            str += color_l + " 00 00";
            try
            {
                byte[] buffer = new byte[2048];
                buffer = GetByteArray(s);
                socketSend.Send(buffer);
                byte[] buffer1 = new byte[2048];
                buffer1 = GetByteArray(str);
                socketSend.Send(buffer1);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex.Message);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }


        /// <summary>
        /// 打开小灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[2048];
                buffer = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 BC 2E 00 00 00 00 00 00 08 00 00 01");
                socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }

        }

        /// <summary>
        /// 关闭小灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[2048];
                buffer = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 BC 2E 00 00 00 00 00 00 08 00 00 00");
                socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }
        #endregion
        #region 开关开关的操作
        /// <summary>
        /// 智能开关一打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer1 = new byte[2048];
                buffer1 = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 52 23 00 00 00 00 00 00 0A 00 00 01");
                socketSend.Send(buffer1);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }
        /// <summary>
        /// 智能开关一关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer1 = new byte[2048];
                buffer1 = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 52 23 00 00 00 00 00 00 0A 00 00 0");
                socketSend.Send(buffer1);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }
        /// <summary>
        /// 智能开关二打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[2048];
                buffer = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 52 23 00 00 00 00 00 00 08 00 00 01");
                socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }
        /// <summary>
        /// 智能开关二关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[2048];
                buffer = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 52 23 00 00 00 00 00 00 08 00 00 00");
                socketSend.Send(buffer);
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }
        #endregion

        #region 类型转换操作
        /// <summary>
        /// 将接受到的byte[]转换为string类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes, int p) //函数,字节数组转16进制字符串
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < p; i++)
                {
                    if (i != p - 1) returnStr += bytes[i].ToString("X2") + " ";
                    else returnStr += bytes[i].ToString("X2");
                }

            }
            return returnStr;
        }

        /// <summary>
        /// 将string类型的数据转换为byte[]数组
        /// </summary>
        /// <param name="shex"></param>
        /// <returns></returns>
        public static byte[] GetByteArray(string shex)
        {
            string[] ssArray = shex.Split(' ');
            List<byte> bytList = new List<byte>();
            foreach (var s in ssArray)
            {
                //将十六进制的字符串转换成数值
                bytList.Add(Convert.ToByte(s, 16));
            }
            //返回字节数组
            return bytList.ToArray();
        }

        /// <summary>
        /// RGB转换HSV
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public void RgbToHsv()
        {
            float min, max, tmp, H, S, V;
            float R = color.R * 1.0f / 255, G = color.G * 1.0f / 255, B = color.B * 1.0f / 255;
            tmp = Math.Min(R, G);
            min = Math.Min(tmp, B);
            tmp = Math.Max(R, G);
            max = Math.Max(tmp, B);
            // H
            H = 0;
            if (max == min)
            {
                H = 0;
            }
            else if (max == R && G > B)
            {
                H = 60 * (G - B) * 1.0f / (max - min) + 0;
            }
            else if (max == R && G < B)
            {
                H = 60 * (G - B) * 1.0f / (max - min) + 360;
            }
            else if (max == G)
            {
                H = H = 60 * (B - R) * 1.0f / (max - min) + 120;
            }
            else if (max == B)
            {
                H = H = 60 * (R - G) * 1.0f / (max - min) + 240;
            }
            // S
            if (max == 0)
            {
                S = 0;
            }
            else
            {
                S = (max - min) * 1.0f / max;
            }
            // V
            V = max;
            S *= 255;
            V *= 255;
            H1 = (int)Math.Round(H, 0);
            S1 = (int)Math.Round(S, 0);
            L1 = (int)Math.Round(V, 0);
        }
        #endregion

        #region 用户登录操作
        /// <summary>
        /// 登陆账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (inname.Text == "admin" && inpassword.Password == "admin" && insn.Text == "F1 80 11 53 A5 60")
        //        {
        //            if (socketSend == null)
        //            {
        //                MessageBox.Show("请连接网关");
        //            }
        //            else
        //            {//                        |          sn    |
        //                string strMsg = "32 00 F1 80 11 53 A5 60 fe af 27 05 61 64 6d 69 6e 20 32 31 32 33 32 66 32 39 37 61 35 37 61 35 61 37 34 33 38 39 34 61 30 65 34 61 38 30 31 66 63 33";
        //                byte[] buffer = new byte[2048];
        //                buffer = GetByteArray(strMsg);
        //                int receive = socketSend.Send(buffer);
        //            }

        //        }
        //        else MessageBox.Show("登陆失败！");
        //    }
        //    catch (Exception ex)
        //    {
        //        lb.Items.Add(ex);
        //    }
        //}


        #endregion

        #region 场景
        /// <summary>
        /// 蹦迪场景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (trad1_1 == 1) trad1_1++;
                if (trad1_1 == 0)
                {
                    C = new Task(trad1);
                    lb.Items.Add("KTV模式已开启");
                    lb.SelectedIndex = lb.Items.Count - 1;
                    this.lb.ScrollIntoView(lb.SelectedItem);
                    C.Start();
                    trad1_1++;
                }





            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }
        private void trad1()
        {
            #region 参数设置
            string[] s = new string[3];
            string[] str = new string[3];
            s[0] = "1B 00 F1 80 11 53 A5 60 FE 84 11 02 BC 2E 00 00 00 00 00 00 08 00 00 0 64 00 00";
            s[1] = "1B 00 F1 80 11 53 A5 60 FE 84 11 02 BC 2E 00 00 00 00 00 00 08 00 00 3b 64 00 00";
            s[2] = "1B 00 F1 80 11 53 A5 60 FE 84 11 02 BC 2E 00 00 00 00 00 00 08 00 00 7d 64 00 00";
            str[0] = "1B 00 F1 80 11 53 A5 60 FE 83 11 02 BC 2E 00 00 00 00 00 00 08 00 00 63 00 00";
            str[1] = "B 00 F1 80 11 53 A5 60 FE 83 11 02 BC 2E 00 00 00 00 00 00 08 00 00 63 00 00";
            str[2] = "1B 00 F1 80 11 53 A5 60 FE 83 11 02 BC 2E 00 00 00 00 00 00 08 00 00 63 00 00";
            byte[] buffer2 = new byte[2048];
            buffer2 = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 BC 2E 00 00 00 00 00 00 08 00 00 01");
            #endregion
            try
            {
                //socketSend.Send(buffer2);
            }
            catch(Exception ex)
            {
                lb.Dispatcher.Invoke(() =>
                {
                    lb.Items.Add(ex);
                    lb.SelectedIndex = lb.Items.Count - 1;
                    this.lb.ScrollIntoView(lb.SelectedItem);
                });
            }
            while (trad1_1 != 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    byte[] buffer = new byte[2048];
                    buffer = GetByteArray(s[i]);
                    byte[] buffer1 = new byte[2048];
                    buffer1 = GetByteArray(str[i]);
                    try
                    {
                        socketSend.Send(buffer);
                        socketSend.Send(buffer1);
                    }
                    catch(Exception ex)
                    {
                        lb.Dispatcher.Invoke(() =>
                        {
                            lb.Items.Add(ex);
                            lb.SelectedIndex = lb.Items.Count - 1;
                            this.lb.ScrollIntoView(lb.SelectedItem);
                        });
                    }
                    Thread.Sleep(300);
                }
            }
            lb.Dispatcher.Invoke(() =>
            {
                lb.Items.Add("KTV模式已关闭");
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            });
            trad1_1 = 0;
        }
        /// <summary>
        /// 定时开启场景
        /// </summary>
        /// <param name="i"></param>
        private void trad2(object i)
        {

            int p = int.Parse((string)i) * 1000;
            Thread.Sleep(p);
            byte[] buffer2 = new byte[2048];
            buffer2 = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 BC 2E 00 00 00 00 00 00 08 00 00 01");
            
            string s = "1B 00 F1 80 11 53 A5 60 FE 84 10 02 BC 2E 00 00 00 00 00 00 08 00 00 0 0 00 00";
            string str = "1B 00 F1 80 11 53 A5 60 FE 84 10 02 BC 2E 00 00 00 00 00 00 08 00 00 0 0 00 00";
            byte[] buffer = new byte[2048];
            buffer = GetByteArray(s);
            byte[] buffer1 = new byte[2048];
            buffer1 = GetByteArray(str);

            try
            {
                socketSend.Send(buffer2);
                socketSend.Send(buffer);
                socketSend.Send(buffer1);
                lb.Dispatcher.Invoke(() =>
                {
                    lb.Items.Add("小灯已开启！");
                    lb.SelectedIndex = lb.Items.Count - 1;
                    this.lb.ScrollIntoView(lb.SelectedItem);
                });
            }
            catch(Exception ex)
            {
                lb.Dispatcher.Invoke(() =>
                {
                    lb.Items.Add(ex);
                    lb.SelectedIndex = lb.Items.Count - 1;
                    this.lb.ScrollIntoView(lb.SelectedItem);

                });
                
            }
            
            trad1_2 = 0;
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            try
            {
                if (trad1_2 == 1)
                {
                    if (int.Parse(tbshow.Text) > 0 && int.Parse(tbshow.Text) < 50000)
                    {
                        tbshow.Visibility = Visibility.Hidden;
                        object o = tbshow.Text;
                        Thread t = new Thread(trad2);
                        string sp = "灯将在" + tbshow.Text + "秒后开启!";
                        lb.Items.Add(sp);
                        lb.SelectedIndex = lb.Items.Count - 1;
                        this.lb.ScrollIntoView(lb.SelectedItem);
                        t.IsBackground = true;
                        t.Start(o);
                    }
                    else MessageBox.Show("请重新输入！");
                }
                if (trad1_2 == 0)
                {
                    tbshow.Visibility = Visibility.Visible;
                    trad1_2++;
                }
            }
            catch (Exception ex)
            {
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }

        }

        /// <summary>
        /// 小灯闹钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            try
            {
                if (trad1_3 == 1)
                {
                    if (int.Parse(tbshow.Text) > 0 && int.Parse(tbshow.Text) < 50000)
                    {
                        tbshow.Visibility = Visibility.Hidden;
                        object o = tbshow.Text;
                        Thread t = new Thread(trad3);
                        string sp = "灯将在" + tbshow.Text + "秒后定时提醒您!";
                        lb.Items.Add(sp);
                        
                        lb.SelectedIndex = lb.Items.Count - 1;
                        this.lb.ScrollIntoView(lb.SelectedItem);
                        t.IsBackground = true;
                        trad1_3++;
                        t.Start(o);
                    }
                }
                else if (trad1_3 == 0)
                {
                    tbshow.Visibility = Visibility.Visible;
                    trad1_3++;
                }
                else
                {
                    trad1_3 = 0;
                    lb.Items.Add("您的闹钟已关闭！");
                    lb.SelectedIndex = lb.Items.Count - 1;
                    this.lb.ScrollIntoView(lb.SelectedItem);
                }
            }
            catch (Exception ex)
            {    
                lb.Items.Add(ex);
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            }
        }
        private void trad3(object i)
        {

            int p = int.Parse((string)i) * 1000;
            Thread.Sleep(p);
            byte[] buffer2 = new byte[2048];
            buffer2 = GetByteArray("18 00 F1 80 11 53 A5 60 FE 82 0d 02 BC 2E 00 00 00 00 00 00 08 00 00 01");
            string[] s = new string[2];
            string[] str = new string[2];
            s[0] = "1B 00 F1 80 11 53 A5 60 FE 84 10 02 BC 2E 00 00 00 00 00 00 08 00 00 0 0 00 00";
            str[0] = "1B 00 F1 80 11 53 A5 60 FE 84 10 02 BC 2E 00 00 00 00 00 00 08 00 00 0 0 00 00";
            str[1] = "B 00 F1 80 11 53 A5 60 FE 83 11 02 BC 2E 00 00 00 00 00 00 08 00 00 63 00 00";
            s[1] = "1B 00 F1 80 11 53 A5 60 FE 84 11 02 BC 2E 00 00 00 00 00 00 08 00 00 3b 64 00 00";
            
            lb.Dispatcher.Invoke(() =>
            {
                lb.Items.Add("您设置的闹铃小灯已开启！");
                lb.Items.Add("再次点击闹铃按钮进行关闭！");
                lb.SelectedIndex = lb.Items.Count - 1;
                this.lb.ScrollIntoView(lb.SelectedItem);
            });
            try
            {
                //socketSend.Send(buffer2);
                while (trad1_3 == 2)
                {
                    for (int ip = 0; ip < 2; ip++)
                    {
                        byte[] buffer = new byte[2048];
                        buffer = GetByteArray(s[ip]);
                        byte[] buffer1 = new byte[2048];
                        buffer1 = GetByteArray(str[ip]);
                        try
                        {
                            socketSend.Send(buffer);
                            socketSend.Send(buffer1);
                        }
                        catch (Exception ex)
                        {
                            lb.Dispatcher.Invoke(() =>
                            {
                                lb.Items.Add(ex);
                                lb.SelectedIndex = lb.Items.Count - 1;
                                this.lb.ScrollIntoView(lb.SelectedItem);
                            });
                        }
                        Thread.Sleep(300);
                    }
                }
                
            }
            catch (Exception ex)
            {
                lb.Dispatcher.Invoke(() =>
                {
                    lb.Items.Add(ex);
                    lb.SelectedIndex = lb.Items.Count - 1;
                    this.lb.ScrollIntoView(lb.SelectedItem);

                });

            }

            trad1_3 = 0;
        }
        #endregion





        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }



        private void lb_DataContextChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            //ListBoxItem item = new ListBoxItem();
            //item = (ListBoxItem)lb.Items[lb.Items.Count - 1];

            //this.lb.ScrollIntoView(item);
        }

        private void lb_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            //ListBoxItem item = new ListBoxItem();
            //item = (ListBoxItem)lb.Items[lb.Items.Count - 1];
            //this.lb.ScrollIntoView(item);
        }

        private void lb_Loaded(object sender, RoutedEventArgs e)
        {
            //if (lb.Items.Count != 0)
            //{
            //    ListBoxItem item = new ListBoxItem();
            //    item = (ListBoxItem)lb.Items[lb.Items.Count - 1];
            //    this.lb.ScrollIntoView(item);
            //}
            
        }

        
    }
}

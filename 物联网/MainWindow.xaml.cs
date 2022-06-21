using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace 物联网
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        private Socket socketSend;
        IPAddress ip;
        int port;
        int x = 0;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        //登录
        private void bt_login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (x==0)
            {
                igshow.Visibility = Visibility.Visible;
                lbshow.Visibility = Visibility.Visible;
                lbshow.Content = "请先点击我连接网关！";
                showig.Text = "登录网关";
                login_showup.Text = "IP地址";
                login_showdown.Text = "端口";
                login_name.Text = "192.168.7.1";
                login_password.Password = "8124";
                x = 1;
            }
            else
            {
                if (showig.Text == "登录账号")
                {
                    if (login_name.Text == "admin" && login_password.Password == "admin")
                    {
                        Show_say s = new Show_say(socketSend, ip, port);
                        igshow.Visibility = Visibility.Hidden;
                        lbshow.Visibility = Visibility.Hidden;
                        s.ShowDialog();
                    }
                    else
                    {
                        igshow.Visibility = Visibility.Visible;
                        lbshow.Visibility = Visibility.Visible;
                        lbshow.Content = "密码错误哦！";
                    }
                }
                else if (showig.Text == "登录网关")
                {
                    try
                    {
                        if (this.login_name.Text == "192.168.7.1" && this.login_password.Password == "8124")
                        {
                            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            ip = IPAddress.Parse(this.login_name.Text.Trim());
                            port = Convert.ToInt32(this.login_password.Password.Trim());
                            x = 1;
                            igshow.Visibility = Visibility.Hidden;
                            lbshow.Visibility = Visibility.Hidden;
                            showig.Text = "登录账号";
                            login_showup.Text = "账号";
                            login_showdown.Text = "密码";
                            login_name.Text = "admin";
                            login_password.Password = "admin";
                        }
                        else
                        {
                            igshow.Visibility = Visibility.Visible;
                            lbshow.Visibility = Visibility.Visible;
                            lbshow.Content = "IP错误哦！";
                        }
                        //    });

                        //});
                        //Task.Run(() =>
                        //{
                        //    while (i)
                        //    {
                        //threadReceive = new Thread(new ThreadStart(Receive));
                        //threadReceive.IsBackground = true;
                        //threadReceive.Start();
                        //    }
                        //});

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("连接服务端出错:" + ex.ToString());
                    }

                }
            }
           
        }

        //切换
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (showig.Text == "登录账号")
            {
                showig.Text = "登录网关";
                login_showup.Text = "IP地址";
                login_showdown.Text = "端口";
            }
            else if (showig.Text == "登录网关")
            {
                showig.Text = "登录账号";
                login_showup.Text = "账号";
                login_showdown.Text = "密码";
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}

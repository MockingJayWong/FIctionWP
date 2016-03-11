using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Networking.Connectivity;
//WI-Fi  TEST
using Windows.Devices.WiFiDirect;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;
using Windows.UI.Popups;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using Windows.UI;
using Windows.Storage;
using Windows.ApplicationModel;

using Windows.ApplicationModel.Background;
using System.Threading;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.System;
using System.Diagnostics;
using sqlite.share;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace fiction_test
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 
    
    public sealed partial class MainPage : Page
    {
        ObservableCollection<fiction> list;
        //Windows.Devices.WiFiDirect.WiFiDirectDevice wfdDevice;


        private string baidu_f;
        private string baidu_e;
        private bool web_s;



        public MainPage()
        {
            this.InitializeComponent();
            baidu_f = "http://www.baidu.com/from=844b/s?word=";
            baidu_e = "&sa=tb&ts=0968524&t_kt=0&ms=1&amp;ss=101";

            web_s = false;

            list = new ObservableCollection<fiction>();
            //list.Add(new fiction("全职法师", "http://www.qidian.com/Book/3489766.aspx"));
            //list.Add(new fiction("飞天", "http://www.qidian.com/Book/2227457.aspx"));
            //list.Add(new fiction("天醒之路", "http://www.qidian.com/Book/3206900.aspx"));
            //list.Add(new fiction("暗夜游侠", "http://www.qidian.com/Book/3434836.aspx"));
            
            list = ReadFictions();
            get_all();
            ls.ItemsSource = list;


            //this.NavigationCacheMode = NavigationCacheMode.Required;
        }






        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string appVersion = string.Format("{0}.{1}.{2}.{3}",
            Package.Current.Id.Version.Build,
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Revision);

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AppVersion"))
            {
                if (ApplicationData.Current.LocalSettings.Values["AppVersion"].ToString() != appVersion)
                {
                    // Our app has been updated
                    ApplicationData.Current.LocalSettings.Values["AppVersion"] = appVersion;

                    // Call RemoveAccess
                    BackgroundExecutionManager.RemoveAccess();
                }
            }
            else
            {
                // App first launched
                ApplicationData.Current.LocalSettings.Values["AppVersion"] = appVersion;
            }
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            //Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。


        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (!Frame.CanGoBack)
            {

                //Windows.Storage.ApplicationData.Current.
            }

        }



        private async Task<string> gget(Regex para, string s1) {
            //正则表达式内容
            //string match = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*$";
            //string match = @"[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*$";
            string match = @"[a-zA-z]+://[^\s]*";
            //初始化正则表达式实例
            Regex reg = new Regex(match);
            //开始验证
            bool HasValidate = reg.IsMatch(s1);
            string tmp, tmpend = "";
            string output = "";
            if (HasValidate)
            {
                try
                {
                    //MessageBox.Show("这是网站有效URL格式。");

                    tmp = await GetHtml(s1);
                    tmpend = WebHelper.StripHTML(tmp);
                    //tmpend = Regex.Replace(tmpend, "[^\u4e00-\u9fa5]" , "");  //剔除非中文



                    Match outp = para.Match(tmpend);
                    string a_ = outp.Groups["data"].Value;

                    Regex ma = new Regex(@"\u7b2c\w+\u7ae0.*?$", RegexOptions.RightToLeft);           //选取第XX章；
                    output = ma.Match(a_).Result("$0");     //正则中？代表懒惰匹配,Result模式，0就是加上范围，1就是下面那句代码
                                                            //string a = output.Groups["data"].Value;
                }
                catch (Exception)
                {
                    //MessageDialog m1 = new MessageDialog("3.该网站只能手动查询！");
                    //await m1.ShowAsync();
                }
            }
            return output;
        }

        /// <summary>
        /// 获取有效的HTML
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public async Task<String> GetHtml(String Url)
        {
            
                               

            string sException = null;

            string sRslt = null;
            //string GBsRslt = null;
            string result = null;
            //string Gbresult = null;
            StreamReader htm = null;
            WebResponse oWebRps = null;
            //WebResponse bWebRps = null;
            //int a = 0;

            WebRequest oWebRqst = WebRequest.Create(Url);
            ///WebRequest bWebRqst = WebRequest.Create(Url);
            

            try
            {
                oWebRps = await oWebRqst.GetResponseAsync();
                var oHttpWebRps = (HttpWebResponse)oWebRps;
                result = "网络加载：" + oHttpWebRps.StatusCode + " 网络状态说明：" + oHttpWebRps.StatusDescription + Environment.NewLine;
                     
                // 响应失败将抛出异常

                //bWebRps = await bWebRqst.GetResponseAsync();
                //var bHttpWebRps = (HttpWebResponse)bWebRps;
                //Gbresult =  "网络加载：" + bHttpWebRps.StatusCode + " 网络状态说明：" + bHttpWebRps.StatusDescription + Environment.NewLine;
            }
            catch (WebException e)
            {
                //sException = e.Message.ToString();

                //await new MessageDialog(sException).ShowAsync();
            }
            catch (Exception e)
            {
                //sException = e.ToString();

                //await new MessageDialog(sException).ShowAsync();
            }
            finally
            {
                if (oWebRps != null)
                {
                    StreamReader oStreamRd = new StreamReader(
                       oWebRps.GetResponseStream(), Encoding.UTF8
                        );

                   // StreamReader GBoStreamRd = new StreamReader(
                   //      bWebRps.GetResponseStream(), await DBCSEncoding.GetDBCSEncoding("gb2312")
                   //     );
                    //var b = oStreamRd.CurrentEncoding;
                    sRslt = oStreamRd.ReadToEnd();
                    
                    //tt.Text = sRslt;
                    //GBsRslt = GBoStreamRd.ReadToEnd();

                        htm = oStreamRd;
                        



                }
            }

                rbt.Content = result;
                rbt.IsEnabled = false;

                return sRslt;

 
        }


        bool isLuan(string txt)
        {
            var bytes = Encoding.UTF8.GetBytes(txt);
            //239 191 189
            for (var i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }
            return false;
        }

        private void StackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started) {
                StackPanel stk = sender as StackPanel;
                RemoveFromList(((TextBlock)stk.Children[0]).Text, 0);
                ls.Items.Remove(sender);
                stk.Background = new SolidColorBrush(Colors.Red);
            }
        }


        private  void AddToList(fiction tmp)
        {
            if (list.Count < 10)
            {

                using (var dbConn = new SQLiteConnection(App.db_path))
                {

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Insert(tmp);
                    });
                }
                    list.Add(tmp);
                    ls.ItemsSource = list;


            } else {
                //await new MessageDialog("抱歉，最多存储十本小说").ShowAsync();
            }
        }



        private void RemoveFromList(string title, int way, string newchapter = "") 
        {
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name == title) {
                    num = list[i].Id;
                    if (way == 0)
                        list.RemoveAt(i);
                    break;
                }
            }
            using (var dbConn = new SQLiteConnection(App.db_path))
            {
                var existingconact = dbConn.Query<fiction>("select * from fiction where Id =" + num).FirstOrDefault();
                if (existingconact != null)
                {
                    if (way == 0)
                        dbConn.RunInTransaction(() =>
                            {
                                dbConn.Delete(existingconact);
                            });
                    else
                        dbConn.RunInTransaction(() =>
                        {
                            existingconact.Newchapter = newchapter;
                            dbConn.Update(existingconact);
                        });
                }
            }
        }

        public ObservableCollection<fiction> ReadFictions()
        {
            using (var dbConn = new SQLiteConnection(App.db_path))
            {
                List<fiction> myCollection = dbConn.Table<fiction>().ToList<fiction>();
                ObservableCollection<fiction> ContactsList = new ObservableCollection<fiction>(myCollection);
                return ContactsList;
            }
        }

        private async void StackPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            StackPanel stk = sender as StackPanel;
            stk.Background = new SolidColorBrush(Colors.LightPink);
            if (web_s) {
                await Launcher.LaunchUriAsync(new Uri(UTF8Encoder(((TextBlock)stk.Children[0]).Text)));
            }
        }

        private void StackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            StackPanel stk = sender as StackPanel;
            stk.Background = new SolidColorBrush(Colors.OliveDrab);
        }

        private async void rbt_Click(object sender, RoutedEventArgs e)
        {
            await get_all();

        }

        private string UTF8Encoder(string fic_name)
        {
            string sb = "";
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(fic_name); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb += (@"%" + Convert.ToString(byStr[i], 16));
            }
            return baidu_f + sb + baidu_e;
        }

        public async Task get_all()
        {
            try
            {
                string result = "";
                Regex m = new Regex(@"\u6700\u65b0\u7ae0\u8282(?<data>.*)\(\u66f4\u65b0");           //起点网，更新于；
                for (int i = 0; i < list.Count; i++)
                {
                    string tmp = await gget(m, list[i].Website);
                    if (tmp.Length > 35)
                        tmp = "";
                    result += list[i].Name + " : " + tmp + "\n";

                    list[i].Newchapter = tmp;
                    RemoveFromList(list[i].Name, 1, tmp);

                }
                    rbt.Content = "点击刷新起点小说";
                    rbt.IsEnabled = true;
                    ls.ItemsSource = list;
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.Message + " " + e.Source);
            }
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            sp.Visibility = Visibility.Visible;
            main.Visibility = Visibility.Collapsed;
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            sp.Visibility = Visibility.Collapsed;
            main.Visibility = Visibility.Visible;
            AddToList(new fiction(fic_t.Text, fic_w.Text));
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            sp.Visibility = Visibility.Collapsed;
            main.Visibility = Visibility.Visible;
        }


        private async void tg1_Toggled(object sender, RoutedEventArgs e)
        {


            var toggle = sender as ToggleSwitch;
            if (toggle.IsOn)
            {
                var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                    taskBuilder.Name = "TaskOne";
                    taskBuilder.TaskEntryPoint = "BackGoundTask.Taskone";
                    taskBuilder.SetTrigger(new TimeTrigger(30, false));
                    taskBuilder.AddCondition(new SystemCondition(SystemConditionType.FreeNetworkAvailable));
                    var registration = taskBuilder.Register();
                }
            }
            else {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == "TaskOne")
                    {
                        task.Value.Unregister(true);
                    }
                }
            }
        }

        private void tg2_Toggled(object sender, RoutedEventArgs e)
        {
            var tg_tmp = sender as ToggleSwitch;
            if (tg_tmp.IsOn)
            {
                web_s = true;
            } else
            {
                web_s = false;
            }
        }
    }
}

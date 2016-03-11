using sqlite.share;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackGoundTask
{
    public sealed class Taskone : IBackgroundTask
    {
        private List<fiction> fiction_list;

        private void Update(string title, string newchapter = "")
        {
            int num = 0;
            for (int i = 0; i < fiction_list.Count; i++)
            {
                if (fiction_list[i].Name == title)
                {
                    num = fiction_list[i].Id;
                    break;
                }
            }
            using (var dbConn = new SQLiteConnection(Path.Combine(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "fion_d.sqlite"))))
            {
                var existingconact = dbConn.Query<fiction>("select * from fiction where Id =" + num).FirstOrDefault();
                if (existingconact != null)
                {
                        dbConn.RunInTransaction(() =>
                        {
                            existingconact.Newchapter = newchapter;
                            dbConn.Update(existingconact);
                        });
                }
            }
        }

        private async Task DoWebSearch()
        {
            string notify_text = "";
            string initdata;
            foreach (var fiction_one in fiction_list)
            {
                initdata = WebHelper.StripHTML(await WebHelper.getHtml(fiction_one.Website));
                initdata = WebHelper.getNeededData(new Regex(@"\u6700\u65b0\u7ae0\u8282(?<data>.*)\(\u66f4\u65b0"), initdata);
                if (initdata.Length < 35 && fiction_one.Newchapter != initdata)
                {
                    fiction_one.Newchapter = initdata;
                    Update(fiction_one.Name, initdata);
                    notify_text += fiction_one.Name + " |...| ";
                }

            }
            Notification(notify_text);
        }

        private void Notification(string notify_list)
        {
            if (notify_list != "")
            {

                XmlDocument doctoast = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                var txtnodes = doctoast.GetElementsByTagName("text");
                if (txtnodes.Count > 1)
                {
                    // 修改XML值
                    ((XmlElement)txtnodes[0]).InnerText = "New Online:";
                    ((XmlElement)txtnodes[1]).InnerText = notify_list;
                }
                // 发送Toast通知
                ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(doctoast));
                notify_list = "";
            }
        }
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            using (var dbConn = new SQLiteConnection(Path.Combine(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "fion_d.sqlite"))))
            {
                fiction_list  = dbConn.Table<fiction>().ToList<fiction>();
            }
            await DoWebSearch();
            deferral.Complete();
        }
    }
}

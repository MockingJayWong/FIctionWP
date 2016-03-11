using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sqlite.share
{
    public class WebHelper
    {
        public static async Task<string> getHtml(string addr)
        {
            string htmlCode = "";
            var client = new HttpClient();

            // 准备获取用户头像及ID
            try
            {
                // 开始联网
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                var response = await client.GetAsync(addr);
                response.EnsureSuccessStatusCode();

                // 加载完成
                htmlCode = await response.Content.ReadAsStringAsync();
                //htmlCode = htmlCode.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", "").Replace("&amp;", "").Replace(" ", " ").Replace("  ", " "); ;
            }
            catch (HttpRequestException)
            {
                // 网络连接异常
            }
            catch (Exception)
            {
                // 其他异常

            }

            return htmlCode;
        }

        public static string StripHTML(string strHtml)
        {
            //regex_str="<script type=\\s*[^>]*>[^<]*?</script>";//替换<script>内容</script>为空格
            string regex_str = "(?is)<script[^>]*>.*?</script>";//替换<script>内容</script>为空格
            strHtml = Regex.Replace(strHtml, regex_str, "");

            //regex_str="<script type=\\s*[^>]*>[^<]*?</script>";//替换<style>内容</style>为空格
            regex_str = "(?is)<style[^>]*>.*?</style>";//替换<style>内容</style>为空格
            strHtml = Regex.Replace(strHtml, regex_str, "");

            //regex_str = "(&nbsp;)+";//替换&nbsp;为空格
            regex_str = "(?i)&nbsp;";//替换&nbsp;为空格
            strHtml = Regex.Replace(strHtml, regex_str, " ");

            //regex_str = "(\r\n)*";//替换\r\n为空
            regex_str = @"[\r\n]*";//替换\r\n为空
            strHtml = Regex.Replace(strHtml, regex_str, "", RegexOptions.IgnoreCase);

            //regex_str = "<[^<]*>";//替换Html标签为空
            regex_str = "<[^<>]*>";//替换Html标签为空
            strHtml = Regex.Replace(strHtml, regex_str, "");

            //regex_str = "\n*";//替换\n为空
            regex_str = @"\n*";//替换\n为空
            strHtml = Regex.Replace(strHtml, regex_str, "", RegexOptions.IgnoreCase);

            //可以这样
            regex_str = "\t*";//替换\t为空
            strHtml = Regex.Replace(strHtml, regex_str, "", RegexOptions.IgnoreCase);

            //可以
            regex_str = "'";//替换'为’
            strHtml = Regex.Replace(strHtml, regex_str, "’", RegexOptions.IgnoreCase);

            //可以
            regex_str = " +";//替换若干个空格为一个空格
            strHtml = Regex.Replace(strHtml, regex_str, "  ", RegexOptions.IgnoreCase);

            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);

            string strOutput = regex.Replace(strHtml, "");//替换掉"<"和">"之间的内容
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("&nbsp;", "");


            return strOutput;

        }

        public static string getNeededData(Regex para, string tmp)
        {
            //正则表达式内容
            //string match = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*$";
            //string match = @"[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*$";
            string match = @"[a-zA-z]+://[^\s]*";
            //初始化正则表达式实例
            Regex reg = new Regex(match);
            //开始验证
            string tmpend, output = "";
            if (true)
            {
                try
                {
                    //MessageBox.Show("这是网站有效URL格式。");

                    //tmp = await getHtml(s1);
                    tmpend = StripHTML(tmp);
                    //tmpend = Regex.Replace(tmpend, "[^\u4e00-\u9fa5]" , "");  //剔除非中文



                    Match outp = para.Match(tmpend);
                    var a_ = outp.Groups["data"].Value;

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


    }
}

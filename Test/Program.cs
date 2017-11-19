using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using HtmlAgilityPack;
using PageExtractor.Models;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> unload = new Dictionary<string, int>();
            Dictionary<string, int> loaded = new Dictionary<string, int>();

            unload.Add("http://my.yingjiesheng.com/xuanjianghui_province_11.html", 0);
            string baseUrl = "my.yingjiesheng.com";

            while (unload.Count > 0)
            {
                string url = unload.First().Key;
                int depth = unload.First().Value;
                loaded.Add(url, depth);
                unload.Remove(url);

                Console.WriteLine("Now loading " + url);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.Accept = "text/html";
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)";

                try
                {
                    Encoding GB18030 = Encoding.GetEncoding("GB18030");   // GB18030兼容GBK和GB2312
                    Encoding _encoding = GB18030;

                    string html = null;

                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), _encoding))
                    {
                        html = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(html))
                        {
                            Console.WriteLine("Download OK!\n");
                        }
                    }

                    GetJobInfo(html);

                    string[] links = GetLinks(html);
                    AddUrls(links, depth + 1, baseUrl, unload, loaded);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.Message);
                }
            }
        }

        private static string[] GetLinks(string html)
        {
            const string pattern = @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection m = r.Matches(html);
            string[] links = new string[m.Count];

            for (int i = 0; i < m.Count; i++)
            {
                links[i] = m[i].ToString();
            }
            return links;
        }

        private static bool UrlAvailable(string url, Dictionary<string, int> unload, Dictionary<string, int> loaded)
        {
            if (unload.ContainsKey(url) || loaded.ContainsKey(url))
            {
                return false;
            }
            if (url.Contains(".jpg") || url.Contains(".gif")
                || url.Contains(".png") || url.Contains(".css")
                || url.Contains(".js"))
            {
                return false;
            }
            return true;
        }

        private static void AddUrls(string[] urls, int depth, string baseUrl, Dictionary<string, int> unload, Dictionary<string, int> loaded)
        {
            if (depth >= 3)
            {
                return;
            }
            foreach (string url in urls)
            {
                string cleanUrl = url.Trim();
                int end = cleanUrl.IndexOf(' ');
                if (end > 0)
                {
                    cleanUrl = cleanUrl.Substring(0, end);
                }
                if (UrlAvailable(cleanUrl, unload, loaded))
                {
                    if (cleanUrl.Contains(baseUrl))
                    {
                        unload.Add(cleanUrl, depth);
                    }
                    else
                    {
                        // 外链
                    }
                }
            }
        }

        private static void GetJobInfo(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(html);

            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            int count = 0;

            var jobinfos = new List<JobInfo>();

            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
            {
                foreach (HtmlNode row in table.SelectNodes("tr"))
                {
                    if (!row.InnerText.Contains("举办日期"))
                    {
                        var jobInfo = new JobInfo();
                        //foreach (var cell in row.SelectNodes("td"))
                        //{
                        //    jobInfo.CollegeName = cell.InnerText;

                        //}
                        for (int i = 0; i < row.SelectNodes("td").Count(); i++)
                        {
                            jobInfo.CollegeName = row.SelectNodes("td")[i].InnerText;
                        }
                    }
                }


                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://my.yingjiesheng.com/xjh-000-818-836.html");
                req.Method = "GET";
                req.Accept = "text/html";
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)";

                try
                {
                    Encoding GB18030 = Encoding.GetEncoding("GB18030");   // GB18030兼容GBK和GB2312
                    Encoding _encoding = GB18030;

                    string html2 = null;

                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), _encoding))
                    {
                        html2 = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(html2))
                        {
                            Console.WriteLine("Download OK!\n");
                        }
                    }

                    GetJobInfo2(html2);

                    string[] links = GetLinks(html2);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.Message);
                }



            }
        }

        private static void GetJobInfo2(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(html);

            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            int count = 0;

            var jobinfos = new List<JobInfo>();

            var contents = doc.DocumentNode.SelectNodes("//*[@id='wrap']/div[2]/div[1]/div[3]/div[2]/div[1]");

            //foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
            //{
            //    foreach (HtmlNode row in table.SelectNodes("tr"))
            //    {
            //        if (!row.InnerText.Contains("举办日期"))
            //        {
            //            var jobInfo = new JobInfo();
            //            //foreach (var cell in row.SelectNodes("td"))
            //            //{
            //            //    jobInfo.CollegeName = cell.InnerText;

            //            //}
            //            for (int i = 0; i < row.SelectNodes("td").Count(); i++)
            //            {
            //                jobInfo.CollegeName = row.SelectNodes("td")[i].InnerText;
            //            }
            //        }
            //    }
            //}
        }
    }
}

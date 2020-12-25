using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

namespace WebSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            List<article> articles = new List<article>();

            WebRequest request = WebRequest.Create("https://www.ncbi.nlm.nih.gov/bioproject/?term=skin+cancer");
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")); //reader.ReadToEnd() 表示取得网页的源码
            List<string> URL = new List<string>();
            MatchCollection matches = Regex.Matches(reader.ReadToEnd(), @"/bioproject/+[0-9]+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            //foreach (object url in matches)
            //{
            //    URL.Add(url.ToString());
            //}
            string path = @"..\..\..\url.txt";
            string line = "";
            try
            {
                line = myRead(path);
                Console.WriteLine("读取成功");
            }
            catch (Exception e)
            {
                Console.WriteLine("读取失败");
            }
            URL = line.Trim().Split("\n").ToList();

            string url_1 = "https://www.ncbi.nlm.nih.gov";
            int i = 0;
            foreach (string url_2 in URL)
            {
                string url = url_1 + url_2;
                WebRequest req = WebRequest.Create(url);
                WebResponse res = req.GetResponse();
                StreamReader nextreader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                string str = nextreader.ReadToEnd();
                string pattern_1 = "<div id=\"DescrAll\" style=\"display:none\">[\\S\\s]+?<";
                string pattern_2 = "Registration date:[\\S\\s]+?<";

                MatchCollection match_1 = Regex.Matches(str, pattern_1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection match_2 = Regex.Matches(str, pattern_2, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string summary = "", time = "";
                Date date = new Date();
                bool flag1 = false, flag2 = false;
                try
                {
                    if (match_1[0] != null)
                    {
                        summary = match_1[0].ToString();
                        summary = summary.Remove(0, 40);
                        summary = summary.Remove(summary.Length - 1, 1);
                        flag1 = true;
                        Console.WriteLine(i + "爬取成功");
                    }
                    if (match_2[0] != null)
                    {
                        time = match_2[0].ToString();
                        time = time.Remove(0, 19);
                        time = time.Remove(time.Length - 1, 1);
                        string[] info = time.Trim().Split("-");
                        date.day = int.Parse(info[0]);
                        date.getmonth(info[1]);
                        date.year = int.Parse(info[2]);
                        flag2 = true;
                    }
                    if (flag1)
                    {
                        article art = new article(date, summary);
                        articles.Add(art);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(i + "爬取失败");
                }
                i++;
            }
            while(true)
            {
                Console.WriteLine("请输入热词年份[20xx]");
                Console.WriteLine();
                string year_ = Console.ReadLine();
                Console.WriteLine("是否继续？[0:否1:季度2:月份]");
                Console.WriteLine();
                string flag = Console.ReadLine();
                if (flag == "0")
                {
                    string str = "";
                    int number = 0;
                    foreach (article a in articles)
                    {
                        if (a.time.year == int.Parse(year_))
                        {
                            number++;
                            str += a.summary;
                        }
                    }
                    Console.WriteLine("论文有" + number + "篇");
                    noun(str);
                }
                if (flag == "1")
                {
                    Console.WriteLine("请输入热词季度[1,2,3,4]");
                    Console.WriteLine();
                    string season_ = Console.ReadLine();
                    string str = "";
                    int number = 0;
                    foreach (article a in articles)
                    {
                        if (a.time.year == int.Parse(year_) && a.time.season == int.Parse(season_))
                        {
                            number++;
                            str += a.summary;
                        }
                    }
                    Console.WriteLine("论文有" + number + "篇");
                    noun(str);
                }
                if (flag == "2")
                {
                    Console.WriteLine("请输入热词月份[1-12]");
                    Console.WriteLine();
                    string month_ = Console.ReadLine();
                    string str = "";
                    int number = 0;
                    foreach (article a in articles)
                    {
                        if (a.time.year == int.Parse(year_) && (int)a.time.month == int.Parse(month_))
                        {
                            number++;
                            str += a.summary;
                        }
                    }
                    Console.WriteLine("论文有" + number + "篇");
                    noun(str);
                }
            }          
        }
        public static void noun(string str)
        {
            string[] sp = str.Split(' ');
            var temp = sp.GroupBy(i => i).ToList();
            temp.Sort((x, y) => -x.Count().CompareTo(y.Count()));
            temp.ForEach(i => {
                string danci = i.Key;
                int cishu = i.Count();
                Console.WriteLine(danci + " : " + cishu);
            });
        }

        static public string myRead(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            line = sr.ReadToEnd();
            return line;
        }
    }
    class article
    {
        public Date time;
        public string summary;
        public article(Date d,string s)
        {
            time = d;
            summary = s;
        }
    }

    public enum Month {Jan = 1,Feb = 2,Mar = 3,Apr = 4,May = 5,Jun = 6,Jul = 7,Aug = 8,Sep = 9,Oct = 10,Nov = 11,Dec = 12}

    class Date
    {
        public int year { get; set; }
        public Month month { get; set; }
        public int day { get; set; }
        public int season;
        public Date() { }
        public Date(int y,Month m,int d)
        {
            year = y;
            month = m;
            day = d;
        }

        public void getmonth(string m)
        {
            if (m == "Jan")
                month = Month.Jan;
            else if (m == "Feb")
                month = Month.Feb;
            else if (m == "Mar")
                month = Month.Mar;
            else if (m == "Apr")
                month = Month.Apr;
            else if (m == "May")
                month = Month.May;
            else if (m == "Jun")
                month = Month.Jun;
            else if (m == "Jul")
                month = Month.Jul;
            else if (m == "Aug")
                month = Month.Aug;
            else if (m == "Sep")
                month = Month.Sep;
            else if (m == "Oct")
                month = Month.Oct;
            else if (m == "Nov")
                month = Month.Nov;
            else if (m == "Dec")
                month = Month.Dec;
            else
                Console.WriteLine("Time Error");

            season = ((int)month - 1) / 3 + 1;
        }
    }
}


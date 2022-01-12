using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

namespace epub2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        }
        String Rstring;
        
        
         void button1_Click(object sender, EventArgs e)
         {
            Rstring = textBox1.Text;
            //textBlock1.Text = Rstring;
            List<string> titleList = new List<string>();
            List<string> hrefList = new List<string>();
            List<string> contentList = new List<string>();
            File.WriteAllText(@"C:\Users\97489\Desktop\test.txt", "");

            var html = @"https://www.uukanshu.com/b/167/";

            HttpClient client = new HttpClient();
            
            var Task = client.GetStringAsync(html);
            Task.Wait();
            var result = Task.Result;

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//body/div[@class='xiaoshuo_content clear']/div[@class='zhangjie clear']/ul[@id='chapterList']/li/a");

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var titleContent = node.Attributes["title"].Value;
                var hrefContent = node.Attributes["href"].Value;
                titleList.Add(titleContent);
                hrefList.Add(hrefContent);

                var newHTML = "https://www.uukanshu.com" + node.Attributes["href"].Value;
                HtmlWeb newWeb = new HtmlWeb();

                var newHtmlDoc = web.Load(newHTML);

                var newNode = newHtmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='zhengwen_box']/div[@class='box_left']/div[@class='w_main']/div[@class='contentbox']/div[@id='contentbox']");
                var chapterContent = newNode.InnerText;
                chapterContent = chapterContent.Replace("&nbsp;", "\t");
                contentList.Add(chapterContent);
                
                //string readText = File.ReadAllText(@"C:\Users\97489\Desktop\test.txt");

            }

            for (int i = contentList.Count-1;i > 0; i--)
            {
                File.AppendAllText(@"C:\Users\97489\Desktop\test.txt", titleList[i] + Environment.NewLine + contentList[i] + Environment.NewLine);
            }

            textBlock1.Text = "Download finished";


        }

        
    }
}

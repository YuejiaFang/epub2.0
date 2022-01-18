using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        
        
         void button1_Click(object sender, EventArgs e)
         {
            //textBlock1.Text = Rstring;
            File.WriteAllText(@"C:\Users\97489\Desktop\test.txt", "");

            var html = @textBox1.Text;

            // HttpClient client = new HttpClient();

            // var Task = client.GetStringAsync(html);
            // Task.Wait();
            // var result = Task.Result;

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//body/div[@class='xiaoshuo_content clear']/div[@class='zhangjie clear']/ul[@id='chapterList']/li/a");
            var title = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='xiaoshuo_content clear']/dl[@class='jieshao']/dd[@class='jieshao_content']/h1/a").InnerText;
            var author = htmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='xiaoshuo_content clear']/dl[@class='jieshao']/dd[@class='jieshao_content']/h2/a").InnerText;

            //Now Create all of the directories
            var sourcePath = @"C:\Users\97489\Desktop\text";
            var targetPath = @"C:\Users\97489\Desktop\targetPath";
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }

            int numNodes = nodes.Count;
            string[] titleList = new string[numNodes];
            string[] contentList = new string[numNodes];


            Parallel.For(0, numNodes, i =>
            {   
                var node = nodes[i];
                var titleContent = node.Attributes["title"].Value;
                var hrefContent = node.Attributes["href"].Value;
                titleList[i] = titleContent; // concurrent bag

                var newHTML = "https://www.uukanshu.com" + node.Attributes["href"].Value;
                HtmlWeb newWeb = new HtmlWeb();

                var newHtmlDoc = web.Load(newHTML);

                var newNode = newHtmlDoc.DocumentNode.SelectSingleNode("//body/div[@class='zhengwen_box']/div[@class='box_left']/div[@class='w_main']/div[@class='contentbox']/div[@id='contentbox']");
                var chapterContent = newNode.InnerText;
                chapterContent = chapterContent.Replace("&nbsp;", "\t");
                contentList[i] = chapterContent;
                
                //string readText = File.ReadAllText(@"C:\Users\97489\Desktop\test.txt");

            });
            int n = numNodes;

            for (int i = n-1;i >= 0; i--)
            {
                File.AppendAllText(@"C:\Users\97489\Desktop\test.txt", titleList[i] + Environment.NewLine + contentList[i] + Environment.NewLine);
                var path = @"C:\Users\97489\Desktop\targetPath\OEBPS\ch" + (n - i).ToString() + ".html";

                File.Copy(@"C:\Users\97489\Desktop\text\OEBPS\ch1.html", path, true);

                File.WriteAllText(path, Regex.Replace(File.ReadAllText(path), "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", contentList[i]));
                File.WriteAllText(path, Regex.Replace(File.ReadAllText(path), "Xtitle", titleList[i]));
                // string readText = File.ReadAllText(path);
            }

            string manifest = "";
            string spine = "";
            string navMap = "";
            for (int i = 0; i < numNodes; i++)
            {
                manifest = manifest + "<item href=\"ch" + (i+1).ToString() + ".html\" id=\"ch" + (i + 1).ToString() + "\" media-type=\"application/xhtml+xml\"/>" + Environment.NewLine;
                spine = spine + "<itemref idref=\"ch" + (i + 1).ToString() + "\"/>" + Environment.NewLine;
                navMap = navMap + "<navPoint id=\"ch" + (i + 1).ToString() + "\" playOrder=\"" + (i + 1).ToString() + "\"><navLabel><text>" + titleList[n-i-1] + "</text></navLabel><content src=\"ch" + (i + 1).ToString() + ".html\"/></navPoint>" + Environment.NewLine;
            }
            var opfPath = @"C:\Users\97489\Desktop\targetPath\OEBPS\content.opf";
            var ncxPath = @"C:\Users\97489\Desktop\targetPath\OEBPS\toc.ncx";
            File.WriteAllText(opfPath, Regex.Replace(File.ReadAllText(opfPath), "XmanifestX", manifest));
            File.WriteAllText(opfPath, Regex.Replace(File.ReadAllText(opfPath), "<itemref idref=\"ch1\"/>", spine));
            File.WriteAllText(opfPath, Regex.Replace(File.ReadAllText(opfPath), "XtitleX", title));
            File.WriteAllText(opfPath, Regex.Replace(File.ReadAllText(opfPath), "XauthorX", author));
            File.WriteAllText(ncxPath, Regex.Replace(File.ReadAllText(ncxPath), "XnavMapX", navMap));
            File.WriteAllText(ncxPath, Regex.Replace(File.ReadAllText(ncxPath), "XtitleX", title));

            textBlock1.Text = "Download finished";
            var epubPath = @"C:\Users\97489\Desktop\" + title + ".epub";
            ZipFile.CreateFromDirectory(targetPath, epubPath);

        }

        
    }
}

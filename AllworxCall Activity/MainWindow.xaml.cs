using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml.XPath;
using Microsoft.Win32;
using HtmlAgilityPack;

namespace AllworxCall_Activity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void btnOpenRawCallList_Click(object sender, RoutedEventArgs e)
        {

            lblStatus.Content = "Waiting for call list to be loaded";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "HTML Files (.htm)|*.htm|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            bool? userClickedOK = openFileDialog1.ShowDialog();

            if (userClickedOK == true)
            {
              
                // load html invoice
                HtmlDocument htmlSnippet = new HtmlDocument();
                htmlSnippet = LoadHtmlSnippetFromFile(openFileDialog1.FileName.ToString());
                

                // extract call data
                bool callTags = new bool();
                callTags = CreateCSV(htmlSnippet);


            }


        }

        private HtmlDocument LoadHtmlSnippetFromFile(String filename)
        {
            lblStatus.Content = "Call list loaded...preparing file.";
            TextReader reader = File.OpenText(filename);
            HtmlDocument doc = new HtmlDocument();
            doc.Load(reader);

            reader.Close();

            return doc;
        }

      

        private bool CreateCSV(HtmlDocument htmlSnippet)
        {
            int count = 0;
            string[] nodestocall = new string[3] { "//article[h5='Incoming Toll Free (800) 742-3763']", "//article[h5='Incoming Toll Free (800) 323-1190']", "//article[h5='Outgoing']" };
            

            string path = @"callslist.csv";


            if (!File.Exists(path))
            {

                using (StreamWriter sw = File.CreateText(path))
                {
                    for (int i = 0; i < nodestocall.Length; i++)
                    {
                        foreach (HtmlNode outertag in htmlSnippet.DocumentNode.SelectNodes(nodestocall[count]))
                        {

                            //this will get you the text in the H5 tag
                            //outertag.SelectSingleNode("h5").InnerText
                            foreach (HtmlNode tags in outertag.SelectNodes("div/table/tbody/tr"))
                            {
                                //MessageBox.Show(tags.InnerText);
                                int fieldcount = 0;
                                foreach (HtmlNode tag in tags.SelectNodes("td"))
                                {
                                    if (fieldcount < 8)
                                    {
                                        sw.Write(tag.InnerText + ","); 
                                    }
                                    else
                                    {
                                        sw.Write(tag.InnerText);
                                    }
                                    //MessageBox.Show(tag.InnerText);
                                    fieldcount++;
                                }
                                sw.WriteLine();
                            }
                        }
                        count++;
                    }  
                }
                lblStatus.Content = "File is complete.";
            }
            else
            {
                lblStatus.Content = "The output file already exists.  Delete and try again.";
            }
            return true;   
        }
    }
}

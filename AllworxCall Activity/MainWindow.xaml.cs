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
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using Microsoft.VisualBasic.FileIO;


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

                //import csv into database
                InsertDataIntoSQLServerUsingSQLBulkCopy(GetDataTableFromCSVFile(@"c:\imports\chargedcalls.csv"));


                


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

        private void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData)
        {
            using(SqlConnection dbConnection = new SqlConnection(@"Data Source=PIC-9R0QYZ1\LOCALSQL;Initial Catalog=Allworx;User ID=sa;Password=password"))
            {
                 dbConnection.Open();
                 using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                 {
                     s.DestinationTableName = "chargedcalls";
                     s.WriteToServer(csvFileData);
                 }
             }

            lblStatus.Content = "Database has been loaded.  Complete.";
         }

        private static DataTable GetDataTableFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {
                using(TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }

                    //need to add this row so we don't miss it
                    csvData.Rows.Add(colFields);

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                        
                    }
                }
            }
            catch (AmbiguousMatchException)
            {
        }
            return csvData;
     }
  
    
        private bool CreateCSV(HtmlDocument htmlSnippet)
        {
            int count = 0;
            string[] nodestocall = new string[3] { "//article[h5='Incoming Toll Free (800) 742-3763']", "//article[h5='Incoming Toll Free (800) 323-1190']", "//article[h5='Outgoing']" };
            

            string path = @"c:\imports\chargedcalls.csv";


            if (!File.Exists(path))
            {

                using (StreamWriter sw = File.CreateText(path))
                {
                    for (int i = 0; i < nodestocall.Length; i++)
                    {
                        foreach (HtmlNode outertag in htmlSnippet.DocumentNode.SelectNodes(nodestocall[count]))
                        {

                            foreach (HtmlNode tags in outertag.SelectNodes("div/table/tbody/tr"))
                            {
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

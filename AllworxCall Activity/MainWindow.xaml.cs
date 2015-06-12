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
                 dbConnection.Close();
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
                        //  added this so we can check to see if there were calls made to each 800 number first
                        //  this avoids null exception errors when an 800 number wasn't on the invoice
                        var outertags = htmlSnippet.DocumentNode.SelectNodes(nodestocall[count]);
                        if (outertags != null)
                        {

                            
                           foreach (var outertag in outertags)
                           //foreach (HtmlNode outertag in htmlSnippet.DocumentNode.SelectNodes(nodestocall[count]))
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

        private void btnOutCheck_Click(object sender, RoutedEventArgs e)
        {
            string[] knownFromNumbers = new string[22] { "(401) 433-3100", "(401) 434-4547", "(401) 434-8599", "(401) 725-8060", "(401) 732-1244", "(401) 732-3226", "(401) 732-3700", "(401) 737-5200", "(401) 738-7856", "(401) 738-8333", "(401) 765-1320", "(401) 785-1161", "(401) 827-9971", "(401) 847-3110", "(401) 849-2700", "(401) 942-5585", "(401) 942-6768", "(401) 943-7131", "(860) 225-2471", "(860) 229-0173", "(860) 229-7536", "(860) 229-8133" };
            string[] knownToNumbers = new string[2] {"(401) 808-0834","(401) 276-6715"};
            bool matched = false;
            int i = 0;

            using (SqlConnection dbConnection = new SqlConnection(@"Data Source=PIC-9R0QYZ1\LOCALSQL;Initial Catalog=Allworx;User ID=sa;Password=password"))
            {
                dbConnection.Open();
                try
                {
                    SqlDataReader myReader = null;
                    SqlCommand myCommand = new SqlCommand("select * from chargedcalls where calltype in ('domestic') order by calltype",
                                                             dbConnection);
                    myReader = myCommand.ExecuteReader();
                    while (myReader.Read())
                    {
                        lblOutCheck.Content = "New record being evaluated";
                        //Console.WriteLine(myReader["placedto"].ToString() + " was called from " + myReader["placedfrom"].ToString());
                        while(matched == false)
                        {
                            lblOutCheck.Content = "Check started";
                            Console.WriteLine(knownFromNumbers[i].ToString() + " matches " + myReader["placedfrom"].ToString());
                            if (knownFromNumbers[i].ToString() == myReader["placedfrom"].ToString())
                            {
                                matched = true;
                                i = 0;
                            }
                            else
                            {
                                if(i < knownFromNumbers.Length - 1)
                                {
                                    i++;
                                }
                                else
                                {
                                    i = 0;
                                    matched = true; 
                                    lblOutCheck.Content = "Found orphan";
                                    using (StreamWriter w = File.AppendText(@"c:\exports\log.txt"))
                                    {
                                        w.WriteLine(myReader["placedto"].ToString() + " was called from " + myReader["placedfrom"].ToString());
                                    }
                                }
                            }
                        }
                        matched = false;
                    }
                    lblOutCheck.Content = "Check complete";
                }
                catch (Exception d)
                {
                    Console.WriteLine(d.ToString());
                }
                dbConnection.Close();
            }

        }
    }
}

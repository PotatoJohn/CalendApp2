using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;


namespace Calendar_App_Test_2
{
    public partial class Form1 : Form
    {
        private static string Lesson {get; set;}
        private static string Today { get; set;}
        private static string LearningGoal { get; set;}

        public Form1()
        {
            string output = "";
            InitializeComponent();
            output = CalenderPrint();
            label1.Text = output;
            label2.Text = "Lesson " + Lesson;
            Lesson = "";
            label3.Text = Today;
            label4.Text = LearningGoal;
            

           


        }
        //-------------------------------------------------------------------------

        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        public static string CalenderPrint()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            string description = "Upcoming Events:";
            int count = 0;
            int lG1 = 0, lG2 = 0;

            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                        if (Today == null)
                        {
                            Today = when;
                        }
                    }

                    description = description + "\n" + when + " " + eventItem.Summary;

                    if (count == 0)
                    {
                       for(int i = 0; i < eventItem.Summary.Length; i++)
                        {
                           if (char.IsDigit(eventItem.Summary[i]))
                            {
                                Lesson = eventItem.Summary.Substring(i, 3);
                                i = eventItem.Summary.Length;
                            }
                        }            
                       
                       for(int i = 0; i < eventItem.Description.Length; i++)
                        {
                            lG1 = eventItem.Description.IndexOf("]");
                            lG2 = eventItem.Description.LastIndexOf("[");
                            LearningGoal = eventItem.Description.Substring(lG1+1, lG2 - lG1 - 1);
                            i = eventItem.Description.Length;
                        }
                    }
                    count++; 

                    

                    //if (String.IsNullOrEmpty(description))
                    //{ }
                    //else
                    //{
                      //  LG = LearningGoal(description);
                    //}
                    //Console.WriteLine(LG);

                }
            }
            else
            {
                //Console.WriteLine("No upcoming events found.");
            }
            
            return (description);

        }
        /*public static string LearningGoal(string str)
        {

            int sep = 0;
            string lg = "";

            //for(int i = 1; i <= str.Length; i++)
            sep = str.IndexOf('[', 2);
            lg = str.Substring(0, sep - 8);

            return (lg);
        } 
        */


        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}

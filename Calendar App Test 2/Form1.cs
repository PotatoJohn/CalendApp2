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
        private static string LearningTarget { get; set;}
        private static string HomeWork { get; set; }
        private static string Quiz { get; set; }
        private static string TestDate { get; set; }
        private static string QuizDate { get; set; }

        public Form1()
        {
            string output = "";
            InitializeComponent();
            output = CalenderPrint();
            //label1.Text = output;
            label2.Text = "Lesson " + Lesson;
            Lesson = "";
            label3.Text = Today;
            label4.Text = "Learning Target: " + LearningTarget;
            label5.Text = "HW: " + HomeWork;
            if (Quiz == null)
            {
                label6.Text = "";
            }
            else
            {
                label6.Text = Quiz;
            }
            
            label7.Text = TestDate;
            label8.Text = QuizDate;

            

           


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
            request.MaxResults = 14;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            string description = "Upcoming Events:";
            int count = 0;
            int lG1 = 0, lG2 = 0;
            Boolean foundQuiz = false, foundTest = false;

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
                       for(int i = 0; i < eventItem.Summary.Length; i++) //lesson
                        {
                           if (char.IsDigit(eventItem.Summary[i]))
                            {
                                Lesson = eventItem.Summary.Substring(i, 3);
                                i = eventItem.Summary.Length;
                            }
                        }

                        if (eventItem.Description != null)
                        {

                            for (int i = 0; i < eventItem.Description.Length; i++) //learning target
                            {
                                lG1 = eventItem.Description.IndexOf("]");
                                lG2 = eventItem.Description.LastIndexOf("[");
                                LearningTarget = eventItem.Description.Substring(lG1 + 1, lG2 - lG1 - 1);
                                while (LearningTarget.IndexOf("<") >= 0)
                                {
                                    int loc = LearningTarget.IndexOf("<");
                                    int closeloc = LearningTarget.IndexOf(">");
                                    LearningTarget = LearningTarget.Remove(loc, closeloc - loc + 1);
                                }
                                i = eventItem.Description.Length;
                            }
                        }
                        else
                        {
                            LearningTarget = "'Yeet' - The Council";
                        }
                       
                       if (!eventItem.Summary.Contains("Quiz"))
                        {
                            HomeWork = eventItem.Summary;
                                
                        }
                       else if (eventItem.Summary.Contains("Quiz") || eventItem.Summary.Contains("Test"))
                        {
                            Quiz = eventItem.Summary;
                                
                        }
                        
                       for (int i = 1; i < events.Items.Count; i++) //upcoming assignment                     
                        {
                            if (!foundQuiz)
                            {
                                if (events.Items[i].Summary.Contains("Quiz"))
                                {
                                    QuizDate = "Upcoming Quiz: " + events.Items[i].Start.Date;
                                    foundQuiz = true;
                                }
                                if (!foundQuiz && i == events.Items.Count)
                                {
                                    
                                    QuizDate = "No Quiz For 2 Weeks";
                                }
                            }
                            if (!foundTest)
                            {
                                if (events.Items[i].Summary.Contains("Test"))
                                {
                                    TestDate = "Upcoming Test: " + events.Items[i].Start.Date;
                                    foundTest = true;
                                }
                                if (!foundTest && i == events.Items.Count)
                                {
                                    TestDate = "No Test For 2 Weeks";
                                }
                            }
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

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}

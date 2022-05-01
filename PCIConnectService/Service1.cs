
using System;
using System.Collections.Generic;

using System.IO;

using System.Net.Http;
using System.ServiceProcess;

using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace PCIConectService
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer();
      
        public Service1()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
           WriteToFile("Service is started at " + DateTime.Now);

            // GetCalendar();
            ReturnPCIDisconnectAsync();
            string x = await ReturnPCIConnectAsync("*", "*");
            WriteToFile(x);
       
            timer.Elapsed += new ElapsedEventHandler(Timer_ElapsedAsync);
            timer.Interval = 300000; //number in milisecs
            timer.Enabled = true;
        }

        private async void Timer_ElapsedAsync(object sender, ElapsedEventArgs e)
        {
           
            WriteToFile("Service is recalled at " + DateTime.Now);

          
            WriteToFile(ReturnPCIDisconnectAsync());

            string x = await ReturnPCIConnectAsync("*", "*");
            WriteToFile(x);

            WriteToFile(x);
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }


        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_PCIConnect_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }



        public static async Task<string> ReturnPCIConnectAsync(string username, string password)
        {
            string result = string.Empty;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("sid","0")
            });

            var url = "https://uo-pci-login.use.ucdp.net:10408/netaccess/loginuser.html";

            //use browser to grab scripts.

          
          //  System.Diagnostics.Process.Start("msedge.exe", url);


            using (var client = new HttpClient())
            {

               var response = await client.PostAsync(url, content);
             
                   result =  response.Content.ReadAsStringAsync().Result;
         


            }
            return result;
        }

        public static string  ReturnPCIDisconnectAsync()
        {
            string result = string.Empty;


            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("sid","0"),
                new KeyValuePair<string, string>("logout", "Log Out Now"),


            });

            var url = "url here";
            using (var client = new HttpClient())
            {

                var response = client.PostAsync(url, content);

                //result = response.Content.ReadAsStringAsync().Result;
                result = response.Result.Content.ToString();
                // Console.WriteLine(result);


            }

            return result;

        }

    }
}

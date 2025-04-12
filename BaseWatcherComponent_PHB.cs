using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eto.Forms;
using GH_IO.Serialization;
using gh_quest.CustomClasses;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.UI.CodeEditor;

namespace gh_quest
{
    public class BaseWatcherComponentPHB : GH_Component
    {
        //************************** GLOBAL VARIABLES **************************//
        //public static string Id { get; set; } = "77d216f5-3250-4e42-a196-595b56e798d2";



        //************************** CONSTRUCTOR **************************//
        public BaseWatcherComponentPHB()
        : base("Base Watcher_PHB", "BW",
            "Description",
            "GH Quest", "Primary")

        {
        }

        public override void CreateAttributes()
        {
            m_attributes = new BaseWatcherAttributes(this, LaunchGHQuest);
        }

        public override bool Write(GH_IWriter writer)
        {
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            return base.Read(reader);
        }

        public override void ComputeData()
        {
            base.ComputeData();
        }


        //************************** INPUTS/OUTPUTS **************************//


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }


        //************************** SOLVE INSTANCE **************************//

        protected override void BeforeSolveInstance()
        {

        }

        protected override void AfterSolveInstance()
        {

        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            LoadTutorial tutorialLoader = new LoadTutorial();

           // string filePath = OSManager.GetFilePath("GH_Beginner_Course_Pack", "tutorials.json");
            string tutorialName = "Parametric_Box";
            string filePath = "C:\\Users\\Puja.Bhagat\\gh-quest\\GH_Beginner_Course_Pack\\tutorials.json";
            List<string> tutorialNameList = LoadTutorial.GetAllTutorialNames(filePath);
            TutorialClass tutorialData = LoadTutorial.DeconstructTutorialJson(filePath, tutorialName);

            RhinoApp.WriteLine(tutorialData._Properties._TargetGraph);


        
            tutorialLoader.LoadTutorialPanel(tutorialData, tutorialName);

        }





        //************************** CUSTOM METHODS **************************//

        public void LaunchGHQuest()
        {
            OpenWebPanel();
            StartWebSocketServer();
        }


        // Method to open an Eto web panel
        public void OpenWebPanel()
        {
            RhinoApp.WriteLine("Opening web panel...");
            var webView = new WebView
            {
                Url = new Uri("http://localhost:5173/"), // Replace with your desired URL
                Size = new Eto.Drawing.Size(800, 600)
            };

            var dialog = new Dialog
            {
                Title = "Web Panel",
                ClientSize = new Eto.Drawing.Size(800, 600),
                Content = webView
            };

            dialog.ShowModal();
        }

        // Method to start a WebSocket server
        public async void StartWebSocketServer()
        {
            RhinoApp.WriteLine("Starting WebSocket server...");
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:8181/ws/");
            httpListener.Start();

            RhinoApp.WriteLine("WebSocket server started at ws://localhost:8181/ws/");

            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                WebSocket webSocket = webSocketContext.WebSocket;

                RhinoApp.WriteLine("Client connected!");

                await HandleWebSocketConnection(webSocket);
                }
                else
                {
                context.Response.StatusCode = 400;
                context.Response.Close();
                }
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];

            // Send an initial message to the client
            // Read the dummy.json file into a string
            string filePath = "/Users/ammarnaqvi/Code/Ammar/gh-quest/dummy.json";
            string jsonContent = System.IO.File.ReadAllText(filePath);
            byte[] initialBuffer = Encoding.UTF8.GetBytes(jsonContent);
            await webSocket.SendAsync(new ArraySegment<byte>(initialBuffer), WebSocketMessageType.Text, true, CancellationToken.None);


            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                RhinoApp.WriteLine("Client disconnected!");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                else
                {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                RhinoApp.WriteLine($"Received: {message}");

                // Echo the message back to the client
                byte[] responseBuffer = Encoding.UTF8.GetBytes($"Echo: {message}");
                await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }



        //**************************UTILITIES**************************//

        //Set Component Exposure
        public override GH_Exposure Exposure => GH_Exposure.primary;

        //Override the keywords for searching
        public override IEnumerable<string> Keywords => new List<string>() { };

        //Set Icons
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("e9bbe8bb-8790-4949-b16d-0438bc697fc2");

    }
}
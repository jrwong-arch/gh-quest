using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Eto.Forms;
using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Newtonsoft.Json;
using gh_quest.CustomClasses;

namespace gh_quest
{
    public class BaseWatcherComponent : GH_Component
    {
        //************************** GLOBAL VARIABLES **************************//
        public static string Id { get; set; } = "584cb7bd-05fd-4e50-a7de-d6e47bdf4c4f";
        public List<string> _TutorialsList = new List<string>();
        public int _SelectedTutorialIndex = 0;

        public TutorialClass _ActiveTutorial { get; set; }

        public string _FilePath = "C:\\Users\\Puja.Bhagat\\gh-quest\\GH_Beginner_Course_Pack\\tutorials.json";


        //************************** CONSTRUCTOR **************************//
        public BaseWatcherComponent()
        : base("Base Watcher", "BW",
            "Description",
            "GH Quest", "Primary")

        {
            LoadTutorials();
        }

        public override void CreateAttributes()
        {
            m_attributes = new BaseWatcherAttributes(this, LaunchGHQuest, SelectLesson);
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
            LoadTutorials();
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

        }





        //************************** CUSTOM METHODS **************************//

        public void SelectLesson()
        {
            OpenSelectionForm();
        }

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
                Size = new Eto.Drawing.Size(1200, 1000)
            };

            var dialog = new Dialog
            {
                Title = "Web Panel",
                ClientSize = new Eto.Drawing.Size(1200, 1000),
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

        public GH_Document GetActiveDocument()
        {
            return Grasshopper.Instances.DocumentServer.First();
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];

            // Send an initial message to the client

            GraphSchema userState = new GraphSchema(GetActiveDocument());
            var userScore = ScoreSolution.ScoreGraph(userState);

            byte[] jsonBytes = Convert.FromBase64String(_ActiveTutorial._Properties._TargetGraph);
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            GraphSchema goalState = JsonConvert.DeserializeObject<GraphSchema>(jsonString);

            var questState = new QuestState(userState, goalState, userScore);
            byte[] initialBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(questState));
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

        //Method to Open Lesson Selection Window
        public void OpenSelectionForm()
        {
            int windowWidth = 400;
            int windowHeight = 100;

            Eto.Forms.Form lessonForm = new Eto.Forms.Form
            {
                Title = "Select Lesson",
                Padding = new Eto.Drawing.Padding(10),
                Size = new Eto.Drawing.Size(windowWidth, windowHeight),
                Resizable = false,
                MovableByWindowBackground = true,
                BackgroundColor = Eto.Drawing.Color.FromArgb(245,245,245,255),
                WindowStyle = Eto.Forms.WindowStyle.Utility,
                Topmost = true,
                Location = new Eto.Drawing.Point((int)(Eto.Forms.Mouse.Position.X - (windowWidth / 2.0)), (int)(Eto.Forms.Mouse.Position.Y - (windowHeight / 2.0))),
            };

            Eto.Forms.Button selectButton = new Eto.Forms.Button
            {
                Text = "Select",
                TextColor = Eto.Drawing.Colors.Black,
                Size = new Eto.Drawing.Size(25, 15),
                BackgroundColor = Eto.Drawing.Colors.White,
            };

            selectButton.Click += (object sender, EventArgs e) =>
            {
                GH_Document doc = OnPingDocument();
                doc.ScheduleSolution(100, selectLesson => 
                {
                    //Load Tutorial Stuff
                    _ActiveTutorial = LoadTutorial.DeconstructTutorialJson(_FilePath, _TutorialsList[_SelectedTutorialIndex]);
                    LoadTutorial tutorialLoader = new LoadTutorial();
                    tutorialLoader.LoadTutorialPanel(_ActiveTutorial, _TutorialsList[_SelectedTutorialIndex]);     
                });
            };

            Eto.Forms.DropDown dropDown = new Eto.Forms.DropDown
            {
                TextColor = Eto.Drawing.Colors.Black,
                DataStore = _TutorialsList,
                SelectedIndex = _SelectedTutorialIndex,
            };

            dropDown.SelectedIndexChanged += (object sender, EventArgs e) =>
            {
                int selectedIndex = dropDown.SelectedIndex;
                GH_Document doc = OnPingDocument();
                doc.ScheduleSolution(100, selectLesson => 
                {
                    _SelectedTutorialIndex = selectedIndex;
                });
            };

            TableLayout layout = new TableLayout
            {
                Padding = new Eto.Drawing.Padding(0),
                Spacing = new Eto.Drawing.Size(10, 5),

                Rows = 
                {
                    new Eto.Forms.TableRow(dropDown, selectButton)
                }
            };

            lessonForm.Content = layout;
            lessonForm.Show();
        }

        public void LoadTutorials()
        {
            _TutorialsList = LoadTutorial.GetAllTutorialNames(_FilePath);
        }



        //**************************UTILITIES**************************//

        //Set Component Exposure
        public override GH_Exposure Exposure => GH_Exposure.primary;

        //Override the keywords for searching
        public override IEnumerable<string> Keywords => new List<string>() { "GH Quest" };

        //Set Icons
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid(BaseWatcherComponent.Id);

    }
}
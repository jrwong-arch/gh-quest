using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino;


namespace gh_quest
{
  public class gh_questInfo : GH_AssemblyInfo
  {
    public override string Name => "gh-quest Info";

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "";

    public override Guid Id => new Guid("211e239c-a548-47c0-b48c-cd5a0e130d25");

    //Return a string identifying you or your company.
    public override string AuthorName => "";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "";

    //Return a string representing the version.  This returns the same version as the assembly.
    public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();

    // Method to open an Eto web panel
    public void OpenWebPanel()
    {
      RhinoApp.WriteLine("Opening web panel...");
      var webView = new WebView
      {
        Url = new Uri("http://localhost:5173/"), // Replace with your desired URL
        Size = new Size(800, 600)
      };

      var dialog = new Dialog
      {
        Title = "Web Panel",
        ClientSize = new Size(800, 600),
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

  }
}
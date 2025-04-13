using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Newtonsoft;
using Rhino;
using Rhino.Geometry;

namespace gh_quest.CustomClasses
{
    public class LoadTutorial
    {
        public Guid _TutorialPanelGuid = Guid.Empty;

        public static List<string> GetAllTutorialNames(string jsonFilePath)
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                var jsonParsed = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TutorialClass>>(jsonData);
                List<string> nameList = jsonParsed.Keys.ToList();
                return nameList;
            }

            else { return null; }
        }

        public static TutorialClass DeconstructTutorialJson(string jsonFilePath, string tutorialName)
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                var jsonParsed = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TutorialClass>>(jsonData);

                if (jsonParsed.TryGetValue(tutorialName, out var tutorialData))
                {
                    TutorialClass tutorial = tutorialData;
                    return tutorial;
                }
                
                else
                {
                    RhinoApp.WriteLine($"Json: tutorial data not found for {tutorialName}");
                    return null;
                }
            }
            else { return null; }
        }

        public void LoadTutorialPanel(TutorialClass tutorial, string tutorialName)
        {
            GH_Document doc = Instances.DocumentServer.First();

            //if there is a tutorial on the canvas already, re-wrtie the info on it the panel for it
            if (this._TutorialPanelGuid != Guid.Empty)
            {
                //doc.Objects
                IGH_DocumentObject docObj = doc.Objects.FirstOrDefault(obj => obj.InstanceGuid == this._TutorialPanelGuid);
                GH_Panel panel = docObj as GH_Panel;

                if(panel != null)
                {
                    panel.UserText = panel.UserText = $"Level: {tutorial._Properties._Level} \n What You'll Learn: {tutorial._Properties._Learn} \n Your Goal: {tutorial._Properties._Learn}";
                    panel.CreateAttributes();
                    doc.AddObject(panel, false);
                    
                    panel.Attributes.Pivot = new PointF(0, 0);

                    RectangleF bounds = panel.Attributes.Bounds;
                    float newWidth = 250;
                    float newHeight = 150;
                    panel.Attributes.Bounds = new RectangleF(bounds.X, bounds.Y, newWidth, newHeight);
                    panel.NickName = $"0{tutorial._Properties._TutorialNumber}: {tutorialName}";
                
                    this._TutorialPanelGuid = panel.InstanceGuid;

                    panel.ExpireSolution(false);
                }
                
                else
                {
                    RhinoApp.WriteLine("Panel Is Null");
                }
            }

            else
            {
                GH_Panel panel = new GH_Panel();
                panel.UserText = panel.UserText = $"Level: {tutorial._Properties._Level} \n What You'll Learn: {tutorial._Properties._Learn} \n Your Goal: {tutorial._Properties._Learn}";
                panel.CreateAttributes();
                doc.AddObject(panel, false);
                
                panel.Attributes.Pivot = new PointF(0, 0);

                RectangleF bounds = panel.Attributes.Bounds;
                float newWidth = 250;
                float newHeight = 150;
                panel.Attributes.Bounds = new RectangleF(bounds.X, bounds.Y, newWidth, newHeight);
                panel.NickName = $"0{tutorial._Properties._TutorialNumber}: {tutorialName}";
                
                this._TutorialPanelGuid = panel.InstanceGuid;

                panel.ExpireSolution(false);
            }
        }

        public List<Brep> LoadTutorialGeometry(string folderPath, string tutorialFolder, string tutorialName)
        {
            List<Brep> objectList = new List<Brep>();
            string filePath = Path.Combine(folderPath, tutorialFolder, tutorialName);
            RhinoApp.WriteLine(filePath);

            if(File.Exists(filePath))
            {
                var file3dm = Rhino.FileIO.File3dm.Read(filePath, Rhino.FileIO.File3dm.TableTypeFilter.None, Rhino.FileIO.File3dm.ObjectTypeFilter.Brep);
                var objectTable = file3dm.Objects;

                foreach(var docObject in objectTable)
                {
                    if(docObject.Geometry is Brep)
                    {
                        objectList.Add(docObject.Geometry as Brep);
                    }
                    else
                    {
                        RhinoApp.WriteLine("object is not a brep");
                    }
                }
            }

            return objectList;
        }

    }


    public class TutorialClass
    {
        public TutorialProperties _Properties;

        public TutorialClass(TutorialProperties properties)
        {
            _Properties = properties;
        }
    }



    public class TutorialProperties
    {

        public string _TutorialNumber;
        public string _Level;
        public string _Learn;
        public string _Goal;
        public string _ResultingGeo;
        public string _SolutionScript;
        public string _PsuedoCode;
        public string _TargetGraph;


        public TutorialProperties(string Tutorial_Number, string Level, string Learn, string Goal, 
        string Resulting_Geometry, string Solution_Script, string Pseudocode, string TargetGraph)
        {
            _TutorialNumber = Tutorial_Number;
            _Level = Level;
            _Learn = Learn;
            _Goal = Goal;
            _ResultingGeo = Resulting_Geometry;
            _SolutionScript = Solution_Script;
            _PsuedoCode = Pseudocode;
            _TargetGraph = TargetGraph;
        }
    }

}


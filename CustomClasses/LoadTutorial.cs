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

namespace gh_quest.CustomClasses
{
    public class LoadTutorial
    {
        public Guid _TutorialPanelGuid = Guid.Empty;

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
            //if there is a tutorial on the canvas already, re-wrtie the info on it the panel for it
            if (this._TutorialPanelGuid != Guid.Empty)
            {
                GH_Document doc = Instances.DocumentServer.First();

                RhinoApp.WriteLine(doc.ToString());

                //doc.Objects
                IGH_DocumentObject docObj = doc.Objects.FirstOrDefault(obj => obj.InstanceGuid == this._TutorialPanelGuid);
                GH_Panel panel = docObj as GH_Panel;

                RhinoApp.WriteLine(docObj.ToString());

                if(panel != null)
                {
                    panel.UserText = $"{tutorialName} \n Tutorial Number: {tutorial._Properties._TutorialNumber} \n Level: {tutorial._Properties._Level} \n What You'll Learn: {tutorial._Properties._Learn} \n Your Goal: {tutorial._Properties._Learn}";
                    RhinoApp.WriteLine(panel.UserText.ToString());
                    panel.ExpireSolution(true);
                }
                else
                {
                    RhinoApp.WriteLine("Panel Is Null");
                }
            }

            else
            {
                GH_Panel panel = new GH_Panel();
                panel.UserText = panel.UserText = $"{tutorialName} \n Tutorial Number: {tutorial._Properties._TutorialNumber} \n Level: {tutorial._Properties._Level} \n What You'll Learn: {tutorial._Properties._Learn} \n Your Goal: {tutorial._Properties._Learn}";
                panel.Attributes.Pivot = new PointF(200, 200);
                this._TutorialPanelGuid = panel.InstanceGuid;
                panel.Attributes.ExpireLayout();
            }
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


        public TutorialProperties(string Tutorial_Number, string Level, string Learn, string Goal, 
        string Resulting_Geometry, string Solution_Script, string Pseudocode)
        {
            _TutorialNumber = Tutorial_Number;
            _Level = Level;
            _Learn = Learn;
            _Goal = Goal;
            _ResultingGeo = Resulting_Geometry;
            _SolutionScript = Solution_Script;
            _PsuedoCode = Pseudocode;

        }
    }

}


using System.Collections.Generic;
using System.IO;
using Newtonsoft;

namespace gh_quest.CustomClasses
{
    public class LoadTutorial
    {






        

    }

    public class TutorialClass
    {

        string tutorialNumber;
        string level;
        string learn;
        string goal;
        string resultingGeo;
        string psuedoCode;



    
        public static DeconstructTutorialJson(string jsonFilePath, string tutorialName)
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            var jsonParsed = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TutorialClass>(jsonData);

            // Access the "section_tool" part
            if (jsonParsed.TryGetValue(tutorialName, out var tutorialData))
            {

                TutorialClass tutorial =  tutorialData["properties"];

                return defaults;

            }

        }

        
    }

}


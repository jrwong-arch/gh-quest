using Grasshopper;
using Grasshopper.Kernel;
using System.Collections.Generic;
using System.Linq;

namespace gh_quest
{
    public static class ScriptCrawler
    {
        public static List<IGH_Component> GetDocumentComponents(GH_Document document)
        {
            List<IGH_Component> components = new List<IGH_Component>();

            foreach (IGH_DocumentObject obj in document.Objects)
            {
                switch (obj)
                {
                    case IGH_Component component:
                        {
                            components.Add(component);
                            break;
                        }
                }
            }

            return components;
        }
    }
}
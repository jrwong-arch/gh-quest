
using Grasshopper;
using Grasshopper.Kernel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.Design;

namespace gh_quest
{
    public class GraphSchema
    {
        public Dictionary<string, GraphComponent> GraphComponents { get; set; } = new Dictionary<string, GraphComponent>();

        public GraphSchema(GH_Document doc)
        {
            foreach (IGH_Component component in ScriptCrawler.GetDocumentComponents(doc))
            {
                GraphComponents.Add(component.InstanceGuid.ToString(), new GraphComponent(component));
            }
        }
        public static List<IGH_Component> GetDocumentComponents()
        {
            List<IGH_Component> components = new List<IGH_Component>();

            foreach (IGH_DocumentObject obj in Grasshopper.Instances.DocumentServer.First().Objects)
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

    public class GraphComponent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GraphNode> Inputs { get; set; } = new List<GraphNode>();
        public List<GraphNode> Outputs { get; set; } = new List<GraphNode>();

        public GraphComponent(IGH_Component components)
        {

        }
    }

    public class GraphNode
    {
        public string Name { get; set; }
        public string Side { get; set; }
        public List<string> Connections { get; set; } = new List<string>();

        public GraphNode(IGH_Param param)
        {

        }
    }
}
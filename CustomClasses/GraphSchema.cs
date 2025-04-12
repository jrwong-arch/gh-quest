
using Grasshopper.Kernel;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace gh_quest
{
    public class GraphSchema
    {
        public Dictionary<string, GraphComponent> Document { get; set; } = new Dictionary<string, GraphComponent>();

        public GraphSchema(GH_Document doc)
        {
            foreach (IGH_Component component in ScriptCrawler.GetDocumentComponents(doc))
            {
                Document.Add(component.InstanceGuid.ToString(), new GraphComponent(component));
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(Document);
        }
    }

    public class GraphComponent
    {
        public GraphComponentData ComponentData { get; set; }
        public GraphComponentNodeData NodeData { get; set; }

        public GraphComponent(IGH_Component component)
        {
            ComponentData = new GraphComponentData(component);
            NodeData = new GraphComponentNodeData(component);
        }
    }

    public class GraphComponentData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string RunTime { get; set; }

        public GraphComponentData(IGH_Component component)
        {
            Name = component.Name;
            Description = component.Description;
            RunTime = component.ProcessorTime.TotalMilliseconds.ToString();
        }
    }

    public class GraphComponentNodeData
    {
        public List<GraphNode> Inputs { get; set; } = new List<GraphNode>();
        public List<GraphNode> Outputs { get; set; } = new List<GraphNode>();

        public GraphComponentNodeData(IGH_Component component)
        {
            foreach (IGH_Param param in component.Params.Input)
            {
                Inputs.Add(new GraphNode(param));
            }

            foreach (IGH_Param param in component.Params.Output)
            {
                Outputs.Add(new GraphNode(param));
            }
        }
    }

    public class GraphNode
    {
        public string Name { get; set; }
        public string InstanceGuid { get; set; }
        public List<string> Connections { get; set; } = new List<string>();

        public GraphNode(IGH_Param param)
        {
            Name = param.Name;
            InstanceGuid = param.InstanceGuid.ToString();

            foreach (IGH_Param source in param.Sources)
            {
                Connections.Add(source.InstanceGuid.ToString());
            }
        }
    }
}
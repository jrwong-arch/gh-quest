using System;

namespace gh_quest.CustomClasses
{

    public static class ScoreSolution
    {

        public static ScoreSolutionResult ScoreGraph(GraphSchema graph)
        {
            var componentCount = 0;
            double totalComputeTime = 0;

            foreach (var component in graph.Document.Values)
            {
                componentCount = componentCount + 1;
                if (component.ComponentData.RunTime != null)
                {
                    totalComputeTime = totalComputeTime + Convert.ToDouble(component.ComponentData.RunTime);
                }
            }

            return new ScoreSolutionResult(componentCount, totalComputeTime);
        }

    }

    public class ScoreSolutionResult
    {

        public int ComponentCount { get; set; }
        public double TotalComputeTime { get; set; }

        public ScoreSolutionResult(int componentCount, double totalComputeTime)
        {
            ComponentCount = componentCount;
            TotalComputeTime = totalComputeTime;
        }
    }
}


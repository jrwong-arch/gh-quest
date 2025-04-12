using System;

namespace gh_quest.CustomClasses
{

    public static class ScoreSolution
    {

        public static ScoreSolutionResult ScoreGraph(GraphSchema graph)
        {
            var componentCount = 0;
            var totalComputeTime = 0;

            foreach (var component in graph.Document.Values)
            {
                componentCount = componentCount + 1;
                totalComputeTime = totalComputeTime + Convert.ToInt32(component.ComponentData.RunTime);
            }

            return new ScoreSolutionResult(componentCount, totalComputeTime);
        }

    }

    public class ScoreSolutionResult
    {

        public int ComponentCount { get; set; }
        public int TotalComputeTime { get; set; }

        public ScoreSolutionResult(int componentCount, int totalComputeTime)
        {
            ComponentCount = componentCount;
            TotalComputeTime = totalComputeTime;
        }
    }
}


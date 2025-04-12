
using gh_quest.CustomClasses;

namespace gh_quest
{
    public class QuestState
    {
        public GraphSchema UserState { get; set; }
        public GraphSchema GoalState { get; set; }
        public ScoreSolutionResult Score { get; set; }

        public QuestState(GraphSchema userState, GraphSchema goalState, ScoreSolutionResult score)
        {
            UserState = userState;
            GoalState = goalState;
            Score = score;
        }

    }

}
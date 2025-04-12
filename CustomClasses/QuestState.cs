
namespace gh_quest
{
    public class QuestState
    {
        public GraphSchema UserState { get; set; }
        public GraphSchema GoalState { get; set; }

        public QuestState(GraphSchema userState, GraphSchema goalState)
        {
            UserState = userState;
            GoalState = goalState;
        }

    }

}
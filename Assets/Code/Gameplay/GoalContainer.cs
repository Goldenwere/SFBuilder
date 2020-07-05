namespace SFBuilder.Gameplay
{
    /// <summary>
    /// A GoalContainer is a structure that allows one to define goal items in the inspector as well as the required viability for a goal
    /// </summary>
    [System.Serializable]
    public struct GoalContainer
    {
        public GoalItem[] goalExtras;
        public GoalItem[] goalRequirements;
        public int goalViability;
    }
}
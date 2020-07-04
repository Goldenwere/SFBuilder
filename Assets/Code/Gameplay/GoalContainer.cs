namespace SFBuilder.Gameplay
{
    [System.Serializable]
    public struct GoalContainer
    {
        public Goal[] goalExtras;
        public Goal[] goalRequirements;
        public int goalViability;
    }
}
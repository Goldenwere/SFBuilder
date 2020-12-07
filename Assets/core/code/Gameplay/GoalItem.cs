namespace SFBuilder.Gameplay
{
    /// <summary>
    /// A GoalItem is a part of an overall goal with a BuilderObject ID and a count for how many can/need to be placed
    /// </summary>
    [System.Serializable]
    public struct GoalItem
    {
        public int          goalStructureCount;
        public ObjectType   goalStructureID;
    }
}
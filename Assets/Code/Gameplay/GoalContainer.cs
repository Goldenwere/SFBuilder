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

        /// <summary>
        /// Returns a copy of a GoalContainer
        /// </summary>
        /// <param name="other">The container being copied</param>
        /// <returns>The newly copied GoalContainer</returns>
        public static GoalContainer Copy(GoalContainer other)
        {
            GoalContainer copy = new GoalContainer();
            copy.goalExtras = new GoalItem[other.goalExtras.Length];
            copy.goalRequirements = new GoalItem[other.goalRequirements.Length];
            for (int i = 0; i < copy.goalExtras.Length; i++)
                copy.goalExtras[i] = new GoalItem { goalStructureCount = other.goalExtras[i].goalStructureCount, goalStructureID = other.goalExtras[i].goalStructureID };
            for (int i = 0; i < copy.goalRequirements.Length; i++)
                copy.goalRequirements[i] = new GoalItem { goalStructureCount = other.goalRequirements[i].goalStructureCount, goalStructureID = other.goalRequirements[i].goalStructureID };
            copy.goalViability = other.goalViability;
            return copy;
        }

        /// <summary>
        /// Returns a copy of a GoalContainer
        /// </summary>
        /// <param name="other">The container being copied</param>
        /// <param name="viability">Overrides viability for use in infini-play presets</param>
        /// <returns>The newly copied GoalContainer</returns>
        public static GoalContainer Copy(GoalContainer other, int viability)
        {
            GoalContainer copy = new GoalContainer();
            copy.goalExtras = new GoalItem[other.goalExtras.Length];
            copy.goalRequirements = new GoalItem[other.goalRequirements.Length];
            for (int i = 0; i < copy.goalExtras.Length; i++)
                copy.goalExtras[i] = new GoalItem { goalStructureCount = other.goalExtras[i].goalStructureCount, goalStructureID = other.goalExtras[i].goalStructureID };
            for (int i = 0; i < copy.goalRequirements.Length; i++)
                copy.goalRequirements[i] = new GoalItem { goalStructureCount = other.goalRequirements[i].goalStructureCount, goalStructureID = other.goalRequirements[i].goalStructureID };
            copy.goalViability = viability;
            return copy;
        }
    }
}
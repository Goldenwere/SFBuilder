namespace SFBuilder.Obj
{
    /// <summary>
    /// Represents the associated type which describes what a specific BuilderObject is and what kind of values it gives for game scoring
    /// </summary>
    public enum ObjectType
    {
        // Ranges defined/reserved:
        // 0-31: prototyping

        /// <summary>
        /// pA is a power source, giving 3 power; -5 happiness for each pB in range, +3 power for each pA in range
        /// </summary>
        PrototypeA = 0,
        /// <summary>
        /// pB is a residence, taking -5 sustenance and -5 power; no change for buildings in range
        /// </summary>
        PrototypeB = 1,
        /// <summary>
        /// pC is a community garden, giving 5 base sustenance; gives 3 sustenance and 3 happiness for each pB in range
        /// </summary>
        PrototypeC = 2
    }
}

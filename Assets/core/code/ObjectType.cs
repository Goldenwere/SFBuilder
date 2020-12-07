namespace SFBuilder
{
    /// <summary>
    /// Represents the associated type which describes what a specific BuilderObject is and what kind of values it gives for game scoring
    /// </summary>
    public enum ObjectType
    {
        // Ranges defined/reserved:
        // 000-015: prototyping
        // 016-031: natural
        // 032-047: power
        // 048-063: residential
        // 064-095: environment
        // 096-127: commercial/municipal

        /**** Used for prototyping ****/
        /// <summary> Power Source </summary>
        Pro_A = 00,
        /// <summary> Residence </summary>
        Pro_B = 01,
        /// <summary> Community Garden </summary>
        Pro_C = 02,

        /**** Pre-existing, no rules ****/
        /// <summary> Alien Wildlife </summary>
        Nat_A = 16,
        /// <summary> Small Rocks </summary>
        Nat_B = 17,
        /// <summary> Large Rocks </summary>
        Nat_C = 18,
        /// <summary> Lectrinium Vein </summary>
        Nat_D = 19,
        /// <summary> Lectrinium Deposit </summary>
        Nat_E = 20,
        /// <summary> Metals Deposit </summary>
        Nat_F = 21,

        /**** +P, -H ****/
        /// <summary> Solar Panel </summary>
        Pow_A = 32,
        /// <summary> Solar Tower </summary>
        Pow_B = 33,
        /// <summary> Solar Farm </summary>
        Pow_C = 34,
        /// <summary> Small Turbine </summary>
        Pow_D = 35,
        /// <summary> Large Turbine </summary>
        Pow_E = 36,
        /// <summary> Lectrinium Mine </summary>
        Pow_F = 37,

        /**** -S, -P, -H when crowding ****/
        /// <summary> Small Cabin </summary>
        Res_A = 48,
        /// <summary> Large Cabin </summary>
        Res_B = 49,
        /// <summary> Small Apartment </summary>
        Res_C = 50,
        /// <summary> Apartment Tower </summary>
        Res_D = 51, // apartment tower

        /**** +S &/or +H, sometimes -P ****/
        /// <summary> Community Garden </summary>
        Env_A = 64,
        /// <summary> Small Farm </summary>
        Env_B = 65,
        /// <summary> Large Farm </summary>
        Env_C = 66,
        /// <summary> Vertical Farm </summary>
        Env_D = 67

        // +H, -P, sometimes -S
    }

    /*  OBJECT PALETTE (for object material)        */
    /*  METALLIC(neutral)                           */
    /*  METALLIC(slate)                             */
    /*  METALLIC(rustic)                            */
    /*  METALLIC(colorful)                          */
    /*  EMISSION(windows, windows, windows, misc)   */
    /*  EMISSION(other lighting)                    */
    /*  GLASS(neutral x3, colorful)                 */
    /*  NATURAL                                     */
    /*  NATURAL                                     */
    /*  NATURAL                                     */
    /*  SCHEME(di 1)                                */
    /*  SCHEME(di 2)                                */
    /*  SCHEME(di 3)                                */
    /*  SCHEME(di 4)                                */
    /*  SCHEME(tetra 1)                             */
    /*  SCHEME(tetra 2)                             */
    /*  SCHEME(tetra 3)                             */
    /*  SCHEME(tetra 4)                             */
}

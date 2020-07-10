namespace SFBuilder.Obj
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
        // 096-127: commercial

        Pro_A = 00, // power source                 | +00 H +03 P +00 S | -5 H @ residence, +3 P @ power source
        Pro_B = 01, // residence                    | +00 H -05 P -05 S | x
        Pro_C = 02, // community garden             | +00 H +00 P +05 S | +3 S +3 H @ residence

        // Pre-existing, no rules
        Nat_A = 16, // alien wildlife               | +00 H +00 P +00 S | x
        Nat_B = 17, // small rocks                  | +00 H +00 P +00 S | x
        Nat_C = 18, // large rocks                  | +00 H +00 P +00 S | x
        Nat_D = 19, // lectrinium vein              | +00 H +00 P +00 S | x
        Nat_E = 20, // lectrinium deposit           | +00 H +00 P +00 S | x
        Nat_F = 21, // metals deposit               | +00 H +00 P +00 S | x
        
        // +P, -H
        Pow_A = 32, // solar panel                  | +00 H +03 P +00 S | -1 H @ Res
        Pow_B = 33, // solar tower                  | +00 H +05 P +00 S | -1 H @ Res
        Pow_C = 34, // solar farm                   | +00 H +10 P +00 S | -3 H @ Res, +1 P @ s panel/s tower
        Pow_D = 35, // small turbine                | +00 H +06 P +00 S | -3 H @ Res
        Pow_E = 36, // large turbine                | +00 H +12 P +00 S | -5 H @ Res
        Pow_F = 37, // lectrinium mine              | +00 H +00 P +00 S | -8 H @ Res, +3 P @ l vein, +10 P @ l deposit
        
        // -S, -P, -H when crowding
        Res_A = 48, // small cabin                  | +00 H -05 P -06 S | +1 P +1S @ cabins
        Res_B = 49, // large cabin                  | +00 H -06 P -08 S | +1 P +1S @ cabins
        Res_C = 50, // small apartment              | +00 H -10 P -10 S | -5 H @ apartments
        Res_D = 51, // apartment tower              | +00 H -15 P -15 S | -8 H @ apartments
        
        // +S &/or +H, sometimes -P
        Env_A = 64, // community garden             | +00 H +00 P +05 S | +3 H @ cabins, +5 H @ apartments, +2 S @ a wildlife, +1 S @ garden
        Env_B = 65, // small farm                   | +00 H +00 P +08 S | +1 H @ cabins, +3 H @ apartments, +2 S @ farms
        Env_C = 66, // large farm                   | +00 H +00 P +10 S | +1 S @ farms
        Env_D = 67  // vertical farm                | +00 H -03 P +20 S | -1 H @ Res

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

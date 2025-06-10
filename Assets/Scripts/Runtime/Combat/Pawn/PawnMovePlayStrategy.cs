using Runtime.Combat.Tilemap;

namespace Runtime.Combat.Pawn
{
    /// <summary>
    ///     Base class for abilities that can modify a pawn's movement.
    /// </summary>
    public abstract class PawnMovePlayStrategy : PawnPlayStrategy
    {
        /// <summary>
        ///     Allows the strategy to alter the intended next tile during movement.
        /// </summary>
        /// <param name="pawn">The moving pawn.</param>
        /// <param name="nextTile">Reference to the tile the pawn intends to move to. Implementations
        /// may modify this reference to change the final destination.</param>
        public abstract void ModifyMove(PawnController pawn, ref Tile nextTile);
    }
}
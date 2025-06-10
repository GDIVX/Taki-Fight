namespace Runtime.Combat.Pawn
{
    public class PawnInstant
    {
        public PawnInstant(PawnData data)
        {
            Data = data;
        }

        public PawnData Data { get; }

        public int CurrentHealth { get; set; }

        public bool IsFullHealth()
        {
            return CurrentHealth >= Data.Health;
        }
    }
}
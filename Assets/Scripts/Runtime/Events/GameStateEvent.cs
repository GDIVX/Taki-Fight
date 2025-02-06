namespace Runtime.Events
{
    public class GameStateEvent
    {
        public GameState GameState;

        public GameStateEvent(GameState gameState)
        {
            GameState = gameState;
        }
    }

    public enum GameState
    {
        Menu, // Title screen, settings, meta-progression menu
        RunStart, // Initializes a new run (sets up deck, difficulty, etc.)
        Pause, // Freezes gameplay (UI menu, settings, etc.)

        Exploration, // Traversing unknown paths, managing resources
        RoomEntered, // A known room is entered (shop, event, treasure, etc.)
        RoomResolved, // Player has finished interacting with the room

        Combat, // Turn-based battle begins
        CombatEnd, // Combat finishes, applies results (player wins or loses)

        RunEnd, // Player dies, voluntarily exits, or beats the final boss

        Hub // If applicable, a central area for meta-progression
    }
}
public class GameConfig
{
    public readonly short PlayerCount;
    public readonly int MinBet;
        
    public GameConfig(short playerCount, int minBet)
    {
        PlayerCount = playerCount;
        MinBet = minBet;
    }
}

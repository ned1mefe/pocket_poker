public class GameConfig
{
    public readonly short PlayerCount;
    public readonly int BuyIn;
        
    public GameConfig(short playerCount, int buyIn)
    {
        PlayerCount = playerCount;
        BuyIn = buyIn;
    }
}

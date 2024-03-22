public class GameConfig
{
    public readonly short PlayerCount;
    public readonly int BuyIn;
    public readonly int BigBlind;
        
    public GameConfig(short playerCount, int buyIn, int bigBlind)
    {
        PlayerCount = playerCount;
        BuyIn = buyIn;
        BigBlind = bigBlind;
    }
}

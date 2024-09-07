using System.Collections.Generic;

public class Pot
{
    public List<Player> Players { get; }
    public int Money;
    public readonly Player BigBlind;
    public readonly Player SmallBlind;

    public Pot(Player bigBlind, Player smallBlind)
    {
        Players = new List<Player>();
        Money = 0;
        BigBlind = bigBlind;
        SmallBlind = smallBlind;
    }

    public void HandlePlayerBet(Player player, int bet)
    {
        player.Bet(bet);
        Money += bet;
    }
    public void HandlePlayerFold(Player player) => Players.Remove(player);
    public void HandlePlayerJoin(Player player) => Players.Add(player);
    public void WinnedBy(List<Player> winners)
    {
        var winnerCount = winners.Count;
        var remainder = Money % winnerCount;
        var winAmount = Money / winnerCount;

        foreach (var player in winners)
        {
            player.WinPot(winAmount);
        }

        winners[0].WinPot(remainder); // one lucky player gets the remainder
    }

}

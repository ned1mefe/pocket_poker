using System.Collections.Generic;

public class Pot
{
    public List<Player> Players { get; }
    private int _money;
    public readonly Player BigBlind;
    public readonly Player SmallBlind;

    public Pot(Player bigBlind, Player smallBlind)
    {
        Players = new List<Player>();
        _money = 0;
        BigBlind = bigBlind;
        SmallBlind = smallBlind;
    }

    public void HandlePlayerBet(Player player, int bet)
    {
        player.Bet(bet);
        _money += bet;
    }
    public void HandlePlayerFold(Player player) => Players.Remove(player);
    public void HandlePlayerJoin(Player player) => Players.Add(player);
    public void WinnedBy(List<Player> winners)
    {
        var winnerCount = winners.Count;
        var remainder = _money % winnerCount;
        var winAmount = _money / winnerCount;

        foreach (var player in winners)
        {
            player.WinPot(winAmount);
        }

        winners[0].WinPot(remainder); // one lucky player gets the remainder
    }

}

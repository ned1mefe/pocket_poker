using System.Collections.Generic;

public class Pot
{
    public List<Player> Players { get; }
    private int _money;

    Pot()
    {
        Players = new List<Player>();
        _money = 0;
    }

    public void PlayerFolded(Player player) => Players.Remove(player);
    public void PlayerJoinedPot(Player player) => Players.Add(player);

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

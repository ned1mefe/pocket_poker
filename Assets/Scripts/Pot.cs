using System.Collections.Generic;
using System.Linq;

public class Pot
{
    public List<Player> Players { get; }
    public int Money;

    public Pot(List<Player> players)
    {
        Players = new List<Player>(players);
        Money = 0;
    }

    public void HandlePlayerBet(Player player, int bet)
    {
        player.Bet(bet);
        Money += bet;
    }
    public void HandlePlayerFold(Player player) => Players.Remove(player);
    
    public void EndPot()
    {
        if (Players.Count == 1)
        {
            Players[0].WinPot(Money);
            return;
        }
            
        foreach (var player in Players)
        {
            player.Evaluate();
        }

        var bestStatus = Players.Max(p => p.Status);

        var winners = Players.Where(p => p.Status == bestStatus).ToList();

        winners = TieBreak(winners);
        
        RewardWinners(winners);
    }
    
    // needs to be tested
    private List<Player> TieBreak(List<Player> players) 
    {
        var winners = new List<Player>(players);
    
        for (int i = 0; i < 5; i++)
        {
            var highest = winners.Select(player => player.BestHand[i]).OrderByDescending(card => card.Number).FirstOrDefault();
            var newWinners = new List<Player>();
            
            foreach (var player in winners)
            {
                if (player.BestHand[i].Number == highest.Number)
                    newWinners.Add(player);
            }
            winners = newWinners;
            
            if (winners.Count == 1)
                return winners;
        }

        return winners;
    }
    private void RewardWinners(List<Player> winners)
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

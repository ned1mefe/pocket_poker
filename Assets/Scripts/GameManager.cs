using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    private List<Card> _deck;
    private Random _random;
    private List<Player> _players;
    
    private void Awake()
    {
        _deck = new List<Card>();
        _random = new Random();
        _players = new List<Player>();
        
        for (short i = 2; i <= 14; i++)
        {
            _deck.Add(new Card(i,Kind.Club));
            _deck.Add(new Card(i,Kind.Spade));
            _deck.Add(new Card(i,Kind.Diamond));
            _deck.Add(new Card(i,Kind.Heart));
        }

        Shuffle();
        
        foreach (var card in _deck)
        {
            print(card);
        }
    }

    private void Shuffle()
    {
        _deck = _deck.OrderBy(_ => _random.Next()).ToList();
    }
    
    
    
}

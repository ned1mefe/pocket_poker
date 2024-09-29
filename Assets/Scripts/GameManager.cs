using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public enum GamePhase
{
    PreFlop = 1,
    Flop = 2,
    Turn = 3,
    River = 4
}

public class GameManager
{
    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }
    private static GameManager _instance;
    
    public Player ActivePlayer { get; private set; }
    public List<Card> CommunityCards { get; private set; }
    public GameConfig Config { get; private set; }
    public List<Pot> Pots { get; private set; }
    public GamePhase GamePhase;
    
    private readonly Random _random;
    private List<Card> _deck;
    private int _cardIndex;
    private List<Player> _players;
    private Pot _activePot;
    private int _sbIndex;
    private int _queueIndex;

    
    private GameManager()
    {
        _deck = new List<Card>();
        CommunityCards = new List<Card>();
        _random = new Random();
        _players = new List<Player>();
        Pots = new List<Pot>();
        _sbIndex = -1;
        for (short i = 2; i <= 14; i++)
        {
            _deck.Add(new Card(i, Kind.Club));
            _deck.Add(new Card(i, Kind.Spade));
            _deck.Add(new Card(i, Kind.Diamond));
            _deck.Add(new Card(i, Kind.Heart));
        }
    }

    public void InitializeGame(GameConfig config, List<String> nameList)
    {
        Config = config;

        foreach (var name in nameList)
        {
            var pl = new Player(name, config.BuyIn);
            _players.Add(pl);
        }

        //Test();
        StartMainPot();
    }

    private void StartMainPot()
    {
        _sbIndex = (_sbIndex + 1) % _players.Count;
        Shuffle();
        Pots.Clear();
        foreach (var player in _players)
        {
            player.TurnStatus = TurnStatus.Waiting;
        }

        GamePhase = GamePhase.PreFlop;
        
        _activePot = new Pot(_players);
        
        Pots.Add(_activePot);
        ActivePlayer = _players[_sbIndex];
        _queueIndex = _sbIndex;
        
        HandleBlinds();
        
        DealToPlayers();
    }
    

    

    public void HandleCheck()
    {
        ActivePlayer.TurnStatus = TurnStatus.Acted;
        do
        {
            _queueIndex = ++_queueIndex % _players.Count;
            ActivePlayer = _players[_queueIndex];
        } while (ActivePlayer.TurnStatus is TurnStatus.Folded or TurnStatus.AllIn);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            NextPhase();
        }
    }
    public void HandleCall(int bet) //should take the bet from self
    {
        if (bet == ActivePlayer.Stack)
        {
            ActivePlayer.TurnStatus = TurnStatus.AllIn;
            if (_players.All(p => p.TurnStatus is TurnStatus.AllIn or TurnStatus.Folded))
            {
                //finish game right here
            }
        }
        else
        {
            ActivePlayer.TurnStatus = TurnStatus.Acted;
        }
        do
        {
            _queueIndex = ++_queueIndex % _players.Count;
            ActivePlayer = _players[_queueIndex];
        } while (ActivePlayer.TurnStatus is TurnStatus.Folded or TurnStatus.AllIn);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            NextPhase();
        }
    }
    public void HandleRaise(int bet)
    {
        _activePot.HandlePlayerBet(ActivePlayer, bet);
        
        
        foreach (var player in _players.Where(player => player.TurnStatus == TurnStatus.Acted))
        {
            player.TurnStatus = TurnStatus.Waiting;
        }

        if (bet == ActivePlayer.Stack)
        {
            ActivePlayer.TurnStatus = TurnStatus.AllIn;
            if (_players.All(p => p.TurnStatus is TurnStatus.AllIn or TurnStatus.Folded))
            {
                //finish game right here
            }
        }
        else
        {
            ActivePlayer.TurnStatus = TurnStatus.Acted;
        }
        
        do
        {
            _queueIndex = ++_queueIndex % _players.Count;
            ActivePlayer = _players[_queueIndex];
        } while (ActivePlayer.TurnStatus is TurnStatus.Folded or TurnStatus.AllIn);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            NextPhase();
        }
        
    }
    public void HandleFold()
    {
        _activePot.HandlePlayerFold(ActivePlayer);
        
        if (_activePot.Players.Count == 1) // if everyone folds, game ends
        {
            Pots.ForEach(p => p.EndPot());
            StartMainPot();
            return;
        }
        
        ActivePlayer.TurnStatus = TurnStatus.Folded;
        
        do
        {
            _queueIndex = ++_queueIndex % _players.Count;
            ActivePlayer = _players[_queueIndex];
        } while (ActivePlayer.TurnStatus is TurnStatus.Folded or TurnStatus.AllIn);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            NextPhase();
        }
        
    }

    private void HandleBlinds() //check if they are all in
    {
        _activePot.HandlePlayerBet(ActivePlayer, Config.BigBlind/2);
        
        _queueIndex = ++_queueIndex % _players.Count;
        ActivePlayer = _players[_queueIndex];
        
        _activePot.HandlePlayerBet(ActivePlayer, Config.BigBlind);
        
        _queueIndex = ++_queueIndex % _players.Count;
        ActivePlayer = _players[_queueIndex];
    }

    private void Shuffle()
    {
        _cardIndex = 0;
        _deck = _deck.OrderBy(_ => _random.Next()).ToList();
    }

    private void DealToPlayers()
    {
        foreach (var player in _players)
        {
            player.AddHoleCards(_deck[_cardIndex], _deck[_cardIndex+1]);
            _cardIndex += 2;
        }
    }

    private void NextPhase()
    {
        switch (GamePhase)
        {
            case GamePhase.PreFlop:
            {

                foreach (var player in _players)
                {
                    player.AddFlopCards(_deck[_cardIndex],_deck[_cardIndex+1],_deck[_cardIndex+2]);
                }
                CommunityCards.Add(_deck[_cardIndex]);
                CommunityCards.Add(_deck[_cardIndex+1]);
                CommunityCards.Add(_deck[_cardIndex+2]);
                
                _cardIndex += 3;
                
                GamePhase = GamePhase.Flop;
                
                break;
            }
            case GamePhase.Flop:
            {
                foreach (var player in _players)
                {
                    player.AddTurnRiverCard(_deck[_cardIndex]);
                }
                CommunityCards.Add(_deck[_cardIndex]);
                
                _cardIndex ++;
                
                GamePhase = GamePhase.Turn;
                
                break;
            }
            case GamePhase.Turn:
            {
                
                foreach (var player in _players)
                {
                    player.AddTurnRiverCard(_deck[_cardIndex]);
                }
                CommunityCards.Add(_deck[_cardIndex]);
                
                _cardIndex ++;

                GamePhase = GamePhase.River;
                break;
            }
            
            case GamePhase.River: //should check this
            {
                _activePot.EndPot();
                StartMainPot();
                break;
            }
        }
        foreach (var player in _players.Where(player => player.TurnStatus == TurnStatus.Acted))
        {
            player.TurnStatus = TurnStatus.Waiting;
        }
    }

    //This is the old test method
    public void Test()
    {
        Debug.LogWarning("Hand:");
        Shuffle();

        var efe = new Player("efe", 250);
        
        foreach (var card in _deck.GetRange(0,7))
        {
            Debug.Log(card);
            efe.AddCard(card);
        }
        
        efe.Evaluate();

        Debug.LogError(efe.Status);
        foreach (var card in efe.BestHand)
        {
            Debug.Log(card);
        }
        
    }
}

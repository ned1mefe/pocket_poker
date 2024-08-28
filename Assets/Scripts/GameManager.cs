using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

enum GamePhase
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
    public Player ActivePlayer { get; private set; }
    public List<Card> CommunityCards { get; private set; }

    private GamePhase _gamePhase;
    private List<Card> _deck;
    private int _cardIndex;
    
    private readonly Random _random;
    private List<Player> _players;
    private GameConfig _config;
    private static GameManager _instance;
    private Pot _activePot;
    private List<Pot> _pots;
    
    private int _sbIndex;
    private int _queueIndex;

    
    private GameManager()
    {
        _deck = new List<Card>();
        CommunityCards = new List<Card>();
        _random = new Random();
        _players = new List<Player>();
        _pots = new List<Pot>();
        _sbIndex = 0;
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
        _config = config;

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
        Shuffle();

        _gamePhase = GamePhase.PreFlop;
        
        _activePot = new Pot(
            _players[_sbIndex+1 % _players.Count], 
            _players[_sbIndex]
            );
        
        _pots.Add(_activePot);
        ActivePlayer = _players[_sbIndex];
        _queueIndex = _sbIndex;
        
        HandleBlinds();
        
        DealToPlayers();
        
    }
    

    public void Test()
    {
        Debug.LogWarning("Hand:");
        Shuffle();

        var efe = new Player("efe", 250);
        
        foreach (var card in _deck.GetRange(0,7))
        {
            Debug.Log(card);
            //efe.AddCard(card);
        }
        
        efe.Evaluate();

        Debug.LogError(efe.Status);
        foreach (var card in efe.BestHand)
        {
            Debug.Log(card);
        }
        
    }
    

    private void HandleCheck()
    {
        ActivePlayer.TurnStatus = TurnStatus.Acted;
        do
        {
            _queueIndex = ++_queueIndex % _players.Count;
            ActivePlayer = _players[_queueIndex];
        } while (ActivePlayer.TurnStatus == TurnStatus.Folded);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            NextPhase();
        }
    }
    private void HandleRaise(int bet)
    {
        _activePot.HandlePlayerBet(ActivePlayer, bet);
        
        foreach (var player in _players.Where(player => player.TurnStatus == TurnStatus.Acted))
        {
            player.TurnStatus = TurnStatus.Waiting;
        }
        
        ActivePlayer.TurnStatus = TurnStatus.Acted;
        
        do
        {
            _queueIndex = ++_queueIndex % _players.Count;
            ActivePlayer = _players[_queueIndex];
        } while (ActivePlayer.TurnStatus == TurnStatus.Folded);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            NextPhase();
        }
        
    }
    private void HandleFold()
    {
        _activePot.HandlePlayerFold(ActivePlayer);
        
        ActivePlayer.TurnStatus = TurnStatus.Folded;
        
        do
        {
            _queueIndex = ++_queueIndex % _players.Count;
            ActivePlayer = _players[_queueIndex];
        } while (ActivePlayer.TurnStatus == TurnStatus.Folded);

        if (ActivePlayer.TurnStatus == TurnStatus.Acted)
        {
            NextPhase();
        }
        
    }

    private void HandleBlinds()
    {
        _activePot.HandlePlayerBet(ActivePlayer, _config.BigBlind/2);
        
        _queueIndex = ++_queueIndex % _players.Count;
        ActivePlayer = _players[_queueIndex];
        
        _activePot.HandlePlayerBet(ActivePlayer, _config.BigBlind);
        
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
        switch (_gamePhase)
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
                
                _gamePhase = GamePhase.Flop;
                
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
                
                _gamePhase = GamePhase.Turn;
                
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

                _gamePhase = GamePhase.River;
                break;
            }
            
            case GamePhase.River:
            {
                
                //Should choose the winner and start a new main Pot
                break;
            }
        }
        
        foreach (var player in _players.Where(player => player.TurnStatus == TurnStatus.Acted))
        {
            player.TurnStatus = TurnStatus.Waiting;
        }
        
        
    }
    
}

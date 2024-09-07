using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject _mainMenuScreen;
    private GameObject _configScreen;
    private GameObject _passScreen;
    private GameObject _playScreen;
    
    private TMP_InputField _buyInInputField;
    private TMP_InputField _nameInputField;
    private TMP_InputField _bBlindInputField;
    private List<String> _nameList;
    private TextMeshProUGUI _names;
    
    private TextMeshProUGUI _passToPlayerText;

    private RawImage _hCard1;
    private RawImage _hCard2;
    private RawImage _cCard1;
    private RawImage _cCard2;
    private RawImage _cCard3;
    private RawImage _cCard4;
    private RawImage _cCard5;
    private TextMeshProUGUI _turnText;
    private TextMeshProUGUI _betInfoText;
    private TextMeshProUGUI _stackText;
    private TextMeshProUGUI _potText;
    
    private void Awake()
    {
        _mainMenuScreen = GameObject.Find("Canvas").transform.Find("MainMenuScreen").gameObject;
        _configScreen = GameObject.Find("Canvas").transform.Find("ConfigScreen").gameObject;
        _passScreen = GameObject.Find("Canvas").transform.Find("PassScreen").gameObject;
        _playScreen = GameObject.Find("Canvas").transform.Find("PlayScreen").gameObject;
        
        _buyInInputField = _configScreen.transform.Find("BuyIn").GetComponentInChildren<TMP_InputField>();
        _nameInputField = _configScreen.transform.Find("AddPlayer").GetComponentInChildren<TMP_InputField>();
        _bBlindInputField = _configScreen.transform.Find("BigBlind").GetComponentInChildren<TMP_InputField>();
        
        _nameList = new List<string>();
        _names = _configScreen.transform.Find("Players").GetChild(1).GetComponent<TextMeshProUGUI>();
        _passToPlayerText = _passScreen.transform.Find("PassText").GetComponent<TextMeshProUGUI>();

        _hCard1 = _playScreen.transform.Find("HoleCards").transform.Find("HCard1").GetComponent<RawImage>();
        _hCard2 = _playScreen.transform.Find("HoleCards").transform.Find("HCard2").GetComponent<RawImage>();
        _cCard1 = _playScreen.transform.Find("CommunityCards").transform.Find("CCard1").GetComponent<RawImage>();
        _cCard2 = _playScreen.transform.Find("CommunityCards").transform.Find("CCard2").GetComponent<RawImage>();
        _cCard3 = _playScreen.transform.Find("CommunityCards").transform.Find("CCard3").GetComponent<RawImage>();
        _cCard4 = _playScreen.transform.Find("CommunityCards").transform.Find("CCard4").GetComponent<RawImage>();
        _cCard5 = _playScreen.transform.Find("CommunityCards").transform.Find("CCard5").GetComponent<RawImage>();

        _turnText = _playScreen.transform.Find("Infos").transform.Find("TurnText").GetComponent<TextMeshProUGUI>();
        _betInfoText = _playScreen.transform.Find("Infos").transform.Find("BetInfoText").GetComponent<TextMeshProUGUI>();
        _stackText = _playScreen.transform.Find("Infos").transform.Find("StackText").GetComponent<TextMeshProUGUI>();
        _potText = _playScreen.transform.Find("Infos").transform.Find("PotText").GetComponent<TextMeshProUGUI>();
        
    }

    public void PlayButtonPressed()
    {
        _mainMenuScreen.gameObject.SetActive(false);
        _configScreen.gameObject.SetActive(true);
        _nameList = new List<string>();
        UpdateNames();
    }

    public void HtpButtonPressed()
    {
        // should open a window to explain
    }

    public void BackButtonPressed()
    {
        _mainMenuScreen.gameObject.SetActive(true);
        _configScreen.gameObject.SetActive(false);
    }

    public void AddPlayerButtonPressed()
    {
        if (_nameInputField.text is null or "") return;
        if (_nameList.Count >= 8) return;
        
        _nameList.Add(_nameInputField.text);
        UpdateNames();
        _nameInputField.text = "";
    }

    private void UpdateNames()
    {
        if (_nameList.Count == 0)
        {
            _names.text = "No players yet.";
            return;
        }

        StringBuilder builder = new StringBuilder();

        foreach (var person in _nameList)
        {
            builder.Append(person);
            builder.Append(", ");
        }

        builder.Remove(builder.Length-2, 2); // to remove last comma and space
        _names.text = builder.ToString();
    }
    public void StartButtonPressed()
    {
        if (_buyInInputField.text is null or "") return;
        if (_bBlindInputField.text is null) return;
        if (_buyInInputField.text is null) return;
        
        var buyIn = int.Parse(_buyInInputField.text);
        var playerCount = _nameList.Count;
        var bigBlind = int.Parse(_bBlindInputField.text);
        
        if (buyIn <= 0 || buyIn < bigBlind) return;
        if (playerCount is 0 or 1) return;
        
        GameConfig config = new GameConfig((short)playerCount, buyIn, bigBlind);
        GameManager.Instance.InitializeGame(config,_nameList);

        OpenPassScreen();
    }

    public void OpenPassScreen()
    {
        _passToPlayerText.text = "Please pass the device to " + GameManager.Instance.ActivePlayer.Name + ".";
        _configScreen.SetActive(false);
        _playScreen.SetActive(false);
        _passScreen.SetActive(true);
    }

    public void OpenPlayScreen()
    {
        var player = GameManager.Instance.ActivePlayer;

        if (player is null)
        {
            Debug.LogError("player is null");
            return;
        }
        
        _turnText.text = player.Name + "'s Turn";
        _stackText.text = player.Stack.ToString();
        _potText.text = GameManager.Instance.Pots.Sum(x => x.Money).ToString();

        #region cardRendering

        _hCard1.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/" + NameOf(player.Hand[0]));
        _hCard2.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/" + NameOf(player.Hand[1]));

        if (GameManager.Instance.GamePhase == GamePhase.PreFlop)
        {
            _cCard1.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/BackColor_Black");
            _cCard2.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/BackColor_Black");
            _cCard3.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/BackColor_Black");
            _cCard4.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/BackColor_Black");
            _cCard5.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/BackColor_Black");
        }

        if (GameManager.Instance.GamePhase >= GamePhase.Flop)
        {
            _cCard1.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/" + NameOf(GameManager.Instance.CommunityCards[0]));
            _cCard2.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/" + NameOf(GameManager.Instance.CommunityCards[1]));
            _cCard3.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/" + NameOf(GameManager.Instance.CommunityCards[2]));
        }

        if (GameManager.Instance.GamePhase >= GamePhase.Turn)
        {
            _cCard4.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/" + NameOf(GameManager.Instance.CommunityCards[3]));
        }

        if (GameManager.Instance.GamePhase == GamePhase.River)
        {
            _cCard5.texture = Resources.Load<Texture>("Playing Cards/Image/PlayingCards/" + NameOf(GameManager.Instance.CommunityCards[4]));
        }

        #endregion
        
        _passScreen.SetActive(false);
        _playScreen.SetActive(true);
    }

    public void HandleCall()
    {
        GameManager.Instance.HandleCall();
        OpenPassScreen();
    }
    
    public void HandleCheck()
    {
        GameManager.Instance.HandleCheck();
        OpenPassScreen();
    }
    
    public void HandleFold()
    {
        GameManager.Instance.HandleFold();
        OpenPassScreen();
    }
    
    public void HandleRaise(int bet)
    {
        GameManager.Instance.HandleRaise(bet);
        OpenPassScreen();
    }
    

    private string NameOf(Card card)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(card.Kind);
        if (card.Number == 14)
        {
            sb.Append("01");
            return sb.ToString();
        }

        if (card.Number < 10)
        {
            sb.Append("0" + card.Number);
            return sb.ToString();
        }

        sb.Append(card.Number.ToString());
        return sb.ToString();
    }
}

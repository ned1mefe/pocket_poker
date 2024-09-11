using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
    public class PlayScreen : MonoBehaviour, IScreen
    {
        
        private RawImage _hCard1;
        private RawImage _hCard2;
        private RawImage _cCard1;
        private RawImage _cCard2;
        private RawImage _cCard3;
        private RawImage _cCard4;
        private RawImage _cCard5;
        private TextMeshProUGUI _turnText;
        private TextMeshProUGUI _infoText;
        private TextMeshProUGUI _stackText;
        private TextMeshProUGUI _potText;
        private RectTransform _betPanel;
        private int _selectedBet;
        private Slider _slider;
        
        private void Initialize()
        {
            _hCard1 = transform.Find("HoleCards").transform.Find("HCard1").GetComponent<RawImage>();
            _hCard2 = transform.Find("HoleCards").transform.Find("HCard2").GetComponent<RawImage>();
            _cCard1 = transform.Find("CommunityCards").transform.Find("CCard1").GetComponent<RawImage>();
            _cCard2 = transform.Find("CommunityCards").transform.Find("CCard2").GetComponent<RawImage>();
            _cCard3 = transform.Find("CommunityCards").transform.Find("CCard3").GetComponent<RawImage>();
            _cCard4 = transform.Find("CommunityCards").transform.Find("CCard4").GetComponent<RawImage>();
            _cCard5 = transform.Find("CommunityCards").transform.Find("CCard5").GetComponent<RawImage>();

            _turnText = transform.Find("Infos").transform.Find("TurnText").GetComponent<TextMeshProUGUI>();
            _infoText = transform.Find("Infos").transform.Find("BetInfoText").GetComponent<TextMeshProUGUI>();
            _stackText = transform.Find("Infos").transform.Find("StackText").GetComponent<TextMeshProUGUI>();
            _potText = transform.Find("Infos").transform.Find("PotText").GetComponent<TextMeshProUGUI>();
            _betPanel = transform.Find("BetPanel").GetComponent<RectTransform>();
            _selectedBet = Math.Min(GameManager.Instance.Config.BigBlind * 2, GameManager.Instance.ActivePlayer.Stack);
            _slider = _betPanel.transform.Find("Slider").GetComponent<Slider>();

            _slider.minValue = Math.Min(GameManager.Instance.Config.BigBlind * 2, GameManager.Instance.ActivePlayer.Stack);
            _slider.maxValue = Math.Max(GameManager.Instance.Config.BigBlind * 2, GameManager.Instance.ActivePlayer.Stack);
            _slider.onValueChanged.AddListener(delegate { SliderValueChange(); });
        }

        public void HandleShow()
        {
            var player = GameManager.Instance.ActivePlayer;
            
            if (_turnText is null)
                Initialize();

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
        
            gameObject.SetActive(true);
        }

        public void HandleClose()
        {
            gameObject.SetActive(false);
        }

        public void OpenBetPanel()
        {
            _betPanel.gameObject.SetActive(true);
        }

        public void CloseBetPanel()
        {
            _betPanel.gameObject.SetActive(false);
        }

        public void HandleCall()
        {
            GameManager.Instance.HandleCall(50);
            UIManager.Instance.ShowScreenByType<PassScreen>();
        }
    
        public void HandleCheck()
        {
            GameManager.Instance.HandleCheck();
            UIManager.Instance.ShowScreenByType<PassScreen>();
        }
    
        public void HandleFold()
        {
            GameManager.Instance.HandleFold();
            UIManager.Instance.ShowScreenByType<PassScreen>();
        }
    
        public void HandleRaise()
        {
            GameManager.Instance.HandleRaise(_selectedBet);
            CloseBetPanel();
            UIManager.Instance.ShowScreenByType<PassScreen>();
        }

        public void HalfPot()
        {
            _selectedBet = GameManager.Instance.Pots.Sum(x => x.Money) / 2;
        }
        public void FullPot()
        {
            _selectedBet = GameManager.Instance.Pots.Sum(x => x.Money);
        }

        public void AllIn()
        {
            _selectedBet = GameManager.Instance.ActivePlayer.Stack;
        }

        private void SliderValueChange() => _selectedBet = (int)_slider.value;
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
}
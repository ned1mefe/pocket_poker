using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Screens
{
    public class ConfigScreen : MonoBehaviour, IScreen
    {
        private TMP_InputField _buyInInputField;
        private TMP_InputField _nameInputField;
        private TMP_InputField _bBlindInputField;
        private List<String> _nameList;
        private TextMeshProUGUI _names;

        private void Awake()
        {
            _buyInInputField = transform.Find("BuyIn").GetComponentInChildren<TMP_InputField>();
            _nameInputField = transform.Find("AddPlayer").GetComponentInChildren<TMP_InputField>();
            _bBlindInputField = transform.Find("BigBlind").GetComponentInChildren<TMP_InputField>();
        
            _nameList = new List<string>();
            _names = transform.Find("Players").GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        public void HandleShow()
        {
            _nameList = new List<string>();
            gameObject.SetActive(true);            
        }

        public void HandleClose()
        {
            gameObject.SetActive(false);            
        }
        
        public void AddPlayerButtonPressed()
        {
            if (_nameInputField.text is null or "") return;
            if (_nameList.Count >= 8) return;
        
            _nameList.Add(_nameInputField.text);
            UpdateNames();
            _nameInputField.text = "";
        }

        public void BackButtonPressed()
        {
            UIManager.Instance.ShowScreenByType<MainMenuScreen>();
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

            UIManager.Instance.ShowScreenByType<PassScreen>();
        }
        
    }
}

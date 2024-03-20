using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameObject _mainMenuScreen;
    private GameObject _configScreen;
    private TMP_InputField _buyInInputField;
    private TMP_InputField _nameInputField;
    private List<String> _nameList;
    private TextMeshProUGUI _names;
    private void Awake()
    {
        _mainMenuScreen = GameObject.Find("Canvas").transform.Find("MainMenuScreen").gameObject;
        _configScreen = GameObject.Find("Canvas").transform.Find("ConfigScreen").gameObject;
        _buyInInputField = _configScreen.transform.Find("BuyIn").GetComponentInChildren<TMP_InputField>();
        _nameInputField = _configScreen.transform.Find("AddPlayer").GetComponentInChildren<TMP_InputField>();
        _nameList = new List<string>();
        _names = _configScreen.transform.Find("Players").GetChild(1).GetComponent<TextMeshProUGUI>();

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

        builder.Remove(builder.Length-2, 2);
        _names.text = builder.ToString();
    }
    public void StartButtonPressed()
    {
        if (_buyInInputField.text is null or "") return;
        var buyIn = int.Parse(_buyInInputField.text);
        if (buyIn <= 0) return;
        
        GameConfig config = new GameConfig((short)_nameList.Count, buyIn); // FİX
        GameManager.Instance.InitializeGame(config,_nameList);
        // should change the window
    }
}

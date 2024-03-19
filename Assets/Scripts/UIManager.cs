using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class UIManager : MonoBehaviour
{
    private GameObject _mainMenuScreen;
    private GameObject _configScreen;
    private TextMeshProUGUI _playerCountText;
    private Slider _playerCountSlider;

    private void Awake()
    {
        _mainMenuScreen = GameObject.Find("Canvas").transform.Find("MainMenuScreen").gameObject;
        _configScreen = GameObject.Find("Canvas").transform.Find("ConfigScreen").gameObject;
        _playerCountText   = _configScreen.transform.Find("PlayerCount").GetComponentInChildren<TextMeshProUGUI>();
        _playerCountSlider = _configScreen.transform.Find("PlayerCount").GetComponentInChildren<Slider>();
        _playerCountSlider.onValueChanged.AddListener(PlayerCountUpdate);
    }

    public void PlayButtonPressed()
    {
        _mainMenuScreen.gameObject.SetActive(false);
        _configScreen.gameObject.SetActive(true);
    }

    private void PlayerCountUpdate(float value)
    {
        _playerCountText.text = "Player Count: " + value;
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
    public void StartButtonPressed()
    {
        GameConfig config = new GameConfig((short)_playerCountSlider.value, 0); //TODO: fix minBet
        GameManager.Instance.SetConfig(config);
        GameManager.Instance.Test();
        // should change the window
    }
}

using System;
using TMPro;
using UnityEngine;

namespace Screens
{
    public class PassScreen : MonoBehaviour, IScreen
    {
        
        private TextMeshProUGUI _passToPlayerText;

        private void Awake()
        {
            _passToPlayerText = transform.Find("PassText").GetComponent<TextMeshProUGUI>();
        }

        public void HandleDoneButton()
        {
            UIManager.Instance.ShowScreenByType<PlayScreen>();
        }
        public void HandleShow()
        {
            gameObject.SetActive(true);
            _passToPlayerText.text = "Please pass the device to " + GameManager.Instance.ActivePlayer.Name + ".";
        }

        public void HandleClose()
        {
            gameObject.SetActive(false);
        }
    }
}
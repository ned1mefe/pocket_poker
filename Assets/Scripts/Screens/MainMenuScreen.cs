using System;
using UnityEngine;

namespace Screens
{
    public class MainMenuScreen : MonoBehaviour, IScreen
    {
        public void PlayButtonClick()
        {
            UIManager.Instance.ShowScreenByType<ConfigScreen>();
        }

        public void HandleShow()
        {
            gameObject.SetActive(true);
        }

        public void HandleClose()
        {
            gameObject.SetActive(false);
        }
    }
}
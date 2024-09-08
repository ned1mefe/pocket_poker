using System.Collections.Generic;
using Screens;
using UnityEngine;

public class UIManager
{
    private readonly List<IScreen> _screens;
    
    private IScreen _activeScreen;
    
    public static UIManager Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }
    private static UIManager _instance;

    
    private UIManager()
    {
        var canvas = GameObject.Find("Canvas");
        _screens = new List<IScreen>(canvas.GetComponentsInChildren<IScreen>(true));
        ShowScreenByType<MainMenuScreen>();
    }
    
    public void ShowScreenByType<T>() where T : IScreen
    {
        foreach (var screen in _screens)
        {
            if (screen is T)
            {
                ShowScreen(screen);
                break;
            }
        }
    }
    
    
    private void ShowScreen(IScreen newScreen)
    {
        _activeScreen?.HandleClose();

        newScreen.HandleShow();
        _activeScreen = newScreen;
    }
    
}

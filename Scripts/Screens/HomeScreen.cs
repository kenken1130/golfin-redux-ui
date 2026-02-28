using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Home screen — main hub after Splash. 
/// Contains navigation bar and settings button.
/// </summary>
public class HomeScreen : ScreenBase
{
    [Header("Navigation Buttons")]
    [SerializeField] private PressableButton navHomeButton;
    [SerializeField] private PressableButton navShopButton;
    [SerializeField] private PressableButton navPlayButton;
    [SerializeField] private PressableButton navProfileButton;

    [Header("Settings")]
    [SerializeField] private PressableButton settingsButton;
    
    [Header("Screen References")]
    [SerializeField] private ScreenBase settingsScreen;

    [Header("Events")]
    public UnityEvent OnSettingsPressed;
    public UnityEvent OnShopPressed;
    public UnityEvent OnPlayPressed;
    public UnityEvent OnProfilePressed;

    public override void OnScreenEnter()
    {
        if (settingsButton != null)
            settingsButton.onClick.AddListener(HandleSettings);
        if (navShopButton != null)
            navShopButton.onClick.AddListener(HandleShop);
        if (navPlayButton != null)
            navPlayButton.onClick.AddListener(HandlePlay);
        if (navProfileButton != null)
            navProfileButton.onClick.AddListener(HandleProfile);
    }

    public override void OnScreenExit()
    {
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(HandleSettings);
        if (navShopButton != null)
            navShopButton.onClick.RemoveListener(HandleShop);
        if (navPlayButton != null)
            navPlayButton.onClick.RemoveListener(HandlePlay);
        if (navProfileButton != null)
            navProfileButton.onClick.RemoveListener(HandleProfile);
    }

    private void HandleSettings()
    {
        Debug.Log("[HomeScreen] Settings pressed");
        if (settingsScreen != null)
            ScreenManager.Instance.TransitionTo(settingsScreen);
        OnSettingsPressed?.Invoke();
    }

    private void HandleShop()
    {
        Debug.Log("[HomeScreen] Shop pressed");
        OnShopPressed?.Invoke();
    }

    private void HandlePlay()
    {
        Debug.Log("[HomeScreen] Play pressed");
        OnPlayPressed?.Invoke();
    }

    private void HandleProfile()
    {
        Debug.Log("[HomeScreen] Profile pressed");
        OnProfilePressed?.Invoke();
    }
}

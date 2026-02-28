using UnityEngine;
using TMPro;

/// <summary>
/// Settings screen with back navigation to Home.
/// Toggle buttons for sound, music, notifications, and language.
/// </summary>
public class SettingsScreen : ScreenBase
{
    [Header("Navigation")]
    [SerializeField] private PressableButton backButton;
    
    [Header("Screen References")]
    [SerializeField] private ScreenBase homeScreen;

    [Header("Settings Controls")]
    [SerializeField] private PressableButton soundToggle;
    [SerializeField] private PressableButton musicToggle;
    [SerializeField] private PressableButton notificationsToggle;
    [SerializeField] private PressableButton languageButton;

    public override void OnScreenEnter()
    {
        if (backButton != null)
            backButton.onClick.AddListener(HandleBack);
    }

    public override void OnScreenExit()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(HandleBack);
    }

    private void HandleBack()
    {
        Debug.Log("[SettingsScreen] Back pressed -> returning to Home");
        if (homeScreen != null)
            ScreenManager.Instance.TransitionTo(homeScreen);
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Entry point. Manages the startup flow: Logo → Loading → Splash → Home.
/// Attach to a root GameObject in the scene.
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [Header("Screen References")]
    [SerializeField] private LogoScreen logoScreen;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private SplashScreen splashScreen;
    [SerializeField] private HomeScreen homeScreen;
    
    [Header("Timing")]
    [SerializeField] private float logoDisplayTime = 3f;
    
    private void Start()
    {
        // Hide all screens
        logoScreen.Hide();
        loadingScreen.Hide();
        splashScreen.Hide();
        if (homeScreen != null) homeScreen.Hide();
        
        // Wire Splash START button to navigate to Home
        if (splashScreen != null && homeScreen != null)
        {
            splashScreen.OnStartPressed.AddListener(() =>
            {
                ScreenManager.Instance.TransitionTo(homeScreen);
            });
        }
        
        StartCoroutine(StartupSequence());
    }
    
    private IEnumerator StartupSequence()
    {
        // Phase 1: Logo
        ScreenManager.Instance.ShowImmediate(logoScreen);
        yield return new WaitForSeconds(logoDisplayTime);
        
        // Phase 2: Loading with tips
        ScreenManager.Instance.TransitionTo(loadingScreen);
        
        // Wait for loading to complete
        yield return new WaitUntil(() => loadingScreen.IsLoadingComplete);
        
        // Phase 3: Splash with Start button
        ScreenManager.Instance.TransitionTo(splashScreen);
        
        // Phase 4: START button press → HomeScreen (wired in Start())
    }
}

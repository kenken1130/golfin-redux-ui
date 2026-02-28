using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

/// <summary>
/// Editor tool that creates the full GOLFIN UI scene hierarchy
/// AND auto-wires all component references. No manual Inspector linking needed.
/// Usage: Unity menu -> Tools -> Create GOLFIN UI Scene
/// </summary>
public class CreateUIScreen
{
    [MenuItem("Tools/GOLFIN/Create Full UI Scene")]
    public static void CreateUI()
    {
        // --- Scene Root ---
        GameObject root = new GameObject("Scene Root");

        // --- Managers ---
        GameObject managers = new GameObject("Managers");
        managers.transform.SetParent(root.transform, false);
        var locManager = managers.AddComponent<LocalizationManager>();
        var screenManager = managers.AddComponent<ScreenManager>();
        var bootstrap = managers.AddComponent<GameBootstrap>();

        // --- Canvas ---
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(root.transform, false);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1170, 2532);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- Create All Screens ---
        var logoScreen = CreateLogoScreen(canvasGO.transform);
        var loadingScreen = CreateLoadingScreen(canvasGO.transform);
        var splashScreen = CreateSplashScreen(canvasGO.transform);
        var homeScreen = CreateHomeScreen(canvasGO.transform);
        var settingsScreen = CreateSettingsScreen(canvasGO.transform);

        // --- Wire GameBootstrap ---
        SetPrivateField(bootstrap, "logoScreen", logoScreen);
        SetPrivateField(bootstrap, "loadingScreen", loadingScreen);
        SetPrivateField(bootstrap, "splashScreen", splashScreen);
        SetPrivateField(bootstrap, "homeScreen", homeScreen);

        // --- Wire cross-screen references ---
        SetPrivateField(homeScreen, "settingsScreen", (ScreenBase)settingsScreen);
        SetPrivateField(settingsScreen, "homeScreen", (ScreenBase)homeScreen);

        // --- EventSystem ---
        if (!Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>())
        {
            GameObject es = new GameObject("EventSystem");
            es.transform.SetParent(root.transform, false);
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Selection.activeGameObject = root;
        Debug.Log("[GOLFIN] Full UI Scene created with HomeScreen + SettingsScreen! ✅");
    }

    [MenuItem("Tools/GOLFIN/Debug HomeScreen Fields")]
    public static void DebugHomeScreenFields()
    {
        var home = Object.FindAnyObjectByType<HomeScreen>();
        if (home == null) { Debug.LogError("[GOLFIN Debug] HomeScreen not found in scene!"); return; }

        var fields = typeof(HomeScreen).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        Debug.Log($"[GOLFIN Debug] HomeScreen has {fields.Length} fields:");
        foreach (var f in fields)
        {
            var val = f.GetValue(home);
            string status = val == null ? "NULL ⚠️" : val.ToString();
            Debug.Log($"  {f.Name} ({f.FieldType.Name}) = {status}");
        }
    }

    [MenuItem("Tools/GOLFIN/Debug SettingsScreen Fields")]
    public static void DebugSettingsScreenFields()
    {
        var settings = Object.FindAnyObjectByType<SettingsScreen>();
        if (settings == null) { Debug.LogError("[GOLFIN Debug] SettingsScreen not found in scene!"); return; }

        var fields = typeof(SettingsScreen).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        Debug.Log($"[GOLFIN Debug] SettingsScreen has {fields.Length} fields:");
        foreach (var f in fields)
        {
            var val = f.GetValue(settings);
            string status = val == null ? "NULL ⚠️" : val.ToString();
            Debug.Log($"  {f.Name} ({f.FieldType.Name}) = {status}");
        }
    }

    [MenuItem("Tools/GOLFIN/Verify All Screen Wiring")]
    public static void VerifyWiring()
    {
        Debug.Log("[GOLFIN] === Wiring Verification ===");
        
        var sm = Object.FindAnyObjectByType<ScreenManager>();
        Debug.Log(sm != null ? "✅ ScreenManager found" : "❌ ScreenManager MISSING");
        
        var bootstrap = Object.FindAnyObjectByType<GameBootstrap>();
        Debug.Log(bootstrap != null ? "✅ GameBootstrap found" : "❌ GameBootstrap MISSING");
        
        var screens = Object.FindObjectsByType<ScreenBase>(FindObjectsSortMode.None);
        Debug.Log($"📺 Found {screens.Length} screens:");
        foreach (var s in screens)
            Debug.Log($"  - {s.gameObject.name} ({s.GetType().Name})");
        
        var buttons = Object.FindObjectsByType<PressableButton>(FindObjectsSortMode.None);
        Debug.Log($"🔘 Found {buttons.Length} PressableButtons:");
        foreach (var b in buttons)
            Debug.Log($"  - {b.gameObject.name}");
        
        // Check EventSystem
        var es = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
        Debug.Log(es != null ? "✅ EventSystem found" : "❌ EventSystem MISSING - buttons won't work!");
        
        Debug.Log("[GOLFIN] === Verification Complete ===");
    }

    // --- Logo Screen ---
    static LogoScreen CreateLogoScreen(Transform parent)
    {
        GameObject screen = CreateScreenPanel("LogoScreen", parent);
        var component = screen.AddComponent<LogoScreen>();
        CreateImage("Background", screen.transform, Color.black, stretch: true);
        CreateImage("Logo", screen.transform, Color.white, stretch: false);
        return component;
    }

    // --- Loading Screen ---
    static LoadingScreen CreateLoadingScreen(Transform parent)
    {
        GameObject screen = CreateScreenPanel("LoadingScreen", parent);
        var component = screen.AddComponent<LoadingScreen>();
        CreateImage("Background", screen.transform, Color.gray, stretch: true);

        GameObject tipCardGO = CreateContainer("ProTipCard", screen.transform);
        var tipCard = tipCardGO.AddComponent<ProTipCard>();
        var header = CreateTMP("Header", tipCardGO.transform, "PRO TIP");
        AddLocalizedText(header, "tip_header");
        CreateImage("Divider", tipCardGO.transform, new Color(0.85f, 0.65f, 0.13f), stretch: false);
        var tipText = CreateTMP("TipText", tipCardGO.transform, "Tip goes here...");
        GameObject tipImageGO = CreateImage("TipImage", tipCardGO.transform, Color.white, stretch: false);
        var tapNext = CreateTMP("TapNextText", tipCardGO.transform, "TAP FOR NEXT TIP");
        AddLocalizedText(tapNext, "tip_next");

        SetPrivateField(tipCard, "headerText", header.GetComponent<TextMeshProUGUI>());
        SetPrivateField(tipCard, "tipText", tipText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(tipCard, "tapNextText", tapNext.GetComponent<TextMeshProUGUI>());
        SetPrivateField(tipCard, "tipImages", new Image[] { tipImageGO.GetComponent<Image>() });

        var nowLoading = CreateTMP("NowLoadingText", screen.transform, "Loading...");
        AddLocalizedText(nowLoading, "loading_text");

        GameObject barBG = CreateImage("LoadingBarBG", screen.transform, new Color(0.1f, 0.16f, 0.29f), stretch: false);
        var loadingBar = barBG.AddComponent<LoadingBar>();
        GameObject barFill = CreateImage("LoadingBarFill", barBG.transform, new Color(0.2f, 0.4f, 0.8f), stretch: true);
        barFill.GetComponent<Image>().type = Image.Type.Filled;
        barFill.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
        GameObject barGlow = CreateImage("LoadingBarGlow", barBG.transform, new Color(1f, 1f, 1f, 0.3f), stretch: false);

        SetPrivateField(loadingBar, "fillImage", barFill.GetComponent<Image>());
        SetPrivateField(loadingBar, "glowImage", barGlow.GetComponent<Image>());

        var downloadProgress = CreateTMP("DownloadProgress", screen.transform, "0 / 0 MB");
        SetPrivateField(component, "loadingBar", loadingBar);
        SetPrivateField(component, "proTipCard", tipCard);
        SetPrivateField(component, "nowLoadingText", nowLoading.GetComponent<TextMeshProUGUI>());
        SetPrivateField(component, "downloadProgressText", downloadProgress.GetComponent<TextMeshProUGUI>());

        return component;
    }

    // --- Splash Screen ---
    static SplashScreen CreateSplashScreen(Transform parent)
    {
        GameObject screen = CreateScreenPanel("SplashScreen", parent);
        var component = screen.AddComponent<SplashScreen>();
        CreateImage("Background", screen.transform, new Color(0.1f, 0.3f, 0.15f), stretch: true);

        GameObject titleArea = CreateContainer("TitleArea", screen.transform);
        var presents = CreateTMP("PresentsText", titleArea.transform, "GOLFIN presents");
        AddLocalizedText(presents, "splash_presents");
        CreateImage("ShieldLogo", titleArea.transform, Color.white, stretch: false);
        var subtitle = CreateTMP("SubtitleText", titleArea.transform, "The Invitational");
        AddLocalizedText(subtitle, "splash_subtitle");

        GameObject startBtn = CreateButtonWithPressable("StartButton", screen.transform, "START", "btn_start");
        GameObject createBtn = CreateButtonWithPressable("CreateAccountButton", screen.transform, "CREATE ACCOUNT", "btn_create_account");

        SetPrivateField(component, "startButton", startBtn.GetComponent<PressableButton>());
        SetPrivateField(component, "createAccountButton", createBtn.GetComponent<PressableButton>());

        return component;
    }

    // --- Home Screen ---
    static HomeScreen CreateHomeScreen(Transform parent)
    {
        GameObject screen = CreateScreenPanel("HomeScreen", parent);
        var component = screen.AddComponent<HomeScreen>();
        CreateImage("Background", screen.transform, new Color(0.05f, 0.15f, 0.1f), stretch: true);

        // Settings button (top-right)
        GameObject settingsBtn = CreateButtonWithPressable("SettingsButton", screen.transform, "SETTINGS", "btn_settings");
        SetPrivateField(component, "settingsButton", settingsBtn.GetComponent<PressableButton>());

        // Bottom nav bar
        GameObject navBar = CreateContainer("NavBar", screen.transform);
        GameObject navHome = CreateButtonWithPressable("NavHomeButton", navBar.transform, "HOME", "nav_home");
        GameObject navShop = CreateButtonWithPressable("NavShopButton", navBar.transform, "SHOP", "nav_shop");
        GameObject navPlay = CreateButtonWithPressable("NavPlayButton", navBar.transform, "PLAY", "nav_play");
        GameObject navProfile = CreateButtonWithPressable("NavProfileButton", navBar.transform, "PROFILE", "nav_profile");

        SetPrivateField(component, "navHomeButton", navHome.GetComponent<PressableButton>());
        SetPrivateField(component, "navShopButton", navShop.GetComponent<PressableButton>());
        SetPrivateField(component, "navPlayButton", navPlay.GetComponent<PressableButton>());
        SetPrivateField(component, "navProfileButton", navProfile.GetComponent<PressableButton>());

        return component;
    }

    // --- Settings Screen ---
    static SettingsScreen CreateSettingsScreen(Transform parent)
    {
        GameObject screen = CreateScreenPanel("SettingsScreen", parent);
        var component = screen.AddComponent<SettingsScreen>();
        CreateImage("Background", screen.transform, new Color(0.08f, 0.08f, 0.12f), stretch: true);

        var title = CreateTMP("Title", screen.transform, "SETTINGS");
        AddLocalizedText(title, "settings_title");

        GameObject backBtn = CreateButtonWithPressable("BackButton", screen.transform, "BACK", "btn_back");
        SetPrivateField(component, "backButton", backBtn.GetComponent<PressableButton>());

        GameObject soundBtn = CreateButtonWithPressable("SoundToggle", screen.transform, "Sound", "settings_sound");
        GameObject musicBtn = CreateButtonWithPressable("MusicToggle", screen.transform, "Music", "settings_music");
        GameObject notifBtn = CreateButtonWithPressable("NotificationsToggle", screen.transform, "Notifications", "settings_notifications");
        GameObject langBtn = CreateButtonWithPressable("LanguageButton", screen.transform, "Language", "settings_language");

        SetPrivateField(component, "soundToggle", soundBtn.GetComponent<PressableButton>());
        SetPrivateField(component, "musicToggle", musicBtn.GetComponent<PressableButton>());
        SetPrivateField(component, "notificationsToggle", notifBtn.GetComponent<PressableButton>());
        SetPrivateField(component, "languageButton", langBtn.GetComponent<PressableButton>());

        return component;
    }

    // --- Helpers ---
    static GameObject CreateScreenPanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        StretchFull(rt);
        panel.AddComponent<CanvasGroup>();
        Image img = panel.AddComponent<Image>();
        img.color = Color.clear;
        return panel;
    }

    static GameObject CreateContainer(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    static GameObject CreateImage(string name, Transform parent, Color color, bool stretch = false)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        if (stretch) StretchFull(rt);
        Image img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    static GameObject CreateTMP(string name, Transform parent, string text)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return go;
    }

    static GameObject CreateButtonWithPressable(string name, Transform parent, string label, string locKey)
    {
        GameObject buttonGO = CreateImage(name, parent, Color.white, stretch: false);
        buttonGO.AddComponent<Button>();
        buttonGO.AddComponent<PressableButton>();

        GameObject textGO = CreateTMP("Text", buttonGO.transform, label);
        textGO.GetComponent<TextMeshProUGUI>().color = Color.black;
        AddLocalizedText(textGO, locKey);

        return buttonGO;
    }

    static void AddLocalizedText(GameObject go, string key)
    {
        var loc = go.AddComponent<LocalizedText>();
        SetPrivateField(loc, "localizationKey", key);
    }

    static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void SetPrivateField<T>(object target, string fieldName, T value)
    {
        var field = target.GetType().GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        if (field != null)
        {
            field.SetValue(target, value);
            EditorUtility.SetDirty(target as Object);
        }
        else
        {
            Debug.LogWarning($"[GOLFIN] Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }
}

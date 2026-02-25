# GOLFIN Redux — UI System

## Quick Start

### 1. Copy Scripts into Unity
Copy the entire `Scripts/` folder into your Unity project's `Assets/Code/` directory.

### 2. Setup Localization CSV
Copy `Data/localization.csv` into `Assets/Resources/` (must be in Resources for runtime loading).

### 3. Scene Hierarchy Setup

```
Scene Root
├── [Managers] (Empty GO)
│   ├── LocalizationManager.cs
│   ├── ScreenManager.cs
│   └── GameBootstrap.cs
│
├── Canvas (Screen Space - Overlay, Canvas Scaler: Scale With Screen Size, 1170x2532)
│   ├── LogoScreen (Panel + CanvasGroup)
│   │   ├── Background (Image: black)
│   │   └── Logo (Image: GOLFIN logo, centered)
│   │
│   ├── LoadingScreen (Panel + CanvasGroup)
│   │   ├── Background (Image: bokeh/grass photo)
│   │   ├── ProTipCard (Panel + Image semi-transparent white)
│   │   │   ├── Header ("PRO TIP" - TMP + LocalizedText)
│   │   │   ├── Divider (Image: thin gold line)
│   │   │   ├── TipText (TMP - assigned in ProTipCard)
│   │   │   ├── TipImage (Image - optional illustrations)
│   │   │   └── TapNextText ("TAP FOR NEXT TIP" - TMP + LocalizedText)
│   │   ├── NowLoadingText (TMP + LocalizedText, key: "screen_loading")
│   │   ├── LoadingBarBG (Image: pill shape, navy #1a2a4a)
│   │   │   ├── LoadingBarFill (Image: Filled type, blue)
│   │   │   └── LoadingBarGlow (Image: small white circle, optional)
│   │   └── DownloadProgress (TMP: "XX / XXX MB")
│   │
│   └── SplashScreen (Panel + CanvasGroup)
│       ├── Background (Image: golfer illustration)
│       ├── TitleArea
│       │   ├── PresentsText (TMP: "GOLFIN presents")
│       │   ├── ShieldLogo (Image)
│       │   └── SubtitleText (TMP: "The Invitational")
│       ├── StartButton (Image + PressableButton + child TMP + LocalizedText)
│       └── CreateAccountButton (Image + PressableButton + child TMP + LocalizedText)
```

### 4. Component Assignment

**GameBootstrap:** Drag LogoScreen, LoadingScreen, SplashScreen references.

**LoadingScreen:** Assign LoadingBar, ProTipCard, NowLoadingText, DownloadProgressText.

**ProTipCard:** Assign HeaderText, TipText, TapNextText. Tip images are optional.

**LoadingBar:** Assign FillImage (set Image Type to Filled, Fill Method Horizontal).

**PressableButton:** Add to both START and CREATE ACCOUNT button GameObjects. Each needs an Image component.

**LocalizedText:** Add to any TMP text that needs localization. Set the `localizationKey` field.

### 5. Canvas Scaler Settings
- UI Scale Mode: **Scale With Screen Size**
- Reference Resolution: **1170 x 2532** (iPhone 14 Pro)
- Match: **0.5** (balanced width/height)

### 6. Important Notes

- **All screens start hidden** — GameBootstrap controls the flow
- **Localization CSV** is editable in Google Sheets/Excel — just export as CSV
- **Gold highlights** use `{gold}text{/gold}` tags in CSV values
- **Button press states** are automatic — just add PressableButton component
- **ProTipCard** auto-cycles every 8s, tap to advance manually
- **Loading bar** has smooth interpolation built in
- **Screen transitions** use CanvasGroup alpha fading

### 7. Adding New Languages
1. Add a new column to `localization.csv` (e.g., `fr` for French)
2. Fill in translations
3. Call `LocalizationManager.Instance.SetLanguage("fr")` at runtime

### 8. Adding New Tips
1. Add a new row to `localization.csv` with key like `tip_newfeature`
2. Add the key to LoadingScreen's `tipKeys` array in the Inspector
3. Optionally add a tip illustration sprite

## File Structure
```
Scripts/
├── Core/
│   ├── ScreenManager.cs       — Screen transition management
│   ├── ScreenBase.cs          — Abstract base with fade in/out
│   └── GameBootstrap.cs       — Startup sequence controller
├── Screens/
│   ├── LogoScreen.cs          — Logo display (no logic)
│   ├── LoadingScreen.cs       — Loading progress + tips
│   └── SplashScreen.cs        — Start/Create Account buttons
├── UI/
│   ├── PressableButton.cs     — Visual press-down feedback
│   ├── LoadingBar.cs          — Animated progress bar
│   └── ProTipCard.cs          — Tip card with auto-cycle
└── Localization/
    ├── LocalizationManager.cs — CSV loader singleton
    └── LocalizedText.cs       — Auto-updating TMP component
```

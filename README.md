# GOLFIN Redux — UI System

## Quick Start

### 1. Copy Scripts into Unity
Copy the entire `Scripts/` folder into your Unity project's `Assets/Code/` directory.

### 2. Setup Localization CSV
Copy `Data/localization.csv` into `Assets/Resources/` (must be in Resources for runtime loading).

### 3. Auto-Setup (Recommended)
In Unity: **Tools → GOLFIN → Create Full UI Scene**

This creates the entire hierarchy with all references pre-wired. No manual Inspector work needed.

### 4. Scene Hierarchy

```
Scene Root
├── [Managers]
│   ├── LocalizationManager
│   ├── ScreenManager (singleton)
│   └── GameBootstrap (controls full flow)
│
├── Canvas (Screen Space - Overlay, 1170x2532)
│   ├── LogoScreen
│   ├── LoadingScreen (progress bar + pro tips)
│   ├── SplashScreen (START + CREATE ACCOUNT)
│   ├── HomeScreen (nav bar + settings button)
│   │   ├── SettingsButton (PressableButton)
│   │   └── NavBar (Home/Shop/Play/Profile)
│   └── SettingsScreen (back + toggles)
│       ├── BackButton
│       └── Sound/Music/Notifications/Language toggles
│
└── EventSystem
```

### 5. Screen Flow
```
Logo → Loading → Splash --[START]--> Home --[⚙]--> Settings
                                      ↑               |
                                      └───[BACK]──────┘
```

### 6. Debug Tools
- **Tools → GOLFIN → Debug HomeScreen Fields** — check all field assignments
- **Tools → GOLFIN → Debug SettingsScreen Fields** — check settings wiring
- **Tools → GOLFIN → Verify All Screen Wiring** — full system check

### 7. Button Pattern
All buttons use `PressableButton` (custom component, NOT Unity Button):
- Visual press-down effect (scale + tint)
- `onClick` UnityEvent
- Requires: Image component on same GameObject

### 8. Adding New Screens
1. Create class extending `ScreenBase`
2. Add `[SerializeField]` fields for buttons and screen references
3. Wire in `OnScreenEnter()` / `OnScreenExit()`
4. Add to `CreateUIScreen.cs` for auto-setup

## File Structure
```
Scripts/
├── Core/
│   ├── ScreenManager.cs       — Screen transition management
│   ├── ScreenBase.cs          — Abstract base with fade in/out
│   └── GameBootstrap.cs       — Startup sequence (Logo→Loading→Splash→Home)
├── Screens/
│   ├── LogoScreen.cs          — Logo display
│   ├── LoadingScreen.cs       — Loading progress + tips
│   ├── SplashScreen.cs        — Start/Create Account
│   ├── HomeScreen.cs          — Main hub with nav + settings
│   └── SettingsScreen.cs      — Settings with back navigation
├── UI/
│   ├── PressableButton.cs     — Visual press-down feedback
│   ├── LoadingBar.cs          — Animated progress bar
│   └── ProTipCard.cs          — Tip card with auto-cycle
├── Editor/
│   └── CreateUIScreen.cs      — Auto-creates full scene hierarchy
└── Localization/
    ├── LocalizationManager.cs — CSV loader singleton
    └── LocalizedText.cs       — Auto-updating TMP component
```

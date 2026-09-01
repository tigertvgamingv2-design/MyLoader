
# MyLoader

A Windows desktop DLL loader/launcher for **No More Room in Hell 2**, built as a WinForms application with a CefSharp (Chromium Embedded Framework) HTML/CSS/JS front-end.

> ⚠️ **Disclaimer:** This project performs DLL injection into a running game process. Use it only with software you own or have explicit permission to modify, and only in accordance with the target game's terms of service. The maintainers are not responsible for misuse.

## Features

- Modern HTML/CSS/JS UI rendered via an embedded Chromium browser (CefSharp), styled with [Materialize CSS](https://materializecss.com/).
- Login screen with optional "Remember me" credential persistence.
- Product/target selector with optional "Remember selection" persistence.
- Downloads a DLL from a remote server and injects it into a target game process on launch.
- Custom borderless window with a draggable title area and custom minimize/close controls.
- Runs with administrator privileges and declares Per-Monitor-V2 DPI awareness for correct rendering on high-DPI/4K displays.

## Requirements

- Windows 10/11 (x86 or x64)
- [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
- Visual Studio 2022/2026 (or newer) with the **.NET desktop development** workload, if building from source
- Administrator privileges (required at runtime for process injection)

## Getting Started

### Clone the repository

```powershell
git clone https://github.com/tigertvgamingv2-design/MyLoader.git
cd MyLoader
```

### Build

1. Open `Loader\Loader.sln` in Visual Studio.
2. Restore NuGet packages (this happens automatically on build, or run **Tools → NuGet Package Manager → Restore Packages**).
3. Build the solution (`Ctrl+Shift+B`).

Key NuGet dependencies:
- `CefSharp.WinForms` / `CefSharp.Common` — embedded Chromium browser
- `cef.redist.x86` / `cef.redist.x64` — native CEF binaries
- `Newtonsoft.Json` — JSON serialization for the JS↔C# bridge

### Run

Run `Loader.exe` from the build output directory (e.g. `Loader\bin\Debug\`), or launch/debug directly from Visual Studio. The app will request administrator elevation on startup (required for DLL injection).

## Project Structure

```
Loader/
├── Content/                  # Front-end assets loaded into the embedded browser
│   ├── index.html            # Main UI page
│   ├── css/                  # Stylesheets (Materialize CSS + custom styles)
│   ├── img/                  # Logo and toolbar icons
│   └── js/                   # UI scripts (login, selector, animations, particles)
├── Core/
│   ├── Authentication/       # Login logic and result codes
│   ├── Injection/            # DLL download + process injection engine
│   ├── JSON/                 # DTOs used for the JS↔C# bridge (User, Selection)
│   ├── Objects/               # Objects bound into JavaScript via CefSharp
│   │   ├── LoginJsObject.cs      # Backs `loginObject` — login + "remember me"
│   │   ├── SelectorJsObject.cs   # Backs `selectorObject` — target selection + launch
│   │   └── MainJsObject.cs       # Backs `mainObject` — window minimize/close
│   ├── Constants.cs           # Remote API/DLL URLs
│   └── HWID.cs                 # Hardware ID generation for auth
├── Properties/                # Assembly info & user settings
├── UI/
│   ├── Main.cs                 # Main form: window setup + CefSharp browser host
│   └── Main.Designer.cs
├── app.manifest                # Requests admin elevation + DPI awareness
└── Program.cs                  # Application entry point
```

## How It Works

1. `Program.cs` starts a single WinForms `Main` window.
2. `Main.cs` configures the window (size, borderless style, DPI-aware scaling) and hosts a `ChromiumWebBrowser` pointed at `Content/index.html`.
3. Three C# objects are registered into the page's JavaScript context via CefSharp's legacy JS binding:
   - `loginObject` → `LoginJsObject` (login + credential persistence)
   - `selectorObject` → `SelectorJsObject` (target selection + launch/injection)
   - `mainObject` → `MainJsObject` (custom window controls)
4. On clicking **Launch**, `selector.js` calls `selectorObject.handleLaunch(...)`, which downloads the configured DLL (`Core/Constants.cs`) and injects it into the running target game process using the injection engine under `Core/Injection/`.

## Configuration

Remote endpoints are defined in `Loader\Core\Constants.cs`:

```csharp
public const string apiUrl = "http://<host>/handle.php";
public const string dllUrl = "http://<host>/files/example.dll";
```

Update these to point at your own authentication API and DLL host before distributing a build.

## Troubleshooting

- **Window appears as a tiny box on launch:** Ensure the manifest is embedded (`NoWin32Manifest` set to `false` in `Loader.csproj`) and that `app.manifest` declares DPI awareness — this is required for correct sizing on high-DPI/4K displays.
- **Injection doesn't happen:** Confirm the target process name in `Core/Injection/Functions.cs` matches the actual running process name (without `.exe`).
- **Dropdown appears empty or non-interactive:** Materialize CSS requires `$(selector).formSelect()` to be called after the page loads/select value changes, and the `<select>` element must not have the `disabled` attribute.

## License

No license has been specified for this project. Contact the repository owner before reuse or redistribution.

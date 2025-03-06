![Pixed logo](/Pixed.Application/Resources/icon.png "Pixed logo")
Pixed
======

Your imagination and **Pixed**. These two things are needed for pixel-arted image dreams comes true. **Pixed** is actively developed to be best software for creating pixel-art images. I'm focused on making reliable solution for touch-friendly software. If You have any ideas, questions or doubts, fell free to ask or create issue.

![GitHub License](https://img.shields.io/github/license/Mateusz-Nejman/Pixed)
![GitHub Release](https://img.shields.io/github/v/release/Mateusz-Nejman/Pixed)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Mateusz-Nejman/Pixed/dotnet-desktop.yml)
![GitHub commits since latest release](https://img.shields.io/github/commits-since/Mateusz-Nejman/Pixed/latest)
![GitHub top language](https://img.shields.io/github/languages/top/Mateusz-Nejman/Pixed)
![GitHub issue custom search in repo](https://img.shields.io/github/issues-search/Mateusz-Nejman/Pixed?query=is%3Aissue%20is%3Aopen&label=issues)
![GitHub repo size](https://img.shields.io/github/repo-size/Mateusz-Nejman/Pixed)

## Screenshots

![Pixed screenshot1](/Pixed_Screenshot1.png "Screenshot 1")
![Pixed screenshot2](/Pixed_Screenshot2.png "Screenshot 2")
![Pixed screenshot3](/Pixed_Screenshot3.png "Screenshot 3")

## Download
<a href="https://apps.microsoft.com/detail/9nwzsx6x2bgx?mode=direct">
	<img src="https://get.microsoft.com/images/en-us%20dark.svg" width="200"/>
</a>

[<img src="https://play.google.com/intl/en_us/badges/images/generic/en-play-badge.png"
      alt="Get it on Google Play"
      height="80">](https://play.google.com/store/apps/details?id=com.nejman.pixed)

[Releases](https://github.com/Mateusz-Nejman/Pixed/releases)

## Supported formats
- Pixed (own project format)
- Piskel from [piskelapp/piskel](https://github.com/piskelapp/piskel) (only open)
- PNG (open/save/palette)
- ICO (only open)
- SVG (only open)
- JSON custom palette format
- GPL palette format

## Work in progress or planned
- Animations
- Linux/MacOS support
- Better touch screen support
- AI support
- More formats support
- Extensions support

## Stack
- .NET 8
- Avalonia [AvaloniaUI/avalonia](https://github.com/avaloniaui/avalonia)
- Microsoft.Extensions.DependencyInjection [NuGet](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)
- Newtonsoft.JSON [JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- PixiEditor.ColorPicker [PixiEditor/ColorPicker](https://github.com/PixiEditor/ColorPicker)
- System.Reactive (RX) [dotnet/reactive](https://github.com/dotnet/reactive)
- LZMA SDK [Mateusz-Nejman/LZMA_SDK](https://github.com/Mateusz-Nejman/LZMA_SDK)
- AvaloniaProgressRing [Deadpikle/AvaloniaProgressRing](https://github.com/Deadpikle/AvaloniaProgressRing)
- SkiaSharp [mono/SkiaSharp](https://github.com/mono/SkiaSharp)
- BigGustave (Modified in Pixed.BigGustave) [EliotJones/BigGustave](https://github.com/EliotJones/BigGustave)
- WiX Toolset [wixtoolset/wix](https://github.com/wixtoolset/wix)
- Svg.Skia [wieslawsoltes/Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia)

## Build prerequisites
For MSI Build:

```
dotnet tool install --global wix

wix extension add  WixToolset.UI.wixext
```

## Assets
- Fluent UI System Icons [microsoft/fluentui-system-icons](https://github.com/microsoft/fluentui-system-icons)

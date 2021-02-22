# Pokemon NGC Rando
A randomizer for the GC Pokemon games written in C#.

Based off of the [GoD Tool](https://github.com/PekanMmd/Pokemon-XD-Code) written by Stars.

## Installing
### Windows
1. [Download](https://dotnet.microsoft.com/download/dotnet/5.0) the .NET 5 Desktop Runtime.
2. [Download](https://github.com/rotobash/pokemon-ngc-rando/releases) the latest release of the randomizer.
3. Run!

### Mac/Linux
Unfortunately, WinForms are only supported on Windows. The app is basic enough that it can be run through WINE most likely. This was an oversight on my part, I thought "cross-platform" meant everything but apparently not... Regardless, I plan to replace the GUI with something that is actually cross-platform. Something like [Uno](https://platform.uno/) or [MAUI](https://github.com/dotnet/maui) (I'd prefer MAUI I think but it won't be availible till September :coffin:)

The steps to install in this case are the same, but just install WINE first and perform the above through WINE.

## How To Use
This tool should be pretty straight forward to use. Open the application, load your (legally obtained) game file, select your settings, and click "Randomize" to save a new randomized copy of the game file (you'll be prompted to select where). There are tooltips for most options so just hover over an option if you don't understand it (if you still don't afterwards, my bad).

## Roadmap (updated Feb 21, 2021)
- Add Colosseum support (it's 95% there, just needs the extractors to be written)
- Rewrite the GUI to use a cross-platform library for Mac/Linux frens
- Add an in-place editor tool for changing/adding files (the hard part has theoretically been done, just needs the interface)
- Add more complicated file types extraction (like the texture files)

## Contributing
Thanks for you interest! There are many things that would be helpful:

#### Reporting bugs
Since these games are over 5 hours going as fast as possible, I can't test all different configurations and interactions to ensure they work 100%. I have however tested a few save files at different points to make sure things do what they say they do. If you find something that you think is a bug, open an issue! Please include the settings you used and a clear description of what happened and I'll look into it when I can.

#### Suggesting features/changes
I've tried to follow the settings in other popular Pokemon randomizers as a guideline.I realize these games have unique differences that don't apply to other randomizers (and v.v.) that I've tried to account for the best I can. I've also added configuration for some of the subjective settings like "Use Good Moves". If you think something is missing or needs tweaking, open an issue! I will probably close issues that are extremely minor (e.g. move this button 23 pixels down) or conflict with other settings but constructive suggestions will always be considered.

#### Adding features
If you can program and want to add a feature that would be awesome! PRs are always welcome.
The minimum to get started is to download the SDK from the link above and a text editor. If you want to change something with the GUI, I suggest to install Visual Studio as it has a built in designer which makes editing way easier (even if it is a PoS sometimes). 

I will try to review contributions as soon as I can, but I do work full time so I can't give any guarantee as to when. 

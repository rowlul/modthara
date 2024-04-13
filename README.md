# Modthara

Cross-platform Baldur's Gate 3 mod manager and library. Currently provides a documented and tested public API for reading
PAK and LSX files, as well as handling mod order at `%LOCALAPPDATA%/Larian Studios/Baldur's Gate 3/PlayerProfiles/Public/modsettings.lsx`.
GUI is planned to be developed with Avalonia UI with capabilities of obtaining mods directly from Nexus and per-save
orders.

## Usage

### App

WIP

### Library

The library is available under `Modthara.Lari` namespace. At the moment, it is able to read V18 packages (anything
that was made before game full release will fail) via `PackageReader` and LSX documents via `LsxReader`. `Mod` and
`ModSettings` can be used accordingly to parse an `LsxDocument`. Compiles for any supported .NET platform without any
external dependencies.

NuGet release is WIP.

## Credits

Many thanks to:

- [Norbyte](https://github.com/Norbyte) for the immeasurable contribution to the modding community and [LSLib](https://github.com/Norbyte/lslib).
- [Larian Studios](http://larian.com/) for Baldur's Gate 3, the 2023 Game of the Year.
- [Jetbrains](https://www.jetbrains.com/) for Rider and their educational license.

## License

Modthara is licensed under the MIT License. Please consult the attached LICENSE file for further details.

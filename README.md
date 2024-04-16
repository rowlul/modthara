﻿# Modthara

Cross-platform Baldur's Gate 3 mod manager and library. Currently provides a documented and tested public API for 
reading  PAK and LSX files, as well as handling mod order. GUI is planned to be developed with Avalonia UI ([#1](https://github.com/rowlul/modthara/issues/1)).

## Usage

### App

WIP

### Library

The library is available under `Modthara.Lari` namespace. At the moment, it is able to read V18 packages via `PackageReader` 
and LSX documents via `LsxReader`. `ModMetadata` and `ModSettings` can be used accordingly to parse an `LsxDocument`. 
Compiles for any supported .NET platform without any external dependencies.

NuGet release is WIP.

> [!WARNING]  
> `PackageReader` only accepts PAK files versioned no later than V18 (Baldur's Gate 3 Release). Mod Fixer is one of the
> famous examples that is V15, which corresponds to Early Access package format.

> [!WARNING]
> UUID parse fails if the string from LSX contains non-hexadecimal characters. So mods like Carry Weight Increased need
> to be updated to have a valid UUID.

## Credits

Many thanks to:

- [Norbyte](https://github.com/Norbyte) for the immeasurable contribution to the modding community and [LSLib](https://github.com/Norbyte/lslib).
- [Larian Studios](http://larian.com/) for Baldur's Gate 3, the 2023 Game of the Year.
- [Jetbrains](https://www.jetbrains.com/) for Rider and their educational license.

## License

Modthara is licensed under the MIT License. Please consult the attached LICENSE file for further details.

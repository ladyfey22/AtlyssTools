# AtlyssTools

A set of tools to help with Atlyss modding.
Currently a WIP.

Allows for the following:

- Loading scriptable objects from JSON files into the game cache.
- Automatically load assetbundles into AtlyssTools, and automatically load supported assets from them.
- Add new commands to the game, allowing for more interactivity with mods.
- Load asset only mods, which only require assets to be loaded.

## Installation

This expects you have BepInEx installed. If you don't, you can find
it [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html).

Please use BepInEx 5.4.23.

To install AtlyssTools, simply download the latest release from the releases page and extract it into your BepInEx plugins folder.

## Resources
[Wiki](https://github.com/ladyfey22/AtlyssTools/wiki)

[Example AtlyssTools Mod](https://github.com/ladyfey22/AtlyssToolsExampleMod)

## Dump

Includes a dump of all currently supported scriptableobjects (for now, ScriptableSkills and ScriptableStatusConditions)
in the game.
This is in the JSON format loadaable by AtlyssTools.

## Future Plans

- Test and add more scriptable objects, and add their associated JSON dump.
- Provide a full list of all assets and their paths.
- Create an easy method of asset bundling.
- Find a way to only partially replace assets, allowing for more modular modding. IE, only replace the sprite of a skill or add a new item to a shop.
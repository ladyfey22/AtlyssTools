# AtlyssTools

A set of tools to help with Atlyss modding.
Currently a WIP.

Allows for the following:

- Loading scriptable objects from JSON files into the game cache.
- Using existing game prefabs/scriptable objects to create new ones.
- Automatically load assetbundles into AtlyssTools, and automatically load supported assets from them.

## Installation

This expects you have BepInEx installed. If you don't, you can find
it [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html).

Please use BepInEx 5.4.23.
Find the latest release in the relases tab.

## Dump

Includes a dump of all currently supported scriptableobjects (for now, ScriptableSkills and ScriptableStatusConditions)
in the game.
This is in the JSON format loadaable by AtlyssTools.

## Asset Bundling

Asset bundles must be made in Unity, reccommended 2021.3.16f1. Make an Assets folder in your plugin, and put your asset
bundle and associated manifest in the assets folder.

## Json Loading

Json will be automatically loaded from your mods Assets/ScriptableSkills and Assets/ScriptableStatusConditions folders. These will be automatically loaded to the GameManager cache, and can be referenced in other scriptable objects.
Use the JSON format provided in the extracted assets folder. The only requirement is you call
```AtlyssToolsLoader.LoadPlugin("yourmodid", "YourModPath")``` in your Awake() function. Please make your mod id unique.

An example assets folder can be found in the ExampleAssets folder.

## Referencing

You can reference Atlyss assets and your own mod assets in the following ways.

For a reference to an atlyss asset, or to an asset from your mod assetbundle. This works for all unity assets.

```json
{
    "someAtlyssScriptableOrSprite": "_skill/_noviceskills/_recover/_conditionEffect_recovery"
}
```

You can also specify a specific mod to load from. This is useful if you have multiple mods with the same asset name.
It is always recommended to use this, as it is more efficient.

```json
{
    "someAtlyssScriptableOrSprite": "modid:myPicture"
}
```

Json loading is the same as the other two, but it must be in the folder associated with that scriptable object.

```json
{
    "someAtlyssScriptableOrSprite": "modid:ScriptableSkills/mySkill"
}
```

## The Future
For now, you are responsible for adding your own assets to the different parts of the game where they are needed. It is my hope to automate this in the future, as well as provide a way to copy existing assets and only specify the modifications to them, especially for GameObjects.
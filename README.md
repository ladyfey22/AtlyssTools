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

Json will be automatically loaded from your mods Assets/ScriptableSkills and Assets/ScriptableStatusConditions folders.
These will be automatically loaded to the GameManager cache, and can be referenced in other scriptable objects.
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
It is always recommended to use this, as it is more efficient. This works on both asset bundle assets and json assets.

```json
{
    "someAtlyssScriptableOrSprite": "modid:myPicture",
    "someScriptableSkill": "modid:ScriptableSkills/mySkill"
}
```

Json loading is the same as the other two, but it must be in the folder associated with that scriptable object.

```json
{
    "someAtlyssScriptableOrSprite": "ScriptableSkills/mySkill"
}
```

## Replacing Basegame Assets

Currently, scriptable assets can fully replace basegame assets. This is done by using the same name as the asset, and will fully replace the asset type.
```
    ScriptableSkill _skillName
    ScriptableStatusCondition _conditionName (all status conditions. StatusCondition, SceneTransfer, etc)
    ScriptableItem _itemName (all item types. Armor, weapon, consumable, etc)
    ScriptableShopkeep _shopName
    ScriptableQuest _questName
    ScriptableBaseClass _className
    ScriptablePlayerRace _raceName
    ScriptableCombatElement _elementName
    ScriptableCreep _creepName
    ScriptableStatModifier _modifierTag
    
    All others will just use name.
```


A full table for all supported scriptable objects will be added in the future. An example can be found in the ExampleAssets folder, which replaces the shopkeep.
```json
{
    "_skillName": "Restora",
    "_skillDescription": "My own skill",
    ...
}
```

It is highly reccommended you base it off the basegame asset, both for ease of use and to prevent any issues with the game.

## Future Plans

- Test and add more scriptable objects, and add their associated JSON dump.
- Provide a full list of all assets and their paths.
- Create an easy method of asset bundling.
- Find a way to only partially replace assets, allowing for more modular modding. IE, only replace the sprite of a skill or add a new item to a shop.
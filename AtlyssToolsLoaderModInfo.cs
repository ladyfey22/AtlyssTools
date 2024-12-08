using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AtlyssTools;


    public class AtlyssToolsLoaderModInfo
    {
        public readonly List<AssetBundle> Bundles = new();
        public readonly Dictionary<string, Object> PathToAsset = new();

        private readonly Dictionary<System.Type, IList> _scriptableObjects = new();


        // delegate lists
        public readonly List<Action> OnPreLibraryInit = new();
        public readonly List<Action> OnPostLibraryInit = new();
        public readonly List<Action> OnPreCacheInit = new();
        public readonly List<Action> OnPostCacheInit = new();

        public string ModPath;
        public string ModId;

        public void Dispose()
        {
            foreach (var bundle in Bundles)
            {
                bundle.Unload(true);
            }
        }

        public void Initialize()
        {
        }

        public void LoadScriptableType<T>() where T : ScriptableObject
        {
            // check if the type is in the list, if not, add it
            if (!_scriptableObjects.ContainsKey(typeof(T)))
            {
                _scriptableObjects.Add(typeof(T), new List<T>());
            }

            LoadJsonObjects<T>();
            LoadAssetObjects<T>();
        }

        public void LoadAssetBundles()
        {
            // check that the assets folder exists
            if (!Directory.Exists(ModPath + "/Assets"))
            {
                return;
            }
            
            string[] files = Directory.GetFiles(ModPath + "/Assets", "*", System.IO.SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.EndsWith(".manifest"))
                {
                    continue;
                }

                string manifest = file + ".manifest";
                if (!File.Exists(manifest)) // not an asset bundle
                {
                    continue;
                }

                AssetBundle bundle = AssetBundle.LoadFromFile(file);
                if (bundle != null)
                {
                    Bundles.Add(bundle);
                }
            }
        }


        public List<T> GetModScriptableObjects<T>() where T : ScriptableObject
        {
            if (_scriptableObjects.ContainsKey(typeof(T)))
            {
                return _scriptableObjects[typeof(T)] as List<T>;
            }

            return new();
        }

        public IList GetModScriptableObjects(System.Type type)
        {
            if (_scriptableObjects.TryGetValue(type, out var o))
            {
                return o;
            }

            return new List<Object>();
        }

        public void LoadAssetObjects<T>() where T : ScriptableObject
        {
            foreach (var bundle in Bundles)
            {
                T[] objects = bundle.LoadAllAssets<T>();
                foreach (var obj in objects)
                {
                    ((List<T>)_scriptableObjects[typeof(T)])
                        .Add(obj); // we don't need to mark the paths, since we're loading from the asset bundles. unity will handle it.
                }
            }
        }

        private void LoadJsonObjects<T>() where T : ScriptableObject
        {
            // we want to load it from Assets/TypeName folder
            string path = ModPath + "/Assets/" + typeof(T).Name;
            if (!Directory.Exists(path))
            {
                // skip if the directory doesn't exist
                return;
            }

            string[] files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                // cut off the ModPath/Assets/ and .json, because the LoadJsonObject method will add it back

                string relativePath = file.Replace(ModPath + "/Assets/", "");
                relativePath = relativePath.Replace(".json", "");
                relativePath = relativePath.Replace("\\", "/");
                T value = LoadJsonObject<T>(relativePath);
                
                if (value == null)
                {
                    Plugin.Logger.LogError($"Failed to load json object {file} for type {typeof(T).Name}, mod {ModId}");
                }
            }
        }

        private static readonly System.Type[] NonConcreteType =
        [
            typeof(ScriptableCondition),
            typeof(ScriptableItem)
        ];

        private static readonly List<System.Type> ConcreteType =
        [
            typeof(ScriptableStatusCondition),
            typeof(ScriptableSceneTransferCondition),
            typeof(ScriptablePolymorphCondition),
            
            typeof(ScriptableChestpiece),
            typeof(ScriptableArmorDye),
            typeof(ScriptableCape),
            typeof(ScriptableClassTome),
            typeof(ScriptableHelm),
            typeof(ScriptableLeggings),
            typeof(ScriptableRing),
            typeof(ScriptableShield),
            typeof(ScriptableSkillScroll),
            typeof(ScriptableStatusConsumable),
            typeof(ScriptableTradeItem),
            typeof(ScriptableWeapon),
        ];

        private T LoadJsonObject<T>(string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            System.Type type = typeof(T);

            // if this is a non-concrete type, we need to check the json for the type
            if (NonConcreteType.Contains(type))
            {
                // we need to deduce the type from the path
                string[] parts = path.Split('/');
                if (parts.Length < 2)
                {
                    return null;
                }

                string typeName = parts[0].ToLower();
                //find the one that matches a type in ConcreteType
                foreach (var t in ConcreteType)
                {
                    if (t.Name.ToLower() == typeName)
                    {
                        type = t;
                        break;
                    }
                }

                if (type == typeof(T))
                {
                    return null;
                }
                
            }

            // check if the dictionary already has typeof(T). if not, add it
            if (!_scriptableObjects.ContainsKey(type))
            {
                // we need to create the list indirectly
                IList newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                _scriptableObjects.Add(type, newList);
            }

            path = path.Replace(".json", "");

            if (PathToAsset.TryGetValue(path, out var value))
            {
                return value as T;
            }

            if (!File.Exists(ModPath + "/Assets/" + path + ".json"))
            {
                return null;
            }

            // means either it's not loaded, or it's not in the json assets
            string json = File.ReadAllText(ModPath + "/Assets/" + path + ".json");
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            // check that this is a ScriptableObject
            if (!type.IsSubclassOf(typeof(ScriptableObject)) && type != typeof(ScriptableObject))
            {
                Plugin.Logger.LogError($"Type {typeof(T).Name} is not a ScriptableObject");
                return null;
            }

            T obj = Utility.JsonUtility.LoadFromJson(json, type) as T;

            if (obj == null)
            {
                Plugin.Logger.LogError($"Failed to load json asset {path}");
                return null;
            }

            // keep in mind, type may no longer be T, so we can't cast it to List<T>
            IList list = _scriptableObjects[type];
            list.Add(obj);
            PathToAsset.Add(path, obj);
            // write out the type it was 
            return obj;
        }

        public T LoadModAsset<T>(string assetName) where T : Object
        {
            // check if it's in the asset bundles
            
            foreach (var bundle in Bundles)
            {
                T obj = bundle.LoadAsset<T>(assetName);
                if (obj != null)
                {
                    return obj;
                }
            }

            // check if T is a scriptable object
            if (typeof(T).IsSubclassOf(typeof(ScriptableObject)) || typeof(T) == typeof(ScriptableObject))
            {
                T obj = LoadJsonObject<T>(assetName);
                if (obj != null)
                {
                    return obj;
                }
            }

            return null;
        }
    }

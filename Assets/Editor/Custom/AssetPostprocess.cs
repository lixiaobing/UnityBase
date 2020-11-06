using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AssetPostprocess:AssetPostprocessor
{

    public static void PostprocessAssets(string[] paths)
    {
        OnPostprocessAllAssets(paths, new string[0], new string[0], new string[0]);
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        
        foreach (string delPath in deletedAssets)
        {
            string guid = AssetDatabase.AssetPathToGUID(delPath);
            setting.RemoveAssetEntry(guid);
        }

        List<string> assets = new List<string>(importedAssets);
        assets.AddRange(movedAssets);

        var index = 0;
        foreach (string assetPath in assets)
        {
            index++;
            float pregress = (float)index / (float)importedAssets.Length;
            EditorUtility.DisplayProgressBar("Importer Addressables Analysis", assetPath, pregress);





            if (assetPath.EndsWith(".meta") || assetPath.EndsWith(".cs")) continue;


            var guid = AssetDatabase.AssetPathToGUID(assetPath);



            if ((assetPath.StartsWith("Assets/Lua") || assetPath.StartsWith("Assets/ToLua/Lua")) && assetPath.EndsWith(".lua"))
            {

                var group = FindOrCreateGroup(setting, "Lua");

                var key = assetPath.Replace("Assets/Lua/", "Lua/").
                    Replace("Assets/ToLua/Lua/", "Lua/").
                    Replace(".lua", "");
                    CreateEntry(setting, group, guid, key);  //添加Label 没有实际意义

            }else if (assetPath.StartsWith("Assets/Scenes/") && assetPath.EndsWith(".unity"))
            {
                var group = FindOrCreateGroup(setting, "Scene");
                var key = assetPath.Replace("Assets/Scenes/", "").Replace(".unity", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Data/") && assetPath.EndsWith(".asset"))
            {
                var group = FindOrCreateGroup(setting, "Data");
                var key = assetPath.Replace("Assets/ResourcesAsset/Data/", "Data/").Replace(".asset", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Lua/") && (assetPath.EndsWith(".txt") || assetPath.EndsWith(".bytes")))
            {
                var group = FindOrCreateGroup(setting, "Luac");
                var key = assetPath.Replace("Assets/ResourcesAsset/Lua/", "Lua/").Replace(".txt", "").Replace(".bytes", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/UI/"))
            {
                string groupName = string.Empty;
                if (assetPath.StartsWith("Assets/ResourcesAsset/UI/Window") && assetPath.EndsWith(".prefab"))
                {
                    groupName = "UIWindow";
                }else if (assetPath.StartsWith("Assets/ResourcesAsset/UI/Component") && assetPath.EndsWith(".prefab"))
                {
                    groupName = "UIComponent";
                }
                else if (assetPath.StartsWith("Assets/ResourcesAsset/UI/Sprites") && assetPath.EndsWith(".png"))
                {
                    groupName = "UISprites";
                }
                else if (assetPath.StartsWith("Assets/ResourcesAsset/UI/Textures") && assetPath.EndsWith(".png"))
                {
                    groupName = "UITextures";
                }
                if (!string.IsNullOrEmpty(groupName))
                {
                    var group = FindOrCreateGroup(setting, groupName);
                    var key = assetPath.Replace("Assets/ResourcesAsset/UI/", "UI/").Replace(".prefab", "").Replace(".png", "");
                    CreateEntry(setting, group, guid, key);
                }
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Audio/") && assetPath.EndsWith(".prefab"))
            {
                var group = FindOrCreateGroup(setting, "Audio");
                var key = assetPath.Replace("Assets/ResourcesAsset/Audio/", "Audio/").Replace(".prefab", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/AkAudio/"))
            {
                var group = FindOrCreateGroup(setting, "AkAudio");
                var key = assetPath.Replace("Assets/ResourcesAsset/AkAudio/", "AkAudio/").Replace(".bytes", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Bullet/") && (assetPath.EndsWith(".prefab")))
            {
                var group = FindOrCreateGroup(setting, "Bullet");
                var key = assetPath.Replace("Assets/ResourcesAsset/Bullet/", "Bullet/").Replace(".prefab", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Trap/") && (assetPath.EndsWith(".prefab")))
            {
                var group = FindOrCreateGroup(setting, "Trap");
                var key = assetPath.Replace("Assets/ResourcesAsset/Trap/", "Trap/").Replace(".prefab", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Data/") && assetPath.EndsWith(".asset"))
            {
                var group = FindOrCreateGroup(setting, "Data");
                var key = assetPath.Replace("Assets/ResourcesAsset/Data/", "Data/").Replace(".asset", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Effect/") && assetPath.EndsWith(".prefab"))
            {
                var group = FindOrCreateGroup(setting, "Effect");
                var key = assetPath.Replace("Assets/ResourcesAsset/Effect/", "Effect/").Replace(".prefab", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Model/") && assetPath.EndsWith(".prefab"))
            {
                var group = FindOrCreateGroup(setting, "Model");
                var key = assetPath.Replace("Assets/ResourcesAsset/Model/", "Model/").Replace(".prefab", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Other/"))
            {
                var group = FindOrCreateGroup(setting, "Other");
                var key = assetPath.Replace("Assets/ResourcesAsset/Other/", "Other/").Replace(".prefab", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Setting/"))
            {
                var group = FindOrCreateGroup(setting, "Setting");
                var key = assetPath.Replace("Assets/ResourcesAsset/Setting/", "Setting/").Replace(".asset", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Timeline/"))
            {
                var group = FindOrCreateGroup(setting, "Timeline");
                var key = assetPath.Replace("Assets/ResourcesAsset/Timeline/", "Timeline/").Replace(".prefab", "").Replace(".playable", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/Video/") && assetPath.EndsWith(".mp4"))
            {
                var group = FindOrCreateGroup(setting, "Video");
                var key = assetPath.Replace("Assets/ResourcesAsset/Video/", "Video/").Replace(".mp4", "");
                CreateEntry(setting, group, guid, key);
            }else if (assetPath.StartsWith("Assets/ResourcesAsset/Dungeon/") && assetPath.EndsWith(".prefab"))
            {
                var group = FindOrCreateGroup(setting, "Dungeon");
                var key = assetPath.Replace("Assets/ResourcesAsset/Dungeon/", "Dungeon/").Replace(".prefab", "");
                CreateEntry(setting, group, guid, key);
            }
            else if (assetPath.StartsWith("Assets/ResourcesAsset/NavMesh/") && assetPath.EndsWith(".asset"))
            {
                var group = FindOrCreateGroup(setting, "NavMesh");
                var key = assetPath.Replace("Assets/ResourcesAsset/NavMesh/", "NavMesh/").Replace(".asset", "");
                CreateEntry(setting, group, guid, key);
            }
        }
        
        EditorUtility.ClearProgressBar();
    }

    static AddressableAssetGroup FindOrCreateGroup(AddressableAssetSettings setting, string gname)
    {
        var group = setting.FindGroup(gname);
        if (group==null)
        {
            group = setting.CreateGroup(gname, false, false, false, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            var schema = group.GetSchema<BundledAssetGroupSchema>();
            schema.BuildPath.SetVariableByName(setting, "LocalBuildPath");
            schema.LoadPath.SetVariableByName(setting, "LocalLoadPath");
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
        }

        return group;
    }

    static void CreateEntry(AddressableAssetSettings setting, AddressableAssetGroup group, string guid, string key, string label=null)
    {
        var entry = group.GetAssetEntry(guid);
        if (entry == null)
        {
            entry = setting.CreateOrMoveEntry(guid, group);
            entry.SetAddress(key);
            if (label != null)
            {
                setting.AddLabel(label, true);
                entry.SetLabel(label, true);
            }

            EditorUtility.SetDirty(setting);
        }
        else
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                setting.RemoveAssetEntry(guid);
            }
            else
            {
                if(key!=entry.address)
                {
                    entry.SetAddress(key);
                    if (label != null)
                    {
                        setting.AddLabel(label, true);
                        entry.SetLabel(label, true);
                    }

                    EditorUtility.SetDirty(setting);
                }
            }
        }
    }

    public void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;

        if (assetPath.Contains("ResourcesWork/UI"))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.Compressed;

            string folder = Path.GetDirectoryName(assetPath);
            folder = folder.Replace("\\", "/");

            string tagName = folder.Replace("Assets/ResourcesWork/", "");
            importer.spritePackingTag = tagName.ToLower();


            if (!string.IsNullOrEmpty(importer.spritePackingTag))
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                TextureImporterPlatformSettings androidTexture = new TextureImporterPlatformSettings();
                androidTexture.overridden = true;
                androidTexture.name = "Android";
                androidTexture.maxTextureSize = 1024;
                androidTexture.format = TextureImporterFormat.ETC_RGB4;
                androidTexture.allowsAlphaSplitting = true;

                importer.SetPlatformTextureSettings(androidTexture);

                TextureImporterPlatformSettings standTexture = new TextureImporterPlatformSettings();
                standTexture.name = "Standalone";
                standTexture.overridden = true;
                standTexture.format = TextureImporterFormat.RGBA32;

                importer.SetPlatformTextureSettings(standTexture);
            }
        }

        if (assetPath.Contains("ResourcesAsset/UI/Sprites"))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.Compressed;

            string folder = Path.GetDirectoryName(assetPath);
            folder = folder.Replace("\\", "/");

            string tagName = folder.Replace("Assets/ResourcesAsset/UI/Sprites/", "").Replace("/","_");
            importer.spritePackingTag = tagName.ToLower();


            if (!string.IsNullOrEmpty(importer.spritePackingTag))
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                TextureImporterPlatformSettings androidTexture = new TextureImporterPlatformSettings();
                androidTexture.overridden = true;
                androidTexture.name = "Android";
                androidTexture.maxTextureSize = 1024;
                androidTexture.format = TextureImporterFormat.ETC_RGB4;
                androidTexture.allowsAlphaSplitting = true;

                importer.SetPlatformTextureSettings(androidTexture);

                TextureImporterPlatformSettings standTexture = new TextureImporterPlatformSettings();
                standTexture.name = "Standalone";
                standTexture.overridden = true;
                standTexture.format = TextureImporterFormat.RGBA32;

                importer.SetPlatformTextureSettings(standTexture);
            }
        }

        if (assetPath.Contains("ResourcesWork/Effect"))
        {
            TextureImporterPlatformSettings standTexture = new TextureImporterPlatformSettings();
            standTexture.name = "Standalone";
            standTexture.overridden = true;
            standTexture.format = TextureImporterFormat.RGBA32;
            importer.SetPlatformTextureSettings(standTexture);
        }
    }

}
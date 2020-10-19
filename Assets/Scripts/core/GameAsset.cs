using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using UnityEngine.Video;

public static class GameAsset
{
    static readonly List<GameObject> cacheUIModel = new List<GameObject>();
    static readonly Dictionary<string, SceneInstance> cacheScene = new Dictionary<string, SceneInstance>();
    static readonly Dictionary<string, AsyncOperationHandle> cacheMap = new Dictionary<string, AsyncOperationHandle>();
    static readonly Dictionary<string, int> countMap = new Dictionary<string, int>();
    //static readonly Dictionary<string, Sprite> cacheSprites = new Dictionary<string, Sprite>();
    
    public static byte[] GetAssetBytes(string path)
    {
        ResManager.Instance.luaAssets.TryGetValue(path, out TextAsset asset);
        return asset.bytes;
    }

    public static void LoadSceneSingle(string key, System.Action onComplete)
    {
        cacheScene.Clear();

        var handle = Addressables.LoadSceneAsync(key, UnityEngine.SceneManagement.LoadSceneMode.Single);
        handle.Completed += (scene) =>
        {
            onComplete();
            cacheScene.Add(key, handle.Result);
        };
    }

    public static IEnumerator WaitLoadSceneSingle(string key, System.Action<float> onProgress)
    {
        cacheScene.Clear();
        //Debug.LogWarning($"Start Load {key}");

        var handle = Addressables.LoadSceneAsync(key, UnityEngine.SceneManagement.LoadSceneMode.Single);
        while (!handle.IsDone)
        {
            //Debug.LogWarning(handle.PercentComplete);
            onProgress(handle.PercentComplete);
            yield return null;
        }
        onProgress(1);
        cacheScene.Add(key, handle.Result);
    }

    public static void LoadSceneAdditive(string key, System.Action onComplete)
    {
        var handle = Addressables.LoadSceneAsync(key, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        handle.Completed += (scene) =>
        {
            onComplete();
            cacheScene.Add(key, handle.Result);
        };
    }

    public static IEnumerator WaitLoadSceneAdditive(string key, System.Action<float> onProgress)
    {
        var handle = Addressables.LoadSceneAsync(key, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        while (!handle.IsDone)
        {
            onProgress(handle.PercentComplete);
            yield return null;
        }
        onProgress(1);
        cacheScene.Add(key, handle.Result);
    }

    public static void UnloadScene(string sceneName)
    {
        if(cacheScene.TryGetValue(sceneName, out SceneInstance scene))
        {
            cacheScene.Remove(sceneName);
            Addressables.UnloadSceneAsync(scene);
        }
    }

    ////////////////////////////////////////////////////////////////////////

    public static void LoadLabel(string key, System.Action<Object> onComplete)
    {
        var handle = Addressables.LoadAssetAsync<Object>(key);
        handle.Completed += (asset) =>
        {
            if (asset.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete(asset.Result);
                Addressables.Release(handle);
            }
            else
            {
                onComplete(null);
            }
        };
    }

    public static void LoadAsync(string key, System.Action<Object> onComplete)
    {
        var handle = Addressables.LoadAssetAsync<Object>(key);
        handle.Completed += (asset) =>
        {
            if (asset.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete(asset.Result);
                Addressables.Release(handle);
            }
            else
            {
                onComplete(null);
            }
        };
    }

    public static void LoadAsset(string key, System.Action<Object> onComplete)
    {
        LoadAsync(key, (obj) => { onComplete(obj); });
    }

    public static void LoadUIModel(string key, System.Action<GameObject> onComplete)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(key);
        handle.Completed += (asset) =>
        {
            if (asset.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete(asset.Result);
                AddCacheUIModel(asset.Result);
            }
            else
            {
                onComplete(null);
            }
        };
    }

    public static void AddCacheUIModel(GameObject prefab)
    {
        cacheUIModel.Add(prefab);
        if (cacheUIModel.Count>5)
        {
            Addressables.Release(cacheUIModel[0]);
            cacheUIModel.RemoveAt(0);
        }
    }

    public static void LoadObjects(string[] keys, System.Action<int, Object> onComplete)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i];
            int sort = i;

            if (!cacheMap.ContainsKey(key))
            {
                var handle = Addressables.LoadAssetAsync<Object>(key);
                handle.Completed += (asset) => {
                    countMap[key]++;
                    onComplete(sort + 1, handle.Result);
                };

                countMap.Add(key, 0);
                cacheMap.Add(key, handle);
            }
            else
            {
                if (cacheMap[key].IsDone)
                {
                    countMap[key]++;
                    onComplete(sort + 1, cacheMap[key].Convert<Object>().Result);
                }
                else
                {
                    var handle = cacheMap[key];
                    handle.Completed += (asset) => {
                        countMap[key]++;
                        onComplete(sort + 1, handle.Convert<Object>().Result);
                    };
                }
            }
        }
    }

    public static IEnumerator LoadAssets(string[] keys, System.Action<int, Object> onComplete)
    {
        List<string> needKeys = new List<string>();
        List<int> needOrders = new List<int>();

        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i];
            if (!cacheMap.ContainsKey(key))
            {
                var handle = Addressables.LoadAssetAsync<Object>(key);
                cacheMap.Add(key, handle);
                countMap.Add(key, 0);

                needKeys.Add(key);
                needOrders.Add(i);
            }
            else
            {
                if (cacheMap[key].IsDone)
                {
                    countMap[key]++;
                    onComplete(i+1, cacheMap[key].Convert<Object>().Result);
                }
                else
                {
                    needKeys.Add(key);
                    needOrders.Add(i);
                }
            }
        }

        while (needKeys.Count>0)
        {
            List<string> completeKeys = new List<string>();

            for(int i = needKeys.Count - 1; i >= 0; i--)
            {
                string key = needKeys[i];
                if (!cacheMap.ContainsKey(key))
                {
                    Debug.LogError(key);
                }
                var handle = cacheMap[key];
                if (handle.IsDone)
                {
                    int order = needOrders[i];

                    countMap[key]++;
                    onComplete(order + 1, handle.Convert<Object>().Result);

                    needKeys.RemoveAt(i);
                    needOrders.RemoveAt(i);
                }
            }
            yield return null;
        }
    }

    public static void LoadPrefab(string key, System.Action<GameObject> onComplete, bool release = false)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(key);

        handle.Completed += (asset) =>
        {
            if (asset.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete(asset.Result);
                if (release)
                    Addressables.Release(handle);
            }
            else
            {
                onComplete(null);
            }
        };
    }

    public static void LoadSprite(string key, System.Action<Sprite> onComplete, bool release = false)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(key);

        handle.Completed += (asset) =>
        {
            if (asset.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete(asset.Result);
                if (release)
                    Addressables.Release(handle);
            }
            else
            {
                onComplete(null);
            }
        };
    }


    public static void LoadTexture(string key, System.Action<Texture> onComplete, bool release=false)
    {
        var handle = Addressables.LoadAssetAsync<Texture>(key);

        handle.Completed += (asset) =>
        {
            if (asset.Status == AsyncOperationStatus.Succeeded)
            {
                onComplete(asset.Result);
                if (release)
                    Addressables.Release(handle);

            }
            else
            {
                onComplete(null);
            }
        };
    }



    public static void Release(string key, int count)
    {
        int allCount = 0;
        if(countMap.TryGetValue(key, out allCount))
        {
            Debug.Log($"Release Asset {key} {count}");

            allCount = allCount - count;

            if (allCount <= 0)
            {
                var handle = cacheMap[key];
                Addressables.Release(handle);
                cacheMap.Remove(key);
                countMap.Remove(key);
            }
            else
            {
                countMap[key] = allCount;
            }
        }
        else
        {
            if (cacheMap.ContainsKey(key))
            {
                var handle = cacheMap[key];
                Addressables.Release(handle);
                cacheMap.Remove(key);
            }
        }
    }

    public static void ReleaseSingle(Object obj)
    {
        if (obj != null)
        {
            Addressables.Release(obj);
        }
    }

    public static void ReleaseAll(string key)
    {
        int allCount = 0;
        if (countMap.TryGetValue(key, out allCount))
        {
            Debug.Log($"Release Asset {key} {allCount}");

            var handle = cacheMap[key];
            Addressables.Release(handle);
            cacheMap.Remove(key);
            countMap.Remove(key);
        }
    }

    public static void PrintCacheMap()
    {
        
        foreach (KeyValuePair<string, AsyncOperationHandle> kv in cacheMap)
        {
            int count = 0;
            countMap.TryGetValue(kv.Key, out count);
            Debug.Log($"Cache Map Asset {kv.Key} {count}");
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LuaInterface;
using UnityObject = UnityEngine.Object;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Framework
{
    public class ResManager : MonoBehaviour
    {
        public static ResManager Instance
        {
            get;
            protected set;
        }

        List<UnityObject> assetsCache = new List<UnityObject>();

        private Dictionary<string, AsyncOperationHandle<TextAsset>> luaHandles = new Dictionary<string, AsyncOperationHandle<TextAsset>>();
        public Dictionary<string, TextAsset> luaAssets = new Dictionary<string, TextAsset>();

        protected void Awake()
        {
            Instance = this;
        }

        public IEnumerator CheckAssets()
        {
            var handle = Addressables.CheckForCatalogUpdates(false);

            //BootScreen.Instance.SetLabel("正在检查更新...");
            //BootScreen.Instance.SetProgress(0);

            while (!handle.IsDone)
            {
                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                List<string> catalogs = handle.Result;

                Debug.Log("Catalog Update Count:" + catalogs.Count.ToString());
                if (catalogs != null && catalogs.Count > 0)
                {
                    foreach (var catalog in catalogs)
                    {
                        Debug.Log(catalog);
                    }

                    var updateHandle = Addressables.UpdateCatalogs(catalogs, true);

                    while (!updateHandle.IsDone)
                    {
//BootScreen.Instance.SetProgress(updateHandle.PercentComplete);

                        yield return null;
                    }

                    List<object> updateKeys = new List<object>();

                    var locators = updateHandle.Result;
                    foreach (var locator in locators)
                    {
                        updateKeys.AddRange(locator.Keys);
                    }

                    yield return UpdateAssets(updateKeys.ToArray());

                    Addressables.Release(updateHandle);
                }
                else
                {
                    yield return UpdateComplete();
                }
            }
            else
            {
                yield return UpdateComplete();
            }

            Addressables.Release(handle);

            yield return null;
        }

        IEnumerator UpdateAssets(object[] keys)
        {
            var sizeHandle = Addressables.GetDownloadSizeAsync(keys);
            yield return sizeHandle;

/*            BootScreen.Instance.SetLabel("正在更新资源...");
            BootScreen.Instance.SetProgress(0);*/

            long totalDownloadSize = sizeHandle.Result;
            if (totalDownloadSize > 0)
            {
                var downloadHandle = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union);
               
                while (!downloadHandle.IsDone)
                {
                    float percent = downloadHandle.PercentComplete;
/*                    BootScreen.Instance.SetProgress(percent);
                    BootScreen.Instance.SetLabel(($"正在更新资源({(int)(totalDownloadSize * percent)}/{totalDownloadSize})..."));*/
                }
            }
            yield return null;
        }

        IEnumerator UpdateComplete()
        {
            yield return ReadyLuaFiles();
        }



        public void ClearLua()
        {
            foreach (KeyValuePair<string, TextAsset> kv in luaAssets)
            {
                Addressables.Release(kv.Value);
            }
            luaAssets.Clear();
        }

/*        public IEnumerator ReadyLuaFiles() {

            var list = new List<object> { "Lua" };

            var handle  = Addressables.LoadAssetsAsync<TextAsset>(list, (tObject) => {

                Debug.Log("xx:" + tObject.name);

            }, Addressables.MergeMode.None);

            handle.Completed += (asset) =>
            {
                if (asset.Status == AsyncOperationStatus.Succeeded)
                {

                    var count = asset.Result.Count;
                    Debug.Log("count:"+ count);
                }
                Debug.Log("Completed:" + asset.Status);
            };

            yield return handle;

        }*/



        public IEnumerator ReadyLuaFiles()
        {
            ClearLua();
            var handleMap = Addressables.LoadAssetAsync<TextAsset>("Lua/LuaFilesMap");
            yield return handleMap;

            TextAsset map = handleMap.Result as TextAsset;
            string[] files = map.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < files.Length; i++)
            {
                string fileKey = files[i];
                luaHandles[fileKey] = Addressables.LoadAssetAsync<TextAsset>(fileKey);
            }

            while (luaHandles.Count > 0)
            {
                var removeKeys = new List<string>();
                foreach (KeyValuePair<string, AsyncOperationHandle<TextAsset>> kv in luaHandles)
                {
                    if (kv.Value.IsValid() && kv.Value.Status == AsyncOperationStatus.Succeeded)
                    {
                        luaAssets.Add(kv.Key, kv.Value.Result);
                        removeKeys.Add(kv.Key);

                        Debug.Log(kv.Key + " " + kv.Value.Result.bytes.Length);
                    }
                }
                foreach (var key in removeKeys)
                {
                    luaHandles.Remove(key);
                }

                float progress = (float)(files.Length-luaHandles.Count) / (float)files.Length;
                //BootScreen.Instance.SetProgress(progress);

                yield return null;
            }

            //BootScreen.Instance.SetProgress(1);

            yield return new WaitForSeconds(0.5f);
        }

        /*
        public IEnumerator ReadyShaders()
        {
            GameAsset
        }
        */

       



    }

}








using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public enum AssetMode
    {
        AssetDatabase,
        AssetBundle
    }

    private AssetMode modeType = AssetMode.AssetDatabase;//加载模式
    private AssetBundleManifest manifest; //ab清单
    private AssetBundleManifest language_maninfest;//ab清单
    private HashSet<string> assetBundleNames = new HashSet<string>();//所有ab清单
    private Dictionary<string, AssetBundleRef> assetBundles = new Dictionary<string, AssetBundleRef>();//ab缓存
    private List<string> currentAllDependenice = new List<string>();//当前的所有依赖
    private List<string> asyncAllDependenice = new List<string>();//当前的所有依赖
    private Dictionary<string, Dictionary<string, Sprite>> sprites = new Dictionary<string, Dictionary<string, Sprite>>();//图集缓存
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private string defaultAssetPrefix = "Asset/";//根目录
    public string AssetBundleExt => ".ab";
    private float currentTime;//当前时间
    private float countTime = 5 * 60;//总时间

    public void Update(float deltaTime, int frameDuration)
    {
        currentTime += deltaTime;
        if (currentTime > countTime)
        {
            currentTime = 0;
           // ReleaseAssetBundle();
        }
    }
    //初始化
    public IEnumerator Initialize(AssetMode mode, Action<float> progress = null)
    {
        this.modeType = mode;
        if (mode == AssetMode.AssetBundle)
        {
            string mainfestPath = GetFilePath("art", "");
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(mainfestPath);
            manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] assetBundle = manifest.GetAllAssetBundles();

            string[] allABs = new string[assetBundle.Length];
            for (int i = 0; i < allABs.Length; i++)
            {
                assetBundleNames.Add(allABs[i]);
                if (progress != null) progress((float)(i + 1) / allABs.Length);
            }
        }
        yield return null;
    }

    public void Retain(string assetName)
    {
        string bundleName = GetAssetBundleName(assetName);
    }

    private string GetFilePath(string file, string root = null)
    {
        string path = Path.Combine(PeristentAssetBundlePath(root), file).Replace('\\', '/');
        if (!File.Exists(path))
        {
            path = Path.Combine(StreamingAssetBundlePath(root), file).Replace('\\', '/');
        }
        return path;
    }
    //异步加载ab
    private IEnumerator LoadAssetBundleAsync(string bundleName,Action<float> progressCallback = null)
    {
        var datas = GetAssetPaths(bundleName,ref asyncAllDependenice);
        var progress = 0f;
        var length = datas.Count;
        for (int i = 0; i < length; i++)
        {
            bundleName = datas[i];
            AssetBundleRef assetBundleRef;
            if(assetBundles.TryGetValue(bundleName,out assetBundleRef))
            {
                assetBundleRef.Retain();//增加依赖
                continue;
            }
            string path = GetFilePath(bundleName);
            var bundle = AssetBundle.LoadFromFileAsync(path);
            while (bundle.progress < 1)
            {
                progress = i + bundle.progress;
                if (progressCallback != null) progressCallback(progress / length);
                yield return waitForEndOfFrame;
            }
            yield return bundle;
            if(bundle != null)
            {
                assetBundleRef = new AssetBundleRef(bundle.assetBundle);
                assetBundles.Add(bundleName, assetBundleRef);
                yield return waitForEndOfFrame;
            }
            else
            {
                Debug.LogError(string.Format( "Bundle Not Found:{0}", bundleName));
            }
        }
    }
    //获取Bundle名字
    private string GetAssetBundleName(string assetPath)
    {
        string[] splitPath = assetPath.ToLower().Split('/');
        if (splitPath.Length != 0)
        {
            int exIndex = splitPath[splitPath.Length - 1].LastIndexOf('.');
            if(exIndex >= 0)
            {
                splitPath[splitPath.Length - 1] = splitPath[splitPath.Length - 1].Substring(0, exIndex);
            }
            string bundleBaseName = splitPath[0];
            string bundleName = bundleBaseName + AssetBundleExt;
            if (assetBundleNames.Contains(bundleName))
            {
                return bundleName;
            }
            for(int i = 1; i < splitPath.Length; i++)
            {
                bundleBaseName = bundleBaseName + "/" + splitPath[i];
                bundleName = bundleBaseName + AssetBundleExt;
                if (assetBundleNames.Contains(bundleName))
                {
                    return bundleName;
                }
            }
        }
        return string.Empty;
    }

    //stream路径
    private string StreamingAssetBundlePath(string root)
    {
        if (string.IsNullOrEmpty(root)) root = "";
        return Path.Combine("", root);
    }
    //peristen路径
    private string PeristentAssetBundlePath(string root)
    {
        if (string.IsNullOrEmpty(root)) root = "";
        return Path.Combine("", root);
    }
    //获取所有资源路径
    private List<string> GetAssetPaths(string bundleName,ref List<string> datas)
    {
        datas.Clear();
        datas.Add(bundleName);
        GetAllDependencies(bundleName, ref datas);
        return datas;
    }
    //获取所有依赖
    private void GetAllDependencies(string assetBundleName, ref List<string> data)
    {
        if (manifest == null) return;
        string[] dependencies = manifest.GetAllDependencies(assetBundleName);
        if (dependencies != null && dependencies.Length != 0)
        {
            for (int i = 0; i < dependencies.Length; i++)
            {
                if (string.IsNullOrEmpty(dependencies[i]) || data.Contains(dependencies[i])) continue;
                data.Add(dependencies[i]);
                GetAllDependencies(dependencies[i], ref data);
            }
        }
    }
}

public class AssetBundleRef
{
    private string bundleName = "";
    private int refCount = 0;
    public AssetBundle assetBundle;
    public int GetRefCount() { return refCount; }
    public AssetBundleRef(AssetBundle bundle)
    {
        assetBundle = bundle;
        Retain();
    }

    public void Retain()
    {
        refCount++;
    }

    public bool Release()
    {
        if (refCount > 0)
        {
            refCount--;
            return true;
        }
        else
        {
            Debug.Log("AssetBundleRef is 0 and the name is:" + bundleName);
            return false;
        }

    }

    public void Shutdown()
    {
        refCount = 0;
    }
}
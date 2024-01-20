using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Linq;

public class GameDataManager : Singleton<GameDataManager>
{
    private bool _isInitialized;
    public bool IsInitialized => _isInitialized;

    private List<MergeData> _datas;

    void Awake()
    {
        _isInitialized = false;
        _datas = new();
        LoadDatas().Forget();
    }

    private async UniTaskVoid LoadDatas()
    {
        _datas.Clear();

        var loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync("datas", typeof(MergeData));
        await loadResourceLocationsHandle;

        List<AsyncOperationHandle> opList = new();

        foreach (IResourceLocation location in loadResourceLocationsHandle.Result)
        {
            // CreateGenericGroupOperation() 을 사용하여 한번에 모든 처리를 기다리기 위하여 await를 사용하지 않는다.
            AsyncOperationHandle<MergeData> loadAssetHandle = Addressables.LoadAssetAsync<MergeData>(location);
            loadAssetHandle.Completed += obj =>
            {
                _datas.Add(obj.Result);
            };
            opList.Add(loadAssetHandle);
        }

        var groupOp = Addressables.ResourceManager.CreateGenericGroupOperation(opList);

        await groupOp;

        Addressables.Release(loadResourceLocationsHandle);
        _isInitialized = true;
    }

    public MergeData GetData(string key)
    {
        MergeData result = _datas.FirstOrDefault(data => string.Equals(key, data.Key));

        if(result == null)
        {
            Debug.LogError($"{key}에 해당하는 데이터가 없습니다.");
        }

        return result;
    }

    public List<MergeData> GetMergeDatas() => _datas;
}
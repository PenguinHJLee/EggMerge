using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private ObjectPool<BasePoolObject> _pool;
    private GameObject _preloadedPrefab;

    private void Start()
    {
        Initialize().Forget();
    }

#region public method
    public BasePoolObject Get() =>  _pool.Get();
#endregion

    public async UniTaskVoid Initialize()
    {        
        // 풀 생성을 하기 전에 사용할 리소스 프리팹을 먼저 로드한다.
        await LoadResourcePrefabs();
        await MakePools();
    }

    private async UniTask MakePools()
    {
        _pool = new ObjectPool<BasePoolObject>(
            CreatePooledObject,
            GetFromPool,
            ReleaseToPool,
            DestroyPooledObject,
            maxSize : 999);

        await UniTask.Yield();
    }

    private async UniTask LoadResourcePrefabs()
    {
        var op = await Addressables.LoadAssetAsync<GameObject>("BaseMergeObject");
        _preloadedPrefab = op;
    }

    private BasePoolObject CreatePooledObject()
    {
        BasePoolObject instance = _preloadedPrefab.transform.GetComponent<BasePoolObject>();
        BasePoolObject result = Instantiate(instance, transform);
        result.ObjectPool = _pool;

        return result;
    }

    private void GetFromPool(BasePoolObject poolObject)
    {
        poolObject.gameObject.SetActive(true);
    }

    private void ReleaseToPool(BasePoolObject pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }

    private void DestroyPooledObject(BasePoolObject pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
}
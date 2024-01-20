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
    public BasePoolObject Get()
    {
        return _pool.Get();
    }
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
            CreatePooledObject<BasePoolObject>,
            GetFromPool<BasePoolObject>,
            ReleaseToPool<BasePoolObject>,
            DestroyPooledObject<BasePoolObject>,
            maxSize : 999);

        await UniTask.Yield();
    }

    private async UniTask LoadResourcePrefabs()
    {
        var op = await Addressables.LoadAssetAsync<GameObject>("BaseMergeObject");
        _preloadedPrefab = op;
    }

    private T CreatePooledObject<T>() where T : BasePoolObject
    {
        BasePoolObject instance = _preloadedPrefab.transform.GetComponent<BasePoolObject>();
        BasePoolObject result = Instantiate(instance, transform);
        result.ObjectPool = _pool;

        return result as T;
    }

    private void GetFromPool<T>(T poolObject) where T : BasePoolObject
    {
        poolObject.gameObject.SetActive(true);
        poolObject.OnGet();
    }

    private void ReleaseToPool<T>(T pooledObject) where T : BasePoolObject
    {
        pooledObject.gameObject.SetActive(false);
        pooledObject.OnRelease();
    }

    private void DestroyPooledObject<T>(T pooledObject) where T : BasePoolObject
    {
        Destroy(pooledObject.gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public abstract class BasePoolObject : MonoBehaviour
{
    protected IObjectPool<BasePoolObject> objectPool;
    public IObjectPool<BasePoolObject> ObjectPool { set => objectPool = value; }

    public abstract void Initialize();
    public abstract void OnUse();
    public abstract void Release();
    protected abstract void OnDestroy();
}
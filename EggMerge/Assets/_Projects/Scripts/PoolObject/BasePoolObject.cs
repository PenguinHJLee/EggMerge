using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class BasePoolObject : MonoBehaviour
{
    protected IObjectPool<BasePoolObject> objectPool;
    public IObjectPool<BasePoolObject> ObjectPool { set => objectPool = value; }

    public void Release()
    {
        objectPool.Release(this);
    }
}
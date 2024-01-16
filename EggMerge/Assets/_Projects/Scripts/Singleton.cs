using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if(instance == null)
                {
                    GameObject go = new(nameof(T));
                    instance = go.AddComponent<T>();
                }
            }

            return instance;
        }
        private set { instance = value; }
    }
}
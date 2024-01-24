using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Transform _canvas;
    List<BaseUI> _uis;

    void Awake()
    {
        _uis = new();

        _uis = _canvas.GetComponentsInChildren<BaseUI>().ToList();

        foreach(var ui in _uis)
        {
            ui.Initialize();
        }
    }

    public void SendMessage(Type uiType, BaseUIMessage msg)
    {
        var hit = _uis.FirstOrDefault(ui => uiType == ui.GetType());

        if(hit == null)
        {
            Debug.LogError($"{uiType}에 맞는 UI 컴포넌트가 없습니다.");
            return;
        }

        hit.SendMessage(msg);
    }
}

public class BaseUI : MonoBehaviour
{
    public virtual void Initialize() { }
    public virtual void SendMessage(BaseUIMessage msg) { }
}

public class BaseUIMessage {}

public class CoinUIMessage : BaseUIMessage
{
    public int GainedCoinAmount;
}
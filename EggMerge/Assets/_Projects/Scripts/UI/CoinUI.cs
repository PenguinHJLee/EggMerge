using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI _text;
    private int _currentCoin;

    #region virtual method
    public override void Initialize()
    {
        base.Initialize();

        _text.text = "0";
        _currentCoin = 0;
    }

    public override void SendMessage(BaseUIMessage msg)
    {
        base.SendMessage(msg);

        CoinUIMessage coinMsg = msg as CoinUIMessage;

        _currentCoin++;
        _text.text = _currentCoin.ToString();
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image _readyToImg;
    [SerializeField] private Image _timerImg;

    private CancellationTokenSource _cancellationTokenSource;

    public Action OnTimerEndEvent;

    public void Initialze()
    {
        _readyToImg.enabled = false;
        _timerImg.enabled = false;
    }

    void OnDisable()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void SetTimer()
    {
        _readyToImg.enabled = true;
        _timerImg.enabled = false;
    }

    public void StartTimer(BaseMergeElement sender)
    {
        _readyToImg.enabled = false;
        _timerImg.enabled = true;

        _timerImg.fillAmount = 0f;
        _cancellationTokenSource = new();
        TimerOn(sender, _cancellationTokenSource.Token).Forget();
    }

    public void StopTimer()
    {
        _cancellationTokenSource?.Cancel();

        _readyToImg.enabled = false;
        _timerImg.enabled = false;
    }

    private async UniTaskVoid TimerOn(BaseMergeElement sender, CancellationToken token)
    {
        float remainCoolTime = sender.MergeData.CoolTime;

        while(remainCoolTime >= 0)
        {
            await UniTask.Yield(cancellationToken : token);

            remainCoolTime -= Time.deltaTime;

            float coolTimeRate = 1 - remainCoolTime / sender.MergeData.CoolTime;
            _timerImg.fillAmount = coolTimeRate;
        }

        OnTimerEndEvent?.Invoke();

        _readyToImg.enabled = true;
        _timerImg.enabled = false;
    }
}
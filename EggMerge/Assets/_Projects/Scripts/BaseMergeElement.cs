using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Microsoft.Win32.SafeHandles;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BaseMergeElement : BaseDraggable
{
    // 로직에 사용하는건 아니고, 디버깅을 위해 인스펙터에서 확인하려는 용도
    [SerializeField] string _dataKey;

    private Vector3 _originPos;
    public Vector3 OriginPos => _originPos;

    private MergeData _mergeData;
    public MergeData MergeData => _mergeData;

#region component
    private BasePoolObject _mergePoolObject;
    private TimerUI _timerUi;
    private SpriteRenderer _spriteRenderer;
#endregion

    private int _currentGenerateGage;

    void Awake()
    {
        _mergePoolObject = transform.GetComponent<BasePoolObject>();
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        _timerUi = transform.GetComponent<TimerUI>();

        _timerUi.Initialze();
        _timerUi.OnTimerEndEvent += OnTimerEndEvent;
    }

    public void SetItem(MergeData mergeData)
    {
        _mergeData = mergeData;
        _dataKey = _mergeData.Key;
        _currentGenerateGage =_mergeData.MaxGenerateCount;
        this.SetLock(false);

        // 오브젝트 스프라이트 어드레서블로 로드한 다음 set 해주기
        var spriteOp = _mergeData.SpriteAssetRef.LoadAssetAsync();
        _spriteRenderer.sprite = spriteOp.WaitForCompletion();
        _spriteRenderer.color = Color.white;

        _timerUi.StopTimer();
        if(_mergeData.IsGenerator)
        {
            _timerUi.SetTimer();
        }
        
        Addressables.Release(spriteOp);
    }

    public void ChangePosition(Vector3 position)
    {
        transform.position = position;
        _originPos = position;
    }

    public void SetOriginPosition()
    {
        _originPos = transform.position;
    }

    public bool CanBeMerged(BaseMergeElement other)
    {
        if(other.Equals(this))
            return false;

        bool isSame = other.MergeData.MergeCategory == this._mergeData.MergeCategory && other.MergeData.Level == this._mergeData.Level;
        bool isLastLevel = GameDataManager.Instance.GetMergeDatas().Max(data => data.Level) == _mergeData.Level;

        return isSame && !isLastLevel;
    }

    public void Release()
    {
        this._mergePoolObject.Release();
        _timerUi.StopTimer();
    }

    protected override void OnDragging(Vector3 currnetPosition)
    {

    }

    protected override void OnEndDrag(Vector3 currnetPosition)
    {
        if(DraggingLocked)
            return;

        SlotItem originSlot = BoardManager.Instance.GetNearestSlot(_originPos);
        SlotItem nearestSlot = BoardManager.Instance.GetNearestSlot(currnetPosition);

        if(nearestSlot.IsOccupied)
        {
            // 머지 가능한지?
            if(CanBeMerged(nearestSlot.LoadedElement))
            {
                nearestSlot.LoadedElement.Release();
                var obj = ObjectPoolManager.Instance.Get();
                BaseMergeElement mergeElement = obj.transform.GetComponent<BaseMergeElement>();

                string nextObjKey = $"{_mergeData.MergeCategory.ToString()}_{_mergeData.Level + 1}".ToLowerInvariant();
                var nextLevelData = GameDataManager.Instance.GetData(nextObjKey);

                mergeElement.ChangePosition(nearestSlot.Position);
                mergeElement.SetItem(nextLevelData);
                nearestSlot.SetOccupied(mergeElement);

                Release();
                originSlot.SetOccupied(null);
            }
            else
            {
                // 자리에 오브젝트는 있는데, 머지할 수 없는 대상이라면 자리만 바꿔준다.
                nearestSlot.ExchangePosition(originSlot);
            }
        }
        else
        {
            // 원래 있던 자리 비워주기
            originSlot.SetOccupied(null);

            ChangePosition(nearestSlot.Position);
            nearestSlot.SetOccupied(this);
        }
    }

    protected override void OnStartDrag(Vector3 currnetPosition)
    {
        SlotItem slotItem = BoardManager.Instance.GetNearestSlot(currnetPosition);
        if(slotItem.IsLockable)
            this.SetLock(true);
        else
            this.SetLock(false);
    }

    protected override void OnClick()
    {
        base.OnClick();

        // 생성기면 횟수제한이 남아 있을 때 까지 오브젝트를 생겅해준다.
        if(_mergeData.IsGenerator && _currentGenerateGage > 0)
        {
            var generateItemData = GameDataManager.Instance.GetData(_mergeData.GenerateDataKey);
            var poolObj = ObjectPoolManager.Instance.Get();
            var slot = BoardManager.Instance.GetRandomEmptySlot(transform.position);

            BaseMergeElement mergeElement = poolObj.transform.GetComponent<BaseMergeElement>();
            mergeElement.SetItem(generateItemData);
            mergeElement.ChangePosition(slot.Position);

            slot.SetOccupied(mergeElement);
            _currentGenerateGage--;

            if(_currentGenerateGage <= 0)
            {
                _timerUi.StartTimer(this);
                _spriteRenderer.color = Color.gray;
            }
        }
    }

    protected override void OnDoubleClick()
    {
        base.OnDoubleClick();

        if(_mergeData.IsRewardable)
        {
            UIManager.Instance.SendMessage(typeof(CoinUI), new CoinUIMessage() { GainedCoinAmount = _mergeData.RewardAmount });

            SlotItem originSlot = BoardManager.Instance.GetNearestSlot(_originPos);
            originSlot.SetOccupied(null);
            this.Release();
        }
    }

    private void OnTimerEndEvent()
    {
        if(_mergeData.IsGenerator)
            _currentGenerateGage = _mergeData.MaxGenerateCount;

        _spriteRenderer.color = Color.white;
    }
}
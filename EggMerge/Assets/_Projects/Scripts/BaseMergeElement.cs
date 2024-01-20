using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class BaseMergeElement : BaseDraggable
{
    [SerializeField] string _dataKey;

    private Vector3 _originPos;
    public Vector3 OriginPos => _originPos;

    private MergeData _mergeData;
    public MergeData MergeData => _mergeData;

    public void SetItem(MergeData mergeData)
    {
        _mergeData = mergeData;
        _dataKey = _mergeData.Key;
    }

    public void ChangePosition(Vector3 position)
    {
        transform.position = position;
        _originPos = position;
    }

    public bool CanBeMerged(BaseMergeElement other)
    {
        bool isSame = other.MergeData.MergeCategory == this._mergeData.MergeCategory && other.MergeData.Level == this._mergeData.Level;
        bool isLastLevel = GameDataManager.Instance.GetMergeDatas().Max(data => data.Level) == _mergeData.Level;

        return isSame && !isLastLevel;
    }

    protected override void OnDragging(Vector3 currnetPosition)
    {

    }

    protected override void OnEndDrag(Vector3 currnetPosition)
    {
        SlotItem nearestSlot = BoardManager.Instance.GetNearestSlot(currnetPosition);

        if(nearestSlot.IsOccupied)
        {
            // 머지 가능한지?
            if(CanBeMerged(nearestSlot.LoadedElement))
            {
                var obj = ObjectPoolManager.Instance.Get();
            }
            else
            {
                SlotItem originSlot = BoardManager.Instance.GetNearestSlot(_originPos);
                // 머지를 할 수 없으면 자리를 바꿔치기 한다.
                nearestSlot.ExchangePosition(originSlot);
            }
        }
        else
        {
            transform.position = nearestSlot.Position;
        }
    }

    protected override void OnStartDrag(Vector3 currnetPosition)
    {
        _originPos = transform.position;
    }

    protected override void OnClick()
    {
        base.OnClick();

        if(_mergeData.IsGenerator)
        {
            var generateItemData = GameDataManager.Instance.GetData(_mergeData.GenerateDataKey);
            var poolObj = ObjectPoolManager.Instance.Get() as MergePoolObject;
            var slot = BoardManager.Instance.GetRandomEmptySlot(transform.position);
            poolObj.InitMergeElement(generateItemData);

            poolObj.transform.position = slot.Position;
            slot.SetOccupied(poolObj.MergeElement);
        }
    }

    protected override void OnDoubleClick()
    {
        base.OnDoubleClick();
    }
}
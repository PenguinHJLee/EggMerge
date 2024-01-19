using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMergeElement : BaseDraggable
{
    private Vector3 _originPos;
    public Vector3 OriginPos => _originPos;

    private MergeData _mergeData;
    public MergeData MergeData => _mergeData;

    public void SetItem(MergeData mergeData)
    {
        _mergeData = mergeData;
    }

    public bool CanBeMerged(BaseMergeElement other)
    {
        return other.MergeData.MergeCategory == this._mergeData.MergeCategory && other.MergeData.Level == this._mergeData.Level;
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
                transform.position = _originPos;
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
    }

    protected override void OnDoubleClick()
    {
        base.OnDoubleClick();
    }
}
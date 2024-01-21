using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BaseMergeElement : BaseDraggable
{
    [SerializeField] string _dataKey;

    private Vector3 _originPos;
    public Vector3 OriginPos => _originPos;

    private MergeData _mergeData;
    public MergeData MergeData => _mergeData;

    private BasePoolObject _mergePoolObject;
    private SpriteRenderer _spriteRenderer;

    private int _currentGaenrateGage;

    void Awake()
    {
        _mergePoolObject = transform.GetComponent<BasePoolObject>();
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
    }

    public void SetItem(MergeData mergeData)
    {
        Debug.Log($"{mergeData.Key} set item...", gameObject);
        _mergeData = mergeData;
        _dataKey = _mergeData.Key;
        _currentGaenrateGage =_mergeData.MaxGenerateCount;

        // 오브젝트 스프라이트 어드레서블로 로드한 다음 set 해주기
        var spriteOp = _mergeData.SpriteAssetRef.LoadAssetAsync();
        _spriteRenderer.sprite = spriteOp.WaitForCompletion();
        
        Addressables.Release(spriteOp);
    }

    public void ChangePosition(Vector3 position)
    {
        transform.position = position;
        _originPos = position;
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
                nearestSlot.LoadedElement.Release();
                var obj = ObjectPoolManager.Instance.Get();
                BaseMergeElement mergeElement = obj.transform.GetComponent<BaseMergeElement>();

                string nextObjKey = $"{_mergeData.MergeCategory.ToString()}_{_mergeData.Level + 1}".ToLowerInvariant();
                var nextLevelData = GameDataManager.Instance.GetData(nextObjKey);

                mergeElement.transform.position = nearestSlot.Position;
                mergeElement.SetItem(nextLevelData);
                nearestSlot.SetOccupied(mergeElement);

                this._mergePoolObject.Release();
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
            nearestSlot.SetOccupied(this);

            // 원래 있던 자리 비워주기
        }
    }

    protected override void OnStartDrag(Vector3 currnetPosition)
    {
        _originPos = transform.position;
    }

    protected override void OnClick()
    {
        base.OnClick();

        // 생성기면 횟수제한이 남아 있을 때 까지 오브젝트를 생겅해준다.
        if(_mergeData.IsGenerator && _currentGaenrateGage > 0)
        {
            var generateItemData = GameDataManager.Instance.GetData(_mergeData.GenerateDataKey);
            var poolObj = ObjectPoolManager.Instance.Get();
            var slot = BoardManager.Instance.GetRandomEmptySlot(transform.position);

            BaseMergeElement mergeElement = poolObj.transform.GetComponent<BaseMergeElement>();
            mergeElement.SetItem(generateItemData);

            poolObj.transform.position = slot.Position;
            slot.SetOccupied(mergeElement);
            _currentGaenrateGage--;
        }
    }

    protected override void OnDoubleClick()
    {
        base.OnDoubleClick();
    }
}
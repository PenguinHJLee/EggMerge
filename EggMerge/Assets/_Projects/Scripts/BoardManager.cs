using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cysharp.Threading.Tasks;
using System;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Tilemap _board;
    [SerializeField] private BaseMergeElement _firstGenerator; // 최초의 생성기

    List<SlotItem> _slots;

    void Awake()
    {
        Initialize().Forget();
    }
    
    private async UniTaskVoid Initialize()
    {
        // 최초의 생성기의 data를 set 해준다.
        // 게임 데이터 로드가 끝나는 것을 기다린다.
        await UniTask.WaitUntil(() => GameDataManager.Instance.IsInitialized);
        var iceData = GameDataManager.Instance.GetData("ice_1");
        _firstGenerator.SetItem(iceData);

        _slots = new();

        // lock 기능이 있는 블록은 마지막 아랫줄로 지정한다.
        for (int x = _board.cellBounds.xMin; x < _board.cellBounds.xMax; x++)
        {
            for (int y = _board.cellBounds.yMin; y < _board.cellBounds.yMax; y++)
            {
                Vector3Int localPlace = new(x, y, 0);
                Vector3 place = _board.GetCellCenterWorld(localPlace);
                if (_board.HasTile(localPlace))
                {
                    _slots.Add(new SlotItem(place, y == _board.cellBounds.yMin));
                }
            }
        }

        // 디폴트로 만들어져있는 생성기 처리를 해준다.
        SlotItem firstSlot = GetNearestSlot(_firstGenerator.transform.position);
        firstSlot.SetOccupied(_firstGenerator);

    }

    /// <summary>
    /// SlotItem의 자리가 비워져있는지와는 상관없이 가까운 SlotItem을 반환해준다.
    /// </summary>
    /// <param name="objectPosition"></param>
    /// <returns></returns>
    public SlotItem GetNearestSlot(Vector3 objectPosition) => _slots.OrderBy(slot => Vector3.Distance(slot.Position, objectPosition)).FirstOrDefault();

    /// <summary>
    /// 비어있고, 가까운 랜덤 SlotItem을 반환해준다.
    /// </summary>
    /// <returns></returns>
    public SlotItem GetRandomEmptySlot(Vector3 objectPosition) 
        => _slots.OrderBy(slot => Vector3.Distance(slot.Position, objectPosition)).Where(slot => !slot.IsOccupied).First();
}

public class SlotItem
{
    public Vector3 Position { get; private set;}
    public bool IsOccupied { get; private set; }
    public bool IsLockable { get; private set; }
    public BaseMergeElement LoadedElement { get; private set; }

    public SlotItem(Vector3 position, bool isLockable)
    {
        Position = position;
        IsOccupied = false;
        IsLockable = isLockable;
        LoadedElement = null;
    }

    public void SetOccupied(BaseMergeElement loadedElement)
    {
        IsOccupied = loadedElement != null;
        LoadedElement = loadedElement;
    }
}
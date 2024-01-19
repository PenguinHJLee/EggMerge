using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Tilemaps;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Tilemap board;

    List<SlotItem> _slots;

    void Awake()
    {
        Initialize();
    }
    
    void Initialize()
    {
        _slots = new();

        // foreach (var pos in board.cellBounds.allPositionsWithin)
        // {   
        //     Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
        //     Vector3 place = board.GetCellCenterWorld(localPlace);
        //     if (board.HasTile(localPlace))
        //     {
        //         slots.Add(place);
        //         Debug.Log($"{place} : {pos.x}, {pos.y}, {pos.z}");
        //     }
        // }

        // lock 기능이 있는 블록은 마지막 아랫줄로 지정한다.
        for (int x = board.cellBounds.xMin; x < board.cellBounds.xMax; x++)
        {
            for (int y = board.cellBounds.yMin; y < board.cellBounds.yMax; y++)
            {
                Vector3Int localPlace = new(x, y, 0);
                Vector3 place = board.GetCellCenterWorld(localPlace);
                if (board.HasTile(localPlace))
                {
                    _slots.Add(new SlotItem(place, y == board.cellBounds.yMin));
                }
            }
        }
    }

    public SlotItem GetNearestSlot(Vector3 objectPosition) => _slots.OrderBy(slot => Vector3.Distance(slot.Position, objectPosition)).FirstOrDefault();
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
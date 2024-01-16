using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Tilemap board;

    List<Vector3> slots;

    void Awake()
    {
        Initialize();
    }
    
    void Initialize()
    {
        slots = new();

        foreach (var pos in board.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = board.GetCellCenterWorld(localPlace);
            if (board.HasTile(localPlace))
            {
                slots.Add(place);
            }
        }
    }

    public Vector3Int GetCellPosition(Vector3 worldPosition) => board.WorldToCell(worldPosition);
    public Vector3 GetCellWorldPosition(Vector3 objectPosition) => slots.OrderBy(pos => Vector3.Distance(pos, objectPosition)).FirstOrDefault();
}
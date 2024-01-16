using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMergeElement : BaseDraggable
{
    protected override void OnDragging(Vector3 currnetPosition)
    {

    }

    protected override void OnEndDrag(Vector3 currnetPosition)
    {
        var result = BoardManager.Instance.GetCellPosition(currnetPosition);
        var result2 = BoardManager.Instance.GetCellWorldPosition(currnetPosition);

        Debug.Log($"cell position : {result}, cell world position : {result2}");

        transform.position = result2;
    }

    protected override void OnStartDrag(Vector3 currnetPosition)
    {
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;

public abstract class BaseDraggable : MonoBehaviour
{
    private bool isDragging;

    public bool IsDragging => isDragging;

    void Start()
    {
        isDragging = false;
    }

    private void OnMouseDown()
    {
        if(!isDragging)
            OnStartDrag(transform.position);

        isDragging = true;
    }

    private void OnMouseDrag()
    {
        Vector3 currnePosition = GetMouseWorldPosition();
        transform.position = new Vector3(currnePosition.x, currnePosition.y, 0);

        OnDragging(transform.position);
    }

    private void OnMouseUp()
    {
        if(isDragging)
            OnEndDrag(transform.position);

        isDragging = false;
    }

    // 현재 커서의 위치를 월드 포지션 값으로 return 한다
    private Vector3 GetMouseWorldPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);

#region abstract method
    protected abstract void OnStartDrag(Vector3 currnetPosition);
    protected abstract void OnEndDrag(Vector3 currnetPosition);
    protected abstract void OnDragging(Vector3 currnetPosition);
#endregion
}
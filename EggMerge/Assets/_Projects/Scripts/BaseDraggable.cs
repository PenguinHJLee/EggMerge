using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public abstract class BaseDraggable : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool isDragging;

    public bool IsDragging => isDragging;

    void Start()
    {
        isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        OnStartDrag(transform.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 currnePosition = GetMouseWorldPosition();
        transform.position = new Vector3(currnePosition.x, currnePosition.y, 0);

        OnDragging(transform.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDrag(transform.position);
        isDragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount == 1)
            OnClick();
        else if(eventData.clickCount == 2)
            OnDoubleClick();
    }

    // 현재 커서의 위치를 월드 포지션 값으로 return 한다
    private Vector3 GetMouseWorldPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);

#region abstract method
    protected abstract void OnStartDrag(Vector3 currnetPosition);
    protected abstract void OnEndDrag(Vector3 currnetPosition);
    protected abstract void OnDragging(Vector3 currnetPosition);
#endregion

#region virtual method
    protected virtual void OnClick() { }
    protected virtual void OnDoubleClick() { }
#endregion
}
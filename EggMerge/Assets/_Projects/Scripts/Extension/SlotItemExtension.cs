using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SlotItemExtension
{
    public static void ExchangePosition(this SlotItem origin, SlotItem target)
    {
        BaseMergeElement originOne = origin.LoadedElement;

        origin.LoadedElement.transform.position = target.LoadedElement.OriginPos;
        target.LoadedElement.transform.position = origin.LoadedElement.OriginPos;

        origin.LoadedElement.SetOriginPosition();
        target.LoadedElement.SetOriginPosition();

        origin.SetOccupied(target.LoadedElement);
        target.SetOccupied(originOne);
    }
}
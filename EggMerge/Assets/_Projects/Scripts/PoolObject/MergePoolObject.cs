using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergePoolObject : BasePoolObject
{
    private BaseMergeElement _mergeElement;
    public BaseMergeElement MergeElement => _mergeElement;

    public void InitMergeElement(MergeData mergeData)
    {
        _mergeElement.SetItem(mergeData);
    }

#region interfacem method
    public override void OnGet()
    {
        base.OnGet();

        if(_mergeElement == null)
            _mergeElement = transform.GetComponent<BaseMergeElement>();
    }
#endregion
}
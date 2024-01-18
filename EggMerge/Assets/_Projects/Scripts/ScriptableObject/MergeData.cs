using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MergeData : ScriptableObject
{

}

public record MergeThing
{
    public int Level;
    public MergeCategory MergeCategory;
    public AssetReference SpriteAssetRef;
    public bool IsGenerator;
}

public enum MergeCategory
{
    Penguin,
    Ice,
}
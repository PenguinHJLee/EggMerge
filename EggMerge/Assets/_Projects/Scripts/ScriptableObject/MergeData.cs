using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "게임 데이터/머지 오브젝트 데이터")]
public class MergeData : ScriptableObject
{
    [SerializeField] private string _key;
    [SerializeField] private MergeCategory _mergeCategory;
    [SerializeField] private int _level;
    [SerializeField] private AssetReferenceSprite _spriteAssetRef;
    [SerializeField] private bool _isGenerator;
    [SerializeField] private int _maxGenerateCount;
    [SerializeField] private float _coolTime;
    [SerializeField] private string _generateDataKey;
    [SerializeField] private bool _isRewarable;
    [SerializeField] private int _rewardAmount;

    public string Key => _key;
    public MergeCategory MergeCategory => _mergeCategory;
    public int Level => _level;
    public AssetReferenceSprite SpriteAssetRef => _spriteAssetRef;
    public bool IsGenerator => _isGenerator;
    public int MaxGenerateCount => _maxGenerateCount;
    public float CoolTime => _coolTime;
    public string GenerateDataKey => _generateDataKey;
    public bool IsRewardable => _isRewarable;
    public int RewardAmount => _rewardAmount;
}

public enum MergeCategory
{
    Penguin,
    Ice,
    Coin,
}
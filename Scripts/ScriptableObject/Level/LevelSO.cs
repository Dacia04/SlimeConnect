using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelSO",menuName = "Custom/LevelSO")]
public class LevelSO : ScriptableObject
{
    [field: SerializeField] public int BaseScore;
    [field: SerializeField] public int BaseCoin;
    [field: SerializeField] public int BaseMinWidthMap;
    [field: SerializeField] public int BaseMaxWidthMap;
    [field: SerializeField] public int BaseMinHeightMap;
    [field: SerializeField] public int BaseMaxHeightMap;
    [field: SerializeField] public int NumberOfSlimeType;
    [field: SerializeField] public float BaseMinTimePlay;
    [field: SerializeField] public float BaseMaxTimePlay;
    [field: SerializeField] public int ShuffleBuffBase;
    [field: SerializeField] public int HintBuffBase;
    [field: SerializeField] public int AddTimeBuffBase;

    [field: SerializeField] public List<GameObject> GiftList;
}

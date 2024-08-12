using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSO",menuName = "Custom/TileSO")]
public class TileSO : ScriptableObject
{
    [field: SerializeField] public List<GameObject> TilePrefabs;
    [field: SerializeField] public GameObject Node;
}

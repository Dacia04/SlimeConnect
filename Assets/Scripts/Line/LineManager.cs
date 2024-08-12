using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static LineManager Instance { get; private set;}
    [SerializeField]private GameObject linePrefab;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public GameObject SetUp(List<Transform> points)
    {
        GameObject line = Instantiate(linePrefab,Vector2.zero,Quaternion.identity);
        LineControl lineControl = line.GetComponent<LineControl>();
        lineControl.SetUpLineData(points);
        return line;
    }
    public GameObject SetUp(Transform point1, Transform point2)
    {
        GameObject line = Instantiate(linePrefab,Vector2.zero,Quaternion.identity);
        LineControl lineControl = line.GetComponent<LineControl>();
        List<Transform> points = new(){point1, point2};
        lineControl.SetUpLineData(points);
        return line;
    }


}

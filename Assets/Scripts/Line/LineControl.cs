using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineControl : MonoBehaviour
{
    private LineRenderer lr;
    private List<Transform> points;

    private  void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLineData(List<Transform> points)
    {
        lr.positionCount = points.Count;
        this.points = new List<Transform>();
        this.points = points;
    }

    private void Update() {
        for(int i=0;i< points.Count; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }
}

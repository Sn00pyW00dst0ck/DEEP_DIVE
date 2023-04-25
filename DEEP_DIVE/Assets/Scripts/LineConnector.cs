using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public GameObject[] objects;

    private LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
        line.positionCount = objects.Length;
    }

    // Update is called once per frame
    void Update()
    {

        for(int i = 0; i < objects.Length; i++)
        {
            line.SetPosition(i, objects[i].transform.position);
        }
    }
}

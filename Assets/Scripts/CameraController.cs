using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject obj;
    private float offset;
    private Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        transform.position = newPos;
    }

    void UpdatePosition()
    {
        Vector3 focus = obj.transform.position;
        newPos = new Vector3(focus.x, focus.y, offset);
    }
}

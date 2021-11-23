using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform transform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        transform.position = GameObject.Find("Player").GetComponent<Transform>().position;
    }
}

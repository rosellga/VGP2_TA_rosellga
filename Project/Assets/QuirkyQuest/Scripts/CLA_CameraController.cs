using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLA_CameraController : MonoBehaviour
{
    public GameObject parent_;

    private void Update()
    {
        transform.LookAt(parent_.transform.position + Vector3.up);
    }
}

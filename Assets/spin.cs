using System;
using UnityEngine;

public class spin : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(0f, 1f, 0f), Space.World);
    }
}

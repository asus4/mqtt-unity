using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    [SerializeField]
    float positionMultiplier = 1;

    [SerializeField]
    float rotationMultiplier = 1;

    Vector3 initialPos;
    Vector3 initialRot;

    private void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        var time = Time.time;
        Vector3 p = initialPos + new Vector3(
            Mathf.PerlinNoise(time, 0) - 0.5f,
            Mathf.PerlinNoise(time, 1) - 0.5f,
            Mathf.PerlinNoise(time, 2) - 0.5f
        ) * positionMultiplier;
        Vector3 r = initialRot + new Vector3(
            Mathf.PerlinNoise(time, 4) - 0.5f,
            Mathf.PerlinNoise(time, 5) - 0.5f,
            Mathf.PerlinNoise(time, 6) - 0.5f
        ) * rotationMultiplier;
        transform.SetPositionAndRotation(p, Quaternion.Euler(r));
    }
}

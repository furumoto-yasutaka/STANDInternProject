using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisOut : MonoBehaviour
{
    [SerializeField]
    private float simulationSpeed = 1.5f;
    private float destroyTimeLimit = 0.0f;
    private float destroyTimeLimitCount = 0.0f;
    private Material material;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        material.SetFloat("_speed", simulationSpeed);
        destroyTimeLimit = 1.0f / simulationSpeed;
    }

    void Update()
    {
        if (destroyTimeLimitCount >= destroyTimeLimit)
        {
            destroyTimeLimitCount = 0.0f;
            Destroy(gameObject);
        }
        else
        {
            destroyTimeLimitCount += Time.deltaTime;
            if (destroyTimeLimitCount >= destroyTimeLimit)
            {
                destroyTimeLimitCount = destroyTimeLimit - 0.0001f;
            }
            material.SetFloat("_time", destroyTimeLimitCount);
        }
    }
}

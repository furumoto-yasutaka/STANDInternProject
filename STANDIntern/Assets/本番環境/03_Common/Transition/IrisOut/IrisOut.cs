using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisOut : MonoBehaviour
{
    public enum State
    {
        Wait = 0,
        Transition,
    }

    [SerializeField]
    private float simulationSpeed = 1.5f;
    private float destroyTimeLimit = 0.0f;
    private float destroyTimeLimitCount = 0.0f;
    private Material material;
    [SerializeField]
    private bool IrisIn = false;
    [SerializeField]
    private float WaitTime = 0.5f;
    private float WaitTimeCount = 0.0f;
    private State state = State.Wait;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        material.SetFloat("_speed", simulationSpeed);
        destroyTimeLimit = 1.0f / simulationSpeed;
        WaitTimeCount = WaitTime;
    }

    void Update()
    {
        switch (state)
        {
            case State.Wait:
                if (WaitTimeCount <= 0.0f)
                {
                    WaitTimeCount = 0.0f;
                    state = State.Transition;
                }
                else
                {
                    WaitTimeCount -= Time.deltaTime;
                }

                if (IrisIn)
                {
                    material.SetFloat("_time", destroyTimeLimit - 0.00001f);
                }
                else
                {
                    material.SetFloat("_time", 0.0f);
                }
                break;

            case State.Transition:
                if (destroyTimeLimitCount >= destroyTimeLimit)
                {
                    destroyTimeLimitCount = destroyTimeLimit - 0.00001f;
                    Destroy(gameObject);
                }
                else
                {
                    destroyTimeLimitCount += Time.deltaTime;
                    if (destroyTimeLimitCount >= destroyTimeLimit)
                    {
                        destroyTimeLimitCount = destroyTimeLimit - 0.00001f;
                        if (IrisIn)
                        {
                            GetComponent<TransitionCallBack>().FadeInCompleteCallBack();
                        }
                        else
                        {
                            GetComponent<TransitionCallBack>().FadeOutCompleteCallBack();
                        }
                    }

                    if (IrisIn)
                    {
                        material.SetFloat("_time", destroyTimeLimit - destroyTimeLimitCount);
                    }
                    else
                    {
                        material.SetFloat("_time", destroyTimeLimitCount);
                    }
                }
                break;
        }
    }
}

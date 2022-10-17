using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningWindow : NormalWindow
{
    [SerializeField]
    private RectTransform ShakeObj;
    [SerializeField]
    private float ShakeRange = 10.0f;
    [SerializeField]
    private float ShakeTime = 0.5f;
    private float ShakeTimeCount = 0.0f;
    private Vector3 InitialPos;

    protected override void Awake()
    {
        base.Awake();

        InitialPos = transform.position;
    }

    void Update()
    {
        if (ShakeTimeCount > 0.0f)
        {
            WindowShake();

            ShakeTimeCount -= Time.deltaTime;
            if (ShakeTimeCount <= 0.0f)
            {
                ShakeTimeCount = 0.0f;
                transform.position = InitialPos;
            }
        }
    }

    public override void Open()
    {
        base.Open();

        ShakeTimeCount = ShakeTime;
    }

    public void WindowShake()
    {
        Vector3 pos = Random.insideUnitSphere * ShakeRange;
        pos.z = 0.0f;
        ShakeObj.localPosition = InitialPos + pos;
    }
}

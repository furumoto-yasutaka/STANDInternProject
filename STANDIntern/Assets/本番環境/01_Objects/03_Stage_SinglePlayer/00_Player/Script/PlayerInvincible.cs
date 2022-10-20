using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincible : MonoBehaviour
{
    [SerializeField]
    private float invincibleTime = 5.0f;
    private float invincibleTimeCount = 0.0f;
    private bool isInvincible = false;

    public bool IsInvincible { get { return isInvincible; } }

    void Update()
    {
        if (isInvincible)
        {
            if (invincibleTimeCount <= 0.0f)
            {
                invincibleTimeCount = 0.0f;
                isInvincible = false;
            }
            else
            {
                invincibleTimeCount -= Time.deltaTime;
            }
        }
    }

    public void SetInvincible()
    {
        isInvincible = true;
        invincibleTimeCount = invincibleTime;
    }
}

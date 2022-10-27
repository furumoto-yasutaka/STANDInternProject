using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincible : MonoBehaviour
{
    private float invincibleTimeCount = 0.0f;
    private bool isInvincible = false;

    private float effectWeightMax = 0.5f;
    private float effectWeightMin = 0.1f;
    private float effectWeight = 0.0f;
    private float effectSubSpeed = 0.6f;

    private Material bodyMaterial;
    private Material legMaterial;

    public bool IsInvincible { get { return isInvincible; } }

    void Awake()
    {
        bodyMaterial = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material;
        legMaterial = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().material;

        bodyMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
        legMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }

    void Start()
    {
        bodyMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
        legMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }

    void Update()
    {
        if (isInvincible)
        {
            if (invincibleTimeCount <= 0.0f)
            {
                invincibleTimeCount = 0.0f;
                isInvincible = false;

                bodyMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
                legMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
            else
            {
                invincibleTimeCount -= Time.deltaTime;

                bodyMaterial.SetColor("_AddColor", new Color(effectWeight, effectWeight, effectWeight, 0.0f));
                legMaterial.SetColor("_AddColor", new Color(effectWeight, effectWeight, effectWeight, 0.0f));

                effectWeight -= effectSubSpeed * Time.deltaTime;

                if (effectWeight <= effectWeightMin)
                {
                    effectWeight = effectWeightMax;
                }
            }
        }
    }

    public void SetInvincible(float invincibleTime)
    {
        isInvincible = true;
        invincibleTimeCount = invincibleTime;
        effectWeight = effectWeightMax;
    }
}

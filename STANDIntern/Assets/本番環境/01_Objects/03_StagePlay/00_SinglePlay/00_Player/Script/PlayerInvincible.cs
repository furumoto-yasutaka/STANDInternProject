using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincible : MonoBehaviour
{
    //=====��������擾
    [SerializeField]
    private Material bodyMaterial;
    [SerializeField]
    private Material legMaterial;

    [SerializeField]
    private float spownInvincibleTime = 2.0f;

    // ���G���ǂ���
    private bool isInvincible = false;
    // �c��̖��G����
    private float invincibleTimeCount = 0.0f;
    // ���Z�F�̋���
    private float effectColorWeight = 0.0f;

    // ���Z�F�̋����̍ő�l
    private static readonly float effectWeightMax = 0.5f;
    // ���Z�F�̋����̍ŏ��l
    private static readonly float effectWeightMin = 0.1f;
    // ���Z�F�̌������x(���b)
    private static readonly float effectWeightSubSpeed = 0.6f;

    public bool IsInvincible { get { return isInvincible; } }

    void Awake()
    {
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
            //=====���G���Ԃ̊ԓ_�ŏ������J��Ԃ�
            if (invincibleTimeCount <= 0.0f)
            {// �_�ŏI��
                invincibleTimeCount = 0.0f;
                isInvincible = false;

                bodyMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
                legMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
            else
            {// �_�ŏ���
                invincibleTimeCount -= Time.deltaTime;

                bodyMaterial.SetColor("_AddColor", new Color(effectColorWeight, effectColorWeight, effectColorWeight, 0.0f));
                legMaterial.SetColor("_AddColor", new Color(effectColorWeight, effectColorWeight, effectColorWeight, 0.0f));

                // �F�̋���������
                effectColorWeight -= effectWeightSubSpeed * Time.deltaTime;

                // ���̒l�܂ŉ���������ő�l�ɖ߂�
                if (effectColorWeight <= effectWeightMin)
                {
                    effectColorWeight = effectWeightMax;
                }
            }
        }
    }

    /// <summary>
    /// ���G��Ԃ��J�n
    /// </summary>
    public void StartSpownInvincible()
    {
        isInvincible = true;
        invincibleTimeCount = spownInvincibleTime;
        effectColorWeight = effectWeightMax;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem blowEffect;  // �Ԃ���уG�t�F�N�g

    public static readonly float impactWeakEffThreshold = 3.0f;
    public static readonly float impactMiddleEffThreshold = 8.0f;
    public static readonly float impactStrongEffThreshold = 15.0f;

    public static readonly float blowEffStartThreshold = 10.0f;
    public static readonly float blowEffEndThreshold = 8.0f;

    public static readonly float hitEffectStrongThreshold = 12.0f;

    public static readonly float kickedImpactWaveStrongThreshold = 15.0f;

    void Start()
    {

    }

    public void PlayBlowEff()
    {
        blowEffect.Play();
    }

    public void StopBlowEff()
    {
        blowEffect.Stop();
    }

    public void PlayImpactWeakEff(Vector3 pos, Quaternion rotate)
    {
        EffectContainer.Instance.PlayEffect("�Փ�(��)", pos, rotate);
    }

    public void PlayImpactMiddleEff(Vector3 pos, Quaternion rotate)
    {
        EffectContainer.Instance.PlayEffect("�Փ�(��)", pos, rotate);
    }

    public void PlayImpactStrongEff(Vector3 pos, Quaternion rotate)
    {
        EffectContainer.Instance.PlayEffect("�Փ�(��)", pos, rotate);
    }

    public void PlayDeathEff(Vector3 pos, Quaternion rotate)
    {
        EffectContainer.Instance.PlayEffect("���S", pos, rotate);
    }

    public void PlayKickedImpactStrongEff(Vector3 pos)
    {
        EffectContainer.Instance.PlayEffect("�L�b�N(��)", pos);
    }

    public void PlayKickedImpactNormalEff(Vector3 pos)
    {
        EffectContainer.Instance.PlayEffect("�L�b�N(��)", pos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerBattleSumoPoint playerBattleSumoPoint;
    [SerializeField]
    private PlayerEffect playerEffectManager;
    [SerializeField]
    private Rigidbody2D rb;

    private void Start()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            //=====�Փ˃G�t�F�N�g����
            float sqrMag = rb.velocity.sqrMagnitude;
            Vector2 normal = collision.contacts[0].normal;
            float angle = Vector2.SignedAngle(Vector2.up, normal);

            if (sqrMag >= PlayerEffect.impactStrongEffThreshold * PlayerEffect.impactStrongEffThreshold)
            {
                playerEffectManager.PlayImpactWeakEff(collision.contacts[0].point, Quaternion.AngleAxis(angle, Vector3.back));
            }
            else if (sqrMag >= PlayerEffect.impactMiddleEffThreshold * PlayerEffect.impactMiddleEffThreshold)
            {
                playerEffectManager.PlayImpactMiddleEff(collision.contacts[0].point, Quaternion.AngleAxis(angle, Vector3.back));
            }
            else if (sqrMag >= PlayerEffect.impactWeakEffThreshold * PlayerEffect.impactWeakEffThreshold)
            {
                playerEffectManager.PlayImpactStrongEff(collision.contacts[0].point, Quaternion.AngleAxis(angle, Vector3.back));
            }
        }

        // ���ʉ��Đ�
        AudioManager.Instance.PlaySe("�o�E���h");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // �G�v���C���[�Ɏ������}�[�N����悤���N�G�X�g����
            playerBattleSumoPoint.RequestContactMark(
                collision.transform.parent.GetComponent<PlayerBattleSumoPoint>(),
                collision.transform.GetComponent<Rigidbody2D>().velocity);


            // �Ԃ���уG�t�F�N�g�̍Đ��m�F
            collision.transform.parent.GetComponent<PlayerController>().CheckBlowStart();
        }
    }
}

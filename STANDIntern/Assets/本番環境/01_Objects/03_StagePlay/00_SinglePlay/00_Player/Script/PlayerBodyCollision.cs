using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyCollision : MonoBehaviour
{
    private BattleSumoManager battleSumoManager;
    private static float hitEffectWeakThreshold = 3.0f;
    private static float hitEffectMidleThreshold = 8.0f;
    private static float hitEffectStrongThreshold = 15.0f;
    private Rigidbody2D rb;

    private void Start()
    {
        int index = (int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager;
        battleSumoManager = GameObject.FindGameObjectWithTag("Managers").transform.GetChild(index).GetComponent<BattleSumoManager>();
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlaySe("ÉoÉEÉìÉh");

        if (collision.collider.CompareTag("Platform"))
        {
            float sqrMag = rb.velocity.sqrMagnitude;
            Vector2 normal = collision.contacts[0].normal;
            float angle = Vector2.SignedAngle(Vector2.up, normal);

            if (sqrMag >= hitEffectStrongThreshold * hitEffectStrongThreshold)
            {
                EffectContainer.Instance.PlayEffect("è’ìÀ(ã≠)", collision.contacts[0].point, Quaternion.AngleAxis(angle, Vector3.back));
            }
            else if (sqrMag >= hitEffectMidleThreshold * hitEffectMidleThreshold)
            {
                EffectContainer.Instance.PlayEffect("è’ìÀ(íÜ)", collision.contacts[0].point, Quaternion.AngleAxis(angle, Vector3.back));
            }
            else if (sqrMag >= hitEffectWeakThreshold * hitEffectWeakThreshold)
            {
                EffectContainer.Instance.PlayEffect("è’ìÀ(é„)", collision.contacts[0].point, Quaternion.AngleAxis(angle, Vector3.back));
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            battleSumoManager.RequestContactMark(
                collision.transform.parent.GetComponent<PlayerId>().Id,
                transform.parent.parent.GetComponent<PlayerId>().Id,
                collision.transform.GetComponent<Rigidbody2D>().velocity);
            collision.transform.parent.GetComponent<PlayerController>().CheckBlowStart();
        }
    }
}

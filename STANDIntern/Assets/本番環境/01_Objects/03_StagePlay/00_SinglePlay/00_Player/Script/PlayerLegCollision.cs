using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    private List<Collider2D> playerList = new List<Collider2D>();
    private BattleSumoManager battleSumoManager;
    private float strongKickWaveThreshold = 15.0f;

    private void Start()
    {
        int index = (int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager;
        battleSumoManager = GameObject.FindGameObjectWithTag("Managers").transform.GetChild(index).GetComponent<BattleSumoManager>();
    }

    private void OnEnable()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerController.transform.GetChild(0).GetChild(0).GetComponent<Collider2D>());
    }

    private void OnDisable()
    {
        playerList.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") &&
            !playerList.Contains(collision))
        {
            playerController.KickPlayerAddForce();
            battleSumoManager.RequestDeleteMark(playerController.transform.GetComponent<PlayerId>().Id);

            playerList.Add(collision);
            if (!collision.transform.parent.parent.GetComponent<PlayerInvincible>().IsInvincible)
            {
                collision.transform.parent.parent.GetComponent<PlayerController>().KickedAddForce(
                    collision.transform.position - transform.position,
                    playerController.KickDirection);
                battleSumoManager.RequestKickMark(
                    collision.transform.parent.parent.GetComponent<PlayerId>().Id,
                    playerController.transform.GetComponent<PlayerId>().Id);
            }

            if (collision.transform.parent.GetComponent<Rigidbody2D>().velocity.sqrMagnitude >= strongKickWaveThreshold * strongKickWaveThreshold)
            {
                EffectContainer.Instance.PlayEffect("キック(強)", collision.ClosestPoint(transform.position));
                AudioManager.Instance.PlaySe("蹴りを受けた(重)");
            }
            else
            {
                EffectContainer.Instance.PlayEffect("キック(弱)", collision.ClosestPoint(transform.position));
                AudioManager.Instance.PlaySe("蹴りを受けた(軽)");
            }
        }
        else
        {
            playerController.KickPlatformAddForce(collision);
            playerController.EndBlow();

            AudioManager.Instance.PlaySe("ジャンプ");
        }
    }
}

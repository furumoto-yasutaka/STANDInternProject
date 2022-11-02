using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerEffectManager playerEffectManager;
    [SerializeField]
    private PlayerId playerId;

    //private float strongKickWaveThreshold = 15.0f;

    private List<Collider2D> playerList = new List<Collider2D>();
    private BattleSumoManager battleSumoManager;

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
            // R‚Á‚½”½“®‚ğ”½‰f
            playerController.KickPlayerAddForce();
            
            // ©g‚Ìƒ}[ƒN‚ğíœ
            battleSumoManager.RequestDeleteMark(playerId.Id);

            // R‚Á‚½‘Šè‚ğ‹L˜^‚µ‚Ä˜A‘±‚ÅR‚ç‚È‚¢‚æ‚¤‚É‚·‚é
            playerList.Add(collision);

            // –³“Gó‘Ô‚Å‚È‚¢ê‡‚Ì‚İ‘Šè‚ğR‚è”ò‚Î‚·
            if (!collision.transform.parent.parent.GetComponent<PlayerInvincible>().IsInvincible)
            {
                collision.transform.parent.parent.GetComponent<PlayerController>().KickedAddForce(
                    collision.transform.position - transform.position,
                    playerController.KickDirection,
                    collision.ClosestPoint(transform.position));
                battleSumoManager.RequestKickMark(
                    collision.transform.parent.parent.GetComponent<PlayerId>().Id,
                    playerId.Id);
            }
        }
        else
        {
            playerController.KickPlatformAddForce(collision);
            playerController.EndBlow();
        }
    }
}

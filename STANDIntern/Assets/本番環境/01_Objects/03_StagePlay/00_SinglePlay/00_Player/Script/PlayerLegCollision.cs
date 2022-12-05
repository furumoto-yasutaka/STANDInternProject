using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerBattleSumoPoint playerBattleSumoPoint;
    [SerializeField]
    private PlayerEffect playerEffectManager;

    // ÚG‚µ‚½ƒvƒŒƒCƒ„[
    private List<Collider2D> playerList = new List<Collider2D>();

    private void Start()
    {

    }

    private void OnEnable()
    {
        // ©•ª‚Ì‘Ì‚É“–‚½‚ç‚È‚¢‚æ‚¤Õ“Ë‚ğ–³Œø‰»‚µ‚Ä‚¨‚­
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerController.transform.GetChild(0).GetChild(0).GetComponent<Collider2D>());
    }

    private void OnDisable()
    {
        playerList.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerLeg"))
        {//=====“GƒvƒŒƒCƒ„[‚ğR‚Á‚½
            //=====‚Ü‚¾Õ“Ë‚ğŠm”F‚µ‚Ä‚¢‚È‚¢
            if (!playerList.Contains(collision))
            {
                // R‚Á‚½”½“®‚ğ”½‰f
                playerController.KickPlayerAddForce();

                // ©g‚Ìƒ}[ƒN‚ğíœ
                //battleSumoManager.RequestDeleteMark(playerId.Id);
                playerBattleSumoPoint.RequestDeleteMark();

                // R‚Á‚½‘Šè‚ğ‹L˜^‚µ‚Ä˜A‘±‚ÅR‚ç‚È‚¢‚æ‚¤‚É‚·‚é
                playerList.Add(collision);

                // –³“Gó‘Ô‚Å‚È‚¢ê‡‚Ì‚İ‘Šè‚ğR‚è”ò‚Î‚·
                if (!collision.transform.parent.parent.GetComponent<PlayerInvincible>().IsInvincible)
                {
                    collision.transform.parent.parent.GetComponent<PlayerController>().KickedAddForce(
                        collision.transform.position - transform.position,
                        playerController.KickDirection,
                        collision.ClosestPoint(transform.position));
                    playerBattleSumoPoint.RequestKickMark(
                        collision.transform.parent.parent.GetComponent<PlayerBattleSumoPoint>());
                    //battleSumoManager.RequestKickMark(
                    //    collision.transform.parent.parent.GetComponent<PlayerId>().Id,
                    //    playerId.Id);
                }
            }
        }
        else
        {//=====’nŒ`‚ğR‚Á‚½
            playerController.KickPlatformAddForce(collision);
            playerController.EndBlow();
        }
    }
}

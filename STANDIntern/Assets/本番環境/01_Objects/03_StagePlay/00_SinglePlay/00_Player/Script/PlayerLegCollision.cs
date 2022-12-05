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

    // 接触したプレイヤー
    private List<Collider2D> playerList = new List<Collider2D>();

    private void Start()
    {

    }

    private void OnEnable()
    {
        // 自分の体に当たらないよう衝突を無効化しておく
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerController.transform.GetChild(0).GetChild(0).GetComponent<Collider2D>());
    }

    private void OnDisable()
    {
        playerList.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {//=====敵プレイヤーを蹴った
            //=====まだ衝突を確認していない
            if (!playerList.Contains(collision))
            {
                // 蹴った反動を反映
                playerController.KickPlayerAddForce();

                // 自身のマークを削除
                playerBattleSumoPoint.RequestDeleteMark();

                // 蹴った相手を記録して連続で蹴らないようにする
                playerList.Add(collision);

                // 無敵状態でない場合のみ相手を蹴り飛ばす
                if (!collision.transform.parent.parent.GetComponent<PlayerInvincible>().IsInvincible)
                {
                    collision.transform.parent.parent.GetComponent<PlayerController>().KickedAddForce(
                        collision.transform.position - transform.position,
                        playerController.KickDirection,
                        collision.ClosestPoint(transform.position));
                    playerBattleSumoPoint.RequestKickMark(
                        collision.transform.parent.parent.GetComponent<PlayerBattleSumoPoint>());
                }
            }
        }
        else
        {//=====地形を蹴った
            playerController.KickPlatformAddForce(collision);
            playerController.EndBlow();
        }
    }
}

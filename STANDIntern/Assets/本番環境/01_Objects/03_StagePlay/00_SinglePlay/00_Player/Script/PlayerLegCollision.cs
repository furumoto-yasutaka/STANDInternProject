using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private PlayerEffect playerEffectManager;
    [SerializeField]
    private PlayerId playerId;

    // �ڐG�����v���C���[
    private List<Collider2D> playerList = new List<Collider2D>();
    // �Q�[���}�l�[�W���[
    private BattleSumoManager battleSumoManager;

    private void Start()
    {
        int index = (int)BattleSumoModeManagerList.BattleSumoModeManagerId.BattleSumoManager;
        battleSumoManager = GameObject.FindGameObjectWithTag("Managers").transform.GetChild(index).GetComponent<BattleSumoManager>();
    }

    private void OnEnable()
    {
        // �����̑̂ɓ�����Ȃ��悤�Փ˂𖳌������Ă���
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerController.transform.GetChild(0).GetChild(0).GetComponent<Collider2D>());
    }

    private void OnDisable()
    {
        playerList.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerLeg"))
        {//=====�G�v���C���[���R����
            //=====�܂��Փ˂��m�F���Ă��Ȃ�
            if (!playerList.Contains(collision))
            {
                // �R���������𔽉f
                playerController.KickPlayerAddForce();

                // ���g�̃}�[�N���폜
                battleSumoManager.RequestDeleteMark(playerId.Id);

                // �R����������L�^���ĘA���ŏR��Ȃ��悤�ɂ���
                playerList.Add(collision);

                // ���G��ԂłȂ��ꍇ�̂ݑ�����R���΂�
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
        }
        else
        {//=====�n�`���R����
            playerController.KickPlatformAddForce(collision);
            playerController.EndBlow();
        }
    }
}

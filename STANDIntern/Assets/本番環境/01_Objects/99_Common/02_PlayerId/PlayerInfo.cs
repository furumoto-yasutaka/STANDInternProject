using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private int id;
    [SerializeField, ReadOnly]
    private int skinId;
    // スプライト情報
    private PlayerSkinInfo skinInfo;

    public int Id { get { return id; } }
    public int SkinId { get { return skinId; } }
    public PlayerSkinInfo SkinInfo { get { return skinInfo; } }

    public void SetId(int index, PlayerSkinDataBase dataBase)
    {
        id = index;
        skinId = SkinSelectManager.PrevSkinId[id];
        if (skinId != -1)
        {
            skinInfo = dataBase.PlayerSkinInfos[skinId];
        }
        else
        {
            skinInfo = dataBase.PlayerSkinInfos[0];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkinDataBase", menuName = "CreatePlayerSkinDataBase")]
public class PlayerSkinDataBase : ScriptableObject
{
    public enum PlayerColorId
    {
        Red = 0,
        Pink,
        Orange,
        Yellow,
        Green,
        Sky,
        Blue,
        Purple,
        Gray,
    }
    [EnumIndex(typeof(PlayerColorId))]
    public PlayerSkinInfo[] PlayerSkinInfos;
}

[System.Serializable]
public struct PlayerSkinInfo
{
    public Sprite Normal;
    public Sprite Leg;
    public Sprite Kick;
}

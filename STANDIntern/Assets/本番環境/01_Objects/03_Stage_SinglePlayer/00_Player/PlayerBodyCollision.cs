using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlaySe("ƒoƒEƒ“ƒh");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincible : MonoBehaviour
{
    //=====内部から取得
    [SerializeField]
    private Material bodyMaterial;
    [SerializeField]
    private Material legMaterial;

    [SerializeField]
    private float spownInvincibleTime = 2.0f;

    // 無敵かどうか
    private bool isInvincible = false;
    // 残りの無敵時間
    private float invincibleTimeCount = 0.0f;
    // 加算色の強さ
    private float effectColorWeight = 0.0f;

    // 加算色の強さの最大値
    private static readonly float effectWeightMax = 0.5f;
    // 加算色の強さの最小値
    private static readonly float effectWeightMin = 0.1f;
    // 加算色の減衰速度(毎秒)
    private static readonly float effectWeightSubSpeed = 0.6f;

    public bool IsInvincible { get { return isInvincible; } }

    void Awake()
    {
        bodyMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
        legMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }

    void Start()
    {
        bodyMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
        legMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }

    void Update()
    {
        if (isInvincible)
        {
            //=====無敵時間の間点滅処理を繰り返す
            if (invincibleTimeCount <= 0.0f)
            {// 点滅終了
                invincibleTimeCount = 0.0f;
                isInvincible = false;

                bodyMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
                legMaterial.SetColor("_AddColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }
            else
            {// 点滅処理
                invincibleTimeCount -= Time.deltaTime;

                bodyMaterial.SetColor("_AddColor", new Color(effectColorWeight, effectColorWeight, effectColorWeight, 0.0f));
                legMaterial.SetColor("_AddColor", new Color(effectColorWeight, effectColorWeight, effectColorWeight, 0.0f));

                // 色の強さを減衰
                effectColorWeight -= effectWeightSubSpeed * Time.deltaTime;

                // 一定の値まで下がったら最大値に戻す
                if (effectColorWeight <= effectWeightMin)
                {
                    effectColorWeight = effectWeightMax;
                }
            }
        }
    }

    /// <summary>
    /// 無敵状態を開始
    /// </summary>
    public void StartSpownInvincible()
    {
        isInvincible = true;
        invincibleTimeCount = spownInvincibleTime;
        effectColorWeight = effectWeightMax;
    }
}

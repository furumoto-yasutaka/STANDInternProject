using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPostEffect : MonoBehaviour
{
    [SerializeField] private Material postEffectMaterial; // 画面遷移ポストエフェクトのマテリアル
    [SerializeField] private float transitionTime = 2f; // 画面遷移の時間
    private readonly int _progressId = Shader.PropertyToID("_Progress"); // シェーダープロパティのReference名

    /// <summary>
    /// 開始時に実行
    /// </summary>
    void Start()
    {
        if (postEffectMaterial != null)
        {
            StartCoroutine(Transition());
        }
    }

    /// <summary>
    /// 画面遷移
    /// </summary>
    IEnumerator Transition()
    {
        float t = 0f;
        while (t < transitionTime)
        {
            float progress = t / transitionTime;

            // シェーダーの_Progressに値を設定
            postEffectMaterial.SetFloat(_progressId, progress);
            yield return null;

            t += Time.deltaTime;
        }

        postEffectMaterial.SetFloat(_progressId, 1f);
    }
}

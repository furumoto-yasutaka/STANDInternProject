using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPostEffect : MonoBehaviour
{
    [SerializeField] private Material postEffectMaterial; // ��ʑJ�ڃ|�X�g�G�t�F�N�g�̃}�e���A��
    [SerializeField] private float transitionTime = 2f; // ��ʑJ�ڂ̎���
    private readonly int _progressId = Shader.PropertyToID("_Progress"); // �V�F�[�_�[�v���p�e�B��Reference��

    /// <summary>
    /// �J�n���Ɏ��s
    /// </summary>
    void Start()
    {
        if (postEffectMaterial != null)
        {
            StartCoroutine(Transition());
        }
    }

    /// <summary>
    /// ��ʑJ��
    /// </summary>
    IEnumerator Transition()
    {
        float t = 0f;
        while (t < transitionTime)
        {
            float progress = t / transitionTime;

            // �V�F�[�_�[��_Progress�ɒl��ݒ�
            postEffectMaterial.SetFloat(_progressId, progress);
            yield return null;

            t += Time.deltaTime;
        }

        postEffectMaterial.SetFloat(_progressId, 1f);
    }
}

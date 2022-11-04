using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleCountDown : MonoBehaviour
{
    private TextMeshProUGUI second; // �b����\���e�L�X�g
    private Animator animator;      // �e�L�X�g�̃A�j���[�^�[
    private int count = 5;          // �J�E���g�_�E���̕b��

    public int Count { get { return count; } }

    void Start()
    {
        second = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        second.text = count.ToString();
    }

    /// <summary>
    /// �e�L�X�g�����̐����ɕς���
    /// </summary>
    public void NextNumber()
    {
        count--;
        second.text = count.ToString();
    }

    /// <summary>
    /// �A�j���[�V�����̃��[�v�����Ŏ~�߂邩�ǂ������f����
    /// </summary>
    public void CheckStop()
    {
        // �J�E���g���P(���̃J�E���g�ŏI��)�̏ꍇ�t���O�𗧂Ă�
        if (count == 1)
        {
            animator.SetBool("Stop", true);
        }
    }
}

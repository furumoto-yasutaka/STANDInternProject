using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonCommentManager : MonoBehaviour
{
    [SerializeField]
    private ButtonSelectManager buttonParent;
    private TextMeshProUGUI textMeshPro;

    void Awake()
    {
        textMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        ButtonComment commnet = buttonParent.transform.GetChild(buttonParent.SelectIndex).GetComponent<ButtonComment>();
        textMeshPro.text = commnet.CommnetText;
    }

    public void ChangeText()
    {
        ButtonComment commnet = buttonParent.transform.GetChild(buttonParent.SelectIndex).GetComponent<ButtonComment>();
        textMeshPro.text = commnet.CommnetText;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonCommentManager : MonoBehaviour
{
    [SerializeField]
    private ButtonSelectManager buttonParent;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    void Awake()
    {
        ButtonComment commnet = buttonParent.transform.GetChild(buttonParent.SelectCursorIndex).GetComponent<ButtonComment>();
        textMeshPro.text = commnet.CommnetText;
    }

    public void ChangeText()
    {
        ButtonComment commnet = buttonParent.transform.GetChild(buttonParent.SelectCursorIndex).GetComponent<ButtonComment>();
        textMeshPro.text = commnet.CommnetText;
    }
}

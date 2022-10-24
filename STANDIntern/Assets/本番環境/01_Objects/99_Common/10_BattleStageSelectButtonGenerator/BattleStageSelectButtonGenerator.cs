using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStageSelectButtonGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform buttonParent;
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private BattleStageSelectButtonDataBase database;

    void Start()
    {
        Destroy(gameObject);
    }

    public void Generate()
    {
        Clear();

        for (int i = 0; i < database.ButtonInfo.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonParent);
            button.transform.localPosition = (Vector3)database.ButtonSpace * i;
            button.GetComponent<ButtonComment>().CommnetText = database.ButtonInfo[i].Comment;
            button.transform.GetChild(0).GetComponent<Image>().sprite = database.ButtonInfo[i].ButtonBackSprite;
            button.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = database.StageTextInitial + (i + 1);
        }
    }

    public void Clear()
    {
        for (int i = buttonParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(buttonParent.GetChild(i).gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectCourceGenerator : MonoBehaviour
{
    // ステージ情報
    [SerializeField]
    private StageDataBase stageDataBase;
    [SerializeField]
    private RectTransform pointsParent;
    [SerializeField]
    private RectTransform linesParent;
    [SerializeField]
    private GameObject pointPrefab;
    [SerializeField]
    private GameObject linePrefab;

    private Vector3 prevPos = Vector3.zero;

    void Start()
    {
        Destroy(gameObject);
    }

    public void GenerateCource()
    {
        ClearCource();

        for (int i = 0; i < stageDataBase.StageInfos.Length; i++)
        {
            GameObject point = Instantiate(pointPrefab, pointsParent);
            point.GetComponent<RectTransform>().localPosition = prevPos + (Vector3)stageDataBase.StageInfos[i].StageSelectDrawPos;
            prevPos += (Vector3)stageDataBase.StageInfos[i].StageSelectDrawPos;
        }
        for (int i = 0; i < stageDataBase.StageInfos.Length - 1; i++)
        {
            GameObject line = Instantiate(linePrefab, linesParent);
            Vector2 startPos = pointsParent.GetChild(i).GetComponent<RectTransform>().localPosition;
            Vector2 endPos = pointsParent.GetChild(i + 1).GetComponent<RectTransform>().localPosition;
            RectTransform trans = line.GetComponent<RectTransform>();

            trans.localPosition = startPos;
            Vector2 distance = endPos - startPos;
            float angle = Vector2.SignedAngle(Vector2.right, distance.normalized);
            trans.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            trans.sizeDelta = new Vector2(distance.magnitude, trans.sizeDelta.y);
        }

        prevPos = Vector3.zero;
    }

    public void ClearCource()
    {
        foreach (Transform child in pointsParent)
        {
            DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in linesParent)
        {
            DestroyImmediate(child.gameObject);
        }

        for (int i = pointsParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(pointsParent.GetChild(i).gameObject);
        }
        for (int i = linesParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(linesParent.GetChild(i).gameObject);
        }
    }
}

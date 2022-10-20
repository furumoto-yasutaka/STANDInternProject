using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundAutoScroll : MonoBehaviour
{
    [SerializeField]
    private int backGroundNum;
    private Material[] materialList;
    [SerializeField]
    private Vector2[] moveSpeed;

    void Start()
    {
        materialList = new Material[backGroundNum];

        for (int i = 0; i < backGroundNum; i++)
        {
            materialList[i] = transform.GetChild(i).GetComponent<Image>().material;
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < backGroundNum; i++)
        {
            Vector2 newTiling = materialList[i].GetTextureOffset("_MainTex") + moveSpeed[i] * Time.deltaTime;
            materialList[i].SetTextureOffset("_MainTex", newTiling);
        }
    }
}

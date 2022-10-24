using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundAutoScroll : MonoBehaviour
{
    private Material[] materialList;
    [SerializeField]
    private Vector2[] moveSpeed;

    void Start()
    {
        materialList = new Material[moveSpeed.Length];

        for (int i = 0; i < moveSpeed.Length; i++)
        {
            materialList[i] = transform.GetChild(i).GetComponent<Image>().material;
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < moveSpeed.Length; i++)
        {
            Vector2 newTiling = materialList[i].GetTextureOffset("_MainTex") + moveSpeed[i] * Time.deltaTime;
            materialList[i].SetTextureOffset("_MainTex", newTiling);
        }
    }
}

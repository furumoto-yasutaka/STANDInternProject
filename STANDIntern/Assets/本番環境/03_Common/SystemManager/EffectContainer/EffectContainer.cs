using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectContainer : SingletonMonoBehaviour<EffectContainer>
{
    public EffectData[] EffectDataList;

    private Dictionary<string, GameObject> EffectPrefab = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < EffectDataList.Length; i++)
        {
            EffectPrefab.Add(EffectDataList[i].Name, EffectDataList[i].Prefab);
        }
    }

    /// <summary> エフェクトを再生 </summary>
    /// <param name="Name"> エフェクト名 </param>
    /// <param name="Pos"> エフェクトの生成座標 </param>
    /// <param name="Rot"> エフェクトの回転 </param>
    public void PlayEffect(string Name, Vector3 Pos, Quaternion Rot)
    {
        GameObject effectObj = Instantiate(EffectPrefab[Name], Pos, Rot);
        effectObj.GetComponent<ParticleSystem>().Play(true);
    }

    /// <summary> エフェクトを再生 </summary>
    /// <param name="Name"> エフェクト名 </param>
    /// <param name="Pos"> エフェクトの生成座標 </param>
    /// <param name="Rot"> エフェクトの回転 </param>
    /// <param name="Parent"> エフェクトの親に指定するオブジェクト </param>
    public void PlayEffect(string Name, Vector3 Pos, Quaternion Rot, Transform Parent)
    {
        GameObject effectObj = Instantiate(EffectPrefab[Name], Pos, Rot);
        effectObj.GetComponent<ParticleSystem>().Play(true);
        effectObj.transform.parent = Parent;
    }

    /// <summary> エフェクトを再生 </summary>
    /// <param name="Name"> エフェクト名 </param>
    /// <param name="Pos"> エフェクトの生成座標 </param>
    /// <param name="Rot"> エフェクトの回転 </param>
    /// <param name="Parent"> エフェクトの親に指定するオブジェクト </param>
    /// <param name="EffectObject"> 生成したエフェクトを受け取るオブジェクト </param>
    public void PlayEffect(string Name, Vector3 Pos, Quaternion Rot, Transform Parent, ref GameObject EffectObject)
    {
        EffectObject = Instantiate(EffectPrefab[Name], Pos, Rot);
        EffectObject.GetComponent<ParticleSystem>().Play(true);
        EffectObject.transform.parent = Parent;
    }
}

[System.Serializable]
public struct EffectData
{
    public string Name;
    public GameObject Prefab;
}

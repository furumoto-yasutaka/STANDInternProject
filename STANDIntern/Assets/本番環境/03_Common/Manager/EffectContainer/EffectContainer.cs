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

    /// <summary> �G�t�F�N�g���Đ� </summary>
    /// <param name="Name"> �G�t�F�N�g�� </param>
    /// <param name="Pos"> �G�t�F�N�g�̐������W </param>
    /// <param name="Rot"> �G�t�F�N�g�̉�] </param>
    public void PlayEffect(string Name, Vector3 Pos, Quaternion Rot)
    {
        GameObject effectObj = Instantiate(EffectPrefab[Name], Pos, Rot);
        effectObj.GetComponent<ParticleSystem>().Play(true);
    }

    /// <summary> �G�t�F�N�g���Đ� </summary>
    /// <param name="Name"> �G�t�F�N�g�� </param>
    /// <param name="Pos"> �G�t�F�N�g�̐������W </param>
    /// <param name="Rot"> �G�t�F�N�g�̉�] </param>
    /// <param name="Parent"> �G�t�F�N�g�̐e�Ɏw�肷��I�u�W�F�N�g </param>
    public void PlayEffect(string Name, Vector3 Pos, Quaternion Rot, Transform Parent)
    {
        GameObject effectObj = Instantiate(EffectPrefab[Name], Pos, Rot);
        effectObj.GetComponent<ParticleSystem>().Play(true);
        effectObj.transform.parent = Parent;
    }

    /// <summary> �G�t�F�N�g���Đ� </summary>
    /// <param name="Name"> �G�t�F�N�g�� </param>
    /// <param name="Pos"> �G�t�F�N�g�̐������W </param>
    /// <param name="Rot"> �G�t�F�N�g�̉�] </param>
    /// <param name="Parent"> �G�t�F�N�g�̐e�Ɏw�肷��I�u�W�F�N�g </param>
    /// <param name="EffectObject"> ���������G�t�F�N�g���󂯎��I�u�W�F�N�g </param>
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

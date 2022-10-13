using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLogoManager : MonoBehaviour
{
    [SerializeField, RenameField("���̃V�[���Ɉڂ�܂ł̎���")]
    private float transitionLimit = 5.0f;
    [SerializeField, RenameField("���S��\������܂ł̎���")]
    private float showTeamLogoTiming = 1.0f;
    [SerializeField, RenameField("���S�I�u�W�F�N�g")]
    private GameObject teamlogoObj;

    void Update()
    {
        // �`�[�����S�֘A
        if (showTeamLogoTiming > 0.0f)
        {
            showTeamLogoTiming -= Time.deltaTime;

            if (showTeamLogoTiming <= 0.0f)
            {
                teamlogoObj.SetActive(true);
            }
        }

        // �V�[���J�ڊ֘A
        if (transitionLimit > 0.0f)
        {
            transitionLimit -= Time.deltaTime;

            if (transitionLimit <= 0.0f)
            {
                GetComponent<SceneChange>().StartSceneChange(SceneNameEnum.TitleScene);
            }
        }
    }
}

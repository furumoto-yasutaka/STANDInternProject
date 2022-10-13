using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLogoManager : MonoBehaviour
{
    [SerializeField, RenameField("次のシーンに移るまでの時間")]
    private float transitionLimit = 5.0f;
    [SerializeField, RenameField("ロゴを表示するまでの時間")]
    private float showTeamLogoTiming = 1.0f;
    [SerializeField, RenameField("ロゴオブジェクト")]
    private GameObject teamlogoObj;

    void Update()
    {
        // チームロゴ関連
        if (showTeamLogoTiming > 0.0f)
        {
            showTeamLogoTiming -= Time.deltaTime;

            if (showTeamLogoTiming <= 0.0f)
            {
                teamlogoObj.SetActive(true);
            }
        }

        // シーン遷移関連
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

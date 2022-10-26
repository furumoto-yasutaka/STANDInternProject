using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanNotPressIconAlphaSetter : MonoBehaviour
{
    [SerializeField]
    private NormalWindow parentWindow;
    private CanvasGroup parentCanvasGroup;
    private CanvasGroup myCanvasGroup;

    void Start()
    {
        parentCanvasGroup = parentWindow.GetComponent<CanvasGroup>();
        myCanvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        myCanvasGroup.alpha = parentCanvasGroup.alpha;
    }
}

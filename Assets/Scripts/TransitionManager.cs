using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [Header("�t�F�C�h����")]
    public float duration;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Panel�̓����x�𑀍삵�ăQ�[����ʂ̃t�F�C�h�������s��
    /// </summary>
    /// <param name="alpha">Panel�̓����x</param>
    /// <returns></returns>
    public IEnumerator FadePanel(float alpha)
    {
        canvasGroup.DOFade(alpha, duration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(duration);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeMgr : SingletonBehaviour<FadeMgr> {

    [SerializeField] private CanvasGroup m_fadeGroup;
    public Text countText;
    public GameObject animeObj;
    private int m_count;

    /*-----------------------------------------------------/
    |               ここから適当にFade処理                 |
    /-----------------------------------------------------*/

    public void FadeIn(float duration, Action action = null)
    {
        m_fadeGroup.DOFade(0, duration).OnComplete(()=> {
            m_fadeGroup.blocksRaycasts = false;
            if (action != null) {
                action();
            }
        });
    }

    public void FadeIn(CanvasGroup group, float duration = 1.0f, Action action = null)
    {
        group.DOFade(0, duration).OnComplete(() => {
            group.blocksRaycasts = false;
            if (action != null)
            {
                action();
            }
        });
    }

    public void FadeOut(float duration, Action action = null)
    {
        m_fadeGroup.DOFade(1, duration).OnComplete(() => {
            m_fadeGroup.blocksRaycasts = true;
            if (action != null) {
                action();
            }
        });
    }

    public void FadeOut(CanvasGroup group, float duration = 1.0f, Action action = null)
    {
        group.DOFade(1, duration).OnComplete(() => {
            group.blocksRaycasts = true;
            if (action != null)
            {
                action();
            }
        });
    }

    // 読み込みパーセンテージプロパティ
    public int Counter {
        get { return m_count; }
        private set {
            m_count = value;
            if(m_count == 100)
            {
                countText.text = "Load... Complete!!";
                return;
            }
            countText.text = string.Format("Load... {0}%", m_count.ToString());
        }
    }

    // 読み込みパーセンテージSet用関数
    public void SetCounter(float value)
    {
        if (value == 1)
        {
            DOTween.To(() => Counter, x => Counter = x, 100, 0.5f);
            return;
        }
        Counter = (int)(value * 100);
    }
}

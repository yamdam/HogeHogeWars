using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Title,
}

// 最低限の機能しかないSceneManagerクラス
// 適時機能を追加して下さいby難波
public class SceneMgr : SingletonBehaviour<SceneMgr> {

    private SceneType m_currentScene = SceneType.Title;
    private bool m_isFade = false;
    private float m_duration = 0.5f;
    private AsyncOperation m_async;

    /*-----------------------------------------------------/
    |               ここから適当にScene処理                |
    /-----------------------------------------------------*/


    // 通常の暗転するFade
    public void SceneTransitionNonFade(SceneType name)
    {
        StartCoroutine(transition(name, 0));
    }
    public void SceneTransition(SceneType name)
    {
        m_isFade = true;
        FadeMgr.Instance.FadeOut(m_duration, () => { m_isFade = false; });
        StartCoroutine(transition(name, m_duration));
    }
    public void SceneTransition(SceneType name, float duration)
    {
        m_isFade = true;
        FadeMgr.Instance.FadeOut(duration, ()=> { m_isFade = false; });
        StartCoroutine(transition(name, duration));
    }

    IEnumerator transition(SceneType name, float duration)
    {
        yield return new WaitWhile(() => m_isFade);

        yield return SceneManager.LoadSceneAsync(name.ToString(), LoadSceneMode.Additive);

        UnLoadScene(m_currentScene);
        m_currentScene = name;

        if(duration != 0) {
            FadeMgr.Instance.FadeIn(duration, () =>
            {
                MyDebug.Log(name.ToString() + "_Scene : LoadComplete!!");
            });
        }
        else {
            MyDebug.Log(name.ToString() + "_Scene : LoadComplete!!");
        }
    }

    // Loadアニメーション付きFade
    public void SceneTransitionAnim(SceneType name, float duration)
    {
        m_isFade = true;
        FadeMgr.Instance.FadeOut(duration, () => { m_isFade = false; });
        StartCoroutine(transitionAsync(name, duration));
    }
    IEnumerator transitionAsync(SceneType name, float duration)
    {
        FadeMgr.Instance.countText.enabled = true;
        FadeMgr.Instance.animeObj.SetActive(true);

        yield return new WaitWhile(() => m_isFade);

        m_async = SceneManager.LoadSceneAsync(name.ToString(), LoadSceneMode.Additive);
        // 読み込みが終わっても表示しない
        m_async.allowSceneActivation = false;

        // 読み込み終わるまで待機(0.9までしか増えない)
        while (m_async.progress < 0.9f)
        {
            FadeMgr.Instance.SetCounter(m_async.progress);
            yield return 0;
        }

        // 強制的に100％に
        FadeMgr.Instance.SetCounter(1);

        // 90%->100%のTwennが終わるまで待機
        yield return new WaitWhile(() => FadeMgr.Instance.Counter != 100);

        // Load完了から少し待つ
        yield return new WaitForSeconds(1);

        // 読み込みが完了しているSceneを表示
        m_async.allowSceneActivation = true;

        yield return null;

        UnLoadScene(m_currentScene);
        m_currentScene = name;

        if (duration != 0)
        {
            FadeMgr.Instance.FadeIn(duration, () =>
            {
                FadeMgr.Instance.countText.enabled = false;
                FadeMgr.Instance.animeObj.SetActive(false);
                FadeMgr.Instance.SetCounter(0);
                MyDebug.Log(name.ToString() + "_Scene : LoadComplete!!");
            });
        }
        else {
            FadeMgr.Instance.countText.enabled = false;
            FadeMgr.Instance.animeObj.SetActive(false);
            FadeMgr.Instance.SetCounter(0);
            MyDebug.Log(name.ToString() + "_Scene : LoadComplete!!");
        }
    }

    // 現在読み込んでいるシーンType
    public SceneType GetCurrentSceneType()
    {
        return m_currentScene;
    }

    // シーンをアンロード
    public void UnLoadScene(SceneType name)
    {
        MyDebug.Log(name.ToString() + "_Scene : UnLoad!!");
        SceneManager.UnloadSceneAsync(name.ToString());
    }
}

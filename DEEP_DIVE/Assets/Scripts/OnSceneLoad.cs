using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// When the scene is played, run some specific functionality
/// </summary>
public class OnSceneLoad : MonoBehaviour
{
    // When scene is loaded and play begins
    public UnityEvent OnLoad = new UnityEvent();
    public UnityEvent OnExit = new UnityEvent();

    private void Awake()
    {
        SceneManager.sceneLoaded += PlayEvent;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= PlayEvent;
    }

    private void PlayEvent(Scene scene, LoadSceneMode mode)
    {
        OnLoad.Invoke();
    }

    IEnumerator Fade()
    {
        OnExit.Invoke();
        yield return new WaitForSeconds(5);

    }

    public void QuitApplication()
    {
        StartCoroutine(Fade());
#if UnityEditor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
        #endif
    }

    
}

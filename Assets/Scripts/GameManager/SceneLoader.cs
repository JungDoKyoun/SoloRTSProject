using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    [SerializeField] private GameObject _loadingScene;
    [SerializeField] private Image _loadingBar;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static SceneLoader Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<SceneLoader>();
            }
            return _instance;
        }
    }

    public void LoadSceneInstant(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log("¤±¤±");
        _loadingScene.SetActive(true);
        DontDestroyOnLoad(_loadingScene);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float minTime = 3f;
        float elapseTime = 0f;
        _loadingBar.fillAmount = 0;

        while(!asyncLoad.isDone)
        {
            float progres = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            elapseTime += Time.deltaTime;
            _loadingBar.fillAmount = progres;

            if(asyncLoad.progress >= 0.9f && elapseTime >= minTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        _loadingScene.SetActive(false);
    }
}

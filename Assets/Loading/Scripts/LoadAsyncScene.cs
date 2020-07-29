using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadAsyncScene : MonoBehaviour {

    //显示进度的文本
    public Text progress;
    //进度条的数值
    private float progressValue;
    //进度条
    public Slider slider;
    [Tooltip("下个场景的名字")]
    public string nextSceneName;

    private AsyncOperation async = null;

    private void Start() {

        //progress = GetComponent<Text>();

        //slider = FindObjectOfType<Slider>();

        StartCoroutine("LoadScene");
    }

    IEnumerator LoadScene() {

        async = SceneManager.LoadSceneAsync(nextSceneName);

        async.allowSceneActivation = true;

        while(!async.isDone) {

            if (async.progress < 0.9f)

                progressValue = async.progress;

            else

                progressValue = 1.0f;

            slider.value = progressValue;

            progress.text = (int)(slider.value * 100) + " %";

            if (progressValue >= 0.9) {

                if (Input.anyKeyDown) {

                    async.allowSceneActivation = true;

                }

            }

            yield return null;
        }

    }

}
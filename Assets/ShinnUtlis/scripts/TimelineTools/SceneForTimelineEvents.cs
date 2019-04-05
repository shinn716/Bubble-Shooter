using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneForTimelineEvents : MonoBehaviour
{
    public int[] LoadLevel;
    AsyncOperation[] asyncOperation;

    private void Start()
    {
        asyncOperation = new AsyncOperation[LoadLevel.Length];
        for (int i=0; i<LoadLevel.Length; i++)
            StartCoroutine(LoadScene(i, LoadLevel[i]));
    }

    public void nextScene(int index)
    {
        asyncOperation[index].allowSceneActivation = true;
    }


    IEnumerator LoadScene(int index, int level)
    {
        yield return null;
        asyncOperation[index] = SceneManager.LoadSceneAsync(level);
        asyncOperation[index].allowSceneActivation = false;
    }
}

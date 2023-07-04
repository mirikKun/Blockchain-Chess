using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string[] sceneNames;
    [SerializeField] private GameObject exitButton;
    private int _sceneIndex;

    private void Start()
    {
        if (exitButton)
        {
#if UNITY_ANDROID
      exitButton.SetActive(false);
#endif
        }
    }

    public void ChooseScene(int sceneIndex)
    {
        _sceneIndex = sceneIndex;
    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log("Change");

        SceneManager.LoadScene(sceneName);
    }

    public void ChangeSceneWithId(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ChangeSceneWithNameNumber()
    {
        SceneManager.LoadScene(sceneNames[_sceneIndex]);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
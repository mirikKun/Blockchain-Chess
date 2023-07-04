using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnvironmentChooser : MonoBehaviour
{
    [SerializeField] private ClanButton[] environments;
    [SerializeField] private Sprite redTitle;
    [SerializeField] private Sprite blackTitle;
    [FormerlySerializedAs("_sceneChanger")] [SerializeField] private SceneChanger sceneChanger;
    private string _sceneName;
    private ClanButton _lastClanButton;
    [Serializable]
    public class ClanButton
    {
        public string SceneName;
        public Button ChooseEnvironment;
        public Image RedTitle;
        public GameObject RedOutline;
    }

    private void Start()
    {
        ChooseScene(environments[0]);
        foreach (var environment in environments)
        {
            environment.ChooseEnvironment.onClick.AddListener(delegate
            {
                ChooseScene(environment);
            });
        }
    }

    public void ChooseScene(ClanButton clanButton)
    {
        if (_lastClanButton!=null)
        {
            UnselectLastScene(_lastClanButton);
        }

        _sceneName = clanButton.SceneName;
        clanButton.RedOutline.SetActive(true);
        _lastClanButton = clanButton;
        clanButton.RedTitle.sprite = redTitle;
    }

    private void UnselectLastScene(ClanButton clanButton)
    {
        clanButton.RedOutline.SetActive(false);
        clanButton.RedTitle.sprite = blackTitle;
        
    }

    public void ConfirmScene()
    {
        sceneChanger.ChangeScene(_sceneName);
    }
}

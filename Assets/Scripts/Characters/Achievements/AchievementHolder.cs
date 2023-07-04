using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI mainDescription;
    [SerializeField] private Image background;
    [SerializeField] private SubAchievement[] subAchievements;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Sprite redBack;
    [SerializeField] private Sprite blackBack;
    [SerializeField] private Sprite completedBack;
    [SerializeField] private GameObject completedIcon;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progress;
    [SerializeField] private GameObject completed;
    [SerializeField] private float mainPartHeight;
    [SerializeField] private float subPartHeight;

    private bool _opened;
    private AchievementDetails _achievementDetails;
    private CharacterPanel _characterPanel;


    private void Start()
    {
        _characterPanel = GetComponentInParent<CharacterPanel>();
        button.onClick.AddListener(AchievementListStateSwitch);
        if(_characterPanel.inMainMenu)
        {
            for (int index = 0; index < subAchievements.Length; index++)
            {
                var subAchievement = subAchievements[index];
                var index1 = index;
                subAchievement.button.onClick.AddListener(delegate { ClaimAchievement(subAchievement, index1); });
            }
        }
    }

    public void SetupAchievement(AchievementDetails achievementDetails, Transform inProgressHolder = null,
        Transform completedHolder = null)
    {
        // button.onClick.RemoveAllListeners();
        //button.onClick.AddListener(AchievementListStateSwitch);

        
        bool allClaimed = true;
        _achievementDetails = achievementDetails;
        title.text = achievementDetails.achievementName;
        mainDescription.text = achievementDetails.achievementSteps[0].description;
        completedIcon.SetActive(false);
        for (int i = 0; i < achievementDetails.achievementSteps.Length; i++)
        {
            AchievementDetails.AchievementStep curStep = achievementDetails.achievementSteps[i];
            int leftEdge = 0;

            if (i > 0)
            {
                leftEdge = achievementDetails.achievementSteps[i - 1].goal;
            }


            bool completed = CheckAchievementPassed(curStep.goal, achievementDetails.GetCurValue());

            if (i == 0)
            {
                background.sprite = completed ? redBack : blackBack;
            }

            bool inCurProgress = CheckAchievementInProgress(leftEdge, curStep.goal,
                achievementDetails.GetCurValue());
            if (inCurProgress)
            {
                progress.text = achievementDetails.GetCurValue() + "/" + curStep.goal;
                progressBar.value = (float)(achievementDetails.GetCurValue() - leftEdge) / (curStep.goal - leftEdge);
            }

            subAchievements[i].description.text = curStep.description;
            subAchievements[i].active = true;
            if (completed)
            {
                if (achievementDetails.achievementSteps[i].claimed)
                {
                    subAchievements[i].button.enabled = false;
                    subAchievements[i].background.sprite = completedBack;
                    subAchievements[i].completed.SetActive(true);
                }
                else
                {
                    subAchievements[i].button.enabled = true;
                    subAchievements[i].background.sprite = redBack;
                    subAchievements[i].completed.SetActive(false);
                    allClaimed = false;
                }
            }
            else
            {
                subAchievements[i].button.enabled = false;
                subAchievements[i].background.sprite = blackBack;
                subAchievements[i].completed.SetActive(false);

            }

            if (completed && i == achievementDetails.achievementSteps.Length - 1)
            {
                progress.text = curStep.goal + "/" + curStep.goal;
                progressBar.value = 1;


                background.sprite = completedBack;
                if (achievementDetails.achievementSteps[i].claimed && allClaimed)
                {
                    if (completedHolder)
                    {
                        transform.parent = completedHolder;
                    }

                    completedIcon.SetActive(true);
                    button.enabled = false;
                    CloseAchievement();
                }
                else
                {
                    button.enabled = true;
                }
            }
            else
            {
                if (inProgressHolder)
                {
                    transform.parent = inProgressHolder;
                }

                completedIcon.SetActive(false);
                button.enabled = true;
            }
        }
    }


    private void ClaimAchievement(SubAchievement subAchievement, int index)
    {
        subAchievement.completed.SetActive(true);
        subAchievement.background.sprite = completedBack;
        subAchievement.button.enabled = false;
        _achievementDetails.achievementSteps[index].claimed = true;
        _characterPanel.ClaimAchievement(_achievementDetails.roleOfKilled,
            _achievementDetails.classOfKilled, index, _achievementDetails.achievementSteps[index].expReward);
        bool allClaimed = true;
        foreach (var detailsStep in _achievementDetails.achievementSteps)
        {
            if (!detailsStep.claimed)
            {
                allClaimed = false;
            }
        }

        if (allClaimed)
        {
            completed.SetActive(true);
        }
    }

    private void ClaimAchievement(int index)
    {
        completedIcon.SetActive(true);
        button.enabled = false;
        _characterPanel.ClaimAchievement(_achievementDetails.roleOfKilled,
            _achievementDetails.classOfKilled, index, _achievementDetails.achievementSteps[index].expReward);
    }

    private bool CheckAchievementPassed(int rightEdge, int curValue)
    {
        return (curValue >= rightEdge);
    }

    private bool CheckAchievementInProgress(int leftEdge, int rightEdge, int curValue)
    {
        return (curValue >= leftEdge && curValue < rightEdge);
    }

    public void AchievementListStateSwitch()
    {
        if (_opened)
        {
            CloseAchievement();
        }
        else
        {
            OpenAchievement();
        }

        _opened = !_opened;
    }

    public void OpenAchievement()
    {
        int index = 0;
        foreach (var part in subAchievements)
        {
            if (part.active)
            {
                part.panel.SetActive(true);
            }
            else
            {
                rectTransform.sizeDelta =
                    new Vector2(rectTransform.sizeDelta.x, mainPartHeight + subPartHeight * index);
                break;
            }

            index++;
        }

        rectTransform.sizeDelta =
            new Vector2(rectTransform.sizeDelta.x, mainPartHeight + subPartHeight * index);
    }

    public void CloseAchievement()
    {
        foreach (var part in subAchievements)
        {
            part.panel.SetActive(false);
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, mainPartHeight);
    }
}
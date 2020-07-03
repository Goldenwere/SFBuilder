using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void GoalDelegate(int newGoal);

public class ProtoGoalSystem : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GoalContainer[]    goals;
    [SerializeField] private GameObject         templateExtraButton;
    [SerializeField] private GameObject         templateRequirementButton;
    [SerializeField] private Button             uiButtonNextGoal;
    [SerializeField] private int                uiButtonPadding;
    [SerializeField] private GameObject         uiButtonPanel;
#pragma warning restore 0649
    /**************/ private bool               canMoveOn;

    public int                      CurrentGoal { get; private set; }
    public GoalContainer            CurrentGoalWorkingSet { get; private set; }
    public GoalContainer[]          Goals { get { return goals; } }
    public static ProtoGoalSystem   Instance { get; private set; }

    public static GoalDelegate newGoal;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        CurrentGoalWorkingSet = goals[CurrentGoal];
        SetupUI();
    }

    public void OnNextGoalButtonPressed()
    {
        if (canMoveOn)
        {
            CurrentGoal++;
            if (CurrentGoal < goals.Length)
            {
                CurrentGoalWorkingSet = goals[CurrentGoal];
                newGoal?.Invoke(CurrentGoal);
            }
            else
            {
                ProtoLevelSystem.Instance.CurrentLevel++;
                // will be made redundant as there will be a scene change 
                CurrentGoal = 0;
            }
            SetupUI();
        }
    }

    public void VerifyForNextGoal()
    {
        bool test = true;
        foreach (Goal g in CurrentGoalWorkingSet.goalRequirements)
            if (g.goalStructureCount > 0)
                test = false;
        canMoveOn = test && GameScoring.Instance.ScoreViability >= CurrentGoalWorkingSet.goalViability &&
            GameScoring.Instance.ScoreHappiness > 0 && GameScoring.Instance.ScorePower > 0 && GameScoring.Instance.ScoreSustenance > 0;
        uiButtonNextGoal.interactable = canMoveOn;
    }

    private void SetupUI()
    {
        uiButtonNextGoal.interactable = false;
        int buttonCount = 0;
        for (int i = 0, count = uiButtonPanel.transform.childCount; i < count; i++)
            Destroy(uiButtonPanel.transform.GetChild(i).gameObject);

        foreach(Goal g in CurrentGoalWorkingSet.goalRequirements)
        {
            RectTransform rt = Instantiate(templateRequirementButton, uiButtonPanel.transform, false).GetComponent<RectTransform>();
            Vector3 pos = rt.anchoredPosition;
            pos.x = (rt.rect.width / 2) + (buttonCount * rt.rect.width) + (uiButtonPadding * (buttonCount + 1));
            rt.anchoredPosition = pos;
            rt.GetComponent<ProtoButton>().Initialize(g.goalStructureID, g.goalStructureCount, true);
            buttonCount++;
        }

        foreach (Goal g in CurrentGoalWorkingSet.goalExtras)
        {
            RectTransform rt = Instantiate(templateExtraButton, uiButtonPanel.transform, false).GetComponent<RectTransform>();
            Vector3 pos = rt.anchoredPosition;
            pos.x = (rt.rect.width / 2) + (buttonCount * rt.rect.width) + (uiButtonPadding * (buttonCount + 1));
            rt.anchoredPosition = pos;
            rt.GetComponent<ProtoButton>().Initialize(g.goalStructureID, g.goalStructureCount, false);
            buttonCount++;
        }
    }
}

[Serializable]
public struct GoalContainer
{
    public Goal[]   goalExtras;
    public Goal[]   goalRequirements;
    public int      goalViability;
}

[Serializable]
public struct Goal
{
    public int  goalStructureCount;
    public int  goalStructureID;
}
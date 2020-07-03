﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoGoalSystem : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GoalContainer[]    goals;
    [SerializeField] private GameObject         templateExtraButton;
    [SerializeField] private GameObject         templateRequirementButton;
    [SerializeField] private int                uiButtonPadding;
    [SerializeField] private GameObject         uiButtonPanel;
#pragma warning restore 0649

    public int              CurrentGoal { get; private set; }
    public GoalContainer    CurrentGoalWorkingSet { get; private set; }
    public GoalContainer[]  Goals { get { return goals; } }
    public ProtoGoalSystem  Instance { get; private set; }

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

    public void VerifyForNextGoal()
    {
        bool canMoveOn = true;
        foreach (Goal g in CurrentGoalWorkingSet.goalRequirements)
            if (g.goalStructureCount > 0)
                canMoveOn = false;
        if (canMoveOn)
        {
            CurrentGoal++;
            if (CurrentGoal < goals.Length)
                CurrentGoalWorkingSet = goals[CurrentGoal];
            SetupUI();
        }
    }

    private void SetupUI()
    {
        int buttonCount = 0;
        for (int i = 0, count = uiButtonPanel.transform.childCount; i < count; i++)
            Destroy(uiButtonPanel.transform.GetChild(i).gameObject);

        foreach(Goal g in CurrentGoalWorkingSet.goalRequirements)
        {
            RectTransform rt = Instantiate(templateRequirementButton, uiButtonPanel.transform, false).GetComponent<RectTransform>();
            Vector3 pos = rt.anchoredPosition;
            pos.x = (rt.rect.width / 2) + (buttonCount * rt.rect.width) + (uiButtonPadding * (buttonCount + 1));
            rt.anchoredPosition = pos;
            rt.GetComponent<ProtoButton>().Initialize(g.goalStructureID, g.goalStructureCount);
            buttonCount++;
        }

        foreach (Goal g in CurrentGoalWorkingSet.goalExtras)
        {
            RectTransform rt = Instantiate(templateExtraButton, uiButtonPanel.transform, false).GetComponent<RectTransform>();
            Vector3 pos = rt.anchoredPosition;
            pos.x = (rt.rect.width / 2) + (buttonCount * rt.rect.width) + (uiButtonPadding * (buttonCount + 1));
            rt.anchoredPosition = pos;
            rt.GetComponent<ProtoButton>().Initialize(g.goalStructureID, g.goalStructureCount);
            buttonCount++;
        }
    }
}

[Serializable]
public struct GoalContainer
{
    public Goal[]   goalExtras;
    public Goal[]   goalRequirements;
}

[Serializable]
public struct Goal
{
    public int  goalStructureCount;
    public int  goalStructureID;
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoGoalSystem : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GoalContainer[]    goals;
    [SerializeField] private GameObject         templateExtraButton;
    [SerializeField] private GameObject         templateRequirementButton;
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
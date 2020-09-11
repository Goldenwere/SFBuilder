﻿using UnityEngine;
using UnityEngine.UI;
using SFBuilder.Gameplay;

namespace SFBuilder.UI
{
    /// <summary>
    /// Displays value for a stat in a radial element
    /// </summary>
    /// <remarks>Currently, only viability is a </remarks>
    public class RadialStatIndicator : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private ScoreType  associatedType;
        [SerializeField] private Image      associatedImageElement;
#pragma warning restore 0649

        /// <summary>
        /// On Start, ensure the UI displays proper values by calling the OnScoreWasChanged handler
        /// </summary>
        private void Start()
        {
            switch (associatedType)
            {
                case ScoreType.TotalViability:
                    OnScoreWasChanged(ScoreType.TotalViability, GameSave.Instance.currentHappiness + GameSave.Instance.currentPower + GameSave.Instance.currentSustenance);
                    break;
                case ScoreType.PotentialViability:
                    OnScoreWasChanged(ScoreType.PotentialViability, 0);
                    break;
            }
        }

        /// <summary>
        /// On Enable, subscribe to the ScoreWasChanged event
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.ScoreWasChanged += OnScoreWasChanged;
        }

        /// <summary>
        /// On Disable, unsubscribe from the ScoreWasChanged event
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.ScoreWasChanged -= OnScoreWasChanged;
        }

        /// <summary>
        /// On the ScoreWasChanged event, update UI if the proper type matches
        /// </summary>
        /// <param name="type">The type that was updated</param>
        /// <param name="val">The value at the passed type</param>
        private void OnScoreWasChanged(ScoreType type, int val)
        {
            if (type == associatedType)
            {
                switch (associatedType)
                {
                    case ScoreType.PotentialViability:
                        if (val == 0)
                            associatedImageElement.fillAmount = 0;
                        else
                            associatedImageElement.fillAmount = (float)(val + GameScoring.Instance.TotalViability) / GoalSystem.Instance.CurrentGoalWorkingSet.goalViability;
                        break;
                    case ScoreType.TotalViability:
                        associatedImageElement.fillAmount = (float)val / GoalSystem.Instance.CurrentGoalWorkingSet.goalViability;
                        break;
                }
            }
        }
    }
}
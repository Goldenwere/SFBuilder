using TMPro;
using UnityEngine;

namespace SFBuilder.UI
{
    public class StatIndicator : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private ScoreType  associatedType;
        [SerializeField] private TMP_Text   associatedTextElement;
#pragma warning restore 0649
        /**************/ private string[]   workingString;

        /// <summary>
        /// On Start, ensure the UI displays proper values by calling the OnScoreWasChanged handler
        /// </summary>
        private void Start()
        {
            workingString = new string[2] { "", "" };
            if (associatedType == ScoreType.CurrentGoal)
                OnScoreWasChanged(associatedType, 1);
            else
                OnScoreWasChanged(associatedType, 0);
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
            bool changedUI = false;
            if (type == associatedType)
            {
                workingString[0] = val.ToString();
                changedUI = true;
            }

            else if (type == ScoreType.PotentialHappiness && associatedType == ScoreType.TotalHappiness ||
                     type == ScoreType.PotentialPower && associatedType == ScoreType.TotalPower ||
                     type == ScoreType.PotentialSustenance && associatedType == ScoreType.TotalSustenance ||
                     type == ScoreType.PotentialViability && associatedType == ScoreType.TotalViability)
            {
                changedUI = true;

                if (val == 0)
                {
                    associatedTextElement.color = Color.white;
                    workingString[1] = "";
                }

                else
                {
                    if (val > 0)
                    {
                        associatedTextElement.color = Color.cyan;
                        workingString[1] = "(+";
                    }

                    else
                    {
                        associatedTextElement.color = Color.red;
                        workingString[1] = "(";
                    }

                    workingString[1] += val.ToString() + ")";
                }
            }

            if (changedUI)
                associatedTextElement.text = workingString[0] + workingString[1];
        }
    }
}
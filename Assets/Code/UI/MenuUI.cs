using UnityEngine;

namespace SFBuilder.UI
{
    public class MenuUI : MonoBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private GameObject canvas;
#pragma warning restore 0649

        /// <summary>
        /// When the play button is pressed, load the game
        /// </summary>
        public void OnPlayPressed()
        {
            canvas.SetActive(false);
        }
    }
}
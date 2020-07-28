using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SFBuilder.UI
{
    /// <summary>
    /// Handler for post processing settings per volume/scene
    /// </summary>
    public class PostProcessingHandler : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private PostProcessVolume  attachedVol;
#pragma warning restore 0649
        #endregion
        #region Methods
        /// <summary>
        /// Load settings on Start
        /// </summary>
        private void Start()
        {
            LoadSettings();
        }

        /// <summary>
        /// Subscribe to SettingsUpdated event
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.SettingsUpdated += LoadSettings;
        }

        /// <summary>
        /// Unsubscribe from SettingsUpdated event
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.SettingsUpdated -= LoadSettings;
        }

        /// <summary>
        /// Loads settings at Start and when settings are updated
        /// </summary>
        private void LoadSettings()
        {
            attachedVol.profile.GetSetting<AmbientOcclusion>().enabled.value = GameSettings.Instance.Settings.postprocAO;
            attachedVol.profile.GetSetting<Bloom>().enabled.value = GameSettings.Instance.Settings.postprocBloom;
            attachedVol.profile.GetSetting<ScreenSpaceReflections>().enabled.value = GameSettings.Instance.Settings.postprocSSR;
        }
        #endregion
    }
}
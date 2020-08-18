using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace SFBuilder
{
    /// <summary>
    /// Manages game settings
    /// </summary>
    public class GameSettings : MonoBehaviour
    {
        #region Fields
        private SettingsData    settings;
        #endregion
        #region Properties
        /// <summary>
        /// Singleton instance of GameSettings in the base scene
        /// </summary>
        public static GameSettings  Instance { get; private set; }

        /// <summary>
        /// The current settings
        /// </summary>
        public SettingsData         Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                GameEventSystem.Instance.NotifySettingsChanged();
                SaveSettings();
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// Set singleton instance on Awake
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            LoadSettings();
        }

        /// <summary>
        /// Loads game settings
        /// </summary>
        private void LoadSettings()
        {
            if (File.Exists(Application.persistentDataPath + GameConstants.DataPathSettings))
            {
                XmlSerializer xs;
                TextReader txtReader = null;

                try
                {
                    xs = new XmlSerializer(typeof(SettingsData));
                    txtReader = new StreamReader(Application.persistentDataPath + GameConstants.DataPathSettings);
                    Settings = (SettingsData)xs.Deserialize(txtReader);
                }

                catch (Exception)
                {
                    // TO-DO: singleton exception handler that opens a UI canvas outputting errors
                }

                finally
                {
                    if (txtReader != null)
                        txtReader.Close();
                }
            }

            else
            {
                settings = new SettingsData
                {
                    postprocAO = false,
                    postprocBloom = false,
                    postprocSSR = false,
                    volEffects = 1.0f,
                    volMusic = 1.0f
                };
                if (SaveSettings())
                    LoadSettings();
            }
        }

        /// <summary>
        /// Save game settings
        /// </summary>
        /// <returns>Whether there was an error or not</returns>
        private bool SaveSettings()
        {
            XmlSerializer xs;
            TextWriter txtWriter = null;

            try
            {
                xs = new XmlSerializer(typeof(SettingsData));
                txtWriter = new StreamWriter(Application.persistentDataPath + GameConstants.DataPathSettings);
                xs.Serialize(txtWriter, settings);
                return true;
            }

            catch (Exception)
            {
                // TO-DO: singleton exception handler that opens a UI canvas outputting errors
            }

            finally
            {
                if (txtWriter != null)
                    txtWriter.Close();
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// Data structure for game settings
    /// </summary>
    public struct SettingsData
    {
        public bool     postprocAO;
        public bool     postprocBloom;
        public bool     postprocSSR;
        public float    volEffects;
        public float    volMusic;
    }
}
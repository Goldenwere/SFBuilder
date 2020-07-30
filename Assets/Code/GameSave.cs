using UnityEngine;
using System.Collections.Generic;

namespace SFBuilder
{
    /// <summary>
    /// Class which manages game SaveData
    /// </summary>
    public class GameSave : MonoBehaviour
    {
        #region Fields
        private List<PlacedBuilderObjectData>   currentlyPlacedObjects;
        public  int                             currentLevel;
        public  int                             currentHappiness;
        public  int                             currentPower;
        public  int                             currentSustenance;
        #endregion
        #region Properties
        /// <summary>
        /// Singleton instance of GameSettings in the base scene
        /// </summary>
        public static GameSave  Instance { get; private set; }
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
        }

        /// <summary>
        /// On Enable, subscribe to events
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.GameStateChanged += OnGameStateChanged;
        }

        /// <summary>
        /// On Disable, unsubscribe from events
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.GameStateChanged -= OnGameStateChanged;
        }

        /// <summary>
        /// On ApplicationQuit, save data
        /// </summary>
        private void OnApplicationQuit()
        {
            DataSave();
        }

        /// <summary>
        /// Save data when leaving to menus
        /// </summary>
        /// <param name="prev">The previous GameState</param>
        /// <param name="curr">The current GameState</param>
        private void OnGameStateChanged(GameState prev, GameState curr)
        {
            if (curr == GameState.MainMenus)
                DataSave();
        }

        /// <summary>
        /// Adds a BuilderObject's data to be used in SaveData
        /// </summary>
        /// <param name="_position">The position of the BuilderObject</param>
        /// <param name="_rotation">The rotation of the BuilderObject</param>
        /// <param name="_type">The ObjectType of the BuilderObject</param>
        public void AddBuilderObject(Vector3 _position, Quaternion _rotation, ObjectType _type)
        {
            currentlyPlacedObjects.Add(new PlacedBuilderObjectData
            { 
                position = _position,
                rotation = _rotation,
                type = _type
            });
        }

        /// <summary>
        /// Loads game save from JSON
        /// </summary>
        public void DataLoad()
        {

        }

        /// <summary>
        /// Saves current data to JSON
        /// </summary>
        public void DataSave()
        {

        }

        /// <summary>
        /// Removes the last placed object when an object is undone
        /// </summary>
        public void PopBuilderObject()
        {
            currentlyPlacedObjects.RemoveAt(currentlyPlacedObjects.Count - 1);
        }
        #endregion
    }

    /// <summary>
    /// Holds data regarding placed BuilderObjects for use in SaveData
    /// </summary>
    public struct PlacedBuilderObjectData
    {
        public Vector3      position;
        public Quaternion   rotation;
        public ObjectType   type;
    }

    /// <summary>
    /// Holds data regarding SaveData
    /// </summary>
    public struct SaveData
    {
        public int                          currentLevel;
        public int                          currentStatHappiness;
        public int                          currentStatPower;
        public int                          currentStatSustenance;
        public PlacedBuilderObjectData[]    placedObjects;
    }
}
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
        private List<PlacedBuilderObjectData> currentlyPlacedObjects;
        #endregion
        #region Properties
        /// <summary>
        /// Singleton instance of GameSettings in the base scene
        /// </summary>
        public static GameSave Instance { get; private set; }
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
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SFBuilder
{
    /// <summary>
    /// Class which manages game SaveData
    /// </summary>
    public class GameSave : MonoBehaviour
    {
        #region Fields
        public  int                             currentGoal;
        public  int[]                           currentGoalSetCount;
        public  int                             currentGoalSetIndex;
        public  int                             currentGoalSetViability;
        public  int                             currentHappiness;
        public  int                             currentLevel;
        public  int                             currentPower;
        public  int                             currentSustenance;
        #endregion
        #region Properties
        /// <summary>
        /// Singleton instance of GameSettings in the base scene
        /// </summary>
        public static GameSave  Instance { get; private set; }

        /// <summary>
        /// Currently placed BuilderObjects based on SaveData
        /// </summary>
        public List<PlacedBuilderObjectData> CurrentlyPlacedObjects { get; private set; }
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

            DataLoad();
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
            CurrentlyPlacedObjects.Add(new PlacedBuilderObjectData
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
            if (!File.Exists(Application.persistentDataPath + GameConstants.DataPathSave))
            {
                if (DataSave())
                    DataLoad();
            }

            else
            {
                TextReader txtReader = null;

                try
                {
                    txtReader = new StreamReader(Application.persistentDataPath + GameConstants.DataPathSave);
                    SaveData dataToLoad = (SaveData)JsonUtility.FromJson(txtReader.ReadToEnd(), typeof(SaveData));
                    currentGoal = dataToLoad.goal;
                    currentGoalSetCount = dataToLoad.goalSetCount;
                    currentGoalSetIndex = dataToLoad.goalSetIndex;
                    currentGoalSetViability = dataToLoad.goalSetViability;
                    currentLevel = dataToLoad.level;
                    currentHappiness = dataToLoad.statHappiness;
                    currentPower = dataToLoad.statPower;
                    currentSustenance = dataToLoad.statSustenance;
                    if (dataToLoad.placedObjects != null)
                        CurrentlyPlacedObjects = new List<PlacedBuilderObjectData>(dataToLoad.placedObjects);
                    else
                        CurrentlyPlacedObjects = new List<PlacedBuilderObjectData>();
                }

                catch (System.Exception)
                {

                }

                finally
                {
                    if (txtReader != null)
                        txtReader.Close();
                }
            }
        }

        /// <summary>
        /// Saves current data to JSON
        /// </summary>
        /// <returns>Whether there was an error or not</returns>
        public bool DataSave()
        {
            SaveData dataToSave;
            if (!File.Exists(Application.persistentDataPath + GameConstants.DataPathSave))
                dataToSave = new SaveData
                {
                    goal = 0,
                    // this is currently based off of what is set in Level_01
                    goalSetCount = new int[] { 3, 8, 2 },
                    goalSetIndex = 0,
                    goalSetViability = 0,
                    level = 1,
                    placedObjects = new PlacedBuilderObjectData[0],
                    statHappiness = 0,
                    statPower = 0,
                    statSustenance = 0
                };
            else
                dataToSave = new SaveData
                {
                    goal = currentGoal,
                    goalSetCount = currentGoalSetCount,
                    goalSetIndex = currentGoalSetIndex,
                    goalSetViability = currentGoalSetViability,
                    level = currentLevel,
                    placedObjects = CurrentlyPlacedObjects.ToArray(),
                    statHappiness = currentHappiness,
                    statPower = currentPower,
                    statSustenance = currentSustenance
                };

            string data = JsonUtility.ToJson(dataToSave);
            TextWriter txtWriter = null;

            try
            {
                txtWriter = new StreamWriter(Application.persistentDataPath + GameConstants.DataPathSave);
                txtWriter.Write(data);
                return true;
            }

            catch(System.Exception)
            {

            }

            finally
            {
                if (txtWriter != null)
                    txtWriter.Close();
            }

            return false;
        }

        /// <summary>
        /// Removes the last placed object when an object is undone
        /// </summary>
        public void PopBuilderObject()
        {
            CurrentlyPlacedObjects.RemoveAt(CurrentlyPlacedObjects.Count - 1);
        }
        #endregion
    }

    /// <summary>
    /// Holds data regarding placed BuilderObjects for use in SaveData
    /// </summary>
    [System.Serializable]
    public struct PlacedBuilderObjectData
    {
        public Vector3      position;
        public Quaternion   rotation;
        public ObjectType   type;
    }

    /// <summary>
    /// Holds data regarding GameSave
    /// </summary>
    [System.Serializable]
    public struct SaveData
    {
        public int                          goal;
        public int[]                        goalSetCount;
        public int                          goalSetIndex;
        public int                          goalSetViability;
        public int                          level;
        public PlacedBuilderObjectData[]    placedObjects;
        public int                          statHappiness;
        public int                          statPower;
        public int                          statSustenance;
    }
}
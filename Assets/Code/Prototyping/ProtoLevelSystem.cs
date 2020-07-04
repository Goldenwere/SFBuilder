using UnityEngine;

namespace SFBuilder.Prototyping
{
    public class ProtoLevelSystem : MonoBehaviour
    {
        private int currentLevel;   // directly equivalent to unity scenes, with scene 0 being the base scene
        public int CurrentLevel
        {
            get { return currentLevel; }
            set
            {
                currentLevel = value;
                // TO-DO: transitions
            }
        }
        public static ProtoLevelSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            currentLevel = 1;
        }
    }
}
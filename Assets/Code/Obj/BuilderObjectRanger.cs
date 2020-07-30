using UnityEngine;
using System.Collections.Generic;

namespace SFBuilder.Obj
{
    /// <summary>
    /// The ranger (representing a sphere surrounding a BuilderObject) is used for determining updates to the game scoring when placing BuilderObjects
    /// </summary>
    public class BuilderObjectRanger : MonoBehaviour
    {
        #region Fields
#pragma warning disable 0649
        [SerializeField] private MeshRenderer           rangerMeshRenderer;
        [SerializeField] private SphereCollider         rangerSphereCollider;
#pragma warning restore 0649
        /**************/ private List<BuilderObject>    othersCollided;
        /**************/ private BuilderObject          parent;
        /**************/ private int                    objectHappiness;
        /**************/ private int                    objectPower;
        /**************/ private int                    objectSustenance;
        /**************/ private int                    placedHappiness;
        /**************/ private int                    placedPower;
        /**************/ private int                    placedSustenance;
        /**************/ private bool                   toBeBanished;
        #endregion
        #region Methods
        /// <summary>
        /// Instantiate list and determine base score values on Start
        /// </summary>
        private void Start()
        {
            othersCollided = new List<BuilderObject>();
            parent = transform.parent.GetComponent<BuilderObject>();
            BuilderObject.ScoreOfSingleType(parent.Type, out int hp, out int pp, out int sp);
            objectHappiness = hp;
            objectPower = pp;
            objectSustenance = sp;
            if (rangerSphereCollider.enabled)
                Calculate();
        }

        /// <summary>
        /// On Enable, subscribe to the LevelBanished event
        /// </summary>
        private void OnEnable()
        {
            GameEventSystem.LevelBanished += OnLevelBanished;
        }

        /// <summary>
        /// OnDisable, unsubscribe from the LevelBanished event
        /// </summary>
        private void OnDisable()
        {
            GameEventSystem.LevelBanished -= OnLevelBanished;
        }

        /// <summary>
        /// When the ranger is being destroyed, ensure the score system gets updated
        /// </summary>
        private void OnDestroy()
        {
            if (!toBeBanished)
            {
                if (placedHappiness != 0)
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialHappiness, -placedHappiness);
                if (placedPower != 0)
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialPower, -placedPower);
                if (placedSustenance != 0)
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialSustenance, -placedSustenance);
            }
        }

        /// <summary>
        /// Track other BuilderObjects in range of the ranger
        /// </summary>
        /// <param name="other">The other collider that entered</param>
        private void OnTriggerEnter(Collider other)
        {
            BuilderObject otherObj = other.GetComponent<BuilderObject>();
            if (otherObj != null && otherObj != parent)
            {
                othersCollided.Add(otherObj);
                Calculate();
            }
        }

        /// <summary>
        /// Untrack other BuilderObjects no longer in range of the ranger
        /// </summary>
        /// <param name="other">The other collider that exited</param>
        private void OnTriggerExit(Collider other)
        {
            BuilderObject otherObj = other.GetComponent<BuilderObject>();
            if (otherObj != null && otherObj != parent)
            {
                othersCollided.Remove(otherObj);
                Calculate();
            }
        }

        /// <summary>
        /// When setting IsPlaced on a BuilderObject, toggle the ranger so that it is hidden from the game (view and code)
        /// </summary>
        /// <param name="placed">Whether the BuilderObject is placed or not</param>
        public void SetPlaced(bool placed)
        {
            if (placed)
            {
                rangerMeshRenderer.enabled = false;
                rangerSphereCollider.enabled = false;
                if (placedHappiness != 0)
                {
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.TotalHappiness, placedHappiness);
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialHappiness, -placedHappiness);
                }
                if (placedPower != 0)
                {
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.TotalPower, placedPower);
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialPower, -placedPower);
                }
                if (placedSustenance != 0)
                {
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.TotalSustenance, placedSustenance);
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialSustenance, -placedSustenance);
                }
                if (othersCollided != null)
                    othersCollided.Clear();
            }

            else
            {
                rangerMeshRenderer.enabled = true;
                rangerSphereCollider.enabled = true;
                if (placedHappiness != 0)
                {
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.TotalHappiness, -placedHappiness);
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialHappiness, placedHappiness);
                }
                if (placedPower != 0)
                {
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.TotalPower, -placedPower);
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialPower, placedPower);
                }
                if (placedSustenance != 0)
                {
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.TotalSustenance, -placedSustenance);
                    GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialSustenance, placedSustenance);
                }
            }
        }

        /// <summary>
        /// Calculates the resulting score based on the BuilderObject's position and others within the BuilderObject's ranger
        /// </summary>
        private void Calculate()
        {
            int prevHappiness = placedHappiness;
            int prevPower = placedPower;
            int prevSustenance = placedSustenance;

            placedHappiness = objectHappiness;
            placedPower = objectPower;
            placedSustenance = objectSustenance;

            foreach (BuilderObject po in othersCollided)
            {
                BuilderObject.ScoreOfTwoTypes(parent.Type, po.Type, out int workingHappiness, out int workingPower, out int workingSustenance);
                placedHappiness += workingHappiness;
                placedPower += workingPower;
                placedSustenance += workingSustenance;
            }

            int deltaHappiness = placedHappiness - prevHappiness;
            int deltaPower = placedPower - prevPower;
            int deltaSustenance = placedSustenance - prevSustenance;

            if (deltaHappiness != 0)
                GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialHappiness, deltaHappiness);
            if (deltaPower != 0)
                GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialPower, deltaPower);
            if (deltaSustenance != 0)
                GameEventSystem.Instance.UpdateScoreSystem(ScoreType.PotentialSustenance, deltaSustenance);
        }

        /// <summary>
        /// When the level is to be banished, track this to ensure score doesn't get updated OnDestroy
        /// </summary>
        private void OnLevelBanished()
        {
            toBeBanished = true;
        }
        #endregion
    }
}

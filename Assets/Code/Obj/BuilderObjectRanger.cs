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
            //GameScoring.Instance.PotentialPower = pp;
            //GameScoring.Instance.PotentialSustenance = sp;
            //GameScoring.Instance.PotentialHappiness = hp;
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
                //GameScoring.Instance.ApplyScore();
                othersCollided.Clear();
            }

            else
            {
                rangerMeshRenderer.enabled = true;
                rangerSphereCollider.enabled = true;
                //GameScoring.Instance.RevokeScore(placedHappiness, placedPower, placedSustenance);
            }
        }

        /// <summary>
        /// Calculates the resulting score based on the BuilderObject's position and others within the BuilderObject's ranger
        /// </summary>
        private void Calculate()
        {
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

            //GameScoring.Instance.PotentialHappiness = placedHappiness;
            //GameScoring.Instance.PotentialPower = placedPower;
            //GameScoring.Instance.PotentialSustenance = placedSustenance;
        }
        #endregion
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding
{
    using Pathfinding.Util;
    [RequireComponent(typeof(Seeker))]
    public class distanceCalculatorAStar : VersionedMonoBehaviour, IAstarAI
    {

        #region IAstarAI implementation
        float repathRate = 0;

        /// <summary>\copydoc Pathfinding::IAstarAI::canSearch</summary>
        bool canSearch = true;

        /// <summary>\copydoc Pathfinding::IAstarAI::canMove</summary>
        bool canMove = false;

        /// <summary>Speed in world units</summary>
        float speed = 0;


        /// <summary>\copydoc Pathfinding::IAstarAI::Move</summary>
        void IAstarAI.Move(Vector3 deltaPosition) { }

        /// <summary>\copydoc Pathfinding::IAstarAI::radius</summary>
        float IAstarAI.radius { get { return 0; } set { } }

        /// <summary>\copydoc Pathfinding::IAstarAI::height</summary>
        float IAstarAI.height { get { return 0; } set { } }

        /// <summary>\copydoc Pathfinding::IAstarAI::maxSpeed</summary>
        float IAstarAI.maxSpeed { get { return speed; } set { speed = value; } }

        /// <summary>\copydoc Pathfinding::IAstarAI::canSearch</summary>
        bool IAstarAI.canSearch { get { return canSearch; } set { canSearch = value; } }

        /// <summary>\copydoc Pathfinding::IAstarAI::canMove</summary>
        bool IAstarAI.canMove { get { return canMove; } set { canMove = value; } }

        Vector3 IAstarAI.position { get; }

        Quaternion IAstarAI.rotation { get; }

        Vector3 IAstarAI.velocity { get; }

        Vector3 IAstarAI.desiredVelocity { get; }

        float IAstarAI.remainingDistance { get; }

        bool IAstarAI.reachedDestination { get; }

        bool IAstarAI.reachedEndOfPath { get; }

        Vector3 IAstarAI.destination { get; set; }

        bool IAstarAI.hasPath { get; }

        bool IAstarAI.pathPending { get; }

        bool IAstarAI.isStopped { get; set; }

        Vector3 IAstarAI.steeringTarget { get; }

        System.Action IAstarAI.onSearchPath { get; set; }

        void IAstarAI.SearchPath() { }

        void IAstarAI.SetPath(Path path) { }

        void IAstarAI.Teleport(Vector3 newPosition, bool clearPath = true) { }

        void IAstarAI.MovementUpdate(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
        {
            nextPosition = new Vector3(0, 0, 0);
            nextRotation = new Quaternion(0, 0, 0, 0);
        }

        void IAstarAI.FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation) { }
        #endregion

        [SerializeField] GameObject target;
        [SerializeField] int queuedUnits;
        Queue<GameObject> queueList;

        Seeker seeker;

        void Start()
        {
            seeker = GetComponent<Seeker>();
            queueList = new Queue<GameObject>();
            queuedUnits = queueList.Count;
        }

        private void calculateDistance(Vector3 origin)
        {
            seeker.StartPath(origin, target.transform.position, onPathComplete);
        }
        
        private void onPathComplete(Path p)
        {
            float distanceAux = p.GetTotalLength();
            queueList.Peek().GetComponent<PlayerController>().setDistanceToEnd(distanceAux);
            //Debug.Log(queueList[0].name + "  distance: " + distanceAux);
            queueList.Dequeue();
            queuedUnits = queueList.Count;
        }

        private IEnumerator courutinePathCalc()
        {
            if (queueList.Count > 0)
            {
                calculateDistance(queueList.Peek().transform.position);
                yield return new WaitUntil(() => seeker.IsDone() == true);
                StartCoroutine(courutinePathCalc());
            }
            else
            {
                yield return new WaitForSeconds(0.0f);
            }
        }

        public void queuePath(GameObject origin)
        {
            if (!queueList.Contains(origin))
            {
                queuedUnits = queueList.Count;
                queueList.Enqueue(origin);
                StartCoroutine(courutinePathCalc());
            }
        }

        public void resetQueue()
        {
            queueList.Clear();
        }

        public bool seekerIsDone()
        {
            return seeker.IsDone();
        }

        public bool queueIsDone()
        {
            return queueList.Count == 0;
        }
    }
}
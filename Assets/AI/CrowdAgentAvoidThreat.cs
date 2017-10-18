﻿using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.AI
{
    public class CrowdAgentAvoidThreat : CrowdAgent
    {
        private GameObject _nearestThreat;
        private bool _seekingExit;
        private GameObject[] _threats;
        public float PanicRunAmount;


        protected override void Start()
        {
            base.Start();
            //TODO: GameObject.FindObjectOfType<Threat>();
            _threats = GameObject.FindGameObjectsWithTag("Threat");
            _seekingExit = gameObject.GetComponent<CrowdAgentSeekExit>().enabled;

            PanicRunAmount = DangerDistance * .6f;
            StartCoroutine("DoProximityCheck");

            InvokeRepeating("AvoidThreats", 0, ReactionTime);
        }

        [UsedImplicitly]
        private void AvoidThreats()
        {
            //if theres no threat detected, seek the exit
            if (_nearestThreat == null)
            {
                if (!_seekingExit) _seekingExit = true;
                return;
            }

            _seekingExit = false;

            transform.rotation = Quaternion.LookRotation(transform.position - _nearestThreat.transform.position);

            var runTo = transform.position + transform.forward * PanicRunAmount;

            NavMeshHit hit;
            NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));

            Agent.SetDestination(hit.position);
        }

        private GameObject ProximityCheck()
        {
            return _threats.FirstOrDefault(t =>
                Vector3.Distance(transform.position, t.transform.position) < DangerDistance);
        }

        //proximity check for threats, every tenth of a second
        private IEnumerator DoProximityCheck()
        {
            for (;;)
            {
                _nearestThreat = ProximityCheck();
                yield return new WaitForSeconds(.5f);
            }
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

namespace CodeBase.Game.LevelParts.ioPlayer
{
    public class Bot : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public Transform target;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            _agent.SetDestination(target.position);
        }
    }
}

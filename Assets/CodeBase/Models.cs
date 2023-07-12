using UnityEngine;

namespace CodeBase
{
    public class Leader
    {
        public readonly Transform Player;
        public readonly float Dist;

        public Leader(Transform leader, float distance)
        {
            Player = leader;
            Dist = distance;
        }
    }

}


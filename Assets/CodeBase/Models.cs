using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpUp.Data
{
    public class Leader
    {
        public Transform player;
        public float dist;

        public Leader(Transform _leader, float _dist)
        {
            player = _leader;
            dist = _dist;
        }
    }

}


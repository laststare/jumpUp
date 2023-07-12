using System.Collections.Generic;
using CodeBase.Content;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Level
{
    public class Level : MonoBehaviour
    {

        public Vector3 finishPosiiton;
        public Vector3 playersPosiiton;
        public List<LevelContainer.Jumper> jumprs;

        public List<LevelContainer.Floor> floors;
        public LevelContainer _container;

        public List<LevelContainer.IoPlayer> ioplayers;
             
        public Level(LevelContainer container)
        {
            finishPosiiton = container.finish;

            playersPosiiton = container.playerPlace;

            jumprs = new List<LevelContainer.Jumper>();
            jumprs.AddRange(container.jumpers);

            floors = new List<LevelContainer.Floor>();
            floors.AddRange(container.floors);

            _container = container;

            ioplayers = new List<LevelContainer.IoPlayer>();
            ioplayers.AddRange(container.ioplayers);
        }
    }
}

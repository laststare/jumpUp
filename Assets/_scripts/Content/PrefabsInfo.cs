using System.Collections;
using System.Collections.Generic;
using _scripts.Game.LevelParts.Camera;
using _scripts.Game.LevelParts.Finish;
using _scripts.Game.LevelParts.ioPlayer;
using _scripts.Game.LevelParts.Player;
using UnityEngine;
using JumpUp.Player;

namespace JumpUp.Content
{
    [CreateAssetMenu(fileName = "Prefabs Info", menuName = "GameData/Create Prefabs Info")]
    public class PrefabsInfo : ScriptableObject
    {

        public PlayerView       playerPrefab;
        public CameraView       camera;
        public FinishView       finish;
        public JumperView[]     jumper;
        public GameObject[]     cell;
        public GameObject[]     navFloor;
        public IoPLayerView[]     ioplayer;
        public PlayersNameView  playersName;


    }
}

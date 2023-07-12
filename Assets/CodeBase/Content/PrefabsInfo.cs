using CodeBase.Game.LevelParts.Camera;
using CodeBase.Game.LevelParts.Finish;
using CodeBase.Game.LevelParts.ioPlayer;
using CodeBase.Game.LevelParts.Jumper;
using CodeBase.Game.LevelParts.Player;
using CodeBase.UI;
using UnityEngine;

namespace CodeBase.Content
{
    [CreateAssetMenu(fileName = "Prefabs Info", menuName = "GameData/Create Prefabs Info")]
    public class PrefabsInfo : ScriptableObject
    {

        public PlayerView playerPrefab;
        public CameraView camera;
        public FinishView finish;
        public JumperView[] jumper;
        public GameObject[] cell;
        public GameObject[] navFloor;
        public IoPLayerView[] ioplayer;
        public PlayersNameView playersName;
        public GameObject startTutorText;
        public StartCounterView startCounter;
        public GameInfoView gameInfoView;
        public GameObject endlessSignTutor;
        public WinTextView winTextView;
    }
}

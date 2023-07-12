using System;
using JumpUp.External;
using System.Collections;
using System.Collections.Generic;
using CodeBase;
using CodeBase.Content;
using CodeBase.Game.LevelParts.ioPlayer;
using CodeBase.Game.LevelParts.Jumper;
using CodeBase.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace JumpUp.Content
{
   
    public interface IContent : System.IDisposable
    {
        LevelContainer  GetLevelContainer(int levelIndex);     
        GameObject GetPlayerView();
        GameObject GetCamera();
        GameObject GetFinish();

        JumperView GetJumper(JumperType type);

        GameObject GetCell(CellColor type);

        GameObject GetNavFloor(FloorType type);

        GameObject GetEmptyCell();
        Material GetGlassMat();
        Material GetWhiteMat();
        IoPLayerView GetIoPlayer(ioPlayerType type);
        string GetIoName();

        void ResetLevel();
        Material GetManMaterial();
        GameObject GetStartTutorText();
        GameInfoView GetGameInfoView();
        StartCounterView GetStartCounterView();
        GameObject GetEndlessSignTutor();
        WinTextView GetWinTextView();

    }

    public class ContentLoader : BaseDisposable, IContent
    {
        public struct Ctx
        {
            public PrefabsInfo Prefabs;
        }

        private readonly LevelContainer[] _levels;
        private readonly PrefabsInfo _prefabs;
        private readonly Material _glass, _white;
        private readonly Material[] _manMaterials;
        private List<Material> _skins;
        private string[] _names;

        public ContentLoader(Ctx ctx)
        {
            _levels = Resources.LoadAll<LevelContainer>("Levels");
            _prefabs = ctx.Prefabs;
            _glass = Resources.Load<Material>("CubeOpacityMat");
            _white = Resources.Load<Material>("cubeWhiteMat");
            _manMaterials = Resources.LoadAll<Material>("ManMaterials");
            LoadNames();
        }

        public GameObject GetPlayerView() => _prefabs.playerPrefab.gameObject;

        public LevelContainer GetLevelContainer(int levelIndex)
        {
            if (levelIndex < _levels.Length)
            {
                return _levels[levelIndex];
            }
            var s = CalcLevel(levelIndex);
            return _levels[CalcLevel(s)];
        }

        private int CalcLevel(int i)
        {
            var cel = i / _levels.Length;
            var ost = i - cel * _levels.Length;
            return ost;
        }

        public GameObject GetCamera() => _prefabs.camera.gameObject;

        public GameObject GetFinish() => _prefabs.finish.gameObject;

        public JumperView GetJumper(JumperType type)
        {
            return type switch
            {
                JumperType.rocket => _prefabs.jumper[1],
                JumperType.bat => _prefabs.jumper[2],
                JumperType.oldCell => _prefabs.jumper[3],
                _ => _prefabs.jumper[0]
            };
        }

        public GameObject GetCell(CellColor type)
        {
            return type switch
            {
                CellColor.blue => _prefabs.cell[0],
                CellColor.darkblue => _prefabs.cell[1],
                CellColor.green => _prefabs.cell[2],
                CellColor.red => _prefabs.cell[3],
                CellColor.violet => _prefabs.cell[4],
                _ => _prefabs.cell[0]
            };
        }

        public Material GetGlassMat() => _glass;
        public Material GetWhiteMat() => _white;

        public GameObject GetNavFloor(FloorType type)
        {
            return type switch
            {
                FloorType.cells30x30 => _prefabs.navFloor[1],
                FloorType.cells25x25 => _prefabs.navFloor[2],
                _ => _prefabs.navFloor[0]
            };
        }

        public GameObject GetEmptyCell() => _prefabs.cell[^1];

        public IoPLayerView GetIoPlayer(ioPlayerType type)
        {
            return type switch
            {
                _ => _prefabs.ioplayer[0]
            };
        }

        public PlayersNameView GetPlayersName() => _prefabs.playersName;

        public string GetIoName()
        {
            var name = _names[Random.Range(0, _names.Length)];
            name = name.ToUpper();
            return name;
        }

        public Material GetManMaterial()
        {
            var i = Random.Range(0, _skins.Count);
            var m = _skins[i];
            _skins.RemoveAt(i);
            return m;
        }

        public GameObject GetStartTutorText() => _prefabs.startTutorText;
        public GameInfoView GetGameInfoView() => _prefabs.gameInfoView;
        public StartCounterView GetStartCounterView() => _prefabs.startCounter;
        public GameObject GetEndlessSignTutor() => _prefabs.endlessSignTutor;
        public WinTextView GetWinTextView() => _prefabs.winTextView;

        public void ResetLevel()
        {
            _skins = new List<Material>();
            _skins.AddRange(_manMaterials);
        }

        private void LoadNames()
        {
            var loadedTextFile = Resources.Load("Names") as TextAsset;
            _names = loadedTextFile.text.Split(char.Parse(" "));
        }
        
    }
}

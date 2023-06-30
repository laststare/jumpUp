using System;
using System.Collections.Generic;
using System.Linq;
using _scripts.Game.LevelParts.Finish;
using _scripts.Game.LevelParts.ioPlayer;
using _scripts.Game.LevelParts.Player;
using JumpUp;
using JumpUp.Player;
using UnityEditor;
using UnityEngine;

namespace _scripts.Content
{
  

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelContainer))]
    public class LevelContainerEditor : Editor
    {
        private LevelContainer _levelContainer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var data = target as LevelContainer;

            if (!isTexturesLoaded)
            {
                LoadTextures();
                isTexturesLoaded = true;
            }
            #region buttons
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            if (GUILayout.Button("SAVE JUMPERS") && Application.isPlaying)
                SaveJumpers();

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            if (GUILayout.Button("SAVE IOPLAYERS") && Application.isPlaying)
                SaveIoPlayers();

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            if (GUILayout.Button("SAVE FINISH") && Application.isPlaying)
                SaveFinishData();

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            if (GUILayout.Button("SAVE PLAYER") && Application.isPlaying)
                SavePlayersData();

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            if (GUILayout.Button("Clear cells"))
                ClearCells();
            #endregion

            EditorGUILayout.BeginScrollView(Vector2.zero, false, false, GUILayout.Height(60));
            GUILayout.Label("Пустая клетка: Click + Shift", EditorStyles.boldLabel);
            GUILayout.Label("Прыгалка: Click + Alt", EditorStyles.boldLabel);
            GUILayout.Label("Стандартная клетка: Click + Ctrl", EditorStyles.boldLabel);
            EditorGUILayout.EndScrollView();
            for (int i = 0; i < data.floors.Length; i++)
            {
                EditorGUILayout.BeginScrollView(Vector2.zero, false, false, GUILayout.Height(450));
                DrawCells(data.floors[i], i, data);
                EditorGUILayout.EndScrollView();
            }

            EditorUtility.SetDirty(data);
        }

        private void ClearCells()
        {
            var data = target as LevelContainer;
            if (data != null)
                foreach (var t in data.floors)
                {
                    CreateFloor(t, data);
                }

            EditorUtility.SetDirty(data);
        }

        private void SaveFinishData()
        {
            _levelContainer = target as LevelContainer;
            var finishView = FindObjectsOfType<FinishView>();
            _levelContainer.finish = finishView[0].transform.position;
            EditorUtility.SetDirty(_levelContainer);
        }

        private void SavePlayersData()
        {
            _levelContainer = target as LevelContainer;
            var playerView = FindObjectsOfType<PlayerView>();
            _levelContainer.playerPlace = playerView[0].transform.position;
            EditorUtility.SetDirty(_levelContainer);
        }

        private void SaveJumpers()
        {
            _levelContainer = target as LevelContainer;
            var tmp_jumperView = FindObjectsOfType<JumperView>();
            var rocketjumperView = tmp_jumperView.Where(x => x.type != JumperType.medium);
            var jumperView = rocketjumperView.ToArray();
            var jumperContainers = new LevelContainer.Jumper[jumperView.Length];

            for (var i = 0; i < jumperView.Length; i++)
            {
                if (jumperView[i].type != JumperType.medium)
                {
                    jumperContainers[i] = new LevelContainer.Jumper
                    {
                        position = jumperView[i].transform.position,
                        type = jumperView[i].type
                    };
                }
            }

            _levelContainer.jumpers = jumperContainers;
            EditorUtility.SetDirty(_levelContainer);
        }

        private void SaveIoPlayers()
        {
            _levelContainer = target as LevelContainer;
            var ioplayerViews = FindObjectsOfType<IoPLayerView>();

            var ioplayerContainers = new LevelContainer.IoPlayer[ioplayerViews.Length];

            for (var i = 0; i < ioplayerViews.Length; i++)
            {
                ioplayerContainers[i] = new LevelContainer.IoPlayer
                {
                    position = ioplayerViews[i].transform.position,
                    rotation = ioplayerViews[i].transform.rotation,
                    type = ioplayerViews[i].type
                };
            }

            _levelContainer.ioplayers = ioplayerContainers;
            EditorUtility.SetDirty(_levelContainer);
        }

        /// <summary>
        /// draw grid
        /// </summary>
        const float X_OFFSTEP = 10f;
        const float Y_OFFSTEP = 10f;

        private bool isTexturesLoaded;
        private Texture simpleTex, empty, jumper;

        private void LoadTextures()
        {
            simpleTex = Resources.Load<Sprite>("Editor/Simple").texture;
            empty = Resources.Load<Sprite>("Editor/Finish").texture;
            jumper = Resources.Load<Sprite>("Editor/Destroyer").texture;

        }

        private void CreateFloor(LevelContainer.Floor floor, LevelContainer _levelContainer)
        {
            var _size = _levelContainer.CellCount(floor.FloorType);
            floor.cells = new List<LevelContainer.CellInfo>();
            for (var y = _size - 1; y >= 0; y--)
            {
                for (var x = 0; x < _size; x++)
                {
                    floor.cells.Add(new LevelContainer.CellInfo
                    {
                        type = CellType.simple,
                        x = x,
                        y = y,
                    });
                }
            }
        }

        private void DrawCells( LevelContainer.Floor floor, int num, LevelContainer _levelContainer)
        {
            GUILayout.BeginVertical($"Floor {num+1}", GUI.skin.box);
            var _size = _levelContainer.CellCount(floor.FloorType);
            var CELL_SIZE = _levelContainer.CellSize(floor.FloorType);
            if (floor.cells.Count == 0 ) CreateFloor(floor, _levelContainer);

            using (var verticalScope = new GUILayout.VerticalScope(GUILayout.Width(CELL_SIZE * _size + (_size -1) * EditorGUIUtility.standardVerticalSpacing),
                                                                 GUILayout.Height(CELL_SIZE * _size + (_size +2) * EditorGUIUtility.standardVerticalSpacing)))
            {
                for (var y = _size - 1; y >= 0; y--)
                {
                    using (var horizontalScope = new GUILayout.HorizontalScope())
                    {
                        for (var x = 0; x < _size; x++)
                        {
                            var tex = simpleTex;
                            var cell = _levelContainer.GetCellInfo(floor, x, y, out int index);
                            if (cell != null)
                            {
                                tex = GetTex(cell.type);
                            }
                            var rect = new Rect(CELL_SIZE * x + X_OFFSTEP + (x - 1) * EditorGUIUtility.standardVerticalSpacing,
                                                CELL_SIZE * _size + (_size - 1) * EditorGUIUtility.standardVerticalSpacing + Y_OFFSTEP - (CELL_SIZE * y + (y - 1) * EditorGUIUtility.standardVerticalSpacing),
                                                CELL_SIZE,
                                                CELL_SIZE);

                            if (GUI.Button(rect, tex))
                            {

                                if (Event.current.shift) floor.cells[index].type = CellType.empty;
                                if (Event.current.alt) floor.cells[index].type = CellType.jumper;
                                if (Event.current.control) floor.cells[index].type = CellType.simple;                              
                                tex = GetTex(cell.type);

                            }

                            rect = new Rect(rect.x + (CELL_SIZE - CELL_SIZE) / 2.0f,
                                               rect.y + (CELL_SIZE - CELL_SIZE) / 2.0f,
                                               CELL_SIZE,
                                               CELL_SIZE);
                            GUI.DrawTexture(rect, tex);
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }

        private Texture GetTex(CellType cellType)
        {
            return cellType switch
            {
                CellType.empty => empty,
                CellType.jumper => jumper,
                _ => simpleTex
            };
        }
    }
#endif

    [CreateAssetMenu(fileName = "Level", menuName = "GameData/Create Level")]
    public class LevelContainer : ScriptableObject
    {
        public Jumper[] jumpers;
        public IoPlayer[] ioplayers;
        public Vector3 finish;
        public Vector3 playerPlace;
        public Floor[] floors;


        [Serializable]
        public class Jumper
        {
            public Vector3 position;
            public JumperType type;
        }

        [Serializable]
        public class Floor
        {
            public FloorType FloorType;
            public CellColor color;
            public List<CellInfo> cells;   
        }

        [Serializable]
        public class CellInfo
        {
            public CellType type;
            public int x;
            public int y;
        }

        [Serializable]
        public class IoPlayer
        {
            public ioPlayerType type;
            public Vector3 position;
            public Quaternion rotation;
        }


        public CellInfo GetCellInfo(Floor floor, int x, int y, out int index)
        {
            for (var i = 0; i < floor.cells.Count; i++)
            {
                if (floor.cells[i].x == x && floor.cells[i].y == y)
                {
                    index = i;
                    return floor.cells[i];
                }
            }
            index = -1;
            return null;
        }

        public int CellCount(FloorType type)
        {
            return type switch
            {
                FloorType.cells30x30 => 30,
                FloorType.cells25x25 => 25,
                _ => 15
            };
        }

        public int CellSize(FloorType type)
        {
            return type switch
            {
                FloorType.cells30x30 => 12,
                FloorType.cells25x25 => 12,
                _ => 15
            };
        }
    }





}
    

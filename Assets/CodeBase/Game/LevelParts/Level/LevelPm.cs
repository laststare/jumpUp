using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeBase.Content;
using CodeBase.Game.LevelParts.Finish;
using CodeBase.Game.LevelParts.ioPlayer;
using CodeBase.Game.LevelParts.Jumper;
using Cysharp.Threading.Tasks;
using JumpUp;
using JumpUp.Data;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Level
{
    public class LevelPm : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;
            public IReactiveProperty<GameState> gameState;
            public Level Level;
            public ReactiveProperty<Transform> player;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public Transform blocksContainer;
            public ReactiveTrigger destroy;
            public IReactiveProperty<int> levelIndex;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveEvent<Transform> leader;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveProperty<string> winnerName;
            public ReactiveProperty<int> playersRacePlace;
        }


        private readonly Ctx _ctx;
        private readonly Material _glass;
        private readonly Material _white;
        private bool _isAlive;

        public LevelPm(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.content.ResetLevel();
            SpawnRockets();
            SpawnFinish();
            SpawnFloors();
            SpawnIoPlayers();
            AddUnsafe(_ctx.floorPart.SubscribeWithSkip(DeleteFloorPart));
            AddUnsafe(_ctx.roofPart.SubscribeWithSkip(DeleteRoofPart));
            AddUnsafe(_ctx.destroy.Subscribe(DestCells));
            _glass = _ctx.content.GetGlassMat();
            _white = _ctx.content.GetWhiteMat();
            AddUnsafe(_ctx.gameState.Subscribe(GameStateReceiver));
        }

        private void SpawnRockets()
        {
            foreach (var jumperEntity in _ctx.Level.jumprs.Select(jumper => new JumperEntity.Ctx()
                     {
                         jumper = jumper,
                         view = _ctx.content.GetJumper(jumper.type),
                         blocksContainer = _ctx.blocksContainer,
                         emptyCell = _ctx.content.GetEmptyCell().gameObject
                     }).Select(jumperEntityCtx => new JumperEntity(jumperEntityCtx)))
            {
                AddUnsafe(jumperEntity);
            }
        }

        private void SpawnFinish()
        {
            var finishEntityCtx = new FinishEntity.Ctx()
            {
                player = _ctx.player,
                view = _ctx.content.GetFinish().GetComponent<FinishView>(),
                gameState = _ctx.gameState,

            };
            var finishEntity = new FinishEntity(finishEntityCtx, _ctx.Level.finishPosiiton);
            AddUnsafe(finishEntity);
        }

        private void SpawnFloors()
        {
            var startPoint = Vector3.zero;
            var l = 0;
            foreach (var floor in _ctx.Level.floors)
            {
                startPoint = PointerOffset(floor.FloorType, startPoint);
                MakeFloorCells(floor, _ctx.Level._container.CellCount(floor.FloorType), startPoint, l == _ctx.Level.floors.Count - 1);
                startPoint = new Vector3(startPoint.x, startPoint.y + 15, startPoint.z);
                l++;
            }
        }

        private void SpawnIoPlayers()
        {
            foreach (var player in _ctx.Level.ioplayers)
            {
                var ioPlayerEntityCtx = new IoPLayerEntity.Ctx()
                {
                    gameState = _ctx.gameState,
                    view = _ctx.content.GetIoPlayer(player.type),
                    _ioPlayer = player,
                    roofPart = _ctx.roofPart,
                    floorPart = _ctx.floorPart,
                    levelIndex = _ctx.levelIndex,
                    camera = _ctx.camera,
                    ioName = _ctx.content.GetIoName(),
                    skinMat = _ctx.content.GetManMaterial(),
                    players = _ctx.players,
                    leader = _ctx.leader,
                    winnerName = _ctx.winnerName
                };
                var ioPlayerEntity = new IoPLayerEntity(ioPlayerEntityCtx);
                AddUnsafe(ioPlayerEntity);
            }
        }

        private void MakeFloorCells(LevelContainer.Floor floor, int size, Vector3 startPoint, bool last)
        {
            var point = startPoint;
            Object.Instantiate(_ctx.content.GetNavFloor(floor.FloorType), point, Quaternion.identity, _ctx.blocksContainer);
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var cell = _ctx.Level._container.GetCellInfo(floor, j, i, out int index);
                    if (cell.type == CellType.simple)
                    {
                       var c=  Object.Instantiate(_ctx.content.GetCell(floor.color), point, Quaternion.identity, _ctx.blocksContainer);
                        if (last) c.layer = 11;
                    }

                    if (cell.type == CellType.empty)
                        Object.Instantiate(_ctx.content.GetEmptyCell(), point, Quaternion.identity, _ctx.blocksContainer);
                    if (cell.type == CellType.jumper)
                    {
                        var jumperEntityCtx = new JumperEntity.Ctx()
                        {
                            view = _ctx.content.GetJumper(JumperType.medium),
                            position = point
                        };
                        var jumperEntity = new JumperEntity(jumperEntityCtx);
                        AddUnsafe(jumperEntity);
                    }
                    point = new Vector3(point.x+1, point.y, point.z);

                }
                point = new Vector3(startPoint.x, startPoint.y, point.z + 1);

            }
            ActivateLod(false);
        }

        private void ActivateLod(bool state)
        {
            var allLod = GameObject.FindObjectsOfType<LODGroup>();
            allLod.ToList().ForEach(x => x.enabled = state);
        }

        private async void ShowIO()
        {
            await UniTask.Delay(100);
            var plCount = _ctx.players.Value.Count+1;
            var interval = 3000 / plCount;
            var i = 0;
            while (i < plCount-1)
            {
                await UniTask.Delay(interval);
                var person = _ctx.players.Value[i];
                if (person != _ctx.player.Value) person.gameObject.SetActive(true);
                i++;
            }
        }

        private void DeleteRoofPart(GameObject part)
        {
            part.SetActive(false);
        }
        private void DeleteFloorPart(GameObject part)
        {
            try
            {
                SetWhite(part.transform.GetChild(1).GetComponent<MeshRenderer>());
            }
            catch { }
           
        }

        private async void SetWhite(MeshRenderer m)
        {
            if (!await Waiter(250)) return;
            if (m == null) return;
            m.material = _white;
            SetGlass(m);
        }

        private async void SetGlass(MeshRenderer m)
        {
            if (!await Waiter(250)) return;
            if (m == null) return;
            m.material = _glass;
            DelOne(m.gameObject);
        } 

        private async void DelOne(GameObject o)
        {
            if (!await Waiter(125)) return;
            try
            {
                Object.Instantiate(_ctx.content.GetEmptyCell(), o.transform.position, Quaternion.identity, _ctx.blocksContainer);
                Object.Destroy(o.transform.parent.gameObject);
            }
            catch { };
        }
            
        private async Task<bool> Waiter(int time)
        {
            try
            {
                await UniTask.Delay(time);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Vector3 PointerOffset(FloorType floorType, Vector3 point)
        {
            return floorType switch
            {
                FloorType.cells30x30 => new Vector3(-8, point.y, -8),
                FloorType.cells25x25 => new Vector3(-5.5f, point.y, -5.5f),
                _ => new Vector3(0, point.y, 0)
            };
        }

        private void DestCells()
        {
            foreach (Transform child in _ctx.blocksContainer)
            {
                Object.Destroy(child.gameObject);
            }
        }

        private async void GetLeader()
        {
            var leaders = new List<Leader>();
            while (_isAlive)
            {
                foreach (var p in _ctx.players.Value)
                {
                    var d = Vector3.Distance(p.position, _ctx.Level.finishPosiiton);
                    leaders.Add(new Leader(p, d));
                }

                leaders = leaders.OrderBy(w => w.dist).ToList();
                _ctx.leader.Notify(leaders[0].player);
                for (var i = 0; i < leaders.Count; i++)
                {
                    if (leaders[i].player == _ctx.player.Value)
                        _ctx.playersRacePlace.Value = i + 1;            
                }
                leaders.Clear();
                await UniTask.Yield();
            }
            
        }

        private void GameStateReceiver(GameState state)
        {
            switch (state)
            {
                case GameState.PLAY:
                    _isAlive = true;
                    ActivateLod(true);
                    GetLeader();
                    break;
                case GameState.COUNTER:
                    ShowIO();
                    break;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _isAlive = false;
        }

    }
}

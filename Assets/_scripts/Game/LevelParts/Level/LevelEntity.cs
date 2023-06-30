using System.Collections.Generic;
using JumpUp;
using JumpUp.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace _scripts.Game.LevelParts.Level
{
    public class LevelEntity : BaseDisposable
    {
        public struct Ctx
        {
            public IContent content;   
            public IReactiveProperty<GameState> gameState;  
            public Level Level;
            public ReactiveProperty<Transform> player;
            public ReactiveEvent<Vector2> moveCoor;
            public ReactiveEvent<GameObject> floorPart;
            public ReactiveEvent<GameObject> roofPart;
            public Transform otherTransform;
            public ReactiveTrigger destroy;
            public IReactiveProperty<int> levelIndex;
            public Transform otherCanvas;
            public ReactiveProperty<UnityEngine.Camera> camera;
            public ReactiveProperty<List<Transform>> players;
            public ReactiveEvent<Transform> leader;
            public ReactiveProperty<string> winnerName;
            public ReactiveProperty<int> playersRacePlace;
        }

        private Ctx _ctx;
        private readonly LevelPm _levelPm;

        public LevelEntity(Ctx ctx)
        {
            _ctx = ctx;
           
            var levelPmCtx = new LevelPm.Ctx()
            {
                content = _ctx.content,
                gameState = _ctx.gameState,
                Level = _ctx.Level,
                player = _ctx.player,
                moveCoor = _ctx.moveCoor,
                floorPart = _ctx.floorPart,
                roofPart = _ctx.roofPart,
                otherTransform = _ctx.otherTransform,
                destroy = _ctx.destroy,
                levelIndex = _ctx.levelIndex,
                otherCanvas = _ctx.otherCanvas,
                camera = _ctx.camera,
                players = _ctx.players,
                leader = _ctx.leader,
                winnerName = _ctx.winnerName,
                playersRacePlace = _ctx.playersRacePlace
            };
            _levelPm = new LevelPm(levelPmCtx);

            AddUnsafe(_levelPm);
        }


       

    }
}
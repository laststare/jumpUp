using System.Collections.Generic;
using CodeBase.Content;
using JumpUp;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Level
{
    public class LevelEntity : BaseDisposable
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
                floorPart = _ctx.floorPart,
                roofPart = _ctx.roofPart,
                blocksContainer = _ctx.blocksContainer,
                destroy = _ctx.destroy,
                levelIndex = _ctx.levelIndex,
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
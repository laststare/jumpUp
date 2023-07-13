using System.Collections.Generic;
using CodeBase.Content;
using JumpUp.External;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Level
{
    public class LevelEntity : BaseDisposable
    {
        public struct Context
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

        private Context _context;
        private readonly LevelPm _levelPm;

        public LevelEntity(Context context)
        {
            _context = context;
           
            var levelPmContext = new LevelPm.Context()
            {
                content = _context.content,
                gameState = _context.gameState,
                Level = _context.Level,
                player = _context.player,
                floorPart = _context.floorPart,
                roofPart = _context.roofPart,
                blocksContainer = _context.blocksContainer,
                destroy = _context.destroy,
                levelIndex = _context.levelIndex,
                camera = _context.camera,
                players = _context.players,
                leader = _context.leader,
                winnerName = _context.winnerName,
                playersRacePlace = _context.playersRacePlace
            };
            _levelPm = new LevelPm(levelPmContext);

            AddUnsafe(_levelPm);
        }


       

    }
}
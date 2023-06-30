﻿using JumpUp.External;
using JumpUp.Content;
using System.Collections;
using System.Collections.Generic;
using _scripts.Game.Ads;
using _scripts.UI;
using _scripts.UI.Input;
using UnityEngine;
using UniRx;

namespace JumpUp
{
    public class RootEntity : BaseDisposable
    {
        public struct Ctx
        {
            public PrefabsInfo prefabs;
            public GameInfoView gameInfoView;
            public InputView inputview;
            public Transform tutor;
            public DigitalRubyShared.FingersJoystickScript controll;
            public Transform otherTransform;
            public Transform otherCanvas;
        }

        private readonly Ctx _ctx;
        private GameEntity _gameEntity;
        private UIEntity _uIEntity;
        private AdsEntity _adsEntity;
        private IContent _contentLoader;

        private readonly ReactiveProperty<GameState> _gameState;
        private readonly ReactiveProperty<int> _levelIndex;
        private readonly ReactiveProperty<int> _levelCounter;
        private readonly ReactiveEvent<Vector2> _moveCoor;
        private readonly ReactiveTrigger _start;
        private readonly ReactiveTrigger _gameOver;
        private readonly ReactiveTrigger _finish;
        private readonly ReactiveProperty<Transform> _player;
        private readonly ReactiveTrigger _onClick;
        private readonly ReactiveTrigger _showTutor; 
        private readonly ReactiveProperty<bool> _needBigTutor;
        private readonly ReactiveProperty<string> _winnerName; 
        private readonly ReactiveTrigger _startGame;

        private readonly ReactiveProperty<int> _playersRacePlace;
        //ads
        private readonly ReactiveTrigger _reloadAds;
        private readonly ReactiveProperty<bool> _interShowTime; 

        public RootEntity(Ctx ctx)
        {
            _ctx = ctx;
            _moveCoor = new ReactiveEvent<Vector2>();
            _start =  new ReactiveTrigger();
            _gameOver = new ReactiveTrigger();
            _finish = new ReactiveTrigger();
            _gameState = new ReactiveProperty<GameState>();
            var level = PlayerPrefs.GetInt("level");
            _levelIndex = new ReactiveProperty<int>(level);
            var levelCounter = PlayerPrefs.GetInt("levelCounter");
            _levelCounter = new ReactiveProperty<int>(levelCounter);
            _player = new ReactiveProperty<Transform>();
            _onClick = new ReactiveTrigger();
            _showTutor = new ReactiveTrigger();
            _winnerName = new ReactiveProperty<string>();
            _needBigTutor = new ReactiveProperty<bool>(PlayerPrefs.GetInt("bigTutor") == 0);
            _playersRacePlace = new ReactiveProperty<int>();
            _startGame = new ReactiveTrigger();
            _reloadAds = new ReactiveTrigger();
            _interShowTime =  new ReactiveProperty<bool>();
            Init();
        }

        private void Init()
        {
            CreateContentLoader();
            CreateUIEntity();
            CreateGameEntity();
            CreateAdsEntity();
            _gameOver.Notify();  
        }

        private void CreateContentLoader()
        {
            var contentLoaderCtx = new ContentLoader.Ctx
            {
                Prefabs = _ctx.prefabs
            };
            _contentLoader = new ContentLoader(contentLoaderCtx);
            AddUnsafe(_contentLoader);
        }

        private void CreateGameEntity()
        {
            var gameEntityCtx = new GameEntity.Ctx
            {
                contentLoader = _contentLoader,
                levelIndex = _levelIndex,
                gameState = _gameState,
                Start = _start,
                GameOver = _gameOver,
                Finish = _finish,
                moveCoor = _moveCoor,
                player = _player,
                _onClick = _onClick,
                tutor = _ctx.tutor,
                controll = _ctx.controll,
                otherTransform = _ctx.otherTransform,
                otherCanvas = _ctx.otherCanvas,
                showTutor = _showTutor,
                needBigTutor = _needBigTutor,
                winnerName = _winnerName,
                startGame = _startGame,
                interShowTime = _interShowTime,
                ReloadAds = _reloadAds,
                playersRacePlace = _playersRacePlace
            };
            _gameEntity = new GameEntity(gameEntityCtx);
            AddUnsafe(_gameEntity);
        }

        private void CreateUIEntity()
        {
            var uiEntityCtx = new UIEntity.Ctx()
            {
                gameState = _gameState,
                _levelIndex = _levelIndex,
                inputview = _ctx.inputview,
                moveCoor = _moveCoor,
                _onClick = _onClick,
                gameInfoView = _ctx.gameInfoView,
                player = _player,
                controll = _ctx.controll,
                showTutor = _showTutor,
                winnerName = _winnerName,
                go = _startGame,
                levelCounter = _levelCounter,
                playersRacePlace = _playersRacePlace
            };
            _uIEntity = new UIEntity(uiEntityCtx);
            AddUnsafe(_uIEntity);
        
        }

        private void CreateAdsEntity()
        {
            var adsEntityCtx = new AdsEntity.Ctx()
            {
                content = _contentLoader,
                Finish = _finish,
                ReloadAds = _reloadAds,
                levelIndex = _levelIndex,
                levelCounter = _levelCounter,
                interShowTime = _interShowTime
            };
            _adsEntity = new AdsEntity(adsEntityCtx);
            AddUnsafe(_adsEntity);
        }
    }
}

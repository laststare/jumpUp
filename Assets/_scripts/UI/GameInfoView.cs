using System.Collections.Generic;
using System.Threading.Tasks;
using GameAnalyticsSDK;
using JumpUp;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _scripts.UI
{
    public class GameInfoView : MonoBehaviour
    {
        private Text tx;
        [SerializeField] 
        private Text downSideText, levelnumText, counetrTx,  winLoseTx, winnerName, playersRacePlace;
        [SerializeField]
        private GameObject  waitingText, bigTutortx, complete, backDown, backUp;
        private int fakeLevelCounter;
        public struct Ctx
        {
            public IReactiveProperty<GameState> gameState;
            public IReadOnlyReactiveProperty<int> _levelIndex;
            public ReactiveTrigger showTutor;
            public ReactiveProperty<string> winnerName;
            public ReactiveTrigger go;
            public ReactiveProperty<int> levelCounter;
            public ReactiveProperty<int> playersRacePlace;
        }

        private Ctx _ctx;
        private int _level, _time;
        private bool _counting;
        private string _currenLevel, _winnerName;
        private Animator _anim;
        private IYandexAppMetrica _appmetrica;

        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.gameState.Subscribe(ReactGameState).AddTo(this);
            _ctx._levelIndex.Subscribe(LevelConunter).AddTo(this);
            _ctx.winnerName.Subscribe(SetWinner);
            _ctx.playersRacePlace.Subscribe(x => playersRacePlace.text = NumConverter(x)).AddTo(this);
            tx = GetComponent<Text>();
            _anim = GetComponent<Animator>();
            _ctx.go.Subscribe(DeactivateDownStripe);
            _appmetrica = AppMetrica.Instance;
            GameAnalytics.Initialize();
        }

        private void DeactivateDownStripe() => backDown.SetActive(false);

        private void SetWinner(string winner) => _winnerName = $"{winner} WINS";

        private void LevelConunter(int i)
        {
            _level = PlayerPrefs.GetInt("level");
            _currenLevel = $"LEVEL {_level +1}";
        }


        private void ReactGameState(GameState _state)
        {
            switch (_state)
            {
                case GameState.START:
                    _anim.enabled = true;
                    backDown.SetActive(true);
                    tx.text = "TAP TO PLAY";
                    complete.SetActive(false);
                    backUp.SetActive(false);
                    _ctx.levelCounter.Value++;
                    PlayerPrefs.SetInt("levelCounter", _ctx.levelCounter.Value);
                    break;
                case GameState.BIGTUTOR:
                    levelnumText.text = "";
                    _anim.enabled = false;
                    tx.text = "";
                    bigTutortx.SetActive(true);
                    break;
                case GameState.COUNTER:
                    levelnumText.text = _currenLevel;
                    _anim.enabled = false;
                    tx.text = "";
                    winnerName.text = "";
                    _winnerName = "";
                    bigTutortx.SetActive(false);
                    ShowWaiting();
                    break;
                case GameState.PLAY:
                    playersRacePlace.enabled = true;
                    _anim.enabled = false;
                    tx.text = "";
                    TimeCounter();
                    //Analytics
                    SendLevelStartEvent();
                    //GA
                    SendGAProgressionEvent(GAProgressionStatus.Start);
                    break;
                case GameState.FINISH:
                    complete.SetActive(true);
                    backDown.SetActive(true);
                    winLoseTx.text = "YOU WIN!";
                    _anim.enabled = true;
                    tx.text = "TAP TO NEXT";
                    playersRacePlace.enabled = false;
                    winnerName.text = $"LEVEL {_level + 1} COMPLETE";
                    //Analytics
                    SendLevelFinishEvent("win");
                    //GA
                    _counting = false;
                    //GA
                    SendGAProgressionEvent(GAProgressionStatus.Complete);
                    break;
                case GameState.GAMEOVER:
                    playersRacePlace.enabled = false;
                    complete.SetActive(true);
                    winLoseTx.text = "YOU LOSE!";                
                    _anim.enabled = true;
                    tx.text = "TAP TO REPLAY";
                    winnerName.text = _winnerName;
                    backDown.SetActive(true);


                    //Analytics
                    _counting = false;
                    //Analytics
                    SendLevelFinishEvent("lose");
                    //GA
                    SendGAProgressionEvent(GAProgressionStatus.Fail);
                    break;
            }
        }


        private void SendLevelFinishEvent(string result)
        {
            var eventParameters = new Dictionary<string, object>();
            var _level = CalcLevel(_ctx._levelIndex.Value);
            eventParameters["level_number"] = _level;
            eventParameters["level_name"] = _level;
            eventParameters["level_count"] = _ctx.levelCounter.Value;
            eventParameters["level_diff"] = "standart";
            eventParameters["level_loop"] = CalcLoop(_ctx._levelIndex.Value);
            // eventParameters["continue"] = _continue;
            eventParameters["result"] = result;
            eventParameters["time"] = Mathf.CeilToInt(_time);

            //eventParameters["progress"] = PersantageProgress();
            _appmetrica.ReportEvent("level_finish", eventParameters);
            _appmetrica.SendEventsBuffer();
        }

        private void SendLevelStartEvent()
        {
            var eventParameters = new Dictionary<string, object>();
            var _level = CalcLevel(_ctx._levelIndex.Value);
            eventParameters["level_number"] = _level;
            eventParameters["level_name"] = _level;
            eventParameters["level_count"] = _ctx.levelCounter.Value;
            eventParameters["level_diff"] = "standart";
            eventParameters["level_loop"] = CalcLoop(_ctx._levelIndex.Value);
            _appmetrica.ReportEvent("level_start", eventParameters);
            _appmetrica.SendEventsBuffer();
        }

        private void SendGAProgressionEvent(GAProgressionStatus status)
        {
            var p = _level + 1;
            GameAnalytics.NewProgressionEvent(status, p.ToString());
        }

        private async void ShowWaiting()
        {
            waitingText.SetActive(true);
            await Task.Delay(3000);
            waitingText.SetActive(false);
            _ctx.showTutor.Notify();
            backUp.SetActive(true);
            counetrTx.text = "3";
            await Task.Delay(1000);
            counetrTx.text = "2";
            await Task.Delay(1000);
            counetrTx.text = "1";
            await Task.Delay(1000);
            counetrTx.text = "GO!";
            _ctx.gameState.Value = GameState.PLAY;
            await Task.Delay(1000);
            counetrTx.text = "";
            backUp.SetActive(false);

        }


        void OnApplicationQuit()
        {
            SendLevelFinishEvent("leave");
            _counting = false;
        }


        private async Task TimeCounter()
        {
            _time = 0;
            _counting = true;
            while (_counting)
            {
                _time++;
                await Task.Delay(1000);
            }
        }

        private int CalcLoop(int i)
        {
            var cel = i / _ctx.levelCounter.Value;
            return cel + 1;
        }

        private int CalcLevel(int i)
        {
            var cel = i / _ctx.levelCounter.Value;
            var ost = i - cel * _ctx.levelCounter.Value;
            return ost + 1;
        }

        private string NumConverter(int n)
        {
            return n switch
            {
                1 => $"{n}ST",
                2 => $"{n}ND",
                3 => $"{n}RD",
                _ => $"{n}TH"
            };
        }


    }
}

using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class GameInfoView : MonoBehaviour
    {
        private Text _centralText;
        [SerializeField] 
        private Text downSideText, levelnumText, counetrTx,  winLoseTx, winnerName, playersRacePlace;
        [SerializeField]
        private GameObject  waitingText, bigTutortx, complete, backDown, backUp;
        private int _fakeLevelCounter;
        public struct Ctx
        {
            public IReactiveProperty<GameState> gameState;
            public IReadOnlyReactiveProperty<int> _levelIndex;
            public ReactiveTrigger showTutor;
            public ReactiveProperty<string> winnerName;
            public ReactiveTrigger startGame;
            public ReactiveProperty<int> levelCounter;
            public ReactiveProperty<int> playersRacePlace;
        }

        private Ctx _ctx;
        private int _level, _time;
        private bool _counting;
        private string _currenLevel, _winnerName;
        private Animator _anim;

        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            _centralText = GetComponent<Text>();
            _anim = GetComponent<Animator>();
            _ctx.gameState.Subscribe(ReactGameState).AddTo(this);
            _ctx._levelIndex.Subscribe(LevelCounter).AddTo(this);
            _ctx.winnerName.Subscribe(SetWinner).AddTo(this);
            _ctx.playersRacePlace.Subscribe(x => playersRacePlace.text = NumConverter(x)).AddTo(this);
            _ctx.startGame.Subscribe(DeactivateDownStripe).AddTo(this);
        }

        private void DeactivateDownStripe() => backDown.SetActive(false);

        private void SetWinner(string winner) => _winnerName = $"{winner} WINS";

        private void LevelCounter(int i)
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
                    _centralText.text = "TAP TO PLAY";
                    complete.SetActive(false);
                    backUp.SetActive(false);
                    _ctx.levelCounter.Value++;
                    PlayerPrefs.SetInt("levelCounter", _ctx.levelCounter.Value);
                    break;
                case GameState.BIGTUTOR:
                    levelnumText.text = "";
                    _anim.enabled = false;
                    _centralText.text = "";
                    bigTutortx.SetActive(true);
                    break;
                case GameState.COUNTER:
                    levelnumText.text = _currenLevel;
                    _anim.enabled = false;
                    _centralText.text = "";
                    winnerName.text = "";
                    _winnerName = "";
                    bigTutortx.SetActive(false);
                    ShowWaiting();
                    break;
                case GameState.PLAY:
                    playersRacePlace.enabled = true;
                    _anim.enabled = false;
                    _centralText.text = "";
                    break;
                case GameState.FINISH:
                    complete.SetActive(true);
                    backDown.SetActive(true);
                    winLoseTx.text = "YOU WIN!";
                    _anim.enabled = true;
                    _centralText.text = "TAP TO NEXT";
                    playersRacePlace.enabled = false;
                    winnerName.text = $"LEVEL {_level + 1} COMPLETE";
                    break;
                case GameState.GAMEOVER:
                    playersRacePlace.enabled = false;
                    complete.SetActive(true);
                    winLoseTx.text = "YOU LOSE!";                
                    _anim.enabled = true;
                    _centralText.text = "TAP TO REPLAY";
                    winnerName.text = _winnerName;
                    backDown.SetActive(true);
                    break;
            }
        }

        
        

        private async void ShowWaiting()
        {
            waitingText.SetActive(true);
            await UniTask.Delay(3000);
            waitingText.SetActive(false);
            _ctx.showTutor.Notify();
            backUp.SetActive(true);
            counetrTx.text = "3";
            await UniTask.Delay(1000);
            counetrTx.text = "2";
            await UniTask.Delay(1000);
            counetrTx.text = "1";
            await UniTask.Delay(1000);
            counetrTx.text = "GO!";
            _ctx.gameState.Value = GameState.PLAY;
            await UniTask.Delay(1000);
            counetrTx.text = "";
            backUp.SetActive(false);
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

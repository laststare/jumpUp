using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class GameInfoView : MonoBehaviour
    {
        [SerializeField]
        private Text centralText;
        public struct Ctx
        {
            public IReactiveProperty<GameState> gameState;
        }

        private Ctx _ctx;
        
        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.gameState.Subscribe(ReactGameState).AddTo(this);
        }

        private void ReactGameState(GameState _state)
        {
            switch (_state)
            {
                case GameState.START:
                    gameObject.SetActive(true);
                    centralText.text = "TAP TO PLAY";
                    break;
                case GameState.STARTTUTOR:
                    gameObject.SetActive(false);
                    break;
                case GameState.COUNTER:
                    gameObject.SetActive(true);
                    centralText.text = "WAITING FOR PLAYERS";
                    CloseAfterTime(3000);
                    break;
                case GameState.PLAY:
                    gameObject.SetActive(false);
                    break;
                case GameState.FINISH:
                    gameObject.SetActive(true);
                    centralText.text = "TAP TO NEXT";
                    break;
                case GameState.GAMEOVER:
                    gameObject.SetActive(true);
                    centralText.text = "TAP TO REPLAY";
                    break;
            }
        }

        private async void CloseAfterTime(int time)
        {
            await UniTask.Delay(time);
            gameObject.SetActive(false);
        }
        
        
    }
}

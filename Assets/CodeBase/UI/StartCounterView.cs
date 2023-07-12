using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class StartCounterView : MonoBehaviour
    {
        public struct Ctx
        {
            public IReactiveProperty<GameState> gameState;
            public ReactiveTrigger showEndlessTutor;
            public ReactiveTrigger countingIsOver;
        }

        private Ctx _ctx;
        private Text _counterText;
        
        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            _counterText = transform.GetChild(0).GetComponent<Text>();
            _ctx.gameState.Subscribe(x =>
            {
                if (x == GameState.COUNTER) ShowCounting();
            }).AddTo(this);
        }

        private async void ShowCounting()
        {
            await UniTask.Delay(3000);
            _ctx.showEndlessTutor.Notify();
            gameObject.SetActive(true);
            _counterText.text = "3";
            await UniTask.Delay(1000);
            _counterText.text = "2";
            await UniTask.Delay(1000);
            _counterText.text = "1";
            await UniTask.Delay(1000);
            _counterText.text = "GO!";
            _ctx.countingIsOver.Notify();
            _ctx.gameState.Value = GameState.PLAY;
            await UniTask.Delay(1000);
            _counterText.text = "";
            gameObject.SetActive(false);
        }
    }
}
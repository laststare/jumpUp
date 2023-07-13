using Cysharp.Threading.Tasks;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class StartCounterView : MonoBehaviour
    {
        public struct Context
        {
            public IReactiveProperty<GameState> gameState;
            public ReactiveTrigger showEndlessTutor;
            public ReactiveTrigger countingIsOver;
        }

        private Context _context;
        private Text _counterText;
        
        public void Init(Context context)
        {
            _context = context;
            _counterText = transform.GetChild(0).GetComponent<Text>();
            _context.gameState.Subscribe(x =>
            {
                if (x == GameState.COUNTER) ShowCounting();
            }).AddTo(this);
        }

        private async void ShowCounting()
        {
            await UniTask.Delay(3000);
            _context.showEndlessTutor.Notify();
            gameObject.SetActive(true);
            _counterText.text = "3";
            await UniTask.Delay(1000);
            _counterText.text = "2";
            await UniTask.Delay(1000);
            _counterText.text = "1";
            await UniTask.Delay(1000);
            _counterText.text = "GO!";
            _context.countingIsOver.Notify();
            _context.gameState.Value = GameState.PLAY;
            await UniTask.Delay(1000);
            _counterText.text = "";
            gameObject.SetActive(false);
        }
    }
}
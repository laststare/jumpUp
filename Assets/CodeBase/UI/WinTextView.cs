using System;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class WinTextView : MonoBehaviour
    {
        public struct Ctx
        {
            public IReactiveProperty<GameState> gameState;

        }

        private Ctx _ctx;
        [SerializeField]
        private Text winnerText;
        
        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.gameState.Subscribe(ShowView);
        }

        private void ShowView(GameState state)
        {
            switch (state)
            {
                case GameState.START:
                    gameObject.SetActive(false);
                    break;
                case GameState.FINISH:
                    gameObject.SetActive(true);
                    winnerText.text = "YOU WIN!";
                    break;
                case GameState.GAMEOVER:
                    gameObject.SetActive(true);
                    winnerText.text = "YOU LOSE!";  
                    break;

            }
        }
    }
}
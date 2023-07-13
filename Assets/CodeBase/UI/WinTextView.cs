using System;
using JumpUp.External;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class WinTextView : MonoBehaviour
    {
        public struct Context
        {
            public IReactiveProperty<GameState> gameState;

        }

        private Context _context;
        [SerializeField]
        private Text winnerText;
        
        public void Init(Context context)
        {
            _context = context;
            _context.gameState.Subscribe(ShowView);
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
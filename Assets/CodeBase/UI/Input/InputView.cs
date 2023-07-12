using System.Threading.Tasks;
using JumpUp.External;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI.Input
{
    public class InputView : MonoBehaviour, IPointerDownHandler
    {
        public struct Ctx
        {
            public ReactiveTrigger _onClick;
        }

        private Ctx _ctx;
        public void SetMain(Ctx Ctx)
        {
            _ctx = Ctx;
         //   Touching();   
        }

        private async Task Touching()
        {
            while (gameObject.activeSelf)
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    Touch touch = UnityEngine.Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case UnityEngine.TouchPhase.Began:
                            _ctx._onClick.Notify();                          
                            break;
                        case UnityEngine.TouchPhase.Ended:
                            break;
                    }
                }
                await Task.Yield();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _ctx._onClick.Notify();
        }
    }
}

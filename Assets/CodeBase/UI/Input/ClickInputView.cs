using System.Threading.Tasks;
using JumpUp.External;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI.Input
{
    public class ClickInputView : MonoBehaviour, IPointerDownHandler
    {
        public struct Ctx
        {
            public ReactiveTrigger onClick;
        }

        private Ctx _ctx;
        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _ctx.onClick.Notify();
        }
    }
}

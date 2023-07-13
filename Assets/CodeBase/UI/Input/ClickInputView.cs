using System.Threading.Tasks;
using JumpUp.External;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI.Input
{
    public class ClickInputView : MonoBehaviour, IPointerDownHandler
    {
        public struct Context
        {
            public ReactiveTrigger onClick;
        }

        private Context _context;
        public void Init(Context context)
        {
            _context = context;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _context.onClick.Notify();
        }
    }
}

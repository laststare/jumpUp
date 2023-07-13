using System.Threading.Tasks;
using CodeBase.Game.interfaces;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Player
{
    public class HitTriggerView : MonoBehaviour
    {
        public struct Context
        {
            public ReactiveTrigger hit;
        }

        private Context _context;
    
        public void Init(Context context)
        {
            _context = context;
        }

        private async void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponent<IBatTarget>();
            if (target == null) return;
            _context.hit.Notify();
            await UniTask.Delay(200);
            target.HitByBat(transform);
        }



   
    }
}

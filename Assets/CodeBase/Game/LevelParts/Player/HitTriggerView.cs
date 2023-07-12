using System.Threading.Tasks;
using CodeBase.Game.interfaces;
using Cysharp.Threading.Tasks;
using JumpUp.External;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Player
{
    public class HitTriggerView : MonoBehaviour
    {
        public struct Ctx
        {
            public ReactiveTrigger hit;
        }

        private Ctx _ctx;
    
        public void SetMain(Ctx ctx)
        {
            _ctx = ctx;
        }

        private async void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponent<IBatTarget>();
            if (target == null) return;
            _ctx.hit.Notify();
            await UniTask.Delay(200);
            target.HitByBat(transform);
        }



   
    }
}

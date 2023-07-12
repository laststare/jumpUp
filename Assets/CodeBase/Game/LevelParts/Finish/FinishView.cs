using System.Linq;
using CodeBase.Game.interfaces;
using JumpUp;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Finish
{
    public class FinishView : MonoBehaviour, ITrigger
    {
        public struct Ctx
        {
            public ReactiveProperty<Transform> player;
            public IReactiveProperty<GameState> gameState;
        }

        private Ctx _ctx;
        [SerializeField]
        private GameObject[] conf;
        private bool entered;
        public void SetMain(Ctx Ctx)
        {
            _ctx = Ctx;
        }
        

        private void OnTriggerEnter(Collider other)
        {
            if (entered) return;
            if (other.transform.position.y > transform.position.y+0.5f)
            Entering(other);
        }

        public void Entering(Collider other)
        {
            var finish = other.GetComponent<IFinish>();
            if (finish != null) finish.DoFinish();
            entered = true;
            conf.ToList().ForEach(x => x.SetActive(true));
        }
    }
}

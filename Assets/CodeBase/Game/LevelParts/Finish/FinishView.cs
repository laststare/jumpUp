using System.Linq;
using CodeBase.Game.interfaces;
using UniRx;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Finish
{
    public class FinishView : MonoBehaviour, ITrigger
    {
        public struct Context
        {
            public ReactiveProperty<Transform> player;
            public IReactiveProperty<GameState> gameState;
        }

        private Context _context;
        [SerializeField]
        private GameObject[] conf;
        private bool entered;
        public void Init(Context context)
        {
            _context = context;
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

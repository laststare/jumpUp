using CodeBase.Content;
using JumpUp.External;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Jumper
{
    public class JumperEntity : BaseDisposable
    {

        public struct Ctx
        {
            public LevelContainer.Jumper jumper;
            public JumperView view;
            public Vector3 position;
            public Transform blocksContainer;
            public GameObject emptyCell;
        }

        private readonly Ctx _ctx;
        private readonly JumperView _view;
        private readonly JumperPm _pm;

        public JumperEntity(Ctx Ctx)
        {
            _ctx = Ctx;

            var JumperPmDep = new JumperPm.Ctx()
            {

            };
            _pm = new JumperPm(JumperPmDep);
            AddUnsafe(_pm);


            _view = Object.Instantiate(_ctx.view, _ctx.position, Quaternion.identity);

            _view.SetMain(new JumperView.Ctx
            {
                jumper = _ctx.jumper,
                blocksContainer = _ctx.blocksContainer,
                emptyCell = _ctx.emptyCell
            });
        }


        protected override void OnDispose()
        {
            base.OnDispose();

            if (_view != null)
            {
                Object.Destroy(_view.gameObject);
            }
        }
    }
}

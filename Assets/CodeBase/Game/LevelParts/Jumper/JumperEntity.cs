using CodeBase.Content;
using JumpUp.External;
using UnityEngine;

namespace CodeBase.Game.LevelParts.Jumper
{
    public class JumperEntity : BaseDisposable
    {

        public struct Context
        {
            public LevelContainer.Jumper jumper;
            public JumperView view;
            public Vector3 position;
            public Transform blocksContainer;
            public GameObject emptyCell;
        }

        private readonly Context _context;
        private readonly JumperView _view;
        private readonly JumperPm _pm;

        public JumperEntity(Context context)
        {
            _context = context;

            var JumperPmDep = new JumperPm.Context()
            {

            };
            _pm = new JumperPm(JumperPmDep);
            AddUnsafe(_pm);


            _view = Object.Instantiate(_context.view, _context.position, Quaternion.identity);

            _view.Init(new JumperView.Context
            {
                jumper = _context.jumper,
                blocksContainer = _context.blocksContainer,
                emptyCell = _context.emptyCell
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

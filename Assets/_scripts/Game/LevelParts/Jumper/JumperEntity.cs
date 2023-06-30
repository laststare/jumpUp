using JumpUp.Content;
using JumpUp.External;
using System.Collections;
using System.Collections.Generic;
using _scripts.Content;
using UniRx;
using UnityEngine;

namespace JumpUp.Player
{
    public class JumperEntity : BaseDisposable
    {

        public struct Ctx
        {
            public LevelContainer.Jumper jumper;
            public JumperView view;
            public Vector3 position;
            public Transform otherTransform;
            public GameObject emptyCell;
        }

        private Ctx _ctx;
        private JumperView _view;
        private JumperPm _pm;

        public JumperEntity(Ctx Ctx)
        {
            _ctx = Ctx;

            JumperPm.Ctx JumperPmDep = new JumperPm.Ctx()
            {

            };
            _pm = new JumperPm(JumperPmDep);
            AddUnsafe(_pm);


            _view = Object.Instantiate(_ctx.view, _ctx.position, Quaternion.identity);

            _view.SetMain(new JumperView.Ctx
            {
                jumper = _ctx.jumper,
                otherTransform = _ctx.otherTransform,
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

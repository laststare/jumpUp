using System.Collections;
using System.Collections.Generic;
using CodeBase.Content;
using UniRx;
using UnityEngine;

public class PlayersNameView : MonoBehaviour
{
    public struct Ctx
    {
        public IContent content;
        public ReactiveProperty<Transform> _nameView;
    }

    private Ctx _ctx;
    public void SetMain(Ctx Ctx)
    {
        _ctx = Ctx;
        _ctx._nameView.Value = transform;


    }
}

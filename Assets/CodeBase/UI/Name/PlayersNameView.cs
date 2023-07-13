using System.Collections;
using System.Collections.Generic;
using CodeBase.Content;
using UniRx;
using UnityEngine;

public class PlayersNameView : MonoBehaviour
{
    public struct Context
    {
        public IContent content;
        public ReactiveProperty<Transform> _nameView;
    }

    private Context _context;
    public void Init(Context Context)
    {
        _context = Context;
        _context._nameView.Value = transform;


    }
}

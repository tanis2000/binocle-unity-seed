using System;
using System.Collections.Generic;
using Binocle;
using Binocle.Processors;
using UnityEngine;
using Prime31.ZestKit;
using System.Collections;

namespace App
{
  public enum SwipeDirection
  {
    Unknown = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Up = 1 << 2,
    Down = 1 << 3,
    Horizontal = Left | Right,
    Vertical = Up | Down,
    Cardinal = Horizontal | Vertical
  }

  public class InputProcessor : BaseEntityProcessor
  {
    private MapProcessor _mapProcessor;
    private ScaleProcessor _scaleProcessor;
    private bool _initialized = false;
    private SwipeDirection _currentSwipeDirection = SwipeDirection.Unknown;

    public InputProcessor(Matcher matcher) : base(matcher)
    {

    }

    protected override void Begin ()
		{
			base.Begin ();
      _mapProcessor = Scene.GetEntityProcessor<MapProcessor>();
      _scaleProcessor = Scene.GetEntityProcessor<ScaleProcessor>();

      if (!_initialized) {
        // Place all one-time initialization code here
        _initialized = true;
      }

      // Check if we're moving any of the gems
      var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      var hit = Physics2D.OverlapPoint(mousePosition);
		}

    public override void Process(Entity entity)
    {
      var coll = entity.GetComponent<BoxCollider2D>();
      var sr = entity.GetComponent<SpriteRenderer>();
    }


  }
}
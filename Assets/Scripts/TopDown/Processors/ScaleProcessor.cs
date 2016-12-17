using Binocle;
using Binocle.Processors;
using UnityEngine;

namespace App.TopDown
{
    public class ScaleProcessor : BaseEntityProcessor
  {
    private InputProcessor _inputProcessor;
    private MapProcessor _mapProcessor;

    public ScaleProcessor(Matcher matcher) : base(matcher)
    {
    }

		protected override void Begin ()
		{
			base.Begin ();
      _inputProcessor = Scene.GetEntityProcessor<InputProcessor>();
      _mapProcessor = Scene.GetEntityProcessor<MapProcessor>();
		}

    public override void Process(Entity entity)
    {
      TickScale(entity);
    }


    public void StartScaling(Entity entity)
    {
      if (entity == null) return;
      
      var sc = entity.GetComponent<ScaleComponent>();
      if (sc.Enabled) return;

      sc.Enabled = true;
      sc.ScaleStartTime = Time.time;
      sc.ScaleStart = Vector2.one;
      sc.ScaleEnd = new Vector2(0.3f, 0.3f);
      sc.ScaleTotalTime = 0.5f;
    }

    private void TickScale(Entity entity)
    {
      var sc = entity.GetComponent<ScaleComponent>();
      if (sc.ScaleTotalTime <= 0.0f) return;
      float scaleCompleteFraction = (Time.time - sc.ScaleStartTime) / sc.ScaleTotalTime;

      if (scaleCompleteFraction >= 1.0f)
      {
        FinishScaling(entity);
      }
      else
      {
        Vector2 newScale = Vector2.Lerp(sc.ScaleStart, sc.ScaleEnd, scaleCompleteFraction);
        entity.transform.localScale = newScale;
      }
    }

    private void FinishScaling(Entity entity)
    {
      var sc = entity.GetComponent<ScaleComponent>();
      entity.transform.localScale = sc.ScaleEnd;
      sc.Enabled = false;
      sc.ScaleTotalTime = 0.0f;
    }





  }
}
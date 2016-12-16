using Binocle;
using Binocle.Processors;
using UnityEngine;

namespace App
{
    public class PositionProcessor : BaseEntityProcessor
    {
        private InputProcessor _inputProcessor;
        private MapProcessor _mapProcessor;

        public PositionProcessor(Matcher matcher) : base(matcher)
        {
        }

        protected override void Begin()
        {
            base.Begin();
            _inputProcessor = Scene.GetEntityProcessor<InputProcessor>();
            _mapProcessor = Scene.GetEntityProcessor<MapProcessor>();
        }

        public override void Process(Entity entity)
        {
            Reposition(entity);
        }

        private void Reposition(Entity entity)
        {
            var pos = entity.GetComponent<PositionComponent>();
            var gp = entity.GetComponent<GridPositionComponent>();
            // If we have a grid position we also set our position
            if (gp != null) {
                pos.Position = new Vector2((-EntityFactory.MapGridSize.x / 2 + gp.Position.x) * 16 + 8, (-EntityFactory.MapGridSize.y / 2 + gp.Position.y) * 16 + 8);
            }
            entity.transform.localPosition = pos.Position;
        }






    }
}
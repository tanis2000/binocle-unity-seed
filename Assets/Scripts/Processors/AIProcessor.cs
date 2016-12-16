using System;
using System.Collections.Generic;
using Binocle;
using Binocle.Processors;
using UnityEngine;
using Prime31.ZestKit;
using System.Collections;

namespace App
{
    public class AIProcessor : BaseEntityProcessor
    {
        private MapProcessor _mapProcessor;
        private bool _initialized = false;

        public AIProcessor(Matcher matcher) : base(matcher)
        {

        }

        protected override void Begin()
        {
            base.Begin();
            _mapProcessor = Scene.GetEntityProcessor<MapProcessor>();

            if (!_initialized)
            {
                // Place all one-time initialization code here
                _initialized = true;
            }

        }

        public override void Process(Entity entity)
        {
            var unit = entity.GetComponent<UnitComponent>();
        }


    }
}
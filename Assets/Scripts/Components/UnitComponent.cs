using Binocle.Components;
using System.Collections.Generic;

namespace App
{
    public class UnitComponent : BaseMonoBehaviour
    {
        /// <summary>
        /// The type of unit
        /// </summary>
        public UnitType UnitType;

        /// <summary>
        /// The unit containing this one. If null this isn't in a container
        /// </summary>
        public UnitComponent Container;

        /// <summary>
        /// The list of units contained in this one
        /// </summary>
        public List<UnitComponent> ContainedUnits = new List<UnitComponent>();

        /// <summary>
        /// Whether this is an unit that can move on land
        /// </summary>
        public bool LandUnit;

        /// <summary>
        ///  Whether this is an unit that can fly and move anywhere
        /// </summary>
        public bool AirUnit;

        /// <summary>
        /// Whether this is an unit that can sail the sea
        /// </summary>
        public bool SailUnit;
    }
}

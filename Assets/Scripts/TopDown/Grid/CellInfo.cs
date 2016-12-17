using System;
using Binocle;

namespace App.TopDown
{
  [Serializable]
  public class CellInfo
  {
    /// <summary>
    /// The type of cell (Water, Earth, None, ...)
    /// </summary>
    public CellType CellType;
    
    /// <summary>
    /// The background tile (sprite) used by this cell
    /// </summary>
    public Entity Tile;

    /// <summary>
    /// The unit in this cell
    /// </summary>
    public UnitComponent Unit;
  }
}
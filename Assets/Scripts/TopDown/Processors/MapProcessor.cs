using Binocle;
using Binocle.Processors;
using UnityEngine;

namespace App.TopDown
{
    public class MapProcessor : BaseEntityProcessor
  {
    /// <summary>
    /// Strong reference to the only map that should be in the game.
    /// Bad design but good until I find a better option.
    /// </summary>
    public MapComponent Map;

    public MapProcessor(Matcher matcher) : base(matcher)
    {
    }

    public override void Process(Entity entity)
    {
      Map = entity.GetComponent<MapComponent>();
    }

    public Entity CellAtPosition(Vector2 pos)
    {
      if (Map == null) {
        Debug.LogError("Map has not yet been initialized.");
        return null;
      }

      var x = (int)pos.x;
      var y = (int)pos.y;
      if (x < 0 || y < 0 || x > (9-1) || y > (9-1)) {
        Debug.Log("Querying a cell outside the map [" + x + ", " + y + "]");
        return null;
      } 
      var c = Map.Cells[x, y];
      return c.Tile;
    }

    public void SetCellAtPosition(Vector2 pos, Entity entity)
    {
      if (Map == null) {
        Debug.LogError("Map has not yet been initialized.");
        return;
      }

      var x = (int)pos.x;
      var y = (int)pos.y;
      if (x < 0 || y < 0 || x > (9-1) || y > (9-1)) {
        Debug.Log("Querying a cell outside the map [" + x + ", " + y + "]");
        return;
      } 
      var c = Map.Cells[x, y];
      c.Tile = entity;
    }

  }
}
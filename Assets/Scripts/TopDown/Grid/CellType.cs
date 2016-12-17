namespace App.TopDown
{
  public enum CellType
  {
    None, // Should be ignored as that isn't a tile
    Water, // Water type, can't build on top of it
    Earth, // Earth, can build on top of it
  }
}
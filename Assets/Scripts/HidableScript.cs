using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HidableScript : MonoBehaviour {
  private bool isTilemap = false;
  private Tilemap tilemap;
  void Start() {
    isTilemap = GetComponent<Tilemap>() != null;
    if (isTilemap)
      tilemap = GetComponent<Tilemap>();
  }
  public Vector3 Hide(Vector3 point) {

    if (isTilemap) {
      Vector3Int tile = tilemap.layoutGrid.WorldToCell(point);

      tilemap.SetTileFlags(tile, TileFlags.None);
      tilemap.SetColor(tile, new Color(1, 1, 1, 0.2f));


      tile.z = 2;
      tilemap.SetTileFlags(tile, TileFlags.None);
      tilemap.SetColor(tile, new Color(1, 1, 1, 0.2f));

      tile.z = 4;
      tilemap.SetTileFlags(tile, TileFlags.None);
      tilemap.SetColor(tile, new Color(1, 1, 1, 0.2f));
      return tilemap.layoutGrid.CellToWorld(tile);
    }
    return point;
  }

  public Vector3 Show(Vector3 point) {
    if (isTilemap) {
      Vector3Int tile = tilemap.layoutGrid.WorldToCell(point);

      tilemap.SetTileFlags(tile, TileFlags.None);
      tilemap.SetColor(tile, new Color(1, 1, 1, 1));

      tile.z = 2;
      tilemap.SetTileFlags(tile, TileFlags.None);
      tilemap.SetColor(tile, new Color(1, 1, 1, 1));

      tile.z = 4;
      tilemap.SetTileFlags(tile, TileFlags.None);
      tilemap.SetColor(tile, new Color(1, 1, 1, 1));
      return tilemap.layoutGrid.CellToWorld(tile);
    }
    return point;
  }
}

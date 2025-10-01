using UnityEngine;
using UnityEngine.Tilemaps;
public class ScanMap : MonoBehaviour
{
    public Tilemap tilemap;
    public bool [,] walkable;
    private Vector3Int origin;
    private Vector3Int size;
    void Awake(){
        origin = tilemap.cellBounds.min;  // góc dưới trái
        size = tilemap.cellBounds.size; 
        walkable = new bool[size.x,size.y];
        for (int x=0;x <size.x;x++){
            for (int y=0;y <size.y;y++){
                Vector3Int cellPos = new Vector3Int(origin.x + x, origin.y + y, 0);
                TileBase tile = tilemap.GetTile(cellPos);
                walkable[x,y] = (tile == null);
            }
        }
        PrintWalkable();
    }
    void PrintWalkable()
    {
        string mapStr = "";
        for (int y = 0; y < size.y; y++) {
            for (int x = 0; x < size.x; x++) {
                if (walkable[x,y]) mapStr += ".";
                else mapStr += "#";
            }
            mapStr += "\n";
        }
        Debug.Log("ScanMap:\n" + mapStr);
    }
    public Vector2Int WorldToGrid(Vector3 worldPos){
        Vector3Int gridPos = tilemap.WorldToCell(worldPos);
        return new Vector2Int(gridPos.x-origin.x,gridPos.y-origin.y);
    }
    public Vector3 GridToWorld(Vector2Int gridPos){
        Vector3Int cellPos = new Vector3Int(gridPos.x+origin.x,gridPos.y+origin.y,0);
        return tilemap.GetCellCenterWorld(cellPos);
    }
}
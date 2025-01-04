using UnityEngine;

public class TileMapSystem : MonoBehaviour {

    [Header("Settings:")]
    public Camera cam;
    public Vector2Int mapChunks = new Vector2Int(5, 2);
    public int tilePixelWidth;
    public float brushRadius = 1;
    public int tileId = 1;
    public TileMap tileMap;
    public Transform chunkObject;

    public static TileMapSystem instance;


    private void Awake() {
        instance = this;
    }


    private void Update() {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0)) {
            if (WorldToGridPoint(cam.ScreenToWorldPoint(Input.mousePosition), out Vector2Int chunkPoint, out Vector2Int gridPoint)) {
                tileMap.chunks[chunkPoint.x, chunkPoint.y].ManipulateChunk(gridPoint, new Tile(tileId));

                //Update neighbor chunks:
                int x = gridPoint.x % TileMap.CHUNKSIZE.x;
                int y = gridPoint.y % TileMap.CHUNKSIZE.y;

                if (x == 0) {
                    if (chunkPoint.x > 0) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x - 1, chunkPoint.y].UpdateChunk();
                    }
                } else if (x == TileMap.CHUNKSIZE.x - 1) {
                    if (chunkPoint.x + 1 < tileMap.chunks.GetLength(0)) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x + 1, chunkPoint.y].UpdateChunk();
                    }
                }

                if (y == 0) {
                    if (chunkPoint.y > 0) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x, chunkPoint.y - 1].UpdateChunk();
                    }
                } else if (y == TileMap.CHUNKSIZE.y - 1) {
                    if (chunkPoint.y + 1 < tileMap.chunks.GetLength(1)) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x, chunkPoint.y + 1].UpdateChunk();
                    }
                }
            }
        }

        if (Input.GetMouseButton(1)) {
            if (WorldToGridPoint(cam.ScreenToWorldPoint(Input.mousePosition), out Vector2Int chunkPoint, out Vector2Int gridPoint)) {
                tileMap.chunks[chunkPoint.x, chunkPoint.y].ManipulateChunk(gridPoint, null);

                //Update neighbor chunks:
                int x = gridPoint.x % TileMap.CHUNKSIZE.x;
                int y = gridPoint.y % TileMap.CHUNKSIZE.y;

                if (x == 0) {
                    if (chunkPoint.x > 0) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x - 1, chunkPoint.y].UpdateChunk();
                    }
                } else if (x == TileMap.CHUNKSIZE.x - 1) {
                    if (chunkPoint.x + 1 < tileMap.chunks.GetLength(0)) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x + 1, chunkPoint.y].UpdateChunk();
                    }
                }

                if (y == 0) {
                    if (chunkPoint.y > 0) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x, chunkPoint.y - 1].UpdateChunk();
                    }
                } else if (y == TileMap.CHUNKSIZE.y - 1) {
                    if (chunkPoint.y + 1 < tileMap.chunks.GetLength(1)) { //Chunk exists?
                        tileMap.chunks[chunkPoint.x, chunkPoint.y + 1].UpdateChunk();
                    }
                }
            }
        }
    }


    private bool WorldToGridPoint(Vector2 worldPoint, out Vector2Int chunkPoint, out Vector2Int gridPoint) {
        gridPoint = new Vector2Int((int)worldPoint.x, (int)worldPoint.y);
        chunkPoint = new Vector2Int();

        //Invalid tile:
        if (gridPoint.x < 0 || gridPoint.x >= mapChunks.x * TileMap.CHUNKSIZE.x || gridPoint.y < 0 || gridPoint.y >= mapChunks.y * TileMap.CHUNKSIZE.y) {
            return false;
        }

        //Calc chunk position:
        chunkPoint.x = gridPoint.x / TileMap.CHUNKSIZE.x;
        chunkPoint.y = gridPoint.y / TileMap.CHUNKSIZE.y;

        return true;
    }
}

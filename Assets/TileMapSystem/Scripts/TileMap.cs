using UnityEngine;

public class TileMap : MonoBehaviour {

    public Tile[,] tiles;
    public Chunk[,] chunks;

    public static readonly Vector2Int CHUNKSIZE = new Vector2Int(10, 10);
    public static int TILES_PER_CHUNK;


    private void Start() {
        TILES_PER_CHUNK = CHUNKSIZE.sqrMagnitude;

        Transform chunkObject = TileMapSystem.instance.chunkObject;
        Transform chunkBuffer;

        chunks = new Chunk[TileMapSystem.instance.mapChunks.x, TileMapSystem.instance.mapChunks.y];
        tiles = new Tile[TileMapSystem.instance.mapChunks.x * CHUNKSIZE.x, TileMapSystem.instance.mapChunks.y * CHUNKSIZE.y];

        for (int x = 0; x < chunks.GetLength(0); x++) {
            for (int y = 0; y < chunks.GetLength(1); y++) {
                chunkBuffer = Instantiate(chunkObject, new Vector2(x * CHUNKSIZE.x, y * CHUNKSIZE.y), Quaternion.identity);
                chunkBuffer.name = "Chunk_" + x + "/" + y;
                chunkBuffer.parent = transform;
                chunks[x, y] = chunkBuffer.GetComponent<Chunk>();

                for (int x2 = 0; x2 < CHUNKSIZE.x; x2++) {
                    for (int y2 = 0; y2 < CHUNKSIZE.y; y2++) {
                        tiles[x * CHUNKSIZE.x + x2, y * CHUNKSIZE.y + y2] = new Tile(0);
                    }
                }
            }
        }

        //Init chunks:
        for (int x = 0; x < chunks.GetLength(0); x++) {
            for (int y = 0; y < chunks.GetLength(1); y++) {
                chunks[x, y].InitChunk(this, new Vector2Int(x * CHUNKSIZE.x, y * CHUNKSIZE.y), new Vector2Int(x * CHUNKSIZE.x + CHUNKSIZE.x, y * CHUNKSIZE.y + CHUNKSIZE.y));
            }
        }
    }

    /*public void DestroyTile(Vector2Int gridPos) {
        tiles[gridPos.x, gridPos.y] = null;
        UpdateMesh();
    }

    public void CreateTile(Vector2Int gridPos, Tile tile) {
        tiles[gridPos.x, gridPos.y] = tile;
        UpdateMesh();
    }*/
}
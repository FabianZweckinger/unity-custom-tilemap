using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour {

    public TileMap tileMap;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    private Vector2Int chunkRangeStart;
    private Vector2Int chunkRangeEnd;

    public void InitChunk(TileMap tileMap, Vector2Int chunkRangeStart, Vector2Int chunkRangeEnd) {
        this.tileMap = tileMap;
        this.chunkRangeStart = chunkRangeStart;
        this.chunkRangeEnd = chunkRangeEnd;

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        UpdateChunk();
    }

    public void UpdateChunk() {
        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[TileMap.TILES_PER_CHUNK * 4];
        int[] tris = new int[TileMap.TILES_PER_CHUNK * 6];
        Vector2[] uvs = new Vector2[TileMap.TILES_PER_CHUNK * 4];

        int vc = 0;
        int tc = 0;

        TileSet[] tileSet = TileManager.instance.tileSet;
        float perTileUVDisplacement = (float)TileMapSystem.instance.tilePixelWidth / GetComponent<MeshRenderer>().material.GetTexture("_MainTex").width;
        float perTileSetUVDisplacement = perTileUVDisplacement * 4;
        Vector2 uvBuffer;

        for (int x = chunkRangeStart.x, iX = 0; x < chunkRangeEnd.x; x++, iX++) {
            for (int y = chunkRangeStart.y, iY = 0; y < chunkRangeEnd.y; y++, iY++) {

                if (tileMap.tiles[x, y] != null) {
                    //Set vertices:
                    verts[vc] = new Vector3(iX, iY, 0);
                    verts[vc + 1] = new Vector3(iX + 1, iY, 0);
                    verts[vc + 2] = new Vector3(iX + 1, iY + 1, 0);
                    verts[vc + 3] = new Vector3(iX, iY + 1, 0);

                    //Set uvs:
                    Vector2 tileUVSetting = CalcTileUVSetting(x, y, tileMap.tiles);
                    tileUVSetting.x *= perTileUVDisplacement;
                    tileUVSetting.y *= perTileUVDisplacement;

                    uvBuffer = new Vector2(tileSet[tileMap.tiles[x, y].id].uvPos.x * perTileSetUVDisplacement + tileUVSetting.x, tileSet[tileMap.tiles[x, y].id].uvPos.y * perTileSetUVDisplacement + tileUVSetting.y);
                    uvs[vc] = new Vector2(uvBuffer.x, uvBuffer.y);
                    uvs[vc + 1] = new Vector2(uvBuffer.x + perTileUVDisplacement, uvBuffer.y);
                    uvs[vc + 2] = new Vector2(uvBuffer.x + perTileUVDisplacement, uvBuffer.y + perTileUVDisplacement);
                    uvs[vc + 3] = new Vector2(uvBuffer.x, uvBuffer.y + perTileUVDisplacement);

                    //Set triangles:
                    tris[tc] = vc;
                    tris[tc + 1] = vc + 3;
                    tris[tc + 2] = vc + 2;
                    tris[tc + 3] = vc + 0;
                    tris[tc + 4] = vc + 2;
                    tris[tc + 5] = vc + 1;

                    //Update counters:
                    vc += 4;
                    tc += 6;
                }
            }
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;

        mesh.Optimize(); //Deletes unnecessary vertices 
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

        //Update collider:
        if (mesh.vertexCount == 0) { //Collider can't have 0 verts
            meshCollider.sharedMesh = mesh;
        }
    }

    private Vector2 CalcTileUVSetting(int x, int y, Tile[,] tiles) {
        int horizontal = -1, vertical = -1;

        #region horizontal
        //First row of tiles (horizontal):
        if (x == 0) {
            if (tiles[x + 1, y] == null) {
                horizontal = 3; //Center
            } else {
                horizontal = 0; //Left
            }

            //Last row of tiles (horizontal):
        } else if (x == tiles.GetLength(0) - 1) {
            if (tiles[x - 1, y] == null) {
                horizontal = 3; //Center
            } else {
                horizontal = 2; //Right
            }
        }

        //Not first and not last row of tiles (horizontal):
        if (horizontal != 3) {
            if (x != 0 && tiles[x - 1, y] == null && x != tiles.GetLength(0) - 1 && tiles[x + 1, y] == null) {
                horizontal = 3; //Center
            } else {
                if (x != 0 && tiles[x - 1, y] == null) {
                    horizontal = 0; //Left
                } else if (x != tiles.GetLength(0) - 1 && tiles[x + 1, y] == null) {
                    horizontal = 2; //Right
                }
            }
        }

        //Every tile that has no border edges, gets converted to a filled tile:
        if (horizontal == -1) {
            horizontal = 1; //Center filled
        }
        #endregion

        #region vertical
        //First column of tiles (vertical):
        if (y == 0) {
            if (tiles[x, y + 1] != null) {
                vertical = 1; //Bottom
            } else {
                vertical = 0; //Center
            }

            //Last column of tiles (vertical):
        } else if (y == tiles.GetLength(1) - 1) {
            if (tiles[x, y - 1] != null) {
                vertical = 3; //Top 
            } else {
                vertical = 0; //Center
            }

            //Not first and not last column of tiles (vertical):
        } else if (tiles[x, y - 1] == null && tiles[x, y + 1] == null) {
            vertical = 0; //Center
        } else {
            if (y != 0 && tiles[x, y - 1] == null) {
                vertical = 1; //Bottom
            } else if (y != tiles.GetLength(1) - 1 && tiles[x, y + 1] == null) {
                vertical = 3; //Top
            }
        }

        if (vertical == -1) { //Every tile that has no border edges, gets converted to a filled tile:
            vertical = 2; //Center filled
        }
        #endregion

        return new Vector2(horizontal, vertical);
    }

    public void ManipulateChunk(Vector2Int tilePos, Tile newTile) {
        tileMap.tiles[tilePos.x, tilePos.y] = newTile;
        UpdateChunk();
    }
}

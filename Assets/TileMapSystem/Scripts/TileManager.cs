using UnityEngine;

public class TileManager : MonoBehaviour {

    public TileSet[] tileSet;
    
    public static TileManager instance;


    private void Awake() {
        instance = this;
    }
}


[System.Serializable]
public class TileSet {
    public string name = "unnamed tile";
    public Vector2Int uvPos = new Vector2Int(0,0);
    public int health = 100;
}
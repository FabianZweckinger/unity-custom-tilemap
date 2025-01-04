public class Tile {

    public int id;
    public int health;

    public Tile(int id) {
        this.id = id;
        this.health = TileManager.instance.tileSet[id].health;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    Tilemap map;
    Grid grid;
    [Range(0.1f,0.9f)]
    public float widthAdjust = 0.8f;
    [Header("TileTextures")]
    public Texture2D OpenTileTexture;
    public Texture2D CloseTileTexture;
    public Texture2D BlankTileTexture;
    public Texture2D BlockTileTexture;

    Tile OpenTile;
    Tile CloseTile;
    Tile BlankTile;
    Tile BlockTile;

    float screenWidth;
    float screenHeight;
    private Camera mainCam;
    private void Awake()
    {
        mainCam = Camera.main;
        grid = GameObject.FindObjectOfType<Grid>();
        map = GameObject.FindObjectOfType<Tilemap>();
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }
    void Start()
    {
        ResetCameraPos();
    }
    void InitTiles(float cellSize)
    {
        OpenTile = InitTile(OpenTileTexture, cellSize);
        CloseTile = InitTile(CloseTileTexture, cellSize);
        BlankTile = InitTile(BlankTileTexture,cellSize);
        BlockTile = InitTile(BlockTileTexture, cellSize);
    }


    void ResetCameraPos()
    {
        Vector3 leftButtom = mainCam.ScreenToWorldPoint(Vector3.zero);
        Vector3 diff = mainCam.transform.position - leftButtom;
        mainCam.transform.position = new Vector3(0,0,-10) + diff;
    }
    Tile InitTile(Texture2D tileTexture, float cellsize)
    {
        //Debug.Log(tileTexture.width);
        Sprite tileSprite = Sprite.Create(tileTexture, new Rect(0, 0, tileTexture.width, tileTexture.height), Vector2.one / 2, tileTexture.width/cellsize);
        Tile tile = new Tile();
        tile.sprite = tileSprite;
        return tile;
    }
    Tile InitTile(Texture2D tileTexture, int pixelAmount,float pixelsPerUnit,float cellsize)
    {
        //Debug.Log(tileTexture.width);
        Sprite tileSprite = Sprite.Create(tileTexture, new Rect(0, 0,pixelAmount, pixelAmount), Vector2.one/2, pixelsPerUnit);
        Tile tile = new Tile();
        tile.sprite = tileSprite;
        return tile;
    }

    float ResetGridSize(float width,float height,int m,int n)
    {
        float size;
        size = ((float)m / n > width / height) ? width / m : height / n;//方格偏宽or偏窄
        grid.cellSize = new Vector3(size, size);
        return size;
    }
    void GenerateMap(int m,int n,float blockPercent) //暂时用
    {
        map.ClearAllTiles();
        Vector3 rightTop = mainCam.ScreenToWorldPoint(new Vector3(screenWidth * widthAdjust, screenHeight));
        float width = rightTop.x;
        float height = rightTop.y;
        Vector3Int FirstTilePos = map.WorldToCell(Vector3.zero);
        float cellSize = ResetGridSize(width, height, m, n);
        InitTiles(cellSize);
        map.SetTile(FirstTilePos,CloseTile);
        int i;
        int j = 1;
        // Vector3Int currentGenerateTilepos = FirstTilePos;
        Debug.Log(Time.realtimeSinceStartup);
        for (i = 0; i < n; i++)
        {
            for (; j < m; j++)
            {
                map.SetTile(FirstTilePos + new Vector3Int(j, i, 0),BlankTile);
            }
            j = 0;
        }
        Debug.Log(Time.realtimeSinceStartup);

        /*
        map.ClearAllTiles();
        Vector3Int[] varray = new Vector3Int[n * m];
        Tile[] tilearray = new Tile[n * m];
        int mm = 0;
        for (i = 0; i < n; i++)
        {
            for (j=0; j < m; j++)
            {

                varray[mm] = new Vector3Int(m, n,0);
                tilearray[mm] = CurrentTile;
                mm++;
            }
        }
        map.SetTiles(varray, tilearray);
        Debug.Log(Time.realtimeSinceStartup);
        */
    }

    public void CleanMap()
    {
        map.ClearAllTiles();
    }
    public void InitCellsize(int width,int height)
    {
        map.ClearAllTiles();
        Vector3 rightTop = mainCam.ScreenToWorldPoint(new Vector3(screenWidth * widthAdjust, screenHeight));
        float worldWidth = rightTop.x;
        float worldHeight = rightTop.y;
        Vector3Int FirstTilePos = map.WorldToCell(Vector3.zero);
        float cellSize = ResetGridSize(worldWidth, worldHeight, width, height);
        InitTiles(cellSize);
    }
    public void Draw(int x,int y,TileType type)
    {
        map.SetTile(new Vector3Int(x, y, 0), TypeTranslate(type));
    }

    Tile TypeTranslate(TileType type)
    {
        switch (type)
        {
            case TileType.Blank:
                return BlankTile;
            case TileType.Block:
                return BlockTile;
            case TileType.Close:
                return CloseTile;
            case TileType.Open:
                return OpenTile;
            default:
                return null;
        }

    }
}

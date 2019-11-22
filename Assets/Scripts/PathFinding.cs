using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class PathFinding : MonoBehaviour
{
    public Vector2Int startPos;
    public Vector2Int nextPos;
    public Vector2Int endPos;
    public InputField widthField, heightField;
    public int width, height;
    public bool isFinding;
    [Range(0, 0.5f)]
    public float blockPercent;
    SingleGrid[,] grids;
    TileManager tileManager;
    private List<SingleGrid> OpenList = new List<SingleGrid>();
    private List<SingleGrid> ClosedList = new List<SingleGrid>();
    private void Awake()
    {
        tileManager = this.GetComponent<TileManager>();
    }

    private void Update()
    {
        if(!isFinding)
        { return; }
        for (int i=0; i < 10; i++)
        {
            if (OpenList.Count > 0)
            {
                step();
            }
        }
    }
    public void GenerateButton_OnClick()
    {
        if (widthField.text == null || widthField.text == "")
        {
            return;
        }
        if (heightField.text == null || heightField.text == "")
        {
            return;
        }
        width = int.Parse(widthField.text);
        width = Mathf.Min(width, 500);
        height = int.Parse(heightField.text);
        height = Mathf.Min(height, 500);
        startPos = Vector2Int.zero;
        endPos = new Vector2Int(width, height);
        //tileManager.GenerateMap(m, n,obstaclePercent);
        GenerateMap(width, height, blockPercent);
    }

    public void StartButton_OnClick()
    {
        isFinding = true;
    }

    void GenerateMap(int width,int height,float blockPercent) //m:width ; n:height
    {
        endPos = new Vector2Int(width-1, height-1);
        tileManager.InitCellsize(width, height);
        grids=new SingleGrid[width, height];
        OpenList.Clear();
        ClosedList.Clear();
        isFinding = false;

        for(int i=0;i<width;i++)
        {
            for(int j=0;j<height;j++)
            {
                grids[i, j] = new SingleGrid();
                if (Random.Range(0,1f)<blockPercent)
                {
                    grids[i, j].isBlock = true;
                    tileManager.Draw(i, j, TileType.Block);
                    grids[i, j].Position = new Vector2Int(i, j);
                }
                else
                {
                    grids[i, j].isBlock = false;
                    tileManager.Draw(i, j, TileType.Blank);
                    grids[i, j].Position = new Vector2Int(i, j);
                    grids[i, j].Assume = endPos.x - i + endPos.y - j;
                }
            }
        }
        grids[0, 0].isBlock = false;
        grids[0, 0].Assume = endPos.x + endPos.y;
        //tileManager.Draw(0, 0, TileType.Open);
        AddIntoOpenList(grids[0, 0],grids[0,0]);

        grids[width - 1, height - 1].isBlock = false;
        tileManager.Draw(width - 1, height - 1, TileType.Blank);
    }

    /*
    void step(int i,int j)//visit grids around it from a single step 
    {
        if (grids[i, j].isBlock)
        { return; }
        if (grids[i, j].isInClosedList)
        { return; }
        if (!grids[i, j].isInOpenList)
        { return; }
        visit(i + 1, j,i,j);
        visit(i, j + 1,i,j);
        visit(i - 1, j, i, j);
        visit(i, j - 1, i, j);
        ClosedList.Add(grids[i,j]);
        grids[i, j].isInClosedList = true;
    }
    */
    void step()//visit grids around it from a single step 
    {
        SingleGrid grid = OpenList[OpenList.Count - 1];
        Debug.Log("step"+grid.Position);
        if (grid.isBlock)
        {  return; }
        if (grid.isInClosedList)
        { return; }
        if (!grid.isInOpenList)
        {return; }
        OpenList.RemoveAt(OpenList.Count - 1);
        int i = grid.Position.x;
        int j = grid.Position.y;
        visit(i + 1, j, grid);
        visit(i, j + 1, grid);
        visit(i - 1, j,grid);
        visit(i, j - 1, grid);
        ClosedList.Add(grid);
        tileManager.Draw(i, j, TileType.Close);
        grid.isInClosedList = true;
        grid.isInOpenList = false;
    }

    void AddIntoOpenList(SingleGrid grid,SingleGrid parent)  //val从大到小排序(高优先级在最后)
    {
        int index = OpenList.Count-1;
        for(;index>-1;index--)
        {
            if (grid.Val <=OpenList[index].Val)
            { break; }
        }
        OpenList.Insert(index+1, grid);
        grid.isInOpenList = true;
        grid.parentPos = parent.Position;
        grid.FromStart = parent.FromStart + 1;
        tileManager.Draw(grid.Position.x, grid.Position.y, TileType.Open);
    }

    void visit(int i,int j,SingleGrid from)
    {
        if(i<0||i>=width)
        {
            return;
        }
        if(j<0||j>=height)
        {
            return;
        }
        if(grids[i,j].Assume==0)
        {
            //reach end
        }
        if(grids[i,j].isBlock)
        { return; }
        if(grids[i,j].isInClosedList)
        { return; }
        if (grids[i,j].isInOpenList)
        {
            if (grids[i, j].FromStart >from.FromStart + 1)
            {
                OpenList.Remove(grids[i, j]);
                AddIntoOpenList(grids[i, j], from);
            }
        }
        else
        {
            AddIntoOpenList(grids[i, j], from);
        }
    }
    /*
    void AddIntoOpenList(SingleGrid grid,Vector2Int parentPos)
    {
        if (grid.isInOpenList)
        { return; }
        OpenList.Add(grid);
        grid.isInOpenList = true;
        grid.parentPos = parentPos;
    }
    */
}


public enum TileType
{
    Close,Open,Blank,Block
}

public class SingleGrid
{
    public bool isBlock;
    public bool isInOpenList;
    public bool isInClosedList;
    public int Val { get { return Assume + FromStart; } }
    //初始化时赋值
    public int Assume;    
    public Vector2Int Position;
    //初加入openList时赋值,在openList中被访问时看情况更新
    public int FromStart;  
    public Vector2Int parentPos; 
}

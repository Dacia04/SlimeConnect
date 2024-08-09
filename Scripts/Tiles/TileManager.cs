using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set;}

    public LevelSO LevelSO;
    public TileSO TileSO;
    public GameObject TileParent;
    public GameObject NodeParent;

    public Dictionary<int,List<GameObject>> TileDictionary {get;private set;}   // <loại, list gameobject>
    public GameObject[,] Nodes {get; private set;}
    public GameObject[,] Tiles {get;private set;}

    public List<int> SlimeType;
    private Vector2 sizeTile;
    private int width,height;
    private int numberOfSlimeTypes;


    private void Awake() {
        if(Instance ==null)
        {
            Instance = this;
        }
    }

    private void Start() {
        width = InitializeWidth();
        height = InitializeHeight();
        numberOfSlimeTypes = InitializeNumberOfSlimeType();
        GenerateSlimeTileDictinary();
        GenerateGrid();
        GenerateNode();
    }

    private void Update()
    {
        UpdateNodeState();
        if(!GameManager.Instance.IsShuffling)
        {
            UpdatePlayMode();
        }
    }

    #region Generation Method
    //node
    private void GenerateNode()
    {
        Nodes = new GameObject[height+2,width+2];

        for(int x=0;x<=width+1;x++)
        {
            for(int y=0;y<=height+1;y++)
            {
                GameObject nodePrefab = TileSO.Node;
                GameObject nodeObject = Instantiate(nodePrefab,new Vector2(0,0),Quaternion.identity);
                nodeObject.transform.parent = NodeParent.transform;
                Node node = nodeObject.GetComponent<Node>();
                node.Col = x;
                node.Row = y;
                nodeObject.transform.position = new Vector2( x * sizeTile.x + sizeTile.x/2,y * sizeTile.y + sizeTile.y/2);
                nodeObject.name = $"node {y}-{x}";

                Nodes[y,x] = nodeObject;
            }
        }

        // for(int x=0;x<=LevelSO.WidthMap+1;x++)
        // {
        //     for(int y=0;y<=LevelSO.HeightMap+1;y++)
        //     {
        //         Debug.Log(Nodes[y,x].name);
        //     }
        // }
    }

    //grid
    private void GenerateSlimeTileDictinary()
    {
        SlimeType = RandomTypeSlime(numberOfSlimeTypes);
        TileDictionary = new();
        for(int i=0;i<numberOfSlimeTypes;i++)
        {
            TileDictionary.Add(SlimeType[i],new List<GameObject>());
        }
    }
    private void GenerateGrid()
    {
        List<int> generationTypeSlimeCount = Enumerable.Repeat(0,numberOfSlimeTypes).ToList();   // list đếm số slime random
        List<int> typeSlimeCount = DistributeTypeSlime(); // list cho biết số slime cần random
        Tiles = new GameObject[height+2,width+2];

        for(int x=1;x<=width;x++)
        {
            for(int y=1;y<=height;y++)
            {
                int rd;
                do
                {
                    rd = UnityEngine.Random.Range(0,SlimeType.Count);
                }while(generationTypeSlimeCount[rd] >= typeSlimeCount[rd]);

                int type = SlimeType[rd];   // slimeType là 1 dãy type dc random từ trước // rd là để random 1 số trong dãy type đó
                GameObject tilePrefab = TileSO.TilePrefabs[type];
                GameObject tileObject = Instantiate(tilePrefab,new Vector2(0,0),Quaternion.identity);
                tileObject.transform.parent = TileParent.transform;
                Tile tile = tileObject.GetComponent<Tile>();
                //tile.transform.localScale = 2 * tile.transform.localScale;
                sizeTile = tile.SizeTile;
                //Debug.Log(sizeTile);
                tile.Col = x;
                tile.Row = y;
                tile.Type = type;

                tileObject.transform.position = new Vector2( x * sizeTile.x + sizeTile.x/2,y * sizeTile.y + sizeTile.y/2);
                tileObject.name = $"tile {y}-{x}";

                generationTypeSlimeCount[rd]++;
                TileDictionary[type].Add(tileObject);
                Tiles[y,x] = tileObject;
            }
        }

        Camera.main.gameObject.transform.position = new Vector3((float)(width+2)/(float)2  * sizeTile.x, (float)(height+2)/(float)2 * sizeTile.y,-10);
    }
    
    private List<int> RandomTypeSlime(int count)
    {
        System.Random rd = new System.Random();
        List<int> randomInt = new List<int>();
        for(int i=0;i<count;i++)
        {
            int temp;
            do
            {
                temp = rd.Next(0,50);
            }
            while(randomInt.Contains(temp));
            randomInt.Add(temp);
        }
        return randomInt;
    }
    
    private List<int> DistributeTypeSlime()
    {
        int total = width * height;
        int quotient = total / numberOfSlimeTypes;
        if(quotient % 2 !=0)
        {
            quotient--;
        }
        int remainder = total - quotient * numberOfSlimeTypes;



        List<int> temp = Enumerable.Repeat(quotient,numberOfSlimeTypes).ToList();
        int index =0;
        while(remainder!=0)
        {
            temp[index] = temp[index] + 2;
            remainder -=2;
            index++;
        }

        // string x = "";
        // foreach(int i in temp)
        // {
        //     x = x + i.ToString() + " ";
        // }
        // Debug.Log(x);
        return temp;
    }
    
    #endregion


    #region Update State Node Tile
    public void UpdateNodeState()
    {
        for(int x=1;x<=width;x++)
        {
            for(int y=1;y<=height;y++)
            {
                if(Tiles[y,x].GetComponent<Tile>().IsActive)
                {
                     if(Nodes[y,x].activeInHierarchy) Nodes[y,x].SetActive(false);
                    //if(Nodes[y,x].activeInHierarchy) Nodes[y,x].GetComponent<BoxCollider2D>().enabled = false;
                }
                else
                {
                    if(!Nodes[y,x].activeInHierarchy) Nodes[y,x].SetActive(true);
                    //if(Nodes[y,x].activeInHierarchy) Nodes[y,x].GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }
    }
    public void UpdateTileList()   // update tiles lại sau khi  shuffle
    {
        List<GameObject> tileList = new();
        for(int x=1;x<=width;x++)
        {
            for(int y=1;y<=height;y++)
            {
                tileList.Add(Tiles[y,x]);
            }
        }

        var tileSort = tileList.OrderBy(item => item.GetComponent<Tile>().Row).ThenBy(item => item.GetComponent<Tile>().Col);
        tileList = tileSort.ToList();
        int index =0;
        for(int y=1;y<=height;y++)
        {
            for(int x=1;x<=width;x++)
            {
                Tiles[y,x] = tileList[index];
                index++;
            }
        }
    }
    #endregion

    #region Play Mode
    public void UpdatePlayMode()
    {
        if(GameManager.Instance.GetPlayMode() == PlayMode.LeftLineShift) LeftLineShiftPlayMode();
        else if(GameManager.Instance.GetPlayMode() == PlayMode.RightLineShift) RightLineShiftPlayMode();
        else if(GameManager.Instance.GetPlayMode() == PlayMode.TopLineShift) TopLineShiftPlayMode();
        else if(GameManager.Instance.GetPlayMode() == PlayMode.BottomLineShift) BottomLineShiftPlayMode();
        else if(GameManager.Instance.GetPlayMode() == PlayMode.LeftShift) LeftShiftPlayMode();
        else if(GameManager.Instance.GetPlayMode() == PlayMode.RightShift) RightShiftPlayMode();
        else if(GameManager.Instance.GetPlayMode() == PlayMode.TopShift) TopShiftPlayMode();
        else if(GameManager.Instance.GetPlayMode() == PlayMode.BottomShift) BottomShiftPlayMode();
        
    }
    private void LeftLineShiftPlayMode()
    {
        for(int y=1;y<= height;y++)
        {
            for(int x=1;x<=width;x++)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
                tile.UpdateTile();
                if(tile.NodeLeft== null) continue;
                if(tile.NodeLeft.Col ==0 && tile.Col !=1 && 
                    Tiles[y,x-1].GetComponent<Tile>().IsActive == false)
                {
                    //Debug.LogError("left line shift " + tile.gameObject.name);
                    ShiftRowToLeft(1,x-1,y);
                    break;
                }
            }
        }
        //Debug.Log("end turn");
    }
    private void RightLineShiftPlayMode()
    {
        for(int y=1;y<= height;y++)
        {
            for(int x= width;x>=1;x--)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
                tile.UpdateTile();
                if(tile.NodeRight== null) continue;
                if(tile.NodeRight.Col == width+1 && tile.Col != width &&
                        Tiles[y,x+1].GetComponent<Tile>().IsActive == false)
                {
                    //Debug.LogError("right line shift " + tile.gameObject.name);
                    ShiftRowToRight( width, width-x,y);
                    break;
                }
            }
        }
    }
    private void TopLineShiftPlayMode()
    {
        for(int x=1;x<= width;x++)
        {
            for(int y=1;y<= height;y++)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
                tile.UpdateTile();
                if(tile.NodeTop== null) continue;
                if(tile.NodeTop.Row == height+1 && tile.Row !=height &&
                    Tiles[y+1,x].GetComponent<Tile>().IsActive == false)
                {
                    //Debug.Log("Top shift");
                    ShiftColToTop(height,height-y,x);
                    break;
                }
            }
        }
    }
    private void BottomLineShiftPlayMode()
    {
        for(int x=1;x<=width;x++)
        {
            for(int y=1;y<= height;y++)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
                tile.UpdateTile();
                if(tile.NodeBottom== null) continue;
                if(tile.NodeBottom.Row == 0 && tile.Row !=1 &&
                    Tiles[y-1,x].GetComponent<Tile>().IsActive == false)
                {
                    Debug.Log("Bottom shift");
                    ShiftColToBottom(1,y-1,x);
                    break;
                }
            }
        }
    }
    private void LeftShiftPlayMode()
    {
        for(int y=1;y<= height;y++)
        {
            for(int x=1;x<=width;x++)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
                tile.UpdateTile();
                if(tile.NodeLeft== null) continue;
                if(tile.NodeLeft != null && tile.Col != 1 &&
                    Tiles[y,x-1].GetComponent<Tile>().IsActive == false)
                {
                    if(tile.NodeLeft.Col==0)
                    {
                        ShiftRowToLeft(1,x - 1,y);
                    }
                    else
                    {
                        ShiftRowToLeft(tile.NodeLeft.Col,x - tile.NodeLeft.Col,y);
                    }
                    //break;
                }
            }
        }
    }
    private void RightShiftPlayMode()
    {
        for(int y=1;y<= height;y++)
        {
            for(int x=1;x<=width;x++)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
            tile.UpdateTile();
                if(tile.NodeRight== null) continue;
                if(tile.NodeRight !=null && tile.Col !=width &&
                    Tiles[y,x+1].GetComponent<Tile>().IsActive == false)
                {
                    if(tile.NodeRight.Col==width+1)
                    {
                        ShiftRowToRight(width,width-x,y);
                    }
                    else
                    {
                        ShiftRowToRight(tile.NodeRight.Col,tile.NodeRight.Col-x,y);
                    }
                    //break;
                }
            }
        }
    }
    private void TopShiftPlayMode()
    {
        for(int x=1;x<=width;x++)
        {
            for(int y=1;y<= height;y++)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
                tile.UpdateTile();
                if(tile.NodeTop== null) continue;
                if(tile.NodeTop != null && tile.Row !=height &&
                    Tiles[y+1,x].GetComponent<Tile>().IsActive == false)
                {
                    if(tile.NodeTop.Row == height+1)
                    {
                        ShiftColToTop(height,height-y,x);
                    }
                    else
                    {
                        ShiftColToTop(tile.NodeTop.Row,tile.NodeTop.Row-y,x);
                    }
                    //break;
                }
            }
        }
    }
    private void BottomShiftPlayMode()
    {
        for(int x=1;x<=width;x++)
        {
            for(int y=1;y<= height;y++)
            {
                if(!Tiles[y,x].GetComponent<Tile>().IsActive) continue;
                Tile tile = Tiles[y,x].GetComponent<Tile>();
                tile.UpdateTile();
                if(tile.NodeBottom== null) continue;
                if(tile.NodeBottom != null && tile.Row !=1 &&
                    Tiles[y-1,x].GetComponent<Tile>().IsActive == false)
                {
                    if(tile.NodeBottom.Row == 0)
                    {
                        ShiftColToBottom(1,y-1,x);
                    }
                    else
                    {
                        ShiftColToBottom(tile.NodeBottom.Row,y-tile.NodeBottom.Row,x);
                    }
                    //break;
                }
            }
        }
    }
    
    // public void ShiftRight()
    // {
    //     Debug.Log("Shift right");
    //     ShiftRowToRight(LevelSO.WidthMap,1,1);
    // }

    
    #endregion


    #region Other Function

    public void ShiftRowToLeft(int start,int number,int row)
    {
        while(number > 0)
        {
            for(int i= width;i>= start;i--)
            {
                if(i==start)
                {
                    Tile tile = Tiles[row,i].GetComponent<Tile>();
                    //tile.SetDestination(Nodes[row,LevelSO.WidthMap].transform.position);
                    tile.SetPosition(Nodes[row,width].transform.position);
                    tile.Col = width;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[row,LevelSO.WidthMap].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                else
                {
                    Tile tile = Tiles[row,i].GetComponent<Tile>();
                    //tile.SetDestination( Nodes[row,i-1].transform.position);
                    tile.SetPosition( Nodes[row,i-1].transform.position);
                    tile.Col = i-1;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[row,i-1].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                
            }

            UpdateTileList();
            number--;
        }
    }
    public void ShiftRowToRight(int end,int number ,int row)
    {
        while(number > 0)
        {
            for(int i=1;i<=end;i++)
            {
                if(i==end)
                {
                    Tile tile = Tiles[row,i].GetComponent<Tile>();
                    //tile.SetDestination(Nodes[row,1].transform.position);
                    tile.SetPosition(Nodes[row,1].transform.position);
                    tile.Col = 1;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[row,start].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                else
                {
                    Tile tile = Tiles[row,i].GetComponent<Tile>();
                    // tile.SetDestination( Nodes[row,i+1].transform.position);
                    tile.SetPosition( Nodes[row,i+1].transform.position);
                    tile.Col = i+1;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[row,i+1].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                
            }

            UpdateTileList();
            number--;
        }
    }
    public void ShiftColToTop(int end,int number,int col)
    {
        while(number > 0)
        {
            for(int i=1;i<=end;i++)
            {
                if(i==end)
                {
                    Tile tile = Tiles[i,col].GetComponent<Tile>();
                    tile.SetPosition(Nodes[1,col].transform.position);
                    tile.Row = 1;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[1,col].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                else
                {
                    Tile tile = Tiles[i,col].GetComponent<Tile>();
                    tile.SetPosition( Nodes[i+1,col].transform.position);
                    tile.Row = i+1;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[i+1,col].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                
            }

            UpdateTileList();
            number--;
        }
    }
    public void ShiftColToBottom(int start,int number,int col)
    {
        while(number > 0)
        {
            for(int i=start;i<=height;i++)
            {
                if(i==start)
                {
                    Tile tile = Tiles[i,col].GetComponent<Tile>();
                    tile.SetPosition(Nodes[height,col].transform.position);
                    tile.Row = height;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[1,col].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                else
                {
                    Tile tile = Tiles[i,col].GetComponent<Tile>();
                    tile.SetPosition( Nodes[i-1,col].transform.position);
                    tile.Row = i-1;
                    //Debug.Log(tile.gameObject.name + " to " + Nodes[i-1,col].name);
                    tile.gameObject.name = $"tile {tile.Row}-{tile.Col}";

                }
                
            }

            UpdateTileList();
            number--;
        }
    }

    #endregion

    #region Getter Setter
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }

    #endregion

    private int InitializeHeight()
    {
       // return 6;
        if(GameData.GameMode == GameMode.Random)
        {
            int temp = UnityEngine.Random.Range(LevelSO.BaseMinHeightMap,LevelSO.BaseMaxHeightMap);
            if(temp%2==0) return temp;
            else return temp+1;
        }
        else
        {
            int temp = GameData.GameStreak/5 + LevelSO.BaseMinHeightMap;
            if(temp > LevelSO.BaseMaxHeightMap)
                return LevelSO.BaseMaxHeightMap;
            if(temp%2==0) return temp;
            else return temp+1;
            
        }
    }
    private int InitializeWidth()
    {
        //return 5;
        if(GameData.GameMode == GameMode.Random)
        {
            return UnityEngine.Random.Range(LevelSO.BaseMinWidthMap,LevelSO.BaseMaxWidthMap);
        }
        else
        {
            int temp = GameData.GameStreak/5 + LevelSO.BaseMinWidthMap;
            if(temp > LevelSO.BaseMaxWidthMap)
                return LevelSO.BaseMaxWidthMap;
            return temp;
            
        }
    }
    private int InitializeNumberOfSlimeType()
    {
        //return 5;
        return (height * width /3);
    }
    

   
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}

    public LayerMask TileLayer;
    public LayerMask NodeLayer;
    public float TimeDisable;
    public LevelSO LevelSO;
    

    private GameObject TileChoose1;
    private GameObject TileChoose2;
    private Tile tileChoose1;
    private Tile tileChoose2;

    public PlayMode Mode;
    private GameObject Tile1;
    private GameObject Tile2;
    private Tile tile1;
    private Tile tile2;

    private float timeRemain;
    private int buffUsed;
    public bool IsShuffling;
    public int CheckGameState;


    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        InitializeGameData();
        SaveSystem.LoadGameData();
    }

    private void Start() {
        IsShuffling = false;
        GameData.TimePlay = InitializeTimePlay();
        timeRemain = GameData.TimePlay;
        CheckGameState =0;
        if(GameData.GameMode == GameMode.Random)
        {
            RandomPlayMode();
        }
        else
        {
            ClassicPlayMode();
        }
    }   

    private void Update() {
        TimeCountdown();
        
        if(!HintTile(false) && CheckGameState==0)
        {
            if(GameData.ShuffleBuff==0)
            {
                GameData.ShuffleBuff=1;
                ShuffleTile();
                GameData.ShuffleBuff=0;
            } 
            else ShuffleTile();

        }
        CheckGameState = CheckEndGame(); // 2 - lose // 1 - win
    }

    


    #region Check Condition Methods
    public bool CheckPairTile(GameObject T1, GameObject T2,bool isHint = false)
    {
        Tile1 = T1;
        Tile2 = T2;
        tile1 = T1.GetComponent<Tile>();
        tile2 = T2.GetComponent<Tile>();
        if(!tile1.IsActive || !tile2.IsActive) return false;
        if(!isHint)
        {
            if(CheckType())
            {
                if(CheckBetween1() || CheckBetween2()||CheckRightSide() || CheckLeftSide() || CheckTopSide() || CheckBottomSide() )
                {
                    TileManager.Instance.UpdateNodeState();
                    return true;
                }
            }
        }
        else
        {
            if(CheckType())
            {
                if(CheckBetween1(true) || CheckBetween2(true)||CheckRightSide(true) || CheckLeftSide(true) || CheckTopSide(true) || CheckBottomSide(true) )
                {
                    //Debug.Log("Hint: " + T1.name +" " + T2.name);
                    
                    return true;
                }
            }
        }
        return false;

    }
    private bool CheckType()
    {
        return tile1.Type == tile2.Type;
    }

    private bool CheckRightSide(bool isHint = false)
    {
        int rightNode1= (tile1.NodeRight==null) ? tile1.Col :tile1.NodeRight.Col; 
        int rightNode2= (tile2.NodeRight==null) ? tile2.Col :tile2.NodeRight.Col;
        int maxWidth = Mathf.Min(rightNode1,rightNode2);
        int minWidth = Mathf.Max(tile1.Col, tile2.Col);
        int maxHeight = Mathf.Max(tile1.Row,tile2.Row);
        int minHeight = Mathf.Min(tile1.Row,tile2.Row);
        for(int i= minWidth; i<= maxWidth;i++)
        {
            RaycastHit2D[] raycastHit2Ds 
                = Physics2D.LinecastAll(TileManager.Instance.Nodes[minHeight,i].transform.position,TileManager.Instance.Nodes[maxHeight,i].transform.position,TileLayer);
            int count = raycastHit2Ds.Length;
            foreach(var hit in raycastHit2Ds)
            {
                if(hit.collider.gameObject == Tile1 || hit.collider.gameObject == Tile2)
                {
                    count--;
                }
            }
            if(count==0)
            {
                if(!isHint)
                {
                    //draw line
                    List<Transform> points = new();
                    points.Add(Tile1.transform);
                    if(tile1.Row  == maxHeight)
                    {
                        points.Add(TileManager.Instance.Nodes[maxHeight,i].transform);
                        points.Add(TileManager.Instance.Nodes[minHeight,i].transform);
                    }
                    else
                    {
                        points.Add(TileManager.Instance.Nodes[minHeight,i].transform);
                        points.Add(TileManager.Instance.Nodes[maxHeight,i].transform);
                    }
                    points.Add(Tile2.transform);
                    GameObject line = LineManager.Instance.SetUp(points);
                    //StopAllCoroutines();
                    tile1.Disappear();
                    tile2.Disappear();
                    // StartCoroutine(DisableGameObjectCoroutine(Tile1));
                    // StartCoroutine(DisableGameObjectCoroutine(Tile2));
                    StartCoroutine(DestroyGameObjectCoroutine(line));
                }
                //else
                    return true;
            }
        }

       
        return false;
    }
    private bool CheckLeftSide(bool isHint = false)
    {
        int leftNode1= (tile1.NodeLeft==null) ? tile1.Col :tile1.NodeLeft.Col; 
        int leftNode2= (tile2.NodeLeft==null) ? tile2.Col :tile2.NodeLeft.Col;
        int minWidth = Mathf.Max(leftNode1,leftNode2);
        int maxWidth = Mathf.Min(tile1.Col, tile2.Col);
        int maxHeight = Mathf.Max(tile1.Row,tile2.Row);
        int minHeight = Mathf.Min(tile1.Row,tile2.Row);
        for(int i= minWidth; i<= maxWidth;i++)
        {
            RaycastHit2D[] raycastHit2Ds 
                = Physics2D.LinecastAll(TileManager.Instance.Nodes[minHeight,i].transform.position,TileManager.Instance.Nodes[maxHeight,i].transform.position,TileLayer);
            int count = raycastHit2Ds.Length;
            foreach(var hit in raycastHit2Ds)
            {
                if(hit.collider.gameObject == Tile1 || hit.collider.gameObject == Tile2)
                {
                    count--;
                }
            }
            if(count==0)
            {
                if(!isHint)
                {
                    //draw line
                    List<Transform> points = new();
                    points.Add(Tile1.transform);
                    if(tile1.Row  == maxHeight)
                    {
                        points.Add(TileManager.Instance.Nodes[maxHeight,i].transform);
                        points.Add(TileManager.Instance.Nodes[minHeight,i].transform);
                    }
                    else
                    {
                        points.Add(TileManager.Instance.Nodes[minHeight,i].transform);
                        points.Add(TileManager.Instance.Nodes[maxHeight,i].transform);
                    }
                    points.Add(Tile2.transform);
                    GameObject line = LineManager.Instance.SetUp(points);
                    //StopAllCoroutines();
                    tile1.Disappear();
                    tile2.Disappear();
                    // StartCoroutine(DisableGameObjectCoroutine(Tile1));
                    // StartCoroutine(DisableGameObjectCoroutine(Tile2));
                    StartCoroutine(DestroyGameObjectCoroutine(line));
                }
                //else
                return true;
            }
        }

        return false;
    }
    private bool CheckTopSide(bool isHint = false)
    {
        int topNode1= (tile1.NodeTop==null) ? tile1.Row :tile1.NodeTop.Row; 
        int topNode2= (tile2.NodeTop==null) ? tile2.Row :tile2.NodeTop.Row;
        int maxHeight = Mathf.Min(topNode1,topNode2);
        int minHeight = Mathf.Max(tile1.Row, tile2.Row);
        int maxWidth = Mathf.Max(tile1.Col,tile2.Col);
        int minWidth = Mathf.Min(tile1.Col,tile2.Col);
        for(int i= minHeight; i<= maxHeight;i++)
        {
            RaycastHit2D[] raycastHit2Ds 
                = Physics2D.LinecastAll(TileManager.Instance.Nodes[i,minWidth].transform.position,TileManager.Instance.Nodes[i,maxWidth].transform.position,TileLayer);
            int count = raycastHit2Ds.Length;
            foreach(var hit in raycastHit2Ds)
            {
                if(hit.collider.gameObject == Tile1 || hit.collider.gameObject == Tile2)
                {
                    count--;
                }
            }
            if(count==0)
            {
                if(!isHint)
                {
                    //draw line
                    List<Transform> points = new();
                    points.Add(Tile1.transform);
                    if(tile1.Col  == maxWidth)
                    {
                        points.Add(TileManager.Instance.Nodes[i,maxWidth].transform);
                        points.Add(TileManager.Instance.Nodes[i,minWidth].transform);
                    }
                    else
                    {
                        points.Add(TileManager.Instance.Nodes[i,minWidth].transform);
                        points.Add(TileManager.Instance.Nodes[i,maxWidth].transform);
                    }
                    points.Add(Tile2.transform);
                    GameObject line = LineManager.Instance.SetUp(points);
                    //StopAllCoroutines();
                    tile1.Disappear();
                    tile2.Disappear();
                    // StartCoroutine(DisableGameObjectCoroutine(Tile1));
                    // StartCoroutine(DisableGameObjectCoroutine(Tile2));
                    StartCoroutine(DestroyGameObjectCoroutine(line));
                }
                //else
                return true;
            }
        }

        return false;
    }
    private bool CheckBottomSide(bool isHint = false)
    {
        int bottomNode1= (tile1.NodeBottom==null) ? tile1.Row :tile1.NodeBottom.Row; 
        int bottomNode2= (tile2.NodeBottom==null) ? tile2.Row :tile2.NodeBottom.Row;
        int minHeight = Mathf.Max(bottomNode1,bottomNode2);
        int maxHeight = Mathf.Min(tile1.Row, tile2.Row);
        int maxWidth = Mathf.Max(tile1.Col,tile2.Col);
        int minWidth = Mathf.Min(tile1.Col,tile2.Col);
        for(int i= minHeight; i<= maxHeight;i++)
        {
            RaycastHit2D[] raycastHit2Ds 
                = Physics2D.LinecastAll(TileManager.Instance.Nodes[i,minWidth].transform.position,TileManager.Instance.Nodes[i,maxWidth].transform.position,TileLayer);
            int count = raycastHit2Ds.Length;
            foreach(var hit in raycastHit2Ds)
            {
                if(hit.collider.gameObject == Tile1 || hit.collider.gameObject == Tile2)
                {
                    count--;
                }
            }
            if(count==0)
            {
                if(!isHint)
                {
                    
                    //draw line
                    List<Transform> points = new();
                    points.Add(Tile1.transform);
                    if(tile1.Col  == maxWidth)
                    {
                        points.Add(TileManager.Instance.Nodes[i,maxWidth].transform);
                        points.Add(TileManager.Instance.Nodes[i,minWidth].transform);
                    }
                    else
                    {
                        points.Add(TileManager.Instance.Nodes[i,minWidth].transform);
                        points.Add(TileManager.Instance.Nodes[i,maxWidth].transform);
                    }
                    points.Add(Tile2.transform);
                    GameObject line = LineManager.Instance.SetUp(points);
                    //StopAllCoroutines();
                    tile1.Disappear();
                    tile2.Disappear();
                    // StartCoroutine(DisableGameObjectCoroutine(Tile1));
                    // StartCoroutine(DisableGameObjectCoroutine(Tile2));
                    StartCoroutine(DestroyGameObjectCoroutine(line));
                }
                return true;
            }
        }

        return false;
    }

    private bool CheckBetween1(bool isHint = false)   // line check là dọc
    {
        int range1Start = (tile1.NodeLeft == null) ? tile1.Col : tile1.NodeLeft.Col;
        int range1End = (tile1.NodeRight == null) ? tile1.Col : tile1.NodeRight.Col;
        int range2Start = (tile2.NodeLeft == null) ? tile2.Col : tile2.NodeLeft.Col;
        int range2End = (tile2.NodeRight == null) ? tile2.Col : tile2.NodeRight.Col;
        //Debug.Log(range1Start + " " + range1End + " " + range2Start + " " + range2End);
        int maxWidth;
        int minWidth;
        if(tile1.Col > tile2.Col)
        {
            if(range2End >= range1Start)
            {
                maxWidth = Mathf.Min(range2End,tile1.Col);
                minWidth = Mathf.Max(range1Start,tile2.Col);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if(range1End >= range2Start)
            {
                maxWidth = Mathf.Min(range1End,tile2.Col);
                minWidth = Mathf.Max(range2Start,tile1.Col);
            }
            else
            {
                return false;
            }
        }
        //Debug.Log("width: " + minWidth + " " + maxWidth);
        int maxHeight = Mathf.Max(tile1.Row,tile2.Row);
        int minHeight = Mathf.Min(tile1.Row,tile2.Row);
        for(int i= minWidth; i<= maxWidth;i++)
        {
            RaycastHit2D[] raycastHit2Ds 
                = Physics2D.LinecastAll(TileManager.Instance.Nodes[minHeight,i].transform.position,TileManager.Instance.Nodes[maxHeight,i].transform.position,TileLayer);
            int count = raycastHit2Ds.Length;
            foreach(var hit in raycastHit2Ds)
            {
                if(hit.collider.gameObject == Tile1 || hit.collider.gameObject == Tile2)
                {
                    count--;
                }
            }
            if(count==0)
            {
                if(!isHint)
                {
                    //draw line
                    List<Transform> points = new();
                    points.Add(Tile1.transform);
                    if(tile1.Row  == maxHeight)
                    {
                        points.Add(TileManager.Instance.Nodes[maxHeight,i].transform);
                        points.Add(TileManager.Instance.Nodes[minHeight,i].transform);
                    }
                    else
                    {
                        points.Add(TileManager.Instance.Nodes[minHeight,i].transform);
                        points.Add(TileManager.Instance.Nodes[maxHeight,i].transform);
                    }
                    points.Add(Tile2.transform);
                    GameObject line = LineManager.Instance.SetUp(points);
                    //StopAllCoroutines();
                    tile1.Disappear();
                    tile2.Disappear();
                    // StartCoroutine(DisableGameObjectCoroutine(Tile1));
                    // StartCoroutine(DisableGameObjectCoroutine(Tile2));
                    StartCoroutine(DestroyGameObjectCoroutine(line));
                }
                return true;
            }
        }   
        return false;
    }
    private bool CheckBetween2(bool isHint = false)   // line check là ngang
    {
        int range1Start = (tile1.NodeBottom == null) ? tile1.Row : tile1.NodeBottom.Row;
        int range1End = (tile1.NodeTop == null) ? tile1.Row : tile1.NodeTop.Row;
        int range2Start = (tile2.NodeBottom == null) ? tile2.Row : tile2.NodeBottom.Row;
        int range2End = (tile2.NodeTop == null) ? tile2.Row : tile2.NodeTop.Row;

        int maxHeight;
        int minHeight;
        if(tile1.Row > tile2.Row)
        {
            if(range2End >= range1Start)
            {
                maxHeight = Mathf.Min(range2End,tile1.Row);
                minHeight = Mathf.Max(range1Start,tile2.Row);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if(range1End >= range2Start)
            {
                maxHeight = Mathf.Min(range1End,tile2.Row);
                minHeight = Mathf.Max(range2Start,tile1.Row);
            }
            else
            {
                return false;
            }
        }
        int maxWidth = Mathf.Max(tile1.Col,tile2.Col);
        int minWidth = Mathf.Min(tile1.Col,tile2.Col);
        for(int i= minHeight; i<= maxHeight;i++)
        {
            RaycastHit2D[] raycastHit2Ds 
                = Physics2D.LinecastAll(TileManager.Instance.Nodes[i,minWidth].transform.position,TileManager.Instance.Nodes[i,maxWidth].transform.position,TileLayer);
            int count = raycastHit2Ds.Length;
            foreach(var hit in raycastHit2Ds)
            {
                if(hit.collider.gameObject == Tile1 || hit.collider.gameObject == Tile2)
                {
                    count--;
                }
            }
            if(count==0)
            {
                if(!isHint)
                {
                    //draw line
                    List<Transform> points = new();
                    points.Add(Tile1.transform);
                    if(tile1.Col  == maxWidth)
                    {
                        points.Add(TileManager.Instance.Nodes[i,maxWidth].transform);
                        points.Add(TileManager.Instance.Nodes[i,minWidth].transform);
                    }
                    else
                    {
                        points.Add(TileManager.Instance.Nodes[i,minWidth].transform);
                        points.Add(TileManager.Instance.Nodes[i,maxWidth].transform);
                    }
                    points.Add(Tile2.transform);
                    GameObject line = LineManager.Instance.SetUp(points);
                    //StopAllCoroutines();
                    tile1.Disappear();
                    tile2.Disappear();
                    // StartCoroutine(DisableGameObjectCoroutine(Tile1));
                    // StartCoroutine(DisableGameObjectCoroutine(Tile2));
                    StartCoroutine(DestroyGameObjectCoroutine(line));
                }
                return true;
            }
        }
        return false;
    }

    private int CheckEndGame()
    {
        bool tileAlive = false;
        for(int x=1;x<=TileManager.Instance.GetWidth();x++)
        {
            for(int y=1;y<= TileManager.Instance.GetHeight();y++)
            {
                if(TileManager.Instance.Tiles[y,x].GetComponent<Tile>().IsActive)
                {
                    tileAlive = true;
                    break;
                } 
            }
            if(tileAlive) break;  
        }


        if(timeRemain <= 0 && tileAlive) return 2;  // lose
        if(timeRemain > 0 && !tileAlive) return 1; // win
        return 0;  // the game is still alive
    }
    #endregion

    #region Buff Method
    public void CountShuffleBuff()
    {
        if(GameData.ShuffleBuff>0)
        {
            buffUsed++;
            GameData.ShuffleBuff--;
        }
    }
    public void CountHintBuff()
    {
        if(GameData.HintBuff>0)
        {
            buffUsed++;
            GameData.HintBuff--;
        }
    }
    public void CountAddTimeBuff()
    {
        if(GameData.AddTimeBuff>0)
        {
            buffUsed++;
            GameData.AddTimeBuff--;
        }
    }
    public void ShuffleTile()
    {
        if(GameData.ShuffleBuff <=0) return;
        IsShuffling = true;
        Debug.Log("Shuffle");
        List<KeyValuePair<int,int>> listTile1 = new();
        for(int x=1;x<=TileManager.Instance.GetWidth();x++)
        {
            for(int y=1;y<=TileManager.Instance.GetHeight();y++)
            {
                if(TileManager.Instance.Tiles[y,x].GetComponent<Tile>().IsActive)
                {
                    Tile  tile = TileManager.Instance.Tiles[y,x].GetComponent<Tile>();
                    listTile1.Add(new KeyValuePair<int,int>(tile.Row,tile.Col));
                }
            }
        }
        List<KeyValuePair<int,int>> listTile2 = listTile1;
        System.Random rd = new();
        var rdList = listTile2.OrderBy(item => rd.Next());
        listTile2 = rdList.ToList();

        for(int i=0;i<listTile1.Count;i++)
        {
            Tile tile = TileManager.Instance.Tiles[listTile1[i].Key,listTile1[i].Value].GetComponent<Tile>();
            //Debug.Log(TileManager.Instance.Nodes[listTile2[i].Key,listTile2[i].Value].transform.position);
            tile.SetPosition(TileManager.Instance.Nodes[listTile2[i].Key,listTile2[i].Value].transform.position);
            tile.Row = listTile2[i].Key;
            tile.Col = listTile2[i].Value;
            tile.gameObject.name =  $"tile {tile.Row}-{tile.Col}";
        }

        TileManager.Instance.UpdateTileList();
        IsShuffling = false;
    }
    public bool HintTile(bool isGetHint = true)
    {
        
        for(int i=0;i< TileManager.Instance.SlimeType.Count;i++)
        {
            for(int x=0;x< TileManager.Instance.TileDictionary[TileManager.Instance.SlimeType[i]].Count ;x++)
            {
                for(int y=x+1;y< TileManager.Instance.TileDictionary[TileManager.Instance.SlimeType[i]].Count ;y++)
                {
                    if(CheckPairTile( TileManager.Instance.TileDictionary[TileManager.Instance.SlimeType[i]][x],
                                    TileManager.Instance.TileDictionary[TileManager.Instance.SlimeType[i]][y],true))
                    {
                        if(isGetHint)
                        {
                            // tile1.SetHintTile();
                            // tile2.SetHintTile();
                            TileManager.Instance.TileDictionary[TileManager.Instance.SlimeType[i]][x].GetComponent<Tile>().SetHintTile();
                            TileManager.Instance.TileDictionary[TileManager.Instance.SlimeType[i]][y].GetComponent<Tile>().SetHintTile();
                            Debug.Log("still have hint");
                            return true;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public void GetHintTile()
    {
        if(GameData.HintBuff <=0) return;
        HintTile();
    }
    public void AddTime()
    {
        if(GameData.AddTimeBuff <=0) return;
        float temp = UnityEngine.Random.Range(30, 60);
        timeRemain += temp;
        if(timeRemain > GameData.TimePlay)
        {
            timeRemain = GameData.TimePlay;
        }
    }
    private void InitializeGameData()
    {
        GameData.ShuffleBuff = LevelSO.ShuffleBuffBase;
        GameData.AddTimeBuff = LevelSO.AddTimeBuffBase;
        GameData.HintBuff = LevelSO.HintBuffBase;
    }
    private float InitializeTimePlay()
    {
        if(GameData.GameMode == GameMode.Random)
        {
            return UnityEngine.Random.Range(LevelSO.BaseMinTimePlay, LevelSO.BaseMaxTimePlay);
        }
        float temp = LevelSO.BaseMaxTimePlay - GameData.GameStreak/5 * 10;
        if(temp < LevelSO.BaseMinTimePlay)
            return LevelSO.BaseMinTimePlay;
        return temp;
    }
    #endregion

    #region PlayMode
    public void RandomPlayMode()
    {
        Mode = (PlayMode)UnityEngine.Random.Range(0,7);
    }
    public void ClassicPlayMode()
    {
        int temp = GameData.GameStreak%10;
        Mode = (PlayMode)temp;
    }

    #endregion



    #region Other Method
    // Set tile while player choose tile
    public void SetTile(GameObject tileGO)
    {
        if(TileChoose1 == null)
        {
            TileChoose1 = tileGO;
            tileChoose1 = TileChoose1.GetComponent<Tile>();
            tileChoose1.ChooseTile();
            return;
        }
        if(TileChoose2 == null)
        {
            if(tileGO == TileChoose1) return; 
            TileChoose2 = tileGO;
            tileChoose2 = TileChoose2.GetComponent<Tile>();
            tileChoose2.ChooseTile();

            CheckPairTile(TileChoose1,TileChoose2);
            tileChoose1.CancelTile();
            tileChoose2.CancelTile();
            TileChoose1 = null;
            TileChoose2 = null;
            tileChoose1 = null;
            tileChoose2 = null;
        }
    }

    //Time Remainer
    private void TimeCountdown()
    {
        if(CheckGameState==0)
            timeRemain -= Time.deltaTime;
    }
    //point
    public int ScoreCalculation()
    {
        if(CheckGameState ==2) return 0;
        return (int)(LevelSO.BaseScore * 0.2 + LevelSO.BaseScore * 0.8 *timeRemain /GameData.TimePlay - buffUsed * 50);
    }
    public int CoinCalculation()
    {
        int temp;
        if(CheckGameState ==2)
        {
            temp = (int)(LevelSO.BaseCoin * 0.2 + LevelSO.BaseCoin * 0.8 *timeRemain /GameData.TimePlay - buffUsed * 10);
            GameData.Coins += temp;
            return temp;
        } 
        temp = (int)((LevelSO.BaseCoin * 0.2 + LevelSO.BaseCoin * 0.8 *timeRemain /GameData.TimePlay - buffUsed * 10) * 0.2);
        GameData.Coins+= temp;
        return temp;
    }

    //gift
    /// <summary>
    /// if type == 0, buff is shuffle.
    /// if type == 1, buff is add time.
    /// if type == 2, buff is hint.
    /// </summary>
    public void GetGift(int amount,int type)
    {
        if(type==0) 
        {
            GameData.ShuffleBuff+= amount;
            if(GameData.ShuffleBuff<0) GameData.ShuffleBuff=0;
        }
        else if(type==2) 
        {
            GameData.HintBuff+= amount;
            if(GameData.HintBuff<0) GameData.HintBuff =0;
        }
        else if(type==1) 
        {
            timeRemain+= amount;
        }
    }
    
    private void DisableGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    private IEnumerator DisableGameObjectCoroutine(GameObject gameObject)
    {
        yield return new WaitForSeconds(TimeDisable);
        DisableGameObject(gameObject);
    }

    private void DestroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
    private IEnumerator DestroyGameObjectCoroutine(GameObject gameObject)
    {
        yield return new WaitForSeconds(TimeDisable);
        DestroyGameObject(gameObject);
    }
    #endregion





    #region Getter,Setter
    public float GetTimeRemain()
    {
        return timeRemain;
    }
    
    public PlayMode GetPlayMode()
    {
        return Mode;
    }

    public int GetBuffUsed()
    {
        return buffUsed;
    }

    #endregion


    
}

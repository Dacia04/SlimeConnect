using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour,IClickable
{
    public Vector2 SizeTile;
    public int Row;
    public int Col;
    public Node NodeRight;
    public Node NodeLeft;
    public Node NodeTop;
    public Node NodeBottom;
    public int Type;
    public float SpeedTile = 20f;
    

    private SpriteRenderer spriteRenderer;
    private GameObject hoverBorder;
    private GameObject clickBorder;
    private GameObject hintBorder;
    private Animator animator;
    public Vector3 DesPos;
    private BoxCollider2D boxCollider2D;
    private bool isMove;

    public bool IsActive;


    public TextMeshProUGUI rowText;
    public TextMeshProUGUI colText;

    private void Awake() {
        DesPos = new Vector3(-100,-100,-100);
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        SizeTile = spriteRenderer.sprite.bounds.size;
        animator = GetComponent<Animator>();
    }
    private void OnEnable() {
        animator.SetBool("IsDisappear",false);
        IsActive = true;
    }

    private void Start() {
        hoverBorder = gameObject.transform.GetChild(1).gameObject;
        clickBorder = gameObject.transform.GetChild(2).gameObject;
        hintBorder = gameObject.transform.GetChild(3).gameObject;
    }

    private void Update() {
        if(Move()) return;
        if(DesPos != transform.position  && DesPos != new Vector3(-100,-100,-100))
            SetPosition(DesPos);

        if(IsActive)
        {
            UpdateTile();
        }
        else
        {
            NodeRight = null;
            NodeLeft = null;
            NodeTop = null;
            NodeBottom = null;
        }

    }

    private void UpdateText()
    {
        rowText.text = Row.ToString();
        colText.text = Col.ToString();
    }


    #region Handle Event
    public void OnMouseEnter()
    {
        if(clickBorder.activeInHierarchy == false && hintBorder.activeInHierarchy == false)
        {
            // hoverBorder.SetActive(true);
            // clickBorder.SetActive(false);
        }
    }
    public void OnMouseDown()
    {
        GameManager.Instance.SetTile(this.gameObject);
        AudioManager.Instance.PlayClickTileSFX();
    }

    public void OnMouseExit()
    {
        hoverBorder.SetActive(false);
    }

    public void ChooseTile()
    {
        hoverBorder.SetActive(false);
        clickBorder.SetActive(true);
        hintBorder.SetActive(false);
    }
    public void CancelTile()
    {
        hoverBorder.SetActive(false);
        clickBorder.SetActive(false);
        hintBorder.SetActive(false);
    }
    public void Disappear()
    {
        animator.SetBool("IsDisappear",true);
    }
    public void DisableTile()
    {
        Component[] components = gameObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is not Tile && component is not Transform)
            {
                if (component is Behaviour)
                {
                    (component as Behaviour).enabled = false;
                }
                else if (component is Renderer)
                {
                    (component as Renderer).enabled = false;
                }
            }
        }
        foreach (Transform child in transform)
        {
            if(child.gameObject.layer != LayerMask.NameToLayer("UI"))
                child.gameObject.SetActive(false);
        }
        IsActive = false;
    }
    public void EnableTile()
    {
        Component[] components = gameObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is not Tile && component is not Transform)
            {
                if (component is Behaviour)
                {
                    (component as Behaviour).enabled = true;
                }
                else if (component is Renderer)
                {
                    (component as Renderer).enabled = true;
                }
            }
        }
        foreach (Transform child in transform)
        {
            if(child.gameObject.layer != LayerMask.NameToLayer("UI"))
                child.gameObject.SetActive(false);
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        IsActive = true;
    }
    public void SetHintTile()
    {
        //Debug.Log(gameObject.name + ": set hint");
        hintBorder.SetActive(true);
        hoverBorder.SetActive(false);
        clickBorder.SetActive(false);
    }

    public bool Move()
    {
        if(Mathf.Abs(Vector2.Distance(transform.position,DesPos)) < 0.2) return false;
        if(DesPos != transform.position  && DesPos != new Vector3(-100,-100,-100))
        {
            transform.position = Vector2.Lerp(transform.position, DesPos, Time.deltaTime * SpeedTile);
            return true;
        }
        return false;
    }






    #endregion

    #region Collider and Raycast
    public void UpdateTile()
    {
        UpdateTileRight();
        UpdateTileLeft();
        UpdateTileTop();
        UpdateTileBottom();
        UpdateText();
    }
    private void UpdateTileRight()
    {
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(gameObject.transform.position,Vector2.right);
        if(raycastHit2Ds.Length==1)
        {
            NodeRight = null;
            return;
        }
        int index =1;
        while(raycastHit2Ds[index].collider.gameObject.tag == "Node")
        {
            index++;
            if(index == raycastHit2Ds.Length) break;
        }
        NodeRight = raycastHit2Ds[index-1].collider.gameObject.GetComponent<Node>();
    }
    private void UpdateTileLeft()
    {
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(gameObject.transform.position,Vector2.left);
        if(raycastHit2Ds.Length==1)
        {
            NodeLeft = null;
            return;
        }
        int index =1;
        while(raycastHit2Ds[index].collider.gameObject.tag == "Node")
        {
            index++;
            if(index == raycastHit2Ds.Length) break;
        }
        NodeLeft = raycastHit2Ds[index-1].collider.gameObject.GetComponent<Node>();
    }
    private void UpdateTileTop()
    {
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(gameObject.transform.position,Vector2.up);
        if(raycastHit2Ds.Length==1)
        {
            NodeTop = null;
            return;
        }
        int index =1;
        while(raycastHit2Ds[index].collider.gameObject.tag == "Node")
        {
            index++;
            if(index == raycastHit2Ds.Length) break;
        }
        NodeTop = raycastHit2Ds[index-1].collider.gameObject.GetComponent<Node>();
    }
    private void UpdateTileBottom()
    {
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(gameObject.transform.position,Vector2.down);
        if(raycastHit2Ds.Length==1)
        {
            NodeBottom = null;
            return;
        }
        int index =1;
        while(raycastHit2Ds[index].collider.gameObject.tag == "Node")
        {
            index++;
            if(index == raycastHit2Ds.Length) break;
        }
        NodeBottom = raycastHit2Ds[index-1].collider.gameObject.GetComponent<Node>();
    }

    #endregion

    #region Getter and Setter
    public void SetDestination(Vector3 Des)
    {
        this.DesPos = Des;
    }
    public void SetPosition(Vector3 Pos)
    {
        this.transform.position = Pos;
    }

    #endregion


}

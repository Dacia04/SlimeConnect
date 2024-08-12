using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    public TextMeshProUGUI HighestGameStreakText;
    public GameObject ShopCoinConfirmPanel;
    public GameObject ShopCoinRejectPanel;
    public GameObject ShopGemConfirmPanel;
    public GameObject ShopGemRejectPanel;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gemText;

    public Image BGMImage;
    public Image SFXImage;
    private ShopItem shopItem;
    private bool moveToShop;
    private bool moveToMain;
    private Animator animator;

    private void Awake() {
        SaveSystem.LoadGameData();
    }


    private void Start() {
        LoadSoundButton();
        animator = GetComponent<Animator>();
        moveToShop = false;
        moveToMain = false;
        UpdateHighestGameStreakText();
    }

    private void Update() {
        UpdateCoin();
        UpdateGem();
        MoveCamToMain();
        MoveCamToShop();

    }

    public void PlayClickButtonSFX()
    {
        AudioManager.Instance.PlayClickButtonSFX();
    }
    

    #region Shop
    //Coin&Gem
    public void UpdateCoin()
    {
        coinText.text = GameData.Coins.ToString();
    }
    public void UpdateGem()
    {
        gemText.text = GameData.Gem.ToString();
    }
    // cancel shop
    public void CancelButton()
    {
        moveToShop = false;
        moveToMain = true;
    }


    //Shop coins
    public void BuyGem(Button button)
    {
        shopItem = button.GetComponent<ShopItem>();

        if(GameData.Coins >= shopItem.GetPrice())
        {
            ShopCoinConfirmPanel.SetActive(true);
            ConfirmPanel confirmPanel = ShopCoinConfirmPanel.GetComponent<ConfirmPanel>();
            string text = $"Are you sure to purchase {shopItem.GetPrice()} {shopItem.GetNamePrice()}s for {shopItem.GetItem()} {shopItem.GetNameItem()}s ?";
            ShopCoinConfirmPanel.GetComponent<ConfirmPanel>().SetText(text);
        }
        else
        {
            ShopCoinRejectPanel.SetActive(true);
        }
    }

    public void ConfirmBuyGem()
    {
        //Debug.Log(shopItem.GetPrice());
        GameData.Coins -= shopItem.GetPrice();
        GameData.Gem += shopItem.GetItem();
        SaveSystem.SaveGameData();
        ShopCoinConfirmPanel.SetActive(false);
    }
    
    
    //Shop Gem
    public void BuyBuff(Button button)
    {
        shopItem = button.GetComponent<ShopItem>();
        if(GameData.Gem >= shopItem.GetPrice() )
        {
            ShopGemConfirmPanel.SetActive(true);
            ConfirmPanel confirmPanel = ShopCoinConfirmPanel.GetComponent<ConfirmPanel>();
            string text;
            if(shopItem.GetItem()>1)
                text = $"Are you sure to purchase {shopItem.GetPrice()} {shopItem.GetNamePrice()}s for {shopItem.GetItem()} {shopItem.GetNameItem()}s ?";
            else
                text = $"Are you sure to purchase {shopItem.GetPrice()} {shopItem.GetNamePrice()}s for {shopItem.GetItem()} {shopItem.GetNameItem()} ?";
            ShopGemConfirmPanel.GetComponent<ConfirmPanel>().SetText(text);
        }
        else
        {
            ShopGemRejectPanel.SetActive(true);
        }
    }
    public void ConfirmBuyBuff()
    {
        GameData.Gem -= shopItem.GetPrice();
        if(shopItem.GetNameItem() == "shuffle" ) GameData.ShuffleBuff+= shopItem.GetItem();
        else if(shopItem.GetNameItem() == "time" ) GameData.ShuffleBuff+= shopItem.GetItem();
        else if(shopItem.GetNameItem() == "hint" ) GameData.ShuffleBuff+= shopItem.GetItem();
        else if(shopItem.GetNameItem() == "combo")
        {
            GameData.ShuffleBuff+= 1;
            GameData.ShuffleBuff+= 1;
            GameData.ShuffleBuff+= 1;
        }
        SaveSystem.SaveGameData();
        ShopGemConfirmPanel.SetActive(false);
    }
    #endregion



    #region MainMenu
    public void LoadSoundButton()
    {
        if(GameData.BGM ==1 ) BGMImage.color = Color.white;
        else BGMImage.color = Color.red;
        if(GameData.SFX ==1) SFXImage.color = Color.white;
        else SFXImage.color = Color.red;
    }
    public void ClassicButton()
    {
        GameData.GameMode = GameMode.Classic;
        PlayGame();
    }
    public void RandomButton()
    {
        GameData.GameMode = GameMode.Random;
        PlayGame();
    }
    public void SFXButton()
    {
        if(GameData.SFX ==1)
        {
            SFXImage.color = Color.red;
            AudioManager.Instance.MuteSFX();
        }
        else 
        {
            SFXImage.color = Color.white;
            AudioManager.Instance.UnMuteSFX();
        }
        SaveSystem.SaveGameData();
    }
    public void BGMButton()
    {
        if(GameData.BGM ==1)
        {
            BGMImage.color = Color.red;
            AudioManager.Instance.MuteBGM();
        }
        else 
        {
            BGMImage.color = Color.white;
            AudioManager.Instance.UnMuteBGM();
        }
        SaveSystem.SaveGameData();
    }
    public void ShopButton()
    {
        moveToShop = true;
        moveToMain = false;
    }
    public void UpdateHighestGameStreakText()
    {
        HighestGameStreakText.text = $"Highest Game Streak\n{GameData.HighestGameStreak}";
    }
    #endregion


    #region Sub Function
    public void CancelGameObject(GameObject go)
    {
        go.SetActive(false);
    }
    private void MoveCamToShop()
    {
        if(moveToShop)
        {
            animator.SetBool("IsMainMenu",false);
            moveToShop = false;
        }
    }
    private void MoveCamToMain()
    {
        if(moveToMain)
        {
            animator.SetBool("IsMainMenu",true);
            moveToMain = false;
        }
    }
    private void PlayGame()
    {
        SaveSystem.SaveGameData();
        SceneTransition.Instance.ChangeGameScene();
    }
    #endregion
}

using UnityEngine;
using UnityEngine.UI;

public class UIMoney : MonoBehaviour {
    public Text Money;
    private Player mPlayer;
    
    void Start()
    {
        mPlayer = Player.Instance;
        UpdateMoney();
        mPlayer.MoneyChangeEvent += UpdateMoney;
    }
    
    void UpdateMoney()
    {
        Money.text = "$ " + mPlayer.Money;
    }

    void OnDestroy()
    {
        mPlayer.MoneyChangeEvent -= UpdateMoney;
    }
}

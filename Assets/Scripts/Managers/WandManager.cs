using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WandManager : MonoBehaviour {

    public static WandManager instance;
    public Transform wandTip;
    //public TextMeshProUGUI moneyText, pointsText, silverKeyText, goldKeyText;
    //public Image pointProgressBar;

    //public ParticleSystem smallGemPS, mediumGemPS, largeGemPS, smallGemPS_x2, mediumGemPS_x2, largeGemPS_x2;

    void Awake ()
    {
        instance = this;
    }

 //   // Use this for initialization
 //   void Start () {
 //       UpdateMoney("$0.00");
 //       UpdatePoints(0);
 //       UpdateSilverKeys("0");
 //       UpdateGoldKey("0");
	//}

    //public void UpdateMoney (string newValue)
    //{
    //    moneyText.text = newValue;
    //}

    //public void UpdatePoints (int newValue)
    //{
    //    pointsText.text = newValue.ToString();
    //    pointProgressBar.fillAmount = newValue / 20000f;
    //}

    //public void UpdateSilverKeys (string newValue)
    //{
    //    silverKeyText.text = newValue;
    //}

    //public void UpdateGoldKey (string newValue)
    //{
    //    goldKeyText.text = newValue;
    //}

    //public void CreateGemFeedback(int _value, bool _doubleScore)
    //{
    //    switch (_value)
    //    {
    //        case 5:
    //            if(!_doubleScore)
    //                smallGemPS.Emit(1);
    //            else
    //                smallGemPS_x2.Emit(1);

    //            break;
    //        case 25:
    //            if (!_doubleScore)
    //                mediumGemPS.Emit(1);
    //            else
    //                mediumGemPS_x2.Emit(1);

    //            break;
    //        case 100:
    //            if(!_doubleScore)
    //                largeGemPS.Emit(1);
    //            else
    //                largeGemPS_x2.Emit(1);
    //            break;
    //    }
    //}
}

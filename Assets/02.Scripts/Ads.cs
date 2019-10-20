using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class Ads : MonoBehaviour
{

    public string placementld = "rewardedVideo";


#if UNITY_IOS
    private string gameId = "1234555";
#elif UNITY_ANDRIOD
    private string gameId = "44341231";
#elif UNITY_EDITOR
    private string gameId = "44341231";
#else
    private string gameId = "44341231";
#endif


    // Start is called before the first frame update
    void Start()
    {
        if (Monetization.isSupported) {
            Monetization.Initialize(gameId, true); //출시전에는 false, 출시 후에는 true
        }
    }

    public void ShowAd() {
        ShowAdCallbacks options = new ShowAdCallbacks();
        options.finishCallback = HandleShowResult;

        ShowAdPlacementContent ad = Monetization.GetPlacementContent(placementld) as ShowAdPlacementContent;

        ad.Show(options); 
    }

    void HandleShowResult(ShowResult result) {
        if (result == ShowResult.Finished)
        { //광고 재생을 스킵 없이 모두 마쳤을때
            GameManager.money += 1000;
        }
        else if (result == ShowResult.Skipped)
        {//스킵했을때

        }
        else {
        
        }
    }
}

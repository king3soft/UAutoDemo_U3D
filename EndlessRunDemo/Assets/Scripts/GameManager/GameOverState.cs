using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
using System.Collections.Generic;
 
/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    public TrackManager trackManager;
    public Canvas canvas;
    public MissionUI missionPopup;

	public AudioClip gameOverTheme;

	public Leaderboard miniLeaderboard;
	public Leaderboard fullLeaderboard;

    public GameObject addButton;

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

		miniLeaderboard.playerEntry.inputName.text = PlayerData.instance.previousName;
		
		miniLeaderboard.playerEntry.score.text = trackManager.score.ToString();
		miniLeaderboard.Populate();

        if (PlayerData.instance.AnyMissionComplete())
            missionPopup.Open();
        else
            missionPopup.gameObject.SetActive(false);

		CreditCoins();

		if (MusicPlayer.instance.GetStem(0) != gameOverTheme)
		{
            MusicPlayer.instance.SetStem(0, gameOverTheme);
			StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }
    }

	public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        FinishRun();
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {
        
    }

	public void OpenLeaderboard()
	{
		fullLeaderboard.forcePlayerDisplay = false;
		fullLeaderboard.displayPlayer = true;
		fullLeaderboard.playerEntry.playerName.text = miniLeaderboard.playerEntry.inputName.text;
		fullLeaderboard.playerEntry.score.text = trackManager.score.ToString();

		fullLeaderboard.Open();
    }

	public void GoToStore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("shop", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }


    public void GoToLoadout()
    {
        trackManager.isRerun = false;
		manager.SwitchState("Loadout");
    }

    public void RunAgain()
    {
        trackManager.isRerun = false;
        manager.SwitchState("Game");
    }
    
    protected void CreditCoins()
	{
		PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "gameplay";
        var level = PlayerData.instance.rank.ToString();
        var itemType = "consumable";
        
        if (trackManager.characterController.coins > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                trackManager.characterController.coins,
                "fishbone",
                PlayerData.instance.coins,
                itemType,
                level,
                transactionId
            );
        }

        if (trackManager.characterController.premium > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                trackManager.characterController.premium,
                "anchovies",
                PlayerData.instance.premium,
                itemType,
                level,
                transactionId
            );
        }

    a,b,c
    1,2,3
    a=2,c=3 
    a=3,b=2
    a=1,c=2
    各猜对一半
    a=1,b=2,c=3

    22,24,27,32,39,50
    2,3,5,7,11
    1,2,3,4
    1,1
    0

    1 6
    2 3
    3 0

    ab 10
    bc 12
    ac 4 + b 12
    ac 4 = b ?

    10a + 10b = 12b + 12c = 4a + 4c + 12b
    10a + 10b = 12b + 12c
    12c = 4a + 4c
    8c = 4a
    2c = a
    20c + 10b = 12b + 12c
    4c = b
    2a = b
    合作工作用加

    
    10ab = 12bc = 4ac + 12b = ?b
    a比c高2

    5 5 14 38 87 167
    5+5+4 = 14
    5+5+14+14 = 38
    5+5+14+38+25 = 87

    1*1-1 3*3 5*5-1 7*7 9*9-1
    0 9 24 49 80
    9 15 25
    
    组合(m,n)
    n*(n-1)....(n - m+1)/m!

    1/2  1  1  ?  9/11  11/13
    1/2 2/1+1 7/7 7/7+2 9/11 11/9+4
    分子分母还可以分开看

    爬楼梯（递归类似算法的解结果）可以用斐波那契数列求解，用抽象的方法求解

    6 14 30 62 126 
    8 16 32 64

    69 36 19 10 5 2
    33 17 9 5 3
    16 8 4 2
    y = 1.1x
    x + 1.1x + 1.1*1.1x = 662 *3 = 1986
    3.31x = 1986

    a = (b + c)/2 + 7.5
    b = (a + c)/2 - 6
    c = 80
    a = (b + 80)/2 + 7.5
    b = (a + 80)/2 - 6
    a - b = 


#endif
    }

    protected void FinishRun()
    {
		if(miniLeaderboard.playerEntry.inputName.text == "")
		{
			miniLeaderboard.playerEntry.inputName.text = "Trash Cat";
		}
		else
		{
			PlayerData.instance.previousName = miniLeaderboard.playerEntry.inputName.text;
		}

        PlayerData.instance.InsertScore(trackManager.score, miniLeaderboard.playerEntry.inputName.text );

        CharacterCollider.DeathEvent de = trackManager.characterController.characterCollider.deathData;
        //register data to analytics
#if UNITY_ANALYTICS
        AnalyticsEvent.GameOver(null, new Dictionary<string, object> {
            { "coins", de.coins },
            { "premium", de.premium },
            { "score", de.score },
            { "distance", de.worldDistance },
            { "obstacle",  de.obstacleType },
            { "theme", de.themeUsed },
            { "character", de.character },
        });
#endif

        PlayerData.instance.Save();

        trackManager.End();
    }

    //----------------
}

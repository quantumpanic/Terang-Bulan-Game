using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    private void Awake() {
        if (!Instance) Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Static variables
    public string playerName;
    public int totalScore;
    public int totalCoins;

    // Dynamic variables
    public StageDataObject selectedStageData;
    public int _penalty;

    // References

    // Methods
    public void OpenStageSelect(){
        SceneManager.LoadScene("StageSelect");
    }

    public void LoadGame(StageDataObject dataObj){
        if (dataObj.hasNoGameplay){
            UnlockStagesFromDataObj(dataObj);
            RefreshStageSelectButtons();
            return;
        }

        selectedStageData = dataObj;
        SceneManager.LoadScene("Game");
    }

    public void UnlockStagesFromDataObj(StageDataObject dataObj){
        foreach (StageDataObject o in dataObj.stagesToUnlock){
            o._stageIsUnlocked = true;
        }
    }

    void RefreshStageSelectButtons(){
        OpenStageSelect();
    }
}

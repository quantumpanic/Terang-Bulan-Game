using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{
    // the object holding our stage data
    public StageDataObject dataObject;
    private void Awake() {
        RefreshUI();
    }

    public void LoadStage(){
        GameManager.Instance.LoadGame(dataObject);
    }

    public void RefreshUI(){
        GetComponentInChildren<Text>().text = dataObject.stageName;
        GetComponentInChildren<Button>().interactable = dataObject._stageIsUnlocked;
        // GetComponentInChildren<Button>().interactable = dataObject.stageIsUnlocked;
    }

    public void PlaySfx(){
        GetComponent<AudioSource>().Play();
    }
}

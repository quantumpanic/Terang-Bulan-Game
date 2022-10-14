using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageDataObject", menuName = "ScriptableObjects/Create StageData Object")]
public class StageDataObject : ScriptableObject
{
    // statics
    public string stageName;
    public bool hasNoGameplay;
    public int coinsGoal;
    public int customersGoal;
    public float timeLimitBase;
    public float custSpwnTimeBase = 10;
    public float custSpwnTimeRand = 5;

    // dynamics
    [SerializeField] public bool stageIsUnlocked;
    [HideInInspector] public bool _stageIsUnlocked;
    private int customerPenalty = 0;
    [HideInInspector] public int _customerPenalty;

    public List<StageDataObject> stagesToUnlock = new List<StageDataObject>();

    private void OnEnable() {
        _stageIsUnlocked = stageIsUnlocked;
        _customerPenalty = customerPenalty;

        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
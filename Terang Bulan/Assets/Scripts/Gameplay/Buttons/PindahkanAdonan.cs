using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PindahkanAdonan : MonoBehaviour
{
    public Kue kue;
    public bool isBusy;
    public float _timerBaking;
    public float bakeTime = 5f;

    private void Update()
    {
        // countdown timer until baking done
        if (isBusy)
        {
            _timerBaking += Time.deltaTime;

            if (_timerBaking >= bakeTime)
            {
                BakingDone();
            }
        }
    }

    public void BeginBaking(Kue cake)
    {
        // while baking, disable send to table
        _timerBaking = 0;
        kue = cake;
        isBusy = true;
    }

    void BakingDone()
    {
        // enable the button
        isBusy = false;
        print("cake finished baking!");

        // smoke animation
        GameObject smoke = Instantiate(Gameplay.Instance.smokePrefab);
        smoke.transform.SetParent(transform);
        smoke.transform.localPosition = Vector3.zero;
        smoke.transform.localScale = Vector3.one;
    }

    public void MoveCakeToTable()
    {
        if (kue == null || isBusy) return;
        // fill the left side first
        Gameplay.Instance.MoveCakeToTable(kue, this);
    }

    public void CakeMoved()
    {
        kue = null;
    }
}

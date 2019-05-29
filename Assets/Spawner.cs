using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PerformanceData {
    public int instanceCount;
    public float fps;
}

public abstract class Spawner : MonoBehaviour
{
    protected const float FPSThreshold = 20f;
    protected const int startSpawnCount = 1000;
    public const int iterationCount = 1000;

    protected static string result = "";

    protected int instanceCount;
    protected bool isRun = true;
    protected float fps;
    protected float timer;
    protected float fpsSum;
    protected int fpsRecordsCount;
    protected float stopTimer;
    protected List<PerformanceData> performanceData = new List<PerformanceData>();

    [SerializeField]
    protected Spawner nextSpawner;
    [SerializeField]
    Text fpsStat;
    [SerializeField]
    Text instanceCountStat;
    [SerializeField]
    Image stopProgressImg;

    virtual protected void Start()
    {
        CreateEntity(startSpawnCount);
    }

    void Update()
    {
        timer += Time.deltaTime;
        fps = 1f / Time.deltaTime;
        fpsSum += fps;
        fpsRecordsCount++;
        if (timer > 0.5f) {
            var avgFps = fpsSum / fpsRecordsCount;
            fpsStat.text = ((int)avgFps).ToString();

            isRun = avgFps > FPSThreshold;
            if (isRun) {
                performanceData.Add(new PerformanceData { instanceCount = instanceCount, fps = avgFps });
                CreateEntity(Mathf.Max(1, instanceCount / 20));
            }

            timer = 0;
            fpsSum = 0;
            fpsRecordsCount = 0;
        }

        if (isRun) {
            stopTimer = 0;
            stopProgressImg.fillAmount = 0;
        }
        else
        {
            stopTimer += Time.deltaTime;
            if (stopTimer > 10f)
                End();
        }
        stopProgressImg.fillAmount = stopTimer / 10f;
    }

    virtual protected void CreateEntity(int amount) {
        instanceCount += amount;
        instanceCountStat.text = instanceCount.ToString("N0");
    }

    virtual protected void End() {
        result += ToString();

        if (nextSpawner == null)
            System.IO.File.WriteAllText(@"TestExcel.csv", result);
        else {
            nextSpawner.enabled = true;
        }

        Destroy(gameObject);
    }

    public override string ToString()
    {
        string result = "";

        performanceData.Reverse();
        foreach (PerformanceData perfData in performanceData)
            result += perfData.fps + ";";
        result += "\n";
        foreach (PerformanceData perfData in performanceData)
            result += perfData.instanceCount + ";";
        result += "\n";

        return result;
    }
}

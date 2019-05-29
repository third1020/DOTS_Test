using UnityEngine;

public class ClassicSpawner : Spawner {
    [SerializeField]
    GameObject[] entity;

    override protected void CreateEntity(int amount)
    {
        base.CreateEntity(amount);
        for (int i = 0; i < amount; i++)
            GameObject.Instantiate(entity[i % 2], transform);
    }

    protected override void End()
    {
        result += SystemInfo.processorType + ";" + ((float)SystemInfo.processorFrequency / 1000).ToString() + ";" + SystemInfo.processorCount + ";\n";
        base.End();
    }
}

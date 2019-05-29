using UnityEngine;

public class ClassicEntityB : MonoBehaviour {
    float floatFiled = 0;

    void Update()
    {
        for (int i = 0; i < Spawner.iterationCount; i++)
        {
            floatFiled += Time.deltaTime;
            if (floatFiled > 1000)
                floatFiled = 0;
        }
    }
}
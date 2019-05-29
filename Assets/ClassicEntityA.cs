using UnityEngine;

public class ClassicEntityA : MonoBehaviour
{
    int int32filed = 0;

    void Update()
    {
        for (int i = 0; i < Spawner.iterationCount; i++) {
            int32filed++;
            if (int32filed > 1000)
                int32filed = 0;
        }
    }
}
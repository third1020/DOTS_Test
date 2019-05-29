using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ECSSpawner : Spawner {
    EntityManager entityManager;
    EntityArchetype entityAAt;
    EntityArchetype entityBAt;

    override protected void Start()
    {
        entityManager = World.Active.EntityManager;

        entityAAt = entityManager.CreateArchetype(typeof(CompA));
        entityBAt = entityManager.CreateArchetype(typeof(CompB));

        base.Start();
    }

    override protected void CreateEntity(int amount)
    {
        base.CreateEntity(amount);

        var newEntitiesA = new NativeArray<Entity>(amount / 2, Allocator.Temp);
        entityManager.CreateEntity(entityAAt, newEntitiesA);

        var newEntitiesB = new NativeArray<Entity>(amount / 2, Allocator.Temp);
        entityManager.CreateEntity(entityBAt, newEntitiesB);
    }

    override protected void End()
    {
        entityManager.DestroyEntity(entityManager.GetAllEntities());
        base.End();
    }
}

public struct CompA : IComponentData {
    public int int32Field;
}
public struct CompB : IComponentData {
    public float floatField;
}

class UpdSysA : ComponentSystem {
    protected override void OnUpdate()
    {
        Entities.ForEach((ref CompA compA) =>
        {
            for (int i = 0; i < ECSSpawner.iterationCount; i++) {
                compA.int32Field++;
                if (compA.int32Field > 1000)
                    compA.int32Field = 0;
            }
        });
    }
}
class UpdSysB : ComponentSystem {
    protected override void OnUpdate()
    {
        var dt = Time.deltaTime;

        Entities.ForEach((ref CompB compB) =>
        {
            for (int i = 0; i < ECSSpawner.iterationCount; i++)
            {
                compB.floatField += dt;
                if (compB.floatField > 1000)
                    compB.floatField = 0;
            }
        });
    }
}
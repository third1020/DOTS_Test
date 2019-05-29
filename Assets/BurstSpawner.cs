using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class BurstSpawner : Spawner {
    EntityManager entityManager;
    EntityArchetype jobEntityAAt;
    EntityArchetype jobEntityBAt;

    override protected void Start()
    {
        entityManager = World.Active.EntityManager;

        jobEntityAAt = entityManager.CreateArchetype(typeof(BurstCompA));
        jobEntityBAt = entityManager.CreateArchetype(typeof(BurstCompB));

        base.Start();
    }

    override protected void CreateEntity(int amount)
    {
        base.CreateEntity(amount);

        var newEntitiesA = new NativeArray<Entity>(amount / 2, Allocator.Temp);
        entityManager.CreateEntity(jobEntityAAt, newEntitiesA);

        var newEntitiesB = new NativeArray<Entity>(amount / 2, Allocator.Temp);
        entityManager.CreateEntity(jobEntityBAt, newEntitiesB);
    }

    override protected void End()
    {
        entityManager.DestroyEntity(entityManager.GetAllEntities());
        base.End();
    }
}

public struct BurstCompA : IComponentData {
    public int int32Field;
}
public struct BurstCompB : IComponentData {
    public float floatField;
}

class BurstUpdSysA : JobComponentSystem {
    [BurstCompile]
    struct JobA : IJobForEach<BurstCompA> {
        public void Execute(ref BurstCompA compA)
        {
            for (int i = 0; i < ECSSpawner.iterationCount; i++)
            {
                compA.int32Field++;
                if (compA.int32Field > 1000)
                    compA.int32Field = 0;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new JobA().Schedule(this, inputDeps);
    }
}
class BurstUpdSysB : JobComponentSystem {
    [BurstCompile]
    struct JobB : IJobForEach<BurstCompB> {
        public float dt;

        public void Execute(ref BurstCompB compB)
        {
            for (int i = 0; i < ECSSpawner.iterationCount; i++)
            {
                compB.floatField += dt;
                if (compB.floatField > 1000)
                    compB.floatField = 0;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new JobB() { dt = Time.deltaTime }.Schedule(this, inputDeps);
    }
}
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class JobSpawner : Spawner {
    EntityManager entityManager;
    EntityArchetype jobEntityAAt;
    EntityArchetype jobEntityBAt;

    override protected void Start()
    {
        entityManager = World.Active.EntityManager;

        jobEntityAAt = entityManager.CreateArchetype(typeof(JobCompA));
        jobEntityBAt = entityManager.CreateArchetype(typeof(JobCompB));

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

public struct JobCompA : IComponentData {
    public int int32Field;
}
public struct JobCompB : IComponentData {
    public float floatField;
}

class JobUpdSysA : JobComponentSystem {
    struct JobA : IJobForEach<JobCompA> {

        public void Execute(ref JobCompA compA)
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
class JobUpdSysB : JobComponentSystem {
    struct JobB : IJobForEach<JobCompB> {
        public float dt;

        public void Execute(ref JobCompB compB)
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
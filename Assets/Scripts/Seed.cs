using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Creature
{
    public static List<Seed> seeds = new();
    private Func<Plant> createNewPlant;
    private float sproutTime;
    private Vector3 sproutPosition;
    public static Seed CreateNewSeed(Func<Plant> createNewPlant, float sproutTime)
    {
        var seed = Instantiate(Resources.Load<GameObject>("Prefabs/Seed")).GetComponent<Seed>();
        seed.createNewPlant = createNewPlant;
        seed.maxLifeTime = 1000f;
        return seed;
    }

    public static Seed CreateNewSeed(Func<Plant> createNewPlant, Vector3 position, float sproutTime)
    {
        var seed = CreateNewSeed(createNewPlant, sproutTime);
        seed.transform.position = position;
        return seed;
    }

    public Seed SetLifeTime(float lifeTime)
    {
        this.maxLifeTime = lifeTime;
        return this;
    }

    public Plant Sprout()
    {
        Plant plant = createNewPlant();
        plant.transform.position = transform.position;
        Destroy(gameObject);
        return plant;
    }

    protected override void Init()
    {
        seeds.Add(this);
    }

    protected override Creature Reproduce()
    {
        return Sprout();
    }

    protected override void LivingActivities()
    {
        if (lifeTime > sproutTime)
            Sprout();
    }

    protected override void Die() => Destroy(gameObject);

    private void OnDestroy()
    {
        seeds.Remove(this);
    }
}

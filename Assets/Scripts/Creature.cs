using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//所有东西都是creature,都是MonoBehaviour(组件)
public abstract class Creature : MonoBehaviour
{
    [Header("Creature")]
    public float lifeTime = 0;
    public float maxLifeTime;
    [Range(0,1)] public float lifeTimeRandomness;
    public float satiety;
    
    public static Creature AddNewCreature(Func<Creature> method) => method();

    protected virtual void Awake()
    {
        Init();
        maxLifeTime *= Random.Range(1-lifeTimeRandomness, 1f);
    }

    protected virtual void Update()
    {
        lifeTime += Time.deltaTime;
        LivingActivities();
        if (lifeTime > maxLifeTime)
            Die();
    }

    /// <summary>
    /// Initialize, called at awake
    /// </summary>
    protected abstract void Init();
    /// <summary>
    /// have baby
    /// </summary>
    protected abstract Creature Reproduce();
    /// <summary>
    /// How a creature behaves when alive
    /// </summary>
    protected abstract void LivingActivities();
    /// <summary>
    /// when died
    /// </summary>
    protected abstract void Die();

    protected void Kill(Creature other)
    {
        other.Die();
    }
}

//植物
public abstract class Plant : Creature
{
    public float matureTime;
    public bool matured = false;
    public float reproduceTimer = 0f;
    public float reproduceInterval;

    /// <summary>
    /// a plant can grow but, well, doesn't matter
    /// </summary>
    protected abstract void Grow();
    /// <summary>
    /// already overriden, so, I provided plant living activities for you (o゜▽゜)o☆
    /// </summary>
    protected override void LivingActivities()
    {
        Grow();
        CheckMaturityAndReproduction();
    }
    /// <summary>
    /// a plant can reproduce only when matured
    /// </summary>
    protected void CheckMaturityAndReproduction()
    {
        if (!matured && lifeTime > matureTime)
        {
            matured = true;
        }
        else
        {
            if (reproduceTimer > reproduceInterval)
                HandleReproduction();
            else
                reproduceTimer += Time.deltaTime;
        }
    }
    protected void HandleReproduction()
    {
        reproduceTimer += Time.deltaTime;
        if (reproduceTimer > reproduceInterval)
        {
            Reproduce();
            reproduceTimer = 0;
        }
    }
}

public abstract class Animal : Creature
{
    [Header("Animal")]
    [SerializeField,] protected float hungry;
    [SerializeField] protected float maxHungry = 20f;
    [SerializeField] protected float hungryLevel_SearchForFood;
    [SerializeField] protected float hungryLevel_EagerForFood;
    public float matureTime;
    public bool matured = false;
    public float reproduceTimer = 0f;
    public float reproduceInterval;
    [SerializeField] protected float maxMoveSpeed;
    public bool willingToEat = false;

    protected Vector3 moveTarget;
    protected float currentMoveSpeed;
    protected bool isMoving = false;
    protected abstract void Eat(Creature food);
    protected abstract void DecideMoveTarget();

    protected void SetMoveTarget(Vector3 target, float speed)
    {
        if (Vector3.SqrMagnitude(target - transform.position) < 0.01f)
            return;
        moveTarget = target;
        currentMoveSpeed = speed;
        isMoving = true;
    }
    /// <summary>
    /// reproduce
    /// </summary>
    /// <param name="mate"></param>
    /// <returns></returns>
    protected abstract Animal Breed(Animal mate);

    protected override void LivingActivities()
    {
        hungry += Time.deltaTime;
        if (hungry > maxHungry)
        {
            Die();
            return;
        }
        if (isMoving)
        {
            Vector3 moveDirection = (moveTarget - transform.position).normalized;
            transform.position = transform.position + moveDirection * (currentMoveSpeed * Time.deltaTime);
            if (Vector3.SqrMagnitude(moveTarget - transform.position) < 0.1f)
                isMoving = !isMoving;
        }
    }
}

public abstract class Bird : Animal
{
    protected override void DecideMoveTarget() => Fly();

    protected abstract void Fly();
    protected abstract bool FindFood(out Creature food);
}

public abstract class Herbivore : Animal
{
    /// <summary>
    /// for accesing all herbivores
    /// </summary>
    private static List<Herbivore> _herbivores = new List<Herbivore>();
    public static List<Herbivore> Herbivores => _herbivores;

    protected override void Awake()
    {
        base.Awake();
        Herbivores.Add(this);
    }

    protected abstract Plant FindFood(Type[] targetTypes);
    /// <summary>
    /// Detect all carnivores within fear distance and out them
    /// </summary>
    /// <param name="carnivores"></param>
    /// <returns>true if there's carnivores within the fear distance</returns>
    protected abstract bool DetectCarnivores(out Carnivore[] carnivores);
    /// <summary>
    /// Just see if there's any carnivore here and out the first on detected
    /// not guaranteed that is the closest one
    /// </summary>
    /// <param name="carnivore">the carnivore detected</param>
    /// <returns>true if there's carnivores within the fear distance</returns>
    protected abstract bool DetectCarnivores(out Carnivore carnivore);
    protected abstract void Move_Escape();
}

public abstract class Carnivore : Animal
{
    /// <summary>
    /// for accessing all carnivores
    /// </summary>
    private static List<Carnivore> _carnivores = new List<Carnivore>();
    public static List<Carnivore> Carnivores => _carnivores;

    protected override void Awake()
    {
        base.Awake();
        Carnivores.Add(this);
    }

    public Type[] PreferedPrey;
    protected abstract void SetPrefferedPreyType(Type[] types);
    protected abstract bool Move_FindPrey(out Animal desiredPrey);
    protected abstract void Move_ChasePrey(Animal pery);
}
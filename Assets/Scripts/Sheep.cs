using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sheep : Herbivore
{
    [SerializeField] private float findFoodDistance;
    protected override void Init()
    {
        InvokeRepeating("DecideMoveTarget",1,1.0f);
    }

    protected override Creature Reproduce()
    {
        var child = Instantiate(Resources.Load<GameObject>("Prefabs/Sheep"));
        child.transform.position = transform.position;
        return child.GetComponent<Sheep>();
    }

    protected override void LivingActivities()
    {
        base.LivingActivities();
        if (!matured)
        {
            if (lifeTime < matureTime)
            {
                transform.localScale = Vector3.one * (.2f + lifeTime / matureTime * .8f);
            }
            else
            {
                matured = true;
            }
        }
        else
        {
            reproduceTimer += Time.deltaTime;
            if (reproduceTimer > reproduceInterval)
            {
                Reproduce();
                reproduceTimer = 0;
            }
        }
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    protected override void Eat(Creature food)
    {
        hungry = Mathf.Clamp(hungry - food.satiety, 0, maxHungry);
        if (hungry < hungryLevel_SearchForFood)
            willingToEat = false;
        
        Kill(food);
    }

    protected override void DecideMoveTarget()
    {
        if (hungry < hungryLevel_SearchForFood)
        {
            FindRandomPlaceToGo();
            return;
        }

        Plant food;
        if (hungry < hungryLevel_EagerForFood)
        {
            int d2Result = Random.Range(0, 2);
            if (d2Result > 0)
            {
                FindRandomPlaceToGo();
                return;
            }
            food = FindFood(new []{typeof(Grass)});
            if (food)
            {
                willingToEat = true;
                SetMoveTarget(food.transform.position, maxMoveSpeed * .5f);
            }
            else
                FindRandomPlaceToGo();
            return;
        }
        
        food = FindFood(new []{typeof(Grass)});
        if (food)
            {
                willingToEat = true;
                SetMoveTarget(food.transform.position, maxMoveSpeed);
            }
        else
            FindRandomPlaceToGo();
    }

    private void FindRandomPlaceToGo()
    {
        Bounds bounds = helper.util.SpriteUtility.GetSpriteWorldBounds(GameObject.Find("Ground").GetComponent<SpriteRenderer>());
        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            transform.position.z
        );
        SetMoveTarget(randomPoint,maxMoveSpeed * .2f);
    }

    protected override Animal Breed(Animal mate)
    {
        throw new NotImplementedException();
    }

    protected override Plant FindFood(Type[] targetTypes)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, findFoodDistance);
        foreach (Collider collider in colliders) {
            if (collider.CompareTag("Grass"))
                return collider.GetComponent<Grass>();
        }

        if (Grass.grasses.Count != 0)
            return Grass.grasses[0];
        return null;
    }

    protected override bool DetectCarnivores(out Carnivore[] carnivores)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
        List<Carnivore> carnivoreList = new();
        foreach (Collider col in colliders)
            if (col.CompareTag("Wolves"))
                carnivoreList.Add(col.gameObject.GetComponent<Carnivore>());

        
        if (carnivoreList.Count == 0)
        {
            carnivores = null;
            return false;
        }
        else
        {
            carnivores = carnivoreList.ToArray();
            return true;
        }
    }

    protected override bool DetectCarnivores(out Carnivore carnivore)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Wolves"))
            {
                carnivore = col.gameObject.GetComponent<Carnivore>();
                return true;
            }
        }

        carnivore = null;
        return false;
    }

    protected override void Move_Escape()
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("collides something");
        if (!willingToEat) return;
        if (other == null) return;
        if (!other.CompareTag("Grass")) return;
        Eat(other.GetComponent<Grass>());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        print("collides something");
        if (!willingToEat) return;
        if (other == null) return;
        if (!other.collider.CompareTag("Grass")) return;
        Eat(other.collider.GetComponent<Grass>());
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wolf : Carnivore
{
    protected override Animal Breed(Animal mate)
    {
        throw new NotImplementedException();
    }

    protected override void DecideMoveTarget()
    {
        if (hungry < hungryLevel_SearchForFood)
        {
            FindRandomPlaceToGo();
            return;
        }

        if (hungry < hungryLevel_EagerForFood)
        {
            if (Move_FindPrey(out var desiredPrey))
            {
                if (desiredPrey)
                {
                    willingToEat = true;
                    SetMoveTarget(desiredPrey.transform.position, maxMoveSpeed * .5f);
                    return;
                }
                
                FindRandomPlaceToGo();
                return;
            }
            else
            {
                FindRandomPlaceToGo();
                return;
            }
        }
    }

    private void FindRandomPlaceToGo()
    {
        Bounds bounds = helper.util.SpriteUtility.GetSpriteWorldBounds(GameObject.Find("Ground").GetComponent<SpriteRenderer>());
        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            transform.position.z
        );
        SetMoveTarget(randomPoint, maxMoveSpeed * .2f);
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

    protected override void Init()
    {
        InvokeRepeating("DecideMoveTarget", 1, 1.0f);
    }

    protected override void Move_ChasePrey(Animal pery)
    {
        throw new NotImplementedException();
    }

    protected override bool Move_FindPrey(out Animal desiredPrey)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Sheeps"))
            {
                desiredPrey = collider.GetComponent<Sheep>();
                return true;
            }
        }

        if (Grass.grasses.Count != 0)
        {
            desiredPrey = Sheep.Herbivores[0];
            return true;
        }

        desiredPrey = null;
        return false;
    }

    protected override Creature Reproduce()
    {
        var child = Instantiate(Resources.Load<GameObject>("Prefabs/Wolf"));
        child.transform.position = transform.position;
        return child.GetComponent<Wolf>();
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

    protected override void SetPrefferedPreyType(Type[] types)
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("collides something");
        if (!willingToEat) return;
        if (other == null) return;
        if (!other.CompareTag("Sheeps")) return;
        Eat(other.GetComponent<Sheep>());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        print("collides something");
        if (!willingToEat) return;
        if (other == null) return;
        if (!other.collider.CompareTag("Sheeps")) return;
        Eat(other.collider.GetComponent<Sheep>());
    }
}

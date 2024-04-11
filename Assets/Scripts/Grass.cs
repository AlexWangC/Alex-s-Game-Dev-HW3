using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grass : Plant
{
    private static GameObject _prefab;
    public static List<Grass> grasses = new();
    public static int amountLimit = 50;

    [SerializeField] private SpriteRenderer ground;

    private static GameObject grassPrefab
    {
        get
        {
            if (!_prefab)
                _prefab = Resources.Load<GameObject>("Prefabs/Grass");
            return _prefab;
        }
    }
    
    [SerializeField] private bool isMadeOfPrivateShapes;
    private SpriteRenderer[] primitiveSpriteRenderers;
    private int growPhase = 0;
    protected override void Init()
    {
        grasses.Add(this);
        if (isMadeOfPrivateShapes)
            primitiveSpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        ground = GameObject.Find("Ground").GetComponent<SpriteRenderer>();
    }

    protected override Creature Reproduce()
    {
        if (grasses.Count > amountLimit)
            return null;
        Bounds bounds = helper.util.SpriteUtility.GetSpriteWorldBounds(ground);
        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            transform.position.z // 保持与原始位置相同的 z 值
        );
        return Seed.CreateNewSeed(Sprout,randomPoint,5f).SetLifeTime(100f);
    }

    public static Grass Sprout() => Instantiate(grassPrefab).GetComponent<Grass>();
    
    protected override void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        grasses.Remove(this);
    }

    protected override void Grow()
    {
        if (!matured)
            transform.localScale = Vector3.one * (.2f + lifeTime / matureTime * .8f);
        if (lifeTime / maxLifeTime * 10f > growPhase)
        {
            //change color
            if (isMadeOfPrivateShapes)
            {
                growPhase++;
                Color brown = new Color(245, 222, 179);
                foreach (var spriteRenderer in primitiveSpriteRenderers)
                    spriteRenderer.color = Color.Lerp(Color.green, Color.red, (lifeTime-matureTime) / maxLifeTime / 1.5f);
            }
            else
            {
                
            }
        }
    }
    
}

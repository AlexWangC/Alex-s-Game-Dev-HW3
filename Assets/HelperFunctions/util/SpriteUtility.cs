using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper.util
{
    public static class SpriteUtility
    {
        public static Bounds GetSpriteWorldBounds(SpriteRenderer spriteRenderer)
        {
            Bounds localBounds = spriteRenderer.sprite.bounds;

            // 将本地边界框的中心点转换为世界空间
            Vector3 center = spriteRenderer.transform.TransformPoint(localBounds.center);

            // 将本地边界框的大小（乘以缩放比例）转换为世界空间
            Vector3 size = Vector3.Scale(spriteRenderer.transform.lossyScale, localBounds.size);

            // 创建一个新的 Bounds 对象，表示 Sprite 在世界空间中的边界框
            Bounds worldBounds = new Bounds(center, size);

            return worldBounds;
        }
    }
}


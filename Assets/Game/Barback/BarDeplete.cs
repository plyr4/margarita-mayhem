using System.Collections.Generic;
using UnityEngine;

public static class BarDeplete
{
    public static float DepleteValue(float value, float speed)
    {
        if (value > 0f)
        {
            value -= Time.deltaTime * speed;
            if (value < 0f)
            {
                value = 0f;
            }
        }

        return value;
    }

    public static int GetDepletionSpriteIndex(float value, List<Sprite> sprites)
    {
        int spriteCount = sprites.Count;
        int spriteIndex = spriteCount - 1;
        if (spriteCount > 1)
        {
            float step = 1f / (spriteCount - 1);
            for (int i = 0; i < spriteCount; i++)
            {
                if (value >= 1f - i * step)
                {
                    spriteIndex = i;
                    break;
                }
            }
        }

        return spriteIndex;
    }
}
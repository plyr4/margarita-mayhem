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
        if (spriteCount <= 1) return 0;

        if (value <= 0f) return spriteCount - 1;

        float step = 1f / (spriteCount - 1);
        for (int i = 0; i < spriteCount - 1; i++)
        {
            if (value > (spriteCount - 2 - i) * step)
            {
                return i;
            }
        }

        return spriteCount - 1;
    }
}
using TMPro;

public static class SpriteTextWriter
{
    public static void WriteText(TextMeshPro textMeshPro, string text)
    {
        string spriteText = "";
        foreach (char c in text)
        {
            if (c >= 'a' && c <= 'z')
            {
                int index = c - 'a';
                spriteText += $"<sprite={index}>";
            }
            else if (c >= '0' && c <= '9')
            {
                int index = c - '0' + 26;
                spriteText += $"<sprite={index}>";
            }
            else if (c == '.')
            {
                spriteText += "<sprite=36>";
            }
            else if (c == ',')
            {
                spriteText += "<sprite=37>";
            }
            else if (c == '!')
            {
                spriteText += "<sprite=38>";
            }
            else if (c == '?')
            {
                spriteText += "<sprite=39>";
            }
            else
            {
                // ignore other characters
                continue;
            }
        }

        textMeshPro.text = spriteText;
    }
}
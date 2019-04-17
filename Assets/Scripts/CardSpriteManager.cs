using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardSpriteManager
{
    public static Dictionary<string, Sprite> spriteDic;
    public static Dictionary<string, Sprite> bigspriteDic;
    private string picspath;

    public static void Initialize()
    {
        spriteDic = new Dictionary<string, Sprite>();
        bigspriteDic = new Dictionary<string, Sprite>();
    }

    public CardSpriteManager()
    {
        if (Application.platform == RuntimePlatform.Android)
            picspath = Main.AndroidSdcard + "/" + Main.rule + "/pics/";
        else
            picspath = Main.streamAssetsPath + "/" + Main.rule + "/pics/";
    }

    public Sprite getCardSprite(string id, bool small)
    {
        if (spriteDic.ContainsKey(id) && small)
            return spriteDic[id];
        if (bigspriteDic.ContainsKey(id) && !small)
            return bigspriteDic[id];
        string cardpath = picspath + id + ".jpg";
        Sprite sprite = null;
        if (File.Exists(cardpath))
        {
            FileStream files = new FileStream(cardpath, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            Texture2D texture;
            if (Application.platform == RuntimePlatform.Android)
                texture = new Texture2D(236, 344, TextureFormat.ETC_RGB4, false);
            else
                texture = new Texture2D(236, 344, TextureFormat.DXT1, false);
            texture.LoadImage(imgByte);
            if (small) texture = ScaleTexture(texture, 59, 86);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (small) spriteDic.Add(id, sprite);
            else
            {
                if (bigspriteDic.Count >= 100)
                {
                    bigspriteDic.Clear();//大图最多保存100张
                    Resources.UnloadUnusedAssets();
                }
                bigspriteDic.Add(id, sprite);
            }
        }
        return sprite;
    }

    public Texture2D ScaleTexture(Texture2D sourceTex, int targetWidth, int targetHeight)
    {
        Texture2D destTex = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        Color[] destPix = new Color[destTex.width * destTex.height];

        // For each pixel in the destination texture...
        for (int y = 0; y < destTex.height; y++)
        {
            for (int x = 0; x < destTex.width; x++)
            {
                // Calculate the fraction of the way across the image
                // that this pixel positon corresponds to.
                float xFrac = x * 1.0f / (destTex.width - 1);
                float yFrac = y * 1.0f / (destTex.height - 1);

                // Get the non-integer pixel positions using GetPixelBilinear.
                destPix[y * destTex.width + x] = sourceTex.GetPixelBilinear(xFrac, yFrac);
            }
        }

        // Copy the pixel data to the destination texture and apply the change.
        destTex.SetPixels(destPix);
        destTex.Apply();
        Object.Destroy(sourceTex);
        return destTex;
    }
}

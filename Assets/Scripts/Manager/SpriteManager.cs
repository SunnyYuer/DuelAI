using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteManager
{
    public static Dictionary<string, Sprite> spriteDic;
    public static Dictionary<string, Sprite> bigspriteDic;
    public static Dictionary<string, Sprite> textureDic;
    private string picspath;
    private string texturespath;

    public static void Initialize()
    {
        spriteDic = new Dictionary<string, Sprite>();
        bigspriteDic = new Dictionary<string, Sprite>();
        textureDic = new Dictionary<string, Sprite>();
    }

    public SpriteManager()
    {
        picspath = Main.rulePath + "/pics/";
        texturespath = Main.rulePath + "/textures/";
    }

    public Sprite GetCardSprite(string id, bool small)
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
            Texture2D texture = null;
#if UNITY_EDITOR || UNITY_STANDALONE
            texture = new Texture2D(236, 344, TextureFormat.DXT1, false);
#elif UNITY_ANDROID
            texture = new Texture2D(236, 344, TextureFormat.ETC_RGB4, false);
#elif UNITY_IOS
            texture = new Texture2D(236, 344, TextureFormat.PVRTC_RGB4, false);
#endif
            texture.LoadImage(imgByte);
            if (small) texture = ScaleTexture(texture, 59, 86);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (small) spriteDic.Add(id, sprite);
            else
            {
                if (bigspriteDic.Count >= 200)
                {
                    bigspriteDic.Clear();//大图最多保存200张
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

    public Sprite GetTextureSprite(string name)
    {
        if (textureDic.ContainsKey(name))
            return textureDic[name];
        string texturepath = texturespath + name + ".jpg";
        Sprite sprite = null;
        if (File.Exists(texturepath))
        {
            FileStream files = new FileStream(texturepath, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            // Texture2D大小可任意填，LoadImage后大小会被替换
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imgByte);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            textureDic.Add(name, sprite);
        }
        return sprite;
    }
}

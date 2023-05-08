using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility {
    public static class TextureExtensions {
        public static ReadOnlySpan<Color32> GetPixels32Reliable(this Texture2D texture, int mipLevel = 0) {
            return
                texture.isReadable ?
                    texture.GetPixelData<Color32>(mipLevel).AsReadOnlySpan() :
                    GetPixels32WithRenderTexture(texture, mipLevel);
        }

        private static Color32[] GetPixels32WithRenderTexture(this Texture2D texture, int mipLevel = 0) {
            int mipWidth = Mathf.Max(1, texture.width >> mipLevel);
            int mipHeight = Mathf.Max(1, texture.height >> mipLevel);

            Texture2D tempTexture = new(mipWidth, mipHeight, TextureFormat.ARGB32, false);

            GL.PushMatrix();
            Rect textureRect = new(0f, 0f, mipWidth, mipHeight);
            RenderTexture rt = RenderTexture.GetTemporary(mipWidth, mipHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            Graphics.Blit(texture, rt);
            RenderTexture.active = rt;
            tempTexture.ReadPixels(textureRect, 0, 0, false);
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            GL.PopMatrix();

            Color32[] pixelsBuffer = tempTexture.GetPixels32(0);
            Object.DestroyImmediate(tempTexture);

            return pixelsBuffer;
        }
    }
}

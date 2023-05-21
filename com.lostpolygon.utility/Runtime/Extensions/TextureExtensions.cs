using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility {
    public static class TextureExtensions {
        public static NativeArray<Color32> GetPixels32Reliable(this Texture2D texture, int mipLevel = 0) {
            return
                texture.isReadable ?
                    texture.GetPixelData<Color32>(mipLevel) :
                    GetPixels32WithRenderTexture(texture, mipLevel);
        }

        private static NativeArray<Color32> GetPixels32WithRenderTexture(this Texture2D texture, int mipLevel = 0) {
            int mipWidth = Mathf.Max(1, texture.width >> mipLevel);
            int mipHeight = Mathf.Max(1, texture.height >> mipLevel);
            Rect textureRect = new(0f, 0f, mipWidth, mipHeight);
            
            GL.PushMatrix();
            RenderTexture tempRt = RenderTexture.GetTemporary(
                mipWidth, mipHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, tempRt);
            RenderTexture prevRt = RenderTexture.active;
            RenderTexture.active = tempRt;
            Texture2D tempTexture = new(mipWidth, mipHeight, TextureFormat.ARGB32, false);
            tempTexture.ReadPixels(textureRect, 0, 0, false);
            RenderTexture.active = prevRt;
            RenderTexture.ReleaseTemporary(tempRt);
            GL.PopMatrix();

            NativeArray<Color32> pixelData = tempTexture.GetPixelData<Color32>(0);
            NativeArray<Color32> pixelDataCopy = new(pixelData, Allocator.Temp);
            Object.DestroyImmediate(tempTexture);

            return pixelDataCopy;
        }
    }
}

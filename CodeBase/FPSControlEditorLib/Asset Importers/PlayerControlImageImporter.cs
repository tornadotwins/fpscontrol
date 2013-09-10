using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FPSControlEditor;

namespace FPSControlEditor.AssetImporters
{
    public class PlayerControlImageImporter : AssetPostprocessor
    {

        public void OnPostprocessTexture(Texture2D texture)
        {
            if (assetPath.Contains(FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + InputEditorModule.PATH))
            {
                TextureImporter importer = (TextureImporter)assetImporter;
                importer.filterMode = FilterMode.Bilinear;
                importer.convertToNormalmap = false;
                importer.isReadable = false;
                importer.textureFormat = TextureImporterFormat.RGBA32;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 2048;
                importer.normalmap = false;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.textureType = TextureImporterType.GUI;
            }
        }

    }
}

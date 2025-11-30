using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RemapMaterial : MonoBehaviour
{
    public List<UnityEngine.Object> Objects;

    [ContextMenu("Remap")]
    public void Remap()
    {
        foreach(var x in Objects)
        {
            AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(x));
            if(importer is ModelImporter model)
            {
                model.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnTextureName, ModelImporterMaterialSearch.Everywhere);
            }
            importer.SaveAndReimport();
        }
    }
}

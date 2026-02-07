using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RemapMaterial : MonoBehaviour
{
#if UNITY_EDITOR
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
#endif
}

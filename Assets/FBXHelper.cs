using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FBXHelper : MonoBehaviour
{
#if UNITY_EDITOR
    public ModelImporterMaterialImportMode ImportMode;
	public ModelImporterMaterialLocation MaterialLocation;
	public ModelImporterMaterialSearch MaterialSearch;

	[Space]
	public List<Object> Assets;

    [ContextMenu("Change material import settings")]
    public void ChangeMaterialImport(MenuCommand command)
	{
		for(int i = 0; i < Assets.Count; i++)
		{
			string path = AssetDatabase.GetAssetPath(Assets[i]);
			if (AssetImporter.GetAtPath(path) is ModelImporter modelImporter)
			{
				modelImporter.materialImportMode = ImportMode;
				modelImporter.materialLocation = MaterialLocation;
				modelImporter.materialSearch = MaterialSearch;
			}
			else
			{
				Debug.LogWarning(path + " is not model!");
			}
		}
	}
#endif
}
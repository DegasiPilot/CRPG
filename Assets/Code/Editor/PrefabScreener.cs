using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
	[RequireComponent(typeof(Camera))]
	public class PrefabScreener : MonoBehaviour
	{
		public GameObject Prefab;
		public RenderTexture _rendererTexture;
		[SerializeField] private Camera _camera;

		[Header("Путь сохраненния")]
		[Tooltip("Относительно папки Asset, должен начинаться с обратного слэша /")]
		[SerializeField] private string _storagePath;

		[Header("Настройки спрайта")]
		[SerializeField] private TextureImporterCompression _compression;
		[Tooltip("Сфокусироваться на спрайте после создания?")]
		[SerializeField] private bool _needFocus;

		private void OnValidate()
		{
			if (_camera == null) TryGetComponent(out _camera);
		}

		[ContextMenu("Make picture")]
		void DoIt()
		{
			string fileName = Prefab.name + ".png";
			var stream = System.IO.File.Open(
				Application.dataPath + _storagePath + fileName,
				System.IO.FileMode.OpenOrCreate);
			try
			{
				_camera.targetTexture = _rendererTexture;
				_camera.Render();
				RenderTexture.active = _rendererTexture;
				var texture = new Texture2D(_rendererTexture.width, _rendererTexture.height);
				texture.ReadPixels(new Rect(Vector2.zero,
					new Vector2(_rendererTexture.width, _rendererTexture.height))
					,0,0);
				byte[] bytes = texture.EncodeToPNG();
				stream.Write(bytes, 0, bytes.Length);
			}
			catch
			{

			}
			stream.Dispose();
			string path = "Assets" + _storagePath + fileName;
			AssetDatabase.ImportAsset(path);
			var importer = AssetImporter.GetAtPath(path) as TextureImporter;
			if (importer != null)
			{
				importer.textureType = TextureImporterType.Sprite;
				importer.spriteImportMode = SpriteImportMode.Single;
				importer.alphaIsTransparency = true;
				importer.textureCompression = _compression;
				importer.SaveAndReimport();
			}
			if (_needFocus)
			{
				EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture>(path));
			}
		}
	}
}
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }

	[SerializeField] private AudioClip _mainAudioClip;
	[SerializeField] private AudioClip _battleAudioClip;

	private AudioSource _audioSource;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			_audioSource = GetComponent<AudioSource>();
			SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
			BattleManager.OnBattleStartEvent.AddListener(StartBattleMusic);
			BattleManager.OnBattleEndEvent.AddListener(StartMainMusic);
		}
	}

	private void SceneManager_activeSceneChanged(Scene lastScene, Scene newScene)
	{
		StopMusic();
		if(newScene.name == "MainScene")
		{
			StartMainMusic();
		}
	}

	public void StartMainMusic()
	{
		_audioSource.clip = _mainAudioClip;
		_audioSource.Play();
	}

	public void StartBattleMusic()
	{
		_audioSource.clip = _battleAudioClip;
		_audioSource.Play();
	}

	public void StopMusic()
	{
		_audioSource.Stop();
	}
}

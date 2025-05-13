using CRPG.DataSaveSystem;
using CRPG.DataSaveSystem.SaveData;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AuthRegManager : MonoBehaviour
{
	public InputField LoginInput;
	public InputField PasswordInput;
	public GameObject LoginPanel;
	public Toggle RememberMeToggle;
	public Text UserPanelLogin;
	public Button ExitButton;

	bool _isSearchInProgress;
	StringBuilder errors = new StringBuilder(2);
	IDataSaveLoader _dataSaveLoader;
	MessageBoxManager _messageBoxManager;

	internal void Activate(IDataSaveLoader dataSaveLoader, MessageBoxManager messageBoxManager)
	{
		_dataSaveLoader = dataSaveLoader;
		if (dataSaveLoader.IsUserLogined)
		{
			AfterUserInitialized(dataSaveLoader.UserLogin);
			return;
		}

		LoginPanel.SetActive(true);
		_messageBoxManager = messageBoxManager;
		User user = LocalCashManager.LoadUserCash();
		if (user != null)
		{
			TryEnter(user.Login, user.Password);
		}
	}

	private bool CheckFields()
	{
		if (string.IsNullOrWhiteSpace(LoginInput.text))
		{
			errors.AppendLine("Заполните поле логин");
		}
		if (string.IsNullOrWhiteSpace(PasswordInput.text))
		{
			errors.AppendLine("Заполните поле пароль");
		}
		if (errors.Length > 0)
		{
			errors.Remove(errors.Length - 1, 1);
			_messageBoxManager.ShowMessage(errors.ToString());
			errors.Clear();
			return false;
		}
		else
		{
			return true;
		}
	}

	public void TryEnter()
	{
		if (!_isSearchInProgress && CheckFields())
		{
			TryEnter(LoginInput.text, PasswordInput.text);
		}
	}

	private void TryEnter(string login, string password)
	{
		_isSearchInProgress = true;
		if (_dataSaveLoader.TryLogin(login, password, out string errors))
		{
			AfterUserInitialized(login, password);
		}
		else
		{
			_messageBoxManager.ShowMessage(errors);
		}
		_isSearchInProgress = false;
	}

	public void TryRegistrate()
	{
		if (!_isSearchInProgress && CheckFields())
		{
			_isSearchInProgress = true;
			if (_dataSaveLoader.TryRegistrate(LoginInput.text, PasswordInput.text, out string errors))
			{
				AfterUserInitialized(LoginInput.text, PasswordInput.text);
			}
			else
			{
				_messageBoxManager.ShowMessage(errors);
			}
			_isSearchInProgress = false;
		}
	}

	private void AfterUserInitialized(string login, string password)
	{
		if (RememberMeToggle.isOn)
		{
			LocalCashManager.SaveUserCash(login, password);
		}
		else
		{
			LocalCashManager.CleanCash();
		}
		AfterUserInitialized(login);
	}

	private void AfterUserInitialized(string login)
	{
		UserPanelLogin.text = login;
		ExitButton.interactable = _dataSaveLoader.CanExit;
		LoginPanel.SetActive(false);
	}

	public void ExitFromAccount()
	{
		LoginPanel.SetActive(true);
		LoginInput.text = "";
		PasswordInput.text = "";
		LocalCashManager.CleanCash();
	}
}
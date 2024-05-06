using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Diagnostics;

public class AuthRegManager : MonoBehaviour
{
    public UnityEvent OnUserEnter = new();
    public InputField LoginInput;
    public InputField PasswordInput;
    public GameObject LoginPanel;
    public Toggle RememberMeToggle;

    bool _isSearchInProgress;
    StringBuilder errors = new StringBuilder(2);

    private void Awake()
    {
        Process.Start(@"D:\Programs\mongodb-win32-x86_64-windows-6.0.13\bin\mongod.exe");
        User user = LocalCashManager.LoadUserCash();
        if(user != null)
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
            MessageBoxManager.ShowMessage(errors.ToString());
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

    private async void TryEnter(string login, string password)
    {
        _isSearchInProgress = true;
        User user = await CRUD.GetUserWithLoginAsync(login);
        if (user != null)
        {
            if (user.Password == password)
            {
                AfterUserInitialized(user);
            }
            else
            {
                MessageBoxManager.ShowMessage("Неверный пароль");
            }
        }
        else
        {
            MessageBoxManager.ShowMessage("Нет пользователя с таким логином");
        }
        _isSearchInProgress = false;
    }

    public async void TryRegistrate()
    {
        if (!_isSearchInProgress && CheckFields())
        {
            _isSearchInProgress = true;
            User user = await CRUD.GetUserWithLoginAsync(LoginInput.text);
            if (user == null)
            {
                user = new User()
                {
                    Login = LoginInput.text,
                    Password = PasswordInput.text,
                };
                CRUD.CreateUser(user);
                AfterUserInitialized(user);
            }
            else
            {
                MessageBoxManager.ShowMessage("Пользователь с таким логином уже существует");
            }
            _isSearchInProgress = false;
        }
    }

    private void AfterUserInitialized(User user)
    {
        GameData.CurrentUser = user;
        LoginPanel.SetActive(false);
        OnUserEnter.Invoke();
        if (RememberMeToggle.isOn)
        {
            LocalCashManager.SaveUserCash(user);
        }
        else
        {
            LocalCashManager.CleanCash();
        }
    }

    public void ExitFromAccount()
    {
        LoginPanel.SetActive(true);
        LoginInput.text = "";
        PasswordInput.text = "";
        LocalCashManager.CleanCash();
    }
}
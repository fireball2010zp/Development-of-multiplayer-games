using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.PackageManager;
using System.Linq;
using UnityEngine.SceneManagement;

public class Authorization : MonoBehaviour
{
    [SerializeField] private InputField _userNameField;
    [SerializeField] private InputField _userPasswordField;
    [SerializeField] private Button _registrationButton;
    [SerializeField] private Text _errorText;

    [SerializeField] private Slider Slider;
    [SerializeField] private GameObject Indicator;
    
    private string _userName;
    private string _userPassword;

    private void Awake()
    {
        _userNameField.onValueChanged.AddListener(SetUserName); // +=
        _userPasswordField.onValueChanged.AddListener(SetUserPassword);
        _registrationButton.onClick.AddListener(Submit);
        //LogInBackButton.onClick.AddListener(() => { panelManager.BackOnMainPanel(gameObject); });
    }

    private void SetUserName(string value)
    {
        _userName = value;
    }
    
    private void SetUserPassword(string value)
    {
        _userPassword = value;
    }

    public void Submit()
    {
        var slider = StartSlider();

        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _userName,
            Password = _userPassword,
        }, result =>
        {
            _errorText.gameObject.SetActive(false);
            Debug.Log($"User entered: {result.LastLoginTime}");

            StopSlider(slider);
            SceneManager.LoadScene(1); // go to Lobby Scene
        }, error =>
        {
            _errorText.gameObject.SetActive(true);
            if(error.ErrorDetails is not null)
            _errorText.text = error.ErrorDetails.FirstOrDefault().Value.FirstOrDefault() ?? "";
            Debug.LogError(error);
            StopSlider(slider);
        });
    }

    public Coroutine StartSlider()
    {
        return StartCoroutine(CorutineSlider());
    }

    public void StopSlider(Coroutine coroutine)
    {
        Indicator.SetActive(false);
        StopCoroutine(coroutine);
    }

    private IEnumerator CorutineSlider()
    {
        Indicator.SetActive(true);
        Slider.value = 0;

        for (int i = 0; i < 100; i++)
        {
            Slider.value = i;
            yield return new WaitForFixedUpdate();
        }

        Indicator.SetActive(false);
    }


}

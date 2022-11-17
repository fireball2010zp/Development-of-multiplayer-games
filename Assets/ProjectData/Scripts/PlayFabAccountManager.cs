using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private Text _titleLabel;
    [SerializeField] private Text _catalogLabel;

    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnFailure);

        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnFailure);
    }
    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        _titleLabel.text = $"Welcome back, Player ID {result.AccountInfo.PlayFabId}\n\n" +
            $"Name: {result.AccountInfo.Username ?? ""}\n" +
            $"Email: {result.AccountInfo.PrivateInfo.Email ?? ""}\n" +
            $"Last login: {result.AccountInfo.TitleInfo.LastLogin.ToString() ?? ""}\n";
    }
    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        // Debug.LogError($"Something went wrong: {errorMessage}");
        _titleLabel.text = $"Something went wrong: {errorMessage}";
        _catalogLabel.text = $"Something went wrong: {errorMessage}";
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
            HandleCatalog(result.Catalog);
            //Debug.Log($"Catalog was loaded successfully!");
    }

    private void HandleCatalog(List<CatalogItem> catalog)
    {
        var buildStr = new StringBuilder();
        foreach (var item in catalog)
        {
            buildStr.Append($"{item.DisplayName,50}  " +
                $"{string.Join("; ", item.VirtualCurrencyPrices.Select(v => $"{v.Value} {v.Key}")),20}   " +
                $"{item.Description,40}\n");
            _catalog.Add(item.ItemId, item);
            // Debug.Log($"Catalog item {item.ItemId} was added successfully!");
        }
        _catalogLabel.text = $"Game Store Catalog:\n\n" + 
            buildStr.ToString();
    }
}

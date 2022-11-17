using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogManager : MonoBehaviour
{
    private Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();
    
    public void Init()
    {
        var catalogRequest = new GetCatalogItemsRequest();
        PlayFabClientAPI.GetCatalogItems(catalogRequest, Success, Failure);
    }

    private void Success(GetCatalogItemsResult result)
    {
        foreach (var item in result.Catalog)
        {
            _catalog.Add(item.ItemId, item);
            Debug.Log($"Name item: {item.DisplayName}\n ID item: {item.ItemId}");
        }
    }

    private void Failure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    } 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ItemPickUp : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private GameObject pickUpText;

    public void Interact()
    {
        //EventBus.Instance.PauseGameplay();

        //DialougePrinter.Instance.PrintDialougeLine(_itemData.WorldDescription);

        EventBus.Instance.PickUpItem(_itemData);
        Destroy(gameObject);
    }

    public GameObject GetGameObject()
    {
        return pickUpText;
    }
    void OnDestroy()
    {
        if (pickUpText != null && pickUpText.activeSelf)
        {
            pickUpText.SetActive(false);
        }
    }
}

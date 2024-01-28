using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class ReadNotes : MonoBehaviour, IInteractable
{
    public GameObject player;
    public GameObject noteUI;
    public GameObject hud;
    public GameObject inv;

    public GameObject pickUpText;

    public AudioSource pickUpSound;
    

    public void Interact()
    {
        noteUI.SetActive(true);
        pickUpSound.Play();
        hud.SetActive(false);
        inv.SetActive(false);
        player.GetComponent<CharacterController>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public GameObject GetGameObject()
    {
        return pickUpText;
    }

    void Start()
    {
        noteUI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        pickUpText.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        pickUpText.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        pickUpText.SetActive(false);
    }
    public void ExitButton()
    {

        noteUI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        player.GetComponent<CharacterController>().enabled = true;

    }

    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }
}

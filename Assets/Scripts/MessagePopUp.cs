using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//simple pop up to give users feedback
public class MessagePopUp : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;

    public void Pop(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
    }
}

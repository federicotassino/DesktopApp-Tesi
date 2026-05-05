using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ListController : MonoBehaviour
{
    public MainScene mainScene;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI ShelvingUnit;
    public TextMeshProUGUI Description;
    public Button deleteButton;
    public Button updateButton;

    public void SetData(Artifact listItem, int index)
    {  
        Name.text = listItem.name;
        ShelvingUnit.text = listItem.shelvingUnit.ToString();
        Description.text = listItem.textDescription;
        deleteButton.onClick.AddListener(() => 
        { 
            mainScene.DeleteConfirmed(listItem);
            mainScene.scrollView.GetComponent<ScrollRectNoDrag>().enabled = false;
            mainScene.scrollView.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>().interactable = false;
        });
        updateButton.onClick.AddListener(() =>
        {
            mainScene.OpenEditPage(listItem);
        });
    }
}

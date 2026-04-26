using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ListController : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI ShelvingUnit;
    public TextMeshProUGUI Description;

    public void SetData(Artifact listItem)
    {  
        Name.text = listItem.Name;
        ShelvingUnit.text = listItem.ShelvingUnit.ToString();
        Description.text = listItem.TextDescription;
    }
}

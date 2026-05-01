using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ListGetData : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject itemPrefab2;
    public Transform contentTrf;
    public List<Artifact> itemData = new List<Artifact>();
    public Image sourceImageName;
    public Image sourceImageUnit;
    public Sprite downArrow;
    public Sprite upArrow;
    public Sprite noArrow;


    private bool naturalOrder = true;
    private bool orderByName = true;
    private bool orderByShUnit = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void UpdateListView()
    {
        ClearListView(contentTrf.gameObject);

        ReorderListView();

        bool isEven = true;

        foreach (var item in itemData)
        {
            GameObject obj;
            
            if (isEven)
                { obj = Instantiate(itemPrefab, contentTrf); }
            else
                { obj = Instantiate(itemPrefab2, contentTrf); }

            ListController listController = obj.GetComponent<ListController>();
            listController.SetData(item);

            isEven = !isEven;
        }
    }

    public void ClearListView(GameObject content)
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ReorderListView()
    {
        if (naturalOrder)
        {
            if (orderByName)
            { //itemData.Sort((x, y) => x.Name.CompareTo(y.Name));
                itemData = itemData.OrderBy(x => x.name).ThenBy(x => x.shelvingUnit).ToList();
            }

            if (orderByShUnit)
            { //itemData.Sort((x, y) => x.ShelvingUnit.CompareTo(y.ShelvingUnit));
                itemData = itemData.OrderBy(x => x.shelvingUnit).ThenBy(x => x.name).ToList();
            }
        }
        else
        {
            if (orderByName)
            { //itemData.Sort((x, y) => -x.Name.CompareTo(y.Name));
                itemData = itemData.OrderByDescending(x => x.name).ThenBy(x => x.shelvingUnit).ToList();
            }

            if (orderByShUnit)
            { //itemData.Sort((x, y) => -x.ShelvingUnit.CompareTo(y.ShelvingUnit));
                itemData = itemData.OrderByDescending(x => x.shelvingUnit).ThenBy(x => x.name).ToList();
            }
        }
    }

    public void ReorderByName()
    {
        Debug.Log("Reorder Button");
        if (!orderByName)
        {
            orderByName = true;
            naturalOrder = true;
            orderByShUnit = false;

            sourceImageName.sprite = downArrow;
            sourceImageUnit.sprite = noArrow;
        }
        else
        {
            naturalOrder = !naturalOrder;

            if(naturalOrder)
                sourceImageName.sprite = downArrow;
            else
                sourceImageName.sprite = upArrow;
        }

        UpdateListView();
    }

    public void ReorderByShUnit()
    {
        Debug.Log("Reorder Button");
        if (!orderByShUnit)
        {
            orderByShUnit = true;
            naturalOrder = true;
            orderByName = false;

            sourceImageName.sprite = noArrow;
            sourceImageUnit.sprite = downArrow;
        }
        else
        {
            naturalOrder = !naturalOrder;
            
            if (naturalOrder)
                sourceImageUnit.sprite = downArrow;
            else
                sourceImageUnit.sprite = upArrow;
        }
        
        UpdateListView();
    }
}

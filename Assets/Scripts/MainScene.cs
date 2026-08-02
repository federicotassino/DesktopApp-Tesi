using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    APIService apiService;
    public GameObject confirmDialog;
    public ListGetData listGetData;
    public GameObject scrollView;
    public GameObject topBar;
    public GameObject artifactPage;
    public TMP_InputField newArtifact_name;
    public TMP_InputField newArtifact_description;
    public TMP_InputField newArtifact_shUnit;
    //public Button addButton;
    public TMP_Text emptyFieldsText;
    public GameObject ShelfPage;
    public TMP_InputField shelfNameTMP;
    public TMP_InputField shelfParentIdTMP;

    private List<Artifact> artifacts = new();
    private Artifact selectedArtifact;
    private string confirmText = "Eliminare l'elemento ";

    [Header("Hierarchy")]
    public GameObject hierarchyPage;
    public Transform hierarchyContent;
    public GameObject treeNodePrefab;
    public GameObject nodeMenu;
    public GameObject indentPrefab;
    public GameObject createShelfButton;
    public GameObject createArtifactButton;


    private StorageContainer selectedShelf;

    // Start is called before the first frame update
    async void Start()
    {
        scrollView.SetActive(true);
        topBar.SetActive(true);
        artifactPage.SetActive(false);
        confirmDialog.SetActive(false);
        emptyFieldsText.enabled = false;
        ShelfPage.SetActive(false);
        hierarchyPage.SetActive(false);

        apiService = new APIService();

        artifacts = await apiService.GetAllArtifactsAsync();
        if (artifacts == null)
        {
            Debug.LogError("Artifacts null!");
            return;
        }

        createShelfButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateShelf();
        });

        createArtifactButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateArtifactButtonClick();
        });

        ToScrollView(artifacts);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (AddPage.activeSelf && !addButton.isActiveAndEnabled)
        {
            if (newArtifact_name.text != "" && newArtifact_description.text != "" && newArtifact_shUnit.text != "")
            {
                addButton.enabled = true;
            }
        }*/
    }

    private void ToScrollView(IEnumerable<Artifact> artifacts)
    {
        listGetData.itemData.Clear();
        foreach (var artifact in artifacts)
        {
            //ToConsole(artifact.ToString());

            //ScrollView
            listGetData.itemData.Add(artifact);
        }

        listGetData.UpdateListView();
    }

    private void ToConsole(string msg)
    {
        Debug.Log(msg);
    }


    #region[Handle UI Buttons]
    public async void OnGetAllArtifactsButtonClickAsync()
    {
        hierarchyPage.SetActive(false);
        scrollView.SetActive(true);
        topBar.SetActive(true);
        artifactPage.SetActive(false);
        ShelfPage.SetActive(false);

        //var artifacts = apiService.GetAllArtifacts();
        artifacts.Clear();
        artifacts = await apiService.GetAllArtifactsAsync();
        ToScrollView(artifacts);
    }

    public void OnCreateArtifactButtonClick()
    {
        Debug.Log("-----OnAddArtifactButtonClick-----");
        AddArtifactAsync(-1);
    }

    public void OnCreateArtifactButtonClickFromHierarchy()
    {
        Debug.Log("-----OnAddArtifactButtonClickFromHierarchy-----");
        AddArtifactAsync(selectedShelf.id);
    }

    public async void AddArtifactAsync(int _shelfID)
    {
        Debug.Log("-----OnAddArtifactButtonClick-----");
        hierarchyPage.SetActive(false);
        scrollView.SetActive(false);
        topBar.SetActive(false);
        artifactPage.SetActive(true);
        ShelfPage.SetActive(false);

        if (newArtifact_name.text != "" && newArtifact_description.text != "")
        {
            int shelfID = _shelfID;
            Artifact newArtifact = new Artifact
            {
                name = newArtifact_name.text,
                textDescription = newArtifact_description.text,
                shelvingUnit = shelfID,
                lastShelvingUnit = shelfID
            };

            Artifact created = await apiService.CreateArtifactAsync(newArtifact);

            if (created != null)
            {
                artifacts.Add(created);
                ToScrollView(artifacts);

                // reset UI
                ResetArtifactPage();
                artifactPage.SetActive(false);
                scrollView.SetActive(true);
                topBar.SetActive(true);
            }
            else
            {
                Debug.LogError("Errore durante il POST");
            }
        }
        else
        {
            emptyFieldsText.enabled = true;
        }

        createArtifactButton.GetComponent<Button>().onClick.RemoveAllListeners();
        createArtifactButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateArtifactButtonClick();
        });
    }

    public void OnAddArtifactButtonClick()
    {
        Debug.Log("-----OnAddArtifactButtonClick-----");
        hierarchyPage.SetActive(false);
        scrollView.SetActive(false);
        topBar.SetActive(false);
        artifactPage.SetActive(true);
        ShelfPage.SetActive(false);
    }

    public void OnAddShelfButtonClick()
    {
        Debug.Log("-----OnAddShelfButtonClick-----");
        hierarchyPage.SetActive(false);
        scrollView.SetActive(false);
        topBar.SetActive(false);
        artifactPage.SetActive(false);
        ShelfPage.SetActive(true);
    }


    public async void OnHierarchyButtonClick()
    {
        Debug.Log("-----OnHierarchyButtonClick-----");
        hierarchyPage.SetActive(true);
        scrollView.SetActive(false);
        topBar.SetActive(false);
        artifactPage.SetActive(false);
        ShelfPage.SetActive(false);

        List<StorageContainer> shelves =
            await apiService.GetAllShelvesAsync();

        BuildHierarchy(shelves);
    }

    public void OnArtifactTableButtonClick()
    {
        Debug.Log("-----OnArtifactTableButtonClick-----");

        //apiService.CreateArtifactTable();
    }

    public void OnCloseNodeMenu()
    {
        Debug.Log("-----OnCloseNodeMenu-----");
        nodeMenu.SetActive(false);
    }

    public void OnAddShelfFromHierarchy()
    {
        Debug.Log("-----OnAddShelfFromHierarchy-----");
        nodeMenu.SetActive(false);

        OnAddShelfButtonClick();

        createShelfButton.GetComponent<Button>().onClick.RemoveAllListeners();
        createShelfButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateShelfFromHierarchy();
        });
    }

    public void OnAddArtifactFromHierarchy()
    {
        Debug.Log("-----OnAddArtifactFromHierarchy-----");
        nodeMenu.SetActive(false);

        OnAddArtifactButtonClick();

        createArtifactButton.GetComponent<Button>().onClick.RemoveAllListeners();
        createArtifactButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateArtifactButtonClickFromHierarchy();
        });
    }
    #endregion[Handle UI Buttons]


    //////////////////////////////////////

    private void BuildHierarchy(List<StorageContainer> shelves)
    {
        foreach (Transform child in hierarchyContent)
            Destroy(child.gameObject);

        foreach (StorageContainer root in shelves.Where(x => x.parentShelfId == -1))
        {
            CreateNode(root, shelves, 0);
        }
    }

    private void CreateNode(StorageContainer shelf, List<StorageContainer> allShelves, int depth)
    {
        GameObject node = Instantiate(treeNodePrefab, hierarchyContent);

        // Aggiunge gli indent all'inizio
        for (int i = 0; i < depth; i++)
        {
            GameObject indent = Instantiate(indentPrefab, node.transform);

            // lo mette prima degli altri elementi
            indent.transform.SetSiblingIndex(0);
        }

        // Testo
        node.GetComponentsInChildren<TMP_Text>().FirstOrDefault(x => x.gameObject.name == "Label").text = $"{shelf.name} ({shelf.id})";

        // Click
        node.GetComponentsInChildren<Button>().FirstOrDefault().onClick.AddListener(() =>
        {
            OnShelfNodeClicked(shelf);
        });

        // Figli
        foreach (StorageContainer child in allShelves.Where(x => x.parentShelfId == shelf.id))
        {
            CreateNode(child, allShelves, depth + 1);
        }
    }

    private void OnShelfNodeClicked(StorageContainer shelf)
    {
        selectedShelf = shelf;

        nodeMenu.SetActive(true);
        nodeMenu.transform.Find("Text").GetComponent<TMP_Text>().text = shelf.name;
    }

    //////////////////////////////////////

    public void ResetArtifactPage()
    {
        newArtifact_name.text = "";
        newArtifact_description.text = "";
        newArtifact_shUnit.text = "";
        emptyFieldsText.enabled = false;
        artifactPage.transform.Find("AddButton").gameObject.SetActive(true);
        artifactPage.transform.Find("UpdateButton").gameObject.SetActive(false);
    }


    public void DeleteConfirmed(Artifact artifact)
    {
        confirmDialog.SetActive(true);
        confirmDialog.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = confirmText + $"<b>{artifact.name}</b>" + "?";
        Button deleteBtn = confirmDialog.transform.Find("Confirm Button").GetComponent<Button>();
        deleteBtn.onClick.RemoveAllListeners();
        deleteBtn.onClick.AddListener(() => 
        {
            OnDeleteArtifactAsync(artifact.id);
        });
    }

    public async void OnDeleteArtifactAsync(int id)
    {
        bool success = await apiService.DeleteArtifactAsync(id);

        if (success)
        {
            //Rimozione dalla lista locale artifacts
            Artifact toRemove = artifacts.Find(a => a.id == id);

            if (toRemove != null)
            {
                artifacts.Remove(toRemove);
                ToScrollView(artifacts);
            }
        }

        //reset UI
        ResetUIAfterDelete();
    }

    public void ResetUIAfterDelete()
    {
        confirmDialog.SetActive(false);
        scrollView.GetComponent<ScrollRectNoDrag>().enabled = true;
        scrollView.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>().interactable = true;
    }

    public void OpenEditPage(Artifact artifact)
    {
        selectedArtifact = artifact;

        newArtifact_name.text = artifact.name;
        newArtifact_description.text = artifact.textDescription;
        newArtifact_shUnit.text = artifact.shelvingUnit.ToString();

        artifactPage.transform.Find("AddButton").gameObject.SetActive(false);
        artifactPage.transform.Find("UpdateButton").gameObject.SetActive(true);
        artifactPage.SetActive(true);
        scrollView.SetActive(false);
        topBar.SetActive(false);
    }

    public async void OnUpdateArtifactButtonClickAsync()
    {
        if (selectedArtifact == null)
            return;

        selectedArtifact.name = newArtifact_name.text;
        selectedArtifact.textDescription = newArtifact_description.text;
        selectedArtifact.shelvingUnit = Int32.Parse(newArtifact_shUnit.text);
        selectedArtifact.lastShelvingUnit = Int32.Parse(newArtifact_shUnit.text);

        Artifact updated = await apiService.UpdateArtifactAsync(selectedArtifact);

        if (updated != null)
        {
            //aggiorna lista locale
            int index = artifacts.FindIndex(a => a.id == updated.id);

            if (index != -1)
            {
                artifacts[index] = updated;
                ToScrollView(artifacts);
            }

            ResetArtifactPage();
            artifactPage.SetActive(false);
            scrollView.SetActive(true);
            topBar.SetActive(true);
        }
    }

    public void OnGeArtifactByNameButtonClick()
    {
        //var artifacts = apiService.GetArtifactByName("Scarabeo");
        //ToConsole(artifacts.ToString());
    }

    public void OnCreateShelf()
    {
        string name = shelfNameTMP.text;
            //ShelfPage.transform
            //.Find("Name_txt")
            //.GetComponent<TMP_InputField>()
            //.text

        StorageContainer dto = new StorageContainer
        {
            name = name,
            worldTransform = "",
            parentShelfId = -1, // By default create a shelf with no parent (top of the hierarchy).
            isRoom = false,
            markerId = -1,
            roomWidth = 0,
            roomHeight = 0,
            roomDepth = 0,
            roomCenterPose = "",
        };

        _ = apiService.CreateShelf(dto);

        ResetShelfUI();
    }

    public void OnCreateShelfFromHierarchy()
    {
        string name = shelfNameTMP.text;
        //ShelfPage.transform
        //.Find("Name_txt")
        //.GetComponent<TMP_InputField>()
        //.text
        int parentId = selectedShelf != null ? selectedShelf.id : -1;

        StorageContainer dto = new StorageContainer
        {
            name = name,
            worldTransform = "",
            parentShelfId = parentId,
            isRoom = false,
            markerId = -1,
            roomWidth = 0,
            roomHeight = 0,
            roomDepth = 0,
            roomCenterPose = "",
        };

        _ = apiService.CreateShelf(dto);

        ResetShelfUI();

        createShelfButton.GetComponent<Button>().onClick.RemoveAllListeners();
        createShelfButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateShelf();
        });
    }

    private void ResetShelfUI()
    {
        shelfNameTMP.text = "";
        shelfParentIdTMP.text = "";
        ShelfPage.SetActive(false);
        scrollView.SetActive(true);
        topBar.SetActive(true);
    }

    public void UpdateArtifact(Artifact artifact)
    {
        Debug.Log("Chiamata funzione UpdateArtifact");

        int index = artifacts.FindIndex(x => x.id == artifact.id);
        if (index != -1)
        {
            artifacts[index] = artifact;
        }
        ToScrollView(artifacts);
    }

    public void UpdateShelf(StorageContainer shelf)
    {
        Debug.Log("Chiamata funzione UpdateShelf");
    }
}

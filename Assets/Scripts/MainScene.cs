using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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
    public GameObject containerPage;
    public TMP_InputField containerNameTMP;

    [Header("Header Buttons")]
    public GameObject getAllArtifactsButton;
    public GameObject hierarchyButton;
    public GameObject addContainerButton;
    public GameObject addArtifactButton;

    [Header("Hierarchy")]
    public GameObject hierarchyPage;
    public Transform hierarchyContent;
    public GameObject treeNodePrefab;
    public GameObject nodeMenu;
    public GameObject indentPrefab;

    [Header("Container")]
    public GameObject createContainerButton;
    public GameObject createArtifactButton;
    public GameObject roomSizeInputFields;
    public Toggle isRoomToggle;
    public TMP_InputField h_input;
    public TMP_InputField w_input;
    public TMP_InputField d_input;
    public TMP_Text errorText;

    private List<Artifact> artifacts = new();
    private Artifact selectedArtifact;
    private string confirmText = "Delete the element ";

    private StorageContainer selectedContainer;

    // Start is called before the first frame update
    async void Start()
    {
        scrollView.SetActive(true);
        topBar.SetActive(true);
        artifactPage.SetActive(false);
        confirmDialog.SetActive(false);
        emptyFieldsText.enabled = false;
        containerPage.SetActive(false);
        hierarchyPage.SetActive(false);
        nodeMenu.SetActive(false);
        roomSizeInputFields.SetActive(false);
        errorText.gameObject.SetActive(false);

        apiService = new APIService();

        artifacts = await apiService.GetAllArtifactsAsync();
        if (artifacts == null)
        {
            Debug.LogError("Artifacts null!");
            return;
        }
        getAllArtifactsButton.GetComponent<Button>().Select();

        createContainerButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateContainer();
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
        containerPage.SetActive(false);

        getAllArtifactsButton.GetComponent<Button>().Select();

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
        AddArtifactAsync(selectedContainer.id);
    }

    public async void AddArtifactAsync(int _containerID)
    {
        Debug.Log("-----OnAddArtifactButtonClick-----");
        hierarchyPage.SetActive(false);
        scrollView.SetActive(false);
        topBar.SetActive(false);
        artifactPage.SetActive(true);
        containerPage.SetActive(false);

        if (newArtifact_name.text != "" && newArtifact_description.text != "")
        {
            int containerID = _containerID;
            Artifact newArtifact = new Artifact
            {
                name = newArtifact_name.text,
                textDescription = newArtifact_description.text,
                shelvingUnit = containerID,
                lastShelvingUnit = containerID
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
        containerPage.SetActive(false);

        addArtifactButton.GetComponent<Button>().Select();
    }

    public void OnAddContainerButtonClick()
    {
        Debug.Log("-----OnAddContainerButtonClick-----");
        hierarchyPage.SetActive(false);
        scrollView.SetActive(false);
        topBar.SetActive(false);
        artifactPage.SetActive(false);
        containerPage.SetActive(true);

        addContainerButton.GetComponent<Button>().Select();

        isRoomToggle.isOn = false;
        roomSizeInputFields.SetActive(false);
        h_input.text = "";
        w_input.text = "";
        d_input.text = "";
    }


    public async void OnHierarchyButtonClick()
    {
        Debug.Log("-----OnHierarchyButtonClick-----");
        hierarchyPage.SetActive(true);
        scrollView.SetActive(false);
        topBar.SetActive(false);
        artifactPage.SetActive(false);
        containerPage.SetActive(false);

        hierarchyButton.GetComponent<Button>().Select();

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

    public void OnAddContainerFromHierarchy()
    {
        Debug.Log("-----OnAddContainerFromHierarchy-----");
        nodeMenu.SetActive(false);

        OnAddContainerButtonClick();

        createContainerButton.GetComponent<Button>().onClick.RemoveAllListeners();
        createContainerButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateContainerFromHierarchy();
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

    public void OnIsRoomToggled()
    {
        Debug.Log("-----OnIsRoomToggled-----");
        roomSizeInputFields.SetActive(isRoomToggle.isOn);
    }

    public async void OnDeleteContainerClicked()
    {
        if (selectedContainer == null)
            return;

        bool success = await apiService.DeleteContainerAsync(selectedContainer.id);

        if (!success)
            return;

        // Aggiorna la gerarchia
        List<StorageContainer> shelves =
            await apiService.GetAllShelvesAsync();

        BuildHierarchy(shelves);

        nodeMenu.SetActive(false);

        selectedContainer = null;
    }

    private async void OnContainerNodeClicked(StorageContainer container)
    {
        selectedContainer = container;

        nodeMenu.SetActive(true);
        List<Artifact> artifacts = await GetArtifactsInContainer(container);

        nodeMenu.transform.Find("Text").GetComponent<TMP_Text>().text =
            $"{container.name} (ID: {container.id})\nNumber of Artifacts: {artifacts.Count}";
    }
    #endregion[Handle UI Buttons]


    //////////////////////////////////////

    private void BuildHierarchy(List<StorageContainer> shelves)
    {
        foreach (Transform child in hierarchyContent)
            Destroy(child.gameObject);

        List<StorageContainer> roots = shelves
            .Where(x => x.parentShelfId == -1)
            .ToList();

        for (int i = 0; i < roots.Count; i++)
        {
            CreateNode(
                roots[i],
                shelves,
                0,
                i == roots.Count - 1
            );
        }
    }

    private void CreateNode(StorageContainer container, List<StorageContainer> allShelves, int depth, bool isLastSibling)
    {
        GameObject node = Instantiate(treeNodePrefab, hierarchyContent);

        // =====================================================
        // ANTENATI
        // =====================================================

        List<StorageContainer> ancestors = new List<StorageContainer>();

        StorageContainer current = container;

        while (current.parentShelfId != -1)
        {
            StorageContainer parent = allShelves.FirstOrDefault(x => x.id == current.parentShelfId);

            if (parent == null)
                break;

            ancestors.Insert(0, parent);
            current = parent;
        }

        // =====================================================
        // INDENT
        // =====================================================

        for (int i = 0; i < depth; i++)
        {
            GameObject indent = Instantiate(indentPrefab, node.transform);

            // Un indent per livello
            indent.transform.SetSiblingIndex(i);

            Transform orizontalLine = indent.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "OrizontalLine");

            Transform verticalLine = indent.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "VerticalLine");

            Transform angle = indent.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "Angle");

            // =================================================
            // RESET
            // =================================================

            orizontalLine?.gameObject.SetActive(false);
            verticalLine?.gameObject.SetActive(false);
            angle?.gameObject.SetActive(false);

            // =================================================
            // ULTIMO LIVELLO
            // =================================================

            if (i == depth - 1)
            {
                if (isLastSibling)
                {
                    // └──
                    angle?.gameObject.SetActive(true);
                    orizontalLine?.gameObject.SetActive(false);
                    verticalLine?.gameObject.SetActive(false);
                }
                else
                {
                    // ├──
                    angle?.gameObject.SetActive(false);
                    orizontalLine?.gameObject.SetActive(true);
                    verticalLine?.gameObject.SetActive(true);
                }

                continue;
            }

            // =================================================
            // LIVELLO DI UN ANTENATO
            // =================================================

            if (i >= ancestors.Count)
                continue;

            StorageContainer ancestor = ancestors[i];

            // -------------------------------------------------
            // Troviamo il ramo che stiamo seguendo.
            //
            // Esempio:
            //
            // A
            // ├── A1
            // │   └── A11
            // └── A2
            //
            // Per A11, a livello A il branch è A1.
            // -------------------------------------------------

            StorageContainer branch = container;

            while (branch != null &&
                   branch.parentShelfId != ancestor.id)
            {
                branch = allShelves.FirstOrDefault(x => x.id == branch.parentShelfId);
            }

            if (branch == null)
                continue;

            // -------------------------------------------------
            // Figli diretti dell'antenato
            // -------------------------------------------------

            List<StorageContainer> ancestorChildren = allShelves
                .Where(x => x.parentShelfId == ancestor.id)
                .ToList();

            int branchIndex = ancestorChildren.FindIndex(
                x => x.id == branch.id
            );

            // -------------------------------------------------
            // Se dopo il branch corrente c'è un fratello,
            // dobbiamo continuare la linea verticale.
            // -------------------------------------------------

            bool branchHasNextSibling =
                branchIndex >= 0 &&
                branchIndex < ancestorChildren.Count - 1;

            verticalLine?.gameObject.SetActive(branchHasNextSibling);
        }

        // =====================================================
        // LABEL
        // =====================================================

        node.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(x => x.name == "Label")?.SetText($"{container.name} ({container.id})");

        // =====================================================
        // ICON
        // =====================================================

        node.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "IconShelving")?.gameObject.SetActive(!container.isRoom);

        node.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.name == "IconRoom")?.gameObject.SetActive(container.isRoom);

        // =====================================================
        // BUTTON
        // =====================================================

        node.GetComponentsInChildren<Button>(true)
            .FirstOrDefault()?.onClick.AddListener(() =>
            {
                OnContainerNodeClicked(container);
            });

        // =====================================================
        // FIGLI
        // =====================================================

        List<StorageContainer> children = allShelves
            .Where(x => x.parentShelfId == container.id)
            .ToList();

        for (int i = 0; i < children.Count; i++)
        {
            CreateNode(children[i], allShelves, depth + 1, i == children.Count - 1
            );
        }
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

    public async Task<List<Artifact>> GetArtifactsInContainer(StorageContainer container)
    {
        return await apiService.GetArtifactsByContainerIdAsync(container.id);
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

    public void OnCreateContainer()
    {
        CreateContainer(-1);
    }

    public void OnCreateContainerFromHierarchy()
    {
        int parentId = selectedContainer != null ? selectedContainer.id : -1;

        CreateContainer(parentId);
    }

    public void CreateContainer(int _parentID)
    {
        errorText.gameObject.SetActive(false);
        string name = containerNameTMP.text;
        //ShelfPage.transform
        //.Find("Name_txt")
        //.GetComponent<TMP_InputField>()
        //.text

        float width = 0f;
        float height = 0f;
        float depth = 0f;

        if (isRoomToggle.isOn)
        {
            if (!float.TryParse(w_input.text, NumberStyles.Float, CultureInfo.InvariantCulture, out width) ||
                !float.TryParse(h_input.text, NumberStyles.Float, CultureInfo.InvariantCulture, out height) ||
                !float.TryParse(d_input.text, NumberStyles.Float, CultureInfo.InvariantCulture, out depth))
            {
                Debug.LogError("Dimensioni non valide.");
                errorText.text = "Invalid dimensions. Please enter valid numbers for width, height, and depth.";
                errorText.gameObject.SetActive(true);
                return;
            }
        }

        StorageContainer dto = new StorageContainer
        {
            name = name,
            worldTransform = "",
            parentShelfId = _parentID,
            isRoom = isRoomToggle,
            markerId = -1,
            roomWidth = width,
            roomHeight = height,
            roomDepth = depth,
            roomCenterPose = "",
        };

        _ = apiService.CreateShelf(dto);

        ResetShelfUI();

        createContainerButton.GetComponent<Button>().onClick.RemoveAllListeners();
        createContainerButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            OnCreateContainer();
        });
    }

    private void ResetShelfUI()
    {
        containerNameTMP.text = "";
        containerPage.SetActive(false);
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

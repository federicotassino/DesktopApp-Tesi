using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Experimental.GraphView;
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

    private List<Artifact> artifacts = new();
    private Artifact selectedArtifact;
    private string confirmText = "Eliminare l'elemento ";

    // Start is called before the first frame update
    async void Start()
    {
        artifactPage.SetActive(false);
        confirmDialog.SetActive(false);
        emptyFieldsText.enabled = false;

        apiService = new APIService();

        artifacts = await apiService.GetAllArtifactsAsync();
        if (artifacts == null)
        {
            Debug.LogError("Artifacts null!");
            return;
        }
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
            ToConsole(artifact.ToString());

            //ScrollView
            listGetData.itemData.Add(artifact);
        }

        listGetData.UpdateListView();
    }

    private void ToConsole(string msg)
    {
        Debug.Log(msg);
    }

    public void OnArtifactTableButtonClick()
    {
        Debug.Log("-----OnArtifactTableButtonClick-----");

        //apiService.CreateArtifactTable();
    }

    public async void OnAddArtifactButtonClickAsync()
    {
        Debug.Log("-----OnAddArtifactButtonClick-----");

        if (newArtifact_name.text != "" && newArtifact_description.text != "" && newArtifact_shUnit.text != "")
        {
            Artifact newArtifact = new Artifact
            {
                name = newArtifact_name.text,
                textDescription = newArtifact_description.text,
                shelvingUnit = Int32.Parse(newArtifact_shUnit.text),
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
    }

    public void ResetArtifactPage()
    {
        newArtifact_name.text = "";
        newArtifact_description.text = "";
        newArtifact_shUnit.text = "";
        emptyFieldsText.enabled = false;
        artifactPage.transform.Find("AddButton").gameObject.SetActive(true);
        artifactPage.transform.Find("UpdateButton").gameObject.SetActive(false);
    }

    public async void OnGetAllArtifactsButtonClickAsync()
    {
        //var artifacts = apiService.GetAllArtifacts();
        List<Artifact> artifacts = await apiService.GetAllArtifactsAsync();
        ToScrollView(artifacts);
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
}

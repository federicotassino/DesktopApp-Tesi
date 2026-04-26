using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    ArtifactService artifactService;

    public ListGetData listGetData;
    public GameObject scrollView;
    public GameObject topBar;
    public GameObject addPage;
    public TMP_InputField newArtifact_name;
    public TMP_InputField newArtifact_description;
    public TMP_InputField newArtifact_shUnit;
    //public Button addButton;
    public TMP_Text emptyFieldsText;

    // Start is called before the first frame update
    void Start()
    {
        addPage.SetActive(false);
        //addButton.enabled = false;
        emptyFieldsText.enabled = false;

        artifactService = new ArtifactService();

        var artifacts = artifactService.GetAllArtifacts();
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

        artifactService.CreateArtifactTable();
    }

    public void OnAddArtifactButtonClick()
    {
        Debug.Log("-----OnAddArtifactButtonClick-----");

        if (newArtifact_name.text != "" && newArtifact_description.text != "" && newArtifact_shUnit.text != "")
        {
            Artifact artifact = new Artifact
            {
                Name = newArtifact_name.text,
                TextDescription = newArtifact_description.text,
                ShelvingUnit = Int32.Parse(newArtifact_shUnit.text),
            };


            int pk = artifactService.AddArtifact(artifact);

            Debug.Log("Primary key: " + pk);

            var artifacts = artifactService.GetAllArtifacts();
            ToScrollView(artifacts);

            //reset di AddPage
            ResetAddPage();
            addPage.SetActive(false);
            scrollView.SetActive(true);
            topBar.SetActive(true);
        }
        else
        {
            emptyFieldsText.enabled = true;
        }
        
    }

    public void ResetAddPage()
    {
        newArtifact_name.text = "";
        newArtifact_description.text = "";
        newArtifact_shUnit.text = "";
        //addButton.enabled = false;
        emptyFieldsText.enabled = false;
    }

    public void OnGetAllArtifactsButtonClick()
    {
        var artifacts = artifactService.GetAllArtifacts();
        ToScrollView(artifacts);
    }

    public void OnGeArtifactByNameButtonClick()
    {
        var artifacts = artifactService.GetArtifactByName("Scarabeo");
        ToConsole(artifacts.ToString());
    }
}

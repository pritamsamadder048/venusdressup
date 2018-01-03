﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SavedLookProperties : MonoBehaviour {


    public string imageName;
    public int imageIndex = -1;

    GameController gameController;
    SaveManager saveManager;
    string dataPath;
    string finalSavePath;

    public bool isInitialized = false;

    //public GameObject look;

    public SaveData sd;

    private void Awake()
    {

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        if (gameController != null)
        {
            
        }
        GetComponent<Button>().onClick.AddListener(CallLoadSavedLook);
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(CallDeleteThisSavedLook);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void InitLookProperties(SaveData s)
    {
        dataPath = Application.persistentDataPath;

        try
        {
            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/wearingsdata";
            }
            finalSavePath = Path.Combine(dataPath, "screenshots");
            finalSavePath = Path.Combine(finalSavePath, imageName);
            sd = s;
            sd.InitializeProperties();
            //sd.dressProperty.InitializeDressProperty(sd.dressData.serializedJsonObject);
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
            isInitialized = true;
        }
        catch
        {
            isInitialized = false;
        }

    }

    public void CallLoadSavedLook()
    {
        if(isInitialized)
        {
            if(gameController!=null)
            {
                gameController.ShowLoadingPanelOnly();
                Invoke("LoadSavedLook", .3f);
            }
        }
    }

    public void LoadSavedLook()
    {
        print("using saved look");
        if(isInitialized)
        {
            print("initialized saved look property");
            if(gameController!=null)
            {


                gameController.mainModelIndex = sd.modelIndex;
                gameController.mainCarouselRotation = sd.modelRotation;
                
                gameController.selectShapeController.SelectThisParticularModel(gameController.mainCarouselRotation,gameController.mainModelIndex );
                gameController.selectShapeController.ChangeBodyTone(sd.bodytoneName);
                gameController.selectShapeController.ChangeEyeColor(sd.eyeName);
                gameController.AcceptBodyShapeChange();
                gameController.AcceptBodyToneChange();
                gameController.AcceptEyeColorChange();

                

                print("game controller is ready");
                if(sd.dressProperty==null)
                {
                    try
                    {
                        if (sd.dressProperty.imgName == "" || sd.dressProperty.imgName == null)
                        {
                            print("dressproperty is null reinitializing dress property");
                            sd.dressProperty = new DressProperties();
                            sd.dressProperty.InitializeDressProperty(sd.dressData.serializedJsonObject);
                            sd.dressProperty.SetDressColor(sd.dressData.pColor);
                            //print(string.Format("dressproperty final save url : {0} and dress property is null : {1}", sd.dressProperty.finalImageUrl, (sd.dressProperty == null)));
                        }
                    }
                    catch
                    {
                        print("dressproperty is null reinitializing dress property");
                        sd.dressProperty = new DressProperties();
                        sd.dressProperty.InitializeDressProperty(sd.dressData.serializedJsonObject);
                        sd.dressProperty.SetDressColor(sd.dressData.pColor);
                        //print(string.Format("dressproperty final save url : {0} and dress property is null : {1}", sd.dressProperty.finalImageUrl, (sd.dressProperty == null)));
                    }
                }
                if(sd.femaleWigProperty==null)
                {
                    try
                    {
                        if (sd.femaleWigProperty.imgName == "" || sd.femaleWigProperty.imgName == null)
                        {
                            sd.femaleWigProperty = new FemaleWigProperties();
                            sd.femaleWigProperty.InitializeWigProperty(sd.femaleWigData.serializedJsonObject);
                            sd.femaleWigProperty.SetFemaleWigColor(sd.femaleWigData.pColor);
                        }
                    }
                    catch
                    {
                        sd.femaleWigProperty = new FemaleWigProperties();
                        sd.femaleWigProperty.InitializeWigProperty(sd.femaleWigData.serializedJsonObject);
                        sd.femaleWigProperty.SetFemaleWigColor(sd.femaleWigData.pColor);
                    }
                }

                sd.ornamentProperty = new OrnamentProperties();
                sd.ornamentProperty.InitializeOrnamentProperty(sd.ornamentData.serializedJsonObject);

                sd.shoeProperty = new ShoeProperties();
                sd.shoeProperty.InitializeShoeProperty(sd.shoeData.serializedJsonObject);
                    
                if(sd.dressProperty.imgName!=null && sd.dressProperty.imgName!="")
                {
                    print("found dressproperty");
                    gameController.selectDressController.PutOnLongDressDynamically(sd.dressProperty, true);
                    Color c = new Color(sd.dressData.pColor[0], sd.dressData.pColor[1], sd.dressData.pColor[2], sd.dressData.pColor[3]);
                    //if(c!=Color.white)
                    //{
                    //    gameController.ChangeToGrayScale(gameController.dress);
                    //    gameController.dress.color = c;

                    //}

                    
                        
                    //    gameController.dress.color = c;
                    //gameController.currentDressColor = c;
                    //gameController.selectDressController.dressColor = c.r;
                    //gameController.selectDressController.dressBrightness = c.b;
                    
                    print("Dress weared");
                }
                else
                {
                    gameController.ToggleDress();
                }

                if(sd.femaleWigProperty.imgName!=null && sd.femaleWigProperty.imgName!="")
                {
                    gameController.selectDressController.PutOnWigDynamically(sd.femaleWigProperty, true);
                    Color c = new Color(sd.femaleWigData.pColor[0], sd.femaleWigData.pColor[1], sd.femaleWigData.pColor[2], sd.femaleWigData.pColor[3]);
                    //if(c!=Color.white)
                    //{
                    //    gameController.ChangeToGrayScale(gameController.wig);
                    //    gameController.wig.color = c;
                    //}

                    
                        
                    //gameController.wig.color = c;

                    //gameController.currentWigColor = c;
                    //gameController.selectDressController.wigColor = c.r;
                    //gameController.selectDressController.wigBrightness = c.b;

                    

                    print("wig weared");
                }
                else
                {
                    gameController.ToggleWig();
                }


                if(sd.ornamentProperty.imgName!=null && sd.ornamentProperty.imgName!="")
                {
                    gameController.selectDressController.PutOnOrnamentDynamically(sd.ornamentProperty);
                }
                else
                {
                    gameController.ToggleOrnament();
                }


                if (sd.shoeProperty.imgName != null && sd.shoeProperty.imgName != "")
                {
                    gameController.selectDressController.PutOnShoeDynamically(sd.shoeProperty);
                }
                else
                {
                    gameController.ToggleShoe();
                }

            }
            gameController.GoToHome();
        }

        gameController.HideLoadingPanelOnly();
        
    }



    public void CallDeleteThisSavedLook()
    {
        gameController.ShowLoadingPanelOnly();
        Invoke("DeleteThisSavedLook", .3f);
    }


    public void DeleteThisSavedLook()
    {
        print(string.Format("Deleting {0} file", sd.saveName));
        saveManager.DeleteSavedLooksData(sd);

    }
}

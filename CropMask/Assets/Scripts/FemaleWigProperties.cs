﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class FemaleWigProperties : MonoBehaviour
{
    public MiniJsonObject mo;

    public string propertyType = "wig";

    public int wearingCode;
    public int mfType = 1;
    public string imgName;
    public string finalImageUrl;
    public string finalSavePath;
    public GameController gameController;

    private bool _isInitialized = false;

    ResourceFileManager rfm;
    // Use this for initialization


    public bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }
    }


    void Awake()
    {
        propertyType = "wig";

        GetComponent<Button>().onClick.AddListener(UseThisWig);
        rfm = GameObject.FindGameObjectWithTag("ResourceFileManager").GetComponent<ResourceFileManager>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void InitializeWigProperty(MiniJsonObject m)
    {
        if (gameController == null)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }

        if (rfm == null)
        {
            rfm = GameObject.FindGameObjectWithTag("ResourceFileManager").GetComponent<ResourceFileManager>();
        }

        mo = m;


        wearingCode = mo.GetField("type_id", -1);

        imgName = mo.GetField("icon", "");



        switch (wearingCode)
        {
            case 1: //download female dress
                {
                    if (imgName != "")
                    {
                        finalImageUrl = string.Format(rfm.imageUrlFormat, imgName);
                        finalSavePath = Path.Combine(rfm.dataPathDict["dressFemaleDataPath"], imgName);
                        

                        //StartCoroutine(DownloadImage(finalImageUrl, finalSavePath));

                        //UpdateDownloadInfo();
                    }
                    break;
                }
            case 2:  //download female wig
                {
                    if (imgName != "")
                    {
                        finalImageUrl = string.Format(rfm.imageUrlFormat, imgName);
                        finalSavePath = Path.Combine(rfm.dataPathDict["wigFemaleDataPath"], imgName);
                        

                        //StartCoroutine(DownloadImage(finalImageUrl, finalSavePath));

                        //UpdateDownloadInfo();
                    }
                    break;
                }
            case 3:  //download female ornament
                {
                    if (imgName != "")
                    {
                        finalImageUrl = string.Format(rfm.imageUrlFormat, imgName);
                        finalSavePath = Path.Combine(rfm.dataPathDict["ornamentFemaleDataPath"], imgName);
                        

                        //StartCoroutine(DownloadImage(finalImageUrl, finalSavePath));

                        //UpdateDownloadInfo();
                    }
                    break;
                }
            case 4:  //download female shoe
                {
                    if (imgName != "")
                    {
                        finalImageUrl = string.Format(rfm.imageUrlFormat, imgName);
                        finalSavePath = Path.Combine(rfm.dataPathDict["shoeFemaleDataPath"], imgName);
                        

                        //StartCoroutine(DownloadImage(finalImageUrl, finalSavePath));

                        //UpdateDownloadInfo();
                    }
                    break;
                }
            case 5: //download male wig
                {
                    if (imgName != "")
                    {
                        finalImageUrl = string.Format(rfm.imageUrlFormat, imgName);
                        finalSavePath = Path.Combine(rfm.dataPathDict["wigMaleDataPAth"], imgName);
                        

                        //StartCoroutine(DownloadImage(finalImageUrl, finalSavePath));

                        //UpdateDownloadInfo();
                    }
                    break;
                }
            case 6: // download male tie
                {
                    if (imgName != "")
                    {
                        finalImageUrl = string.Format(rfm.imageUrlFormat, imgName);
                        finalSavePath = Path.Combine(rfm.dataPathDict["tieMaleDataPath"], imgName);
                        

                        //StartCoroutine(DownloadImage(finalImageUrl, finalSavePath));

                        //UpdateDownloadInfo();
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }

        _isInitialized = true;
        StartCoroutine(SetImage());
    }

    public void UseThisWig()
    {
        if (_isInitialized)
        {
            gameController.selectDressController.PutOnWigDynamically(this);
        }
        else
        {
            return;
        }
    }

    public IEnumerator SetImage()
    {
        print("trying to load wig image from : " + finalSavePath);
        if (IsInitialized)
        {
            if (File.Exists(finalSavePath))
            {
                Texture2D t2d = new Texture2D(10,10);
                t2d.LoadImage(File.ReadAllBytes(finalSavePath));
                t2d.Apply();

                GetComponent<Image>().sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100f);

                gameController.selectDressController.wigLoadingPanel.SetActive(false);
                //DestroyImmediate(t2d, true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        yield return null;
    }
}
﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;



#region SAVEDATA
[Serializable]
public class SaveData
{
    public string saveName;
    public string modelNameame;
    public string bodytoneName;
    public string eyeName;
    public string dressName;
    public string wigName;
    public string ornamentName;
    public string shoeName;
    


    public void Initialize(string model,string bodytone,string eye, string dress="", string wig="", string ornament="", string shoe="")
    {
        modelNameame = model;
        bodytoneName = bodytone;
        eyeName = eye;
        dressName = dress;
        wigName = wig;
        ornamentName = ornament;
        shoeName = shoe;

        
    }

    public static int SaveWearings(string saveDataFileName, SaveData sd,GameController gc)
    {


        try
        {

            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/wearingsdata";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalSavePath = Path.Combine(dataPath, saveDataFileName);
            MonoBehaviour.print(string.Format("Final Save path : {0}", finalSavePath));
            if (File.Exists(finalSavePath))
            {
                MonoBehaviour.print("file exist,saving data");
                List<SaveData> saveDatas = new List<SaveData>();
                saveDatas = LoadData(saveDataFileName);

                int totalSaves = saveDatas.Count;
                MonoBehaviour.print(string.Format("total faces :{0}", totalSaves));



                if (totalSaves < 10)
                {
                    FileStream fileStream;
                    fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                    BinaryFormatter b = new BinaryFormatter();
                    string save_name = string.Format("savedata_{0}.png", (int)(UnityEngine.Random.Range(0, 9999)));
                    sd.saveName = save_name;
                    saveDatas.Add(sd);
                    MonoBehaviour.print(string.Format("total saves : {0}", saveDatas.Count));

                    b.Serialize(fileStream, saveDatas);
                    fileStream.Flush();
                    fileStream.Close();
                    string saveImageName = Path.Combine(dataPath, "screenshots");
                    if (!Directory.Exists(saveImageName))
                    {
                        Directory.CreateDirectory(saveImageName);
                    }
                    saveImageName = Path.Combine(saveImageName, save_name);








#if UNITY_EDITOR
                    MonoBehaviour.print(string.Format("Editor saving screenshot to : {0}", saveImageName));
                    gc.CallBackFromSaveWearings(saveImageName);
#elif UNITY_ANDROID

                        string tsn = string.Format("wearingsdata/screenshots/{0}", save_name);
                        MonoBehaviour.print(string.Format("Android saving screenshot to : {0}", tsn));
                        gc.CallBackFromSaveWearings(tsn);
#elif UNITY_IPHONE

                    string tsn = string.Format("wearingsdata/screenshots/{0}", save_name);
                    Debug.Log(string.Format("Iphone saving screenshot to : {0}", tsn));
                    gc.CallBackFromSaveWearings(tsn);

#endif
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                MonoBehaviour.print("Does Not Exist , Creating new File");
                List<SaveData> saveDatas = new List<SaveData>();

                MonoBehaviour.print(string.Format(" total saves :{0}", saveDatas.Count));
                string save_name = string.Format("savedata_{0}.png", (int)(UnityEngine.Random.Range(0, 9999)));
                sd.saveName = save_name;
                saveDatas.Add(sd);

                FileStream fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(fileStream, saveDatas);

                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
                string saveImageName = Path.Combine(dataPath, "screenshots");
                if (!Directory.Exists(saveImageName))
                {
                    Directory.CreateDirectory(saveImageName);
                }
                saveImageName = Path.Combine(saveImageName, save_name);


#if UNITY_EDITOR
                MonoBehaviour.print(string.Format("Editor saving screenshot to : {0}", saveImageName));
                gc.CallBackFromSaveWearings(saveImageName);


#elif UNITY_ANDROID

                    string tsn = string.Format("wearingsdata/screenshots/{0}", save_name);
                    MonoBehaviour.print(string.Format("Android saving screenshot to : {0}", tsn));
                    gc.CallBackFromSaveWearings(tsn);
#elif UNITY_IPHONE

                    string tsn = string.Format("wearingsdata/screenshots/{0}", save_name);
                    MonoBehaviour.print(string.Format("Android saving screenshot to : {0}", tsn));
                    gc.CallBackFromSaveWearings(tsn);

#endif



                return 1;


            }

            
        }
        catch (Exception e)
        {
            MonoBehaviour.print("error : " + e);
            return -1;
        }


    }

    public static int SaveLooksFromData(List<SaveData> sd, string saveFileName)
    {
        try
        {

            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/wearingsdata";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalSavePath = Path.Combine(dataPath, saveFileName);
            MonoBehaviour.print(string.Format("Final Save path : {0}", finalSavePath));

            if (File.Exists(finalSavePath))
            {
                MonoBehaviour.print("file exist,saving data");



                int totalScavedlooks = sd.Count;
                MonoBehaviour.print(string.Format("total faces :{0}", totalScavedlooks));



                if (totalScavedlooks <= 10)
                {
                    FileStream fileStream;
                    fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                    BinaryFormatter b = new BinaryFormatter();


                    b.Serialize(fileStream, sd);
                    fileStream.Flush();
                    fileStream.Close();
                    MonoBehaviour.print("face data is saved");
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                MonoBehaviour.print("Does Not Exist , Creating new File for saving data from list");


                MonoBehaviour.print(string.Format(" total faces :{0}", sd.Count));


                FileStream fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(fileStream, sd);

                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();

                return 1;


            }
        }
        catch (Exception e)
        {
            MonoBehaviour.print(string.Format("Error occured when trying to save data from list : {0}", e));
            return -1;
        }
    }


    public static List<SaveData> DeleteSavedLook(SaveData s, List<SaveData> sd, string saveFileName)
    {
        List<SaveData> origList = sd;
        try
        {


            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/wearingsdata";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalImagePath = Path.Combine(dataPath, "screenshots");
            if (!Directory.Exists(finalImagePath))
            {
                Directory.CreateDirectory(finalImagePath);
            }
            finalImagePath = Path.Combine(finalImagePath, s.saveName);

            sd.Remove(s);
            if (File.Exists(finalImagePath))
            {
                MonoBehaviour.print(string.Format("Deleting : {0} as it exist", finalImagePath));
                File.Delete(finalImagePath);
                MonoBehaviour.print("file deleted successfully");
            }
            SaveLooksFromData(sd, saveFileName);

            return sd;
        }
        catch (Exception e)
        {
            MonoBehaviour.print(string.Format("Error occured when trying to save data from list : {0}", e));
            return origList;
        }
    }

    public static List<SaveData> LoadData(string fileName)
    {
        FileStream fileStream;
        try
        {
            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/wearingsdata";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalSavePath = Path.Combine(dataPath, fileName);

            if (File.Exists(finalSavePath))
            {
                MonoBehaviour.print(string.Format("file exist : {0} ", finalSavePath));
                fileStream = new FileStream(finalSavePath, FileMode.Open, FileAccess.Read);
                try
                {
                    List<SaveData> saveDatas = new List<SaveData>();

                    BinaryFormatter b = new BinaryFormatter();
                    saveDatas = (List<SaveData>)b.Deserialize(fileStream);
                    fileStream.Close();
                    fileStream.Dispose();
                    return saveDatas;
                }
                catch (Exception e)
                {
                    MonoBehaviour.print(string.Format("error writting save file : {0} ", e));
                    fileStream.Close();
                    fileStream.Dispose();
                    return null;
                }

            }
            else
            {
                throw new FileNotFoundException(string.Format("file : {0} Does Not Exist", finalSavePath));


            }

        }
        catch (Exception e)
        {
            MonoBehaviour.print(string.Format("error occured loading save file : {0}", e));
            return null;
        }

    }


}
#endregion SAVEDATA

#region CROPPEDFACEDATA
[Serializable]
public class CroppedFaceData
{

    //public Byte[] croppedFaceImage;
    public string imageName;
    public float[] scale;
    public float[] sizeDelta;
    public float[] position;
    public float[] rotation;
    public int saveFaceHash;

    public void Initialize( Vector3 scl,Vector2 size_delta,Vector3 pos,Vector3 rot)
    {
        
        //croppedFaceImage = faceTexture.EncodeToPNG();
        
        sizeDelta =new float[] { size_delta.x,size_delta.y};
        position = new float[] { pos.x, pos.y, pos.z };
        rotation = new float[] { rot.x, rot.y, rot.z };
        scale = new float[] { scl.x, scl.y, scl.z };

        saveFaceHash = GetHashCode()*UnityEngine.Random.Range(2,10);
    }

    public void Initialize(RawImage img)
    {
        
        //croppedFaceImage =((Texture2D) img.texture).EncodeToPNG();
        
        sizeDelta = new float[] { img.GetComponent<RectTransform>().sizeDelta.x, img.GetComponent<RectTransform>().sizeDelta.y };
        scale = new float[] { img.transform.localScale.x, img.transform.localScale.y, img.transform.localScale.z };
        position = new float[] { img.GetComponent<RectTransform>().anchoredPosition3D.x, img.GetComponent<RectTransform>().anchoredPosition3D.y, img.GetComponent<RectTransform>().anchoredPosition3D.z };
        rotation = new float[] { img.transform.localEulerAngles.x, img.transform.localEulerAngles.y, img.transform.localEulerAngles.z };
        saveFaceHash = GetHashCode() * UnityEngine.Random.Range(2, 10);


    }

    public static int SaveCroppedFaceDict(string saveDataFileName,CroppedFaceData cfd,RawImage rwimg)
    {
        
        
        try
        {
            MonoBehaviour.print(Application.persistentDataPath);
            MonoBehaviour.print(Application.persistentDataPath);
            string dataPath = Application.persistentDataPath;
            
            if(Application.platform!=RuntimePlatform.OSXPlayer)
            {
                dataPath += "/croppedfaces";
            }
            string finalSavePath = Path.Combine(dataPath, saveDataFileName);
            MonoBehaviour.print(string.Format("Final Save path : {0}", finalSavePath));
            if(File.Exists(finalSavePath))
            {
                MonoBehaviour.print("file exist,saving data");
                Dictionary<string, CroppedFaceData> croppedFaceFiles = new Dictionary<string, CroppedFaceData>();
                croppedFaceFiles = LoadDataDict(saveDataFileName); ;

                int totalScavedFaces = croppedFaceFiles.Count;
                MonoBehaviour.print(string.Format("total faces :{0}",totalScavedFaces ));
                
                foreach(string s in croppedFaceFiles.Keys)
                {
                    MonoBehaviour.print(string.Format("loaded cropped face file : {0}", s));
                }

                if(totalScavedFaces<5)
                {
                    FileStream fileStream;
                    fileStream = new FileStream(finalSavePath, FileMode.Open, FileAccess.Write);
                    BinaryFormatter b = new BinaryFormatter();
                    string save_name = string.Format("croppedface_{0}", croppedFaceFiles.Count);
                    croppedFaceFiles.Add(save_name, cfd);
                    MonoBehaviour.print(string.Format("total faces : {0}", croppedFaceFiles.Count));
                    foreach (string s in croppedFaceFiles.Keys)
                    {
                        MonoBehaviour.print(string.Format("writing cropped face file : {0}", s));
                    }
                    b.Serialize(fileStream, croppedFaceFiles);
                    fileStream.Flush();
                    fileStream.Close();
                    string saveImageName = Path.Combine(dataPath, "faceimages");
                    saveImageName = Path.Combine(saveImageName, save_name + "png");
                    File.WriteAllBytes(saveImageName,(rwimg.texture as Texture2D).EncodeToPNG());
                    MonoBehaviour.print("face is saved");
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                MonoBehaviour.print("Does Not Exist , Creating new File");
                Dictionary<string, CroppedFaceData> croppedFaceFiles = new Dictionary<string, CroppedFaceData>();

                MonoBehaviour.print(string.Format(" total faces :{0}",croppedFaceFiles.Count));
                string save_name = string.Format("croppedface_{0}", croppedFaceFiles.Count);
                croppedFaceFiles.Add(save_name, cfd);
                foreach (string s in croppedFaceFiles.Keys)
                {
                    MonoBehaviour.print(string.Format("writing first cropped face file : {0}", s));
                }
                FileStream fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(fileStream, croppedFaceFiles);
                
                fileStream.Flush();
                fileStream.Close();
                string saveImageName = Path.Combine(dataPath, "faceimages");
                saveImageName = Path.Combine(saveImageName, save_name + "png");
                File.WriteAllBytes(saveImageName, (rwimg.texture as Texture2D).EncodeToPNG());
                MonoBehaviour.print("face is saved");
                return 1;


            }
            
        }
        catch(Exception e)
        {
            MonoBehaviour.print("error : " + e);
            return -1;
        }
    }

    public static Dictionary<string, CroppedFaceData> LoadDataDict(string fileName)
    {
        try
        {
            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/croppedfaces";
            }
                string finalSavePath = Path.Combine(dataPath, fileName);

                if(File.Exists(finalSavePath))
                {
                    Dictionary<string, CroppedFaceData> croppedFaceFiles = new Dictionary<string, CroppedFaceData>();
                    FileStream fileStream = new FileStream(finalSavePath, FileMode.Open, FileAccess.Read);
                    BinaryFormatter b = new BinaryFormatter();
                    croppedFaceFiles = (Dictionary<string, CroppedFaceData>)b.Deserialize(fileStream);
                    return croppedFaceFiles;
                }
                else
                {
                    throw new FileNotFoundException(string.Format("file : {0} Does Not Exist",finalSavePath));
                    
                }
            
        }
        catch (Exception e)
        {
            MonoBehaviour.print(string.Format("error occured loading save file : {0}", e));
            return null;
        }
    }



    public static int SaveCroppedFace(string saveDataFileName, CroppedFaceData cfd, RawImage rwimg)
    {


        try
        {
            MonoBehaviour.print(Application.persistentDataPath);
            MonoBehaviour.print(Application.persistentDataPath);
            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/croppedfaces";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalSavePath = Path.Combine(dataPath, saveDataFileName);
            MonoBehaviour.print(string.Format("Final Save path : {0}", finalSavePath));
            if (File.Exists(finalSavePath))
            {
                MonoBehaviour.print("file exist,saving data");
                List<CroppedFaceData> croppedFaceFiles = new List<CroppedFaceData>();
                croppedFaceFiles = LoadData(saveDataFileName); 

                int totalScavedFaces = croppedFaceFiles.Count;
                MonoBehaviour.print(string.Format("total faces :{0}", totalScavedFaces));

                

                if (totalScavedFaces < 5)
                {
                    FileStream fileStream;
                    fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                    BinaryFormatter b = new BinaryFormatter();
                    string save_name = string.Format("croppedface_{0}.png", (int)(UnityEngine.Random.Range(0,9999)));
                    cfd.imageName = save_name;
                    croppedFaceFiles.Add( cfd);
                    MonoBehaviour.print(string.Format("total faces : {0}", croppedFaceFiles.Count));
                    
                    b.Serialize(fileStream, croppedFaceFiles);
                    fileStream.Flush();
                    fileStream.Close();
                    string saveImageName = Path.Combine(dataPath, "faceimages");
                    if (!Directory.Exists(saveImageName))
                    {
                        Directory.CreateDirectory(saveImageName);
                    }
                    saveImageName = Path.Combine(saveImageName, save_name);
                    File.WriteAllBytes(saveImageName, (rwimg.texture as Texture2D).EncodeToPNG());
                    MonoBehaviour.print("face is saved");
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                MonoBehaviour.print("Does Not Exist , Creating new File");
                List<CroppedFaceData> croppedFaceFiles = new List<CroppedFaceData>();

                MonoBehaviour.print(string.Format(" total faces :{0}", croppedFaceFiles.Count));
                string save_name = string.Format("croppedface_{0}.png", (int)(UnityEngine.Random.Range(0, 9999)));
                cfd.imageName = save_name;
                croppedFaceFiles.Add(cfd);
                
                FileStream fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(fileStream, croppedFaceFiles);

                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
                string saveImageName = Path.Combine(dataPath, "faceimages");
                if (!Directory.Exists(saveImageName))
                {
                    Directory.CreateDirectory(saveImageName);
                }
                saveImageName = Path.Combine(saveImageName, save_name);
                File.WriteAllBytes(saveImageName, (rwimg.texture as Texture2D).EncodeToPNG());
                MonoBehaviour.print("face is saved");

                return 1;


            }

        }
        catch (Exception e)
        {
            MonoBehaviour.print("error : " + e);
            return -1;
        }


    }

    public static List<CroppedFaceData> DeleteCroppedFace(CroppedFaceData c, List<CroppedFaceData> cd, string saveFileName)
    {
        List<CroppedFaceData> origList = cd;
        try
        {
            

            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/croppedfaces";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalImagePath = Path.Combine(dataPath, "faceimages");
            if (!Directory.Exists(finalImagePath))
            {
                Directory.CreateDirectory(finalImagePath);
            }
            finalImagePath = Path.Combine(finalImagePath, c.imageName);
            
            cd.Remove(c);
            if (File.Exists(finalImagePath))
            {
                MonoBehaviour.print(string.Format("Deleting : {0} as it exist", finalImagePath));
                File.Delete(finalImagePath);
                MonoBehaviour.print("file deleted successfully");
            }
            SaveCroppedFaceFromData(cd, saveFileName);

            return cd;
        }
        catch (Exception e)
        {
            MonoBehaviour.print(string.Format("Error occured when trying to save data from list : {0}", e));
            return origList ;
        }
    }
    public static int SaveCroppedFaceFromData(List<CroppedFaceData> cd, string saveFileName)
    {
        try
        {

            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/croppedfaces";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalSavePath = Path.Combine(dataPath, saveFileName);
            MonoBehaviour.print(string.Format("Final Save path : {0}", finalSavePath));
            
            if (File.Exists(finalSavePath))
            {
                MonoBehaviour.print("file exist,saving data");



                int totalScavedFaces = cd.Count;
                MonoBehaviour.print(string.Format("total faces :{0}", totalScavedFaces));



                if (totalScavedFaces <= 5)
                {
                    FileStream fileStream;
                    fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                    BinaryFormatter b = new BinaryFormatter();


                    b.Serialize(fileStream, cd);
                    fileStream.Flush();
                    fileStream.Close();
                    MonoBehaviour.print("face data is saved");
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                MonoBehaviour.print("Does Not Exist , Creating new File for saving data from list");


                MonoBehaviour.print(string.Format(" total faces :{0}", cd.Count));


                FileStream fileStream = new FileStream(finalSavePath, FileMode.Create, FileAccess.Write);
                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(fileStream, cd);

                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();

                return 1;


            }
        }
        catch(Exception e)
        {
            MonoBehaviour.print(string.Format("Error occured when trying to save data from list : {0}", e));
            return -1;
        }
    }

    public static List<CroppedFaceData> LoadData(string fileName)
    {
        FileStream fileStream;
        try
        {
            string dataPath = Application.persistentDataPath;

            if (Application.platform != RuntimePlatform.OSXPlayer)
            {
                dataPath += "/croppedfaces";
            }
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string finalSavePath = Path.Combine(dataPath, fileName);

            if (File.Exists(finalSavePath))
            {
                fileStream = new FileStream(finalSavePath, FileMode.Open, FileAccess.Read);
                try
                {
                    List<CroppedFaceData> croppedFaceFiles = new List<CroppedFaceData>();
                    
                    BinaryFormatter b = new BinaryFormatter();
                    croppedFaceFiles = (List<CroppedFaceData>)b.Deserialize(fileStream);
                    fileStream.Close();
                    fileStream.Dispose();
                    return croppedFaceFiles;
                }
                catch(Exception e)
                {
                    MonoBehaviour.print(string.Format("error writting file : {0} ", e));
                    fileStream.Close();
                    fileStream.Dispose();
                    return null;
                }
                
            }
            else
            {
                throw new FileNotFoundException(string.Format("file : {0} Does Not Exist", finalSavePath));


            }

        }
        catch (Exception e)
        {
            MonoBehaviour.print(string.Format("error occured loading save file : {0}", e));
            return null;
        }
        
    }

    
}

#endregion CROPPEDFACEDATA





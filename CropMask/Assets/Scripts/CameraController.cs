
#if UNITY_ANDROID 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using CameraShot;
using System.Linq;
using System;
using UnityEngine.Networking;
public class CameraController : MonoBehaviour
{

    private bool isDownloadCompleted = false;
    [SerializeField]
    private bool camSupported = false;
    private WebCamTexture backCam;
    private WebCamTexture frontCam;
    private WebCamTexture cam;
    private Texture defaultBackground;
    private Texture2D photoTaken;
    private bool cameraIsOpen = false;


    [SerializeField]
    private GameObject gameControllerObject;
    [SerializeField]
    private GameObject galleryControllerObject;
    [SerializeField]
    private GameObject touchControllerObject;


    private GameController gameController;
    private Gallery galleryController;
    private TouchController touchController;

    [SerializeField]
    private GameObject imagePanel;
    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private Text debugText;

    public bool isFrontCam = true;
    public RawImage background;
    public Image processingImage;

    public AspectRatioFitter fit;


    [SerializeField]
    private GameObject mainModel;

    private bool isDownloading = false;

    [SerializeField]
    private GameObject sceneEditorControllerObj;
    public GameObject infoPopupPrefab;
    public GameObject canvasObject;

    private void Awake()
    {
        //CameraShotEventListener.onImageLoad += OnImageLoad;
        //CameraShotEventListener.onImageSaved += OnImageSaved;
        //CameraShotEventListener.onCancel += CancelCamera;
        //CameraShotEventListener.onError += OnError;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        //AndroidCamera.instance.OnImagePicked -= OnImagePicked;
    }
    // Use this for initialization
    void Start()
    {

        //CameraShotEventListener.onImageLoad += OnImageLoad;


        gameController = gameControllerObject.GetComponent<GameController>();
        galleryController = galleryControllerObject.GetComponent<Gallery>();
        touchController = touchControllerObject.GetComponent<TouchController>();
        panels[3].SetActive(false);
        imagePanel.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {


    }
    public void OpenCamera(bool accessFrontCam = true)
    {
        DestroyImmediate(processingImage.sprite);
        gameController.CollectGurbage(true);


#if UNITY_EDITOR

        OnImageSaved(null);

#else
        StartNativeCamera();
#endif
        gameController.ToggleCameraDownMenu(2);
        sceneEditorControllerObj.SetActive(false);


    }

    public void OpenAndroidNativeCamera()
    {
        try
        {
            AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ajo = new AndroidJavaObject("tico.transindia.com.cameralib.ImageProStatic");
            ajo.CallStatic("OpenCamera", ajc.GetStatic<AndroidJavaObject>("currentActivity"), (int)processingImage.rectTransform.rect.width, (int)processingImage.rectTransform.rect.height,50, (float)10f);
            //ajo.CallStatic("OpenCamera", ajc.GetStatic<AndroidJavaObject>("currentActivity"), 480,640);
        }
        catch(Exception e)
        {
            Debug.Log("Error occured opening Camers : " + e.Message);
            gameController.InstantiateInfoPopup("Error Opening Camera");
            gameController.HideLoadingPanelOnly();
        }
    }





    public void StartNativeCamera()
    {
        Debug.Log("start native camera..");

#if UNITY_ANDROID
        gameController.CollectGurbage(true);
        gameController.ShowLoadingPanelOnly();
        OpenAndroidNativeCamera();

        //AndroidCamera.Instance.OnImagePicked += OnImagePicked;
        //AndroidCamera.Instance.GetImageFromCamera();
        

#elif UNITY_IPHONE
            Debug.Log("opening camera on iphone");
            //IOSCameraShot.LaunchCameraForImageCapture(false);
#endif

    }









    public void OnPhotoPick(string photoPath)
    {
        
        if (!photoPath.Contains("file://"))
        {
            photoPath = "file://" + photoPath;
        }
        Debug.Log("Camera-----" + photoPath);
        StartCoroutine(DownloadImageAndUse(photoPath));
    }


    public void OnPhotoCancel(string message)
    {
        Debug.Log("You cancelled the camera");
        gameController.HideLoadingPanelOnly();
    }

    private IEnumerator DownloadImageAndUse(string imageUrl)
    {

        bool success = false;


        using (UnityEngine.Networking.UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            //print(www.url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Error Occured");
                success = false;
            }
            else
            {
                Texture2D t2d = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
                t2d.Apply();
                Debug.Log(string.Format("width and height of camera image is : {0}  {1}", t2d.width, t2d.height));
                processingImage.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100f);
                success = true;
            }
            //www.Dispose();

        }


        gameController.HideLoadingPanelOnly();
        if(success)
        {
            GoToImageCrop();
        }
        else
        {
            Debug.Log("error setting camera image");
        }
    }
















































    //public void OnImagePicked(AndroidImagePickResult result)
    //{

    //    Debug.Log("image pick");
    //    if (result.IsSucceeded)
    //    {
    //        //DestroyImmediate(result.Image);
    //        //gameController.CollectGurbage(true);
    //        gameController.ShowLoadingPanelOnly();
    //        //AN_PoupsProxy.showMessage("Image Pick Rsult", "Succeeded, path: " + result.ImagePath);
    //        //OnImageSaved(result.ImagePath);
    //        UseImage(result.Image);

    //        //DestroyImmediate(result.Image);

    //    }
    //    else
    //    {
    //        AN_PoupsProxy.showMessage("Image Pick Rsult", "Failed");
    //    }

    //    //gameController.CollectGurbage(true);
    //    AndroidCamera.Instance.OnImagePicked -= OnImagePicked;
    //}

    public void OnImageSaved(string path)
    {
        
#if UNITY_EDITOR
        gameController.ShowLoading();

        //			DownloadImage ("http://dentedpixel.com/wp-content/uploads/2014/12/Unity5-0.png");
        //StartCoroutine(DownloadImage("http://htc-wallpaper.com/wp-content/uploads/2015/10/Old-watch.jpg?982602"));
        StartCoroutine(DownloadImage("https://i.pinimg.com/originals/08/5e/a3/085ea3a5b5f35242aea65b4431294e40.jpg"));
        //DownloadImage("https://i.pinimg.com/originals/99/96/b4/9996b4944a37318dfc00c9a34ed78c6b.png");
        //			DownloadImage("file:///C:/Users/Arun/Desktop/Gimp.jpg");
#elif UNITY_ANDROID
        //Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
        //gameController.ShowLoadingPanelOnly();
        //Handheld.StartActivityIndicator();
        if(!path.Contains("file://"))
        {
            path = "file://" + path;
        }
            print("image path is ........ : " + path);
            StartCoroutine(DownloadImage(path));

#elif UNITY_IPHONE

        //Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.iOSActivityIndicatorStyle.WhiteLarge);
        //gameController.ShowLoadingPanelOnly();
        //Handheld.StartActivityIndicator();
            if(!path.Contains("file://"))
        {
            path = "file://" + path;
        }
            Debug.Log("image path is ........ : " + path);
            StartCoroutine(DownloadImage(path));

#endif

    }

    public void OnError(string message)
    {
        InstantiateInfoPopup(message);
        gameController.HideLoading();
    }





    public void UseImage(Texture2D tex)
    {

        bool success = false;


#if !UNITY_EDITOR
        //gameController.ShowLoadingPanelOnly();
#endif
         


               

                Texture2D t2d = new Texture2D(tex.width, tex.height);
        print(string.Format("Tex width : {0} , height : {1}", tex.width, tex.height));
                t2d.SetPixels(tex.GetPixels());
                t2d.Apply();
                DestroyImmediate(tex);


                try
                {


                    processingImage.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100f);
                    

            Texture2D ttx = MergeImage(t2d);

                    ttx.Apply();


            print("Got image");
            processingImage.sprite = Sprite.Create(ttx, new Rect(0, 0, ttx.width, ttx.height), new Vector2(0.5f, 0.5f), 100f);

                    Destroy(touchController.actualImage);
                    

                    DestroyImmediate(t2d, true);
                    success = true;
            print("All Success");
                }
                catch (Exception e)
                {
                    success = false;
                    Debug.Log(string.Format("Error while downloading image : ", e));
                    
                }

            



        

        if (success)
        {
            
            Handheld.StopActivityIndicator();
            gameController.HideLoading();
            //gameController.HideLoadingPanelOnly();
            print("going to imagecrop");

            GoToImageCrop();
        }
        else
        {
           
            Handheld.StopActivityIndicator();
            //gameController.HideLoadingPanelOnly();
            gameController.HideLoading();
            print("Some Error occured");
        }
        gameController.HideLoadingPanelOnly();
        //gameController.HideLoading(); 

        gameController.CollectGurbage(true);

    }



    private IEnumerator DownloadImage(string imageUrl)
    {

        bool success = false;


//#if UNITY_ANDROID
        
        //Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);

#if UNITY_IPHONE
		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.WhiteLarge);
		Handheld.StartActivityIndicator();

#elif UNITY_TIZEN
            Handheld.SetActivityIndicatorStyle(TizenActivityIndicatorStyle.Small);
		Handheld.StartActivityIndicator();
#endif

        //Handheld.StartActivityIndicator();
#if !UNITY_EDITOR
        gameController.ShowLoadingPanelOnly();
#endif
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            //print(www.url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {



                Texture2D t2d = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
                t2d.Apply();

                try
                {
                    
                    //print("good");
                    //print(string.Format("before Process image size : {0} ", processingImage.rectTransform.rect));
                    //processingImage.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100f);
                    //print(string.Format("after Process image size : {0} {1} ", processingImage.rectTransform.rect, processingImage.sprite.rect));

                    Texture2D ttx = MergeImage(t2d);

                    ttx.Apply();


                    print(string.Format("current size : {0} , {1}", ttx.width, ttx.height));
                    Debug.Log("Image is downloaded");
                    processingImage.sprite = Sprite.Create(ttx, new Rect(0, 0, ttx.width, ttx.height), new Vector2(0.5f, 0.5f), 100f);

                    Destroy(touchController.actualImage);
                    touchController.actualImage = new Texture2D(t2d.width, t2d.height);
                    touchController.actualImage.SetPixels(t2d.GetPixels());
                    touchController.actualImage.Apply();

                    DestroyImmediate(t2d, true);
                    success = true;
                }
                catch (Exception e)
                {
                    success = false;
                    Debug.Log(string.Format("Error while downloading image : ", e));
                    //yield return new WaitForEndOfFrame();
                    //print("stopping loading for camera..");
                    //gameController.HideLoading();
                }
                //UseImage(((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D);

            }

           

        }

        if (success)
        {
            yield return new WaitForSeconds(1.5f);
            Handheld.StopActivityIndicator();
            gameController.HideLoading();
            gameController.HideLoadingPanelOnly();
            GoToImageCrop();
        }
        else
        {
            yield return new WaitForFixedUpdate();
            Handheld.StopActivityIndicator();
            gameController.HideLoadingPanelOnly();
            gameController.HideLoading();
        }

        //gameController.HideLoading(); 

        gameController.HideLoadingPanelOnly();

    }
    
    public void CloseCamera()
    {
        if (backCam != null && backCam.isPlaying)
        {
            backCam.Stop();
        }
        if (frontCam != null && frontCam.isPlaying)
        {
            frontCam.Stop();
        }
        if (cam != null && cam.isPlaying)
        {
            cam.Stop();
            cam = null;
        }
        else
        {
            cam = null;
        }

        cameraIsOpen = false;
    }


    Texture2D RotateImageTest(Texture2D originTexture, float angle)
    {
        Texture2D result;
        result = new Texture2D(originTexture.width, originTexture.height);
        Color32[] pix1 = result.GetPixels32();
        Color32[] pix2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int x = 0;
        int y = 0;
        Color32[] pix3 = rotateSquare(pix2, (Mathf.PI / 180 * angle), originTexture);
        for (int j = 0; j < H; j++)
        {
            for (var i = 0; i < W; i++)
            {
                //pix1[result.width/2 - originTexture.width/2 + x + i + result.width*(result.height/2-originTexture.height/2+j+y)] = pix2[i + j*originTexture.width];
                pix1[result.width / 2 - W / 2 + x + i + result.width * (result.height / 2 - H / 2 + j + y)] = pix3[i + j * W];
            }
        }
        result.SetPixels32(pix1);
        result.Apply();
        return result;
    }


    Color32[] rotateSquare(Color32[] arr, float phi, Texture2D originTexture)
    {
        int x;
        int y;
        int i;
        int j;
        float sn = Mathf.Sin(phi);
        float cs = Mathf.Cos(phi);
        Color32[] arr2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int xc = W / 2;
        int yc = H / 2;
        for (j = 0; j < H; j++)
        {
            for (i = 0; i < W; i++)
            {
                arr2[j * W + i] = new Color32(0, 0, 0, 0);
                x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                if ((x > -1) && (x < W) && (y > -1) && (y < H))
                {
                    arr2[j * W + i] = arr[y * W + x];
                }
            }
        }

        return arr2;
    }


    public void TakePicture()
    {
        if (cam != null)
        {
            Debug.Log("camara available");
            if (cam.isPlaying)
            {
                Debug.Log("camera playing");
                photoTaken = new Texture2D(background.texture.width, background.texture.height);
                photoTaken.SetPixels(cam.GetPixels());
                photoTaken.Apply();
                photoTaken = RotateImageTest(photoTaken, 90);
                photoTaken.Apply();
                CloseCamera();
                processingImage.sprite = Sprite.Create(photoTaken, new Rect(0, 0, photoTaken.width, photoTaken.height), new Vector2(0.5f, 0.5f), 100f);

                Texture2D ttx = MergeImage(photoTaken);
                processingImage.sprite = Sprite.Create(ttx, new Rect(0, 0, ttx.width, ttx.height), new Vector2(0.5f, 0.5f), 100f);
                photoTaken = null;

                GoToImageCrop();

            }
        }
    }
    private void GoToImageCrop()
    {
        //Go To Image Panel;
        panels[0].SetActive(true);
        panels[1].SetActive(true);
        for (int i = 2; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        touchController.ToggleMask(1);
        gameController.HideAcceptCropButton();
        imagePanel.SetActive(true);

        sceneEditorControllerObj.SetActive(false);
        //gameController.HideLoading();
        gameController.HideLoadingPanelOnly();

    }
    public void CancelCamera()
    {
        //CloseCamera ();
        mainModel.SetActive(true);
        gameController.ToggleHomeSideMenu(1);
        panels[3].SetActive(false);
        panels[2].SetActive(true);
        panels[1].SetActive(true);
        panels[0].SetActive(true);
        sceneEditorControllerObj.SetActive(true);
        //		Debug.Log ("closing camera");
    }
    public void UpdateImage()
    {
        if (!cameraIsOpen)
        {
            return;
        }

        //		float ratio = (float)cam.width / (float)cam.height;
        //		fit.aspectRatio = ratio;
        if (!isFrontCam)
        {
            float scaleY = cam.videoVerticallyMirrored ? -1f : 1f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -cam.videoRotationAngle;

            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }
        else
        {
            float scaleY = cam.videoVerticallyMirrored ? 1f : -1f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
            debugText.text = cam.videoVerticallyMirrored.ToString();
            int orient = -cam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }
    }








    public Texture2D ResizeTexture2D(Texture2D texture, int newWidth, int newHeight)
    {
        float warpFactor = 1.0F;
        //		Texture2D destTex = new Texture2D(Screen.width, Screen.height);
        Texture2D destTex = new Texture2D(newWidth, newHeight);
        print("resize texture size : " + destTex.width + "   " + destTex.height);
        //		print ("Screen size : " + Screen.width + "   " + Screen.height);
        Color[] destPix = new Color[destTex.width * destTex.height];
        try
        {
            int y = 0;
            while (y < destTex.height)
            {
                int x = 0;
                while (x < destTex.width)
                {
                    float xFrac = x * 1.0F / (destTex.width - 1);
                    float yFrac = y * 1.0F / (destTex.height - 1);
                    float warpXFrac = Mathf.Pow(xFrac, warpFactor);
                    float warpYFrac = Mathf.Pow(yFrac, warpFactor);
                    destPix[y * destTex.width + x] = texture.GetPixelBilinear(warpXFrac, warpYFrac);
                    x++;
                }
                y++;
            }
            destTex.SetPixels(destPix);
            destTex.Apply();

        }
        catch
        {
            destTex = null;
        }

        //destImage.sprite = Sprite.Create(destTex, new Rect(0, 0, destTex.width, destTex.height), new Vector2(.5f, .5f), 100);

        return destTex;
    }

    private Texture2D MergeImage(Texture2D Overlay, Texture2D Background = null, Texture2D newTexture = null)
    {
        //print(string.Format("overlay width : {0}  overlay height{1}  pimg rect trnsfrm width : {2} pimg rect trnsfrm Height : {3} ", Overlay.width, Overlay.height, processingImage.rectTransform.rect.width, processingImage.rectTransform.rect.height));
        if ((Overlay.width > processingImage.rectTransform.rect.width) || (Overlay.height > processingImage.rectTransform.rect.height))
        {
            //			Overlay.Resize (1300,1300, Overlay.format, false);
            //			Overlay.Apply ();
            int resizeHeight = Overlay.height;
            int resizeWidth = Overlay.width;

            if (Overlay.width > processingImage.rectTransform.rect.width)
            {
                resizeWidth = (int)processingImage.rectTransform.rect.width;
                float r = (float)Overlay.width / (float)resizeWidth;
                resizeHeight = (int)(Overlay.height / r);
                print(string.Format("Width is big Resize width : {0} resize hight {1} when r is : {2}", resizeWidth, resizeHeight, r));
            }
            /*
			if (Overlay.height > processingImage.rectTransform.rect.height) {
				resizeHeight =(int) processingImage.rectTransform.rect.height;
                float r = (float)Overlay.height / (float)resizeHeight;
                resizeWidth = (int)(Overlay.width / r);
                print(string.Format("Height is big Resize width : {0} resize hight {1} when r is : {2}", resizeWidth, resizeHeight, r));
            }
            */



            Texture2D resizedOverlay = ResizeTexture2D(Overlay, resizeWidth, resizeHeight);
            Overlay.Resize(resizeWidth, resizeHeight);
            Overlay.Apply();
            //			Overlay=ResizeTexture2D (Overlay, resizeWidth, resizeHeight);
            Overlay.SetPixels(resizedOverlay.GetPixels());
            Overlay.Apply();
            Destroy(resizedOverlay);
        }

        if (Background == null)
        {
            Background = new Texture2D((int)processingImage.rectTransform.rect.width, (int)processingImage.rectTransform.rect.height, TextureFormat.ARGB32, false);
            Color fillColor = Color.clear;
            //Color[] fillPixels = new Color[Background.width * Background.height];

            //for (int i = 0; i < fillPixels.Length; i++)
            //{
            //	fillPixels[i] = fillColor;
            //}

            Color[] fillPixels = Enumerable.Repeat(Color.clear, Background.width * Background.height).ToArray();

            Background.SetPixels(fillPixels);
            Background.Apply();

        }
        if (newTexture == null)
        {
            newTexture = new Texture2D((int)processingImage.rectTransform.rect.width, (int)processingImage.rectTransform.rect.height, TextureFormat.ARGB32, false);
            //print ("big image rect : "+processingImage.rectTransform.rect);
        }


        Vector2 offset = new Vector2(((newTexture.width - Overlay.width) / 2), ((newTexture.height - Overlay.height) / 2));

        newTexture.SetPixels(Background.GetPixels());

        for (int y = 0; y < Overlay.height; y++)
        {
            for (int x = 0; x < Overlay.width; x++)
            {
                Color PixelColorFore = Overlay.GetPixel(x, y) * Overlay.GetPixel(x, y).a;
                Color PixelColorBack = Background.GetPixel((int)(x + offset.x), (int)(y + offset.y)) * (1 - PixelColorFore.a);
                newTexture.SetPixel((int)(x + offset.x), (int)(y + offset.y), PixelColorBack + PixelColorFore);
            }
        }

        newTexture.Apply();
        Destroy(Background);

        return newTexture;
    }



    public void InstantiateInfoPopup(String message)
    {
        GameObject g = Instantiate<GameObject>(infoPopupPrefab, canvasObject.transform);
        Text t = g.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        t.text = message;

        Button b = g.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        b.onClick.AddListener(() => { Destroy(g); });
    }


    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}

#elif UNITY_IOS || UNITY_EDITOR


using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using CameraShot;
	using System.Linq;
	using System;
	using UnityEngine.Networking;
	public class CameraController : MonoBehaviour
	{

		private bool isDownloadCompleted = false;
		[SerializeField]
		private bool camSupported = false;
		private WebCamTexture backCam;
		private WebCamTexture frontCam;
		private WebCamTexture cam;
		private Texture defaultBackground;
		private Texture2D photoTaken;
		private bool cameraIsOpen = false;


		[SerializeField]
		private GameObject gameControllerObject;
		[SerializeField]
		private GameObject galleryControllerObject;
		[SerializeField]
		private GameObject touchControllerObject;


		private GameController gameController;
		private Gallery galleryController;
		private TouchController touchController;

		[SerializeField]
		private GameObject imagePanel;
		[SerializeField]
		private GameObject[] panels;
		[SerializeField]
		private Text debugText;

		public bool isFrontCam = true;
		public RawImage background;
		public Image processingImage;

		public AspectRatioFitter fit;


		[SerializeField]
		private GameObject mainModel;

		private bool isDownloading = false;

		[SerializeField]
		private GameObject sceneEditorControllerObj;
		public GameObject infoPopupPrefab;
		public GameObject canvasObject;


		private void OnEnable()
		{
			//CameraShotEventListener.onImageLoad += OnImageLoad;
			CameraShotEventListener.onImageSaved += OnImageSaved;
			CameraShotEventListener.onCancel += CancelCamera;
			CameraShotEventListener.onError += OnError;
		}

		private void OnDisable()
		{
			//CameraShotEventListener.onImageLoad -= OnImageLoad;
			CameraShotEventListener.onImageSaved -= OnImageSaved;
			CameraShotEventListener.onCancel -= CancelCamera;
			CameraShotEventListener.onError -= OnError;
		}
		// Use this for initialization
		void Start()
		{

			//CameraShotEventListener.onImageLoad += OnImageLoad;


			gameController = gameControllerObject.GetComponent<GameController>();
			galleryController = galleryControllerObject.GetComponent<Gallery>();
			touchController = touchControllerObject.GetComponent<TouchController>();
			panels[3].SetActive(false);
			imagePanel.SetActive(false);

		}

		// Update is called once per frame
		void Update()
		{


		}
		public void OpenCamera(bool accessFrontCam = true)
		{
			DestroyImmediate(processingImage.sprite);
			gameController.CollectGurbage(true);


#if UNITY_EDITOR

			OnImageSaved(null);

#else
StartNativeCamera();
#endif
			gameController.ToggleCameraDownMenu(2);
			sceneEditorControllerObj.SetActive(false);


		}

		public void OpenAndroidNativeCamera()
		{
			try
			{
				AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject ajo = new AndroidJavaObject("tico.transindia.com.cameralib.ImageProStatic");
				ajo.CallStatic("OpenCamera", ajc.GetStatic<AndroidJavaObject>("currentActivity"), (int)processingImage.rectTransform.rect.width, (int)processingImage.rectTransform.rect.height,50);
				//ajo.CallStatic("OpenCamera", ajc.GetStatic<AndroidJavaObject>("currentActivity"), 480,640);
			}
			catch(Exception e)
			{
				Debug.Log("Error occured opening Camers : " + e.Message);
				gameController.InstantiateInfoPopup("Error Opening Camera");
				gameController.HideLoadingPanelOnly();
			}
		}





		public void StartNativeCamera()
		{
			Debug.Log("start native camera..");

#if UNITY_ANDROID
gameController.CollectGurbage(true);
gameController.ShowLoadingPanelOnly();
OpenAndroidNativeCamera();

//AndroidCamera.Instance.OnImagePicked += OnImagePicked;
//AndroidCamera.Instance.GetImageFromCamera();


#elif UNITY_IPHONE
			Debug.Log("opening camera on iphone");
			IOSCameraShot.LaunchCameraForImageCapture(false);

#endif

		}









		public void OnPhotoPick(string photoPath)
		{

			if (!photoPath.Contains("file://"))
			{
				photoPath = "file://" + photoPath;
			}
			Debug.Log("Camera-----" + photoPath);
			StartCoroutine(DownloadImageAndUse(photoPath));
		}


		public void OnPhotoCancel(string message)
		{
			Debug.Log("You cancelled the camera");
			gameController.HideLoadingPanelOnly();
		}

		private IEnumerator DownloadImageAndUse(string imageUrl)
		{

			bool success = false;


			using (UnityEngine.Networking.UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
			{
				//print(www.url);
				yield return www.SendWebRequest();

				if (www.isNetworkError || www.isHttpError)
				{
					Debug.Log("Error Occured");
					success = false;
				}
				else
				{
					Texture2D t2d = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
					t2d.Apply();
					Debug.Log(string.Format("width and height of camera image is : {0}  {1}", t2d.width, t2d.height));
					processingImage.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100f);
					success = true;
				}
				//www.Dispose();

			}


			gameController.HideLoadingPanelOnly();
			if(success)
			{
				GoToImageCrop();
			}
			else
			{
				Debug.Log("error setting camera image");
			}
		}
















































		//public void OnImagePicked(AndroidImagePickResult result)
		//{

		//    Debug.Log("image pick");
		//    if (result.IsSucceeded)
		//    {
		//        //DestroyImmediate(result.Image);
		//        //gameController.CollectGurbage(true);
		//        gameController.ShowLoadingPanelOnly();
		//        //AN_PoupsProxy.showMessage("Image Pick Rsult", "Succeeded, path: " + result.ImagePath);
		//        //OnImageSaved(result.ImagePath);
		//        UseImage(result.Image);

		//        //DestroyImmediate(result.Image);

		//    }
		//    else
		//    {
		//        AN_PoupsProxy.showMessage("Image Pick Rsult", "Failed");
		//    }

		//    //gameController.CollectGurbage(true);
		//    AndroidCamera.Instance.OnImagePicked -= OnImagePicked;
		//}




		public void OnImageSaved(string path , ImageOrientation orientation=ImageOrientation.UP)
		{
		print("image orientation : " + orientation);
#if UNITY_EDITOR
			gameController.ShowLoading();

        //			DownloadImage ("http://dentedpixel.com/wp-content/uploads/2014/12/Unity5-0.png");
        //			StartCoroutine(DownloadImage("http://htc-wallpaper.com/wp-content/uploads/2015/10/Old-watch.jpg?982602"));
        print("Downloading image for editor");
			StartCoroutine(DownloadImage("https://i.pinimg.com/originals/08/5e/a3/085ea3a5b5f35242aea65b4431294e40.jpg",ImageOrientation.UP));
			//DownloadImage("https://i.pinimg.com/originals/99/96/b4/9996b4944a37318dfc00c9a34ed78c6b.png");
			//			DownloadImage("file:///C:/Users/Arun/Desktop/Gimp.jpg");
#elif UNITY_ANDROID
//Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
//gameController.ShowLoadingPanelOnly();
//Handheld.StartActivityIndicator();
if(!path.Contains("file://"))
{
path = "file://" + path;
}
print("image path is ........ : " + path);
StartCoroutine(DownloadImage(path));

#elif UNITY_IPHONE

//Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.iOSActivityIndicatorStyle.WhiteLarge);
//gameController.ShowLoadingPanelOnly();
//Handheld.StartActivityIndicator();
if(!path.Contains("file://"))
{
path = "file://" + path;
}
Debug.Log("image path is ........ : " + path);
StartCoroutine(DownloadImage(path,orientation));

#endif

}

public void OnError(string message)
{
InstantiateInfoPopup(message);
gameController.HideLoading();
}





public void UseImage(Texture2D tex)
{

bool success = false;


#if !UNITY_EDITOR
//gameController.ShowLoadingPanelOnly();
#endif





Texture2D t2d = new Texture2D(tex.width, tex.height);
print(string.Format("Tex width : {0} , height : {1}", tex.width, tex.height));
t2d.SetPixels(tex.GetPixels());
t2d.Apply();
DestroyImmediate(tex);


try
{


processingImage.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100f);


Texture2D ttx = MergeImage(t2d);

ttx.Apply();


print("Got image");
processingImage.sprite = Sprite.Create(ttx, new Rect(0, 0, ttx.width, ttx.height), new Vector2(0.5f, 0.5f), 100f);

Destroy(touchController.actualImage);


DestroyImmediate(t2d, true);
success = true;
print("All Success");
}
catch (Exception e)
{
success = false;
Debug.Log(string.Format("Error while downloading image : ", e));

}







if (success)
{

Handheld.StopActivityIndicator();
gameController.HideLoading();
//gameController.HideLoadingPanelOnly();
print("going to imagecrop");

GoToImageCrop();
}
else
{

Handheld.StopActivityIndicator();
//gameController.HideLoadingPanelOnly();
gameController.HideLoading();
print("Some Error occured");
}
gameController.HideLoadingPanelOnly();
//gameController.HideLoading(); 

gameController.CollectGurbage(true);

}



    private IEnumerator DownloadImage(string imageUrl, ImageOrientation orientation = ImageOrientation.UP)
    {

        bool success = false;


        //#if UNITY_ANDROID

        //Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);

#if UNITY_IPHONE
        Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.WhiteLarge);
        Handheld.StartActivityIndicator();

#elif UNITY_TIZEN
Handheld.SetActivityIndicatorStyle(TizenActivityIndicatorStyle.Small);
Handheld.StartActivityIndicator();
#endif

        //Handheld.StartActivityIndicator();
#if !UNITY_EDITOR
gameController.ShowLoadingPanelOnly();
#endif
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            //print(www.url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {


                print("image orientation = " + orientation);
                Texture2D t2d = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
                t2d.Apply();

                if (orientation == ImageOrientation.LEFT)
                {
                    print(string.Format("image orientation is : {0}  : {1} rotating 90", "Left", orientation));
                    t2d = RotateImageDegree(t2d, 90f);
                    t2d.Apply();
                }
                else if (orientation == ImageOrientation.RIGHT)
                {
                    print(string.Format("image orientation is : {0}  : {1} rotating -90", "Right", orientation));
                    t2d = RotateImageDegree(t2d, -90f);
                    t2d.Apply();
                }
                else if (orientation == ImageOrientation.DOWN)
                {
                    t2d = RotateImageDegree(t2d, 180f);
                    t2d.Apply();
                }
                else
                {
                    print(string.Format("image orientation is : {0} ", orientation));
                }



                try
                {

                    //print("good");
                    //print(string.Format("before Process image size : {0} ", processingImage.rectTransform.rect));
                    //processingImage.sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100f);
                    //print(string.Format("after Process image size : {0} {1} ", processingImage.rectTransform.rect, processingImage.sprite.rect));

                    Texture2D ttx = MergeImage(t2d);

                    ttx.Apply();


                    print(string.Format("current size : {0} , {1}", ttx.width, ttx.height));
                    Debug.Log("Image is downloaded");
                    processingImage.sprite = Sprite.Create(ttx, new Rect(0, 0, ttx.width, ttx.height), new Vector2(0.5f, 0.5f), 100f);

                    Destroy(touchController.actualImage);
                    touchController.actualImage = new Texture2D(t2d.width, t2d.height);
                    touchController.actualImage.SetPixels(t2d.GetPixels());
                    touchController.actualImage.Apply();

                    DestroyImmediate(t2d, true);
                    success = true;
                }
                catch (Exception e)
                {
                    success = false;
                    Debug.Log(string.Format("Error while downloading image : ", e));
                    //yield return new WaitForEndOfFrame();
                    //print("stopping loading for camera..");
                    //gameController.HideLoading();
                }
                //UseImage(((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D);

            }



        }

        if (success)
        {
            yield return new WaitForSeconds(1.5f);
            Handheld.StopActivityIndicator();
            gameController.HideLoading();
            gameController.HideLoadingPanelOnly();
            GoToImageCrop();
        }
        else
        {
            yield return new WaitForFixedUpdate();
            Handheld.StopActivityIndicator();
            gameController.HideLoadingPanelOnly();
            gameController.HideLoading();
        }

        //gameController.HideLoading(); 

        gameController.HideLoadingPanelOnly();

    }

    public void CloseCamera()
    {
        if (backCam != null && backCam.isPlaying)
        {
            backCam.Stop();
        }
        if (frontCam != null && frontCam.isPlaying)
        {
            frontCam.Stop();
        }
        if (cam != null && cam.isPlaying)
        {
            cam.Stop();
            cam = null;
        }
        else
        {
            cam = null;
        }

        cameraIsOpen = false;
    }


    Texture2D RotateImageDegreeCrop(Texture2D originTexture, float angle)
    {
        Texture2D result;
        result = new Texture2D(originTexture.width, originTexture.height);
        Color32[] pix1 = result.GetPixels32();
        Color32[] pix2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int x = 0;
        int y = 0;
        Color32[] pix3 = rotateSquare(pix2, (Mathf.PI / 180 * angle), originTexture);
        for (int j = 0; j < H; j++)
        {
            for (var i = 0; i < W; i++)
            {
                //pix1[result.width/2 - originTexture.width/2 + x + i + result.width*(result.height/2-originTexture.height/2+j+y)] = pix2[i + j*originTexture.width];
                pix1[result.width / 2 - W / 2 + x + i + result.width * (result.height / 2 - H / 2 + j + y)] = pix3[i + j * W];
            }
        }
        result.SetPixels32(pix1);
        result.Apply();
        return result;
    }


    Texture2D RotateImageDegree(Texture2D originTexture, float angle)
    {
        int originalTextureWidth = originTexture.width;
        int originalTextureHeight = originTexture.height;
        int newSize = Mathf.Max(originalTextureWidth, originalTextureHeight)+10;
        Texture2D result;
        Texture2D mergedOriginalTexture;
        if (angle<180f)
        {
            Texture2D backgroundTexture = new Texture2D(newSize, newSize);
            //Color[] clr= Enumerable.Repeat(Color.clear, newSize * newSize).ToArray();
            backgroundTexture.SetPixels(Enumerable.Repeat(Color.clear, newSize * newSize).ToArray());
            backgroundTexture.Apply();
            mergedOriginalTexture = JustMergeImage(originTexture, backgroundTexture);
            mergedOriginalTexture.Apply();
            result = new Texture2D(newSize, newSize);
        }
        else
        {

            mergedOriginalTexture =new Texture2D(originalTextureWidth,originalTextureHeight);
            mergedOriginalTexture.SetPixels(originTexture.GetPixels());
            mergedOriginalTexture.Apply();
            result = new Texture2D(originalTextureWidth, originalTextureHeight);
        }
        
        
        Color32[] pix1 = result.GetPixels32();
        Color32[] pix2 = mergedOriginalTexture.GetPixels32();
        int W = mergedOriginalTexture.width;
        int H = mergedOriginalTexture.height;
        int x = 0;
        int y = 0;
        Color32[] pix3 = rotateSquare(pix2, (Mathf.PI / 180 * angle), mergedOriginalTexture);
        for (int j = 0; j < H; j++)
        {
            for (var i = 0; i < W; i++)
            {
                //pix1[result.width/2 - originTexture.width/2 + x + i + result.width*(result.height/2-originTexture.height/2+j+y)] = pix2[i + j*originTexture.width];
                pix1[result.width / 2 - W / 2 + x + i + result.width * (result.height / 2 - H / 2 + j + y)] = pix3[i + j * W];
            }
        }
        result.SetPixels32(pix1);
        result.Apply();
        return result;
    }


    Color32[] rotateSquare(Color32[] arr, float phi, Texture2D originTexture)
    {
        int x;
        int y;
        int i;
        int j;
        float sn = Mathf.Sin(phi);
        float cs = Mathf.Cos(phi);
        Color32[] arr2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int xc = W / 2;
        int yc = H / 2;
        for (j = 0; j < H; j++)
        {
            for (i = 0; i < W; i++)
            {
                arr2[j * W + i] = new Color32(0, 0, 0, 0);
                x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                if ((x > -1) && (x < W) && (y > -1) && (y < H))
                {
                    arr2[j * W + i] = arr[y * W + x];
                }
            }
        }

        return arr2;
    }


    public void TakePicture()
{
if (cam != null)
{
Debug.Log("camara available");
if (cam.isPlaying)
{
Debug.Log("camera playing");
photoTaken = new Texture2D(background.texture.width, background.texture.height);
photoTaken.SetPixels(cam.GetPixels());
photoTaken.Apply();
photoTaken = RotateImageDegree(photoTaken, 90);
photoTaken.Apply();
CloseCamera();
processingImage.sprite = Sprite.Create(photoTaken, new Rect(0, 0, photoTaken.width, photoTaken.height), new Vector2(0.5f, 0.5f), 100f);

Texture2D ttx = MergeImage(photoTaken);
processingImage.sprite = Sprite.Create(ttx, new Rect(0, 0, ttx.width, ttx.height), new Vector2(0.5f, 0.5f), 100f);
photoTaken = null;

GoToImageCrop();

}
}
}
private void GoToImageCrop()
{
//Go To Image Panel;
panels[0].SetActive(true);
panels[1].SetActive(true);
for (int i = 2; i < panels.Length; i++)
{
panels[i].SetActive(false);
}
touchController.ToggleMask(1);
gameController.HideAcceptCropButton();
imagePanel.SetActive(true);

sceneEditorControllerObj.SetActive(false);
//gameController.HideLoading();
gameController.HideLoadingPanelOnly();

}
public void CancelCamera()
{
//CloseCamera ();
mainModel.SetActive(true);
gameController.ToggleHomeSideMenu(1);
panels[3].SetActive(false);
panels[2].SetActive(true);
panels[1].SetActive(true);
panels[0].SetActive(true);
sceneEditorControllerObj.SetActive(true);
//		Debug.Log ("closing camera");
}
public void UpdateImage()
{
if (!cameraIsOpen)
{
return;
}

//		float ratio = (float)cam.width / (float)cam.height;
//		fit.aspectRatio = ratio;
if (!isFrontCam)
{
float scaleY = cam.videoVerticallyMirrored ? -1f : 1f;
background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

int orient = -cam.videoRotationAngle;

background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
}
else
{
float scaleY = cam.videoVerticallyMirrored ? 1f : -1f;
background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
debugText.text = cam.videoVerticallyMirrored.ToString();
int orient = -cam.videoRotationAngle;
background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
}
}








public Texture2D ResizeTexture2D(Texture2D texture, int newWidth, int newHeight)
{
float warpFactor = 1.0F;
//		Texture2D destTex = new Texture2D(Screen.width, Screen.height);
Texture2D destTex = new Texture2D(newWidth, newHeight);
print("resize texture size : " + destTex.width + "   " + destTex.height);
//		print ("Screen size : " + Screen.width + "   " + Screen.height);
Color[] destPix = new Color[destTex.width * destTex.height];
try
{
int y = 0;
while (y < destTex.height)
{
int x = 0;
while (x < destTex.width)
{
float xFrac = x * 1.0F / (destTex.width - 1);
float yFrac = y * 1.0F / (destTex.height - 1);
float warpXFrac = Mathf.Pow(xFrac, warpFactor);
float warpYFrac = Mathf.Pow(yFrac, warpFactor);
destPix[y * destTex.width + x] = texture.GetPixelBilinear(warpXFrac, warpYFrac);
x++;
}
y++;
}
destTex.SetPixels(destPix);
destTex.Apply();

}
catch
{
destTex = null;
}

//destImage.sprite = Sprite.Create(destTex, new Rect(0, 0, destTex.width, destTex.height), new Vector2(.5f, .5f), 100);

return destTex;
}

    private Texture2D MergeImage(Texture2D Overlay, Texture2D Background = null, Texture2D newTexture = null)
    {
        //print(string.Format("overlay width : {0}  overlay height{1}  pimg rect trnsfrm width : {2} pimg rect trnsfrm Height : {3} ", Overlay.width, Overlay.height, processingImage.rectTransform.rect.width, processingImage.rectTransform.rect.height));
        if ((Overlay.width > processingImage.rectTransform.rect.width) || (Overlay.height > processingImage.rectTransform.rect.height))
        {
            //			Overlay.Resize (1300,1300, Overlay.format, false);
            //			Overlay.Apply ();
            int resizeHeight = Overlay.height;
            int resizeWidth = Overlay.width;

            if (Overlay.width > processingImage.rectTransform.rect.width)
            {
                resizeWidth = (int)processingImage.rectTransform.rect.width;
                float r = (float)Overlay.width / (float)resizeWidth;
                resizeHeight = (int)(Overlay.height / r);
                print(string.Format("Width is big Resize width : {0} resize hight {1} when r is : {2}", resizeWidth, resizeHeight, r));
            }
            /*
            if (Overlay.height > processingImage.rectTransform.rect.height) {
            resizeHeight =(int) processingImage.rectTransform.rect.height;
            float r = (float)Overlay.height / (float)resizeHeight;
            resizeWidth = (int)(Overlay.width / r);
            print(string.Format("Height is big Resize width : {0} resize hight {1} when r is : {2}", resizeWidth, resizeHeight, r));
            }
            */



            Texture2D resizedOverlay = ResizeTexture2D(Overlay, resizeWidth, resizeHeight);
            Overlay.Resize(resizeWidth, resizeHeight);
            Overlay.Apply();
            //			Overlay=ResizeTexture2D (Overlay, resizeWidth, resizeHeight);
            Overlay.SetPixels(resizedOverlay.GetPixels());
            Overlay.Apply();
            Destroy(resizedOverlay);
        }

        if (Background == null)
        {
            Background = new Texture2D((int)processingImage.rectTransform.rect.width, (int)processingImage.rectTransform.rect.height, TextureFormat.ARGB32, false);
            Color fillColor = Color.clear;
            //Color[] fillPixels = new Color[Background.width * Background.height];

            //for (int i = 0; i < fillPixels.Length; i++)
            //{
            //	fillPixels[i] = fillColor;
            //}

            Color[] fillPixels = Enumerable.Repeat(Color.clear, Background.width * Background.height).ToArray();

            Background.SetPixels(fillPixels);
            Background.Apply();

        }
        if (newTexture == null)
        {
            newTexture = new Texture2D((int)processingImage.rectTransform.rect.width, (int)processingImage.rectTransform.rect.height, TextureFormat.ARGB32, false);
            //print ("big image rect : "+processingImage.rectTransform.rect);
        }


        Vector2 offset = new Vector2(((newTexture.width - Overlay.width) / 2), ((newTexture.height - Overlay.height) / 2));

        newTexture.SetPixels(Background.GetPixels());

        for (int y = 0; y < Overlay.height; y++)
        {
            for (int x = 0; x < Overlay.width; x++)
            {
                Color PixelColorFore = Overlay.GetPixel(x, y) * Overlay.GetPixel(x, y).a;
                Color PixelColorBack = Background.GetPixel((int)(x + offset.x), (int)(y + offset.y)) * (1 - PixelColorFore.a);
                newTexture.SetPixel((int)(x + offset.x), (int)(y + offset.y), PixelColorBack + PixelColorFore);
            }
        }

        newTexture.Apply();
        Destroy(Background);

        return newTexture;
    }


    private Texture2D JustMergeImage(Texture2D Overlay, Texture2D Background = null, Texture2D newTexture = null)
    {
        

        if (Background == null)
        {
            Background = new Texture2D((int)Overlay.width, (int)Overlay.height, TextureFormat.ARGB32, false);
            Color fillColor = Color.clear;
            //Color[] fillPixels = new Color[Background.width * Background.height];

            //for (int i = 0; i < fillPixels.Length; i++)
            //{
            //	fillPixels[i] = fillColor;
            //}

            Color[] fillPixels = Enumerable.Repeat(Color.clear, Background.width * Background.height).ToArray();

            Background.SetPixels(fillPixels);
            Background.Apply();

        }
        if (newTexture == null)
        {
            newTexture = new Texture2D((int)Background.width, (int)Background.height, TextureFormat.ARGB32, false);
            //print ("big image rect : "+processingImage.rectTransform.rect);
        }


        Vector2 offset = new Vector2(((newTexture.width - Overlay.width) / 2), ((newTexture.height - Overlay.height) / 2));

        newTexture.SetPixels(Background.GetPixels());

        for (int y = 0; y < Overlay.height; y++)
        {
            for (int x = 0; x < Overlay.width; x++)
            {
                Color PixelColorFore = Overlay.GetPixel(x, y) * Overlay.GetPixel(x, y).a;
                Color PixelColorBack = Background.GetPixel((int)(x + offset.x), (int)(y + offset.y)) * (1 - PixelColorFore.a);
                newTexture.SetPixel((int)(x + offset.x), (int)(y + offset.y), PixelColorBack + PixelColorFore);
            }
        }

        newTexture.Apply();
        Destroy(Background);

        return newTexture;
    }



    public void InstantiateInfoPopup(String message)
{
GameObject g = Instantiate<GameObject>(infoPopupPrefab, canvasObject.transform);
Text t = g.transform.GetChild(0).GetChild(0).GetComponent<Text>();
t.text = message;

Button b = g.transform.GetChild(0).GetChild(1).GetComponent<Button>();
b.onClick.AddListener(() => { Destroy(g); });
}


Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
{
Color32[] original = originalTexture.GetPixels32();
Color32[] rotated = new Color32[original.Length];
int w = originalTexture.width;
int h = originalTexture.height;

int iRotated, iOriginal;

for (int j = 0; j < h; ++j)
{
for (int i = 0; i < w; ++i)
{
iRotated = (i + 1) * h - j - 1;
iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
rotated[iRotated] = original[iOriginal];
}
}

Texture2D rotatedTexture = new Texture2D(h, w);
rotatedTexture.SetPixels32(rotated);
rotatedTexture.Apply();
return rotatedTexture;
}
}

















/*
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android" package="com.test.camerashot" android:installLocation="preferExternal" android:versionName="1.0" android:versionCode="1">
<supports-screens android:smallScreens="true" android:normalScreens="true" android:largeScreens="true" android:xlargeScreens="true" android:anyDensity="true" />
<application android:theme="@style/UnityThemeSelector" android:icon="@drawable/app_icon" android:label="@string/app_name" android:debuggable="true">
<activity android:name="com.unity3d.player.UnityPlayerActivity" android:label="@string/app_name">
<intent-filter>
<action android:name="android.intent.action.MAIN" />
<category android:name="android.intent.category.LAUNCHER" />
</intent-filter>
<meta-data android:name="unityplayer.UnityActivity" android:value="true" />
</activity>

<activity 
android:name="com.astricstore.camerashots.CameraShotActivity"
android:configChanges="orientation|keyboardHidden|screenSize">
</activity>
<!--
<activity 
android:name="eu.janmuller.android.simplecropimage.CropImage"
android:configChanges="orientation|keyboardHidden|screenSize">
</activity>
-->
<activity 
android:name= "com.radikallabs.androidgallery.Gallery"
android:configChanges="orientation|keyboardHidden|screenSize">
</activity>

<provider
android:name="android.support.v4.content.FileProvider"
android:authorities="com.isis.venus.provider"
android:exported="false"
android:grantUriPermissions="true">
<meta-data
android:name="android.support.FILE_PROVIDER_PATHS"
android:resource="@xml/provider_paths"/>
</provider>

</application>
<uses-sdk android:minSdkVersion="9" android:targetSdkVersion="24" />
<uses-feature android:glEsVersion="0x00020000" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_INTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_INTERNAL_STORAGE" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
<uses-feature android:name="android.hardware.camera.front" android:required="false" />
</manifest>
*/



#endif









/*
 <?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android" package="com.test.camerashot" android:installLocation="preferExternal" android:versionName="1.0" android:versionCode="1">
  <supports-screens android:smallScreens="true" android:normalScreens="true" android:largeScreens="true" android:xlargeScreens="true" android:anyDensity="true" />
  <application android:theme="@style/UnityThemeSelector" android:icon="@drawable/app_icon" android:label="@string/app_name" android:debuggable="true">
    <activity android:name="com.unity3d.player.UnityPlayerActivity" android:label="@string/app_name">
      <intent-filter>
        <action android:name="android.intent.action.MAIN" />
        <category android:name="android.intent.category.LAUNCHER" />
      </intent-filter>
      <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
    </activity>

    <activity 
    android:name="com.astricstore.camerashots.CameraShotActivity"
    android:configChanges="orientation|keyboardHidden|screenSize">
</activity>
    <!--
<activity 
    android:name="eu.janmuller.android.simplecropimage.CropImage"
    android:configChanges="orientation|keyboardHidden|screenSize">
    </activity>
    -->
    <activity 
      android:name= "com.radikallabs.androidgallery.Gallery"
      android:configChanges="orientation|keyboardHidden|screenSize">
    </activity>

    <provider
            android:name="android.support.v4.content.FileProvider"
            android:authorities="com.isis.venus.provider"
            android:exported="false"
            android:grantUriPermissions="true">
            <meta-data
                android:name="android.support.FILE_PROVIDER_PATHS"
                android:resource="@xml/provider_paths"/>
        </provider>

  </application>
  <uses-sdk android:minSdkVersion="9" android:targetSdkVersion="24" />
<uses-feature android:glEsVersion="0x00020000" />
  <uses-permission android:name="android.permission.INTERNET" />
  <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.WRITE_INTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.READ_INTERNAL_STORAGE" />
  <uses-permission android:name="android.permission.CAMERA" />
  <uses-feature android:name="android.hardware.camera" android:required="false" />
  <uses-feature android:name="android.hardware.camera.front" android:required="false" />
</manifest>
     */

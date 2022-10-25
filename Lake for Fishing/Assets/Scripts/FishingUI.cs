using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class FishingUI : MonoBehaviour
{
    [System.Serializable]
    public class Result
    {
        public string result;
        public int id;
        public int bassCaught;
        public int muskieCaught;
        public int blueGillCaught;
        public int bassTotal;
        public int muskieTotal;
        public int blueGillTotal;
        public int coin;
    }

    public static FishingUI instance;

    // Fishing Main UI
    public GameObject main;
    public GameObject signinMenu;
    public GameObject signupMenu;  
    public GameObject inventoryMenu; 
    public GameObject startMenu; 
    public InputField nameInput;
    public InputField passwordInput;
    public InputField name1Input;
    public InputField password1Input;
    public InputField password2Input;  
    public GameObject alert;
    public Text alertText;
    public Text bluegillCaughtText;
    public Text bluegillSellText;
    public Text bluegillTotalText;
    public Text bassCaughtText;
    public Text bassSellText;
    public Text bassTotalText;
    public Text muskieCaughtText;
    public Text muskieSellText;
    public Text muskieTotalText;
    public Text priceText;
    float showingTime = 0;
    bool isShowing = false;

    // Api URL
    public string absURL = "";
    string signupURL;
    string signinURL;
    string readURL;
    string sellURL;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        main.transform.localScale = new Vector3(Screen.width / 1366f, Screen.height / 768f, 1f);
        signupURL = absURL + "api/signup";
        signinURL = absURL + "api/login";
        readURL = absURL + "api/getUserData";
        sellURL = absURL + "api/sell";
    }

    void Update()
    {
        main.transform.localScale = new Vector3(Screen.width / 1366f, Screen.height / 768f, 1f);

        // Show alert(Warning).        
        if(isShowing)
        {
            alert.SetActive(true);
            showingTime -= Time.deltaTime;
        }
        else
        {
            alert.SetActive(false);
        }

        if(showingTime < 0)
        {
            isShowing = false;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // SignIn Menu
    public void SigninBtnClick()
    {
        if(nameInput.text != "" && passwordInput.text != "")
        {            
            // Request "sign in" Api.
            StopAllCoroutines();
            StartCoroutine(PostRequestSign(signinURL, nameInput.text, passwordInput.text));
        }
        else
        {
            isShowing = true;
            showingTime = 2f;
            alertText.text = "Please fill all fields.";
        }        
    }

    public void Signup0BtnClick()
    {
        signinMenu.SetActive(false);
        signupMenu.SetActive(true);
        nameInput.text = "";
        passwordInput.text = "";
    }    

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //SignUp Menu
    public void SignupBtnClick()
    {
        if(name1Input.text != "" && password1Input.text != "" && password2Input.text != "")
        {
            if(password1Input.text == password2Input.text)
            {
                // Request "sign up" Api
                StopAllCoroutines();
                StartCoroutine(PostRequestSign(signupURL, name1Input.text, password1Input.text));                                
            }
            else
            {
                isShowing = true;
                showingTime = 2f;
                alertText.text = "Please type password again.";
                password2Input.text = "";
            }  
        }
        else
        {
            isShowing = true;
            showingTime = 2f;
            alertText.text = "Please fill all fields.";
        }                     
    }

    public void SignupBackBtnClick()
    {
        signinMenu.SetActive(true);
        signupMenu.SetActive(false);
        name1Input.text = "";
        password1Input.text = "";
        password2Input.text = "";
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Inventory Menu    
    public void InventoryBackBtnClick()
    {
        inventoryMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void SellBtnClick()
    {
        // Requrest "sell" Api
        StopAllCoroutines();
        StartCoroutine(RequestSell(sellURL));                
    }

    public void PlusBlueGillBtnClick()
    {
        if(int.Parse(bluegillCaughtText.text) > int.Parse(bluegillSellText.text))
        {
            bluegillSellText.text = (int.Parse(bluegillSellText.text) + 1).ToString();
        }
    }

    public void MinusBlueGillBtnClick()
    {
        if(int.Parse(bluegillSellText.text) > 0)
        {
            bluegillSellText.text = (int.Parse(bluegillSellText.text) - 1).ToString();
        }
    }

    public void PlusBassBtnClick()
    {
        if(int.Parse(bassCaughtText.text) > int.Parse(bassSellText.text))
        {
            bassSellText.text = (int.Parse(bassSellText.text) + 1).ToString();
        }
    }

    public void MinusBassBtnClick()
    {
        if(int.Parse(bassSellText.text) > 0)
        {
            bassSellText.text = (int.Parse(bassSellText.text) - 1).ToString();
        }
    }

    public void PlusMuskieBtnClick()
    {
        if(int.Parse(muskieCaughtText.text) > int.Parse(muskieSellText.text))
        {
            muskieSellText.text = (int.Parse(muskieSellText.text) + 1).ToString();
        }
    }

    public void MinusMuskieBtnClick()
    {
        if(int.Parse(muskieSellText.text) > 0)
        {
            muskieSellText.text = (int.Parse(muskieSellText.text) - 1).ToString();
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Start Menu
    public void StartBtnClick()
    {
        // Start the game.
        SceneManager.LoadScene("Game");
    }

    public void SignoutBtnClick()
    {
        nameInput.text = "";
        passwordInput.text = "";
        startMenu.SetActive(false);
        signinMenu.SetActive(true);
    }

    public void InventoryBtnClick()
    {
        bassSellText.text = "0";
        bluegillSellText.text = "0";
        muskieSellText.text = "0";
        startMenu.SetActive(false);
        inventoryMenu.SetActive(true);

        // Request "read data" Api
        StopAllCoroutines();
        StartCoroutine(RequestRead(readURL));
    }
    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Apis
    IEnumerator PostRequestSign(string url, string name, string password)
    {               
        // Send user name and password.
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("password", password);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            // Recieve data from backend.
            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);
            string result = loadData.result;
            int userId = loadData.id;

            if(url == signinURL)
            {
                if(string.Equals(result, "1")) // Sign in successfully.
                {
                    PlayerPrefs.SetInt("USER_ID", userId);
                    signinMenu.SetActive(false);
                    signupMenu.SetActive(false);
                    startMenu.SetActive(true);                                        
                }
                else if(string.Equals(result, "2")) // User doesn't existed
                {
                    isShowing = true;
                    showingTime = 2f;
                    alertText.text = "Not registered";
                }
                else // Enter wrong password.
                {
                    isShowing = true;
                    showingTime = 2f;
                    alertText.text = "Wrong password";
                    passwordInput.text = "";
                }
            }
            else if(url == signupURL)
            {
                if(string.Equals(result, "1")) // User already exists
                {
                    isShowing = true;
                    showingTime = 2f;
                    alertText.text = "Already Exists";
                }
                else // Sign up succesfully.
                {
                    signinMenu.SetActive(true);
                    signupMenu.SetActive(false);
                    isShowing = true;
                    showingTime = 2f;
                    alertText.text = "Successful";
                }                                                
            }
        }
    }

    IEnumerator RequestRead(string url)
    {               
        // Send user_id to server.
        WWWForm form = new WWWForm();
        form.AddField("id", PlayerPrefs.GetInt("USER_ID"));        

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            // Receive user data
            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);
            bluegillCaughtText.text = loadData.blueGillCaught.ToString();   
            bassCaughtText.text = loadData.bassCaught.ToString();
            muskieCaughtText.text = loadData.muskieCaught.ToString();
            bluegillTotalText.text = loadData.blueGillTotal.ToString();
            bassTotalText.text = loadData.bassTotal.ToString();
            muskieTotalText.text = loadData.muskieTotal.ToString();    
            priceText.text = loadData.coin.ToString();     
        }
    }

    IEnumerator RequestSell(string url)
    {               
        // Send "sell data" to server
        WWWForm form = new WWWForm();
        form.AddField("id", PlayerPrefs.GetInt("USER_ID")); 
        form.AddField("bass", int.Parse(bassSellText.text));
        form.AddField("blueGill", int.Parse(bluegillSellText.text));
        form.AddField("muskie", int.Parse(muskieSellText.text));    
        form.AddField("coin", int.Parse(bluegillSellText.text) * 120 + int.Parse(bassSellText.text) * 150
                            + int.Parse(muskieSellText.text) * 100);  

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            // Receive results from server.
            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);
            bassCaughtText.text = loadData.bassCaught.ToString();
            bluegillCaughtText.text = loadData.blueGillCaught.ToString();
            muskieCaughtText.text = loadData.muskieCaught.ToString();
            priceText.text = loadData.coin.ToString();
            bluegillSellText.text = "0";
            bassSellText.text = "0";
            muskieSellText.text = "0";                           
        }
    }
}

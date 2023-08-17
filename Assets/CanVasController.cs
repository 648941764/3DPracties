using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanVasController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvasBackpack;
    [SerializeField]
    private Canvas canvasSignIn;
    public Button BackpackBtn, SignInBtn;

    private bool isOpenBackpack;
    private bool isOpenSignIn;

    private void Awake()
    {
        //canvasBackpack.enabled = false;
        //  canvasSignIn.enabled = false;
        BackpackBtn.onClick.AddListener(OnBackpackBtnClicked);
        SignInBtn.onClick.AddListener(OnSignInBtnClicked);
    }

    public void OnBackpackBtnClicked()
    {
        if (isOpenBackpack)
        {
            //canvasBackpack.enabled = false;
            canvasBackpack.gameObject.SetActive(false);
            isOpenBackpack = false;
        }
        else
        {
            isOpenBackpack = true;
            isOpenSignIn = false;
            //canvasBackpack.enabled = true;
            canvasBackpack.gameObject.SetActive(true);
            //canvasSignIn.enabled = false;
            canvasSignIn.gameObject.SetActive(false);
        }
    }

    public void OnSignInBtnClicked()
    {
        if (isOpenSignIn)
        {
            //canvasSignIn.enabled = false;
            canvasSignIn.gameObject.SetActive(false);
            isOpenSignIn = false;
        }
        else
        {
            isOpenSignIn = true;
            isOpenBackpack = false;
            //canvasSignIn.enabled = true;
            canvasSignIn.gameObject.SetActive(true);
            //canvasBackpack.enabled = false;
            canvasBackpack.gameObject.SetActive(false);
        }
    }
}

﻿using UnityEngine;
using System.Collections;
using Facebook;
using CloudGoodsSDK.Models;
using System;
using CloudGoods;
using Newtonsoft.Json.Linq;

public class FaceBookMobileAuthentication : MonoBehaviour
{
    public static event System.Action loggedIN;
    public static event System.Action OnRecivedUserInfo;
    public static string FBUserName = string.Empty;

    void OnEnable()
    {
        loggedIN += FaceBookMobileAuthentication_loggedIN;

    }

    void OnDisable()
    {
        loggedIN -= FaceBookMobileAuthentication_loggedIN;
    }

    void Start()
    {
        FB.Init(SetInit, OnHideUnity);

    }
    void FaceBookMobileAuthentication_loggedIN()
    {
        Debug.Log(FB.IsLoggedIn);
        Debug.Log(FB.UserId);


        FB.API("/me?fields=Email", HttpMethod.GET, UserNameCallBack);
    }

    void UserNameCallBack(FBResult response)
    {
        if (string.IsNullOrEmpty(response.Error))
        {
            FBUserName = JToken.Parse(response.Text)["Email"].ToString();
            var socialPlayUserObj = new PlatformUser();
            socialPlayUserObj.appID = new Guid(GameAuthentication.GetAppID());
            socialPlayUserObj.platformID = 1;
            socialPlayUserObj.platformUserID = FB.UserId;
            socialPlayUserObj.userName = FBUserName;
            Systems.UserGetter.GetSocialPlayUser(socialPlayUserObj, GameAuthentication.OnUserAuthorized);
        }

        if (OnRecivedUserInfo != null)
        {
            OnRecivedUserInfo();
        }
    }

    void AuthCallback(FBResult result)
    {

        Debug.Log(result.Text);
        if (FB.IsLoggedIn)
        {
            Debug.Log(FB.UserId);
            if (loggedIN != null)
            {
                loggedIN();
            }
        }
        else
        {
            Debug.Log("User cancelled RecivedLoginCode");
        }
    }



    private void SetInit()
    {

        enabled = true; // "enabled" is a magic global

        if (!FB.IsLoggedIn)
        {
            FB.Login("email", AuthCallback);
        }
        else
        {
            if (loggedIN != null)
            {
                loggedIN();
            }
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        { // pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        { // start the game back up - we're getting focus again
            Time.timeScale = 1;
        }
    }
}

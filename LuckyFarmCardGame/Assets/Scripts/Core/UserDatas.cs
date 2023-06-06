using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosina.Components;
using System.Linq;
using Random = System.Random;

[System.Serializable]
public class UserDatas
{
    public UserInfo info;

    public static UserDatas Instance
   {
      get
      {
         return GameDataManager.Instance.GameDatas.userDatas;
      }
   }
   public UserDatas()
   {
    }
    public static UserDatas Create()
    {
        return new UserDatas()
        {
            info = UserInfo.Create(),
        };
    }
    public void CreateUser()
   {

    }

    public void OpenGame(double totalOffline)
    {
        //CRemoteConfigManager.OnLoadRemoteSuccess -= OnLoadRemoteSuccess;
        //CRemoteConfigManager.OnLoadRemoteSuccess += OnLoadRemoteSuccess;
    }
    private void OnLoadRemoteSuccess()
    {

    }
    public void OnFocusApp()
    {
    }
}
[System.Serializable]
public class UserInfo
{  
   public string id;
   public string nickname;
   public string avatar;

   private Sprite sprAvatar;

   public string Avatar
   {
       get
       {
           
           return this.avatar;
       }
       set { 
           this.avatar = value;
       }
   }

   public Sprite SprAvatar
   {
       get
       {
            if (this.sprAvatar == null)
            {
                //this.sprAvatar = CommonAvatar.Instance.GetAvatarById(this.avatar);
            }
           return this.sprAvatar;
       }
       set
       {
           this.sprAvatar = value;
       }
   }
    public static UserInfo Create()
    {
        int id = GenerateRandomNumber();
        string name = $"Player_{id}";
#pragma warning disable CS0618
        return new UserInfo()
#pragma warning restore CS0618
        {
            id = id.ToString(),
            nickname = name.Substring(0, Math.Min(10, name.Length)) ,
            Avatar = "0",
        };
    }
    private static readonly Random Random = new Random();
    private static int GenerateRandomNumber()
    {
        return Random.Next(int.MaxValue);
    }
    [System.Obsolete("Use UserInfo.Create instead!")]
    public UserInfo()
   {
   }

    public UserInfo(UserInfo i)
    {
        this.id = i.id;
        this.nickname = i.nickname;
        this.Avatar = i.avatar;
    }

    public UserInfo(string id, string name, string avatar, string skinID)
    {
        this.id = id;
        this.nickname = name;
        this.Avatar = avatar;
    }

    public void LoginFirebase(string userId)
    {
        this.id = userId;
        this.SaveData();
    }
    public void ChangeName(string name)
    {
        this.nickname = name;
#if PLAYFAB
        CPSocialManager.Instance.UpdateUserDisplayName(name);
#endif

        SaveData();
    }

    public void ChangeAvatar(string avatar)
    {
        this.Avatar = avatar;

#if PLAYFAB
        CPSocialManager.Instance.UpdateUserAvatar(avatar, onSuccess: ()=> Debug.Log("Update avatar to server"));
#endif

        SaveData();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}

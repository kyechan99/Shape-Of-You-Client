﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reset : MonoBehaviour
{
    public void ResetFn()
    {
        Debug.Log("RESET !!!!!!");
        for (int i = 0; i < GM.NetworkManager.getInstance.v_user.Count; i++)
        {
            if (GM.NetworkManager.getInstance.v_user[i] != null)
            {
                GM.NetworkManager.getInstance.v_user[i].isLive = true;
                GM.NetworkManager.getInstance.v_user[i].nhp = 1;
                GM.NetworkManager.getInstance.v_user[i].pos = new Vector2(0f, 0f);
                GM.NetworkManager.getInstance.v_user[i].bBlind = false;
                GM.NetworkManager.getInstance.v_user[i].fSpeed = 9f;
                GM.NetworkManager.getInstance.v_user[i].SkillGame.SetActive(false);
                GM.NetworkManager.getInstance.v_user[i].proper = PROPER.GENERAL;
                GM.NetworkManager.getInstance.v_user[i].color = COLOR.WHITE;
            }
        }
    }
}
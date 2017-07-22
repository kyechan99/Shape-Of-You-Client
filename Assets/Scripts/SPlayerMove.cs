﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPlayerMove : MonoBehaviour
{
    public string nickName;
    public int myIdx = 0;

    public bool isLive = true;
    public int nhp;
    public bool isPlayer = false;
    public Vector3 pos;
    public GameObject colGame = null;
    public GameObject blindGame = null;
    public bool bBlind;
    public bool bStartup;
    public float fSpeed;
    public Color[] colorcls = null;

    public MOVE_CONTROL myMove = MOVE_CONTROL.STOP;
    public MOVE_CONTROL beforeMove = MOVE_CONTROL.STOP;
    public PROPER proper = PROPER.GENERAL;
    public COLOR color = COLOR.WHITE;

    SWatching WatchScrp = null;
    [SerializeField]
    SpriteRenderer sprite = null;

    void Start()
    {
        fSpeed = 9f;
        nhp = 1;
        SetUp();
        WatchScrp = Camera.main.GetComponent<SWatching>();
    }

    void Update()
    {
        if (isPlayer && isLive)
        {
            if (proper == PROPER.POLICE) { SGameMng.I.uiScrp.GetSkill(nhp.ToString()); }
            Blind();
            if (Input.GetKey(KeyCode.UpArrow)) myMove = MOVE_CONTROL.UP;
            else if (Input.GetKey(KeyCode.LeftArrow)) myMove = MOVE_CONTROL.LEFT;
            else if (Input.GetKey(KeyCode.RightArrow)) myMove = MOVE_CONTROL.RIGHT;
            else if (Input.GetKey(KeyCode.DownArrow)) myMove = MOVE_CONTROL.DOWN;
            else myMove = MOVE_CONTROL.STOP;

            if (myMove != beforeMove && !bStartup)
            {
                GM.NetworkManager.getInstance.SendMsg(string.Format("MOVE:{0}:{1}:{2}:{3}", myIdx, transform.position.x, transform.position.y, (int)myMove));
                beforeMove = myMove;
            }
            if (Input.GetKeyDown(KeyCode.Space) && proper.Equals(PROPER.POLICE))
            {
                GM.NetworkManager.getInstance.SendMsg(string.Format("ATTACK:{0}", myIdx));
                Attack();
                Debug.Log("attack down");
            }
            if (nhp <= 0)
            {
                GM.NetworkManager.getInstance.SendMsg(string.Format("DIE:{0}", myIdx));
            }
            WatchScrp.Move(this.transform);
        }
        if (isLive && nhp <= 0)
        {
            Debug.Log("PlayerDie");
            Die();
        }

        if (myMove == MOVE_CONTROL.UP)
        {
            //anim.Play("UP");
            transform.Translate(Vector3.up * fSpeed * Time.deltaTime);
        }
        else if (myMove == MOVE_CONTROL.DOWN)
        {
            //anim.Play("DOWN");
            transform.Translate(Vector3.down * fSpeed * Time.deltaTime);
        }
        else if (myMove == MOVE_CONTROL.LEFT)
        {
            //anim.Play("LEFT");
            transform.Translate(Vector3.left * fSpeed * Time.deltaTime);
        }
        else if (myMove == MOVE_CONTROL.RIGHT)
        {
            //anim.Play("RIGHT");
            transform.Translate(Vector3.right * fSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!proper.Equals(PROPER.POLICE))       // 경찰이 아닐때
        {
            if (col.CompareTag("Police") && col.GetComponent<SPlayerMove>().color == color && !bStartup)
            {
                nhp = -1;
                WatchScrp.GetLive(isLive);
            }
            if (col.CompareTag("Pcolider"))
            {
                nhp = -1;
                WatchScrp.GetLive(isLive);
            }
            if (nhp <= 0) GM.NetworkManager.getInstance.SendMsg(string.Format("DIE:{0}:{1}", myIdx, col.GetComponent<SPlayerMove>().myIdx));
        }
    }

    public void SetUp()
    {
        ChangeColor();
        if (proper.Equals(PROPER.POLICE))
        {
            gameObject.tag = "Police";
            nhp = 10;
        }
        else if (proper.Equals(PROPER.THIEF))
        {
            gameObject.tag = "Player";
            nhp = 1;
        }

        else if (proper.Equals(PROPER.GENERAL))
        {
            gameObject.tag = "General";
            nhp = 1;
        }
    }

    public void Attack()
    {
        nhp -= 1;
        if (nhp >= 0)
            StartCoroutine("Big");
        else nhp = 0;
    }

    IEnumerator Big()
    {
        colGame.SetActive(true);
        yield return new WaitForSeconds(1f);
        StartCoroutine("Small");
    }
    IEnumerator Small()
    {
        colGame.SetActive(false);
        yield return null;
    }

    public void ChangeColor()
    {
        for (int i = 0; i < (int)COLOR.E_MAX; i++)
        {
            if (i == (int)color)
                sprite.color = colorcls[i];
        }
    }

    void Die()
    {
        if (nhp <= 0)
        {
           if(proper==PROPER.POLICE) GM.NetworkManager.getInstance.SendMsg(string.Format("DIE:{0}:{1}", myIdx, myIdx));
            gameObject.SetActive(false);
        }
    }

    void Blind()
    {
        if (proper == PROPER.POLICE && !bBlind)
        {
            bStartup = true;
            SGameMng.I.uiScrp.SkillActive();
            blindGame.SetActive(true);
            fSpeed = 0f;
            bBlind = true;
        }
        if (SGameMng.I.sTimer.Equals("2:45"))
        {
            bStartup = false;
            blindGame.SetActive(false);
            fSpeed = 10f;
        }
    }
}

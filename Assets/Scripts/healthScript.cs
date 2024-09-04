using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthScript : MonoBehaviour
{
    public LogicScript logic;
    private Image image;
    private Sprite zeroHeal;
    private Sprite oneHeal;
    private Sprite twoHeal;
    private Sprite threeHeal;
    public AudioSource takeDmgVoice;

    private int heal = 3;

    void Start()
    {
        image = GetComponent<Image>();
        zeroHeal = Resources.Load<Sprite>("life bar/heal_zero");
        oneHeal = Resources.Load<Sprite>("life bar/heal_one");
        twoHeal = Resources.Load<Sprite>("life bar/heal_two");
        threeHeal = Resources.Load<Sprite>("life bar/heal_three");
    }

    public void addHealth()
    {
        if(heal == 1)
        {
            heal++;
            image.sprite = twoHeal;
        }
        else if(heal == 2)
        {
            heal++;
            image.sprite = threeHeal;
        }
    }
    public void takeDamage()
    {
        if (heal == 1)
        {
            heal--;
            image.sprite = zeroHeal;
            logic.gameOver();
        }
        else if (heal == 2)
        {
            takeDmgVoice.Play();
            heal--;
            image.sprite = oneHeal;
        }
        else if (heal == 3)
        {
            takeDmgVoice.Play();
            heal--;
            image.sprite = twoHeal;
        }
    }

    public int getHeal()
    {
        return heal;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    private float xpLimit = 10000.0f;
    private float damage;
    private float reloadingTime;
    private float xp;
    private int level;
    private int lifetime;
    private Vector3 impactZone;
    private int mag; //optional
    private bool echo; // optional

    // Start is called before the first frame update

    int upgradeLevel(float damageBuff = 1)
    {
        if (level < 100)
        {
            damage += damageBuff;
            level += 1;
        }
        return level;
    }

    int updateLifeTime(int newLifeTime)
    {
        lifetime = newLifeTime;
        return lifetime;
    }

    int updateMag()
    {
        mag -= 1;
        return mag;
    }

    int updateXp(float newXp)
    {
        if (xp + newXp >= xpLimit)
        {
            if (level < 100)
            {
                level += 1;
                xp = xpLimit - (xp + newXp);
                return upgradeLevel();
            }
            return level;
        }
        xp += newXp;
        return level;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

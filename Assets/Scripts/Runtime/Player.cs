using UnityEngine;

public class Player
{
    public int
        hp, mp, maxHp, maxMp;
    
    public Player(int maxHp, int maxMp)
    {
        hp = maxHp;
        mp = maxMp;
        this.maxHp = maxHp;
        this.maxMp = maxMp;
    }

    public void ChangeHp(int change)
    {
        hp = Mathf.Clamp(hp + change, 0, maxHp);
    }


    public void ChangeMp(int change)
    {
        mp = Mathf.Clamp(mp + change, 0, maxMp);
    }
}


public class ItemData 
{
    public int id;
    public string name;
    public string icon;
    public string description;
    public int attaack;
    public int defense;
    public float speed;
    public int hP;
    public int mP;
    public ItemType type;
    public EquimentType eType;

    public enum ItemType
    {
        Equipment,Normal
    }

    public enum EquimentType
    {
        None,Weapon,Armor,Shoes
    }
}

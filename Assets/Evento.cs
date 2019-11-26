using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evento
{
    public string id;
    public string material;
    public Vector3 position;
    //public DateTime dateTime;


    // Start is called before the first frame update
    public Evento(string id, string material, Vector3 position)
    {
        //this.dateTime = DateTime.Now;
        this.id = id;
        this.material = material;
        this.position = position;
    }

    public override string ToString()
    {
        string res = "";
        res += "id: " + this.id + "\n";
        res += "material: " + this.material + "\n";
        res += "posicion" + position.ToString();
        //res += "x: " + this.position.x + "    y:" + this.position.y + "    z:" + this.position.z;
        return res;
    }
}

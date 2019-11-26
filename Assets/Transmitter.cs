using System.Collections;
using System.Collections.Generic;
using BigInteger = System.Numerics.BigInteger;
using System.Text;
using UnityEngine;

public class Transmitter : MonoBehaviour
{
    public GameObject[] whiteList;
    public bool isTower;
    public float range,
                 espera;
    public BigInteger privateKey = BigInteger.Parse("9516311845790656153499716760847001433441357"), 
                      publicKey = BigInteger.Parse("5617843187844953170308463622230283376298685"),
                      e = 65537;
    public List<Evento> qEv;
    public List<GameObject> dest;

    public List<Evento> logCod = new List<Evento>();
    public List<Evento> logDec = new List<Evento>();

    // Start is called before the first frame update
    void Start()
    {
        this.espera = 0.1f;
        this.isTower = true;
        this.qEv = new List<Evento>();
        StartCoroutine(generateRandomEvent());
        dest = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            // Imprime avion
            GameObject go = GameObject.FindGameObjectWithTag("avion");

            Debug.Log("Historial de mensajes de: " + go.tag);

            List<Evento> cod = go.GetComponent<Transmitter>().logCod;
            List<Evento> decod = go.GetComponent<Transmitter>().logDec;

            Debug.Log("cod size: " + cod.Count);

            for (int i = 0; i < cod.Count; i++)
            {
                Debug.Log("Codificado:\n" + cod[i].ToString());
                Debug.Log("Decodificado:\n" + decod[i].ToString());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            // Imprime torre1
            GameObject go = GameObject.FindGameObjectWithTag("torre1");

            Debug.Log("Historial de mensajes de: " + go.name);

            List<Evento> cod = go.GetComponent<Transmitter>().logCod;
            List<Evento> decod = go.GetComponent<Transmitter>().logDec;

            for (int i = 0; i < cod.Count; i++)
            {
                Debug.Log("Codificado:\n" + cod[i].ToString());
                Debug.Log("Decodificado:\n" + decod[i].ToString());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            // Imprime torre2s
            GameObject go = GameObject.FindGameObjectWithTag("torre2");

            Debug.Log("Historial de mensajes de: " + go.name);

            List<Evento> cod = go.GetComponent<Transmitter>().logCod;
            List<Evento> decod = go.GetComponent<Transmitter>().logDec;
            
            for (int i = 0; i < cod.Count; i++)
            {
                Debug.Log("Codificado:\n" + cod[i].ToString());
                Debug.Log("Decodificado:\n" + decod[i].ToString());
            }
        }
    }

    public IEnumerator generateRandomEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5.0f, 15.0f));
            string[] materiales = { "metalico", "magnético", "translúcido", "componente biológico" };
            string id = Random.Range(10000, 99999).ToString();
            string material = materiales[(int)Random.Range(0, 4)];
            GameObject destino = whiteList[(int)Random.Range(0, whiteList.Length)];
            Vector3 position = new Vector3(Random.Range(-10000.0f, 10000.0f), Random.Range(500.0f, 10000.0f), Random.Range(-10000.0f, 10000.0f));
            Evento ev = new Evento(id, material, position);
            qEv.Add(ev);
            Debug.Log(ev.ToString());
            Debug.Log("\n" + gameObject.tag + " -> " + destino.tag);
            dest.Add(destino);
        }
    }


    IEnumerator enviaMensaje(Evento mensaje, GameObject destino)
    {
        GameObject tmp = destino;
        if (isTower)
        {
            
            destino = GameObject.FindGameObjectWithTag("avion");
        }
        for (int i = 0; i < 1000; i++)
        {
            yield return new WaitForSeconds(this.espera);
            if (checaSiRango(destino))
            {
                destino.GetComponent<Transmitter>().recibeMensaje(RSAencode(mensaje), tmp);
            }
        }
    }

    void recibeMensaje(Evento evento, GameObject destino)
    {
        if (GameObject.ReferenceEquals(destino, this.gameObject))
        {
            logCod.Add(evento);
            Evento dec = RSAdecode(evento);
            logDec.Add(dec);
            Debug.Log("Codificado:\n" + evento.ToString());
            Debug.Log("Decodificado:\n" + dec.ToString());
        }
        else
        {
            enviaMensaje(evento, destino);
        }
        
    } 

    bool checaSiRango(GameObject destino)
    {
        for (int i = 0; i < whiteList.Length; i++)
        {
            if (GameObject.ReferenceEquals(this.whiteList[i], (destino)) && Vector3.Distance(this.transform.position, destino.transform.position) <= this.range)
            {
                return true;
            }
        }
        return false;
    }



    public Evento RSAencode(Evento aCodificar)
    {
        Debug.Log("Codificando mensaje...");
        // Aqui va el algoritmo RSA 8)
        BigInteger id = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aCodificar.id));
        BigInteger mat = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aCodificar.material));
        BigInteger x = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aCodificar.position.x.ToString()));
        BigInteger y = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aCodificar.position.y.ToString()));
        BigInteger z = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aCodificar.position.z.ToString()));

        if (id > this.publicKey || mat > this.publicKey || x > this.publicKey || y > this.publicKey || z > this.publicKey)
        {
            throw new System.ArgumentException("Parameter cannot be encoded", "original");
        }

        string cId = ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(id, this.e, this.publicKey).ToByteArray());
        string cMat = ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(mat, this.e, this.publicKey).ToByteArray());
        float cX = float.Parse(ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(x, this.e, this.publicKey).ToByteArray()));
        float cY = float.Parse(ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(y, this.e, this.publicKey).ToByteArray()));
        float cZ = float.Parse(ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(z, this.e, this.publicKey).ToByteArray()));
        Evento ev = new Evento(cId, cMat, new Vector3(cX, cY, cZ));
        Debug.Log("Codificado: " + ev.ToString());
        return ev;        
    }

    public Evento RSAdecode(Evento aDecodificar)
    {
        // Aqui va el algoritmo RSA 8)
        BigInteger id = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aDecodificar.id));
        BigInteger mat = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aDecodificar.material));
        BigInteger x = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aDecodificar.position.x.ToString()));
        BigInteger y = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aDecodificar.position.y.ToString()));
        BigInteger z = new BigInteger(ASCIIEncoding.ASCII.GetBytes(aDecodificar.position.z.ToString()));

        if (id > this.publicKey)
        {
            throw new System.ArgumentException("Parameter cannot be encoded", "original");
        }

        string dId = ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(id, this.privateKey, this.publicKey).ToByteArray());
        string dMat = ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(mat, this.privateKey, this.publicKey).ToByteArray());
        float dX = float.Parse(ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(x, this.privateKey, this.publicKey).ToByteArray()));
        float dY = float.Parse(ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(y, this.privateKey, this.publicKey).ToByteArray()));
        float dZ = float.Parse(ASCIIEncoding.ASCII.GetString(BigInteger.ModPow(z, this.privateKey, this.publicKey).ToByteArray()));
        Evento ev = new Evento(dId, dMat, new Vector3(dX, dY, dZ));
        return ev;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Entrando en area con: " + other.tag);
        // Hay mensajes por enviar
        if(qEv.Count > 0)
        {
            for(int i = 0; i < qEv.Count; i++)
            {
                if (GameObject.ReferenceEquals(dest[i], other.gameObject))
                {
                    enviaMensaje(qEv[i], dest[i]);
                    qEv.RemoveAt(i);
                    dest.RemoveAt(i);
                }
            }
        }
    }

}

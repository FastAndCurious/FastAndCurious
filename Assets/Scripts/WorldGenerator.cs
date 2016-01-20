using UnityEngine;
using System.Collections;
using UnityEditor;

using System.IO;
using System.Collections.Generic;

/// <summary>
/// klasa koja generira matricu grada za simulaciju
/// 
/// 1. generiranje ruba grada           x
/// 2. generiranje zaobilaznice         x
/// 3. generiranje glavnih avenija      x
/// 4. generiranje kvartovskih ulica    x
/// 5. generiranje visina zgrada
/// 
/// </summary>


enum Smjer { Gore, Desno, Dolje, Lijevo }

// svako polje mape sadrzi objekt ove klase
// tipovi blokova:
    //  0: " "  1: "╨"  2: "╞"  3: "╚"
    //  4: "╥"  5: "║"  6: "╔"  7: "╠"
    //  8: "╡"  9: "╝"  10: "═" 11: "╩"
    //  12: "╗" 13: "╣" 14: "╦" 15: "╬"

public class elementMape
{
    public int id;          //oznacava tip bloka ceste
    public bool[] ogranak;  //reprezentacija bloka pomocu bitova - gore, desno, dolje, lijevo
    public bool avenija;    // da li je blok dio avenije
    public int opcija;      //dodatne opcije za buduce koristenje

    //pretvorba tip objekta u popis ogranaka
    public void idUOgranak()
    {
        ogranak[0] = ogranak[1] = ogranak[2] = ogranak[3] = false;
        if ((id & 1) == 1)
            ogranak[0] = true;
        if ((id & 2) == 2)
            ogranak[1] = true;
        if ((id & 4) == 4)
            ogranak[2] = true;
        if ((id & 8) == 8)
            ogranak[3] = true;

    }

    //pretvorba popis ogranaka u tip objekta
    public void ogranakUId()
    {
        id = 0;
        id |= ogranak[0] ? 1 : 0;
        id |= ogranak[1] ? 2 : 0;
        id |= ogranak[2] ? 4 : 0;
        id |= ogranak[3] ? 8 : 0;
    }

    public elementMape()
    {
        id = -1;
        ogranak = new bool[4];
        ogranak[0] = ogranak[1] = ogranak[2] = ogranak[3] = false;
        avenija = false;
        opcija = 0;
    }
    public elementMape(int id, bool avenija)
    {
        this.id = id;
        this.avenija = avenija;
        idUOgranak();
        opcija = 0; 
    }
    public void dodajOgranak(int smjer)
    {
        ogranak[smjer] = true;
        ogranakUId();
        
    }
    

}

// svaki cravler koji se pojavi je opisan objektom ove klase
public class Cravler
{
    public int x, y, smjer;

    public Cravler(int x, int y, int smjer)
    {
        this.x = x;
        this.y = y;
        this.smjer = smjer;
    }
}

/// <summary>
/// todo:
/// generiranje visina zgrada
/// detekcija blokova pravokutnika za zgrade i parkove
/// 
/// </summary>

public class elementZgrade
{
    public float x1, x2, y1, y2, visina;
    public elementZgrade (float X1, float Y1, float X2, float Y2, float Visina)
    {
        x1 = X1;
        y1 = Y1;
        x2 = X2;
        y2 = Y2;
        visina = Visina;
    }
}


public class WorldGenerator : MonoBehaviour
{
    
    public GameObject prefabZgrada;
    public float velicina_blokova;
    public static elementMape[][] mapa;            //opisuje prometnu povezanost grada
    public static List<elementZgrade> zgrade;
    public GameObject[] U, R, D, L, UD, RL, UR, RD, DL, UL, URD, RDL, UDL, URL, URDL;

    private static GameObject[,] mapaBlokova;
    private static float[] vanjske_tocke;                   //vanjski obrub grada: -polarni zapis
    private static int[,] kartezijeve_tocke;                //                     -kartezijev zapis
    private static Queue<Cravler> cravler;                  //spremnik za cravlera
    private static int velicina_mape = 10;

    private static elementMape[][] pomMapa;
    private static int max_x, max_y, min_x, min_y;
    private static int[] pomakY = new int[4] { 1, 0, -1, 0 };
    private static int[] pomakX = new int[4] { 0, 1, 0, -1 };

    private static int[,] mapaBool;

    //oznacava sve blokove ispod proslijedene linije
    private static void oznaciNaMapi(int x1, int y1, int x2, int y2)
    {
        if (x1 >= x2)
        {
            int buf = x1;
            x1 = x2;
            x2 = buf;
            buf = y1;
            y1 = y2;
            y2 = buf;
        }

        float korak = (y2 - y1) / (float)(x2 - x1 + 1);
        for (int k = x1; k < x2; k++)
        {
            for (int l = 0; l < y1 + korak * (k - x1); l++)
                if (mapa[l][k].id == -1)
                    mapa[l][k].id = 0;
                else
                    mapa[l][k].id = -1;
        }


    }

    private static void napraviZaobilaznicu(int velicina_mape)
    {
        pomMapa = new elementMape[velicina_mape][];
        for (int i = 0; i < velicina_mape; i++)
        {
            pomMapa[i] = new elementMape[velicina_mape];
            for (int j = 0; j < velicina_mape; j++)            
                pomMapa[i][j] = new elementMape();            
        }

        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
                pomMapa[i][j].id = mapa[i][j].id;


        //napravi cestu okolo svih oznacenih blokova te nasumicno odabere neke blokove kao pocetak za cravlera
        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
                if (mapa[i][j].id == 0)
                {

                    if (mapa[i - 1][j].id == -1 && mapa[i - 1][j - 1].id == -1) { pomMapa[i - 1][j].dodajOgranak((int)Smjer.Lijevo); pomMapa[i - 1][j - 1].dodajOgranak((int)Smjer.Desno); }
                    if (mapa[i - 1][j].id == -1 && mapa[i - 1][j + 1].id == -1) { pomMapa[i - 1][j].dodajOgranak((int)Smjer.Desno); pomMapa[i - 1][j + 1].dodajOgranak((int)Smjer.Lijevo); }
                    if (mapa[i + 1][j].id == -1 && mapa[i + 1][j - 1].id == -1) { pomMapa[i + 1][j].dodajOgranak((int)Smjer.Lijevo); pomMapa[i + 1][j - 1].dodajOgranak((int)Smjer.Desno); }
                    if (mapa[i + 1][j].id == -1 && mapa[i + 1][j + 1].id == -1) { pomMapa[i + 1][j].dodajOgranak((int)Smjer.Desno); pomMapa[i + 1][j + 1].dodajOgranak((int)Smjer.Lijevo); }

                    if (mapa[i - 1][j - 1].id == -1 && mapa[i][j - 1].id == -1) { pomMapa[i - 1][j - 1].dodajOgranak((int)Smjer.Gore); pomMapa[i][j - 1].dodajOgranak((int)Smjer.Dolje); }
                    if (mapa[i + 1][j - 1].id == -1 && mapa[i][j - 1].id == -1) { pomMapa[i + 1][j - 1].dodajOgranak((int)Smjer.Dolje); pomMapa[i][j - 1].dodajOgranak((int)Smjer.Gore); }
                    if (mapa[i - 1][j + 1].id == -1 && mapa[i][j + 1].id == -1) { pomMapa[i - 1][j + 1].dodajOgranak((int)Smjer.Gore); pomMapa[i][j + 1].dodajOgranak((int)Smjer.Dolje); }
                    if (mapa[i + 1][j + 1].id == -1 && mapa[i][j + 1].id == -1) { pomMapa[i + 1][j + 1].dodajOgranak((int)Smjer.Dolje); pomMapa[i][j + 1].dodajOgranak((int)Smjer.Gore); }
                    
                    if (Random.value < 0.125)
                        for (int k = 0; k < 4; k++)
                        {
                            if (mapa[i + pomakY[k]][j + pomakX[k]].id == -1)
                            {
                                pomMapa[i][j].dodajOgranak(k);
                                pomMapa[i + pomakY[k]][j + pomakX[k]].dodajOgranak((k + 2) % 4);
                                cravler.Enqueue(new Cravler(j, i, (k + 2) % 4));
                            }
                        }


                }

        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
                mapa[i][j] = pomMapa[i][j];
    }

    void Start()
    {
        // buduci argumenti poziva funkcije, a ne konstruktora

        

        int pojavljivanje_avenije = velicina_mape / 10;
        if (pojavljivanje_avenije < 10)
            pojavljivanje_avenije = 10;
        float vjerojatnost_cravlera = 0.2f;


        //pocetna stanja
        cravler = new Queue<Cravler>();
        mapa = new elementMape[velicina_mape][];
        for (int i = 0; i < velicina_mape; i++)
        {
            mapa[i] = new elementMape[velicina_mape];
            for (int j = 0; j < velicina_mape; j++)            
                mapa[i][j] = new elementMape();
            
        }
            

        //odaberi slucajne polarne tocke s jednakim kutem izmedu njih

        int broj_tocaka = (int)(Random.value * 15 + 5);
        vanjske_tocke = new float[broj_tocaka];
        kartezijeve_tocke = new int[broj_tocaka, 2];

        for (int i = 0; i < broj_tocaka; i++)
        {
            vanjske_tocke[i] = Random.value * velicina_mape/3 + velicina_mape/7 ;
        }

        //pretvori polarne u kartezijeve tocke
        for (int i = 0; i < broj_tocaka; i++)
        {
            int os_x = (int)Mathf.FloorToInt(Mathf.Cos(2 * Mathf.PI * i / broj_tocaka) * vanjske_tocke[i]) + velicina_mape / 2;
            int os_y = (int)Mathf.FloorToInt(Mathf.Sin(2 * Mathf.PI * i / broj_tocaka) * vanjske_tocke[i]) + velicina_mape / 2;
            kartezijeve_tocke[i, 0] = os_x;
            kartezijeve_tocke[i, 1] = os_y;

            //print("x = " +  kartezijeve_tocke[i,0] + "y = " + kartezijeve_tocke[i,1]);

        }

        max_x = min_x = kartezijeve_tocke[0, 0];
        max_y = min_y = kartezijeve_tocke[0, 0];
        for (int i = 1; i < broj_tocaka; i++)
        {
            if (max_x < kartezijeve_tocke[i, 0])
                max_x = kartezijeve_tocke[i, 0];
            if (min_x > kartezijeve_tocke[i, 0])
                min_x = kartezijeve_tocke[i, 0];
            if (max_y < kartezijeve_tocke[i, 1])
                max_y = kartezijeve_tocke[i, 1];
            if (min_y > kartezijeve_tocke[i, 1])
                min_y = kartezijeve_tocke[i, 1];
        }


        // oznaci na mapi podrucja koja su obiljezena tim tockama

        for (int i = 0; i < broj_tocaka - 1; i++)
            oznaciNaMapi(kartezijeve_tocke[i, 0], kartezijeve_tocke[i, 1], kartezijeve_tocke[i + 1, 0], kartezijeve_tocke[i + 1, 1]);

        oznaciNaMapi(kartezijeve_tocke[broj_tocaka-1, 0], kartezijeve_tocke[broj_tocaka-1, 1], kartezijeve_tocke[0, 0], kartezijeve_tocke[0, 1]);

        //napravi zaobilaznicu
        napraviZaobilaznicu(velicina_mape);


        // napravi avenije
        print("min_x:" + min_x + "max_x: " + max_x + " min_y: " + min_y + "max_y: "+ max_y);

         
        for (int i = min_x + (int)(pojavljivanje_avenije * Random.value) + pojavljivanje_avenije; 
            i <  max_x - pojavljivanje_avenije; 
            i+= (int)(pojavljivanje_avenije * Random.value) + pojavljivanje_avenije)
        {
            for (int j = 0; j < velicina_mape; j++)
                if (mapa[j][i].id != -1)
                {
                    if (mapa[j + 1][i].id != -1)
                    {
                       
                        mapa[j][i].avenija = true;
                        mapa[j][i].dodajOgranak((int)Smjer.Gore);
                    }
                    if (mapa[j - 1][i].id != -1)
                    {
                        mapa[j][i].avenija = true;
                        mapa[j][i].dodajOgranak((int)Smjer.Dolje);
                    }
                }
        }

        for (int j = min_y + (int)(pojavljivanje_avenije * Random.value) + pojavljivanje_avenije;
            j < max_y - pojavljivanje_avenije; 
            j += (int)(pojavljivanje_avenije * Random.value) + pojavljivanje_avenije)
        {
            for (int i = 0; i < velicina_mape; i++)
                if (mapa[j][i].id != -1)
                {
                    if (mapa[j][i].avenija )
                        for (int k = -2; k <= 2; k++)
                        {
                            if (mapa[j + k][i].id != -1) mapa[j + k][i].opcija = 1;
                            if (mapa[j][i + k].id != -1) mapa[j][i + k].opcija = 1;
                        }

                    if (mapa[j][i + 1].id != -1)
                    {
                        mapa[j][i].avenija = true;
                        mapa[j][i].dodajOgranak((int)Smjer.Desno);
                    }
                    if (mapa[j][i - 1].id != -1)
                    {
                        mapa[j][i].avenija = true;
                        mapa[j][i].dodajOgranak((int)Smjer.Lijevo);
                    }
                }
        }
        
        //postavljanje cravlera

        

        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
                if (mapa[i][j].avenija && (mapa[i][j].opcija & 1) == 0 && Random.value < vjerojatnost_cravlera)
                {
                    float slucajanBroj = Random.value;
                    
                    if(slucajanBroj < 0.8)
                    {
                        //lijevo grananje
                        if (mapa[i][j].ogranak[(int)Smjer.Desno] && mapa[i - 1][j].id == 0)
                        {
                            for (int k = -3; k <= 3; k++)
                                mapa[i][j + k].opcija |= 1;
                            mapa[i][j].dodajOgranak((int)Smjer.Dolje);
                            mapa[i - 1][j].dodajOgranak((int)Smjer.Gore);
                            cravler.Enqueue(new Cravler(j , i - 1, (int)Smjer.Dolje));
                        }
                        if (mapa[i][j].ogranak[(int)Smjer.Gore] && mapa[i][j - 1].id == 0)
                        {
                            for (int k = -3; k <= 3; k++)
                                mapa[i + k][j].opcija |= 1;
                            mapa[i][j].dodajOgranak((int)Smjer.Lijevo);
                            mapa[i][j - 1].dodajOgranak((int)Smjer.Desno);
                            cravler.Enqueue(new Cravler(j - 1, i, (int)Smjer.Lijevo));
                        }


                    }
                    if(slucajanBroj > 0.2)
                    {
                        //desno grananje
                        if (mapa[i][j].ogranak[(int)Smjer.Lijevo] && mapa[i + 1][j].id == 0)
                        {
                            for (int k = -3; k <= 3; k++)
                                mapa[i][j + k].opcija |= 1;
                            mapa[i][j].dodajOgranak((int)Smjer.Gore);
                            mapa[i + 1][j].dodajOgranak((int)Smjer.Dolje);
                            cravler.Enqueue(new Cravler(j, i + 1, (int)Smjer.Gore));
                        }
                        if (mapa[i][j].ogranak[(int)Smjer.Dolje] && mapa[i][j + 1].id == 0)
                        {
                            for (int k = -3; k <= 3; k++)
                                mapa[i + k][j].opcija |= 1;
                            mapa[i][j].dodajOgranak((int)Smjer.Desno);
                            mapa[i][j + 1].dodajOgranak((int)Smjer.Lijevo);
                            cravler.Enqueue(new Cravler(j + 1, i, (int)Smjer.Desno));
                        }
                    }
                }



      

        // generiranje kvartovskih ulica
        while (cravler.Count != 0)
        {
            Cravler trenutni = cravler.Dequeue();
            float broj_nastavaka = Random.value;
            float[] smjer = { 1, 1, 1, 1 };

            smjer[(trenutni.smjer + 2) % 4] = 0;
            smjer[trenutni.smjer] += 4;

            if (mapa[trenutni.y + 1][trenutni.x].avenija) smjer[(int)Smjer.Gore] = 0;
            if (mapa[trenutni.y - 1][trenutni.x].avenija) smjer[(int)Smjer.Dolje] = 0;
            if (mapa[trenutni.y][trenutni.x + 1].avenija) smjer[(int)Smjer.Desno] = 0;
            if (mapa[trenutni.y][trenutni.x - 1].avenija) smjer[(int)Smjer.Lijevo] = 0;

            int br = 0;
            float zbroj = 0;
            for (int i = 0; i < 4; i++)
                if (smjer[i] != 0)
                {
                    zbroj += smjer[i];
                    br++;
                }

            for (int i = 0; i < broj_nastavaka * br * 0.8- 0.1; i++)
            {
                
                float slucajan_broj = Random.value * zbroj;
                for (int j = 0; j < 4; j++)
                {
                    slucajan_broj -= smjer[j];
                    if (slucajan_broj <= 0)
                    {
                        if (mapa[trenutni.y + pomakY[j]][trenutni.x + pomakX[j]].id == 0)
                            cravler.Enqueue(new Cravler(trenutni.x + pomakX[j], trenutni.y + pomakY[j], j));
                        mapa[trenutni.y][trenutni.x].dodajOgranak(j);
                        mapa[trenutni.y + pomakY[j]][trenutni.x + pomakX[j]].dodajOgranak((j + 2) % 4);
                        
                        break;
                    }
                        
                }
            
            }
            

        }

        mapaBool = new int[velicina_mape, velicina_mape];
        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
                if (mapa[i][j].id == 0)
                    mapaBool[i, j] = 0;
                else
                    mapaBool[i, j] = 1;

        zgrade = new List<elementZgrade>();
        while (najveciPravokutnik() == 0) ;

        foreach (elementZgrade zgrada in zgrade)
        {
            zgrada.visina = 10*Mathf.PerlinNoise((zgrada.x1 + zgrada.x2) / (2*velicina_mape), (zgrada.y1 + zgrada.y2) / (2 * velicina_mape)) ;
            zgrada.visina += (float)(3 * (Random.value - 0.5));
        }

       

        // debug print
        string fileName = "debugIzlaz.txt"; 

		if (File.Exists(fileName))
		{
			Debug.Log(fileName+" already exists.");
			//return;
		}
		var sr = File.CreateText(fileName);

		
        for (int i = velicina_mape-1; i >=0; i--)
        {
			string buffer = "";
            for (int j = 0; j < velicina_mape; j++)
            {
                if (false && mapa[i][j].opcija == 1)
                    buffer += "+";
                else 
                switch (mapa[i][j].id)
                {
                    case 0: buffer += " "; break;
                    case 1: buffer += "╨"; break;
                    case 2: buffer += "╞"; break;
                    case 3: buffer += "╚"; break;
                    case 4: buffer += "╥"; break;
                    case 5: buffer += "║"; break;
                    case 6: buffer += "╔"; break;
                    case 7: buffer += "╠"; break;
                    case 8: buffer += "╡"; break;
                    case 9: buffer += "╝"; break;
                    case 10: buffer += "═"; break;
                    case 11: buffer += "╩"; break;
                    case 12: buffer += "╗"; break;
                    case 13: buffer += "╣"; break;
                    case 14: buffer += "╦"; break;
                    case 15: buffer += "╬"; break;
                    default:
                        buffer = buffer + "█";
                        break;
                }
				
            }
			sr.WriteLine (buffer);

        }
		sr.Close();


        iscrtajMapu();
    }

    private void iscrtajMapu()
    {
        bool playerPostoji = false;
        GameObject ovaj = GameObject.Find("WG_SpawnPoint");
        mapaBlokova = new GameObject[velicina_mape, velicina_mape];
        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
            {
                GameObject noviBlok;
                Vector3 pomak = new Vector3((j) * velicina_blokova, 0, (i) * velicina_blokova);
                Quaternion zakret;
                
                
                switch (mapa[i][j].id)
                {
                    //  0: " "  1: "╨"  2: "╞"  3: "╚"
                    //  4: "╥"  5: "║"  6: "╔"  7: "╠"
                    //  8: "╡"  9: "╝"  10: "═" 11: "╩"
                    //  12: "╗" 13: "╣" 14: "╦" 15: "╬"
                    case 1: noviBlok = U[0];  break;
                    case 2: noviBlok = R[0];  break;
                    case 3: noviBlok = UR[0];  break;
                    case 4: noviBlok = D[0];  break;
                    case 5: noviBlok = UD[0]; break;
                    case 6: noviBlok = RD[0];  break;
                    case 7: noviBlok = URD[0];  break;
                    case 8: noviBlok = L[0];  break;
                    case 9: noviBlok = UL[0];  break;
                    case 10: noviBlok = RL[0];  break;
                    case 11: noviBlok = URL[0];  break;
                    case 12: noviBlok = DL[0];  break;
                    case 13: noviBlok = UDL[0];  break;
                    case 14: noviBlok = RDL[0];  break;
                    case 15: noviBlok = URDL[0];  break;

                    default:
                        noviBlok = null;
                        break;
                }
                if (!playerPostoji && mapa[i][j].id == 5)
                {
                    noviBlok = UD[UD.Length - 1];
                    playerPostoji = true;
                }else if (!playerPostoji && mapa[i][j].id == 10)
                {
                    noviBlok = RL[RL.Length - 1];
                    playerPostoji = true;
                }else if (Random.value < 0.25)
                {
                    float slucajan = Random.value;
                    if (slucajan == 1) slucajan = (float)0.99;
                    if (mapa[i][j].id == 10 && RL.Length > 2)
                    {
                     
                        int broj = (int)(slucajan * (RL.Length - 2) + 1);
                        noviBlok = RL[broj];
                    }
                    if (mapa[i][j].id == 5 && UD.Length > 2)
                    {
                        int broj = (int)(slucajan * (UD.Length - 2) + 1);
                        noviBlok = UD[broj];
                    }
                }
                
                zakret = Quaternion.Euler(0, 0, 0);
                if (noviBlok != null)
                {
                    mapaBlokova[i, j] = ((GameObject)GameObject.Instantiate(noviBlok, pomak, zakret));
                    mapaBlokova[i, j].transform.parent = ovaj.transform;
                }
            }

        
        MapPartController[,] matricaSkripti = new MapPartController[velicina_mape, velicina_mape];

        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
                if (mapa[i][j].id > 0)
                    matricaSkripti[i, j] = mapaBlokova[i, j].GetComponent<MapPartController>();


        for (int i = 0; i < velicina_mape; i++)
            for (int j = 0; j < velicina_mape; j++)
            {
                Debug.Log("mapa:" + i + " , " + j);
                if (mapa[i][j].id > 0)
                {
                    if (i > 0 && mapa[i - 1][j].id > 0)
                    {
                        matricaSkripti[i, j].setSouthPart(matricaSkripti[i - 1, j]);
                       
                    }
                    if (i < velicina_mape - 1 && mapa[i + 1][j].id > 0)
                    {
                        matricaSkripti[i, j].setNorthPart(matricaSkripti[i + 1, j]);
                        
                    }
                    if (j > 0 && mapa[i][j - 1].id > 0)
                    {
                        matricaSkripti[i, j - 1].setEastPart(matricaSkripti[i, j]);
                    }
                    if (j < velicina_mape - 1 && mapa[i][j + 1].id > 0)
                    {
                        matricaSkripti[i, j + 1].setWestPart(matricaSkripti[i, j]);
                    }

                }
            }
        
        Debug.Log("izasao");
        
        foreach (elementZgrade zgrada in zgrade)
        {
            Vector3 position = new Vector3(((zgrada.x2 + zgrada.x1) / 2) * velicina_blokova, zgrada.visina*50 / 2, ((zgrada.y2 + zgrada.y1) / 2 * velicina_blokova));
            GameObject cube = ((GameObject)GameObject.Instantiate(prefabZgrada, position, Quaternion.Euler(0,0,0)));
            cube.transform.localScale = new Vector3((zgrada.x2 - zgrada.x1 + 1) * velicina_blokova, zgrada.visina*50, ((zgrada.y2 - zgrada.y1 + 1) * velicina_blokova));
            cube.transform.parent = ovaj.transform;
        }
    }

    private int najveciPravokutnik() {
        int x1 = -1, x2 = -1 , y1 = -1, y2 = -1, z = -1;

        int[,] lijevo = new int[velicina_mape, velicina_mape];
        int[,] desno = new int[velicina_mape, velicina_mape];
        for (int i = 0; i < velicina_mape; i++)
        {
            int brojac = 0;
            for (int j = 0; j < velicina_mape; j++)
            {
                brojac++;
                if (mapaBool[i , j] != 0) brojac = 0;
                lijevo[i, j] = brojac;
            }
            brojac = 0;
            for (int j = velicina_mape-1; j >= 0; j--)
            {
                brojac++;
                if (mapaBool[i, j] != 0) brojac = 0;
                desno[i, j] = brojac;
            }
        }

        int maxArea = 0;
        for (int j = 0; j < velicina_mape; j++)
        {

            int zadnji = 0, moguLijevo = 10000, moguDesno = 10000;
            for (int i = 0; i < velicina_mape; i++)
            {
                zadnji++;
                moguLijevo = Mathf.Min(moguLijevo, lijevo[i, j]);
                moguDesno = Mathf.Min(moguDesno, desno[i, j]);
                if (mapaBool[i, j] != 0)
                { 
                    zadnji = 0;
                    moguLijevo = moguDesno = 10000;
                }

                int newArea = zadnji * (moguLijevo + moguDesno - 1);
                if (maxArea < newArea)
                {
                    maxArea = newArea;
                    z = i;
                    x1 = j - moguLijevo + 1;
                    x2 = j + moguDesno - 1;
                    y1 = i - zadnji +1;
                    y2 = i;

                } 
            }
        }
        float visina = 1;

        if (x1 == -1)
            return -1;

        for (int j = x1; j <= x2; j++)
            for (int i = y1; i <= y2; i++)
                mapaBool[i, j] = 1;

        zgrade.Add(new elementZgrade(x1, y1, x2, y2, visina));
        //Debug.Log("maxArea:" + maxArea + '\n' + ' ' + x1 + ' ' + y1 + ", " + x2 + ' ' + y2);
        /*
        string fileName = "debugIzlaz2.txt";

        if (File.Exists(fileName))
        {
            Debug.Log(fileName + " already exists.");
            //return 0;
        }
        var sr = File.CreateText(fileName);


        for (int i = velicina_mape - 1; i >= 0; i--)
        {
            string buffer = "";
            for (int j = 0; j < velicina_mape; j++)
            {
                
                buffer += lijevo[i, j].ToString("  00");

            }
            sr.WriteLine(buffer);

        }
        sr.Close();


        fileName = "debugIzlaz1.txt";

        if (File.Exists(fileName))
        {
            Debug.Log(fileName + " already exists.");
            //return;
        }
        sr = File.CreateText(fileName);


        for (int i = velicina_mape - 1; i >= 0; i--)
        {
            string buffer = "";
            for (int j = 0; j < velicina_mape; j++)
            {

                buffer += desno[i, j].ToString("  00");

            }
            sr.WriteLine(buffer);

        }
        sr.Close();
        */
        return 0;
    }

    private void izracunajKutoveZgrada()
    {
        for (int i = 0; i < zgrade.Count; i++)
        {
            zgrade[i].x1 = (zgrade[i].x1 - velicina_mape / 2) * velicina_blokova / 2;
        }
    }
}

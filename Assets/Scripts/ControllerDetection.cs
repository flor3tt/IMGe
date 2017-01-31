using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControllerDetection : MonoBehaviour {

    public SerialPort[] streams = new SerialPort[9];
    public Text UItext;

    string receivedData;
    int digitalData;
    int bitmaskAllButtons = System.Convert.ToInt32("000fc0", 16);

    public string spieler1;
    public string spieler2;
    int anzahlSpieler = 0;
    int spieler1Nr = -1;

    // Use this for initialization
    void Start ()
    {
        spieler1 = null;
        spieler2 = null;

        //Tests COM Ports 1-9 for connected devices and stores them in their
        for(int i = 1; i <= 9; i++)
        {
            try
            {
                streams[i - 1] = new SerialPort("COM" + i, 115200);
                streams[i - 1].Open();
                streams[i - 1].ReadTimeout = 1;
            }
            catch(System.IO.IOException e)
            {
                streams[i - 1] = null;
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if(streams[i] != null)
            {
                Debug.Log("COM Port " + (i + 1) + " is open!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (spieler1 == null)
        {
            UItext.text = "Player 1 press any Controller Button to continue!";
        }
        else
        {
            UItext.text = "Hold controller upside down\n"
                + "Steer by tilting\n"
                + "Shoot with Button 5\n"
                + "Aceelerate or Break with Slider 4\n"
                + "Have Fun!";

            streams[spieler1Nr].Write("1");
            try
            {
                receivedData = streams[spieler1Nr].ReadLine();
                digitalData = System.Convert.ToInt32(receivedData, 16);
                if ((digitalData & bitmaskAllButtons) != 0)
                {
                    StartCoroutine(startLevel());
                }
            }
            catch (TimeoutException e)
            {

            }
        }

        /**
        else if (spieler2 == null)
        {
            UItext.text = "Player 2 press any Controller Button to continue!\nOr Press Space to start Single Player mode!";
        }
        else
        {
            UItext.text = "Press Space to start 2 Player mode!";
        }
        */


        for (int i = 1; i < 9; i++)
        {
            if (streams[i] != null)
            {
                streams[i].Write("1");
                try
                {
                    receivedData = streams[i].ReadLine();
                    digitalData = System.Convert.ToInt32(receivedData, 16);
                    if ((digitalData & bitmaskAllButtons) != 0)
                    {
                        if (spieler1 == null)
                        {
                            spieler1 = "COM" + (i + 1);
                            spieler1Nr = i;
                            Debug.Log("Spieler 1: COM" + (i + 1));
                            anzahlSpieler++;
                        }
                        else
                        {
                            if (spieler2 == null)
                            {
                                spieler2 = "COM" + (i + 1);
                                if (spieler2.Equals(spieler1))
                                {
                                    spieler2 = null;
                                }
                                else
                                {
                                    Debug.Log("Spieler 2: COM" + (i + 1));
                                    anzahlSpieler++;
                                }
                            }
                        }
                    }
                }
                catch(TimeoutException e)
                {

                }
            }
            receivedData = "";
        }
        

        if (anzahlSpieler > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            foreach(SerialPort s in streams)
            {
                if (s != null)
                    s.Close();
            }
            UnityEngine.Object.DontDestroyOnLoad(this);

            SceneManager.LoadScene("Level1");
        }
    


    }

    public IEnumerator startLevel()
    {
        yield return new WaitForSeconds(10);

        foreach (SerialPort s in streams)
        {
            if (s != null)
                s.Close();
        }
        UnityEngine.Object.DontDestroyOnLoad(this);

        SceneManager.LoadScene("Level1");
    }
        
}

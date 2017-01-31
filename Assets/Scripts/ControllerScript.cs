using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ControllerScript : MonoBehaviour
{
    [SerializeField]
    private string comPort = "COM3";
    SerialPort stream;
    string receivedData = "EMPTY";
    int digitalData;
    string[] analogData;
    string[] accelData;

    //Button Bitmasks
    int bitmaskB1 = System.Convert.ToInt32("000040", 16);
    int bitmaskB2 = System.Convert.ToInt32("000080", 16);
    int bitmaskB3 = System.Convert.ToInt32("000100", 16);
    int bitmaskB4 = System.Convert.ToInt32("000200", 16);
    int bitmaskB5 = System.Convert.ToInt32("000400", 16);
    int bitmaskB6 = System.Convert.ToInt32("000800", 16);

    //ButtonPressedValues
    bool buttonPressed1;
    bool buttonPressed2;
    bool buttonPressed3;
    bool buttonPressed4;
    bool buttonPressed5;
    bool buttonPressed6;

    bool tmpVal;

    //ButtonCooldowns
    bool buttonCooldown1;
    bool buttonCooldown2;
    bool buttonCooldown3;
    bool buttonCooldown4;
    bool buttonCooldown5;
    bool buttonCooldown6;

    //Normalized Slider-Values
    float slider1;
    float slider2;
    float rotKnob1;
    float rotKnob2;
    float slider1smooth;
    float slider2smooth;
    float rotKnob1smooth;
    float rotKnob2smooth;

    //Accelerometer Normalized Values
    float accelX;
    float accelY;
    float accelZ;
    float accelXsmooth;
    float accelYsmooth;
    float accelZsmooth;

    //Motor control
    int motorRPM;

    //LED control
    bool LED1;
    bool LED2;
    bool LED3;
    bool LED4;

    // Use this for initialization
    void Start()
    {
        ControllerDetection controllerDetection = GameObject.Find("Playermanager").GetComponent<ControllerDetection>();
        comPort = controllerDetection.spieler1;
        //comPort = "COM3";

        Destroy(controllerDetection.gameObject);

        stream = new SerialPort(comPort, 115200);
        stream.Open();//Open the serial Stream
        Debug.Log("Serial Port was opened");
    }

    //Called in fixed Time Interval
    void FixedUpdate()
    {
        //Smoothing analog Input
        slider1smooth = Mathf.Lerp(slider1, slider1smooth, 0.9f);
        slider2smooth = Mathf.Lerp(slider2, slider2smooth, 0.9f);
        rotKnob1smooth = Mathf.Lerp(rotKnob1, rotKnob1smooth, 0.9f);
        rotKnob2smooth = Mathf.Lerp(rotKnob2, rotKnob2smooth, 0.9f);

        accelXsmooth = Mathf.Lerp(accelX, accelXsmooth, 0.9f);
        accelYsmooth = Mathf.Lerp(accelY, accelYsmooth, 0.9f);
        accelZsmooth = Mathf.Lerp(accelZ, accelZsmooth, 0.9f);
    }

    // Update is called once per frame
    void Update()
    {
        //Buttons
        stream.Write("1");
        receivedData = stream.ReadLine();
        digitalData = System.Convert.ToInt32(receivedData, 16);


        if (!buttonCooldown1)
        {
            tmpVal = buttonPressed1;
            buttonPressed1 = (digitalData & bitmaskB1) != 0;
            if (buttonPressed1 != tmpVal)
            {
                StartCoroutine(buttonCooldown(1));
            }

        }
        if (!buttonCooldown2)
        {
            tmpVal = buttonPressed2;
            buttonPressed2 = (digitalData & bitmaskB2) != 0;
            if (buttonPressed2 != tmpVal)
            {
                StartCoroutine(buttonCooldown(2));
            }
        }
        if (!buttonCooldown3)
        {
            tmpVal = buttonPressed3;
            buttonPressed3 = (digitalData & bitmaskB3) != 0;
            if (buttonPressed3 != tmpVal)
            {
                StartCoroutine(buttonCooldown(3));
            }
        }
        if (!buttonCooldown4)
        {
            tmpVal = buttonPressed4;
            buttonPressed4 = (digitalData & bitmaskB4) != 0;
            if (buttonPressed4 != tmpVal)
            {
                StartCoroutine(buttonCooldown(4));
            }
        }
        if (!buttonCooldown5)
        {
            tmpVal = buttonPressed5;
            buttonPressed5 = (digitalData & bitmaskB5) != 0;
            if (buttonPressed5 != tmpVal)
            {
                StartCoroutine(buttonCooldown(5));
            }
        }
        if (!buttonCooldown6)
        {
            tmpVal = buttonPressed6;
            buttonPressed6 = (digitalData & bitmaskB6) != 0;
            if (buttonPressed6 != tmpVal)
            {
                StartCoroutine(buttonCooldown(6));
            }
        }

        //Sliders/Knobs
        stream.Write("4");
        receivedData = stream.ReadLine();
        analogData = receivedData.Split(' ');

        slider1 = System.Convert.ToInt32(analogData[4], 16) / 8 / 511f;
        slider2 = System.Convert.ToInt32(analogData[3], 16) / 8 / 511f;
        rotKnob1 = System.Convert.ToInt32(analogData[2], 16) / 8 / 511f;
        rotKnob2 = System.Convert.ToInt32(analogData[1], 16) / 8 / 511f;

        //Accelerometer
        stream.Write("a");
        receivedData = stream.ReadLine();
        accelData = receivedData.Split(' ');
        accelX = System.Convert.ToUInt16(accelData[1], 16);
        accelY = System.Convert.ToInt16(accelData[2], 16);
        accelZ = System.Convert.ToInt16(accelData[3], 16);
        if (accelX >= 128)
            accelX -= 256;
        if (accelY >= 128)
            accelY -= 256;
        if (accelZ >= 128)
            accelZ -= 256;
        accelX /= 128f;
        accelY /= 128f;
        accelZ /= 128f;
    }

    //Coroutine for Button Cooldown
    IEnumerator buttonCooldown(int buttonNr)
    {
        switch(buttonNr)
        {
            case 1:
                buttonCooldown1 = true;
                yield return new WaitForSeconds(0.1f);
                buttonCooldown1 = false;
                break;
            case 2:
                buttonCooldown2 = true;
                yield return new WaitForSeconds(0.1f);
                buttonCooldown2 = false;
                break;
            case 3:
                buttonCooldown3 = true;
                yield return new WaitForSeconds(0.1f);
                buttonCooldown3 = false;
                break;
            case 4:
                buttonCooldown4 = true;
                yield return new WaitForSeconds(0.1f);
                buttonCooldown4 = false;
                break;
            case 5:
                buttonCooldown5 = true;
                yield return new WaitForSeconds(0.1f);
                buttonCooldown5 = false;
                break;
            case 6:
                buttonCooldown6 = true;
                yield return new WaitForSeconds(0.1f);
                buttonCooldown6 = false;
                break;
        }
        yield return null;
    }

    //Motor RPM setzen
    public void motor(int rpm)
    {
        if (rpm >= 0 && rpm <= 1000)
        {
            if (System.Math.Abs(rpm - motorRPM) > 50)
            {
                motorRPM = rpm;
                stream.Write("m " + rpm + "\r\n");
                stream.ReadLine();
            }
        }
    }

    //LEDs umschalten
    public void LED(int nr, bool on)
    {
        switch (nr)
        {
            case 1:
                if (!LED1 == on)
                {
                    LED1 = on;
                    stream.Write("l 0 2\r\n");
                    stream.ReadLine();
                }
                break;
            case 2:
                if (!LED2 == on)
                {
                    LED2 = on;
                    stream.Write("l 1 2\r\n");
                    stream.ReadLine();
                }
                break;
            case 3:
                if (!LED3 == on)
                {
                    LED3 = on;
                    stream.Write("l 2 2\r\n");
                    stream.ReadLine();
                }
                break;
            case 4:
                if (!LED4 == on)
                {
                    LED4 = on;
                    stream.Write("l 3 2\r\n");
                    stream.ReadLine();
                }
                break;
        }
    }

    //Getter
    public bool getButton1()
    {
        return buttonPressed1 ;
    }

    public bool getButton2()
    {
        return buttonPressed2;
    }

    public bool getButton3()
    {
        return buttonPressed3;
    }

    public bool getButton4()
    {
        return buttonPressed4;
    }

    public bool getButton5()
    {
        return buttonPressed5;
    }

    public bool getButton6()
    {
        return buttonPressed6;
    }

    public float getSlider1()
    {
        return slider1;
    }

    public float getSlider2()
    {
        return slider2;
    }

    public float getRotKnob1()
    {
        return rotKnob1;
    }

    public float getRotKnob2()
    {
        return rotKnob2;
    }

    public float getAccelX()
    {
        return accelX;
    }

    public float getAccelY()
    {
        return accelY;
    }

    public float getAccelZ()
    {
        return accelZ;
    }

    //smoothed Values
    public float getSlider1smooth()
    {
        return slider1smooth;
    }

    public float getSlider2smooth()
    {
        return slider2smooth;
    }

    public float getRotKnob1smooth()
    {
        return rotKnob1smooth;
    }

    public float getRotKnob2smooth()
    {
        return rotKnob2smooth;
    }

    public float getAccelXsmooth()
    {
        return accelXsmooth;
    }

    public float getAccelYsmooth()
    {
        return accelYsmooth;
    }

    public float getAccelZsmooth()
    {
        return accelZsmooth;
    }

    //Shutdown Methode
    public void OnApplicationQuit()
    {
        stream.Write("l 0 0\r\n");
        stream.ReadLine();
        stream.Write("l 1 0\r\n");
        stream.ReadLine();
        stream.Write("l 2 0\r\n");
        stream.ReadLine();
        stream.Write("l 3 0\r\n");
        stream.ReadLine();
        stream.Write("m 0\r\n");
        stream.ReadLine();
    }
}

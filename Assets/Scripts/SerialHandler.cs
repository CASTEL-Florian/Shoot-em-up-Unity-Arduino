using System;
using System.IO.Ports;
using UnityEngine;
using System.Globalization;

public class SerialHandler : MonoBehaviour
{
    
    private SerialPort _serial;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM4";
    [SerializeField] private int baudrate = 115200;

    [SerializeField] private CharacterMovement movement;
    [SerializeField] private PlayerArduinoInput player;

    private float[] angles = new float[3];

    // Start is called before the first frame update
    void Start()
    {
        _serial = new SerialPort(serialPort,baudrate);
        // Guarantee that the newline is common across environments.
        _serial.NewLine = "\n";
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();

    }

    // Update is called once per frame
    void Update()
    {
        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        if (_serial.BytesToRead <= 0) return;
        
        var message = _serial.ReadLine();
        
        // Arduino sends "\r\n" with println, ReadLine() removes Environment.NewLine which will not be 
        // enough on Linux/MacOS.
        if (Environment.NewLine == "\n")
        {
            message = message.Trim('\r');
        }
        Debug.Log(message);
        string[] messageSplit = message.Split(";");

        for (int i = 0; i < 3; i++)
        {
            print("try parsing : " + messageSplit[i]);
            angles[i] = float.Parse(messageSplit[i], NumberStyles.Float, CultureInfo.InvariantCulture);
            print(i.ToString() + ":" + angles[i].ToString());
        }

        Vector2 speedVector = player.SetMove(new Vector3(angles[0], angles[1], angles[2]));
        Send(speedVector.x.ToString() + ";"+ speedVector.y.ToString());
    }

    public void SendDashMessage()
    {
        Send("Dash!");
    }

    public void Send(string message)
    {
        _serial.WriteLine(message);
    }
    
    private void OnDestroy()
    {
        _serial.Close();
    }
}

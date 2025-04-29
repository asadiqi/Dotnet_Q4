using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace MyApp.Service;
public partial class DeviceOrientationService
{

    // quand utilisateur se connecte et que il va dans la page de AddProduct et que le scanner n'est pas branché il doit voir un pop up 
    SerialPort? mySerialPort;
    string? portDetected = null;
    public partial void OpenPort()
    {
        if (mySerialPort != null)
        {
            try
            {
                if (mySerialPort.IsOpen) mySerialPort.Close();
                mySerialPort.Dispose();
            }
            catch (Exception ex)
            {
                // Display error message in English for closing the port
                Shell.Current.DisplayAlert("❌ Error!", $"Error while closing the port: {ex.Message}", "OK");
            }
            finally
            {
                mySerialPort = null;
            }
        }
        else
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");

            bool portFound = false;
            foreach (ManagementObject queryObj in searcher.Get())
            {
                string id = queryObj["PNPDeviceID"]?.ToString() ?? "";
                string nom = queryObj["Name"]?.ToString() ?? "";

                if (id.Contains("PID_7523"))
                {
                    int debut = nom.LastIndexOf("COM");
                    int fin = nom.LastIndexOf(")");

                    if (debut != -1 && fin != -1)
                    {
                        portDetected = nom.Substring(debut, fin - debut);
                        portFound = true;
                        break;
                    }
                }
            }

            if (portFound)
            {
                mySerialPort = new SerialPort
                {
                    BaudRate = 9600,
                    PortName = portDetected,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    ReadTimeout = 10000,
                    WriteTimeout = 10000
                };

                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataHandler);

                try
                {
                    mySerialPort.Open();
                }
                catch (Exception ex)
                {
                    Shell.Current.DisplayAlert("❌ Error!", $"Error while opening the serial port: {ex.Message}", "OK");
                }
            }
            else
            {
                Shell.Current.DisplayAlert("❌ Error!", "No scanning device detected. Please check the scanner connection.", "OK");
            }
        }
    }

    public partial void ClosePort()
    {
        if (mySerialPort != null && mySerialPort.IsOpen)
        {
            try
            {
                // Fermer proprement le port
                mySerialPort.Close();
                mySerialPort.Dispose();

                // Informer l'utilisateur que le port a été fermé avec succès
                Shell.Current.DisplayAlert("Success", "The port was successfully closed.", "OK");
            }
            catch (Exception ex)
            {
                // En cas d'erreur lors de la fermeture du port, afficher un message d'erreur
                Shell.Current.DisplayAlert("❌ Error!", $"Error while closing the port: {ex.Message}", "OK");
            }
            finally
            {
                // S'assurer que le port est bien vidé et mis à null
                mySerialPort = null;
            }
        }
       
    }


    private void DataHandler(object sender, EventArgs arg)
    {
        SerialPort sp = (SerialPort)sender;

        SerialBuffer.Enqueue(sp.ReadExisting());
    }
}

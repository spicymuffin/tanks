using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net.NetworkInformation;

public class CreateGameScreen : MonoBehaviour
{
    public static CreateGameScreen instance;
    const string SECRET_MESSAGE = "gay";
    public GameObject WaitingRoomPanel;
    public GameObject CreateGamePanel;
    public List<Client> connected = new List<Client>() { null, null, null, null };
    public List<GameObject> connectedPanels = new List<GameObject>() { null, null, null, null };
    public float refreshRate = 0.5f;
    public GameObject PlayerPanelList;
    public GameObject PlayerPanel;
    public Button Start;
    public TMP_InputField nameField;
    public TextMeshProUGUI ipField;
    public int playerCount = 0;


    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    string message;
    UdpClient udpClient;
    byte[] msg;
    Coroutine refreshCoroutine;
    public void CreateGame()
    {
        StartServer();
        WaitingRoomPanel.SetActive(true);
        CreateGamePanel.SetActive(false);
        message = SECRET_MESSAGE + ';';
        string name = nameField.text;
        if(nameField.text == "")
        {
            name = NamesLoader.instance.GetRandomString(NamesLoader.instance.sess_names);
        }
        string ipAddress2brdcst = GetLocalIPAddress();
        message += name;
        message += ';';
        message += ipAddress2brdcst;
        message += ';';
        message += DateTime.Now.ToString("HH:mm:ss");
        message += ';';
        message += playerCount;
        Debug.Log(message);

        //socket stuff
        udpClient = new UdpClient();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Parse(ipAddress2brdcst), 46969));
        refreshCoroutine = StartCoroutine(UpdateConnectedPlayers());
    }
    public void ResetWaitScreen()
    {
        WaitingRoomPanel.SetActive(false);
        CreateGamePanel.SetActive(true);
        Start.interactable = false;
        ServerSend.DisconnectAll();
        GameManager.instance.ResetServer();
        StopCoroutine(refreshCoroutine);
        udpClient.Close();
        nameField.text = "";
        for (int i = 0; i < connectedPanels.Count; i++)
        {
            Destroy(connectedPanels[i]);
        }
        connectedPanels = new List<GameObject>() { null, null, null, null };
        connected = new List<Client>() { null, null, null, null };
        playerCount = 0;
    }
    public void ResetCreateScreen()
    {
        if(refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
        }
        nameField.text = "";
    }
    public void StartServer()
    {
        Server.Start(4, 26950);
    }
    private IEnumerator UpdateConnectedPlayers()
    {
        while (true)
        {
            message = message.Remove(message.Length - 1);
            message += playerCount.ToString();
            msg = Encoding.ASCII.GetBytes(message); 
            //Debug.Log("sending packet");
            udpClient.Send(msg, msg.Length, "255.255.255.255", 46969);
            int connectedCount = 0;
            foreach (Client _client in Server.clients.Values)
            {
                //Debug.Log(_client.id - 1);
                if (_client.connected && !connected.Contains(_client))
                {
                    connected[_client.id - 1] = _client;
                    GameObject panel = Instantiate(PlayerPanel, PlayerPanelList.transform);
                    connectedPanels[_client.id - 1] = panel;
                    TextMeshProUGUI usrnm = panel.GetComponentInChildren<TextMeshProUGUI>();
                    usrnm.text = _client.username;
                    playerCount++;
                }
                if (!_client.connected && connected.Contains(_client))
                {
                    connected[_client.id - 1] = null;
                    Destroy(connectedPanels[_client.id - 1]);
                    connectedPanels[_client.id - 1] = null;
                    playerCount--;
                }
                if (_client.connected)
                {
                    connectedCount++;
                }
            }
            if (connectedCount >= 1)
            {
                Start.interactable = true;
            }
            else
            {
                Start.interactable = false;
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
    public void StartButton()
    {
        int connectedCount = 0;
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.connected)
            {
                connectedCount++;
            }
        }
        FindObjectOfType<MatchMaker>().LoadLevels(connectedCount);
        MainMenu.instance.MoveToLevelSelecting();
        StopCoroutine(refreshCoroutine);
    }
    public string GetLocalIPAddress()
    {
        string ip = "";
        foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (f.OperationalStatus == OperationalStatus.Up)
            {
                IPInterfaceProperties ipInterface = f.GetIPProperties();
                if (ipInterface.GatewayAddresses.Count > 0)
                {
                    foreach (UnicastIPAddressInformation unicastAddress in ipInterface.UnicastAddresses)
                    {
                        if ((unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) && (unicastAddress.IPv4Mask.ToString() != "0.0.0.0"))
                        {
                            ip = unicastAddress.Address.ToString();
                            break;

                        }
                    }
                }
            }
        }
        
        if (ip != "")
        {
            ipField.text = ip;
            return ip;
        }

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var cip in host.AddressList)
        {
            if (cip.AddressFamily == AddressFamily.InterNetwork)
            {
                ip = cip.ToString();
            }
        }

        if (ip != "")
        {
            ipField.text = ip;
            return ip;
        }

        ipField.text = "error, please report to the retarded dev";
        return "";
    }
}

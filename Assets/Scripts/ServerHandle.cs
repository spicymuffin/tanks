using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        if (_username == "")
        {
            if (NamesLoader.instance != null)
            {
                _username = NamesLoader.instance.GetRandomString(NamesLoader.instance.player_names);
            }
            else
            {
                _username = "dummy";
            }
        }
        Server.clients[_fromClient].SetUsername(_username);
        Server.clients[_fromClient].connected = true;
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        Player.Input input = new Player.Input();
        input.sprint = _packet.ReadBool();
        input.use = _packet.ReadBool();
        input.MH = _packet.ReadFloat();
        input.MV = _packet.ReadFloat();
        input.FH = _packet.ReadFloat();
        input.FV = _packet.ReadFloat();
        if (Server.clients[_fromClient].player != null)
        {
            Server.clients[_fromClient].player.SetInput(input);
        }
    }
}

using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Server;

class GameServer
{
    private int port;
    private int readBufferSize = 160;

    public void SetPort(int num)
    {
        this.port = num;
    }
    public void InitServer()
    {

        InitConnection();
    }

    private Player InitPlayersInfo()
    {
        PlayerList playerList = new PlayerList();
        playerList.Init();

    }

    private void InitConnection()
    {
       Console.WriteLine("Initializing the server...");
        IPAddress localAddress = IPAddress.Any;
        Console.WriteLine("Server will run at " + localAddress + this.port);
        IPEndPoint serverEndPoint = new(localAddress, this.port);
        TcpListener mainListener = new TcpListener(serverEndPoint);
        for (;;)
        {
            mainListener.Start();
            TcpClient client= mainListener.AcceptTcpClient();
            //Thread newThread = new Thread(HandleNewConnection);
            //newThread.Start(clientConnection);
            Task newTask = Task.Run(() => HandleNewConnection(client));
        }
    }

    private void HandleNewConnection(TcpClient client)
    {
        //SslStream sslStream = PrepareSslStream(client);
        //self-signed certificate can't be used here. Use RSA encryption as substitution.

        // There are totally 3 statements in the login section. The first byte used to express the statement.
        // 01: User clicked Login, server waiting for ID and password and check if correct.
        // 02: User clicked Register. server waiting for ID and password and check if valid.
        RSACryptoServiceProvider rsa = PrepareRSA(client);
        byte[] readMsg = ReadMessage(client);
        (string userID, string password) userInfo = GetUserInfo(readMsg, rsa);
        if (readMsg[0] == 0x01) // login.
        {

        }

    }

    private RSACryptoServiceProvider PrepareRSA(TcpClient client)
    {
        //just use to XML string can transmit the RSA info.
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        string publicRSA = rsa.ToXmlString(false);
        NetworkStream stream = client.GetStream();
        byte[] publicKeyInBytes = Encoding.ASCII.GetBytes(publicRSA);
        stream.Write(publicKeyInBytes, 0, publicKeyInBytes.Length);
        return rsa;
    }
    //private SslStream PrepareSslStream(TcpClient client)
    //{
    //    SslStream sslStream = new SslStream(client.GetStream(), false);
    //    string certFile = "encryption/public.crt";
    //    X509Certificate serverCertificate = X509Certificate.CreateFromCertFile(certFile);
    //    sslStream.AuthenticateAsServer(serverCertificate);
    //    return sslStream;
    //}



    private byte[] ReadMessage(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] readBuffer = new byte[readBufferSize];
        stream.Read(readBuffer, 0, readBufferSize);
        return readBuffer;
    }

    private (string, string) GetUserInfo(byte[] rawMsg, RSACryptoServiceProvider rsa)
    {
        // the first byte is login or register.
        // msg[1] is 0x0, msg[2] to msg[21] is username. msg[22] is 0x0, 128 bytes from msg[23] is password. Rest bytes is 
        
        byte[] usernameInBytes = new byte[20];
        Array.Copy(rawMsg, 2, usernameInBytes, 0, 20);
        string username = Encoding.ASCII.GetString(usernameInBytes);
        byte[] passwordInBytes = new byte[128];
        Array.Copy(rawMsg, 23, passwordInBytes, 0, 128);
        rsa.Decrypt(passwordInBytes, true);
        string password = Encoding.ASCII.GetString(passwordInBytes);
        return (username, password);
    }
}
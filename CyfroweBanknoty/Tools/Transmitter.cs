using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CyfroweBanknoty.Tools
{
    public class Transmitter
    {
        // implement methods to send over objects between users
        NetworkStream stream;
        BinaryFormatter bf;

        public Transmitter(Socket socket)
        {
            bf = new BinaryFormatter();
            stream = new NetworkStream(socket);
        }

        public bool sendObject(Object obj)
        {
            // Serialize(Stream ..., Object ...) 
            // converts object (like Banknote or Series) to bits,
            // and sends it over through network.
            try
            {
                bf.Serialize(stream, obj);
                return true;
            }
            catch (SerializationException e)
            {
                return false;
            }
        }

        public Object receiveObject()
        {
            // Deserialize(Stream ...)
            // receives object from stream
            // it has to be casted manually elsewhere!
            try
            {
                return bf.Deserialize(stream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Exception thrown during receiving object: " + e.Message);
                throw;
            }
        }
    }
}

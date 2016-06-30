using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Threading;

namespace RemoteChat
{
     [Serializable]
    public class NewMessageEventArgs
    {
         public NewMessageEventArgs(string m)
        {
            message = m;
        }
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
        private string message;
    }


     public delegate void NewMessageEvent(object sender, NewMessageEventArgs e);
     public delegate void RefDel(string message);

    [Serializable]
    public class RemoteObject : MarshalByRefObject
    {
        public RemoteObject()
        {
            for (int i = 0; i <= 14; i++)
            {
                messages[i] = "message {0}" + i;
            }
            Console.WriteLine("remobj constructed");
            потокЗаписи = new Thread(new ThreadStart(SaveFile));
        }
        private static string[] messages = new string[15];
        public event NewMessageEvent NewMessage;
        public FileStream localFile;
        public FileStream принятыйФайл;
        Thread потокЗаписи;
        public string filename;
        public void SendMessage(string msg)
        {
            Console.WriteLine(msg);
            NewMessageEventArgs e = new NewMessageEventArgs(msg);
            for (int i = 14; i > 0; i--)
            {
                messages[i] = messages[i - 1];
            }
            messages[0] = msg; 
            if (NewMessage != null)
            {
                NewMessage(this, e);
                Console.WriteLine("Event called");
            }
            else
            {
                return;
            }
        }
        public string[] GetMessages()
        {
            return messages;
        }
        public void SendFile(FileStream file, string _filename)
        {
            принятыйФайл = file;
            localFile = File.Create(_filename);
            filename = _filename;
            потокЗаписи.Start();
           



        }
        void SaveFile()
        {
            Console.WriteLine("thread started");
            byte[] bufer = new byte[1024];
            int b = принятыйФайл.Read(bufer, 0, bufer.Length);
            while (b != 0)
            {
                localFile.Write(bufer, 0, b);
                b = принятыйФайл.Read(bufer, 0, bufer.Length);
            }
            Console.WriteLine("file saved");
            localFile.Close();
            string message = "file: " + filename;
            NewMessageEventArgs e = new NewMessageEventArgs(message);
            if (NewMessage != null)
            {
                NewMessage(this, e);
                Console.WriteLine("Event called");
            }
            else
            {
                return;
            }

        }
        public FileStream GetFile(string _filename)
        {
            return File.OpenRead(_filename);
        }
    }

}

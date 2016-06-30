using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace RemoteChat
{
    public class EventSink: MarshalByRefObject
    {
  
        public RefDel refmsg;
        public FileStream localFile;
        public FileStream принятыйФайл;
        public RemoteObject obj;
        Thread потокЗаписи;
        public string filename;
        public EventSink(RefDel _refmsg, RemoteObject _obj)
        {
            this.refmsg = _refmsg;
            obj = _obj;
            потокЗаписи = new Thread(new ThreadStart(SaveFile));
            MessageBox.Show("sink constructed");

        }
        void RefreshMsgs(string msge)
        {
            MessageBox.Show(msge);

            //refmsg(msge);
        }

        public void NewMessageHandler(object sender, NewMessageEventArgs e)
        {
            MessageBox.Show("Event was called");
            //textBox.Text += e.Message;
            if (e.Message.Contains("file: "))
            {
                int a = e.Message.LastIndexOf("file: ") + 1;
                filename = e.Message.Remove(0, 6);
                принятыйФайл = obj.GetFile(filename);
                localFile = File.Create(filename);
                потокЗаписи.Start();
            }
            else
            {
                RefreshMsgs(e.Message);
            }
        }
        public void SaveFile()
        {
            string message = "Принять файл?";
            string caption = "Вхоядщее соединение";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                byte[] bufer = new byte[1024];
                int b = принятыйФайл.Read(bufer, 0, bufer.Length);
                while (b != 0)
                {
                    localFile.Write(bufer, 0, b);
                    b = принятыйФайл.Read(bufer, 0, bufer.Length);
                }
                localFile.Close();
            }
        }
    }
}

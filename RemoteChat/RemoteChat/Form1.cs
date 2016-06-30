using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Activation;
using System.IO;

namespace RemoteChat
{

    public partial class Form1 : Form
    {
        EventSink sink;
        RemoteObject obj;
        public string initPath;
        public string srcPath;
        public string FileNameExtension;

        public Form1()
        {
            InitializeComponent();
            RemotingConfiguration.Configure("Client.exe.config", true);
            obj = (RemoteObject)Activator.GetObject(typeof(RemoteObject), "tcp://localhost:6791/RemoteServ/remoteObj");
            sink = new EventSink(new RefDel(this.RefForm), obj);
            obj.NewMessage += new NewMessageEvent(sink.NewMessageHandler);
            textBox1.Text = String.Join("\n", obj.GetMessages());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void RefForm(string message)
        {
            textBox1.Text += message;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            obj.SendMessage(textBox2.Text);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            initPath = Environment.CurrentDirectory;
            openFileDialog1.InitialDirectory = initPath;
            openFileDialog1.FileName = "*.*";
            openFileDialog1.Filter = "All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openFileDialog1.OpenFile() != null)
                    {
                        srcPath = openFileDialog1.FileName;
                        FileNameExtension = System.IO.Path.GetFileName(srcPath);
                        textBox3.Text = srcPath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Файл не может быть прочитан" + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            obj.SendFile(File.OpenRead(srcPath), Path.GetFileName(srcPath));
        }
    }
}

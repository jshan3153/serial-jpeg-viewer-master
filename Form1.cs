using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.IO.Ports;

namespace SimpleSerial
{
    public partial class Form1 : Form
    {
        // Add this variable 
        string RxString;
        int is_receive_jpg;
        int fps;
        byte[] buffer = new byte[1024*512];
        public Form1()
        {
            is_receive_jpg = 0;
            fps = 0;
            InitializeComponent();
            string[] PortNames = SerialPort.GetPortNames();  // 포트 검색.

            foreach (string portnumber in PortNames)
            {
                comboBox1.Items.Add(portnumber);          // 검색한 포트를 콤보박스에 입력. 
            }

        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
                textBox1.ReadOnly = false;

                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                //serialPort1.BaudRate = int.Parse(comboBox2.SelectedItem.ToString());          //콤보 박스에서 Baud Rate 선택.
                serialPort1.BaudRate = 115200;
                serialPort1.Parity = Parity.None;
                serialPort1.Open();
            }
            else
            {
                textBox1.Text = "연결되어 있습니다";

            }



        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                textBox1.ReadOnly = true;
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the port is closed, don't try to send a character.
            if (!serialPort1.IsOpen) return;

            // If the port is Open, declare a char[] array with one element.
            char[] buff = new char[1];

            // Load element 0 with the key character.
            buff[0] = e.KeyChar;

            // Send the one character buffer.
            serialPort1.Write(buff, 0, 1);

            // Set the KeyPress event as handled so the character won't
            // display locally. If you want it to display, omit the next line.
            e.Handled = true;
        }

        private void DisplayText(object sender, EventArgs e)
        {
            textBox1.AppendText(RxString+"\n");
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //if (is_receive_jpg != 1)
            //{
            //    RxString = serialPort1.ReadLine();
            //    if (RxString.Contains("jpg"))
            //    {
            //        is_receive_jpg = 1;
            //    }
            //    this.Invoke(new EventHandler(DisplayText));
            //}
            //else
            {//Now we will read the jpeg data
#if _DEBUG_
                //'\n'까지 시리얼로부터 받아서 string에 저장
                RxString = serialPort1.ReadLine();
                //Console.Write(RxString);
				//스트링에서 ,으로 잘라서 스트링 배열에 저장				
                string[] strbytes = RxString.Split(',');
				//리스트 바이트 선언
                List<byte> bytes1 = new List<byte>();
                //byte[] bytes = strbytes.Select(s => Convert.ToByte(s, 16)).ToArray();
				//스트링 배열의 갯수
                for (int i = 0; i < strbytes.Length; i++)
                {
                    if (strbytes[i] == "")
                    {
                        bytes1.Add(0);
                    }
                    else
                    {
                        try
                        {
                            bytes1.Add(Convert.ToByte(strbytes[i], 16));
                        }
                        catch (Exception ex)
                        {
                            bytes1.Add(0);
                        }
                    }

                }

                var ms = new MemoryStream(bytes1.ToArray());
                pictureBox1.Image = Image.FromStream(ms);

                is_receive_jpg = 0;
                fps++;
#endif
                //RxString = serialPort1.ReadLine();
                //byte[] bytes1 = Encoding.Default.GetBytes(RxString);
                //var ms = new MemoryStream(bytes1);
                //pictureBox1.Image = Image.FromStream(ms);

                int iRecSize = serialPort1.BytesToRead;

                if (iRecSize != 0) // 수신된 데이터의 수가 0이 아닐때만 처리하자
                {
                    //          Console.Write("rx:" + iRecSize.ToString());

                    byte[] buff = new byte[iRecSize];
                    try
                    {
                        serialPort1.Read(buff, 0, iRecSize);

                        bytes1.Add(buff.ToList());
                    }
                    catch
                    {
                    }
                }
                is_receive_jpg = 0;
                fps++;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = fps.ToString();
            fps = 0;
        }
    }
}
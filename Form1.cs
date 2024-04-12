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
        string RxString , str;
        int is_receive_jpg;
        int fps, pictureNum = 0;
        byte[] buffer = new byte[1024*512];
        List<byte> bytes2 = new List<byte>();
        int filesize = 0, filelength = 0;
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
            textBox1.AppendText(str+"\r\n");
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (is_receive_jpg == 0)
            {
                RxString = serialPort1.ReadLine();
                if (RxString.Contains("jpg"))
                {
                    is_receive_jpg = 3;
                }
                if (RxString.Contains("jpgF"))
                {
                    is_receive_jpg = 1;
                }
                str = RxString;
                this.Invoke(new EventHandler(DisplayText));
            }
            else if(is_receive_jpg == 1)
            {
                //var str = "  10FFxxx";
                string numericString = string.Empty;
                str = serialPort1.ReadLine();
                foreach (var c in str)
                {
                    // Check for numeric characters (hex in this case) or leading or trailing spaces.
                    //if ((c >= '0' && c <= '9') || (char.ToUpperInvariant(c) >= 'A' && char.ToUpperInvariant(c) <= 'F') || c == ' ')
                    //{
                    //    numericString = string.Concat(numericString, c.ToString());
                    //}
                    if ((c >= '0' && c <= '9') || c == ' ' || c == '-')
                    {
                        numericString = string.Concat(numericString, c);
                    }
                    else
                    {
                        break;
                    }
                }

                //if (int.TryParse(numericString, System.Globalization.NumberStyles.HexNumber, null, out int i))
                //{
                //    Console.WriteLine($"'{str}' --> '{numericString}' --> {i}");
                //}
                if (int.TryParse(numericString, out int j))
                {
                    Console.WriteLine($"'{str}' --> '{numericString}' --> {j}");
                    is_receive_jpg = 2;
                    filelength = j;
                }
                this.Invoke(new EventHandler(DisplayText));
            }
            else if(is_receive_jpg == 2 )
            {//Now we will read the jpeg data
                int iRecSize = serialPort1.BytesToRead;

                if (iRecSize != 0) // 수신된 데이터의 수가 0이 아닐때만 처리하자
                {
                    //          Console.Write("rx:" + iRecSize.ToString());

                    byte[] buff = new byte[iRecSize];
                    List<byte> bytes1 = new List<byte>();
                    
                    try
                    {
                        serialPort1.Read(buff, 0, iRecSize);

                        bytes1 = buff.ToList();
                        bytes2.AddRange(bytes1);
                        filesize += iRecSize;
                    }
                    catch
                    {
                    }
                }
                //is_receive_jpg = 0;
                fps++;
                Console.Write("fps<{0}>", fps);
                if (filesize >= filelength){
                    Console.Write("rx:\r\n");
                    var ms = new MemoryStream(bytes2.ToArray());
                    if (pictureNum == 0)
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                        pictureNum = 1;
                    }
                    else
                    {
                        pictureBox2.Image = Image.FromStream(ms);
                        pictureNum = 0;
                    }
                    ChangeOutlineColor(pictureNum, false);

                    is_receive_jpg = 0;
                    filesize = 0;
                    filelength = 0;
                    bytes2.Clear();
                }
            }
            else if(is_receive_jpg == 3)
            {
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
                if(pictureNum == 0)
                {
                    pictureBox1.Image = Image.FromStream(ms);
                    pictureNum = 1;
                }
                else
                {
                    pictureBox2.Image = Image.FromStream(ms);
                    pictureNum = 0;
                }
                

                is_receive_jpg = 0;
                fps++;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                pictureBox1.Image = null;
            }
            pictureNum = 0;
            // Set the outline color for pictureBox1
            ChangeOutlineColor(pictureNum, true);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                pictureBox2.Image = null;
            }
            pictureNum = 1;
            ChangeOutlineColor(pictureNum, true);
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

        private void ChangeOutlineColor(int imageNum, bool clear)
        {
            if (imageNum == 1)
            {
                ((ColoredPictureBox)pictureBox1).OutlineColor = Color.Red;
                ((ColoredPictureBox)pictureBox2).OutlineColor = Color.White;
            }
            else
            {
                ((ColoredPictureBox)pictureBox1).OutlineColor = Color.White;
                ((ColoredPictureBox)pictureBox2).OutlineColor = Color.Red;
            }

            if (clear)
            {
                ((ColoredPictureBox)pictureBox1).OutlineColor = Color.Green;
                ((ColoredPictureBox)pictureBox2).OutlineColor = Color.Green;
            }

            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke((MethodInvoker)delegate {
                    pictureBox1.Refresh();
                });
            }
            else
            {
                pictureBox1.Refresh();
            }
            if (pictureBox2.InvokeRequired)
            {
                pictureBox2.Invoke((MethodInvoker)delegate {
                    pictureBox2.Refresh();
                });
            }
            else
            {
                pictureBox2.Refresh();
            }
        }
    }
}
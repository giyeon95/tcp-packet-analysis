using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static ConsoleApp1.Program;

namespace ConsoleApp2
{
    public partial class Form1 : Form
    {
        ArrayList packet;
        Packet_Data struct_packet;
        JObject jObject;
        JObject addObject;

        JObject getObject;
        Random rnd;
        Form2 form2;
        Detail_Form detailForm;
        int i, max_i = 0;
        int cli1 = 0, cli2 = 0;

        public Form1()
        {
            InitializeComponent();

        }

        public Form1(ArrayList list)
        {
            InitializeComponent();
            packet = new ArrayList();
            packet = list;
            struct_packet = new Packet_Data();
            jObject = new JObject();
            addObject = new JObject();
            rnd = new Random();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
         
            String checkOverlap = "";
            String getMsg;
            String getId;
            String getType;

            gridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            gridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView1.RowCount = 1;
            dataGridView1.RowHeadersWidth = 50;
            gridView1.RowCount = 1;
            gridView2.RowCount = 1;

            foreach (Packet_Data a in packet)
            {
               if(a.Push && a.ACK)
               //if(!a.TextData.Equals("ACK") && !a.TextData.Equals("SYN") && !a.TextData.Equals("SYN/ACK") && !a.TextData.Equals("RST"))
                {
                    jObject = JObject.Parse(a.TextData);  
                    getMsg = jObject["msg"].ToString();
                    getId = jObject["id"].ToString();
                    getType = jObject["type"].ToString();
 
                                addObject.Add(new JProperty(i.ToString(), 
                                    new JObject(
                                        new JProperty("id", getId),
                                        new JProperty("msg", getMsg),
                                        new JProperty("type", getType)
                                    )));
                            i++;

                            checkOverlap = getId + getMsg;
                
                } else {
                    getType = "flags";
                    getMsg = "";
                    getId = "";

                    if (a.TextData.Equals("SYN")) getId = "SYN";
                    else if (a.TextData.Equals("SYN, ACK")) getId = "SYN, ACK";
                    else if (a.TextData.Equals("ACK")) getId = "ACK";
                    else if (a.TextData.Equals("RST, ACK")) getId = "RST, ACK";

                    if (i == 0)
                    {
                        addObject = new JObject((new JProperty(i.ToString(),
                        new JObject(
                            new JProperty("id", getId),
                            new JProperty("msg", getMsg),
                            new JProperty("type", getType)
                        ))));
                    }

                    else
                    {
                        addObject.Add(new JProperty(i.ToString(),
                                 new JObject(
                                     new JProperty("id", getId),
                                     new JProperty("msg", getMsg),
                                     new JProperty("type", getType)
                                 )));
                    }
                    i++;
                }
            }
            max_i = i;
            i = 0;
            chatTimer.Enabled = true;

            /**
             * 
             * 
             * "source Mac : "+a.destination_mac
             * " des Mac :" + a.source_mac
             * " type : "+a.type 
             * " source IP :" + a.source_ip 
             * " des IP :" + a.destination_ip
             * "total Length:" + a.total_length 
             * " TTL :" + a.TTL 
             * " Protocol :" + a.protocol 
             * "sourcePort:" + a.source_port
             * "desPort:" + a.destination_port 
             * "identification" + a.identification
             * "분할 하지않음 : "+a.dont_frag
             * " 남은 분할 존재 : "+a.more_frag
             * " 분할 위치 정보( offset) :" a.offset
             * " headlength=" + a.headerLength 
             * " flag Bit : " + a.check_flags  " <-> " + a.CWR + " " + a.ECN_Echo + " " + a.Urgent + " " + a.ACK + " " + a.Push + " " + a.Reset + " " + a.Syn + " " + a.Fin
             * " ip version:"+a.ip_v+" ip state : "+a.ip_header_Status);
             * " squenceNumber : " + a.Sequence_number 
             * " header CheckSum : " + a.header_check_sum 
             * "  Data : " + 
             * );
             Console.WriteLine("");
                **/
        }
        
        private void chatTimer_Tick(object sender, EventArgs e)
        {
            if(i< max_i) {
                
                string getString = addObject[i.ToString()].ToString();
                
                getObject = JObject.Parse(getString);
                //serverBox.Text += ((Packet_Data)packet[i]).source_ip+"\n";

                string clientName = getObject["id"].ToString();
                string clientMsg = getObject["msg"].ToString();
                string messageType = getObject["type"].ToString();
                if(messageType.Equals("flags"))
                {
                    if (clientName.Equals("RST, ACK"))
                    {
                        clientLeft.Text = "RST로 인한 초기화";

           
                        cl_label.Text = "NULL";
                        gridView1.Rows.Remove(gridView1.Rows[7]);
                        gridView1.Rows.Remove(gridView1.Rows[6]);
                        gridView1.Rows.Remove(gridView1.Rows[5]);
                        gridView1.Rows.Remove(gridView1.Rows[4]);
                        gridView1.Rows.Remove(gridView1.Rows[3]);
                        gridView1.Rows.Remove(gridView1.Rows[2]);
                        gridView1.Rows.Remove(gridView1.Rows[1]);
                        gridView1.Rows.Remove(gridView1.Rows[0]);
                      
                        cli1 = 0;
                    }
                    
                }
                if (((Packet_Data)packet[i]).Push && ((Packet_Data)packet[i]).ACK)
                {
                    if (messageType.Equals("server"))
                    {
                        alertTimer.Enabled = true;
                        form2 = new Form2(clientName, clientMsg);

                    }
                    else if (messageType.Equals("flags"))
                    {
                        if(clientName.Equals("RST, ACK"))
                        {
                         
                            clientLeft.Text = "RST로 인한 초기화";
                            for(int n = 0; n < cli1; n ++)
                            {
                                gridView1.Rows.Remove(gridView1.Rows[n]);
                       
                               
                            }
                            cli1 = 0;
                        }

                    }
                    else if (messageType.Equals("msg"))
                    {
                        //18이 시작 IP , Agent2, WANG , 23이 AGENT 1
                        string dst = ((Packet_Data)packet[i]).destination_ip.ToString();
                        // serverBox.Text += messageType + "\n";
                        //if (clientName.Equals("PROF_HANHOWANG"))
                        if (dst.Equals("192.168.0.18."))
                        {
                            gridView1.RowCount += 1;
                      
                            if (clientName.Equals("Agent2") || clientName.Equals("PROF_HANHOWANG")) {
                                //gridView1[2, cli1].Value = clientName + " : ";
                                gridView1[3, cli1].Value = clientMsg + "\n"; 
                                gridView1[3, cli1].Style.ForeColor = System.Drawing.Color.Blue;
                            } else
                            {
                                gridView1[0, cli1].Value = clientName;
                                gridView1[1, cli1].Value = clientMsg + "\n";
                            }
                            gridView1.FirstDisplayedCell = gridView1[0, cli1];

                            cli1++;

                            //cl_label2.Text = clientName;
                            cl_label.Text = "192.168.0.18";
                            clientLeft.Enabled = true;
                            clientLeft.Text += " " + clientName + " : " + clientMsg + "\n";
                       
                        }
                        //else if (clientName.Equals("Agent1"))
                        else if (dst.Equals("192.168.0.22."))
                        {
                       
                            //cl_label.Text = clientName;
                            cl_label2.Text = "192.168.0.22 <Server>";
                            serverBox.Enabled = true;
                            serverBox.Text += " " + clientName + " : " + clientMsg + "\n";
                        }

                        //else if (clientName.Equals("Agent2"))
                        else if (dst.Equals("192.168.0.23."))
                        {
                            gridView2.RowCount += 1;
                     
                            if (clientName.Equals("Agent1"))
                            {
                                //gridView1[2, cli1].Value = clientName + " : ";
                                gridView2[3, cli2].Value = clientMsg + "\n";
                                gridView2[3, cli2].Style.ForeColor = System.Drawing.Color.Blue;
                            }
                            else
                            {
                                gridView2[0, cli2].Value = clientName;
                                gridView2[1, cli2].Value = clientMsg + "\n";
                            }
                            gridView2.FirstDisplayedCell = gridView2[0, cli2];
                            cli2++;

                            //cl_label3.Text = clientName;
                            cl_label3.Text = "192.168.0.23";
                            clientRight.Enabled = true;
                            clientRight.Text += " " + clientName + " : " + clientMsg + "\n";
                        }
                    }
                }
                //serverBox.Text += getString.ToString();

                dataGridView1.RowCount += 1;
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();

                dataGridView1[0, i].Value = ((Packet_Data)packet[i]).source_ip;
                dataGridView1[1, i].Value = ((Packet_Data)packet[i]).destination_ip;
                dataGridView1[2, i].Value = ((Packet_Data)packet[i]).protocol;
                dataGridView1[3, i].Value = ((Packet_Data)packet[i]).total_length + 14;
                string flag_bit = "";

                if (((Packet_Data)packet[i]).CWR)
                    flag_bit += "CWR ";
                if (((Packet_Data)packet[i]).ECN_Echo)
                    flag_bit += "ECE ";
                if (((Packet_Data)packet[i]).Urgent)
                    flag_bit += "URG ";
                if (((Packet_Data)packet[i]).ACK)
                    flag_bit += "ACK ";
                if (((Packet_Data)packet[i]).Push)
                    flag_bit += "PSH ";
                if (((Packet_Data)packet[i]).Reset)
                    flag_bit += "RST ";
                if (((Packet_Data)packet[i]).Syn)
                    flag_bit += "SYN ";
                if (((Packet_Data)packet[i]).Fin)
                    flag_bit += "FIN ";

                dataGridView1[4, i].Value = ((Packet_Data)packet[i]).source_port + " -> " + ((Packet_Data)packet[i]).destination_port + "[ " +flag_bit + "]" +", WindowSize = "+ ((Packet_Data)packet[i]).windowSize + ", Length = "+(((Packet_Data)packet[i]).total_length + 14);
                dataGridView1.FirstDisplayedCell = dataGridView1[0, i];
                if(dataGridView1[0,i].Value.Equals("192.168.0.18.") || dataGridView1[1, i].Value.Equals("192.168.0.18."))
                {
                    dataGridView1[0, i].Style.BackColor = System.Drawing.Color.Pink;
                    dataGridView1[1, i].Style.BackColor = System.Drawing.Color.Pink;
                    dataGridView1[2, i].Style.BackColor = System.Drawing.Color.Pink;
                    dataGridView1[3, i].Style.BackColor = System.Drawing.Color.Pink;
                    dataGridView1[4, i].Style.BackColor = System.Drawing.Color.Pink;
                }
                if (dataGridView1[0, i].Value.Equals("192.168.0.23.") || dataGridView1[1, i].Value.Equals("192.168.0.23."))
                {
                    dataGridView1[0, i].Style.BackColor = System.Drawing.Color.LightGreen;
                    dataGridView1[1, i].Style.BackColor = System.Drawing.Color.LightGreen;
                    dataGridView1[2, i].Style.BackColor = System.Drawing.Color.LightGreen;
                    dataGridView1[3, i].Style.BackColor = System.Drawing.Color.LightGreen;
                    dataGridView1[4, i].Style.BackColor = System.Drawing.Color.LightGreen;
                }

                //clientLeft.Text += "Check dataGrid : " + ((Packet_Data)packet[i]).destination_ip + "\n\n";
                //dataGridView1[0,i].Selected=true;

                i++;
            }
            else
            {
                chatTimer.Enabled = false;
            }
            
                chatTimer.Interval = randomTimer();
           

        }

        private void alertTimer_Tick(object sender, EventArgs e)
        {
            alertTimer.Enabled = false;
            form2.Show();
         
            //form2.Close();


        }

        private void contentsClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            string getString = addObject[rowIndex.ToString()].ToString();

            //getObject = JObject.Parse(getString);

            detailForm = new Detail_Form(e.RowIndex, getString, (Object)packet[rowIndex]);
            detailForm.Show();
            serverBox.Text += "Click! + Contents" + e.RowIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chatTimer.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chatTimer.Enabled = false;
        }

        private void contentsHeaderClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = e.RowIndex;
            string getString = addObject[rowIndex.ToString()].ToString();


            detailForm = new Detail_Form(e.RowIndex, getString, (Object)packet[rowIndex]);
            detailForm.Show();
            serverBox.Text += "Click! + Contents" + e.RowIndex;
        }

        private int randomTimer()
        {
            return 1000 + rnd.Next(-900, -500);
        }
    }
}
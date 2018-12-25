using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ConsoleApp1.Program;

namespace ConsoleApp2
{
    public partial class Detail_Form : Form
    {
        int i;
        string getString;
        JObject getObject;
    
        Object pd;

        public Detail_Form()
        {
            InitializeComponent();
        }

        public Detail_Form(int i, string getString, Object pd)
        {
            InitializeComponent();
            this.i = i;
            this.getString = getString;

           
            this.pd = pd;

            getObject = new JObject();
            getObject = JObject.Parse(getString);
         

        }

        private void Detail_Form_Load(object sender, EventArgs e)
        {
            string clientName = getObject["id"].ToString();
            string clientMsg = getObject["msg"].ToString();
            string messageType = getObject["type"].ToString();
            


            string node_0 = "Frame " + i + " total Length" + (((Packet_Data)pd).total_length+14);
            string node_1 = "Ethernet Ⅱ, Src : " +((Packet_Data)pd).source_mac + ", Dst : " + ((Packet_Data)pd).destination_mac;
            string node_2 = "Internet Protocol Version 4, Src : "+ ((Packet_Data)pd).source_ip + ", Dst : " + ((Packet_Data)pd).destination_ip;
            string node_3 = "Transmission Control Protocol, Src Port : " + ((Packet_Data)pd).source_port + ". Dst Port : " + ((Packet_Data)pd).destination_port + ", Seq : " + ((Packet_Data)pd).Sequence_number + ", Ack : " + ((Packet_Data)pd).Ack_number + ", Len : " + ((Packet_Data)pd).total_length;
            TreeNode frameNode = new TreeNode(node_0);
            frameNode.Nodes.Add("Frame Number : "+i);
            frameNode.Nodes.Add("Frame Length : " + (((Packet_Data)pd).total_length+14) + " (1000 bits)");
            TreeNode ethNode = new TreeNode(node_1);
            ethNode.Nodes.Add("Type : " + ((Packet_Data)pd).ip_v + " (0x" + ((Packet_Data)pd).type + ")");

            TreeNode ipNode = new TreeNode(node_2);
            ipNode.Nodes.Add("Version : 4"); //수정 필요
            ipNode.Nodes.Add("Header Length : " + ((Packet_Data)pd).headerLength +"bytes (5)");
            ipNode.Nodes.Add("Total Length : " + ((Packet_Data)pd).total_length);
            ipNode.Nodes.Add("Identification : " + ((Packet_Data)pd).identification);
            ipNode.Nodes.Add("Flags : " + ((Packet_Data)pd).check_flags); //플래그 뭔지 모름
            ipNode.Nodes.Add("Time to live : " + ((Packet_Data)pd).TTL);
            ipNode.Nodes.Add("Protocol : " + ((Packet_Data)pd).protocol);
            ipNode.Nodes.Add("Header checkSum : " + ((Packet_Data)pd).header_check_sum); //??
            ipNode.Nodes.Add("Source : " + ((Packet_Data)pd).source_ip);
            ipNode.Nodes.Add("Destination : " + ((Packet_Data)pd).destination_ip);


            TreeNode transNode = new TreeNode(node_3);
            transNode.Nodes.Add("Source Port : " + ((Packet_Data)pd).source_port);
            transNode.Nodes.Add("Destination Port : " + ((Packet_Data)pd).destination_port);
            transNode.Nodes.Add("Sequence number : " + ((Packet_Data)pd).Sequence_number + "  (absolute sequence number)");
            transNode.Nodes.Add("Acknowledgment number : " + ((Packet_Data)pd).Ack_number +"  (absolute sequence number)");
            transNode.Nodes.Add("Header Length : " + ((Packet_Data)pd).headerLength + "bytes (5)");

            int flagNum = 0;

            if (((Packet_Data)pd).CWR) flagNum += 8 * 16;
            if (((Packet_Data)pd).ECN_Echo) flagNum += 4 * 16;
            if (((Packet_Data)pd).Urgent) flagNum += 2 * 16;
            if (((Packet_Data)pd).ACK) flagNum += 16;
            if (((Packet_Data)pd).Push) flagNum += 8;
            if (((Packet_Data)pd).Reset) flagNum += 4;
            if (((Packet_Data)pd).Syn) flagNum += 2;
            if (((Packet_Data)pd).Fin) flagNum += 1;

            //string s = "0x0"+(char)(flagNum / 16) + (char)(flagNum%16);
            string s = string.Format("{0:x}", flagNum);
            TreeNode flgasNode = new TreeNode("Flags : 0x0" + s);
            flgasNode.Nodes.Add("000. .... .... = Reserved : Not set");
            flgasNode.Nodes.Add("...0 .... .... = Nonce : Not set");
            s = ".... ";
            if (((Packet_Data)pd).CWR) s += "1"; else s += "0";
            s += "... .... = congestion Window Reduce (CWR)";
            flgasNode.Nodes.Add(s);
            s = ".... .";
            if (((Packet_Data)pd).ECN_Echo) s += "1"; else s += "0";
            s += ".. .... = ECN-Echo";
            flgasNode.Nodes.Add(s);

            s = ".... ..";
            if (((Packet_Data)pd).Urgent) s += "1"; else s += "0";
            s += ". .... = Urgent";
            flgasNode.Nodes.Add(s);

            s = ".... ...";
            if (((Packet_Data)pd).ACK) s += "1"; else s += "0";
            s += " .... = Acknowledgment";
            flgasNode.Nodes.Add(s);

            s = ".... .... ";
            if (((Packet_Data)pd).Push) s += "1"; else s += "0";
            s += "... = Push";
            flgasNode.Nodes.Add(s);

            s = ".... .... .";
            if (((Packet_Data)pd).Reset) s += "1"; else s += "0";
            s += ".. = Reset";
            flgasNode.Nodes.Add(s);

            s = ".... .... ..";
            if (((Packet_Data)pd).Syn) s += "1"; else s += "0";
            s += ". = Syn";
            flgasNode.Nodes.Add(s);

            s = ".... .... ...";
            if (((Packet_Data)pd).Fin) s += "1"; else s += "0";
            s += " = Fin";
            flgasNode.Nodes.Add(s);

            transNode.Nodes.Add("Window size value : " + ((Packet_Data)pd).windowSize);
            transNode.Nodes.Add("Checksum : 0x" + ((Packet_Data)pd).checksum);
            transNode.Nodes.Add("Urgent pointer : " + ((Packet_Data)pd).urgent_pointer);

            tree.Nodes.Add(frameNode);
            tree.Nodes.Add(ethNode);
            tree.Nodes.Add(ipNode);
            tree.Nodes.Add(transNode);

            transNode.Nodes.Add(flgasNode);

            if (((Packet_Data)pd).Syn)
            {
                string node_4 = "Options : Maxium segment size";

                TreeNode optionNode = new TreeNode(node_4);

               
                TreeNode tcpOptions = new TreeNode("TCP Option - Maximum segment size : " + ((Packet_Data)pd).MSS_Value + " bytes");

                tcpOptions.Nodes.Add("Kind : Maximun Segment Size (" + ((Packet_Data)pd).MSS_kind+ ")");
                tcpOptions.Nodes.Add("Length : " + ((Packet_Data)pd).MSS_Length);
                tcpOptions.Nodes.Add("MSS Value : " + ((Packet_Data)pd).MSS_Value);

                TreeNode windowOption = new TreeNode("TCP Option - Window scale : " + ((Packet_Data)pd).Window_ShiftCnt);

                windowOption.Nodes.Add("Kind : Window Scale " + ((Packet_Data)pd).Window_Kind);
                windowOption.Nodes.Add("Length : " + ((Packet_Data)pd).Window_Length);
                windowOption.Nodes.Add("MSS Value : " + ((Packet_Data)pd).Window_ShiftCnt);

                TreeNode sackOption = new TreeNode("TCP Option - SACK permitted");

                sackOption.Nodes.Add("Kind : SACK permitted " + ((Packet_Data)pd).SACK_Kind);
                sackOption.Nodes.Add("Length : " + ((Packet_Data)pd).SACK_Length);


                optionNode.Nodes.Add(tcpOptions);
                optionNode.Nodes.Add(windowOption);
                optionNode.Nodes.Add(sackOption);

                tree.Nodes.Add(optionNode);
            }
            else if(((Packet_Data)pd).Push && ((Packet_Data)pd).ACK)
            {
                string node_4 = "Data info";

                TreeNode optionNode = new TreeNode(node_4);
                optionNode.Nodes.Add(getObject.ToString());

           
                tree.Nodes.Add(optionNode);
            }

            textBox.Text = ((Packet_Data)pd).Original_sign;
        }
    }
}

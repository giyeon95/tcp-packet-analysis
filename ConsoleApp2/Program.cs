using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ConsoleApp1
{
    class Program
    {
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using (var sr = new StringReader(hex))
            {
                for (int i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
                //Console.WriteLine(hex+"/"+(char)bytes[0]+ (char)bytes[1]);
            }
            return bytes;
        }


        public struct Packet_Data
        {
            public string source_mac;
            public string destination_mac;
            public string type; //IP 버전 타입 0800이면 ipv4
            public string ip_v; //ip 버전
            public string ip_header_Status; //헤더 상태
            public string source_ip;
            public string destination_ip;
            public int total_length;
            public string identification; //새로운 데이터그램이 보내질 때마다 1씩 증가 분할된 데이터그램을 다시 합치는데 사용됨
            public bool dont_frag;
            public bool more_frag;
            public int offset; // (분할위치정보)
            public int TTL; //time to live (window는 128)
            public string protocol;
            public string header_check_sum;
            public int source_port;
            public int destination_port;
            public string Sequence_number;
            public string Ack_number;
            public int headerLength;
            public bool CWR; //Congestion Window Reduced
            public bool ECN_Echo;
            public bool Urgent;
            public bool ACK;
            public bool Push;
            public bool Reset;
            public bool Syn;
            public bool Fin;
            public string check_flags; // 플래그 잘됐나 확인 해볼려고 넣은거 ㅎㅎ
            public string TextData;
            public string Original_sign;
            public int NumOfPacket;
            public int windowSize;
            public string checksum;
            public int urgent_pointer;
            public bool MSS;  //maximum segment size
            public string MSS_kind;
            public int MSS_Length;
            public int MSS_Value;
            public string Window_Kind;
            public int Window_Length;
            public int Window_ShiftCnt;
            public string SACK_Kind;
            public int SACK_Length;





        };
        static void Main(string[] args)
        {
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
            string line = File.ReadAllText(Application.StartupPath+"/TESTTEXT.txt", Encoding.UTF8);
            string str = "";
            int r_index = 0;
            int data_index = 0;
            ArrayList list = new ArrayList();
            String Data = "";
            Packet_Data temp = new Packet_Data();
            int numOfPacket = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (!line[i].Equals('\r') && (!line[i].Equals('\n')) && (!line[i].Equals(' ')))
                {
                    str += line[i];
                    r_index++;
                }
                else
                {
                    if (!str.Equals(""))
                    {
                        temp.Original_sign += str + " ";
                        data_index++;
                        int value = Convert.ToInt32(str, 16);

                        if (0 < data_index && data_index <= 6)
                        {
                            temp.destination_mac += (str + " "); // 1
                        }
                        else if (6 < data_index && data_index <= 12)
                        {
                            temp.source_mac += (str + " "); // 2
                        }
                        else if (12 < data_index && data_index <= 14)
                        {
                            temp.type += (str + "");
                        }
                        else if (data_index == 15)
                        {
                            if (str[0] == '4')
                                temp.ip_v = "ip_4";
                            else
                                temp.ip_v = "ip_6";
                            if (value % 16 == 5)
                                temp.ip_header_Status = "normal";
                            else if (value % 16 < 5)
                                temp.ip_header_Status = "Error";
                            else
                                temp.ip_header_Status = "hasOption";
                        }

                        else if (data_index == 16) { } //무시 TOS
                        else if (16 < data_index && data_index <= 18)
                        {
                            temp.total_length += value;
                        }
                        else if (18 < data_index && data_index <= 20)
                        {
                            temp.identification += str;
                        }
                        else if (20 < data_index && data_index <= 21)
                        {
                            if (value / 16 >= 4)
                                temp.dont_frag = true;
                            else
                                temp.dont_frag = false;
                            if ((value / 16) % 4 >= 2)
                                temp.more_frag = true;
                            else
                                temp.more_frag = false;
                            if ((value / 16) % 2 == 1)
                                temp.offset += 256 * 16;
                            temp.offset += (value % 16) * 256;
                        }
                        else if (data_index == 22)
                            temp.offset += value;
                        else if (data_index == 23)
                            temp.TTL = value;
                        else if (data_index == 24)
                        {
                            if (value == 1)
                                temp.protocol = "ICMP";
                            else if (value == 6)
                                temp.protocol = "TCP";
                            else if (value == 8)
                                temp.protocol = "EGP";
                            else if (value == 17)
                                temp.protocol = "UDP";
                            else if (value == 89)
                                temp.protocol = "OSPF";
                            else if (value == 88)
                                temp.protocol = "IGRP";
                            else
                                temp.protocol = "UnknownProtocol";
                        }
                        else if (24 < data_index && data_index <= 26)
                            temp.header_check_sum += str;
                        else if (26 < data_index && data_index <= 30)
                        {
                            temp.source_ip += (value + ".");
                        }
                        else if (30 < data_index && data_index <= 34)
                        {
                            temp.destination_ip += (value + ".");
                        }
                        else if (34 < data_index && data_index <= 36)
                        {
                            if (data_index == 35)
                                temp.source_port = value * 256;
                            else
                                temp.source_port += value;
                        }
                        else if (36 < data_index && data_index <= 38)
                        {
                            if (data_index == 37)
                                temp.destination_port = value * 256;
                            else
                                temp.destination_port += value;
                        }
                        else if (38 < data_index && data_index <= 42)
                        {
                            temp.Sequence_number += str;
                        }
                        else if (42 < data_index && data_index <= 46) { temp.Ack_number += str; }
                        else if (data_index == 47)
                        {
                            temp.headerLength = value / 16 * 4;
                        }
                        else if (data_index == 48)
                        {
                            int t_N = value;
                            int index = 0;
                            int binary = 128;
                            while (t_N > 0)
                            {
                                if (t_N / binary == 1)
                                {
                                    if (index == 0)
                                        temp.CWR = true;
                                    else if (index == 1)
                                        temp.ECN_Echo = true;
                                    else if (index == 2)
                                        temp.Urgent = true;
                                    else if (index == 3)
                                        temp.ACK = true;
                                    else if (index == 4)
                                        temp.Push = true;
                                    else if (index == 5)
                                        temp.Reset = true;
                                    else if (index == 6)
                                        temp.Syn = true;
                                    else if (index == 7)
                                        temp.Fin = true;
                                }
                                t_N %= binary;
                                binary /= 2;
                                index++;
                            }
                            temp.check_flags = Convert.ToString(value, 2);
                        }
                        else if (data_index == 49)
                            temp.windowSize += value * 256;
                        else if (data_index == 50)
                            temp.windowSize += value;
                        else if (50 < data_index && data_index <= 52)
                            temp.checksum += str;
                        else if (53 == data_index)
                            temp.urgent_pointer = 256 * value;
                        else if (54 == data_index)
                            temp.urgent_pointer = value;
                        else
                        {
                            if (temp.Push && temp.ACK)
                                Data += str;
                            else if (temp.Syn == true)
                            {
                                if (data_index == 55)
                                {
                                    if (value == 2)
                                    {
                                        temp.MSS = true;
                                        temp.MSS_kind = value + " : Maximum Segment Size";
                                    }
                                    else
                                        temp.MSS = false;
                                }
                                else if (temp.MSS == true)
                                {
                                    if (data_index == 56)
                                        temp.MSS_Length = value;
                                    else if (data_index == 57)
                                        temp.MSS_Value = value * 256;
                                    else if (data_index == 58)
                                        temp.MSS_Value += value;
                                }
                            }
                            if (data_index == 60)
                            {
                                if (value == 3)
                                    temp.Window_Kind = value + ": window scale";
                            }
                            else if (data_index == 61)
                                temp.Window_Length = value;
                            else if (data_index == 62)
                                temp.Window_ShiftCnt = value;
                            else if (data_index == 65)
                                temp.SACK_Kind = value + " : SACK permitted";
                            else if (data_index == 66)
                                temp.SACK_Length = value;


                        }
                        str = "";
                    }
                    if (line[i].Equals('\n'))
                    {
                        if (r_index != 0) {
                            temp.Original_sign += "\n";
                        }
                        else
                        {
                            data_index = 0;
                            byte[] arr_byteStr = StringToByteArray(Data);

                            String resultHex2 = String.Empty;

                            resultHex2 += Encoding.Default.GetString(arr_byteStr);
                            Data = "";
                            temp.TextData = resultHex2;
                            if (temp.Syn)
                            {
                                if (temp.ACK)
                                    temp.TextData = "SYN, ACK";
                                else
                                    temp.TextData = "SYN";
                            }
                            else if (temp.ACK)
                            {
                                if (temp.Reset)
                                    temp.TextData = "RST, ACK";
                                else if (!temp.Push)
                                    temp.TextData = "ACK";
                            }
                            temp.NumOfPacket = numOfPacket;
                            numOfPacket++;

                            list.Add(temp);

                            temp = new Packet_Data();
                        }
                        r_index = 0;
                    }
                }
            }
            Console.WriteLine();
            foreach (Packet_Data a in list)
            {

                Console.WriteLine("source IP : " + a.source_ip + ",Destination IP : " + a.destination_ip + ", source Port : " + a.source_port + ", destination Port : " + a.destination_port + ", DATA : " + a.TextData);
                if (a.Syn)
                    Console.WriteLine("TCP Options => " + "Kind : " + a.MSS_kind + " , Length : " + a.MSS_Length + " , MSS Value : " + a.MSS_Value + " , " + "Kind : " + a.Window_Kind + " , Length : " + a.Window_Length + " , Shift Count : " + a.Window_ShiftCnt + "Kind : " + a.SACK_Kind + " , Length : " + a.SACK_Length);

            }
        

    ConsoleApp2.Form1 mainForm = new ConsoleApp2.Form1(list);
            mainForm.ShowDialog();
            //mainForm.Show(); 

        }
    }
}
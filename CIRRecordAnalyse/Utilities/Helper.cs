using System;
using System.Collections.Generic;
using System.Text;

namespace CIRRecordAnalyse.Utilities
{
    class Helper
    {

        public static VoiceEncode VoiceEncode = new VoiceEncode();

        static readonly string NewLine = Environment.NewLine;
        private static readonly char[] hexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        private static readonly string[] CommandType = new string[] { "调度命令", "路票", "绿色许可证", "红色许可证", "出站跟踪调车通知书", "轻型车辆使用书", "列车进路预信息", "正线通过", "侧线通过", "正线缓行通过", "进站正线停车", "进站侧线停车" };

        public static int BCD2Int(byte bcd)
        {
            int high = bcd >> 4;
            if (high > 9) return -1;
            int low = bcd & 0x0f;
            if (low > 9) return -1;
            return high * 10 + low;
        }
        private static ulong BCDToHex(ulong pbcd, uint num)         //BCD 转 16进制
        {
            int i;
            byte[] tmp = new byte[num];
            ulong pdiv = pbcd;
            for (i = 0; i < num; i++)
            {
                tmp[i] = (byte)(pdiv % 0x10);
                pdiv /= 0x10;
            }
            ulong pmul = 1;
            pdiv = 0;
            for (i = 0; i < num; i++)
            {
                pdiv += pmul * tmp[i];
                pmul *= 10;
            }
            return pdiv;
        }

        private static string BCDToInverseHex(byte pHex)
        {
            char[] chars;
            if ((pHex & 240) == 240)
            {
                chars = new char[] { hexDigits[pHex & 15] };
            }
            else
            {
                chars = new char[2];
                chars[1] = hexDigits[pHex >> 4];
                chars[0] = hexDigits[pHex & 15];
            }
            return new string(chars);
        }

        private static bool IsRightASCII(byte[] tmparray, int index, int length)
        {
            bool flag = false;
            if (length < 3)
            {
                return false;
            }
            for (int i = index; i < (index + length); i++)
            {
                byte num = tmparray[i];
                flag = false;
                if ((num >= 0x30) && (num <= 0x39))
                {
                    flag = true;
                }
                else if ((num >= 0x41) && (num <= 0x5d))
                {
                    flag = true;
                }
                else if ((num >= 0x61) && (num <= 0x7a))
                {
                    flag = true;
                }
                else if ((((((num == 13) || (num == 10)) || ((num == 0x27) || (num == 0x2e))) || (((num == 0x3b) || (num == 0x20)) || ((num == 0x2f) || (num == 0x3a)))) || (num == 0x3d)) || (num == 0x2c))
                {
                    flag = true;
                }
                if (!flag)
                {
                    return flag;
                }
            }
            return flag;
        }

        private static string GetModeString(int mode)                   //工作模式
        {
            string str1 = "";// "工作模式：/";
            switch (mode)
            {
                case 1:
                    str1 = str1 + "B1制式Ⅰ频组 457.500MHz";
                    break;

                case 2:
                    str1 = str1 + "B1制式Ⅱ频组 457.550MHz";
                    break;

                case 3:
                    str1 = str1 + "B1制式Ⅲ频组 457.700MHz";
                    break;

                case 4:
                    str1 = str1 + "B1制式Ⅳ频组 457.825MHz";
                    break;

                case 5:
                    str1 = str1 + "B1制式Ⅴ频组 457.925MHz";
                    break;

                case 6:
                    str1 = str1 + "B1制式Ⅵ频组 458.000MHz";
                    break;

                case 7:
                    str1 = str1 + "B1制式Ⅶ频组 458.200MHz";
                    break;

                case 8:
                    str1 = str1 + "B1制式Ⅷ频组 458.250MHz";
                    break;

                case 9:
                    str1 = str1 + "B2制式Ⅰ频组 457.500MHz";
                    break;

                case 10:
                    str1 = str1 + "B2制式Ⅱ频组 457.550MHz";
                    break;

                case 11:
                    str1 = str1 + "B2制式Ⅲ频组 457.700MHz";
                    break;

                case 12:
                    str1 = str1 + "B2制式Ⅳ频组 457.825MHz";
                    break;

                case 13:
                    str1 = str1 + "B2制式Ⅴ频组 457.925MHz";
                    break;

                case 14:
                    str1 = str1 + "B2制式Ⅵ频组 458.000MHz";
                    break;

                case 15:
                    str1 = str1 + "B2制式Ⅶ频组 458.200MHz";
                    break;

                case 0x10:
                    str1 = str1 + "B2制式Ⅷ频组 458.250MHz";
                    break;

                case 0x11:
                    str1 = str1 + "C制式Ⅰ频组 457.500MHz";
                    break;

                case 0x12:
                    str1 = str1 + "C制式Ⅱ频组 457.550MHz";
                    break;

                case 0x13:
                    str1 = str1 + "C制式Ⅲ频组 457.700MHz";
                    break;

                case 20:
                    str1 = str1 + "C制式Ⅳ频组 457.825MHz";
                    break;

                case 0x15:
                    str1 = str1 + "C制式Ⅴ频组 457.925MHz";
                    break;

                case 0x16:
                    str1 = str1 + "C制式Ⅵ频组 458.000MHz";
                    break;

                case 0x17:
                    str1 = str1 + "C制式Ⅶ频组 458.200MHz";
                    break;

                case 0x18:
                    str1 = str1 + "C制式Ⅷ频组 458.250MHz";
                    break;

                case 0x65:
                    str1 = str1 + "GSMR";
                    break;

                case 0x6e:
                    str1 = str1 + "大秦线AC制式";
                    break;

                default: str1 = str1 + mode.ToString("X2"); break;
            }
            return (str1 + NewLine);
        }

        private static string GetInformationString(byte[] tmparray1, int tHead)
        {
            string str1 = "主机报告综合信息" + NewLine;
            if (tmparray1[tHead] == 1)
            {
                str1 = str1 + "网络状态：GSM" + NewLine;
            }
            else if (tmparray1[tHead] == 2)
            {
                str1 = str1 + "网络状态：GSMR" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 0x80) == 0x80)
            {
                str1 = str1 + "列尾已连接" + NewLine;
            }
            else
            {
                str1 = str1 + "列尾未连接" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 0x40) == 0x40)
            {
                str1 = str1 + "数据模块工作状态：电路方式" + NewLine;
            }
            else
            {
                str1 = str1 + "数据模块工作状态：GPRS" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 0x20) == 0x20)
            {
                str1 = str1 + "已获取IP地址" + NewLine;
            }
            else
            {
                str1 = str1 + "未获取IP地址" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 0x10) == 0x10)
            {
                str1 = str1 + "机车号已注册" + NewLine;
            }
            else
            {
                str1 = str1 + "机车号未注册" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 8) == 8)
            {
                str1 = str1 + "车次号已注册" + NewLine;
            }
            else
            {
                str1 = str1 + "车次号未注册" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 4) == 4)
            {
                str1 = str1 + "补机状态    序号：" + tmparray1[tHead + 5].ToString() + NewLine;
            }
            else
            {
                str1 = str1 + "本务机状态" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 2) == 2)
            {
                str1 = str1 + "机车号报警(设置的机车号与机车监控装置送的机车号不一致)" + NewLine;
            }
            if ((tmparray1[tHead + 1] & 1) == 1)
            {
                str1 = str1 + "手动工作模式" + NewLine;
            }
            else
            {
                str1 = str1 + "自动工作模式" + NewLine;
            }
            str1 = ((str1 + "GSM-R语音模块场强级别：" + tmparray1[tHead + 2].ToString() + NewLine) + "GPRS数据模块场强级别：" + tmparray1[tHead + 3].ToString() + NewLine) + "工作模式：" + GetModeString(tmparray1[tHead + 4]);
            tHead += 6;
            string CSt0t0002 = str1;
            str1 = CSt0t0002 + "本机IP：" + tmparray1[tHead].ToString() + "." + tmparray1[tHead + 1].ToString() + "." + tmparray1[tHead + 2].ToString() + "." + tmparray1[tHead + 3].ToString() + NewLine;
            tHead += 4;
            CSt0t0002 = str1;
            str1 = CSt0t0002 + "DMIS IP：" + tmparray1[tHead].ToString() + "." + tmparray1[tHead + 1].ToString() + "." + tmparray1[tHead + 2].ToString() + "." + tmparray1[tHead + 3].ToString() + NewLine;
            tHead += 4;
            CSt0t0002 = str1;
            str1 = CSt0t0002 + "列尾IP：" + tmparray1[tHead].ToString() + "." + tmparray1[tHead + 1].ToString() + "." + tmparray1[tHead + 2].ToString() + "." + tmparray1[tHead + 3].ToString() + NewLine;
            tHead += 4;
            int j = 0;
            j = 0;
            while (j < 20)
            {
                if (tmparray1[tHead + j] == 0x3b)
                {
                    break;
                }
                j++;
            }
            str1 = str1 + "车次号：" + Encoding.Default.GetString(tmparray1, tHead, j) + NewLine;
            tHead += j + 1;
            j = 0;
            while (j < 20)
            {
                if (tmparray1[tHead + j] == 0x3b)
                {
                    break;
                }
                j++;
            }
            str1 = str1 + "机车号：" + Encoding.Default.GetString(tmparray1, tHead, j) + NewLine;
            tHead += j + 1;
            j = 0;
            while (j < 50)
            {
                if (tmparray1[tHead + j] == 0x3b)
                {
                    break;
                }
                j++;
            }
            str1 = str1 + "线路名称号：" + Encoding.Default.GetString(tmparray1, tHead, j) + NewLine;
            tHead += j + 1;
            return str1;
        }

        public static string GetPoartName(int portNum)          //所有端口名
        {
            switch (portNum)
            {
                case 0:
                    return "所有端口";
                case 1:
                    return "主机端口";
                case 2:
                    return "两个MMI";
                case 3:
                    return "MMI1";
                case 4:
                    return "MMI2";
                case 5:
                    return "450M端口";
                case 6:
                    return "GPS端口";
                case 7:
                    return "记录单元端口";
                case 0x11:
                    return "DMIS编码器";
                case 0x12:
                    return "监控调机分机";
                case 0x13:
                    return "LBJ/800M";
                case 20:
                    return "监控机车装置";
                case 0x21:
                    return "检测服务器";
                case 0x23:
                    return "DMIS总机";
                case 0x24:
                    return "CTC总机";
                case 0x25:
                    return "监控服务器";
                case 0x26:
                    return "列尾主机";
                case 0x3f:
                    return "维护端口";
                case 0x31:
                    return "库检端口";
            }
            return portNum.ToString("X2");
        }

        public static string GetStyleName(int styleNum)             //所有业务类型
        {
            switch (styleNum)
            {
                case 1:
                    return "维护";

                case 2:
                    return "公共信息";

                case 3:
                    return "调度通信";

                case 4:
                    return "列尾风压";

                case 5:
                    return "无线车次号";

                case 6:
                    return "调度命令";

                case 7:
                    return "列车停稳";

                case 8:
                    return "调车监控";

                case 9:
                    return "工况信息";

                case 10:
                    return "工务信息";

                case 11:
                    return "800MHz";

                case 12:
                    return "监控信息1";

                case 13:
                    return "监控信息2";

                case 14:
                    return "弓况信息";

                case 0xa1:
                    return "交互信息";
            }
            return styleNum.ToString("X2");
        }

        public static string GetExplainString(int sourceport, int destport, int comstyle, int command, int tlen)        //解释字符串??
        {
            int tt;
            string str1 = "";
            if (sourceport == 0x02 && destport == 0x01 && comstyle == 0x06 && command == 0x3C)
            {
                str1 = "MMI请求擦除调度命令";
                return str1;
            }
            if (sourceport == 0x01 && destport == 0x13 && comstyle == 0x0B && command == 0x55)
            {
                str1 = "主机向MMI报告综合信息";
                return str1;
            }
            if (sourceport == 0x03 && destport == 0x13 && comstyle == 0x0B && command == 0xF2)
            {
                str1 = "MMI向LBJ发送800M货列尾设置状态";
                return str1;
            }
            if (sourceport == 0x01 && destport == 0x05 && comstyle == 0x13 && command == 0xF2)
            {
                str1 = "主机发给450M单元的GPS状态：" + NewLine;
                return str1;
            }

            if (destport == 0x02 && sourceport == 0x01 && comstyle == 0x03 && command == 0x92)
            {
                str1 = "主机发送给MMI出入库检测结果发送状态";
                return str1;
            }


            if (destport == 0x02 && sourceport == 0x01 && comstyle == 0x03 && command == 0x92)
            {
                str1 = "主机发送给MMI出入库检测结果发送状态";
                return str1;
            }

            if ((sourceport == 0x03 || sourceport == 0x04 || sourceport == 0x02) && destport == 0x01 && comstyle == 0x03 && command == 0xE1)
            {
                str1 = "MMI发送给主机状态配置信息";
                return str1;
            }




            if ((destport == 0x03 || destport == 0x04 || destport == 0x02) && sourceport == 0x01 && comstyle == 0x03 && command == 0x90)
            {
                str1 = "收到查询功能状态信息后返回数据信息";
                return str1;
            }

            if (destport == 0x02 && sourceport == 0x01 && comstyle == 0x03 && command == 0x91)
            {
                str1 = "主机发送给MMI调度命令签收及提示主控状态";
                return str1;
            }
            if (destport == 0x13 && sourceport == 0x01 && comstyle == 0x03 && command == 0x55)
            {
                str1 = "主机报告综合信息";
                return str1;
            }
            if ((destport == 02 || destport == 03 || destport == 04) && sourceport == 0x13 && comstyle == 0x0B)
            {
                switch (command)
                {
                    case 0x50:
                        str1 = "LBJ向MMI发送非停车状态不能发送报警信息";
                        break;
                    case 0x51:
                        str1 = "LBJ向MMI发送列车移动报警信息解除提示信息";
                        break;
                    case 0xE4:
                        str1 = "LBJ向MMI反馈设置结果";
                        break;
                    case 0x11:
                        str1 = "LBJ向MMI发送防护报警试验信息";
                        break;
                    case 0x0E:
                        str1 = "LBJ向MMI发送出入库检测结果";
                        break;
                    case 0x0F:
                        str1 = "LBJ通知MMI出入库检测失败";
                        break;
                    case 0x21:
                        str1 = "LBJ设备（单元）向MMI回送最新10条发送的报警信息；";
                        break;
                    case 0x22:
                        str1 = "LBJ设备（单元）向MMI回送前10条发送的报警信息；";
                        break;
                    case 0x23:
                        str1 = "LBJ设备（单元）向MMI回送后10条发送的报警信息；";
                        break;
                    case 0x24:
                        str1 = "LBJ设备（单元）向MMI回送最新10条接收的报警信息；";
                        break;
                    case 0x25:
                        str1 = "LBJ设备（单元）向MMI回送前10条接收的报警信息；";
                        break;
                    case 0x26:
                        str1 = "LBJ设备（单元）向MMI回送后10条接收的报警信息；";
                        break;
                    case 0x4F:
                        str1 = "LBJ返回测试状态信息；";
                        break;
                }
            }

            if ((sourceport == 0x02 || sourceport == 0x03 || sourceport == 0x04) && destport == 0x13 && comstyle == 0x0B)
            {
                switch (command)
                {
                    case 0xE3:
                        str1 = "MMI向LBJ发送参数设置命令;";
                        break;
                    case 0x03:
                        str1 = "MMI查询（设置）LBJ工作状态";
                        break;
                    case 0x0C:
                        str1 = "MMI向LBJ发送出入库检请求命令";
                        break;
                    case 0x12:
                        str1 = "MMI向LBJ发送防护报警试验确认信息";
                        break;
                    case 0x4A:
                        str1 = "MMI通知LBJ进入/退出（单项）测试";
                        break;
                }

            }
            if ((destport == 0x03 || destport == 0x04) && sourceport == 0x13 && comstyle == 0x0B && command == 0x4F)
            {
                str1 = "LBJ返回进入/退出(单项)测试状态应答";
            }

            //补充"数据涵义"
            if ((sourceport == 3 || sourceport == 4) && destport == 6 && command == 0x4e)      //MMI向卫星定位单元发送线路设置信息
            {
                str1 = "MMI向卫星定位单元发送线路设置信息";
                return str1;
            }
            if ((destport == 02 || destport == 03 || destport == 04) && sourceport == 0x01 && comstyle == 0x03 && command == 0x56)
            {
                str1 = "车次号获取方式；";
                return str1;
            }

            if ((sourceport == 02 || sourceport == 03 || sourceport == 04) && destport == 0x13 && comstyle == 0x0B)
            {
                switch (command)
                {
                    case 0x21:
                        str1 = "MMI向LBJ设备（单元）查询最新10条发送的报警信息；";
                        break;
                    case 0x22:
                        str1 = "MMI向LBJ设备（单元）查询前10条发送的报警信息；";
                        break;
                    case 0x23:
                        str1 = "MMI向LBJ设备（单元）查询后10条发送的报警信息；";
                        break;
                    case 0x24:
                        str1 = "MMI向LBJ设备（单元）查询最新10条接收的报警信息；";
                        break;
                    case 0x25:
                        str1 = "MMI向LBJ设备（单元）查询前10条接收的报警信息；";
                        break;
                    case 0x26:
                        str1 = "MMI向LBJ设备（单元）查询后10条接收的报警信息；";
                        break;
                }
            }
            if ((destport == 02 || destport == 03 || destport == 04) && sourceport == 0x01 && comstyle == 0x01 && command == 0xC1)
            {
                str1 = "主机应答机车端号；";
                return str1;
            }
            if ((sourceport == 02 || sourceport == 03 || sourceport == 04) && destport == 0x01 && comstyle == 0x01 && command == 0xC0)
            {
                str1 = "MMI读取/设置机车端号；";
                return str1;
            }
            if (sourceport == 6 && destport == 2 && comstyle == 2)
            {
                if (command == 1) str1 = "卫星定位单元通知MMI人工选择线路";
                else if (command == 2) str1 = "卫星定位单元通知MMI人工确认线路";
                else if (command == 0xa1) str1 = "GPS单元应答上下行信息";
            }
            if ((sourceport == 6) && (tlen == 0x9e))
            {
                str1 = "GPS信息数据";
            }
            if (sourceport == 6)       //BQ:GPS的数据涵义补充<
            {
                if ((comstyle == 3) && (command == 1)) str1 = "应答信息";
                else if ((comstyle == 2) && (command == 3)) str1 = " 卫星定位单元向MMI发送退出线路选择区域信息";
            }
            if (sourceport == 5 && destport == 1 && comstyle == 0x71 && command == 0)
            {
                return "主机向MMI传送调度命令";
            }
            //>
            if ((destport == 5) && (sourceport == 1))
            {
                switch (command)
                {
                    case 8:
                        str1 = "主机向450M发送工作模式\r\n";
                        break;

                    case 0x20:
                        str1 = "主机向450M发送机车号\r\n";
                        break;

                    case 0xf1:
                        str1 = "主机向450M发送GPS状态\r\n";
                        break;
                }
            }
            if (sourceport == 1)       //主机源端口
            {
                if ((comstyle == 5) && (command == 0x21))
                {
                    str1 = "发送车次号信息";
                }
                if ((comstyle == 3) && (command == 0xff))
                {
                    str1 = "主机向外部录音口发送录音信号";
                }
                if (comstyle == 7 && (command == 2 || command == 3))
                {
                    str1 = "发送车次号信息";
                }
            }
            if (sourceport == 0x13)     //LBJ
            {
                tt = command;
                if (tt <= 10)
                {
                    switch (tt)
                    {
                        case 1:
                            str1 = "LBJ 向操作显示终端发送报警信息";
                            goto Label_0186;

                        case 2:
                        case 3:
                        case 9:
                            goto Label_0186;

                        case 4:
                            str1 = "LBJ 向操作显示终端发送当前状态";
                            goto Label_0186;

                        case 5:
                            str1 = "LBJ 向操作显示终端发送语音提示命令";
                            goto Label_0186;
                        case 8:
                            str1 = "LBJ 功能单元向CIR主机或操作显示终端报告自检结果";
                            goto Label_0186;

                        case 10:
                            str1 = "LBJ 功能单元报告软件版本";
                            goto Label_0186;
                    }
                }
                else if (comstyle == 0x04)
                {
                    switch (tt)
                    {
                        case 0x21:
                            str1 = "手动查询风压应答";
                            goto Label_0186;

                        case 0x22:
                            str1 = "排风制动应答";
                            goto Label_0186;

                        case 0x23:
                            str1 = "KLW 风压自动提示";
                            goto Label_0186;

                        case 0x24:
                            str1 = "KLW 供电电压欠压自动提示";
                            goto Label_0186;

                        case 0x25:
                            str1 = "输号命令";
                            goto Label_0186;

                        case 0x26:
                            str1 = "消号命令";
                            goto Label_0186;
                    }
                    if (tt == 0x41)
                    {
                        str1 = "应答数据";
                    }
                }
            }
        Label_0186:
            if (destport == 0x13)
            {
                switch (command)
                {
                    case 0:
                        if (sourceport == 0x11)
                        {
                            str1 = "主机向LBJ转发DMIS信息";
                        }
                        break;

                    case 2:
                        str1 = "操作显示终端向LBJ发送报警命令/解除报警命令";
                        break;

                    case 3:
                        str1 = "操作显示终端向LBJ查询当前状态";
                        break;

                    case 7:
                        str1 = "CIR 主机或操作显示终端请求LBJ 功能单元自检";
                        break;

                    case 9:
                        str1 = "查询LBJ 功能单元软件版本";
                        break;

                    case 11:
                        str1 = "操作显示终端在出入库检测状态下发送按键信息";
                        break;
                }
                if (comstyle == 0x04)
                {
                    switch (command)
                    {

                        case 0x21:
                            str1 = "手动查询风压命令";
                            break;

                        case 0x22:
                            str1 = "排风制动命令";
                            break;

                        case 0x23:
                            str1 = "KLW 风压自动提示应答";
                            break;

                        case 0x24:
                            str1 = "KLW 供电电压欠压自动提示应答";
                            break;

                        //BQ
                        case 0x25:
                            str1 = "输号命令";
                            break;
                        case 0x26:
                            str1 = "销号命令";
                            break;
                        //case 0x27:
                        //    str1 = "输号应答";
                        //    break;
                    }
                    switch (command)
                    {


                        case 40:
                            str1 = "自动查询风压应答";
                            break;

                        case 0x55:
                            str1 = "主机向LBJ发送综合信息";
                            break;

                        case 0xe5:
                            str1 = "操作显示终端接收到报警信息后发送确认按键信息";
                            break;

                        case 0x41:
                            str1 = "应答数据";
                            break;

                        case 0x48:
                            str1 = "主机向LBJ转发GPS信息";
                            break;
                    }
                }
            }
            if ((sourceport == 7) || (destport == 7))       //记录单元端口
            {
                switch (command)
                {
                    case 0x41:
                        str1 = "记录单元向MMI发送应答信息\r\n";
                        break;

                    case 70:
                        str1 = "CPU向记录单元发送开关机指令\r\n";
                        break;

                    case 0x33:
                        str1 = "CIR控制记录单元开始/停止播放话音记录\r\n";
                        break;

                    case 0x34:
                        str1 = "CIR查询记录单元当前时钟\r\n";
                        break;

                    case 0x35:
                        str1 = "CIR手动设置记录单元时钟\r\n";
                        break;

                    case 1:
                        str1 = "记录单元向CIR主机发送应答信息\r\n";
                        break;

                    case 0x91:
                        str1 = "记录单元向CIR输出当前时钟\r\n";
                        break;

                    case 0x92:
                        str1 = "记录单元向CIR发送播放结束的命令\r\n";
                        break;

                    case 0xa5:
                        str1 = "CIR查询记录单元软件版本\r\n";
                        break;

                    case 250:
                        str1 = "CIR向记录单元发送问讯测试\r\n";
                        break;

                    case 0xfb:
                        str1 = "记录单元向CIR发送问讯应答\r\n";
                        break;

                    case 170:
                        str1 = "记录单元向CIR输出当前软件版本\r\n";
                        break;
                }
            }
            if (((destport == 1) && (sourceport == 5)) && (comstyle == 0x71))      //主机->450M
            {
                switch (command)
                {
                    case 60:
                        str1 = "擦除调度命令操作";
                        goto Label_0455;

                    case 0x41:
                        str1 = "主机应答";
                        break;

                    case 0x20:
                        str1 = "调度命令";
                        goto Label_0455;

                    case 0x21:
                        str1 = "最近10条命令索引目录";
                        goto Label_0455;

                    case 0x22:
                        str1 = "前10条命令索引目录 ";
                        goto Label_0455;

                    case 0x23:
                        str1 = "后10条命令索引目录";
                        goto Label_0455;

                    case 0x24:
                        str1 = "无查询结果报告";
                        goto Label_0455;

                    case 0x2e:
                        str1 = "调度命令状态";
                        goto Label_0455;
                }
            }
        Label_0455:
            if ((destport < 2) || (destport > 4))                               //非MMI目的端口          
            {
                if ((sourceport >= 2) && (sourceport <= 4))            //MMI源端口
                {
                    switch (comstyle)
                    {
                        case 3:
                            switch (command)
                            {
                                case 1:
                                    return "MMI应答";

                                case 2:
                                    return "向主机申请端口号";

                                case 3:
                                    return "向主机申请综合信息";

                                case 4:
                                    return "向主机报告关电源";

                                case 5:
                                    return "请求主机自检";

                                case 6:
                                    return "设置本务机";

                                case 7:
                                case 12:
                                case 13:
                                case 14:
                                case 15:
                                case 0x10:
                                case 0x12:
                                case 20:
                                case 0x1a:
                                case 0x1b:
                                case 0x1c:
                                case 0x1d:
                                case 30:
                                case 0x1f:
                                    return str1;

                                case 8:
                                    return "选择工作模式 ";

                                case 9:
                                    return "MMI摘/挂机";

                                case 10:
                                    return "MMI(PTT)操作";

                                case 11:
                                    return "上电，MMI状态报告";

                                case 0x11:
                                    return "请求车次号注册或注销";

                                case 0x13:
                                    return "GSM-R呼叫操作";

                                case 0x15:
                                    return "接通保持电话";

                                case 0x16:
                                    return "(呼叫转移选择)操作";

                                case 0x17:
                                    return "(网络选择)操作";

                                case 0x18:
                                    return "申请主控";

                                case 0x19:
                                    return "MMI之间通信";

                                case 0x20:
                                    return "手工设置机车号";

                                case 0x21:
                                    return "MMI之间呼叫";

                                case 0x22:
                                    return "按键呼叫(450MHz)操作";

                                case 0x2e:
                                    return "主机进入或退出库检状态";

                                case 0x2f:
                                case 50:
                                case 0x33:
                                case 0x34:
                                case 0x37:
                                case 0x39:
                                    return str1;

                                case 0x30:
                                    return "设置日期时间";

                                case 0x31:
                                    return "查询有效网络";

                                case 0x35:
                                    return "查询VGCS/VBS组成员及状态";

                                case 0x36:
                                    return "设置VGCS/VBS组成员及状态";

                                case 0x38:
                                    return "查询网络名称";

                                case 0x3a:
                                    return "查询主机软件版本";

                                case 0x3b:
                                    return "查询主机语音模块名称";

                                case 0xe0:
                                    return "查询或设置维护界面中的IP或APN\r\n";

                                case 0xe1:
                                    return str1;

                                case 0xe2:
                                    return "查询状态信息\r\n";
                            }
                            return str1;

                        case 4:
                            switch (command)
                            {
                                case 0x21:
                                    return "MMI风压查询";

                                case 0x22:
                                    return "MMI排风命令";

                                case 0x23:
                                    return "MMI风压告警确认";

                                case 0x24:
                                    return "MMI电压告警确认";

                                case 0x25:
                                    return "MMI建立请求确认";

                                case 0x26:
                                    return "MMI主动拆除";

                                case 0x27:
                                    return str1;

                                case 40:
                                    return "MMI自动风压查询";

                                case 0x29:
                                    return "MMI向列尾建立对应关系启动";

                                case 0x2a:
                                    return "MMI拆除对应关系确认";
                            }
                            return str1;

                        case 5:
                            return str1;

                        case 6:
                            switch (command)
                            {
                                case 0x51:
                                    return "自动确认/签收/打印信息";

                                case 0x52:
                                case 0x57:
                                case 0x5c:
                                case 0x60:
                                    return str1;

                                case 0x53:
                                    return "调车请求信息";

                                case 0x54:
                                    return "最新一条信息 ";

                                case 0x55:
                                    return "上一条信息";

                                case 0x56:
                                    return "下一条信息";

                                case 0x58:
                                    return "最近10条调度命令目录";

                                case 0x59:
                                    return "最近10条行车凭证目录";

                                case 90:
                                    return "最近10条调车作业单目录";

                                case 0x5b:
                                    return "最近10条接车进路预告目录";

                                case 0x5d:
                                    return "申请前10条目录";

                                case 0x5e:
                                    return "申请后10条目录";

                                case 0x5f:
                                    return "申请某条信息";

                                case 0x61:
                                    return "出入库测试请求";

                                case 140:
                                    return "擦除调度命令操作";

                                case 0x41:
                                    return "MMI应答";
                            }
                            return str1;
                    }
                }
                return str1;
            }

            tt = comstyle;
            switch (tt)
            {
                case 3:
                    tt = command;
                    if (tt > 0x30)
                    {
                        switch (tt)
                        {
                            case 0x41:
                                return "主机应答";

                            case 0x42:
                                return "分配端口号";

                            case 0x43:
                                return "主控申请进行确认";

                            case 0x44:
                            case 0x47:
                            case 0x4d:
                            case 0x4e:
                            case 0x4f:
                            case 80:
                            case 0x56:
                            case 0x57:
                            case 0x5c:
                            case 0x5e:
                                return str1;

                            case 0x45:
                                return "自检结果";

                            case 70:
                                return "报告关电源";

                            case 0x48:
                                return "GPS向MMI发布信息";

                            case 0x49:
                                return "450M报告发射状态";

                            case 0x4a:
                                return "450MHz场强";

                            case 0x4b:
                                return "450MHz呼叫进行确认";

                            case 0x4c:
                                return "450MHz来呼";

                            case 0x51:
                                return "450M通话已经终止";

                            case 0x52:
                                return "上行链路状态、PTT请求结果";

                            case 0x53:
                                return "小区位置更新信息";

                            case 0x54:
                                return "网络注册状态";

                            case 0x55:
                                return "主机汇报综合信息";

                            case 0x58:
                                return "车次号注册/注销结果";

                            case 0x59:
                                return "机车号注册/注销结果";

                            case 90:
                                return "AC确认结果";

                            case 0x5b:
                                return "GSM-R呼叫进行确认";

                            case 0x5d:
                                return "GSMR当前的通话列表";

                            case 0x5f:
                                return "上传库检结果";
                            case 0x84:
                                return "报告呼叫转移操作结果";

                            case 0x85:
                            case 0x86:
                            case 0x89:
                            case 140:
                                return str1;

                            case 0x87:
                                return "报告查询有效网络结果";

                            case 0x88:
                                return "报告网络操作结果";

                            case 0x8a:
                                return "返回查询VGCS/VBS查询结果";

                            case 0x8b:
                                return "返回设置VGCS/VBS状态结果";

                            case 0x8d:
                                return "返回查询网络名称";

                            case 0x8e:
                                return "返回主机软件版本";

                            case 0x8f:
                                return "返回语音模块名称";

                            case 0xe0:
                                return "返回维护界面中的IP或APN\r\n";

                            case 0xe1:
                                return str1;

                            case 0xe2:
                                return "返回主机状态";
                        }
                        return str1;
                    }
                    switch (tt)
                    {
                        case 0x2e:
                            return "主机报告自检结果";

                        case 0x2f:
                            return str1;

                        case 0x30:
                            return "主机请求MMI设置时间信息";

                        case 0x19:
                            return "MMI之间通信";

                        //BQ
                        case 0x21:
                            return "MMI之间呼叫";
                    }
                    return str1;

                case 4:
                    switch (command)
                    {
                        case 0x21:
                            return "列尾人工风压查询";

                        case 0x22:
                            return "列尾排风命令确认";

                        case 0x23:
                            return "列尾风压告警";

                        case 0x24:
                            return "列尾电压告警";

                        case 0x25:
                            return "列尾建立请求";

                        case 0x26:
                            return "列尾拆除确认";

                        case 0x27:
                            return "列尾建立成功";

                        case 40:
                            return "列尾自动风压报告";

                        case 0x29:
                            return "主机申请列尾连接";

                        case 0x2a:
                            return "列尾拆除对应关系";
                    }
                    return str1;

                case 5:
                    return str1;

                case 6:
                    tt = command;
                    if (tt > 0x2e)
                    {
                        if (tt == 60)
                        {
                            return "擦除调度命令操作";
                        }
                        if (tt != 0x41)
                        {
                            return str1;
                        }
                        return "主机应答";
                    }
                    switch (tt)
                    {
                        case 0x20:
                            return "调度命令";

                        case 0x21:
                            return "最近10条命令索引目录";

                        case 0x22:
                            return "前10条命令索引目录 ";

                        case 0x23:
                            return "后10条命令索引目录";

                        case 0x24:
                            return "无查询结果报告";

                        case 0x2e:
                            return "向MMI传送调度命令状态";
                    }
                    return str1;

                case 0xa1:
                    if (command == 0xfe)
                    {
                        str1 = "450M向主机和MMI返回工作模式";
                    }
                    return str1;
            }
            return str1;
        }

        public static string GetPortName2(int num)
        {
            string str1 = "";
            int ttt = num;
            if (ttt <= 0x13)
            {
                switch (ttt)
                {
                    case 0:
                        return "所有端口";

                    case 1:
                        return "主机端口";

                    case 2:
                        return "MMI双端口";

                    case 3:
                        return "MMI端口1";

                    case 4:
                        return "MMI端口2";

                    case 5:
                        return "450M端口";

                    case 6:
                        return "GPS端口";

                    case 7:
                        return "记录单元端口";

                    case 0x11:
                        return "TAX箱端口";

                    case 0x12:
                        return str1;

                    case 0x13:
                        return "LBJ端口";
                }
                return str1;
            }
            switch (ttt)
            {
                case 0x23:
                    return "DMIS端口";

                case 0x26:
                    return "列尾端口";

                case 0x31:
                    return "库检端口";
            }
            return str1;
        }

        private static string ProcessGps(int command, byte[] tmparray1, int tHead)                          //GPS端口信息
        {
            ulong year1;
            ulong mon1;
            ulong day1;
            ulong hour1;
            ulong min1;
            ulong sec1;
            string CSt0t0004;
            string str1 = "公用卫星定位位置信息\r\n命令解析：\r\n";
            str1 += (tmparray1[tHead] == 3 ? "外围设备共用信息" : "") + NewLine;
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            if (tmparray1[tHead + 2] == 0x41)
            {
                str1 += "GPS信息有效" + NewLine;
                string str2 = Encoding.Default.GetString(tmparray1, tHead + 3, 8).Trim();
                ushort num1 = BitConverter.ToUInt16(tmparray1, tHead + 11);
                object CSt0t0002 = str1;
                CSt0t0004 = string.Concat(new object[] { CSt0t0002, "线路名称：", str2, "\r\n区段代码：", num1, NewLine }) + "区段名称：" + Encoding.Default.GetString(tmparray1, tHead + 13, 0x15) + NewLine;
                str1 = CSt0t0004 + "工作模式：" +/* tmparray1[tHead + 0x22].ToString("X2") + "," +*/ GetModeString(tmparray1[tHead + 0x22]);
                if ((tmparray1[tHead + 0x23] == 0xff) && (tmparray1[tHead + 0x24] == 0xff))
                {
                    str1 = str1 + "经度纬度：无效" + NewLine;
                }
                else
                {
                    string longitude1 = BCDToHex((ulong)tmparray1[tHead + 0x23], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 0x24], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 0x25], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 0x26], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 0x27], 2).ToString("00") + "\"";
                    string latitude1 = BCDToHex((ulong)tmparray1[tHead + 40], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 0x29], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 0x2a], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 0x2b], 2).ToString("00") + "\"";
                    str1 = (str1 + "经度：" + longitude1 + NewLine) + "纬度：" + latitude1 + NewLine;
                }
                year1 = BCDToHex((ulong)tmparray1[tHead + 0x2c], 2);
                mon1 = BCDToHex((ulong)tmparray1[tHead + 0x2d], 2);
                day1 = BCDToHex((ulong)tmparray1[tHead + 0x2e], 2);
                hour1 = BCDToHex((ulong)tmparray1[tHead + 0x2f], 2);
                min1 = BCDToHex((ulong)tmparray1[tHead + 0x30], 2);
                sec1 = BCDToHex((ulong)tmparray1[tHead + 0x31], 2);
                CSt0t0004 = str1;
                str1 = (CSt0t0004 + "时间：20" + year1.ToString("00") + "-" + mon1.ToString("00") + "-" + day1.ToString("00") + " " + hour1.ToString("00") + ":" + min1.ToString("00") + ":" + sec1.ToString("00") + NewLine) + "调度电话：" + Encoding.Default.GetString(tmparray1, tHead + 50, 8).TrimEnd('\0') + NewLine + NewLine;
                //string[] stationInfo = {"","","",""};
                //for (int i = 1; i < 5; i++)
                //{
                //    //CSt0t0004 = str1;
                //    stationInfo[i - 1] = "车站" + i.ToString() + "名称：" + Encoding.Default.GetString(tmparray1, tHead + 58 + (i - 1) * 18, 8).Trim() + NewLine;
                //    stationInfo[i - 1] += "车站" + i.ToString() + "代码：" + BitConverter.ToUInt16(tmparray1, tHead + 66 + (i - 1) * 18).ToString() + NewLine;
                //    stationInfo[i - 1] += "车站" + i.ToString() + "电话：" + Encoding.Default.GetString(tmparray1, tHead + 68 + (i - 1) * 18, 8).Trim().Trim('\0') +NewLine + NewLine;
                //}
                //return stationInfo[0] + stationInfo[1] + stationInfo[2] + stationInfo[3];

                //for (int i = 1; i < 5; i++)
                //{
                //    CSt0t0004 = str1;
                //    CSt0t0004 = CSt0t0004 + "车站" + i.ToString() + "名称：" + Encoding.Default.GetString(tmparray1, (tHead + 50) + ((i - 1) * 0x12), 8).Trim() + NewLine;
                //    CSt0t0004 = CSt0t0004 + "车站" + i.ToString() + "代码：" + BitConverter.ToUInt16(tmparray1, (tHead + 0x3a) + ((i - 1) * 0x12)).ToString() + NewLine;
                //    str1 = CSt0t0004 + "车站" + i.ToString() + "电话：" + Encoding.Default.GetString(tmparray1, (tHead + 60) + ((i - 1) * 0x12), 8).Trim() + NewLine;
                //}

                for (int i = 1; i < 5; i++)
                {
                    CSt0t0004 = str1;
                    CSt0t0004 = CSt0t0004 + "车站" + i.ToString() + "名称：" + Encoding.Default.GetString(tmparray1, tHead + 58 + (i - 1) * 18, 8).Trim() + NewLine;
                    CSt0t0004 = CSt0t0004 + "车站" + i.ToString() + "代码：" + BitConverter.ToUInt16(tmparray1, tHead + 66 + (i - 1) * 18).ToString() + NewLine;
                    str1 = CSt0t0004 + "车站" + i.ToString() + "电话：" + Encoding.Default.GetString(tmparray1, tHead + 68 + (i - 1) * 18, 8).Trim().TrimEnd('\0');
                    if (i != 4) str1 += NewLine + NewLine;
                }

                return str1;
            }
            if (tmparray1[tHead + 2] == 0x56)
            {
                str1 = str1 + "GPS信息无效" + NewLine;
                year1 = BCDToHex((ulong)tmparray1[tHead + 0x2c], 2);
                mon1 = BCDToHex((ulong)tmparray1[tHead + 0x2d], 2);
                day1 = BCDToHex((ulong)tmparray1[tHead + 0x2e], 2);
                hour1 = BCDToHex((ulong)tmparray1[tHead + 0x2f], 2);
                min1 = BCDToHex((ulong)tmparray1[tHead + 0x30], 2);
                sec1 = BCDToHex((ulong)tmparray1[tHead + 0x31], 2);
                CSt0t0004 = str1;
                str1 = CSt0t0004 + NewLine + "时间：" + year1.ToString("00") + "-" + mon1.ToString("00") + "-" + day1.ToString("00") + " " + hour1.ToString("00") + ":" + min1.ToString("00") + ":" + sec1.ToString("00") + NewLine;
            }
            return str1;
        }

        //----------------------------
        private static string ProcessGps2(int command, byte[] tmparray1, int tHead)                          //GPS端口信息
        {
            ulong year1;
            ulong mon1;
            ulong day1;
            ulong hour1;
            ulong min1;
            ulong sec1;
            string CSt0t0004;
            string str1 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            if (tmparray1[tHead + 2] == 0x41)
            {
                str1 = "GPS信息有效" + " ";
                string str2 = Encoding.Default.GetString(tmparray1, tHead + 3, 8).Trim();
                ushort num1 = BitConverter.ToUInt16(tmparray1, tHead + 11);
                object CSt0t0002 = str1;

                //------------------------------------------------------
                CSt0t0004 = CSt0t0002 + "区段代码：" + num1 + " ";
                str1 = CSt0t0004 + "工作模式：" /*+ tmparray1[tHead + 0x22].ToString("X2") + "," */+ GetModeString(tmparray1[tHead + 0x22]) + " ";
                if ((tmparray1[tHead + 0x23] == 0xff) && (tmparray1[tHead + 0x24] == 0xff))
                {
                    str1 = str1 + "经度/纬度：无效" + " ";
                }
                else
                {
                    string longitude1 = BCDToHex((ulong)tmparray1[tHead + 0x23], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 0x24], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 0x25], 2).ToString("00") + BCDToHex((ulong)tmparray1[tHead + 0x26], 2).ToString("00") + BCDToHex((ulong)tmparray1[tHead + 0x27], 2).ToString("00");
                    string latitude1 = BCDToHex((ulong)tmparray1[tHead + 40], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 0x29], 2).ToString("00") + BCDToHex((ulong)tmparray1[tHead + 0x2a], 2).ToString("00") + BCDToHex((ulong)tmparray1[tHead + 0x2b], 2).ToString("00");
                    str1 = (str1 + "经度：" + longitude1 + " ") + "纬度：" + latitude1 + " ";
                }
                year1 = BCDToHex((ulong)tmparray1[tHead + 0x2c], 2);
                mon1 = BCDToHex((ulong)tmparray1[tHead + 0x2d], 2);
                day1 = BCDToHex((ulong)tmparray1[tHead + 0x2e], 2);
                hour1 = BCDToHex((ulong)tmparray1[tHead + 0x2f], 2);
                min1 = BCDToHex((ulong)tmparray1[tHead + 0x30], 2);
                sec1 = BCDToHex((ulong)tmparray1[tHead + 0x31], 2);
                CSt0t0004 = str1;
                str1 = (CSt0t0004 + "时间：20" + year1.ToString("00") + "-" + mon1.ToString("00") + "-" + day1.ToString("00") + " " + hour1.ToString("00") + ":" + min1.ToString("00") + ":" + sec1.ToString("00") + NewLine) + " 调度电话：" + Encoding.Default.GetString(tmparray1, tHead + 50, 8).TrimEnd('\0') + NewLine + NewLine;

                for (int i = 1; i < 5; i++)
                {
                    CSt0t0004 = str1;
                    CSt0t0004 = CSt0t0004 + "   车站" + i.ToString() + "：" + Encoding.Default.GetString(tmparray1, tHead + 58 + (i - 1) * 18, 8).Trim() + NewLine;
                    CSt0t0004 = CSt0t0004 + " 代码" + i.ToString() + "：" + BitConverter.ToUInt16(tmparray1, tHead + 66 + (i - 1) * 18).ToString() + NewLine;
                    str1 = CSt0t0004 + " 电话" + i.ToString() + "：" + Encoding.Default.GetString(tmparray1, tHead + 68 + (i - 1) * 18, 8).Trim().TrimEnd('\0');
                    if (i != 4) str1 += NewLine + NewLine;
                }

                return str1;
            }
            if (tmparray1[tHead + 2] == 0x56)
            {
                str1 = (str1 + "GPS信息无效" + NewLine) + "命令解析：" + NewLine;
                year1 = BCDToHex((ulong)tmparray1[tHead + 0x2c], 2);
                mon1 = BCDToHex((ulong)tmparray1[tHead + 0x2d], 2);
                day1 = BCDToHex((ulong)tmparray1[tHead + 0x2e], 2);
                hour1 = BCDToHex((ulong)tmparray1[tHead + 0x2f], 2);
                min1 = BCDToHex((ulong)tmparray1[tHead + 0x30], 2);
                sec1 = BCDToHex((ulong)tmparray1[tHead + 0x31], 2);
                CSt0t0004 = str1;
                str1 = CSt0t0004 + NewLine + "时间：" + year1.ToString("00") + "-" + mon1.ToString("00") + "-" + day1.ToString("00") + " " + hour1.ToString("00") + ":" + min1.ToString("00") + ":" + sec1.ToString("00") + NewLine;
            }
            return str1;
        }

        private static string ProcessLbjData(int command, byte[] tmparray1, int tHead, int sourceport, int comstyle)
        {
            //string my_cch1;
            //string my_str2;
            //int j;           
            int jch_l;
            int id_l;
            int press_l;
            string cchtype;
            int cchnum;
            int numgong;
            int timelong;
            float gonglibiao;
            int sec;
            int min;
            int hour;
            int day;
            int mon;
            int year;
            int CSt4t0002;
            //byte []my_tmparray1 = null;
            //string CSt0t0003;
            string str1 = "";
            //int jch;
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            if (sourceport != 0x13)
            {
                CSt4t0002 = command;
                if (comstyle == 0x0B)
                {
                    switch (CSt4t0002)
                    {
                        case 0x21:
                        case 0x22:
                        case 0x23:
                        case 0x24:
                        case 0x25:
                        case 0x26:
                            if (command == 0x21) str1 += "MMI向LBJ设备（单元）查询最新10条发送的报警信息：" + NewLine;
                            else if (command == 0x22) str1 += "MMI向LBJ设备（单元）查询前10条发送的报警信息：" + NewLine;
                            else if (command == 0x23) str1 += "MMI向LBJ设备（单元）查询后10条发送的报警信息：" + NewLine;
                            else if (command == 0x24) str1 += "MMI向LBJ设备（单元）查询最新10条接收的报警信息：" + NewLine;
                            else if (command == 0x25) str1 += "MMI向LBJ设备（单元）查询前10条接收的报警信息：" + NewLine;
                            else if (command == 0x26) str1 += "MMI向LBJ设备（单元）查询后10条接收的报警信息：" + NewLine;
                            return str1;
                    }
                }

                if (CSt4t0002 <= 40)
                {
                    switch (CSt4t0002)
                    {
                        case 0:
                            if (sourceport == 0x11)
                            {
                                str1 = "主机向LBJ转发DMIS信息";
                            }
                            return TAX_Data(command, tmparray1, tHead);

                        case 1:
                        case 4:
                            return str1;

                        case 2:
                            str1 = "操作显示终端向LBJ发送报警命令/解除报警命令" + NewLine;
                            if (tmparray1[tHead] != 1)
                            {
                                return (str1 + "报警解除" + NewLine);
                            }
                            return (str1 + "启动报警" + NewLine);

                        case 3:
                            #region
                            str1 = "MMI查询（设置）LBJ工作状态" + NewLine;
                            switch (tmparray1[tHead])
                            {
                                case 1:
                                    str1 += "客列尾模式；" + NewLine;
                                    break;
                                case 2:
                                    str1 += "800M列尾模式；" + NewLine;
                                    break;
                                default:
                                    str1 += "不在协议内；" + NewLine;
                                    break;
                            }
                            switch (tmparray1[tHead + 1])
                            {
                                case 1:
                                    str1 += "双端机车A段；" + NewLine;
                                    break;
                                case 2:
                                    str1 += "双端机车B段；" + NewLine;
                                    break;
                                case 0:
                                    str1 += "普通机车或动车组；" + NewLine;
                                    break;
                                default:
                                    str1 += "不在协议内；" + NewLine;
                                    break;
                            }
                            return str1;
                            #endregion

                        case 7:                              //BQ:5->7
                            return "MMI通知LBJ自检；";

                        case 9:
                            return "查询LBJ 功能单元软件版本";

                        case 10:
                            return str1;

                        case 11:
                            #region
                            str1 = "操作显示终端在出入库检测状态下发送按键信息：" + NewLine;
                            switch (tmparray1[tHead])
                            {
                                case 1:
                                    return (str1 + "报警按键");
                                case 2:
                                    return (str1 + "列尾排风按键");
                                case 3:
                                    return (str1 + "风压查询按键");
                                case 5:
                                    return (str1 + "列尾确认按键");
                                case 6:
                                    return (str1 + "列尾消号按键");
                            }
                            return str1;
                            #endregion
                        case 0x12:
                            #region
                            str1 = "MMI向LBJ发送防护报警试验确认信息；" + NewLine;
                            string my_str4 = (tmparray1[tHead + 1]).ToString("X2") + (tmparray1[tHead]).ToString("X2");
                            str1 += ("库检设备编号：" + (my_str4 != "0000" ? my_str4 : "00")) + NewLine;
                            return str1;
                            #endregion
                        case 0x0C:
                            str1 = "MMI向LBJ发送出入库检请求命令；" + NewLine;
                            string my_str3 = (tmparray1[tHead + 1]).ToString("X2") + (tmparray1[tHead]).ToString("X2");
                            str1 += ("库检设备编号：" + (my_str3 != "0000" ? my_str3 : "00")) + NewLine;
                            return str1;
                        case 0x21:
                        case 0x27:                 //BQ:输号命令0x27,0x28协议中的sourceport=0x13,不应出现在上文if(sourceport!=0x13)中
                        case 0x28:                 //并且与0x21形式一致
                        case 0x22:
                        case 0x23:
                        case 0x24:
                        case 0x25:
                        case 0x26:
                            if (comstyle == 0x04)
                            {
                                if (command == 0x22) str1 = "排风制动应答" + NewLine;
                                else if (command == 0x24) str1 = "KLW供电电压欠压启动提示" + NewLine;
                                else if (command == 0x25) str1 = "输号命令" + NewLine;//无法运行到此处?               
                                else if (command == 0x26) str1 = "消号命令" + NewLine;  //同上  
                                else if (command == 0x21) str1 = "手动查询风压应答" + NewLine;
                                else if (command == 0x23) str1 = "KLW风压自动提示" + NewLine;
                                else if (command == 0x27) str1 = "输号应答" + NewLine;
                                else if (command == 0x28) str1 = "自动查询风压应答" + NewLine;
                                jch_l = (int)((((BCDToHex((ulong)tmparray1[tHead], 2) * 0xf4240) + (BCDToHex((ulong)tmparray1[tHead + 1], 2) * 0x2710)) + (BCDToHex((ulong)tmparray1[tHead + 2], 2) * 100)) + BCDToHex((ulong)tmparray1[tHead + 3], 2));
                                id_l = (int)(((BCDToHex((ulong)tmparray1[tHead + 4], 2) * 0x2710) + (BCDToHex((ulong)tmparray1[tHead + 5], 2) * 100)) + BCDToHex((ulong)tmparray1[tHead + 6], 2));
                                str1 += "列尾机车号：" + jch_l.ToString() + NewLine + "KLW ID：" + id_l.ToString() + NewLine;
                                if (command == 0x21 || command == 0x23 || command == 0x27 || command == 0x28)      //加上风压信息
                                {
                                    press_l = (int)((BCDToHex((ulong)tmparray1[tHead + 7], 2) * 100) + BCDToHex((ulong)tmparray1[tHead + 8], 2));
                                    str1 = str1 + "风压：" + press_l.ToString() + NewLine;
                                }
                            }
                            return str1;
                    }
                }

                if (CSt4t0002 <= 0x48)
                {
                    switch (CSt4t0002)
                    {
                        case 0x41:
                            return ("应答数据" + NewLine);

                        case 0x48:
                            return "主机向LBJ发送GPS信息";
                    }
                    return str1;
                }
                switch (CSt4t0002)
                {
                    case 0x4A:
                        #region
                        if (tmparray1[tHead] == 0xF1)
                        {
                            str1 = "MMI通知LBJ进入单项测试状态；" + NewLine;
                            switch (tmparray1[tHead + 1])
                            {
                                case 0x02:
                                    str1 += "发射821.2375MHz" + NewLine;
                                    break;
                                case 0x03:
                                    str1 += "发射866.2375MHz" + NewLine;
                                    break;
                                case 0x04:
                                    str1 += "发射821.2375MHz + POCSAG" + NewLine;
                                    break;
                                case 0x06:
                                    str1 += "发射866.2375MHz + 1200" + NewLine;
                                    break;
                                case 0x08:
                                    str1 += "发射866.2375MHz + 1800" + NewLine;
                                    break;
                                default:
                                    str1 += "不在协议内；" + NewLine;
                                    break;
                            }
                        }
                        else if (tmparray1[tHead] == 0xF2)
                        {
                            str1 += "MMI通知LBJ退出单项测试状态；" + NewLine;

                        }
                        else if (tmparray1[tHead] == 0x00)
                        {
                            str1 += "MMI通知LBJ退出测试状态；" + NewLine;

                        }
                        else if (tmparray1[tHead] == 0xFF)
                        {
                            str1 += "MMI通知LBJ进入测试状态；" + NewLine;

                        }
                        return str1;
                        #endregion
                    case 0x55:
                        return GetInformationString(tmparray1, tHead);
                    //0xe2~0xe4未验证!
                    //case 0xe2: return str1 = 记录信息类别代码(command, tmparray1, tHead, tmparray1[tHead]);
                    case 0xe3:
                        #region
                        str1 = "MMI向LBJ发送参数设置命令：" + NewLine;
                        switch (tmparray1[tHead])
                        {
                            case 0x03:
                                str1 += "设置列车接近预警功能参数：" + NewLine; ;
                                switch (tmparray1[tHead + 1])
                                {
                                    case 0x01: str1 += "列车接近预警功能开启" + NewLine; break;
                                    case 0x02: str1 += "列车接近预警功能关闭" + NewLine; break;
                                    default: str1 += "检查记录是否有误!" + NewLine; break;
                                }
                                break;
                            case 0x04:
                                str1 += "设置列车防护报警功能参数：" + NewLine; ;
                                switch (tmparray1[tHead + 1])
                                {
                                    case 0x01: str1 += "列车防护报警功能开启" + NewLine; break;
                                    case 0x02: str1 += "列车防护报警功能关闭" + NewLine; break;
                                    default: str1 += "检查记录是否有误!" + NewLine; break;
                                }
                                break;
                            default: str1 += "检查记录是否有误!" + NewLine; break;
                        }
                        return str1;
                        #endregion

                    case 0xe5:
                        #region
                        str1 = "操作显示终端接收到报警信息后发送确认按键信息" + NewLine;
                        switch (tmparray1[tHead])
                        {
                            case 1:
                                str1 = str1 + "列车防护报警" + NewLine;
                                goto Label_0F26;

                            case 2:
                            case 4:
                                goto Label_0F26;

                            case 3:
                                str1 = str1 + "道口事故报警" + NewLine;
                                goto Label_0F26;

                            case 5:
                                str1 = str1 + "施工防护报警" + NewLine;
                                goto Label_0F26;
                        }
                        goto Label_0F26;
                }
                return str1;
                        #endregion
            }
            CSt4t0002 = command;
            if (comstyle == 0x0B)
            {
                #region
                switch (CSt4t0002)
                {
                    case 0x21:
                    case 0x22:
                    case 0x23:
                    case 0x24:
                    case 0x25:
                    case 0x26:
                        if (command == 0x21) str1 = "LBJ设备（单元）向MMI回送最新10条发送的报警信息：" + NewLine;
                        else if (command == 0x22) str1 = "LBJ设备（单元）向MMI回送前10条发送的报警信息：" + NewLine;
                        else if (command == 0x23) str1 = "LBJ设备（单元）向MMI回送后10条发送的报警信息：" + NewLine;
                        else if (command == 0x24) str1 = "LBJ设备（单元）向MMI回送最新10条发送的报警信息：" + NewLine;
                        else if (command == 0x25) str1 = "LBJ设备（单元）向MMI回送前10条接收的报警信息：" + NewLine;
                        else if (command == 0x26) str1 = "LBJ设备（单元）向MMI回送后10条接收的报警信息：" + NewLine;
                        for (int my_jj = 0; my_jj < 10; my_jj++)
                        {
                            if (my_jj < 9)
                            {
                                str1 = str1 + string.Format("\r\n第{0}条报警信息为：\r\n", (my_jj + 1).ToString("X2"));
                            }
                            else if (my_jj == 9)
                            {
                                str1 += "\r\n第10条报警信息为：\r\n";
                            }
                            if (tmparray1[tHead + my_jj * 30] == 0x01)
                            {
                                #region
                                str1 += "列车防护报警信息:" + NewLine;
                                cchtype = Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 1, 4).Trim();
                                cchnum = (tmparray1[tHead + my_jj * 30 + 5] + (tmparray1[tHead + my_jj * 30 + 6] << 8)) + (tmparray1[tHead + my_jj * 30 + 7] << 0x10);
                                string my_jch = tmparray1[tHead + my_jj * 30 + 8].ToString("X2") + tmparray1[tHead + my_jj * 30 + 9].ToString("X2") + tmparray1[tHead + my_jj * 30 + 10].ToString("X2") + tmparray1[tHead + my_jj * 30 + 11].ToString("X2") + NewLine;
                                str1 += ("车次号：" + cchtype + cchnum.ToString() + NewLine) + "机车号：" + my_jch;
                                numgong = (tmparray1[tHead + my_jj * 30 + 12] + (tmparray1[tHead + my_jj * 30 + 13] << 8)) + ((tmparray1[tHead + my_jj * 30 + 14] & 0x3f) << 0x10);
                                gonglibiao = ((float)(numgong / 100)) / 10f;
                                if ((tmparray1[tHead + my_jj * 30 + 14] & 0x80) == 0x80)
                                {
                                    gonglibiao = -1f * gonglibiao;
                                }
                                str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
                                if ((tmparray1[tHead + my_jj * 30 + 14] & 0x40) == 0x40)
                                {
                                    str1 += "公里标递增" + NewLine;
                                }
                                else
                                {
                                    str1 += "公里标递减" + NewLine;
                                }
                                timelong = ((tmparray1[tHead + my_jj * 30 + 15] + (tmparray1[tHead + my_jj * 30 + 16] << 8)) + (tmparray1[tHead + my_jj * 30 + 17] << 0x10)) + (tmparray1[tHead + my_jj * 30 + 18] << 0x18);
                                sec = timelong & 0x3f;
                                min = (timelong >> 6) & 0x3f;
                                hour = (timelong >> 12) & 0x1f;
                                day = (timelong >> 0x11) & 0x1f;
                                mon = (timelong >> 0x16) & 15;
                                year = (timelong >> 0x1a) & 0x3f;
                                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                                str1 = str1 + "启动报警原因：";
                                switch (tmparray1[tHead + my_jj * 30 + 19])
                                {
                                    case 1:
                                        str1 = str1 + "控制盒1 或MMI1 触发" + NewLine;
                                        break;

                                    case 2:
                                        str1 = str1 + "控制盒2 或MMI2 触发" + NewLine;
                                        break;

                                    case 3:
                                        str1 = str1 + "主机面板按键触发" + NewLine;
                                        break;

                                    case 11:
                                        str1 = str1 + "控制盒1或MMI1触发，启动失败（速度不为0）" + NewLine;
                                        break;

                                    case 12:
                                        str1 = str1 + "控制盒2或MMI2触发，启动失败（速度不为0）" + NewLine;
                                        break;

                                    case 13:
                                        str1 = str1 + "主机面板按键触发，启动失效（速度不为0）" + NewLine;
                                        break;
                                    default:
                                        str1 += "不在协议内；" + NewLine;
                                        break;
                                }
                                str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 20, 8) + NewLine;
                                #endregion
                            }
                            else if (tmparray1[tHead + my_jj * 30] == 0x02)
                            {
                                #region
                                str1 += "列车防护报警解除信息：" + NewLine;
                                cchtype = Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 1, 4).Trim();
                                cchnum = (tmparray1[tHead + my_jj * 30 + 5] + (tmparray1[tHead + my_jj * 30 + 6] << 8)) + (tmparray1[tHead + my_jj * 30 + 7] << 0x10);
                                string my_jch = tmparray1[tHead + my_jj * 30 + 8].ToString("X2") + tmparray1[tHead + my_jj * 30 + 9].ToString("X2") + tmparray1[tHead + my_jj * 30 + 10].ToString("X2") + tmparray1[tHead + my_jj * 30 + 11].ToString("X2") + NewLine;
                                str1 += ("车次号：" + cchtype + cchnum.ToString() + NewLine) + "机车号：" + my_jch;
                                numgong = (tmparray1[tHead + my_jj * 30 + 12] + (tmparray1[tHead + my_jj * 30 + 13] << 8)) + ((tmparray1[tHead + my_jj * 30 + 14] & 0x3f) << 0x10);
                                gonglibiao = ((float)(numgong / 100)) / 10f;
                                if ((tmparray1[tHead + my_jj * 30 + 14] & 0x80) == 0x80)
                                {
                                    gonglibiao = -1f * gonglibiao;
                                }
                                str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
                                if ((tmparray1[tHead + my_jj * 30 + 14] & 0x40) == 0x40)
                                {
                                    str1 += "公里标递增" + NewLine;
                                }
                                else
                                {
                                    str1 += "公里标递减" + NewLine;
                                }
                                timelong = ((tmparray1[tHead + my_jj * 30 + 15] + (tmparray1[tHead + my_jj * 30 + 16] << 8)) + (tmparray1[tHead + my_jj * 30 + 17] << 0x10)) + (tmparray1[tHead + my_jj * 30 + 18] << 0x18);
                                sec = timelong & 0x3f;
                                min = (timelong >> 6) & 0x3f;
                                hour = (timelong >> 12) & 0x1f;
                                day = (timelong >> 0x11) & 0x1f;
                                mon = (timelong >> 0x16) & 15;
                                year = (timelong >> 0x1a) & 0x3f;
                                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                                str1 += "解除报警原因：";
                                switch (tmparray1[tHead + my_jj * 30 + 19])
                                {
                                    case 1:
                                        str1 = str1 + "控制盒1 或MMI1 触发" + NewLine;
                                        break;
                                    case 2:
                                        str1 = str1 + "控制盒2 或MMI2 触发" + NewLine;
                                        break;
                                    case 3:
                                        str1 = str1 + "主机面板按键触发" + NewLine;
                                        break;
                                    case 4:
                                        str1 = str1 + "30s收不到报警信息" + NewLine;
                                        break;
                                    case 5:
                                        str1 = str1 + "列车启动，自动停发" + NewLine;
                                        break;
                                    default:
                                        str1 += "不在协议内；" + NewLine;
                                        break;
                                }
                                str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 20, 8) + NewLine;
                                #endregion
                            }
                            else if (tmparray1[tHead + my_jj * 30] == 0x03)
                            {
                                #region
                                str1 += "道口事故报警信息：" + NewLine;
                                numgong = (tmparray1[tHead + my_jj * 30 + 1] + (tmparray1[tHead + my_jj * 30 + 2] << 8)) + ((tmparray1[tHead + my_jj * 30 + 3] & 0x3f) << 0x10);
                                gonglibiao = ((float)(numgong / 100)) / 10f;
                                if ((tmparray1[tHead + my_jj * 30 + 3] & 0x80) == 0x80)
                                {
                                    gonglibiao = -1f * gonglibiao;
                                }
                                str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
                                timelong = ((tmparray1[tHead + my_jj * 30 + 4] + (tmparray1[tHead + my_jj * 30 + 5] << 8)) + (tmparray1[tHead + my_jj * 30 + 6] << 0x10)) + (tmparray1[tHead + my_jj * 30 + 7] << 0x18);
                                sec = timelong & 0x3f;
                                min = (timelong >> 6) & 0x3f;
                                hour = (timelong >> 12) & 0x1f;
                                day = (timelong >> 0x11) & 0x1f;
                                mon = (timelong >> 0x16) & 15;
                                year = (timelong >> 0x1a) & 0x3f;
                                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                                str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 9, 8) + NewLine;
                                #endregion
                            }
                            else if (tmparray1[tHead + my_jj * 30] == 0x04)
                            {
                                #region
                                str1 += "道口事故报警解除信息：" + NewLine;
                                numgong = (tmparray1[tHead + my_jj * 30 + 1] + (tmparray1[tHead + my_jj * 30 + 2] << 8)) + ((tmparray1[tHead + my_jj * 30 + 3] & 0x3f) << 0x10);
                                gonglibiao = ((float)(numgong / 100)) / 10f;
                                if ((tmparray1[tHead + my_jj * 30 + 3] & 0x80) == 0x80)
                                {
                                    gonglibiao = -1f * gonglibiao;
                                }
                                str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
                                timelong = ((tmparray1[tHead + my_jj * 30 + 4] + (tmparray1[tHead + my_jj * 30 + 5] << 8)) + (tmparray1[tHead + my_jj * 30 + 6] << 0x10)) + (tmparray1[tHead + my_jj * 30 + 7] << 0x18);
                                sec = timelong & 0x3f;
                                min = (timelong >> 6) & 0x3f;
                                hour = (timelong >> 12) & 0x1f;
                                day = (timelong >> 0x11) & 0x1f;
                                mon = (timelong >> 0x16) & 15;
                                year = (timelong >> 0x1a) & 0x3f;
                                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                                str1 += "解除报警原因：";
                                switch (tmparray1[tHead + my_jj * 30 + 8])
                                {
                                    case 1:
                                        str1 = str1 + "道口事故报警解除" + NewLine;
                                        break;
                                    case 4:
                                        str1 = str1 + "30s收不到报警信息" + NewLine;
                                        break;
                                    default:
                                        str1 += "不在协议内；" + NewLine;
                                        break;
                                }
                                str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 9, 8) + NewLine;
                                #endregion
                            }
                            else if (tmparray1[tHead + my_jj * 30] == 0x05)
                            {
                                #region
                                str1 += "施工防护报警信息：" + NewLine;
                                numgong = (tmparray1[tHead + my_jj * 30 + 1] + (tmparray1[tHead + my_jj * 30 + 2] << 8)) + ((tmparray1[tHead + my_jj * 30 + 3] & 0x3f) << 0x10);
                                gonglibiao = ((float)(numgong / 100)) / 10f;
                                if ((tmparray1[tHead + my_jj * 30 + 3] & 0x80) == 0x80)
                                {
                                    gonglibiao = -1f * gonglibiao;
                                }
                                str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
                                timelong = ((tmparray1[tHead + my_jj * 30 + 4] + (tmparray1[tHead + my_jj * 30 + 5] << 8)) + (tmparray1[tHead + my_jj * 30 + 6] << 0x10)) + (tmparray1[tHead + my_jj * 30 + 7] << 0x18);
                                sec = timelong & 0x3f;
                                min = (timelong >> 6) & 0x3f;
                                hour = (timelong >> 12) & 0x1f;
                                day = (timelong >> 0x11) & 0x1f;
                                mon = (timelong >> 0x16) & 15;
                                year = (timelong >> 0x1a) & 0x3f;
                                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                                str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 9, 8) + NewLine;
                                #endregion
                            }
                            else if (tmparray1[tHead + my_jj * 30] == 0x06)
                            {
                                #region
                                str1 += "施工防护报警解除信息：" + NewLine;
                                numgong = (tmparray1[tHead + my_jj * 30 + 1] + (tmparray1[tHead + my_jj * 30 + 2] << 8)) + ((tmparray1[tHead + my_jj * 30 + 3] & 0x3f) << 0x10);
                                gonglibiao = ((float)(numgong / 100)) / 10f;
                                if ((tmparray1[tHead + my_jj * 30 + 3] & 0x80) == 0x80)
                                {
                                    gonglibiao = -1f * gonglibiao;
                                }
                                str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
                                timelong = ((tmparray1[tHead + my_jj * 30 + 4] + (tmparray1[tHead + my_jj * 30 + 5] << 8)) + (tmparray1[tHead + my_jj * 30 + 6] << 0x10)) + (tmparray1[tHead + my_jj * 30 + 7] << 0x18);
                                sec = timelong & 0x3f;
                                min = (timelong >> 6) & 0x3f;
                                hour = (timelong >> 12) & 0x1f;
                                day = (timelong >> 0x11) & 0x1f;
                                mon = (timelong >> 0x16) & 15;
                                year = (timelong >> 0x1a) & 0x3f;
                                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                                str1 += "解除报警原因：";
                                switch (tmparray1[tHead + my_jj * 30 + 8])
                                {
                                    case 1:
                                        str1 = str1 + "施工防护报警解除" + NewLine;
                                        break;
                                    case 4:
                                        str1 = str1 + "30s收不到报警信息" + NewLine;
                                        break;
                                    default:
                                        str1 += "不在协议内；" + NewLine;
                                        break;
                                }
                                str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + my_jj * 30 + 9, 8) + NewLine;
                                #endregion
                            }
                            else
                            {
                                str1 += "检查数据是否有误！" + NewLine;
                            }
                        }
                        return str1;
                #endregion
                }

            }
            switch (CSt4t0002)
            {
                case 0xe4:
                    #region
                    str1 = "LBJ向MMI反馈设置结果：" + NewLine;
                    str1 += "机车号：" + tmparray1[tHead].ToString("X2") + tmparray1[tHead + 1].ToString("X2") + tmparray1[tHead + 2].ToString("X2") + tmparray1[tHead + 3].ToString("X2") + NewLine; ;
                    switch (tmparray1[tHead + 4])
                    {
                        case 0x01: str1 += "列车接近预警功能开启状态" + NewLine; break;
                        case 0x02: str1 += "列车接近预警功能关闭状态" + NewLine; break;
                        default: str1 += "检查记录是否有误!" + NewLine; break;
                    }
                    switch (tmparray1[tHead + 5])
                    {
                        case 0x01: str1 += "列车防护报警功能开启状态" + NewLine; break;
                        case 0x02: str1 += "列车防护报警功能关闭状态" + NewLine; break;
                        default: str1 += "检查记录是否有误!" + NewLine; break;
                    }
                    return str1;
                    #endregion
                case 0x04:
                    #region
                    str1 = "LBJ向MMI发送当前工作状态：" + NewLine;
                    switch (tmparray1[tHead] & 0x01)
                    {
                        case 0:
                            str1 += "未发送过报警信息； " + NewLine;
                            break;
                        case 1:
                            str1 += "已发送过报警信息； " + NewLine;
                            break;
                        default:
                            str1 += "检查记录是否有误! " + NewLine;
                            break;
                    }
                    switch (tmparray1[tHead] & 0x02)
                    {
                        case 0:
                            str1 += "守候状态； " + NewLine;
                            break;
                        case 0x02:
                            str1 += "正在发送报警信息； " + NewLine;
                            break;
                        default:
                            str1 += "检查记录是否有误!" + NewLine;
                            break;
                    }
                    switch (tmparray1[tHead] & 0x04)
                    {
                        case 0:
                            str1 += "未接收过报警信息； " + NewLine;
                            break;
                        case 0x04:
                            str1 += "已接收过报警信息； " + NewLine;
                            break;
                        default:
                            str1 += "检查记录是否有误! " + NewLine;
                            break;
                    }
                    str1 += NewLine;
                    switch (tmparray1[tHead + 1])
                    {
                        case 1:
                            str1 += "正常工作状态；" + NewLine; ;
                            break;
                        case 2:
                            str1 += "出入库检测状态； " + NewLine; ;
                            break;
                        default:
                            str1 += "检查记录是否有误! " + NewLine;
                            break;
                    }
                    switch (tmparray1[tHead + 2])
                    {
                        case 1:
                            str1 += "客车列尾已连接；" + NewLine; ;
                            break;
                        case 2:
                            str1 += "客车列尾未连接； " + NewLine; ;
                            break;
                        case 3:
                            str1 += "客车列尾通信失效； " + NewLine; ;
                            break;
                        case 4:
                            str1 += "800MHz列尾模式； " + NewLine; ;
                            break;
                        default:
                            str1 += "检查记录是否有误! " + NewLine;
                            break;
                    }
                    str1 += "机车号：" + tmparray1[tHead + 3].ToString("X2") + tmparray1[tHead + 4].ToString("X2") + tmparray1[tHead + 5].ToString("X2") + tmparray1[tHead + 6].ToString("X2") + NewLine;
                    //int jch = (int)((((BCDToHex((ulong)tmparray1[tHead + 3], 2) * 0xf4240) + (BCDToHex((ulong)tmparray1[tHead + 4], 2) * 0x2710)) + (BCDToHex((ulong)tmparray1[tHead + 5], 2) * 100)) + BCDToHex((ulong)tmparray1[tHead + 6], 2));
                    str1 = str1 + "KLW ID：";
                    str1 += ((int)(((BCDToHex((ulong)tmparray1[tHead + 7], 2) * 0x2710) + (BCDToHex((ulong)tmparray1[tHead + 8], 2) * 100)) + BCDToHex((ulong)tmparray1[tHead + 9], 2))).ToString() + NewLine;
                    str1 = str1 + "列尾风压：";
                    str1 += ((int)((BCDToHex((ulong)tmparray1[tHead + 10], 2) * 100) + BCDToHex((ulong)tmparray1[tHead + 11], 2))).ToString() + NewLine;
                    if (tmparray1[tHead + 12] != 0xff)
                    {
                        switch (tmparray1[tHead + 12])
                        {
                            case 1:
                                str1 += "启动报警原因：控制盒1或MMI1触发；" + NewLine;
                                break;
                            case 2:
                                str1 += "启动报警原因：控制盒2或MMI2触发；" + NewLine;
                                break;
                            case 3:
                                str1 += "启动报警原因：主机面板按键触发；" + NewLine;
                                break;
                            default:
                                str1 += "检查记录是否有误! " + NewLine;
                                break;
                        }
                    }
                    else str1 += "无效" + NewLine;

                    if (tmparray1[tHead + 13] != 0xff)
                    {
                        switch (tmparray1[tHead + 13])
                        {
                            case 1:
                                str1 += "列车接近预警功能打开；" + NewLine;
                                break;
                            case 2:
                                str1 += "列车接近预警功能关闭；" + NewLine;
                                break;
                            default:
                                str1 += "检查记录是否有误! " + NewLine;
                                break;
                        }
                    }
                    else str1 += "无效" + NewLine;

                    if (tmparray1[tHead + 14] != 0xff)
                    {
                        switch (tmparray1[tHead + 14])
                        {
                            case 1:
                                str1 += "列车防护报警功能打开；" + NewLine;
                                break;
                            case 2:
                                str1 += "列车防护报警功能关闭；" + NewLine;
                                break;
                            default:
                                str1 += "检查记录是否有误! " + NewLine;
                                break;
                        }
                    }
                    else str1 += "无效" + NewLine;

                    return str1;
                    #endregion
                case 0x4F:
                    #region
                    if ((tmparray1[tHead] == 0x4A) && (tmparray1[tHead + 1] == 0xFF))
                    {
                        str1 += "LBJ返回进入测试状态应答；" + NewLine;
                    }
                    if ((tmparray1[tHead] == 0x4A) && (tmparray1[tHead + 1] == 0x00))
                    {
                        str1 += "LBJ返回退出测试状态应答；" + NewLine;
                    }
                    if ((tmparray1[tHead] == 0x4A) && (tmparray1[tHead + 1] == 0xF1))
                    {
                        str1 += "LBJ返回单项进入测试状态应答；" + NewLine;
                        switch (tmparray1[tHead + 2])
                        {
                            case 0x02:
                                str1 += "发射821.2375MHz" + NewLine;
                                break;
                            case 0x03:
                                str1 += "发射866.2375MHz" + NewLine;
                                break;
                            case 0x04:
                                str1 += "发射821.2375MHz + POCSAG" + NewLine;
                                break;
                            case 0x06:
                                str1 += "发射866.2375MHz + 1200" + NewLine;
                                break;
                            case 0x08:
                                str1 += "发射866.2375MHz + 1800" + NewLine;
                                break;

                        }
                    }
                    if ((tmparray1[tHead] == 0x4A) && (tmparray1[tHead + 1] == 0xF2))
                    {
                        str1 += "LBJ返回单项退出测试状态应答；" + NewLine;
                    }
                    return str1;
                    #endregion
                case 0x03:
                    str1 = "LBJ向CIR发送综合信息查询命令；" + NewLine;
                    return str1;
                case 0x50:
                    str1 = "LBJ向MMI发送非停车状态不能发送报警信息；" + NewLine;
                    return str1;
                case 0x51:
                    str1 = "LBJ向MMI发送列车移动报警已自动解除提示信息；" + NewLine;
                    return str1;
                case 0x11:
                    {
                        #region
                        str1 += "命令解析：\r\n出入库状态发送列车防护报警试验信息\r\n";
                        cchtype = Encoding.Default.GetString(tmparray1, tHead + 1, 4).Trim();
                        cchnum = (tmparray1[tHead + 5] + (tmparray1[tHead + 6] << 8)) + (tmparray1[tHead + 7] << 0x10);
                        str1 += "车次号：" + cchtype + cchnum.ToString() + NewLine;
                        str1 += "机车号：" + tmparray1[tHead + 8].ToString("X2") + tmparray1[tHead + 9].ToString("X2") + tmparray1[tHead + 10].ToString("X2") + tmparray1[tHead + 11].ToString("X2") + NewLine;
                        str1 += "公里标：FFFFFF" + NewLine;
                        timelong = ((tmparray1[tHead + 15] + (tmparray1[tHead + 16] << 8)) + (tmparray1[tHead + 17] << 0x10)) + (tmparray1[tHead + 18] << 0x18);
                        sec = timelong & 0x3f;
                        min = (timelong >> 6) & 0x3f;
                        hour = (timelong >> 12) & 0x1f;
                        day = (timelong >> 0x11) & 0x1f;
                        mon = (timelong >> 0x16) & 15;
                        year = (timelong >> 0x1a) & 0x3f;
                        str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;

                        string my_str = (tmparray1[tHead + 20]).ToString("X2") + (tmparray1[tHead + 19]).ToString("X2");
                        str1 += ("库检设备编号：" + (my_str != "0000" ? my_str : "00")) + NewLine;
                        switch (tmparray1[tHead + 21])
                        {
                            case 1: str1 += "双端机车A段"; break;
                            case 2: str1 += "双端机车B段"; break;
                            case 0: str1 += "普通机车或动车组"; break;
                            default: str1 += "检查记录是否有误!" + NewLine; break;
                        }
                        return str1;
                        #endregion
                    }
                case 0x0E:
                    #region
                    str1 = "LBJ向MMI发送出入库检测结果；" + NewLine;
                    str1 += "机车号：" + tmparray1[tHead + 1].ToString("X2") + tmparray1[tHead + 2].ToString("X2") + tmparray1[tHead + 3].ToString("X2") + tmparray1[tHead + 4].ToString("X2") + NewLine;
                    str1 += "控制盒端口：";
                    switch (tmparray1[tHead + 5])
                    {
                        case 3: str1 += "控制盒1或MMI1" + NewLine; break;
                        case 4: str1 += "控制盒2或MMI2" + NewLine; break;
                        case 0xff: str1 += "无效" + NewLine; break;
                        default: str1 += "协议之外的数据!" + NewLine; break;
                    }
                    string my_str15 = (tmparray1[tHead + 7]).ToString("X2") + (tmparray1[tHead + 6]).ToString("X2");
                    str1 += ("库检设备编号：" + (my_str15 != "0000" ? my_str15 : "00")) + NewLine;

                    str1 += "\r\n出入库检测结果：\r\n";
                    str1 += "TAX装置/DMS设备连接连接状态：" + ((tmparray1[tHead + 8] & 0x80) == 0x80 ? "故障" : "正常") + NewLine;
                    str1 += "记录单元状态：" + ((tmparray1[tHead + 8] & 0x40) == 0x40 ? "故障" : "正常") + NewLine;
                    str1 += "信道机状态：" + ((tmparray1[tHead + 8] & 0x20) == 0x20 ? "故障" : "正常") + NewLine;
                    str1 += "电池状态：" + ((tmparray1[tHead + 8] & 0x10) == 0x10 ? "故障" : "正常") + NewLine;
                    str1 += "备用电池电压：" + ((tmparray1[tHead + 9] == 0) ? "无备用电池" : (tmparray1[tHead + 9] / 10 + "." + (tmparray1[tHead + 9] - tmparray1[tHead + 9] / 10 * 10).ToString())) + NewLine;
                    if (tmparray1[tHead + 10] != 0xFF)
                    {
                        str1 += "防护报警信息接收试验：" + ((tmparray1[tHead + 10] & 0x01) == 0x01 ? "故障" : "正常") + NewLine;
                        str1 += "防护报警信息发送试验：" + ((tmparray1[tHead + 10] & 0x02) == 0x02 ? "故障" : "正常") + NewLine;
                        str1 += "接收预警信息发送试验：" + ((tmparray1[tHead + 10] & 0x04) == 0x04 ? "故障" : "正常") + NewLine;
                    }
                    else
                        str1 += "无效；" + NewLine;
                    str1 += "报警按键状态：" + ((tmparray1[tHead + 11] & 0x10) == 0x10 ? "故障" : "正常") + NewLine;
                    str1 += "列尾排风按键状态：" + ((tmparray1[tHead + 11] & 0x08) == 0x08 ? "故障" : "正常") + NewLine;
                    str1 += "查询风压按键状态：" + ((tmparray1[tHead + 11] & 0x04) == 0x04 ? "故障" : "正常") + NewLine;
                    str1 += "列尾确认按键状态：" + ((tmparray1[tHead + 11] & 0x02) == 0x02 ? "故障" : "正常") + NewLine;
                    str1 += "列尾消号按键状态：" + ((tmparray1[tHead + 11] & 0x01) == 0x01 ? "故障" : "正常") + NewLine;

                    str1 += "机车端号：\r\n";
                    switch (tmparray1[tHead + 12])
                    {
                        case 0:
                            str1 += "普通机车或动车组" + NewLine;
                            break;
                        case 1:
                            str1 += "双端机车A段" + NewLine;
                            break;
                        case 2:
                            str1 += "双端机车B段" + NewLine;
                            break;
                        default:
                            str1 += "" + NewLine;
                            break;
                    }
                    return str1;
                case 0x0F:
                    str1 = "LBJ通知MMI出入库检测失败；" + NewLine;
                    return str1;

                case 0x01:
                    {
                        #region
                        str1 += "LBJ向操作显示终端发送报警信息：" + NewLine;
                        if (tmparray1[tHead] == 1)
                        {
                            #region
                            str1 += "列车防护报警信息:" + NewLine;
                            str1 = str1 + "启动报警原因：";
                            switch (tmparray1[tHead + 19])
                            {
                                case 1:
                                    str1 = str1 + "控制盒1 或MMI1 触发" + NewLine;
                                    break;

                                case 2:
                                    str1 = str1 + "控制盒2 或MMI2 触发" + NewLine;
                                    break;

                                case 3:
                                    str1 = str1 + "主机面板按键触发" + NewLine;
                                    break;

                                case 11:
                                    str1 = str1 + "控制盒1或MMI1触发，启动失败（速度不为0）" + NewLine;
                                    break;

                                case 12:
                                    str1 = str1 + "控制盒2或MMI2触发，启动失败（速度不为0）" + NewLine;
                                    break;

                                case 13:
                                    str1 = str1 + "主机面板按键触发，启动失效（速度不为0）" + NewLine;
                                    break;
                                default:
                                    str1 += "不在协议内；" + NewLine;
                                    break;

                            }
                            str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + 20, 8) + NewLine;

                            goto Label_0F27;
                            #endregion
                        }
                        if (tmparray1[tHead] == 2)
                        {
                            #region
                            str1 += "列车防护报警解除信息：" + NewLine;
                            str1 += "解除报警原因：";
                            switch (tmparray1[tHead + 19])
                            {
                                case 1:
                                    str1 = str1 + "控制盒1 或MMI1 触发" + NewLine;
                                    break;

                                case 2:
                                    str1 = str1 + "控制盒2 或MMI2 触发" + NewLine;
                                    break;

                                case 3:
                                    str1 = str1 + "主机面板按键触发" + NewLine;
                                    break;

                                case 4:
                                    str1 = str1 + "30s收不到报警信息" + NewLine;
                                    break;

                                case 5:
                                    str1 = str1 + "列车启动，自动停发" + NewLine;
                                    break;

                                default:
                                    str1 += "不在协议内；" + NewLine;
                                    break;
                            }
                            str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + 20, 8) + NewLine;

                            goto Label_0F27;
                            #endregion
                        }
                        if (tmparray1[tHead] == 3)
                        {
                            str1 += "道口事故报警信息：" + NewLine;
                            goto Label_0F28;
                        }

                        if (tmparray1[tHead] == 4)
                        {
                            #region
                            str1 += "道口事故报警解除信息：" + NewLine;

                            str1 += "解除报警原因：";
                            switch (tmparray1[tHead + 8])
                            {
                                case 1:
                                    str1 += "道口事故报警原因解除；" + NewLine;
                                    break;
                                case 2:
                                    str1 += "30s收不到报警信息；" + NewLine;
                                    break;
                                default:
                                    str1 += "不在协议内；" + NewLine;
                                    break;
                            }
                            goto Label_0F28;
                            #endregion
                        }

                        if (tmparray1[tHead] == 5)
                        {

                            #region
                            str1 += "施工防护报警信息：" + NewLine;
                            goto Label_0F28;
                            #endregion
                        }
                        if (tmparray1[tHead] == 6)
                        {
                            #region
                            str1 += "施工防护报警解除信息：" + NewLine;
                            str1 += "解除报警原因：";
                            switch (tmparray1[tHead + 8])
                            {
                                case 1:
                                    str1 += "施工防护报警解除；" + NewLine;
                                    break;
                                case 4:
                                    str1 += "30s收不到报警信息；" + NewLine;
                                    break;
                                default:
                                    str1 += "不在协议内；" + NewLine;
                                    break;
                            }
                            goto Label_0F28;
                            #endregion
                        }
                        return str1;
                        #endregion
                    }
                    #endregion
                case 5:
                    #region
                    str1 = "LBJ 向操作显示终端发送语音提示命令" + NewLine;
                    if (tmparray1[tHead] != 1)
                    {
                        return (str1 + "列尾连接失败提示" + NewLine);
                    }
                    return (str1 + "断电提示" + NewLine);
                    #endregion
                case 8:
                    {
                        #region
                        str1 = "LBJ 功能单元向CIR主机或操作显示终端报告自检结果" + NewLine;
                        if ((tmparray1[tHead] & 0x80) != 0)
                        {
                            str1 = str1 + "TAX箱连接状态故障" + NewLine;
                        }
                        else
                        {
                            str1 = str1 + "TAX箱连接状态正常" + NewLine;
                        }
                        if ((tmparray1[tHead] & 0x40) == 0)
                        {
                            str1 = str1 + "记录单元状态正常" + NewLine;
                        }
                        else
                        {
                            str1 = str1 + "记录单元状态故障" + NewLine;
                        }
                        if ((tmparray1[tHead] & 0x20) == 0)
                        {
                            str1 = str1 + "信道机状态正常" + NewLine;
                        }
                        else
                        {
                            str1 = str1 + "信道机状态故障" + NewLine;
                        }
                        if (tmparray1[tHead + 1] == 0)
                        {
                            return (str1 + "无备用电池");
                        }
                        float numfd = ((float)tmparray1[tHead + 1]) / 10f;
                        return (str1 + "备用电池电压：" + numfd.ToString());
                        #endregion
                    }
                case 9:
                    return str1;
                case 10:
                    {
                        #region
                        str1 = "LBJ 功能单元报告软件版本" + NewLine;
                        if (tLen > 1)
                        {
                            str1 = str1 + Encoding.Default.GetString(tmparray1, tHead, tLen - 2);
                        }
                        string my_str1 = Encoding.Default.GetString(tmparray1, tHead + tLen - 1, 1);
                        int ww = Convert.ToInt16(my_str1);

                        string[] manufacturer = { "希电公司", "天津通广", "思科泰", "杭州创联", "新干通", "泉州铁通", "兰新集团", "深圳长龙", "上海复旦", "北京华铁", "北京和利时", "北京世纪东方", "河南辉煌" };
                        if ((ww > 0) && (ww < 13))
                        {
                            str1 += manufacturer[ww - 1];
                        }
                        else
                            str1 += "其他厂家";
                        return str1;
                        #endregion
                    }
                case 0x21:
                case 0x27:                 //BQ:输号命令0x27,0x28协议中的sourceport=0x13,不应出现在上文if(sourceport!=0x13)中
                case 0x28:                 //并且与0x21形式一致
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x25:
                case 0x26:
                    #region
                    if (command == 0x22) str1 = "排风制动应答" + NewLine;
                    else if (command == 0x24) str1 = "KLW供电电压欠压启动提示" + NewLine;
                    else if (command == 0x25) str1 = "输号命令" + NewLine;//无法运行到此处?               
                    else if (command == 0x26) str1 = "消号命令" + NewLine;  //同上  
                    else if (command == 0x21) str1 = "手动查询风压应答" + NewLine;
                    else if (command == 0x23) str1 = "KLW风压自动提示" + NewLine;
                    else if (command == 0x27) str1 = "输号应答" + NewLine;
                    else if (command == 0x28) str1 = "自动查询风压应答" + NewLine;
                    jch_l = (int)((((BCDToHex((ulong)tmparray1[tHead], 2) * 0xf4240) + (BCDToHex((ulong)tmparray1[tHead + 1], 2) * 0x2710)) + (BCDToHex((ulong)tmparray1[tHead + 2], 2) * 100)) + BCDToHex((ulong)tmparray1[tHead + 3], 2));
                    id_l = (int)(((BCDToHex((ulong)tmparray1[tHead + 4], 2) * 0x2710) + (BCDToHex((ulong)tmparray1[tHead + 5], 2) * 100)) + BCDToHex((ulong)tmparray1[tHead + 6], 2));
                    str1 += "列尾机车号：" + jch_l.ToString() + NewLine + "KLW ID：" + id_l.ToString() + NewLine;
                    if (command == 0x21 || command == 0x23 || command == 0x27 || command == 0x28)      //加上风压信息
                    {
                        press_l = (int)((BCDToHex((ulong)tmparray1[tHead + 7], 2) * 100) + BCDToHex((ulong)tmparray1[tHead + 8], 2));
                        str1 = str1 + "风压：" + press_l.ToString() + NewLine;
                    }
                    return str1;
                    #endregion
                case 0x41:
                    return ("应答数据" + NewLine);

                default:
                    return str1;
            }

        Label_0F26:
            #region
            numgong = (tmparray1[tHead + 1] + (tmparray1[tHead + 2] << 8)) + ((tmparray1[tHead + 3] & 0x3f) << 0x10);
            gonglibiao = ((float)(numgong / 100)) / 10f;
            if ((tmparray1[tHead + 3] & 0x80) == 0x80)
            {
                gonglibiao = -1f * gonglibiao;
            }
            return (str1 + "公里标：" + gonglibiao.ToString() + "公里");
            #endregion

        Label_0F27:
            #region
            //my_cch1 = Encoding.Default.GetString(tmparray1, tHead + 1, 7);
            cchtype = Encoding.Default.GetString(tmparray1, tHead + 1, 4).Trim();
            cchnum = (tmparray1[tHead + 5] + (tmparray1[tHead + 6] << 8)) + (tmparray1[tHead + 7] << 0x10);
            string my_jch8 = tmparray1[tHead + 8].ToString("X2") + tmparray1[tHead + 9].ToString("X2") + tmparray1[tHead + 10].ToString("X2") + tmparray1[tHead + 11].ToString("X2") + NewLine;
            str1 += ("车次号：" + cchtype + cchnum.ToString() + NewLine) + "机车号：" + my_jch8 + NewLine;
            numgong = (tmparray1[tHead + 12] + (tmparray1[tHead + 13] << 8)) + ((tmparray1[tHead + 14] & 0x3f) << 0x10);
            gonglibiao = ((float)(numgong / 100)) / 10f;
            if ((tmparray1[tHead + 14] & 0x80) == 0x80)
            {
                gonglibiao = -1f * gonglibiao;
            }
            str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
            timelong = ((tmparray1[tHead + 15] + (tmparray1[tHead + 16] << 8)) + (tmparray1[tHead + 17] << 0x10)) + (tmparray1[tHead + 18] << 0x18);
            sec = timelong & 0x3f;
            min = (timelong >> 6) & 0x3f;
            hour = (timelong >> 12) & 0x1f;
            day = (timelong >> 0x11) & 0x1f;
            mon = (timelong >> 0x16) & 15;
            year = (timelong >> 0x1a) & 0x3f;
            str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
            return str1;
            #endregion
        Label_0F28:
            #region
            //tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            numgong = (tmparray1[tHead + 1] + (tmparray1[tHead + 2] << 8)) + ((tmparray1[tHead + 3] & 0x3f) << 0x10);
            gonglibiao = ((float)(numgong / 100)) / 10f;
            if ((tmparray1[tHead + 3] & 0x80) == 0x80)
            {
                gonglibiao = -1f * gonglibiao;
            }
            str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
            timelong = ((tmparray1[tHead + 4] + (tmparray1[tHead + 5] << 8)) + (tmparray1[tHead + 6] << 0x10)) + (tmparray1[tHead + 7] << 0x18);
            sec = timelong & 0x3f;
            min = (timelong >> 6) & 0x3f;
            hour = (timelong >> 12) & 0x1f;
            day = (timelong >> 0x11) & 0x1f;
            mon = (timelong >> 0x16) & 15;
            year = (timelong >> 0x1a) & 0x3f;
            str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;

            str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + 9, 8) + NewLine;
            return str1;
            #endregion
        }


        private static string ProcessMainTo450(int command, byte[] tmparray1, int tHead)
        {
            string str1 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            if (command == 0x20)
            {
                str1 = (("主机向450M发送机车号" + NewLine) + "命令解析：" + NewLine + "机车号：" + Encoding.Default.GetString(tmparray1, tHead, tLen - 1));
                return str1;  //tLen->tLen-1:去分号
            }
            if (command == 8)
            {
                string CSt0t0002 = "主机向450M发送工作模式" + NewLine + "命令解析：" + NewLine;
                return (CSt0t0002 + "工作模式：" /*+ tmparray1[tHead].ToString("X2") + ","*/ + GetModeString(tmparray1[tHead]));
            }
            if (command != 0xf1)
            {
                return str1;
            }
            str1 = "主机向450M发送GPS状态" + NewLine;
            if ((tmparray1[tHead] & 1) == 1)
            {
                str1 = str1 + "GPS状态：故障" + NewLine;
            }
            else
            {
                str1 = str1 + "GPS状态：正常" + NewLine;
            }
            if ((tmparray1[tHead] & 2) == 2)
            {
                return (str1 + "GPS数据：无效" + NewLine);
            }
            return (str1 + "GPS数据：有效" + NewLine);
        }

        private static string ProcessToDiaoDu(int command, byte[] tmparray1, int tHead)
        {
            string CSt0t0003;
            string str1 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            switch (command)
            {
                case 0x51:
                    str1 = "MMI向主机发送自动确认/签收/打印信息" + NewLine;
                    break;

                case 0x53:
                    str1 = "MMI向主机发送调车请求信息" + NewLine;
                    break;

                case 0x54:
                    str1 = "MMI向主机申请最新一条信息 " + NewLine;
                    break;

                case 0x55:
                    str1 = "MMI向主机申请上一条信息" + NewLine;
                    break;

                case 0x56:
                    str1 = "MMI向主机申请下一条信息" + NewLine;
                    break;

                case 0x58:
                    str1 = "MMI向主机申请最近10条调度命令目录" + NewLine;
                    break;

                case 0x59:
                    str1 = "MMI向主机申请最近10条行车凭证目录" + NewLine;
                    break;

                case 90:
                    str1 = "MMI向主机申请最近10条调车作业单目录" + NewLine;
                    break;

                case 0x5b:
                    str1 = "MMI向主机申请最近10条接车进路预告目录" + NewLine;
                    break;

                case 0x5d:
                    str1 = "MMI向主机申请前10条目录" + NewLine;
                    break;

                case 0x5e:
                    str1 = "MMI向主机申请后10条目录" + NewLine;
                    break;

                case 0x5f:
                    str1 = "MMI向主机申请某条信息" + NewLine;
                    break;

                case 0x61:
                    str1 = "MMI发送出入库测试请求" + NewLine;
                    break;

                case 140:
                    str1 = "擦除调度命令操作" + NewLine;
                    break;

                case 0x41:
                    str1 = "MMI应答" + NewLine;
                    break;
            }
            if (command == 0x5f)
            {
                str1 = str1 + "命令解析：" + NewLine;
                int command2 = tmparray1[tHead];
                ulong year2 = BCDToHex((ulong)tmparray1[tHead + 1], 2);
                ulong mon2 = BCDToHex((ulong)tmparray1[tHead + 2], 2);
                ulong day2 = BCDToHex((ulong)tmparray1[tHead + 3], 2);
                ulong hour2 = BCDToHex((ulong)tmparray1[tHead + 4], 2);
                ulong min2 = BCDToHex((ulong)tmparray1[tHead + 5], 2);
                ulong sec2 = BCDToHex((ulong)tmparray1[tHead + 6], 2);
                string commandid2 = Encoding.Default.GetString(tmparray1, tHead + 7, 6);
                str1 = str1 + "命令名称：";
                if ((command2 >= 1) && (command2 <= 12))
                {
                    str1 = str1 + CommandType[command2 - 1];
                }
                else if (command2 == 0x11)
                {
                    str1 = str1 + "调车作业单";
                }
                else if (command2 == 0x20)
                {
                    str1 = str1 + "出入库检测";
                }
                else if ((command2 >= 0x18) && (command2 <= 0x1f))
                {
                    str1 = str1 + "其它信息";
                }
                CSt0t0003 = str1;
                str1 = (CSt0t0003 + NewLine + "时间：20" + year2.ToString("00") + "-" + mon2.ToString("00") + "-" + day2.ToString("00") + " " + hour2.ToString("00") + ":"
                                                                                       + min2.ToString("00") + ":" + sec2.ToString("00") + NewLine) + "命令编号：" + commandid2 + NewLine;
            }
            if (((command != 0x51) && (command != 0x53)) && (command != 0x61))
            {
                return str1;
            }
            str1 = str1 + "命令解析：" + NewLine;
            switch (tmparray1[tHead])
            {
                case 0x80:
                    str1 = str1 + "信息名称：对入库检设备发送入库检请求命令" + NewLine;
                    break;

                case 0x81:
                    str1 = str1 + "信息名称：对调度命令的自动确认信息" + NewLine;
                    break;

                case 130:       //0x82
                    str1 = str1 + "信息名称：对调度命令的手动签收信息" + NewLine;
                    break;

                case 0x91:
                    str1 = str1 + "信息名称：对向DMIS发送调车请求命令" + NewLine;
                    break;

                case 0x71:
                    str1 = str1 + "信息名称：MMI将打印按键信息发送给主控单元" + NewLine;
                    break;

                default:
                    str1 = str1 + "信息名称：待定" + NewLine;
                    break;
            }
            int command1 = tmparray1[tHead + 1];
            ulong year = BCDToHex((ulong)tmparray1[tHead + 2], 2);
            ulong mon = BCDToHex((ulong)tmparray1[tHead + 3], 2);
            ulong day = BCDToHex((ulong)tmparray1[tHead + 4], 2);
            ulong hour = BCDToHex((ulong)tmparray1[tHead + 5], 2);
            ulong min = BCDToHex((ulong)tmparray1[tHead + 6], 2);
            ulong sec = BCDToHex((ulong)tmparray1[tHead + 7], 2);
            string cch = Encoding.Default.GetString(tmparray1, tHead + 8, 7);
            string jch = Encoding.Default.GetString(tmparray1, tHead + 15, 8);
            int house = tmparray1[tHead + 0x17] + (tmparray1[tHead + 0x2a] * 0x100);
            string commandid = Encoding.Default.GetString(tmparray1, tHead + 0x18, 6);
            str1 = str1 + "命令名称：";
            if ((command1 >= 1) && (command1 <= 12))
            {
                str1 = str1 + CommandType[command1 - 1];
            }
            else if (command1 == 0x11)
            {
                str1 = str1 + "调车作业单";
            }
            else if (command1 == 0x20)
            {
                str1 = str1 + "出入库检测";
            }
            else if ((command1 >= 0x18) && (command1 <= 0x1f))
            {
                str1 = str1 + "其它信息";
            }
            CSt0t0003 = str1;
            str1 = ((((CSt0t0003 + NewLine + "时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":"
                                                        + min.ToString("00") + ":" + sec.ToString("00") + NewLine)
                + "车次号：" + cch + NewLine) + "机车号：" + jch + NewLine) + "发令处所编号：" + house.ToString() + NewLine) + "命令编号：" + commandid + NewLine;
            if ((tmparray1[tHead + 30] == 0xff) && (tmparray1[tHead + 0x1f] == 0xff))
            {
                str1 = str1 + "公里标：无效" + NewLine;
            }
            else
            {
                long gonglibiao1 = (tmparray1[tHead + 30] + (tmparray1[tHead + 0x1f] * 0x100)) + (((tmparray1[tHead + 0x20] & 0x3f) * 0x100) * 0x100);
                if (gonglibiao1 == 0x98967f)
                {
                    str1 = str1 + "公里标：CIR处于编组站状态";
                }
                else
                {
                    if ((tmparray1[tHead + 0x20] & 0x80) == 0x80)
                    {
                        gonglibiao1 = -gonglibiao1;
                    }
                    str1 = str1 + "公里标：" + gonglibiao1.ToString() + "米" + NewLine;
                }
            }
            if ((tmparray1[tHead + 0x21] == 0xff) && (tmparray1[tHead + 0x22] == 0xff))
            {
                str1 = str1 + "经度：无效\r\n纬度：无效" + NewLine;
            }
            else
            {
                string longitude1 = BCDToHex((ulong)tmparray1[tHead + 0x21], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 0x22], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 0x23], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 0x24], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 0x25], 2).ToString("00") + "\"";
                string latitude1 = BCDToHex((ulong)tmparray1[tHead + 0x26], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 0x27], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 40], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 0x29], 2).ToString("00") + "\"";
                str1 = (str1 + "经度：" + longitude1 + NewLine) + "纬度：" + latitude1 + NewLine;
            }
            int totalnum = tmparray1[tHead + 0x2f];
            return (str1 + "包号：" + totalnum.ToString());
        }

        private static string ProcessToLieWei(int command, byte[] tmparray1, int tHead)     //MMI发送
        {
            int i;
            string str1 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            switch (command)
            {
                case 0x21:
                    str1 = "MMI向列尾主机传送风压查询命令" + NewLine;
                    break;

                case 0x22:
                    str1 = "MMI向列尾主机传送排风命令" + NewLine;
                    break;

                case 0x23:
                    str1 = "MMI向列尾主机传送风压报警确认信息" + NewLine;
                    break;

                case 0x24:
                    str1 = "MMI向列尾主机传送电压报警确认信息" + NewLine;
                    break;

                case 0x25:
                    str1 = "MMI向列尾主机传送建立对应关系申请信息" + NewLine;
                    break;

                case 0x26:
                    str1 = "MMI向列尾主机传送拆除对应关系确认信息" + NewLine;
                    break;

                case 40:
                    str1 = "MMI自动风压查询" + NewLine;
                    break;

                case 0x29:
                    str1 = "MMI向列尾建立对应关系启动" + NewLine;
                    break;

                case 0x2a:
                    str1 = "MMI拆除对应关系确认" + NewLine;
                    break;
            }
            if (command == 0x26)
            {
                str1 = str1 + "命令解析：" + NewLine;
                ulong jch3 = 0;
                for (i = 0; i < 4; i++)           //0开始4位:机车号
                {
                    jch3 = (jch3 * 100) + BCDToHex((ulong)tmparray1[i + tHead], 2);
                }
                ulong id3 = 0;
                //if (tmparray1[5] == 0)                                        //BQ:如果源端口(目的端口)地址长度为0,则直接返回.
                //{
                //    for (i = 0; i < 3; i++) id3 = (id3 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 4], 2);
                //    return str1 + "列尾jch：" + jch3.ToString() + NewLine + "列尾ID：" + id3.ToString() + NewLine;
                //}
                for (i = 0; i < 4; i++)                           //原版本软件ID只包含3字节,现改4字节
                {
                    id3 = (id3 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 0x11], 2);    //17开始3位:ID,8开始5位:经度,13开始4位:纬度
                }
                string longitude3 = BCDToHex((ulong)tmparray1[tHead + 8], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 9], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 12], 2).ToString("00") + "\"";
                string latitude3 = BCDToHex((ulong)tmparray1[tHead + 13], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 14], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 15], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 0x10], 2).ToString("00") + "\"";
                object CSt0t0005 = str1 + "列尾机车号：" + jch3.ToString() + NewLine;
                //BQ:0x26加入IP的解析
                string strIP = tmparray1[tHead + 4].ToString() + "." + tmparray1[tHead + 5].ToString() + "." + tmparray1[tHead + 6].ToString() + "." + tmparray1[tHead + 7].ToString();
                return ((string.Concat(new object[] { CSt0t0005, "列尾ID：", id3, NewLine }) + "经度：" + longitude3 + NewLine) + "纬度：" + latitude3 + /*加入IP"\r\n主机IP地址：" + strIP +*/ NewLine);
            }
            if ((command <= 0x20) || (command >= 0x27))
            {
                return str1;
            }
            str1 = str1 + "命令解析：" + NewLine;
            ulong jch1 = 0;
            for (i = 0; i < 4; i++)
            {
                jch1 = (jch1 * 100) + BCDToHex((ulong)tmparray1[i + tHead], 2);
            }
            ulong id1 = 0;
            if (tmparray1[5] == 0)                                        //BQ:如果源端口(目的端口)地址长度为0,则直接返回.
            {
                for (i = 0; i < 4; i++) id1 = (id1 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 4], 2);               //列尾ID原版本只包含3字节
                return str1 + "列尾机车号：" + jch1.ToString() + NewLine + "列尾ID：" + id1.ToString() + NewLine;
            }
            for (i = 0; i < 4; i++)
            {
                id1 = (id1 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 13], 2);
            }
            string longitude1 = BCDToHex((ulong)tmparray1[tHead + 4], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 5], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 6], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 7], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 8], 2).ToString("00") + "\"";
            string latitude1 = BCDToHex((ulong)tmparray1[tHead + 9], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 12], 2).ToString("00") + "\"";
            return ((((str1 + "列尾机车号：" + jch1.ToString() + NewLine) + "列尾ID：" + id1.ToString() + NewLine) + "经度：" + longitude1 + NewLine) + "纬度：" + latitude1 + NewLine);
        }

        private static string ProcessToMain(int command, byte[] tmparray1, int tHead)
        {
            string CSt0t0004;
            string str1 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            switch (command)
            {
                case 1:
                    return "MMI应答";
                case 2:
                    return ((("MMI向主机申请端口号" + NewLine) + "命令解析：" + NewLine) + "随机数：" + tmparray1[tHead].ToString());

                case 3:
                    return ("MMI向主机申请综合信息" + NewLine);

                case 4:
                    return ("MMI向主机报告关电源" + NewLine);

                case 5:
                    return "MMI请求主机自检";

                case 6:
                    str1 = ("设置本务机" + NewLine) + "命令解析：" + NewLine;
                    if (tmparray1[tHead] != 0)
                    {
                        return (str1 + "设置为补机" + tmparray1[tHead].ToString() + NewLine);
                    }
                    return (str1 + "设置为本务机" + NewLine);

                case 7:
                case 12:
                case 13:
                case 14:
                case 15:
                case 0x10:
                case 0x12:
                case 20:
                case 0x1a:
                case 0x1b:
                case 0x1c:
                case 0x1d:
                case 30:
                case 0x1f:
                    return str1;

                case 8:
                    return ((("MMI选择工作模式" + NewLine) + "命令解析：" + NewLine) + "工作模式：" + (GetModeString(tmparray1[tHead]) == NewLine ? (tmparray1[tHead].ToString() + NewLine) : GetModeString(tmparray1[tHead])) + Encoding.Default.GetString(tmparray1, tHead + 1, tLen - 2));

                case 9:
                    str1 = ("MMI摘/挂机" + NewLine) + "命令解析：" + NewLine;
                    if (tmparray1[tHead] != 0)
                    {
                        return (str1 + "摘机" + NewLine);
                    }
                    return (str1 + "挂机" + NewLine);

                case 10:
                    str1 = ("MMI(PTT)操作" + NewLine) + "命令解析：" + NewLine;
                    if (tmparray1[tHead] != 0)
                    {
                        return (str1 + "释放PTT" + NewLine);
                    }
                    return (str1 + "按下PTT" + NewLine);

                case 0x0b:
                    str1 = "上电，MMI状态报告" + NewLine + "命令解析：" + NewLine;
                    if ((tmparray1[tHead] & 0x80) == 0x80)
                        str1 += "PTT释放,";
                    else
                        str1 += "PTT按下,";

                    if ((tmparray1[tHead] & 0x40) == 0x40)
                        str1 += "摘机,";
                    else
                        str1 += "挂机,";

                    if ((tmparray1[tHead] & 0x20) == 0x20)
                        str1 += "MMI,";
                    else
                        str1 += "普通控制盒,";
                    if ((tmparray1[tHead] & 0x04) != 0x04)
                        str1 += "111";
                    str1 = str1.TrimEnd(',');
                    return str1;
                //return ("上电，MMI状态报告" + NewLine);

                case 0x11:
                    str1 = ("请求车次号注册或注销" + NewLine) + "命令解析：" + NewLine;
                    if (tmparray1[tHead] != 0)
                    {
                        str1 = str1 + "注册" + NewLine;
                        break;
                    }
                    str1 = str1 + "注销" + NewLine;
                    break;

                case 0x13:
                    str1 = (("GSM-R呼叫操作" + NewLine) + "命令解析：" + NewLine) + "呼叫类型：";
                    switch (tmparray1[tHead])
                    {
                        case 1:
                            str1 += "个呼";
                            goto Label_03FB;

                        case 2:
                            str1 += "组呼";
                            goto Label_03FB;

                        case 3:
                            str1 += "功能号呼叫";
                            goto Label_03FB;

                        case 4:
                            str1 += "广播";
                            goto Label_03FB;
                        case 5:
                            str1 += "呼叫中发起紧急呼叫";
                            goto Label_03FB;
                    }
                    goto Label_03FB;

                case 0x15:
                    str1 = ("接通保持电话" + NewLine) + "命令解析：" + NewLine;
                    /*if (tmparray1[tHead] != 0)
                    {
                        return (str1 + "释放PTT" + NewLine);
                    }
                    return (str1 + "按下PTT" + NewLine);*/

                    if (tmparray1[tHead] == 0x00)
                        str1 += "挂断当前,主机自动接入等候电话";
                    else if (tmparray1[tHead] == 0x01)
                        str1 += "保持当前,切换需要接通的电话";
                    else str1 += "[记录有误!]";

                    return str1 + NewLine + "需要接通的电话序号：" + tmparray1[tHead + 1].ToString("X2");
                case 0x16:
                    str1 = ("(呼叫转移选择)操作" + NewLine) + "命令解析：" + NewLine;
                    switch (tmparray1[tHead])
                    {
                        case 50:
                            str1 = str1 + "无应答呼叫前转" + NewLine;
                            goto Label_0556;

                        case 0x33:
                            str1 = str1 + "无条件呼叫前转" + NewLine;
                            goto Label_0556;

                        case 0x34:
                            str1 = str1 + "遇忙呼叫前转" + NewLine;
                            goto Label_0556;

                        case 0x35:
                            str1 = str1 + "不可到达呼叫前转" + NewLine;
                            goto Label_0556;
                    }
                    goto Label_0556;

                case 0x17:
                    return ((("(网络选择)操作" + NewLine) + "命令解析：" + NewLine) + "网络：" + Encoding.Default.GetString(tmparray1, tHead, tLen - 1));

                case 0x18:
                    return ("MMI申请主控" + NewLine);

                case 0x19:
                    return ("MMI之间通信" + NewLine);

                case 0x20:
                    return ((("手工设置机车号" + NewLine) + "命令解析：" + NewLine) + "机车号：" + Encoding.Default.GetString(tmparray1, tHead, tLen - 1));

                case 0x21:
                    return ("MMI之间呼叫" + NewLine);

                case 0x22:
                    str1 = ("按键呼叫(450MHz)操作" + NewLine) + "命令解析：";
                    switch (tmparray1[tHead])
                    {
                        case 0xf1:
                            return (str1 + NewLine + "呼叫：调度" + NewLine);

                        case 0xf2:
                            return (str1 + NewLine + "呼叫：隧道车站" + NewLine);

                        case 0xf3:
                            return (str1 + NewLine + "呼叫：平原车站" + NewLine);

                        case 0xf4:
                            return (str1 + NewLine + "呼叫：隧道机车" + NewLine);

                        case 0xf5:
                            return (str1 + NewLine + "呼叫：平原机车" + NewLine);

                        case 0xf6:
                            return (str1 + NewLine + "呼叫：同频呼叫" + NewLine);
                    }
                    return str1;

                case 0x2e:
                    str1 = ("主机进入或退出库检状态" + NewLine) + "命令解析：" + NewLine;
                    if (tmparray1[tHead] != 0)
                    {
                        if (tmparray1[tHead] != 1)
                        {
                            return str1;
                        }
                        return (str1 + "进入库检状态");
                    }
                    return (str1 + "退出库检状态");

                case 0x2f:
                    str1 = ("MMI向主机发送启动库检命令" + NewLine) + "命令解析：" + NewLine;
                    str1 += "库检设备IP：" + tmparray1[tHead].ToString() + "." + tmparray1[tHead + 1].ToString() + "." + tmparray1[tHead + 2].ToString() + "." + tmparray1[tHead + 3].ToString();
                    return str1;
                case 50:
                case 0x33:
                case 0x34:
                case 0x37:
                case 0x39:
                    return str1;

                case 0x30:
                    return ("设置日期时间\r\n命令解析：\r\n" + (tmparray1[tHead] * 256 + tmparray1[tHead + 1]).ToString() + "-" + tmparray1[tHead + 2].ToString("00") + "-" + tmparray1[tHead + 3].ToString("00") + " " + tmparray1[tHead + 4].ToString("00") + ":" + tmparray1[tHead + 5].ToString("00") + ":" + tmparray1[tHead + 6].ToString("00") + NewLine);

                case 0x31:
                    return ("查询有效网络" + NewLine);

                case 0x35:
                    return ("查询VGCS/VBS组成员及状态" + NewLine);

                case 0x36:
                    return ("设置VGCS/VBS组成员及状态" + NewLine);

                case 0x38:
                    return ("查询网络名称" + NewLine);

                case 0x3a:
                    return ("查询主机软件版本" + NewLine);

                case 0x3b:
                    return ("查询主机语音模块名称" + NewLine);

                case 0xe0:
                    str1 = "查询或设置维护界面中的IP或APN" + NewLine;
                    switch (tmparray1[tHead])
                    {
                        case 0:
                            return (str1 + "查询维护界面中的IP和APN");

                        case 1:
                            return ((str1 + "设置维护界面中的GROSIP1" + NewLine) + "GROSIP1:" + Encoding.Default.GetString(tmparray1, tHead + 1, tLen - 2) + NewLine);

                        case 2:
                            return ((str1 + "设置维护界面中的GROSIP2" + NewLine) + "GROSIP2:" + Encoding.Default.GetString(tmparray1, tHead + 1, tLen - 2) + NewLine);

                        case 3:
                            return ((str1 + "设置维护界面中的GRISIP" + NewLine) + "GRISIP:" + Encoding.Default.GetString(tmparray1, tHead + 1, tLen - 2) + NewLine);

                        case 4:
                            return ((str1 + "设置维护界面中的APN" + NewLine) + "APN:" + Encoding.Default.GetString(tmparray1, tHead + 1, tLen - 2) + NewLine);
                    }
                    return str1;

                case 0xe1://Boki[131105]:新增的MMI协议
                    if (tmparray1[tHead] == 0x03)
                    {
                        str1 = "功能状态配置" + NewLine;
                        switch (tmparray1[tHead + 1])
                        {
                            case 1:
                                str1 += "手动获取车次号获取方式" + NewLine;
                                break;
                            case 2:
                                str1 += "自动获取车次号获取方式" + NewLine;
                                break;
                        }
                        switch (tmparray1[tHead + 2])
                        {
                            case 0:
                                str1 += "450M列尾开关打开" + NewLine;
                                break;
                            case 1:
                                str1 += "450M列尾开关关闭" + NewLine;
                                break;
                        }
                        switch (tmparray1[tHead + 3])
                        {
                            case 0:
                                str1 += "GPRS活动性检测开关打开" + NewLine;
                                break;
                            case 1:
                                str1 += "GPRS活动性检测开关关闭" + NewLine;
                                break;
                        }
                        switch (tmparray1[tHead + 4])
                        {
                            case 0:
                                str1 += "调度命令优先显示开关打开" + NewLine;
                                break;
                            case 1:
                                str1 += "调度命令优先显示开关关闭" + NewLine;
                                break;
                        }
                    }
                    if (tmparray1[tHead] == 2)
                    {
                        str1 = "查询功能状态配置";
                    }
                    if (tmparray1[tHead] == 0)
                    {
                        str1 = "查询车次号获取方式";
                    }
                    if (tmparray1[tHead] == 1)
                    {
                        str1 = ("设置车次号获取方式" + NewLine) + "命令解析：" + NewLine;
                        //if (tmparray1[tHead + 1] == 0x31)
                        //{
                        //    str1 += "手动获取";
                        //}
                        //else if (tmparray1[tHead + 1] == 0x32 || tmparray1[tHead] == 0x30)
                        //{
                        //    str1 += "自动获取";
                        //}
                        switch (tmparray1[tHead + 1])
                        {
                            case 1:
                                str1 += "手动获取" + NewLine;
                                break;
                            case 2:
                                str1 += "自动获取" + NewLine;
                                break;
                            default:
                                str1 += "不在协议内" + NewLine;
                                break;
                        }

                    }
                    return str1 + NewLine;

                case 0xe2:
                    return ("查询状态信息" + NewLine);

                default:
                    return str1;
            }
            return (str1 + "车次号：" + Encoding.Default.GetString(tmparray1, tHead + 1, tLen - 2));
        Label_03FB:
            CSt0t0004 = str1;
            return ((CSt0t0004 + NewLine + "优先级：" + tmparray1[tHead + 1].ToString() + NewLine) + "呼叫号码：" + Encoding.Default.GetString(tmparray1, tHead + 2, tLen - 3));
        Label_0556:
            switch (tmparray1[tHead + 1])
            {
                case 50:
                    str1 = str1 + "查询" + NewLine;
                    break;

                case 0x33:
                    str1 = str1 + "启用" + NewLine;
                    break;

                case 0x34:
                    str1 = str1 + "取消转移" + NewLine;
                    break;
            }
            return (str1 + "电话号码：" + Encoding.Default.GetString(tmparray1, tHead + 2, tLen - 3));
        }

        private static string ProcessFromMain(int command, byte[] tmparray1, int tHead)
        {
            int i;
            string CSt0t0004;
            string str1 = "";
            string str2 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            int CSt4t0001 = command;
            if (CSt4t0001 <= 0x30)
            {
                switch (CSt4t0001)
                {
                    case 0x2e:
                        return "主机报告自检结果";

                    case 0x2f:
                        return str1;

                    case 0x30:
                        return "主机请求MMI设置时间信息";

                    case 0x19:
                        str1 = "MMI之间通信:" + NewLine; ;
                        goto aaa;

                    //BQ
                    case 0x21:
                        return "MMI之间呼叫";
                }
                return str1;
            }
            switch (CSt4t0001)
            {
                case 0x41:
                    return "主机应答";

                case 0x42:
                    return ((("主机向MMI分配端口号" + NewLine) + "命令解析：" + NewLine) + "原MMI产生的随机数：" + tmparray1[tHead].ToString("x"));

                case 0x43:
                    return ((("主机对MMI的主控申请进行确认" + NewLine) + "命令解析：" + NewLine) + "端口号：" + tmparray1[tHead].ToString("x"));

                case 0x44:
                case 0x47:
                case 0x4d:
                case 0x4e:
                case 0x4f:
                case 80:
                case 0x56://Boki[131105]:新增的MMI协议
                    str1 = "收到查询车次号获取方式命令/需要司机确认车次号" + NewLine;
                    str1 += "主机当前车次号获取方式:";
                    if (tmparray1[tHead] == 0x31)
                    {
                        str1 += "手动获取";
                    }
                    else if (tmparray1[tHead] == 0x32 || tmparray1[tHead] == 0x30)
                    {
                        str1 += "自动获取";
                    }//
                    str1 += NewLine + "是否需要司机确认车次号:";
                    if (tmparray1[tHead + 1] == 0x00)
                    {
                        str1 += "不需要" + NewLine;
                    }
                    else if (tmparray1[tHead + 1] == 0x01)
                    {
                        str1 += "需要" + NewLine + "当前车次号:";
                        //7字节当前车次号
                        int cchLength = tmparray1.Length - tHead - 2 - 4;
                        string cch = Encoding.Default.GetString(tmparray1, tHead + 2, cchLength);
                        str1 += cch;
                    }
                    else
                        str1 += "数据有误!" + NewLine;
                    return str1;

                case 0x57:
                case 0x5c:
                case 0x5e:
                    return str1;

                case 0x45:
                    return "自检结果";

                case 70:
                    str1 = ("主机向MMI报告关电源" + NewLine) + "命令解析：" + NewLine;
                    switch (tmparray1[tHead])
                    {
                        case 0:
                            return (str1 + "关电源");

                        case 1:
                            return (str1 + "开电源");

                        case 0x10:
                            return (str1 + "软关机");
                    }
                    return str1;

                case 0x48:
                    return "GPS数据信息";

                case 0x49:
                    str1 = ("450M机车台向MMI报告发射状态" + NewLine) + "命令解析：" + NewLine;
                    if ((tmparray1[tHead] & 1) != 1)
                    {
                        return (str1 + "450MHz机车电台处于非发射状态");
                    }
                    return (str1 + "450MHz机车电台处于发射状态");

                case 0x4a:
                    return (((("主机报告450MHz场强" + NewLine) + "命令解析：" + NewLine) + "450MHz电台低端场强级别：" + tmparray1[tHead].ToString() + NewLine) + "450MHz电台高端场强级别：" + tmparray1[tHead + 1].ToString() + NewLine);

                case 0x4b:
                    str1 = ("主机对MMI的450MHz呼叫进行确认" + NewLine) + "命令解析：" + NewLine;
                    if ((tmparray1[tHead] & 0x80) != 0x80)
                    {
                        str1 = str1 + "状态：呼叫失败\r\n";
                        break;
                    }
                    str1 = str1 + "状态：呼叫成功\r\n";
                    break;

                case 0x4c:
                    str1 = ("主机向MMI指示450MHz来呼" + NewLine) + "命令解析：" + NewLine;
                    switch (tmparray1[tHead])
                    {
                        case 0xf1:
                            return (str1 + "呼叫：调度" + NewLine);

                        case 0xf2:
                            return (str1 + "呼叫：隧道车站" + NewLine);

                        case 0xf3:
                            return (str1 + "呼叫：平原车站" + NewLine);

                        case 0xf4:
                            return (str1 + "呼叫：隧道司机" + NewLine);      //*隧道机车

                        case 0xf5:
                            return (str1 + "呼叫：平原机车" + NewLine);

                        case 0xf6:
                            return (str1 + "呼叫：同频呼入" + NewLine);      //*原为:同频呼叫
                    }
                    return str1;

                case 0x51:
                    return "450M通话已经终止";

                case 0x52:
                    str1 = ("上行链路状态、PTT请求结果" + NewLine) + "命令解析：" + NewLine;
                    switch (tmparray1[tHead])
                    {
                        case 0:
                            return (str1 + "状态：上行链路空闲" + NewLine);

                        case 1:
                            return (str1 + "状态：上行链路占用" + NewLine);

                        case 2:
                            return (str1 + "状态：PTT抢占成功" + NewLine);

                        case 3:
                            return (str1 + "状态：PTT抢占失败" + NewLine);
                    }
                    return str1;

                case 0x53:
                    str1 += "状态：" + tmparray1[tHead].ToString("X2") + NewLine;
                    str1 += "区域代码、小区号：" + Encoding.ASCII.GetString(tmparray1, tHead + 3, 13);
                    return ("主机报告小区位置更新信息\r\n命令解析：\r\n" + str1);
                //return "主机报告小区位置更新信息";

                case 0x54:
                    str1 = ("主机向MMI报告网络注册状态" + NewLine) + "命令解析：" + NewLine;
                    if (tmparray1[tHead] != 1)
                    {
                        if (tmparray1[tHead] == 2)
                        {
                            str1 = str1 + "网络状态：GSMR";
                        }
                    }
                    else
                    {
                        str1 = str1 + "网络状态：GSM";
                    }
                    goto Label_06FF;

                case 0x55:
                    return GetInformationString(tmparray1, tHead);

                case 0x58:
                    str1 = ("主机向MMI报告车次号注册/注销结果" + NewLine) + "命令解析：" + NewLine;
                    if ((tmparray1[tHead] & 0x40) != 0x40)
                    {
                        str2 = str2 + "注销";
                    }
                    else
                    {
                        str2 = str2 + "注册";
                    }
                    if ((tmparray1[tHead] & 0x80) == 0x80)
                    {
                        CSt0t0004 = str1;
                        str1 = (CSt0t0004 + "车次号" + str2 + "成功" + NewLine) + "注册次数：" + tmparray1[tHead + 2].ToString() + NewLine;
                    }
                    else
                    {
                        CSt0t0004 = str1;
                        str1 = (CSt0t0004 + "车次号" + str2 + "失败" + NewLine) + "注销次数：" + tmparray1[tHead + 2].ToString() + NewLine;
                        switch (tmparray1[tHead + 1])
                        {
                            case 1:
                                str1 = str1 + "失败原因：业务无效\r\n";
                                goto Label_09BE;

                            case 2:
                                str1 = str1 + "失败原因：功能号非法\r\n";
                                goto Label_09BE;

                            case 3:
                                str1 = str1 + "失败原因：功能号已被其他用户注册\r\n";
                                goto Label_09BE;

                            case 4:
                                str1 = str1 + "失败原因：其他原因\r\n";
                                goto Label_09BE;

                            //default:
                            //   str1 = str1 + string.Format("失败原因：{0}\r\n", tmparray1[tHead + 1].ToString("X2"));
                            //    goto Label_09BE;
                        }
                    }
                    goto Label_09BE;

                case 0x59:
                    str1 = ("主机向MMI报告机车号注册/注销结果" + NewLine) + "命令解析：" + NewLine;
                    if ((tmparray1[tHead] & 0x40) != 0x40)
                    {
                        str2 = str2 + "注销";
                    }
                    else
                    {
                        str2 = str2 + "注册";
                    }
                    if ((tmparray1[tHead] & 0x80) == 0x80)
                    {
                        CSt0t0004 = str1;
                        str1 = (CSt0t0004 + "机车号" + str2 + "成功" + NewLine) + "注册次数：" + tmparray1[tHead + 2].ToString() + NewLine;
                    }
                    else
                    {
                        CSt0t0004 = str1;
                        str1 = (CSt0t0004 + "机车号" + str2 + "失败" + NewLine) + "注销次数：" + tmparray1[tHead + 2].ToString();
                        switch (tmparray1[tHead + 1])
                        {
                            case 1:
                                str1 = str1 + "\r\n失败原因：业务无效\r\n";
                                goto Label_0B7E;

                            case 2:
                                str1 = str1 + "\r\n失败原因：功能号非法\r\n";
                                goto Label_0B7E;

                            case 3:
                                str1 = str1 + "\r\n失败原因：功能号已被其他用户注册\r\n";
                                goto Label_0B7E;

                            case 4:
                                str1 = str1 + "\r\n失败原因：其他原因\r\n";
                                goto Label_0B7E;
                        }
                    }
                    goto Label_0B7E;

                case 0x5a:
                    str1 = ("主机向MMI报告AC确认结果" + NewLine) + "命令解析：" + NewLine;
                    switch (tmparray1[tHead])
                    {
                        case 0:
                            return (str1 + "AC确认成功");

                        case 1:
                            return (str1 + "AC确认失败" + tmparray1[tHead + 1].ToString() + "次");

                        case 0x80:
                            return (str1 + "AC确认失败，停止确认！");
                    }
                    return str1;

                case 0x5b:
                    str1 = ("主机对MMI的GSM-R呼叫进行确认" + NewLine) + "命令解析：" + NewLine;
                    if ((tmparray1[tHead] & 0x80) != 0x80)
                    {
                        str1 = str1 + "呼叫失败\r\n";
                        str1 = str1 + "失败原因：";
                        if ((tmparray1[tHead] & 1) == 1) str1 += "GSM-R模块故障,";
                        if ((tmparray1[tHead] & 2) == 2) str1 = str1 + "NO carrier,";
                        if ((tmparray1[tHead] & 4) == 4) str1 += "BUSY,";
                        if ((tmparray1[tHead] & 8) == 8) str1 = str1 + "原因4,";
                        if ((tmparray1[tHead] & 0x10) == 0x10) str1 = str1 + "呼叫超时";
                        str1 = str1.TrimEnd(',');
                        //>
                        goto Label_0D0E;
                    }
                    str1 = str1 + "呼叫成功" + NewLine;
                    goto Label_0D1E;

                case 0x5d:
                    {
                        str1 = ("主机向MMI通知GSMR当前的通话列表" + NewLine) + "命令解析：" + NewLine;
                        int j = 0;
                        int k = 0;
                        byte[] tmpbyte = new byte[200];
                        int status1 = 0;
                        int priority1 = 0;
                        string name1 = "";
                        string name2 = "";
                        string tmpStr = "";
                        for (i = 0; i < tLen; i++)
                        {
                            if (tmparray1[i + tHead] != 0x3b)
                            {
                                tmpbyte[j] = tmparray1[i + tHead];
                                j++;
                            }
                            else
                            {
                                if (j < 4)
                                {
                                    return str1;
                                }
                                status1 = tmpbyte[0];
                                if ((tmpbyte[1] >= 0x30) && (tmpbyte[1] <= 0x39))
                                {
                                    priority1 = tmpbyte[1] - 0x30;
                                }
                                else
                                {
                                    priority1 = 6;
                                }
                                if (IsRightASCII(tmpbyte, 3, j - 3))
                                {
                                    tmpStr = Encoding.Default.GetString(tmpbyte, 3, j - 3);
                                    if ((tmpStr.Length >= 3) && (tmpStr.IndexOf(",") >= 0))
                                    {
                                        name1 = tmpStr.Substring(0, tmpStr.IndexOf(","));
                                        name2 = tmpStr.Remove(0, tmpStr.IndexOf(",") + 1);
                                    }
                                    else
                                    {
                                        name1 = "";
                                        name2 = "";
                                    }
                                }
                                else
                                {
                                    tmpStr = "";
                                    if (tmpbyte[3] != 0x7e)
                                    {
                                        name1 = "";
                                        name2 = "";
                                    }
                                    else
                                    {
                                        int m;
                                        for (m = 8; m < (j - 1); m++)
                                        {
                                            if ((tmpbyte[m] == 0x2c) || (tmpbyte[m] == 0xff))
                                            {
                                                break;
                                            }
                                            tmpStr = tmpStr + BCDToInverseHex(tmpbyte[m]);
                                        }
                                        if (tmpStr.Length < 20)
                                        {
                                            name1 = tmpStr;
                                        }
                                        else
                                        {
                                            name1 = "";
                                        }
                                        if (IsRightASCII(tmpbyte, m + 1, (j - m) - 1))
                                        {
                                            name2 = Encoding.Default.GetString(tmpbyte, m + 1, (j - m) - 1);
                                        }
                                        else
                                        {
                                            name2 = "";
                                        }
                                    }
                                }
                                j = 0;
                                k++;
                                str1 = str1 + "第" + k.ToString() + "个电话：";
                                if ((status1 & 0x80) == 0x80)
                                {
                                    str1 = str1 + "当前电话,";
                                }
                                else
                                {
                                    str1 = str1 + "等待电话,";
                                }
                                if ((status1 & 0x40) == 0x40)
                                {
                                    str1 = str1 + "呼出电话,";
                                }
                                else
                                {
                                    str1 = str1 + "呼入电话,";
                                }
                                if ((status1 & 0x20) == 0x20)
                                {
                                    str1 = str1 + "通话中,";
                                }
                                switch ((status1 & 7))
                                {
                                    case 1:
                                        str1 = str1 + "个呼";
                                        break;

                                    case 2:
                                        str1 = str1 + "组呼";
                                        break;

                                    case 4:
                                        str1 = str1 + "广播";
                                        break;
                                }
                                str1 = (((str1 + NewLine) + "优先级：" + priority1.ToString() + NewLine) + "功能号：" + name1 + NewLine) + "电话号码：" + name2 + NewLine;
                            }
                        }
                        return str1;
                    }
                case 0x5f:
                    //return "上传库检结果";
                    //BQ
                    return "上传库检结果" + NewLine + "命令解析：" + NewLine + 库检结果(tmparray1, tHead);

                case 0x84:
                    return "报告呼叫转移操作结果";

                case 0x85:
                case 0x86:
                case 0x89:
                case 140:
                    return str1;

                case 0x87:
                    return "报告查询有效网络结果";

                case 0x88:
                    return "报告网络操作结果";

                case 0x8a:
                    return "返回查询VGCS/VBS查询结果";

                case 0x8b:
                    return "返回设置VGCS/VBS状态结果";

                case 0x8d:
                    return "返回查询网络名称";

                case 0x8e:
                    return "返回主机软件版本";

                case 0x8f:
                    return "返回语音模块名称";

                case 0xe0:
                    {
                        str1 = "返回维护界面中的IP或APN" + NewLine;
                        str2 = Encoding.Default.GetString(tmparray1, tHead + 1, tLen - 2);
                        i = str2.IndexOf(",");
                        string grosip1 = str2.Substring(0, i);
                        str2 = str2.Remove(0, i + 1);
                        i = str2.IndexOf(",");
                        string grosip2 = str2.Substring(0, i);
                        str2 = str2.Remove(0, i + 1);
                        i = str2.IndexOf(",");
                        string grisip = str2.Substring(0, i);
                        string apn_str = str2.Remove(0, i + 1);
                        return ((((str1 + "grosip1:" + grosip1 + NewLine) + "grosip2:" + grosip2 + NewLine) + "grisip:" + grisip + NewLine) + "APN:" + apn_str + NewLine);
                    }
                case 0xe1:
                    return str1;

                case 0xe2:
                    {
                        str1 = "返回主机状态" + NewLine;
                        int gsmIntensity = tmparray1[tHead];
                        int gprsIntensity = tmparray1[tHead + 1];
                        int baterryIntensity = tmparray1[tHead + 2];
                        ulong year1 = BCDToHex((ulong)tmparray1[tHead + 3], 2);
                        ulong mon1 = BCDToHex((ulong)tmparray1[tHead + 4], 2);
                        ulong day1 = BCDToHex((ulong)tmparray1[tHead + 5], 2);
                        ulong hour1 = BCDToHex((ulong)tmparray1[tHead + 6], 2);
                        ulong min1 = BCDToHex((ulong)tmparray1[tHead + 7], 2);
                        ulong sec1 = BCDToHex((ulong)tmparray1[tHead + 8], 2);
                        int num1 = 0;
                        if ((gsmIntensity > 0x1f) || (gsmIntensity < 0))
                        {
                            str1 = str1 + "GSMR场强：不可测" + NewLine;
                        }
                        else
                        {
                            num1 = -110 + (gsmIntensity * 2);
                            CSt0t0004 = str1;
                            str1 = CSt0t0004 + "GSMR场强：" + num1.ToString() + "dBm" + NewLine;
                        }
                        if ((gprsIntensity <= 0x1f) && (gprsIntensity >= 0))
                        {
                            num1 = -113 + (gprsIntensity * 2);
                            CSt0t0004 = str1;
                            str1 = CSt0t0004 + "GPRS场强：" + num1.ToString() + "dBm" + NewLine;
                        }
                        else
                        {
                            str1 = str1 + "GPRS场强：不可测" + NewLine;
                        }
                        if ((baterryIntensity & 2) == 2)
                        {
                            return (str1 + "电池状态：无法检测");
                        }
                        if ((baterryIntensity & 1) == 1)
                        {
                            CSt0t0004 = str1;
                            return (CSt0t0004 + "电池状态：正常(" + year1.ToString("00") + "-" + mon1.ToString("00") + "-" + day1.ToString("00") + " " + hour1.ToString() + ":" + min1.ToString() + ":" + sec1.ToString("00") + ")");
                        }
                        CSt0t0004 = str1;
                        return (CSt0t0004 + "电池状态：故障(" + year1.ToString("00") + "-" + mon1.ToString("00") + "-" + day1.ToString("00") + " " + hour1.ToString() + ":" + min1.ToString() + ":" + sec1.ToString("00") + ")");
                    }
                default:
                    return str1;
            }
            if ((tmparray1[tHead] & 0x40) == 0x40)
                str1 += "进入通话状态\r\n";
            else str1 += NewLine;

            if ((tmparray1[tHead] & 0x80) == 0)              //假如呼叫失败
            {
                string str_Yfail = "";
                if ((tmparray1[tHead] & 0x10) == 0x10)                  //HBQ
                {
                    str_Yfail += "呼叫超时,";
                }
                if ((tmparray1[tHead] & 0x08) == 0x08)
                {
                    str_Yfail += "原因2,";
                }
                if ((tmparray1[tHead] & 0x04) == 0x04)
                {
                    str_Yfail += "原因3,";
                }
                if ((tmparray1[tHead] & 0x02) == 0x02)
                {
                    str_Yfail += "原因3,";
                }
                if ((tmparray1[tHead] & 1) == 1)
                {
                    str_Yfail += "电台故障";
                }
                str1 += (str_Yfail == "" ? "" : "呼叫失败原因：") + str_Yfail.TrimEnd(',') + NewLine;
            }
            else str1 += NewLine;
            switch (tmparray1[tHead + 1])
            {
                case 0xf1:
                    return (str1 + "呼叫号码：调度" + NewLine);

                case 0xf2:
                    return (str1 + "呼叫号码：隧道车站" + NewLine);

                case 0xf3:
                    return (str1 + "呼叫号码：平原车站" + NewLine);

                case 0xf4:
                    return (str1 + "呼叫号码：隧道机车" + NewLine);

                case 0xf5:
                    return (str1 + "呼叫号码：平原机车" + NewLine);

                case 0xf6:
                    return (str1 + "呼叫号码：同频通信" + NewLine);        //*原为:同频呼叫,翻译上不同而已

                default:
                    return str1;
            }
        Label_06FF:
            if ((tmparray1[tHead + 1] & 0x80) == 0x80)
            {
                str1 = str1 + NewLine + "网络注册中" + NewLine;
            }
            else
            {
                str1 = str1 + NewLine + "网络注册完成" + NewLine;
            }

            if ((tmparray1[tHead + 1] & 2) == 2)
            {
                str1 = str1 + "信息对象：GPRS数据模块" + NewLine;
            }
            else if ((tmparray1[tHead + 1] & 1) == 1)
            {
                str1 = str1 + "信息对象：GSM语音模块" + NewLine;
            }

            if ((tmparray1[tHead + 1] & 0x40) == 0x40)
            {
                str1 = str1 + "网络注册异常" + NewLine;
                switch (tmparray1[tHead + 2])
                {
                    case 0:
                        return (str1 + "异常状态：模块故障" + NewLine);

                    case 1:
                        return (str1 + "异常状态：选网失败" + NewLine);

                    case 2:
                        return (str1 + "异常状态：掉网（切换）" + NewLine);
                }
            }
            return str1;
        Label_09BE:
            if (tLen > 2)
            {
                str1 = str1 + "车次号：" + Encoding.Default.GetString(tmparray1, tHead + 3, tLen - 3);
            }
            return str1;
        Label_0B7E:
            if (tLen > 2)
            {
                str1 = str1 + "机车号：" + Encoding.Default.GetString(tmparray1, tHead + 3, tLen - 3);
            }
            return str1;
        Label_0D0E:
            str1 = str1 + NewLine;
        Label_0D1E:
            if ((tmparray1[tHead] & 0x40) == 0x40)
            {
                str1 = str1 + "进入通话状态" + NewLine;
            }
            else
            {
                str1 = str1 + "处于呼叫状态" + NewLine;
            }
            if ((tmparray1[tHead] & 0x20) == 0x20)
            {
                str1 = str1 + "组呼已存在，自动加入" + NewLine;
            }
            else str1 += NewLine;
            return str1;

        aaa:
            switch (tmparray1[tHead])
            {
                case 0xaa:
                    str1 += "发送按键信息：" + NewLine;
                    switch (tmparray1[tHead + 1])
                    {
                        case 0x0B:
                            str1 += "功能按键1" + NewLine;
                            break;
                        case 0x0C:
                            str1 += "功能按键2" + NewLine;
                            break;
                        case 0x0D:
                            str1 += "功能按键3" + NewLine;
                            break;
                        case 0x0E:
                            str1 += "功能按键4" + NewLine;
                            break;
                        case 15:
                            str1 += "功能按键5" + NewLine;
                            break;
                        case 16:
                            str1 += "功能按键6" + NewLine;
                            break;
                        case 17:
                            str1 += "功能按键7" + NewLine;
                            break;
                        case 18:
                            str1 += "功能按键8" + NewLine;
                            break;
                        case 21:
                            str1 += "列尾排风按键 " + NewLine;
                            break;
                        case 22:
                            str1 += "列尾销号按键 " + NewLine;
                            break;
                        case 23:
                            str1 += "列尾确认按键 " + NewLine;
                            break;
                        case 24:
                            str1 += "风压查询按键 " + NewLine;
                            break;
                        case 25:
                            str1 += "呼叫按键 " + NewLine;
                            break;
                        case 26:
                            str1 += "切换按键 " + NewLine;
                            break;
                        case 27:
                            str1 += "挂断按键 " + NewLine;
                            break;
                        case 31:
                            str1 += "紧急呼叫按键 " + NewLine;
                            break;
                        case 32:
                            str1 += "数字1键 " + NewLine;
                            break;
                        case 33:
                            str1 += "数字2键 " + NewLine;
                            break;
                        case 34:
                            str1 += "数字3键 " + NewLine;
                            break;
                        case 35:
                            str1 += "设置按键 " + NewLine;
                            break;
                        case 36:
                            str1 += "向上按键 " + NewLine;
                            break;
                        case 37:
                            str1 += "界面按键 " + NewLine;
                            break;
                        case 41:
                            str1 += "报警按键 " + NewLine;
                            break;
                        case 42:
                            str1 += "数字4键 " + NewLine;
                            break;
                        case 43:
                            str1 += "数字5键 " + NewLine;
                            break;
                        case 44:
                            str1 += "数字6键 " + NewLine;
                            break;
                        case 45:
                            str1 += "向左按键 " + NewLine;
                            break;
                        case 46:
                            str1 += "确认/签收按键 " + NewLine;
                            break;
                        case 47:
                            str1 += "向右按键 " + NewLine;
                            break;
                        case 51:
                            str1 += "主控按键 " + NewLine;
                            break;
                        case 52:
                            str1 += "数字7键 " + NewLine;
                            break;
                        case 53:
                            str1 += "数字8键 " + NewLine;
                            break;
                        case 54:
                            str1 += "数字9键 " + NewLine;
                            break;
                        case 55:
                            str1 += "查询按键 " + NewLine;
                            break;
                        case 56:
                            str1 += "向下按键 " + NewLine;
                            break;
                        case 57:
                            str1 += "回格按键 " + NewLine;
                            break;
                        case 61:
                            str1 += "复位按键 " + NewLine;
                            break;
                        case 62:
                            str1 += "*按键" + NewLine;
                            break;
                        case 63:
                            str1 += "数字0键" + NewLine;
                            break;
                        case 64:
                            str1 += "#按键 " + NewLine;
                            break;
                        case 65:
                            str1 += "打印按键 " + NewLine;
                            break;
                        case 66:
                            str1 += "调车按键 " + NewLine;
                            break;
                        case 67:
                            str1 += "退出按键 " + NewLine;
                            break;
                        default:
                            str1 += "其它按键" + NewLine;
                            break;
                    }
                    switch (tmparray1[tHead + 2])
                    {
                        case 0:
                            str1 += "按下！" + NewLine;
                            break;
                        case 1:
                            str1 += "抬起！" + NewLine;
                            break;
                        default:
                            str1 += "无法识别该键是否按下" + NewLine;
                            break;
                    }

                    switch (tmparray1[tHead + 4])
                    {
                        case 0:
                            str1 += "副控状态" + NewLine;
                            break;
                        case 1:
                            str1 += "主控状态" + NewLine;
                            break;
                        default:
                            str1 += "无法识别主/副状态！" + NewLine;
                            break;
                    }
                    break;

                case 0xab:
                    str1 += "发送MMI内部调试信息:" + NewLine;
                    int tscs = (int)((tmparray1[tHead + 1]) * 0xf4240 + (tmparray1[tHead + 2]) * 0x2710 + (tmparray1[tHead + 3]) * 100 + tmparray1[tHead + 4]);
                    int jmcs = (int)((tmparray1[tHead + 5]) * 100 + (tmparray1[tHead + 6]));
                    string tscs1 = tscs.ToString();
                    string jmcs1 = jmcs.ToString();
                    str1 += "调试参数为" + tscs1 + " ,界面参数为" + jmcs1;
                    break;

                case 0xac:
                    str1 += "发送按键“确认/签收”的信息" + NewLine;
                    int tscs2 = (int)(((tmparray1[tHead + 1]) * 100) + (tmparray1[tHead + 2]));
                    int jmcs2 = (int)(((tmparray1[tHead + 4]) * 100) + (tmparray1[tHead + 5]));
                    string tscs3 = tscs2.ToString();
                    string jmcs3 = jmcs2.ToString();
                    str1 += "调试参数为" + tscs3 + ",界面参数为" + jmcs3;
                    break;
                 
                case 0x0F:
                    str1 += "发送MMI设置信息" + NewLine;
                    string i1 = (tmparray1[tHead + 1]).ToString();
                    string i2 = (tmparray1[tHead + 2]).ToString();
                    str1 += "设置参数为" + i1 + NewLine;
                    str1 += "类型参数为" + i2 + NewLine;
                    break;
            }
            return str1;          

        }

        private static string ProcessToRecordUnit(int command, byte[] tmparray1, int tHead)
        {
            ulong year;
            ulong mon;
            ulong day;
            ulong hour;
            ulong min;
            ulong sec;
            string CSt0t0003;
            string str1 = "";
            int CSt4t0001 = command;
            if (CSt4t0001 <= 70)
            {
                if (CSt4t0001 <= 0x35)
                {
                    switch (CSt4t0001)
                    {
                        case 0x33:
                            str1 = "CIR控制记录单元开始/停止播放话音记录" + NewLine;
                            if (tmparray1[tHead] != 0)
                            {
                                return (str1 + "开始播放" + NewLine);
                            }
                            return (str1 + "停止播放" + NewLine);

                        case 0x34:
                            return ("CIR查询记录单元当前时钟" + NewLine);

                        case 0x35:
                            str1 = "CIR手动设置记录单元时钟" + NewLine;
                            year = BCDToHex((ulong)tmparray1[tHead], 2);
                            mon = BCDToHex((ulong)tmparray1[tHead + 1], 2);
                            day = BCDToHex((ulong)tmparray1[tHead + 2], 2);
                            hour = BCDToHex((ulong)tmparray1[tHead + 3], 2);
                            min = BCDToHex((ulong)tmparray1[tHead + 4], 2);
                            sec = BCDToHex((ulong)tmparray1[tHead + 5], 2);
                            CSt0t0003 = str1;
                            return (CSt0t0003 + "命令解析：" + NewLine + "当前时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " "
                                                                                                        + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine);

                        case 1:
                            return "记录单元向CIR主机发送应答信息" + NewLine + "命令解析：" + NewLine + "发送方命令字字节：" + tmparray1[tHead].ToString("X2") + "H" + NewLine + "发送方第一字节：" + tmparray1[tHead + 1].ToString("X2") + "H" + NewLine;
                        //return ("记录单元向CIR主机发送应答信息" + NewLine);
                    }
                    return str1;
                }
                switch (CSt4t0001)
                {
                    case 0x41:
                        return "记录单元向CIR主机发送应答信息" + NewLine + "命令解析：" + NewLine + "发送方命令字字节：" + tmparray1[tHead].ToString("X2") + "H" + NewLine + "发送方第一字节：" + tmparray1[tHead + 1].ToString("X2") + "H" + NewLine;
                    //return ("记录单元向MMI发送应答信息" + NewLine);

                    case 70:
                        str1 = "CPU向记录单元发送开关机指令" + NewLine + "命令解析：" + NewLine;
                        if (tmparray1[tHead] == 1)
                        {
                            str1 = str1 + "上电" + NewLine;
                        }
                        if (tmparray1[tHead] == 2)
                        {
                            str1 = str1 + "关机" + NewLine;
                        }
                        if (tmparray1[tHead] != 3)
                        {
                            return str1;
                        }
                        if (tmparray1[tHead + 1] == 0)
                        {
                            str1 = str1 + "外部电源状态：低电压" + NewLine;
                        }
                        else
                        {
                            str1 = str1 + "外部电源状态：高电压" + NewLine;
                        }
                        if (tmparray1[tHead + 2] == 0)
                        {
                            return (str1 + "电池状态：正常" + NewLine);
                        }
                        return (str1 + "电池状态：失效" + NewLine);
                }
                return str1;
            }
            if (CSt4t0001 <= 0xa5)
            {
                switch (CSt4t0001)
                {
                    case 0x91:
                        str1 = "记录单元向CIR输出当前时钟" + NewLine;
                        year = BCDToHex((ulong)tmparray1[tHead], 2);
                        mon = BCDToHex((ulong)tmparray1[tHead + 1], 2);
                        day = BCDToHex((ulong)tmparray1[tHead + 2], 2);
                        hour = BCDToHex((ulong)tmparray1[tHead + 3], 2);
                        min = BCDToHex((ulong)tmparray1[tHead + 4], 2);
                        sec = BCDToHex((ulong)tmparray1[tHead + 5], 2);
                        CSt0t0003 = str1;
                        return (CSt0t0003 /*+ NewLine*/ + "当前时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine);

                    case 0x92:
                        return ("记录单元向CIR发送播放结束的命令" + NewLine);

                    case 0xa5:
                        return ("CIR查询记录单元软件版本" + NewLine);
                }
                return str1;
            }
            switch (CSt4t0001)
            {
                case 250:
                    return ("CIR向记录单元发送问讯测试" + NewLine);

                case 0xfb:
                    str1 = "记录单元向CIR发送问讯应答" + NewLine;
                    if ((tmparray1[tHead] & 1) != 0)
                    {
                        str1 = str1 + "CIR外部电源状态：高电平" + NewLine;
                        break;
                    }
                    str1 = str1 + "CIR外部电源状态：低电平" + NewLine;
                    break;

                case 170:
                    return (("记录单元向CIR输出当前软件版本" + NewLine) + "版本： " + Encoding.Default.GetString(tmparray1, tHead, tmparray1.Length - 14) + NewLine);

                default:
                    return str1;
            }
            if ((tmparray1[tHead] & 2) == 0)
            {
                str1 = str1 + "录音控制信号状态：低电平" + NewLine;
            }
            else
            {
                str1 = str1 + "录音控制信号状态：高电平" + NewLine;
            }
            if ((tmparray1[tHead] & 4) == 0)
            {
                str1 = str1 + "复位控制信号状态：低电平" + NewLine;
            }
            else
            {
                str1 = str1 + "复位控制信号状态：高电平" + NewLine;
            }
            if ((tmparray1[tHead] & 8) == 0)
            {
                str1 = str1 + "接收卫星定位单元公用数据：正常" + NewLine;
            }
            else
            {
                str1 = str1 + "接收卫星定位单元公用数据：故障" + NewLine;
            }
            if ((tmparray1[tHead] & 0x10) == 0)
            {
                str1 = str1 + "卫星定位单元公用数据状态：可用" + NewLine;
            }
            else
            {
                str1 = str1 + "卫星定位单元公用数据状态：无效" + NewLine;
            }
            if ((tmparray1[tHead] & 0x20) == 0)
            {
                str1 = str1 + "接收主控单元数据：正常" + NewLine;
            }
            else
            {
                str1 = str1 + "接收主控单元数据：故障" + NewLine;
            }
            if ((tmparray1[tHead] & 0x40) == 0)
            {
                str1 = str1 + "电池状态：正常" + NewLine;
            }
            else
            {
                str1 = str1 + "电池状态：故障" + NewLine;
            }
            if ((tmparray1[tHead + 1] == 0xff) && (tmparray1[tHead + 2] == 0xff))
            {
                return (str1 + "电池电压未知" + NewLine);
            }
            ulong numdc1 = BCDToHex((ulong)tmparray1[tHead + 1], 2);
            ulong numdc2 = BCDToHex((ulong)tmparray1[tHead + 2], 2);
            CSt0t0003 = str1;
            return (CSt0t0003 + "电池电压：" + numdc1.ToString() + "." + numdc2.ToString() + NewLine);
        }

        private static string ProcessFromDiaoDU(int command, byte[] tmparray1, int tHead)
        {
            int command1;
            ulong year;
            ulong mon;
            ulong day;
            ulong hour;
            ulong min;
            ulong sec;
            string commandid;
            int signed;
            string CSt0t0003;
            string str1 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            switch (command)
            {
                case 60:
                    str1 = "擦除调度命令操作";
                    break;

                case 0x41:
                    str1 = "主机应答";
                    break;

                case 0xf1:
                case 0x20:
                    str1 = ("主机向MMI传送调度命令" + NewLine) + "命令解析：" + NewLine;
                    try
                    {
                        command1 = tmparray1[tHead];
                        year = BCDToHex((ulong)tmparray1[tHead + 1], 2);
                        mon = BCDToHex((ulong)tmparray1[tHead + 2], 2);
                        day = BCDToHex((ulong)tmparray1[tHead + 3], 2);
                        hour = BCDToHex((ulong)tmparray1[tHead + 4], 2);
                        min = BCDToHex((ulong)tmparray1[tHead + 5], 2);
                        sec = BCDToHex((ulong)tmparray1[tHead + 6], 2);
                        string cch = Encoding.Default.GetString(tmparray1, tHead + 10, 7);
                        string jch = Encoding.Default.GetString(tmparray1, tHead + 0x11, 8);
                        int house = tmparray1[tHead + 0x19] + (tmparray1[tHead + 0x29] * 0x100);
                        commandid = Encoding.Default.GetString(tmparray1, tHead + 0x1a, 6);
                        string name = Encoding.Default.GetString(tmparray1, tHead + 0x20, 8);
                        signed = tmparray1[tHead + 40];
                        int totalNum = tmparray1[tHead + 0x2e];
                        int nowNum = tmparray1[tHead + 0x2f];
                        string txt = Encoding.Default.GetString(tmparray1, tHead + 0x30, tLen - 0x30);
                        str1 = str1 + "命令名称：";
                        if ((command1 >= 1) && (command1 <= 12))
                        {
                            str1 = str1 + CommandType[command1 - 1];
                        }
                        else if (command1 == 0x11)
                        {
                            str1 = str1 + "调车作业单";
                        }
                        else if (command1 == 0x20)
                        {
                            str1 = str1 + "出入库检测";
                        }
                        else if ((command1 >= 0x18) && (command1 <= 0x1f))
                        {
                            str1 = str1 + "其它信息";
                        }
                        CSt0t0003 = str1;
                        str1 = CSt0t0003 + NewLine + "时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                        str1 = str1 + "车次号：" + cch + NewLine;
                        str1 = str1 + "机车号：" + jch + NewLine;
                        str1 = str1 + "发令处所编号：" + house.ToString() + NewLine;
                        str1 = str1 + "命令编号：" + commandid + NewLine;
                        str1 = str1 + "调度员：" + name + NewLine;
                        str1 = str1 + "命令状态：";
                        if ((signed & 0x80) == 0x80)
                        {
                            str1 = str1 + "已签收,";
                        }
                        if ((signed & 0x20) == 0x20)
                        {
                            str1 = str1 + "已打印,";
                        }
                        if ((signed & 2) == 2)
                        {
                            str1 = str1 + "非新接收的命令,";
                        }
                        if ((signed & 1) == 1)
                        {
                            str1 = str1 + "补机,";
                        }
                        CSt0t0003 = str1.TrimEnd(',');      //结束则去掉','
                        str1 = CSt0t0003 + NewLine + "总包数：" + totalNum.ToString() + NewLine;
                        str1 = str1 + "本包数：" + nowNum.ToString() + NewLine;
                        str1 = str1 + "调度命令文字内容：" + NewLine + txt;
                    }
                    catch
                    {
                    }
                    break;

                case 0x21:
                    str1 = "主机向MMI传送最近10条命令索引目录" + NewLine;
                    break;

                case 0x22:
                    str1 = "主机向MMI传送前10条命令索引目录" + NewLine;
                    break;

                case 0x23:
                    str1 = "主机向MMI传送后10条命令索引目录" + NewLine;
                    break;

                case 0x24:
                    str1 = "无查询结果报告" + NewLine;
                    break;

                case 0x2e:
                    str1 = "主机向MMI传送状态" + NewLine + "命令解析：" + NewLine;
                    if ((tmparray1[tHead] & 0x80) == 0x80)
                        str1 += "已签收,";
                    else str1 = "未签收,";

                    if ((tmparray1[tHead] & 0x20) == 0x20)
                        str1 += "已打印";
                    else str1 += "未打印";
                    break;
            }
            if ((command > 0x20) && (command < 0x24))
            {
                str1 = str1 + "命令解析：" + NewLine;
                int numlist1 = tLen / 14;
                for (int i = 0; i < numlist1; i++)
                {
                    command1 = tmparray1[tHead + (i * 14)];
                    year = BCDToHex((ulong)tmparray1[(tHead + 1) + (i * 14)], 2);
                    mon = BCDToHex((ulong)tmparray1[(tHead + 2) + (i * 14)], 2);
                    day = BCDToHex((ulong)tmparray1[(tHead + 3) + (i * 14)], 2);
                    hour = BCDToHex((ulong)tmparray1[(tHead + 4) + (i * 14)], 2);
                    min = BCDToHex((ulong)tmparray1[(tHead + 5) + (i * 14)], 2);
                    sec = BCDToHex((ulong)tmparray1[(tHead + 6) + (i * 14)], 2);
                    commandid = Encoding.Default.GetString(tmparray1, (tHead + 7) + (i * 14), 6);
                    signed = tmparray1[(tHead + 13) + (i * 14)];
                    str1 = str1 + (i == 0 ? "" : NewLine) + "第" + i.ToString() + "条命令名称：";
                    if ((command1 >= 1) && (command1 <= 12))
                    {
                        str1 = str1 + CommandType[command1 - 1];
                    }
                    else if (command1 == 0x11)
                    {
                        str1 = str1 + "调车作业单";
                    }
                    else if (command1 == 0x20)
                    {
                        str1 = str1 + "出入库检测";
                    }
                    else if ((command1 >= 0x18) && (command1 <= 0x1f))
                    {
                        str1 = str1 + "其它信息";
                    }
                    CSt0t0003 = str1;
                    str1 = ((CSt0t0003 + NewLine + "时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") +
                                                          ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine) + "命令编号：" + commandid + NewLine) + "命令状态：";
                    if ((signed & 0x80) == 0x80)
                    {
                        str1 = str1 + "已签收,";
                    }
                    if ((signed & 0x20) == 0x20)
                    {
                        str1 = str1 + "已打印,";
                    }
                    if ((signed & 2) == 2)
                    {
                        str1 = str1 + "非新接收的命令,";
                    }
                    if ((signed & 1) == 1)
                    {
                        str1 = str1 + "补机,";
                    }
                    str1 = str1.TrimEnd(',') + NewLine;
                }
            }
            return str1;
        }

        private static string ProcessFromLieWei(int command, byte[] tmparray1, int tHead)
        {
            int i;
            string str1 = "";
            int tLen = (((tmparray1[2] * 0x100) + tmparray1[3]) - tHead) + 2;
            switch (command)
            {
                case 0x21:
                    str1 = "列尾主机向MMI传送风压信息" + NewLine;
                    break;

                case 0x22:
                    str1 = "列尾主机向MMI传送排风确认信息" + NewLine;
                    break;

                case 0x23:
                    str1 = "列尾主机向MMI传送风压报警信息" + NewLine;
                    break;

                case 0x24:
                    str1 = "列尾主机向MMI传送电压报警信息" + NewLine;
                    break;

                case 0x25:
                    str1 = "列尾主机向MMI传送建立对应关系信息" + NewLine;
                    break;

                case 0x26:
                    str1 = "列尾主机向MMI传送拆除对应关系信息" + NewLine;
                    break;

                case 0x27:
                    str1 = "列尾主机向MMI传送对应关系建立成功信息" + NewLine;
                    break;

                case 40:            //(0x28)
                    str1 = "列尾自动风压报告";
                    break;

                case 0x29:
                    str1 = "主机申请列尾连接";
                    break;

                case 0x2a:
                    str1 = "列尾拆除对应关系";
                    break;

                case 0x41:
                    str1 = "主机应答";
                    break;
            }
            if ((command > 0x20) && (command < 0x24))  //21H~23H
            {
                str1 = str1 + "命令解析：" + NewLine;
                ulong jch1 = 0;
                for (i = 0; i < 4; i++)
                {
                    jch1 = (jch1 * 100) + BCDToHex((ulong)tmparray1[i + tHead], 2);
                }
                ulong id1 = 0;
                for (i = 0; i < 4; i++)
                {
                    id1 = (id1 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 15], 2);
                }
                ulong press1 = 0;
                for (i = 0; i < 2; i++)
                {
                    press1 = (press1 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 4], 2);
                }
                string longitude = BCDToHex((ulong)tmparray1[tHead + 6], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 7], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 8], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 9], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 10], 2).ToString("00") + "\"";
                string latitude = BCDToHex((ulong)tmparray1[tHead + 11], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 12], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 13], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 14], 2).ToString("00") + "\"";
                str1 = ((((str1 + "列尾机车号：" + jch1.ToString() + NewLine) + "列尾ID：" + id1.ToString() + NewLine) + "风压：" + press1.ToString() + NewLine) + "经度：" + longitude + NewLine) + "纬度：" + latitude + NewLine;
            }
            if ((command == 0x24) || (command == 0x26))         //BQ:原((command == 0x24) && (command == 0x26))
            {                                                                           //4-4-5-4字节
                str1 = str1 + "命令解析：" + NewLine;
                ulong jch2 = 0;
                for (i = 0; i < 4; i++)
                {
                    jch2 = (jch2 * 100) + BCDToHex((ulong)tmparray1[i + tHead], 2);
                }
                ulong id2 = 0;
                for (i = 0; i < 4; i++)
                {
                    id2 = (id2 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 13], 2);
                }
                string longitude2 = BCDToHex((ulong)tmparray1[tHead + 4], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 5], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 6], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 7], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 8], 2).ToString("00") + "\"";
                string latitude2 = BCDToHex((ulong)tmparray1[tHead + 9], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 12], 2).ToString("00") + "\"";
                str1 = (((str1 + "列尾机车号：" + jch2.ToString() + NewLine) + "列尾ID：" + id2.ToString() + NewLine) + "经度：" + longitude2 + NewLine) + "纬度：" + latitude2 + NewLine;
            }
            if (command == 0x27)    //BQ
            {
                str1 += "命令解析：\r\n";
                ulong jch2 = 0;
                for (i = 0; i < 4; i++)
                { jch2 = (jch2 * 100) + BCDToHex((ulong)tmparray1[i + tHead], 2); }
                str1 += "列尾机车号：" + jch2.ToString() + NewLine;
            }
            if (command != 0x25)
            {
                return str1;
            }
            str1 = str1 + "命令解析：" + NewLine;                      //0x25?
            ulong jch3 = 0;
            for (i = 0; i < 4; i++)
            {
                jch3 = (jch3 * 100) + BCDToHex((ulong)tmparray1[i + tHead], 2);
            }
            ulong id3 = 0;
            for (i = 0; i < 4; i++)
            {
                id3 = (id3 * 100) + BCDToHex((ulong)tmparray1[(i + tHead) + 0x11], 2);
            }
            //string liewei_IP = tmparray1[tHead + 4].ToString() + "." + tmparray1[tHead + 5].ToString() + "." + tmparray1[tHead + 6].ToString() + "." + tmparray1[tHead + 7].ToString();
            string longitude3 = BCDToHex((ulong)tmparray1[tHead + 8], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 9], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 10], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 11], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 12], 2).ToString("00") + "\"";
            string latitude3 = BCDToHex((ulong)tmparray1[tHead + 13], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 14], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 15], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 0x10], 2).ToString("00") + "\"";
            return ((((str1 + "列尾机车号：" + jch3.ToString() + NewLine) + "列尾ID：" + id3.ToString() + NewLine) + "经度：" + longitude3 + NewLine) + "纬度：" + latitude3 + NewLine);
        }

        private static string TAX_Data(int command, byte[] tmparray1, int tHead)
        {
            int timelong;
            // float gonglibiao;
            int sec;
            int min;
            int hour;
            int day;
            int mon;
            int year;

            timelong = ((tmparray1[tHead + 35] + (tmparray1[tHead + 36] << 8)) + (tmparray1[tHead + 37] << 0x10)) + (tmparray1[tHead + 38] << 0x18);
            sec = timelong & 0x3f;
            min = (timelong >> 6) & 0x3f;
            hour = (timelong >> 12) & 0x1f;
            day = (timelong >> 0x11) & 0x1f;
            mon = (timelong >> 0x16) & 15;
            year = (timelong >> 0x1a) & 0x3f;

            string str1 = "发送车次号信息" + NewLine;
            //"命令解析："
            str1 += "本板地址：" + (Convert.ToInt32(tmparray1[tHead]) == 0x38 ? "38H(新增加帧)" : tmparray1[tHead].ToString("X2")) + NewLine;
            str1 += "特征码：0" + NewLine;
            str1 += "标志：" + (Convert.ToInt32(tmparray1[tHead + 2]) == 0x67 ? "67H(新协议)" : tmparray1[tHead + 2].ToString("X2")) + NewLine;
            str1 += "版本号：" + tmparray1[tHead + 3].ToString() + NewLine;
            //保留
            str1 += "车站号扩充字节：" + (tmparray1[tHead + 5] & 0x0f).ToString() + NewLine;
            str1 += "车次种类标识符：" + Encoding.ASCII.GetString(tmparray1, tHead + 6, 4).TrimEnd('\0') + NewLine;
            str1 += "司机号：" + tmparray1[tHead + 10].ToString() + NewLine;
            str1 += "副司机号：" + tmparray1[tHead + 11].ToString() + NewLine;
            //保留
            str1 += "机车型号：" + (tmparray1[tHead + 14] & 0x01).ToString() + NewLine;
            str1 += "实际交路号：" + tmparray1[tHead + 15].ToString() + NewLine;
            // //str1+="保留"
            // //str1+="保留"
            str1 += ((tmparray1[tHead + 27] & 0x01) == 0 ? "货车" : "客车") + "," + ((tmparray1[tHead + 27] & 0x02) == 0 ? "本务机" : "补机") + NewLine;
            str1 += "车次数字部分：" + (tmparray1[tHead + 30] * 256 * 256 + tmparray1[tHead + 29] * 256 + tmparray1[tHead + 28]).ToString() + NewLine;
            str1 += "检查和1：" + tmparray1[tHead + 31].ToString("X2") + "H" + NewLine;

            str1 += "本板地址：" + tmparray1[tHead + 32].ToString("X2") + "H\r\n";
            str1 += "特征码：";
            switch (tmparray1[tHead + 33])
            {
                case 0x30: str1 += "上次接收成功"; break;
                case 0xc0: str1 += "上次接收失败"; break;
                default: str1 += "本串数据通讯过程中受干扰,数据无效"; return str1;
            }
            str1 += NewLine;
            string[] unitCode = new string[] { "轨道检测", "弓网监测", "TMIS", "DMIS", "列控通讯", "语音录音", "轴温报警", "鸣笛检查", "预留给备用单元" };
            try { str1 += "检测单元代号：" + unitCode[tmparray1[tHead + 34] - 1] + NewLine; }
            catch { str1 += "无此单元代号" + NewLine; }
            str1 += "时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
            str1 += "实速：" + ((tmparray1[tHead + 40] & 0x03) * 256 + tmparray1[tHead + 39]).ToString() + NewLine;             //此处低位在前算?
            str1 += "机车信号：";
            string[] CabLight = new string[] { "无灯", "绿", "黄", "双黄", "红黄", "红", "白", "绿黄", "黄2", "机车信号未定义!" };
            str1 += (tmparray1[tHead + 42] & 0x10) == 0x10 ? "多灯," : "单灯," + CabLight[(tmparray1[tHead + 42] & 0x0f) <= 8 ? (tmparray1[tHead + 42] & 0x0f) : 9] + NewLine;

            int gongkuang = tmparray1[tHead + 43];
            str1 += "机车工况：";
            if ((gongkuang & 1) == 1) str1 += "零位,";
            if ((gongkuang & 2) == 2) str1 += "向后,";
            if ((gongkuang & 4) == 4) str1 += "向前,";
            if ((gongkuang & 8) == 8) str1 += "制动,";
            if ((gongkuang & 0x10) == 0x10) str1 += "牵引,";
            str1 = str1.TrimEnd(',') + NewLine;

            str1 += "信号机编号：" + (tmparray1[tHead + 44] + tmparray1[tHead + 45] * 256).ToString() + NewLine;
            str1 += "信号机种类：";
            switch (tmparray1[tHead + 46] & 0x7)
            {
                case 2: str1 += "出站"; break;
                case 3: str1 += "进站"; break;
                case 4: str1 += "通过"; break;
                case 5: str1 += "预告"; break;
                case 6: str1 += "容许"; break;
                default: str1 += "暂未定义"; break;
            }
            str1 += NewLine;
            string str_gonglibiao = ((tmparray1[tHead + 49] & 0x80) == 1 ? -1 : 1) * (tmparray1[tHead + 47] + tmparray1[tHead + 48] * 256 + (tmparray1[tHead + 49] & 0x3f) * 256 * 256) + "米";
            str1 += "公里标：" + (str_gonglibiao == "9999999米" ? "CIR处于编组站状态" : str_gonglibiao) + NewLine;
            str1 += "总重：" + (tmparray1[tHead + 50] + tmparray1[tHead + 51] * 256).ToString() + NewLine;          //高位在前?
            int tmp = tmparray1[tHead + 52] + tmparray1[tHead + 53] * 256;         //高位在前?
            str1 += "计长：" + (tmp / 10).ToString() + "." + (tmp - tmp / 10 * 10).ToString() + NewLine;
            str1 += "辆数：" + tmparray1[tHead + 54] + NewLine;
            str1 += ((tmparray1[tHead + 55] & 0x01) == 0 ? "货车" : "客车") + "," + ((tmparray1[tHead + 55] & 0x02) == 0 ? "本务机" : "补机") + NewLine;
            str1 += "车次号：" + (tmparray1[tHead + 56] + tmparray1[tHead + 57] * 256).ToString() + NewLine;
            str1 += "区段号(交路号)：" + tmparray1[tHead + 58] + NewLine;
            str1 += "车站号：" + tmparray1[tHead + 59] + NewLine;
            str1 += "司机号：" + (tmparray1[tHead + 60] + tmparray1[tHead + 61] * 256).ToString() + NewLine;
            str1 += "副司机号：" + (tmparray1[tHead + 62] + tmparray1[tHead + 63] * 256).ToString() + NewLine;
            str1 += "机车号：" + (tmparray1[tHead + 64] + tmparray1[tHead + 65] * 256).ToString() + NewLine;
            str1 += "机车型号：" + tmparray1[tHead + 66].ToString() + NewLine;
            str1 += "列车管压力：" + ((tmparray1[tHead + 68] & 3) * 256 + tmparray1[tHead + 67]).ToString() + "Kpa" + NewLine;
            str1 += "装置状态：" + ((tmparray1[tHead + 69] & 1) == 1 ? "降级," : "监控,") + ((tmparray1[tHead + 69] & 4) == 4 ? "调车" : "非调车") + NewLine;
            str1 += "检查和2：" + tmparray1[tHead + 71].ToString("X2") + "H" + NewLine;

            return str1;
        }

        private static string TAX_Extend(int command, byte[] tmparray1, int tHead)
        {
            string str1 = "";
            str1 += "位置号：" + tmparray1[tHead + 115].ToString("X2") + tmparray1[tHead + 116].ToString("X2") + NewLine;
            str1 += "小区号：" + tmparray1[tHead + 117].ToString("X2") + tmparray1[tHead + 118].ToString("X2") + NewLine;
            str1 += "定位状态：" + ((tmparray1[tHead + 119] == 0x41) ? "资料可用" : (tmparray1[tHead + 119] == 0x56 ? "资料不可用" : "记录有误!")) + NewLine;
            string longitude = BCDToHex((ulong)tmparray1[tHead + 120], 2).ToString() + BCDToHex((ulong)tmparray1[tHead + 121], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 122], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 123], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 124], 2).ToString("00") + "\"";
            string latitude = BCDToHex((ulong)tmparray1[tHead + 125], 2).ToString("00") + "°" + BCDToHex((ulong)tmparray1[tHead + 126], 2).ToString("00") + "'" + BCDToHex((ulong)tmparray1[tHead + 127], 2).ToString("00") + "." + BCDToHex((ulong)tmparray1[tHead + 128], 2).ToString("00") + "\"";
            str1 += "经度：" + (tmparray1[tHead + 119] == 0x56 ? "无效" : longitude) + NewLine;
            str1 += "纬度：" + (tmparray1[tHead + 119] == 0x56 ? "无效" : latitude) + NewLine;
            str1 += "当前时间：20" + BCDToHex((ulong)tmparray1[tHead + 129], 2).ToString("00") + "-" + BCDToHex((ulong)tmparray1[tHead + 130], 2).ToString("00") + "-" + BCDToHex((ulong)tmparray1[tHead + 131], 2).ToString("00") + " "
                        + BCDToHex((ulong)tmparray1[tHead + 132], 2).ToString("00") + ":" + BCDToHex((ulong)tmparray1[tHead + 133], 2).ToString("00") + ":" + BCDToHex((ulong)tmparray1[tHead + 134], 2).ToString("00");
            return str1;
        }

        private static string 记录信息类别代码(int command, byte[] tmparray1, int tHead, int code)
        {
            string str1 = "";
            int timelong;
            int sec;
            int min;
            int hour;
            int day;
            int mon;
            int year;

            string[] typeCode = {"LBJ发送的列车防护报警记录信息","LBJ发送的列车防护报警解除记录信息","LBJ接收的列车防护报警记录信息","LBJ接收的列车防护报警解除记录信息",
                "LBJ接收的道口事故报警记录信息","LBJ接收的道口事故报警解除记录信息","LBJ接收的施工防护报警记录信息","LBJ接收的施工防护报警解除记录信息",
                "LBJ接收的出入库检测命令记录信息","LBJ发送的报警试验记录信息","LBJ发送的列尾信息","LBJ接收的列尾信息","LBJ故障记录信息","维护记录信息"};
            str1 += typeCode[code == 0x11 ? (code - 1) : 13] + NewLine;

            if (code == 0x0d)
            {
                str1 += "故障记录类别：";
                switch (tmparray1[tHead + 6])
                {
                    case 1: str1 += "TAX箱故障"; break;
                    case 2: str1 += "TAX箱恢复正常"; break;
                    case 3: str1 += "记录单元故障"; break;
                    case 4: str1 += "记录单元恢复正常"; break;
                    case 5: str1 += "信道机故障"; break;
                    case 6: str1 += "信道机恢复正常"; break;
                    default: str1 += "类别未定义,检查记录是否有误!"; break;
                }
                timelong = ((tmparray1[tHead + 7] + (tmparray1[tHead + 8] << 8)) + (tmparray1[tHead + 9] << 0x10)) + (tmparray1[tHead + 10] << 0x18);
                sec = timelong & 0x3f;
                min = (timelong >> 6) & 0x3f;
                hour = (timelong >> 12) & 0x1f;
                day = (timelong >> 0x11) & 0x1f;
                mon = (timelong >> 0x16) & 15;
                year = (timelong >> 0x1a) & 0x3f;
                str1 += NewLine + "故障时间或恢复正常时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
            }
            else if (code == 0x11)
            {
                timelong = ((tmparray1[tHead + 6] + (tmparray1[tHead + 7] << 8)) + (tmparray1[tHead + 8] << 0x10)) + (tmparray1[tHead + 9] << 0x18);
                sec = timelong & 0x3f;
                min = (timelong >> 6) & 0x3f;
                hour = (timelong >> 12) & 0x1f;
                day = (timelong >> 0x11) & 0x1f;
                mon = (timelong >> 0x16) & 15;
                year = (timelong >> 0x1a) & 0x3f;
                str1 += "维护时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                //第11 字节开始：符合表 20“数据管理器向LBJ发送读取／设置命令”数据字段内容定义。 (未显示)
            }
            else if (code >= 0x01 && code <= 0x0c)
            {
                timelong = ((tmparray1[tHead + 6] + (tmparray1[tHead + 7] << 8)) + (tmparray1[tHead + 8] << 0x10)) + (tmparray1[tHead + 9] << 0x18);
                sec = timelong & 0x3f;
                min = (timelong >> 6) & 0x3f;
                hour = (timelong >> 12) & 0x1f;
                day = (timelong >> 0x11) & 0x1f;
                mon = (timelong >> 0x16) & 15;
                year = (timelong >> 0x1a) & 0x3f;
                str1 += "时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                str1 += 应用数据帧信息内容(command, tmparray1, tHead, code);
            }
            return str1;
        }

        private static string 应用数据帧信息内容(int command, byte[] tmparray1, int tHead, int code)
        {
            string str1 = "";
            int timelong;
            int sec;
            int min;
            int hour;
            int day;
            int mon;
            int year;

            if (code == 3 | code == 4 || code == 5 || code == 6)
            {
                str1 += string.Format("信息类别：0{0}H", code) + NewLine;
                str1 += "公里标：" + ((tmparray1[tHead + 14] & 0x80) == 1 ? -1 : 1) * (tmparray1[tHead + 12] + tmparray1[tHead + 13] * 256 + (tmparray1[tHead + 14] & 0x3f) * 256 * 256) + "米" + NewLine;
                timelong = ((tmparray1[tHead + 15] + (tmparray1[tHead + 16] << 8)) + (tmparray1[tHead + 17] << 0x10)) + (tmparray1[tHead + 18] << 0x18);
                sec = timelong & 0x3f; min = (timelong >> 6) & 0x3f; hour = (timelong >> 12) & 0x1f;
                day = (timelong >> 0x11) & 0x1f; mon = (timelong >> 0x16) & 15; year = (timelong >> 0x1a) & 0x3f;
                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
            }
            else if (code == 1 || code == 2)
            {
                str1 += "信息类别：" + code.ToString("X2") + NewLine;
                str1 += "车次号："; for (int i = 0; i < 7; i++) str1 += tmparray1[tHead + 11 + i].ToString("X2");
                str1 += NewLine + "机车号："; for (int i = 0; i < 4; i++) str1 += tmparray1[tHead + 21 - i].ToString("X2");
                str1 += NewLine + "公里标：" + ((tmparray1[tHead + 24] & 0x80) == 1 ? -1 : 1) * (tmparray1[tHead + 22] + tmparray1[tHead + 23] * 256 + (tmparray1[tHead + 24] & 0x3f) * 256 * 256) + "米" + NewLine;
                timelong = ((tmparray1[tHead + 25] + (tmparray1[tHead + 26] << 8)) + (tmparray1[tHead + 27] << 0x10)) + (tmparray1[tHead + 28] << 0x18);
                sec = timelong & 0x3f; min = (timelong >> 6) & 0x3f; hour = (timelong >> 12) & 0x1f;
                day = (timelong >> 0x11) & 0x1f; mon = (timelong >> 0x16) & 15; year = (timelong >> 0x1a) & 0x3f;
                str1 += "信息发送时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
                str1 += tmparray1[tHead + 10] == 0x01 ? "启动报警原因：" : "解除报警原因：";
                str1 += tmparray1[tHead + 29] == 1 ? "控制盒1或MMI1" : (tmparray1[tHead + 29] == 2 ? "控制盒2或MMI2" : "主机面板按键");
                str1 += tmparray1[tHead + 10] == 0x01 ? "触发：" : "解除：" + NewLine;
            }
            else if (code == 0x07)
            {
                str1 += "信息类别：07H" + NewLine;
                str1 += "机车号："; for (int i = 0; i < 4; i++) str1 += tmparray1[tHead + 14 - i].ToString("X2");
                str1 += NewLine;
            }
            else if (code == 0x08)
            {
                str1 += "信息类别：08H" + NewLine;
                str1 += "机车号："; for (int i = 0; i < 4; i++) str1 += tmparray1[tHead + 14 - i].ToString("X2");
                str1 += NewLine + "控制盒端口：";
                str1 += tmparray1[tHead + 15] == 0x03 ? "控制盒1或MMI1" : (tmparray1[tHead + 15] == 0x04 ? "控制盒2或MMI2" : "无效");
                str1 += "自检结果：";
                str1 += (tmparray1[tHead + 16] & 0x80) == 0x80 ? "TAX箱连接故障," : "TAX箱连接正常,";
                str1 += (tmparray1[tHead + 16] & 0x40) == 0x40 ? "记录单元故障," : "记录单元正常,";
                str1 += (tmparray1[tHead + 16] & 0x20) == 0x20 ? "信道机故障," : "信道机正常,";
                str1 += (tmparray1[tHead + 16] & 0x10) == 0x10 ? "报警按键故障," : "报警按键正常,";
                str1 += (tmparray1[tHead + 16] & 0x08) == 0x08 ? "列尾排风按键故障," : "列尾排风按键正常,";
                str1 += (tmparray1[tHead + 16] & 0x04) == 0x04 ? "查询风压按键故障," : "查询风压按键正常,";
                str1 += (tmparray1[tHead + 16] & 0x02) == 0x02 ? "列尾确认按键故障" : "列尾确认按键正常";

                str1 += NewLine + "备用电池电压：";
                int vartmp = tmparray1[tHead + 17];
                str1 += vartmp == 0 ? "无备用电池" : ((vartmp / 10).ToString() + "." + (vartmp - vartmp / 10 * 10).ToString()) + NewLine;
            }
            else if (code == 0x09)
            {
                str1 += "机车号："; for (int i = 0; i < 4; i++) str1 += tmparray1[tHead + 13 - i].ToString("X2");
                str1 += NewLine + "命令：";
                switch (tmparray1[tHead + 15])
                {
                    case 0x24: str1 += "排风制动命令"; break;
                    case 0x25: str1 += "应答排风制动命令"; break;
                    case 0x91: str1 += "手动查询风压命令"; break;
                    case 0x92: str1 += "应答手动查询风压命令"; break;
                    case 0x83: str1 += "消号命令"; break;
                    case 0xb5: str1 += "输号命令"; break;
                    case 0xb6: str1 += "应答输号命令"; break;
                    case 0x30: str1 += "风压自动提示"; break;
                    case 0x31: str1 += "应答风压自动提示"; break;
                    case 0xb0: str1 += "电压欠压自动提示"; break;
                    case 0xb1: str1 += "应答电压欠压自动提示"; break;
                    case 0x93: str1 += "自动查询风压命令"; break;
                    case 0x94: str1 += "应答自动查询风压命令"; break;
                    case 0xa1: str1 += "时钟校准信息"; break;
                    case 0xa2: str1 += "应答时钟校准信息"; break;
                    case 0xa3: str1 += "风压校准信息"; break;
                    case 0xa4: str1 += "应答风压校准信息"; break;
                }
                str1 += NewLine + "KLW ID：" + tmparray1[tHead + 17].ToString("X2") + tmparray1[tHead + 18].ToString("X2") + tmparray1[tHead + 18].ToString("X2") + NewLine;
                str1 += "参数：" + tmparray1[tHead + 19].ToString("X2") + tmparray1[tHead + 20].ToString("X2") + NewLine;
                if (tmparray1[tHead + 15] == 0xa1)
                {
                    str1 += "时钟校准信息：" + tmparray1[tHead + 21].ToString("X2") + "-" + tmparray1[tHead + 22].ToString("X2") + "-" + tmparray1[tHead + 23].ToString("X2") + " " +
                        tmparray1[tHead + 24].ToString("X2") + ":" + tmparray1[tHead + 25].ToString("X2") + ":" + tmparray1[tHead + 26].ToString("X2") + NewLine;
                }
            }
            return str1;
        }

        private static string 库检结果(byte[] tmparray1, int tHead)
        {
            string str1 = "";
            string[] manufacturer = { "希电公司", "天津通广", "思科泰", "杭州创联", "新干通", "泉州铁通", "兰新集团", "深圳长龙", "上海复旦", "北京华铁", "北京和利时", "北京世纪东方", "河南辉煌" };

            str1 = "库检结果回复（遥检）：" + NewLine;
            str1 = "厂家：" + ((tmparray1[tHead] >= 1 && tmparray1[tHead] <= 13) ? (manufacturer[tmparray1[tHead] - 1]) : "其他厂家");
            str1 += NewLine + "车次号：" + Encoding.Default.GetString(tmparray1, tHead + 1, 7).Trim().TrimEnd('\0') + NewLine;
            str1 += NewLine + "机车号：" + Encoding.Default.GetString(tmparray1, tHead + 8, 8).Trim().TrimEnd('\0') + NewLine;
            str1 += "GSM-R话音单元模块状态：" + ((tmparray1[tHead + 16] & 0x80) == 0x80 ? "网络注册," : "网络未注册,") + ((tmparray1[tHead + 16] & 0x40) == 0x40 ? "机车号功能号注册," : "机车号功能号未注册,") + ((tmparray1[tHead + 16] & 0x20) == 0x20 ? "车次号功能号注册," : "车次号功能号未注册,") + ((tmparray1[tHead + 16] & 0x01) == 0x01 ? "模块未配置" : "模块已配置") + NewLine;
            str1 += "GSM-R语音场强(十六进制)：" + tmparray1[tHead + 17].ToString("X2") + NewLine;
            str1 += "GSM-R数据单元模块状态：" + ((tmparray1[tHead + 18] & 0x80) == 0x80 ? "网络注册," : "网络未注册,") + ((tmparray1[tHead + 11] & 0x40) == 0x40 ? "已获得IP地址," : "未获得IP地址,") + ((tmparray1[tHead + 11] & 0x01) == 0x01 ? "模块未配置" : "模块已配置") + NewLine;
            str1 += "GSM-R数据场强(十六进制)：" + tmparray1[tHead + 19].ToString("X2") + NewLine;
            str1 += "GPS单元状态：" + ((tmparray1[tHead + 20] & 0x80) == 0x80 ? "GPS数据无效," : "GPS数据有效,") + ((tmparray1[tHead + 20] & 0x40) == 0x40 ? "GPS单元故障," : "GPS单元正常,") + ((tmparray1[tHead + 20] & 0x01) == 0x01 ? "单元未配置" : "单元已配置") + NewLine;
            str1 += "数据采集编码器状态：" + ((tmparray1[tHead + 21] & 0x80) == 0x80 ? "TAX装置编码器数据接收故障," : "TAX装置编码器数据接收正常,") + ((tmparray1[tHead + 21] & 0x01) == 0x01 ? "编码器未配置" : "编码器已配置") + NewLine;
            str1 += "通用机车电台状态：" + ((tmparray1[tHead + 22] & 0x80) == 0x80 ? "机车电台单未配置," : "机车电台单已配置,") + ((tmparray1[tHead + 22] & 0x20) == 0x20 ? "调制解调器故障," : "调制解调器正常,") + ((tmparray1[tHead + 22] & 0x10) == 0x10 ? "单音编/解码电路故障," : "单音编/解码电路正常,") +
                ((tmparray1[tHead + 22] & 0x08) == 0x08 ? "亚音频编/解码电路故障," : "亚音频编/解码电路正常,") + ((tmparray1[tHead + 22] & 0x04) == 0x04 ? "接收机低端故障," : "接收机低端正常,") + ((tmparray1[tHead + 22] & 0x02) == 0x02 ? "接收机高端故障," : "接收机高端正常,") + ((tmparray1[tHead + 22] & 0x01) == 0x01 ? "发射机故障" : "发射机正常") + NewLine;
            str1 += "记录单元状态：" + ((tmparray1[tHead + 23] & 0x80) == 0x80 ? "通信状态故障," : "通信状态正常,") + ((tmparray1[tHead + 23] & 0x10) == 0x10 ? "接收卫星定位信息故障" : "接收卫星定位信息正常") + ((tmparray1[tHead + 23] & 0x01) == 0x01 ? "记录单元未配置" : "记录单元已配置") + NewLine;
            str1 += "MMI状态：" + ((tmparray1[tHead + 24] & 0x80) == 0x80 ? "MMI2通讯故障," : "MMI2通讯正常,") + ((tmparray1[tHead + 24] & 0x40) == 0x40 ? "MMI1通讯故障," : "MMI1通讯正常,") + ((tmparray1[tHead + 24] & 0x01) == 0x01 ? "单配" : "双配") + NewLine;
            str1 += "电池状态：" + ((tmparray1[tHead + 25] & 0x80) == 0x80 ? "电池故障," : "电池正常,") + ((tmparray1[tHead + 25] & 0x01) == 0x01 ? "电池单元未配置" : "电池单元已配置") + NewLine;
            str1 += "800MHz状态：" + ((tmparray1[tHead + 26] & 0x80) == 0x80 ? "TAX装置/DMS设备连接故障," : "TAX装置/DMS设备连接正常,") + ((tmparray1[tHead + 26] & 0x40) == 0x40 ? "记录单元故障," : "记录单元正常,") + ((tmparray1[tHead + 26] & 0x20) == 0x20 ? "信道机故障," : "信道机正常,") + ((tmparray1[tHead + 26] & 0x02) == 0x02 ? "电池状态故障," : "电池状态正常,") +
                ((tmparray1[tHead + 26] & 0x01) == 0x01 ? "LBJ单元未配置，" : "LBJ单元已配置,") + NewLine;
            //+ ((tmparray1[tHead + 19] & 0x02) == 0x02 ? "电台单元未配置," : "电台单元已配置,") + ((tmparray1[tHead + 19] & 0x01) == 0x01 ? "寻呼编码故障" : "寻呼编码正常")
            str1 += "备用电池电压：" + ((tmparray1[tHead + 27] == 0) ? "无备用电池" : (tmparray1[tHead + 27] / 10 + "." + (tmparray1[tHead + 27] - tmparray1[tHead + 27] / 10 * 10).ToString()));
            str1 += "机车端号：\r\n";
            switch (tmparray1[tHead + 28])
            {
                case 0:
                    str1 += "普通机车或动车组" + NewLine;
                    break;
                case 1:
                    str1 += "双端机车A段" + NewLine;
                    break;
                case 2:
                    str1 += "双端机车B段" + NewLine;
                    break;
                default:
                    str1 += "" + NewLine;
                    break;
            }
            str1 += "MMI端口号：\r\n";
            switch (tmparray1[tHead + 29])
            {
                case 3:
                    str1 += "MMI1" + NewLine;
                    break;
                case 4:
                    str1 += "MMI2" + NewLine;
                    break;
                case 2:
                    str1 += "地面遥控" + NewLine;
                    break;
                default:
                    str1 += "" + NewLine;
                    break;
            }
            string my_str = (tmparray1[tHead + 30]).ToString("X2") + (tmparray1[tHead + 30]).ToString("X2");
            str1 += ("库检设备编号：" + (my_str != "0000" ? my_str : "00")) + NewLine;
            return str1;
        }

        private static string ProcessDiaoDu2(byte[] tmparray1, int tHead)
        {
            ulong year;
            ulong mon;
            ulong day;
            ulong hour;
            ulong min;
            ulong sec;
            int numgong;
            float gonglibiao;
            string str1 = "450MHz通信单元和GPRS数据测试出入库检测请求：" + NewLine;
            string cch = Encoding.Default.GetString(tmparray1, tHead + 8, 7);
            string jch = Encoding.Default.GetString(tmparray1, tHead + 15, 8);
            string xxbh = Encoding.Default.GetString(tmparray1, tHead + 24, 6);
            switch (tmparray1[tHead])
            {
                case 0x80:
                    str1 += "向库检设备发送出入库检测请求命令；" + NewLine;
                    break;
                case 0x81:
                    str1 += "对调度命令信息的自动确认信息；" + NewLine;
                    break;
                case 0x82:
                    str1 += "对调度命令信息的签收信息；" + NewLine;
                    break;
                default:
                    str1 += "非协议内信息；" + NewLine;
                    break;
            }
            year = BCDToHex((ulong)tmparray1[tHead + 2], 2);
            mon = BCDToHex((ulong)tmparray1[tHead + 3], 2);
            day = BCDToHex((ulong)tmparray1[tHead + 4], 2);
            hour = BCDToHex((ulong)tmparray1[tHead + 5], 2);
            min = BCDToHex((ulong)tmparray1[tHead + 6], 2);
            sec = BCDToHex((ulong)tmparray1[tHead + 7], 2);
            str1 += "时间：20" + year.ToString("00") + "-" + mon.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00") + NewLine;
            str1 = str1 + "车次号：" + cch + NewLine;
            str1 = str1 + "机车号：" + jch + NewLine;
            string my_str = ((BCDToHex(tmparray1[tHead + 23], 2)).ToString("X2"));
            str1 += ("库检设备编号低字节显示：" + (my_str != "00" ? my_str : "00")) + NewLine;
            str1 += "调度命令信息编号：" + xxbh + NewLine;
            numgong = (tmparray1[tHead + 30] + (tmparray1[tHead + 31] << 8)) + ((tmparray1[tHead + 32] & 0x3f) << 0x10);
            gonglibiao = ((float)(numgong / 100)) / 10f;
            if ((tmparray1[tHead + 32] & 0x80) == 0x80)
            {
                gonglibiao = -1f * gonglibiao;
            }
            string gonglibiao2 = gonglibiao.ToString();
            str1 += "公里表（米）：" + (gonglibiao2 != "9999999" ? gonglibiao2 : "CIR处于编组站状态") + NewLine;
            string longitude1 = BCD2Int(tmparray1[tHead + 33]).ToString() + BCD2Int(tmparray1[tHead + 34]).ToString("00") + "°" + BCD2Int(tmparray1[tHead + 35]).ToString("00") + BCD2Int(tmparray1[tHead + 36]).ToString("00") + BCD2Int(tmparray1[tHead + 37]).ToString("00") + "'" + "\"";
            string latitude1 = BCD2Int(tmparray1[tHead + 38]).ToString("00") + "°" + BCD2Int(tmparray1[tHead + 39]).ToString("00") + BCD2Int(tmparray1[tHead + 40]).ToString("00") + "." + BCD2Int(tmparray1[tHead + 41]).ToString("00") + "'" + "\"";
            if (longitude1 == "FFFFFFFFFF" || latitude1 == "FFFFFFFFFF")
            {

            }
            else
            {
                str1 = (str1 + "经度：" + longitude1 + NewLine) + "纬度：" + latitude1 + NewLine;
            }
            string my_str1 = ((BCDToHex(tmparray1[tHead + 42], 2)).ToString("X2"));
            str1 += ("发令处所编号高字节显示：" + (my_str1 != "00" ? my_str1 : "00")) + NewLine;
            str1 += "机车端号：" + NewLine;
            switch (tmparray1[tHead + 43])
            {
                case 0:
                    str1 += "普通机车或动车组；" + NewLine;
                    break;
                case 1:
                    str1 += "双端机车A段；" + NewLine;
                    break;
                case 2:
                    str1 += "双端机车B段；" + NewLine;
                    break;
            }
            str1 += "MMI端口号：" + NewLine;
            switch (tmparray1[tHead + 44])
            {
                case 3:
                    str1 += "当前为主控状态的MMI端口号是：MMI1" + NewLine;
                    break;
                case 4:
                    str1 += "当前为主控状态的MMI端口号是：MMI2" + NewLine;
                    break;
                default:
                    str1 += "无主控状态MMI" + NewLine;
                    break;
            }

            ulong pz = BCDToHex((ulong)tmparray1[tHead + 0x2D], 2);
            if ((pz >= 1) && (pz <= 8))
            {
                str1 += "当前工作频组：" + pz + NewLine;

            }
            else if (pz == 0x65)
            {
                str1 += "GPS测试：" + NewLine;

            }
            else
            {
                str1 += "无效" + NewLine;

            }
            return str1;
        }

        private static string Process机车端号设置(int command, byte[] tmparray1, int tHead)
        {
            string str1 = "MMI读取/设置机车端号：" + NewLine;
            switch (tmparray1[tHead])
            {
                case 0x01:
                    str1 += "读取机车端号；" + NewLine;
                    break;
                case 0x02:
                    str1 += "设置机车端号；" + NewLine;
                    switch (tmparray1[tHead + 1])
                    {
                        case 0x00:
                            str1 += "普通机车或动车组" + NewLine;
                            break;
                        case 0x01:
                            str1 += "双端机车A段" + NewLine;
                            break;
                        case 0x02:
                            str1 += "双端机车B段" + NewLine;
                            break;
                    }
                    break;
            }
            return str1;
        }
        private static string Process应答机车端号(int command, byte[] tmparray1, int tHead)
        {
            string str1 = "主机应答机车端号：" + NewLine;
            switch (tmparray1[tHead])
            {
                case 0x00:
                    str1 += "普通机车或动车组" + NewLine;
                    break;
                case 0x01:
                    str1 += "双端机车A段" + NewLine;
                    break;
                case 0x02:
                    str1 += "双端机车B段" + NewLine;
                    break;
            }
            return str1;
        }
        private static string Process主机发送给MMI(int command, byte[] tmparray1, int tHead)
        {
            string str1 = "";
            string cchtype = "";

            if (command == 0x91)
            {
                if ((tmparray1[tHead] == 0xff) && (tmparray1[tHead + 1] == 0xff) && (tmparray1[tHead + 2] == 0xff))
                {
                    str1 += "公里标：无效" + NewLine;
                }
                else
                {
                    int numgong = (tmparray1[tHead] + (tmparray1[tHead + 1] << 8)) + ((tmparray1[tHead + 2] & 0x3f) << 0x10);
                    float gonglibiao = ((float)(numgong / 100)) / 10f;
                    if ((tmparray1[tHead + 2] & 0x80) == 0x80)
                    {
                        gonglibiao = -1f * gonglibiao;
                    }
                    str1 += "公里标：" + gonglibiao.ToString() + "公里" + NewLine;
                }
                switch (tmparray1[tHead + 3])
                {
                    case 0x03:
                        str1 += "03端口MMI是主控";
                        break;
                    case 0x04:
                        str1 += "04端口MMI是主控";
                        break;
                    case 0x00:
                        str1 += "无主控";
                        break;
                    default:
                        str1 += "不在协议内";
                        break;
                }
            }
            else if (command == 0x90)
            {
                switch (tmparray1[tHead])
                {
                    case 0x01:
                        str1 += "手动获取车次号获取方式" + NewLine;
                        break;
                    case 0x02:
                        str1 += "自动获取车次号获取方式" + NewLine;
                        break;
                    default:
                        str1 += "不在协议内" + NewLine;
                        break;
                }
                switch (tmparray1[tHead + 1])
                {
                    case 0x00:
                        str1 += "450M列尾开关打开" + NewLine;
                        break;
                    case 0x01:
                        str1 += "450M列尾开关关闭" + NewLine;
                        break;
                    default:
                        str1 += "不在协议内" + NewLine;
                        break;
                }
                switch (tmparray1[tHead + 2])
                {
                    case 0x00:
                        str1 += "GPRS活动性检测开关打开" + NewLine;
                        break;
                    case 0x01:
                        str1 += "GPRS活动性检测开关关闭" + NewLine;
                        break;
                    default:
                        str1 += "不在协议内" + NewLine;
                        break;
                }
                switch (tmparray1[tHead + 3])
                {
                    case 0x00:
                        str1 += "调度命令优先显示开关打开" + NewLine;
                        break;
                    case 0x01:
                        str1 += "调度命令优先显示开关关闭" + NewLine;
                        break;
                    default:
                        str1 += "不在协议内" + NewLine;
                        break;
                }
            }
            else if (command == 0x92)
            {
                switch (tmparray1[tHead])
                {
                    case 0x00:
                        str1 += "库检结果发送成功";
                        break;
                    case 0x01:
                        str1 += "库检结果发送失败";
                        break;
                    default:
                        str1 += "不在协议内";
                        break;
                }
            }
            else if (command == 0x56)
            {
                switch (tmparray1[tHead])
                {
                    case 0x01:
                        str1 += "主机当前车次号获取方式为手动获取" + NewLine;
                        break;
                    case 0x02:
                        str1 += "主机当前车次号获取方式为自动获取" + NewLine;
                        break;
                    default:
                        str1 += "不在协议内" + NewLine;
                        break;
                }
                switch (tmparray1[tHead + 1])
                {
                    case 0x00:
                        str1 += "不需要司机确认车次号" + NewLine;
                        break;
                    case 0x01:
                        str1 += "需要司机确认车次号，车次号为：" + NewLine;
                        cchtype = Encoding.Default.GetString(tmparray1, tHead + 1, 4).Trim();
                        int cchnum = (tmparray1[tHead + 2] + (tmparray1[tHead + 3] << 8)) + (tmparray1[tHead + 4] << 0x10);
                        str1 += "车次号：" + cchtype + cchnum.ToString() + NewLine;
                        break;
                    default:
                        str1 += "不在协议内" + NewLine;
                        break;
                }
            }
            return str1;
        }

        private static string Process查询功能状态配置应答(int command, byte[] tmparray1, int tHead)
        {
            string str1 = "";
            switch (tmparray1[tHead])
            {
                case 1:
                    str1 += " 手动获取车次号获取方式" + NewLine;
                    break;
                case 2:
                    str1 += "自动获取车次号获取方式" + NewLine;
                    break;
            }
            switch (tmparray1[tHead + 1])
            {
                case 0:
                    str1 += "打开450M列尾开关" + NewLine;
                    break;
                case 1:
                    str1 += "关闭450M列尾开关" + NewLine;
                    break;
            }
            switch (tmparray1[tHead + 2])
            {
                case 0:
                    str1 += "打开GPRS活动性检测开关" + NewLine;
                    break;
                case 1:
                    str1 += "关闭GPRS活动性检测开关" + NewLine;
                    break;
            }
            switch (tmparray1[tHead + 3])
            {
                case 0:
                    str1 += "打开调度命令优先显示开关" + NewLine;
                    break;
                case 1:
                    str1 += "关闭调度命令优先显示开关" + NewLine;
                    break;
            }
            return str1;
        }

        private static string Process功能状态配置(int command, byte[] tmparray1, int tHead)
        {
            string str1 = "";
            switch (tmparray1[tHead])
            {
                case 0x03:
                    str1 = "功能状态配置" + NewLine;
                    switch (tmparray1[tHead + 1])
                    {
                        case 1:
                            str1 += "手动获取车次号获取方式" + NewLine;
                            break;
                        case 2:
                            str1 += "自动获取车次号获取方式" + NewLine;
                            break;
                    }
                    switch (tmparray1[tHead + 2])
                    {
                        case 0:
                            str1 += "打开450M列尾开关" + NewLine;
                            break;
                        case 1:
                            str1 += "关闭450M列尾开关" + NewLine;
                            break;
                    }
                    switch (tmparray1[tHead + 3])
                    {
                        case 0:
                            str1 += "打开GPRS活动性检测开关" + NewLine;
                            break;
                        case 1:
                            str1 += "关闭GPRS活动性检测开关" + NewLine;
                            break;
                    }
                    switch (tmparray1[tHead + 4])
                    {
                        case 0:
                            str1 += "打开调度命令优先显示开关" + NewLine;
                            break;
                        case 1:
                            str1 += "关闭调度命令优先显示开关" + NewLine;
                            break;
                    }
                    break;
                case 0x02:
                    str1 = "查询功能状态配置" + NewLine;
                    break;
            }
            return str1;
        }

        //private static string ProcessMMItoMMI(int command, byte[] tmparray1, int tHead)
        //{
        //    string str1 = "";
        //    if (tmparray1[tHead] == 0xab)
        //        str1 = "MMI向另一个MMI发送调试信息:" + NewLine;
        //    int tscs = (int)((((BCDToHex((ulong)tmparray1[tHead + 1], 2) * 0xf4240) + (BCDToHex((ulong)tmparray1[tHead + 2], 2) * 0x2710)) + (BCDToHex((ulong)tmparray1[tHead + 3], 2) * 100)) + BCDToHex((ulong)tmparray1[tHead + 4], 2));
        //    int jmcs = (int)((BCDToHex((ulong)tmparray1[tHead + 5], 2) * 100) + (BCDToHex((ulong)tmparray1[tHead + 6], 2)));
        //    string tscs1 = tscs.ToString();
        //    string jmcs1 = jmcs.ToString();
        //    str1 += "调试参数为tscs1,界面参数为jmcs1";
        //    return str1;
        //}

        public static string GetExplainData(int sourceport, int destport, int comstyle, int command, byte[] tmparray1, int tHead)       //源端口,远程端口,业务类型,命令,,.
        {
            string str1 = "";
            int tlen = (tmparray1[2] * 0x100) + tmparray1[3];
            if (sourceport == 0x02 && destport == 0x01 && comstyle == 0x06 && command == 0x3C)
            {
                str1 = "MMI请求擦除调度命令";
                return str1;
            }
            if (sourceport == 0x03 && destport == 0x13 && comstyle == 0x0B && command == 0xF2)
            {
                str1 = "MMI向LBJ发送800M货列尾设置状态" + NewLine;
                switch (tmparray1[tHead])
                {
                    case 0:
                        str1 += "普通状态;";
                        break;
                    case 0x4b:
                        str1 += "800M货列尾状态;";
                        break;
                    default:
                        str1 += "";
                        break;
                }
                return str1;
            }
            if (sourceport == 0x01 && destport == 0x05 && comstyle == 0x13 && command == 0xF2)
            {
                str1 = "主机发给450M单元的GPS状态：" + NewLine;
                switch (tmparray1[tHead] & 0x01)
                {
                    case 1:
                        str1 += "GPS连接";
                        break;
                    case 0:
                        str1 += "GPS未连接";
                        break;
                    default:
                        str1 += "";
                        break;
                }
                switch (tmparray1[tHead] & 0x02)
                {
                    case 1:
                        str1 += "GPS数据有效";
                        break;
                    case 0:
                        str1 += "GPS数据无效";
                        break;
                    default:
                        str1 += "";
                        break;
                }
                return str1;
            }
            if ((sourceport == 0x03 || sourceport == 0x04 || sourceport == 0x02) && destport == 0x01 && comstyle == 0x03 && command == 0xE1)
            {
                str1 = Process功能状态配置(command, tmparray1, tHead); return str1;
            }

            if ((sourceport == 0x03 || sourceport == 0x04 || sourceport == 0x02) && destport == 0x01 && comstyle == 0x01 && command == 0xC0)
            {
                str1 = Process机车端号设置(command, tmparray1, tHead); return str1;
            }


            if ((destport == 0x03 || destport == 0x04 || destport == 0x02) && sourceport == 0x01 && comstyle == 0x01 && command == 0xC1)
            {
                str1 = Process应答机车端号(command, tmparray1, tHead); return str1;
            }


            //if ((destport == 0x03 || destport == 0x04) && (sourceport == 0x03 || sourceport == 0x04) && comstyle == 0x03 && command == 0x19)
            //{
            //    str1 = ProcessMMItoMMI(command, tmparray1, tHead); return str1;
            //}


            if ((sourceport == 0x03 || sourceport == 0x04 || sourceport == 0x02) && destport == 0x01 && comstyle == 0x01 && command == 0xC0)
            {
                str1 = Process机车端号设置(command, tmparray1, tHead); return str1;
            }


            if ((sourceport == 6) && (tlen == 0x9e))
            {
                str1 = ProcessGps(command, tmparray1, tHead); return str1;
            }
            if (sourceport == 0x01 && destport == 0x02 && comstyle == 0x03 && (command == 0x91 || command == 0x92))
            {
                str1 = Process主机发送给MMI(command, tmparray1, tHead); return str1;
            }


            if (((destport == 1) && (sourceport == 5)) && (comstyle == 0x71))
            {
                str1 = ProcessFromDiaoDU(0xf1, tmparray1, tHead + 11); return str1;
            }
            if (sourceport == 1)
            {
                if ((comstyle == 6) && (command == 0x61) && (destport == 0x31))
                {
                    str1 = ProcessDiaoDu2(tmparray1, tHead);
                }
                if ((comstyle == 5) && (command == 0x21))
                {
                    str1 = TAX_Data(command, tmparray1, tHead);
                    str1 += TAX_Extend(command, tmparray1, tHead);
                    return str1;
                }

                if ((comstyle == 7 && command == 0x02) || (comstyle == 7 && command == 0x03))
                {
                    str1 = TAX_Data(command, tmparray1, tHead);
                    str1 += TAX_Extend(command, tmparray1, tHead);
                    return str1;
                }
                if ((comstyle == 3) && (command == 0xff))
                {
                    str1 = "主机向外部录音口发送录音信号" + NewLine;
                    if (tmparray1[tHead] == 0) str1 = str1 + "停止录音";
                    else str1 = str1 + "开始录音";
                    return str1;
                }
            }

            if ((sourceport == 7) || (destport == 7))
            {
                str1 = ProcessToRecordUnit(command, tmparray1, tHead); return str1;
            }
            if ((sourceport == 0x13) || (destport == 0x13))     //LBJ数据
            {
                str1 = ProcessLbjData(command, tmparray1, tHead, sourceport, comstyle); return str1;
            }
            if ((destport == 5) && (sourceport == 1))
            {
                str1 = ProcessMainTo450(command, tmparray1, tHead); return str1;
            }
            if ((destport < 2) || (destport > 4))
            {
                if ((sourceport >= 2) && (sourceport <= 4))
                {
                    switch (comstyle)
                    {
                        case 3:
                            if ((sourceport == 3 || sourceport == 4) && destport == 6 && command == 0x4e)      //MMI向卫星定位单元发送线路设置信息
                            {
                                str1 = "MMI向卫星定位单元发送线路设置信息\r\n命令解析：\r\n";
                                str1 += "模式：" + GetModeString(tmparray1[tHead]);
                                str1 += "线路名称：" + Encoding.Default.GetString(tmparray1, tHead + 1, 8) + NewLine;
                                str1 += "区段名称：" + Encoding.Default.GetString(tmparray1, tHead + 9, 21) + NewLine;
                                return str1;
                            }
                            else if ((sourceport == 3 || sourceport == 4) && destport == 6 && command == 0x4f)      //MMI询问当前区域信息
                            {
                                str1 = "MMI询问当前区域信息\r\n";
                                return str1;
                            }
                            else return ProcessToMain(command, tmparray1, tHead);
                        case 4: return ProcessToLieWei(command, tmparray1, tHead);
                        case 5: return str1;
                        case 6: return ProcessToDiaoDu(command, tmparray1, tHead);
                    }
                }
                //return str1;
            }
            switch (comstyle)
            {
                case 1:
                    if ((sourceport == 0x03 || sourceport == 0x04) && destport == 1 && command == 0xC0)
                    {
                        str1 = "MMI读取/设置机车端号；\r\n";
                        switch (tmparray1[tHead])
                        {
                            case 0x01:
                                str1 += "读取机车端号；" + NewLine;
                                break;
                            case 0x02:
                                str1 += "设置机车端号；" + NewLine;
                                switch (tmparray1[tHead + 1])
                                {
                                    case 0x01:
                                        str1 += "双端机车A段；" + NewLine;
                                        break;
                                    case 0x02:
                                        str1 += "双端机车B段；" + NewLine;
                                        break;
                                    case 0x00:
                                        str1 += "普通机车或动车组；" + NewLine;
                                        break;
                                    default:
                                        str1 += "不在协议内；" + NewLine;
                                        break;
                                }
                                break;
                            default:
                                str1 += "不在协议内；" + NewLine;
                                break;
                        }

                    }
                    if ((destport == 0x03 || destport == 0x04) && sourceport == 0x01 && command == 0xC1)
                    {
                        switch (tmparray1[tHead])
                        {
                            case 0x01:
                                str1 += "机车端号：双端机车A段" + NewLine;
                                break;
                            case 0x02:
                                str1 += "机车端号：双端机车B段" + NewLine;
                                break;
                            case 0x00:
                                str1 += "机车端号：普通机车或动车组" + NewLine;
                                break;
                            default:
                                str1 += "不在协议内；" + NewLine;
                                break;
                        }
                    }
                    return str1;

                case 2:     //补充公用信息BQ
                    if (sourceport == 6 && destport == 2 && (command == 1 || command == 2 || command == 0xa1))      //卫星定位单元通知MMI人工选择/确认线路
                    {
                        switch (command)
                        {
                            case 1: str1 = "卫星定位单元通知MMI人工选择线路\r\n命令解析：\r\n"; break;
                            case 2: str1 = "卫星定位单元通知MMI人工确认线路\r\n命令解析：\r\n"; break;
                            case 0xa1: str1 = "GPS单元应答上下行信息\r\n命令解析：\r\n"; break;
                        }
                        if (tmparray1.Length <= 14) return str1;
                        int count = 1;
                        int nameLength = 0;
                        int nameIndex = 0;
                        for (int i = 0; i < tmparray1.Length - 14; i++)
                        {
                            if (tmparray1[tHead + i] != 0x3b) nameLength++;
                            else
                            {
                                str1 += string.Format("线路{0}模式：", count) + GetModeString(tmparray1[tHead + nameIndex]);
                                str1 += string.Format("线路{0}名称与区段名称：", count) + Encoding.Default.GetString(tmparray1, tHead + nameIndex + 1, nameLength - 1) + NewLine + NewLine;

                                nameIndex += nameLength + 1;
                                nameLength = 0;
                                count++;
                            }
                        }
                        return str1;
                    }
                    else if (sourceport == 6 && destport == 2 && command == 0x4e)
                    {
                        str1 = "卫星定位单元向MMI询问当前线路名称\r\n";
                        return str1;
                    }
                    else if (sourceport == 6 && destport == 2 && command == 3)
                    {
                        str1 = "卫星定位单元向MMI发送退出线路选择区域信息\r\n";
                        return str1;
                    }
                    break;
                case 3:
                    if (sourceport == 6 && (destport == 2 || destport == 3 || destport == 4) && command == 1)          //BQ:增加0301解析
                        str1 = "应答信息" + NewLine + "命令解析：" + NewLine + "发送方命令字字节：" + tmparray1[tHead].ToString("X2") + "H" + NewLine + "发送方第一字节：" + tmparray1[tHead + 1].ToString("X2") + "H" + NewLine;
                    else if (sourceport != 1 || destport != 5)       //其他
                        str1 = ProcessFromMain(command, tmparray1, tHead);
                    return str1;
                case 4:
                    return ProcessFromLieWei(command, tmparray1, tHead);
                case 6:
                    return ProcessFromDiaoDU(command, tmparray1, tHead);
                case 0xa1:
                    if (command == 0xfe)
                    {
                        str1 = "450M向主机和MMI返回工作模式" + NewLine + "工作模式："/* + tmparray1[tHead].ToString("X2") + ","*/ ;
                        if (GetModeString(tmparray1[tHead]) == NewLine) str1 = str1.TrimEnd(',');
                        str1 += GetModeString(tmparray1[tHead]);
                    }
                    return str1;
                case 0x0b:
                    if (command == 0x48) return str1 = ProcessGps(command, tmparray1, tHead);
                    else return str1;
            }
            return str1;
        }
    }
}

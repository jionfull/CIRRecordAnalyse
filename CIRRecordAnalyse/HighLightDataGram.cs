using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CIRRecordAnalyse
{
    class HighLightDataGram
    {
        public static Point CaretToBytes(string originData, int lineNum)    //原始数据(字符串),行数->返回在数据段的字节起始位置(从0开始)以及长度.
        {
            int addLen1 = Convert.ToInt32(originData.Substring(15, 2), 16);                                         //地址长度(0或4)
            int addLen2 = Convert.ToInt32(originData.Substring((addLen1 + 7) * 3, 2), 16);
            int dataHead = addLen1 + addLen2 + 10;
            int type = int.Parse(originData.Substring((8 + addLen1 + addLen2) * 3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int command = int.Parse(originData.Substring((9 + addLen1 + addLen2) * 3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int sourcePort = int.Parse(originData.Substring(12, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            int destPort = int.Parse(originData.Substring((6 + addLen1) * 3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            int len = 0;
            int len1 = 0;
            int len2 = 0;
            int len3 = 0;

            switch (type)
            {
                //-----------------------------------------------0x01------------------------------------------------
                #region 维护
                case 0x01:
                    switch (command)
                    {
                        case 0x01:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x33:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x35:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 6);
                                default: return new Point(1, 0);
                            }
                        case 0x41:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x91:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 6);
                                default: return new Point(1, 0);
                            }
                        case 0xaa:
                            switch (lineNum)
                            {
                                case 2:
                                    len = 0;
                                    while (originData.Substring((dataHead + len) * 3, 2).ToLower() != "3b" && len <= 200) len++;
                                    return new Point(1, len + 1);
                                default: return new Point(1, 0);
                            }
                        case 0xfb:
                            if (lineNum >= 2 && lineNum <= 8)
                                return new Point(1, 1);
                            else if (lineNum == 9)
                                return new Point(2, 2);
                            else return new Point(1, 0);
                        default: return new Point(1, 0);
                    }
                #endregion
                //-----------------------------------------------0x02------------------------------------------------              
                #region 公共信息
                case 0x02:          //公共信息
                    switch (command)
                    {
                        case 0x00:
                            if (originData.Substring((dataHead + 2) * 3, 2) == "41")
                                switch (lineNum)
                                {
                                    case 3: return new Point(1, 1);
                                    case 4: return new Point(3, 1);
                                    case 5: return new Point(4, 8);
                                    case 6: return new Point(12, 2);
                                    case 7: return new Point(14, 21);
                                    case 8: return new Point(35, 1);
                                    case 9: return new Point(36, 5);
                                    case 10: return new Point(41, 4);
                                    case 11: return new Point(45, 6);
                                    case 12: return new Point(51, 8);
                                    case 14: return new Point(59, 8);
                                    case 15: return new Point(67, 2);
                                    case 16: return new Point(69, 8);
                                    case 18: return new Point(77, 8);
                                    case 19: return new Point(85, 2);
                                    case 20: return new Point(87, 8);
                                    case 22: return new Point(95, 8);
                                    case 23: return new Point(103, 2);
                                    case 24: return new Point(105, 8);
                                    case 26: return new Point(113, 8);
                                    case 27: return new Point(121, 2);
                                    case 28: return new Point(123, 8);
                                    default: return new Point(1, 0);
                                }
                            else        //0x56
                            {
                                switch (lineNum)
                                {
                                    case 3: return new Point(1, 1);
                                    case 4: return new Point(3, 1);
                                    case 6: return new Point(45, 6);
                                    default: return new Point(1, 0);
                                }
                            }
                        case 0x01:
                            len = 0;
                            for (int i = 0; i < lineNum / 3 - 1; i++)
                            {
                                while (originData.Substring((dataHead + len) * 3, 2).ToLower() != "3b" && len <= 200) len++;
                                len++;
                            }
                            if (lineNum % 3 == 0 && len < originData.Length / 3 - dataHead - 4) return new Point(len + 1, 1);
                            else if (lineNum % 3 == 1 && lineNum != 1)
                            {
                                len1 = 1;
                                while (originData.Substring((dataHead + len1 + 1) * 3, 2).ToLower() != "3b" && len <= 200) len1++;
                                return new Point(len + 2, len1);
                            }
                            else return new Point(1, 0);
                        case 0x02:         //As up
                            len = 0;
                            for (int i = 0; i < lineNum / 3 - 1; i++)
                            {
                                while (originData.Substring((dataHead + len) * 3, 2).ToLower() != "3b" && len <= 200) len++;
                                len++;
                            }
                            if (lineNum % 3 == 0 && len < originData.Length / 3 - dataHead - 4) return new Point(len + 1, 1);
                            else if (lineNum % 3 == 1 && lineNum != 1)
                            {
                                len1 = 1;
                                while (originData.Substring((dataHead + len1 + 1) * 3, 2).ToLower() != "3b" && len <= 200) len1++;
                                return new Point(len + 2, len1);
                            }
                            else return new Point(1, 0);
                        default: return new Point(1, 0);
                    }
                #endregion
                //-----------------------------------------------0x03------------------------------------------------
                #region 调度通信
                case 0x03:          //调度通信
                    switch (command)
                    {
                        case 0x01:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x02:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x06:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x08:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, originData.Length / 3 - dataHead - 4 - 1);
                                default: return new Point(1, 0);
                            }
                        case 0x09:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x0a:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x0b:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x11:
                            len = 1;
                            while (originData.Substring((dataHead + len) * 3, 2).ToLower() != "3b") len++;
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, len);
                                default: return new Point(1, 0);
                            }
                        case 0x13:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                case 5:
                                    len = 1;
                                    while (originData.Substring((dataHead + len + 1) * 3, 2).ToLower() != "3b") len++;
                                    return new Point(3, len - 1);
                                default: return new Point(1, 0);
                            }
                        case 0x15:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x16:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                case 5:
                                    len = 1;
                                    while (originData.Substring((dataHead + len + 1) * 3, 2).ToLower() != "3b") len++;
                                    return new Point(3, len - 1);
                                default: return new Point(1, 0);
                            }
                        case 0x17:
                            switch (lineNum)
                            {
                                case 3:
                                    len = 1;
                                    while (originData.Substring((dataHead + len - 1) * 3, 2).ToLower() != "3b") len++;
                                    return new Point(1, len);
                                default: return new Point(1, 0);
                            }
                        case 0x20:
                            switch (lineNum)
                            {
                                case 3:
                                    len = 1;
                                    while (originData.Substring((dataHead + len - 1) * 3, 2).ToLower() != "3b") len++;
                                    return new Point(1, len);
                                default: return new Point(1, 0);
                            }
                        case 0x22:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x2e:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x2f:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 4);
                                default: return new Point(1, 0);
                            }
                        case 0x30:
                            switch (lineNum)
                            {
                                case 1:
                                    if (originData.Substring(18 + addLen1 * 3, 2) == "02")
                                        return new Point(1, 1);
                                    else return new Point(1, 0);
                                case 3:
                                    if (originData.Substring(18 + addLen1 * 3, 2) == "01")
                                        return new Point(1, 7);
                                    //else if
                                    else return new Point(1, 0);
                                default: return new Point(1, 0);
                            }
                        case 0x35:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x42:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x43:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x46:
                            {
                                if (originData.Substring(12, 2) == "01" && originData.Substring(18 + addLen1 * 3, 2) == "02")
                                    switch (lineNum)
                                    {
                                        case 3: return new Point(1, 1);
                                        default: return new Point(1, 0);
                                    }
                                else
                                    switch (lineNum)
                                    {
                                        case 3:
                                            if (originData.Substring(dataHead * 3, 2) != "03")
                                                return new Point(1, 1);
                                            else return new Point(2, 1);
                                        case 4:
                                            if (originData.Substring(dataHead * 3, 2) != "03")
                                                return new Point(1, 0);
                                            else return new Point(3, 1);
                                        default: return new Point(1, 0);
                                    }
                            }
                        case 0x49:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x4a:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x4b:
                            switch (lineNum)
                            {
                                //case 3: return new Point(1, 1);
                                //case 4:
                                //    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x80) == 0x80)//success
                                //        return new Point(1, 1);
                                //    else return new Point(2, 1);
                                //case 5:
                                //    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x80) == 0x80)
                                //        return new Point(2, 1);
                                //    else return new Point(1, 1);
                                case 3: return new Point(1, 1);
                                case 4:
                                    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x40) == 0x40) return new Point(1, 1);
                                    else return new Point(1, 0);
                                case 5:
                                    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x1f) != 0) return new Point(1, 1);
                                    else return new Point(1, 0);
                                case 6: return new Point(2, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x4c:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x4e:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 8);
                                case 5: return new Point(10, 21);
                                default: return new Point(1, 0);
                            }
                        case 0x52:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x53:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, originData.Length / 3 - dataHead - 4 - 1);
                                default: return new Point(1, 0);
                            }
                        case 0x54:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                case 5: return new Point(2, 1);
                                case 6: return new Point(2, 1);
                                case 7: return new Point(3, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x55:
                            len1 = 1;
                            len2 = 1;
                            len3 = 1;
                            while (originData.Substring((dataHead + len1 + 17) * 3, 2).ToLower() != "3b") len1++;
                            while (originData.Substring((dataHead + len1 + len2 + 17) * 3, 2).ToLower() != "3b") len2++;
                            while (originData.Substring((dataHead + len1 + len2 + len3 + 17) * 3, 2).ToLower() != "3b") len3++;
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                case 3: return new Point(2, 1);
                                case 4: return new Point(2, 1);
                                case 5: return new Point(2, 1);
                                case 6: return new Point(2, 1);
                                case 7: return new Point(2, 1);
                                case 8: return new Point(2, 1);
                                case 9: return new Point(2, 1);
                                case 10: return new Point(3, 1);
                                case 11: return new Point(4, 1);
                                case 12: return new Point(5, 1);
                                case 13: return new Point(7, 4);
                                case 14: return new Point(11, 4);
                                case 15: return new Point(15, 4);
                                case 16: return new Point(19, len1);
                                case 17: return new Point(19 + len1, len2);
                                case 18: return new Point(19 + len1 + len2, len3);
                                default: return new Point(1, 0);
                            }
                        case 0x56://Boki[131105]:新增MMI的协议
                            {
                                switch (lineNum)
                                {
                                    case 2: return new Point(1, 1);
                                    case 3: return new Point(2, 1);
                                    case 4:
                                        {
                                            if (originData.Substring((dataHead + 1) * 3, 2).ToLower() == "00")
                                                return new Point(3, 0);
                                            else if (originData.Substring((dataHead + 1) * 3, 2).ToLower() == "01")
                                            {
                                                int cchLength = originData.TrimEnd(' ').Split(' ').Length - dataHead - 2 - 4;
                                                return new Point(3, cchLength);
                                            }
                                            else
                                                return new Point(3, 0);
                                        }
                                    default: return new Point(1, 0);
                                }
                            }
                        case 0x58:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(3, 1);
                                case 5:
                                    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x80) == 0)
                                        if (Convert.ToInt32(originData.Substring(dataHead * 3 + 3, 2), 16) < 1 || Convert.ToInt32(originData.Substring(dataHead * 3 + 3, 2), 16) > 4)
                                            return new Point(4, originData.Length / 3 - dataHead - 4 - 3);
                                        else return new Point(2, 1);
                                    else return new Point(4, originData.Length / 3 - dataHead - 4 - 3);
                                case 6:
                                    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x80) == 0)
                                        return new Point(4, originData.Length / 3 - dataHead - 4 - 3);
                                    else return new Point(1, 0);
                                default: return new Point(1, 0);
                            }
                        case 0x59:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(3, 1);
                                case 5:
                                    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x80) == 0x80)//机车号成功
                                        return new Point(4, originData.Length / 3 - dataHead - 7);
                                    else
                                        return new Point(2, 1);                     //失败原因
                                case 6:
                                    if ((Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) & 0x80) != 0x80)//机车号失败
                                        return new Point(4, originData.Length / 3 - dataHead - 7);
                                    else return new Point(1, 0);
                                default: return new Point(1, 0);
                            }
                        case 0x5a:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x5b:
                            if (lineNum >= 3 && lineNum <= 6) return new Point(1, 1);
                            else return new Point(1, 0);
                        case 0x5d:                                      //主机向MMI通知GSMR当前的通话列表
                            if (originData.Length > 42)
                            {
                                switch (lineNum - 2 - (lineNum - 2) / 4 * 4)
                                {
                                    case 1:
                                        string oriDataTmp = originData;
                                        len = 0;
                                        for (int i = 0; i < (lineNum - 3) / 4; i++)
                                        {
                                            len += oriDataTmp.Length - oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("3b") + 3).Length;
                                            oriDataTmp = oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("3b") + 3);
                                        }
                                        len = len == 0 ? 0 : len / 3 - dataHead;           //跳过的字节(不包含dataHead)
                                        if (len >= originData.Length / 3 - dataHead - 4)
                                            return new Point(1, 0);
                                        else return new Point(len + 1, 1);
                                    case 2:
                                        oriDataTmp = originData;
                                        len = 0;
                                        for (int i = 0; i < (lineNum - 3) / 4; i++)
                                        {
                                            len += oriDataTmp.Length - oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("3b") + 3).Length;
                                            oriDataTmp = oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("3b") + 3);
                                        }
                                        len = len == 0 ? 0 : len / 3 - dataHead;           //跳过的字节
                                        return new Point(len + 2, 1);
                                    case 3:
                                        oriDataTmp = originData;
                                        len = 0;
                                        for (int i = 0; i < (lineNum - 3) / 4 * 2 + 1; i++)
                                        {
                                            len += oriDataTmp.Length - oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c") + 3).Length;
                                            string str = oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c") + 3);
                                            oriDataTmp = oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c") + 3);
                                        }
                                        len = len / 3 - dataHead;           //跳过的字节
                                        string str1 = oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c"));
                                        int a = str1.Length / 3 + 1;
                                        return new Point(len + 1, oriDataTmp.Length / 3 - (oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c")).Length) / 3);
                                    case 0:
                                        oriDataTmp = originData;
                                        len = 0;
                                        for (int i = 0; i < (lineNum - 3) / 4 * 2 + 2; i++)
                                        {
                                            len += oriDataTmp.Length - oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c") + 3).Length;
                                            string str = oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c") + 3);
                                            oriDataTmp = oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("2c") + 3);
                                        }
                                        len = len / 3 - dataHead;           //跳过的字节
                                        return new Point(len + 1, oriDataTmp.Length / 3 - (oriDataTmp.Substring(oriDataTmp.ToLower().IndexOf("3b")).Length) / 3);
                                    default: return new Point(1, 0);
                                }
                            }
                            else return new Point(1, 0);
                        case 0x5f:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 8);
                                case 5: return new Point(10, 1);
                                case 6: return new Point(11, 1);
                                case 7: return new Point(12, 1);
                                case 8: return new Point(13, 1);
                                case 9: return new Point(14, 1);
                                case 10: return new Point(15, 1);
                                case 11: return new Point(16, 1);
                                case 12: return new Point(17, 1);
                                case 13: return new Point(18, 1);
                                case 14: return new Point(19, 1);
                                case 15: return new Point(20, 1);
                                default: return new Point(1, 0);
                            }
                        case 0xe0:
                            if (destPort == 1)          //toMain
                                switch (lineNum)
                                {
                                    case 2: return new Point(1, 1);
                                    case 3:
                                        if (originData.Substring((10 + addLen1 + addLen2) * 3, 2) == "00")
                                            return new Point(1, 0);
                                        else return new Point(2, originData.Length / 3 - (14 + addLen1 + addLen2) - 1);
                                    default: return new Point(1, 0);
                                }
                            else if (destPort != 1 && sourcePort == 1)
                            {
                                len = 0;
                                switch (lineNum)
                                {
                                    case 1: return new Point(1, 1);
                                    case 2:
                                        while (originData.Substring((dataHead + 1 + len) * 3, 2).ToLower() != "2c" && len <= 200) len++;
                                        return new Point(2, len);
                                    case 3:
                                        len = len1 = 0;
                                        for (int i = 0; i <= 1; i++)
                                        {
                                            while (originData.Substring((dataHead + 1 + len) * 3, 2).ToLower() != "2c" && len <= 200) len++;
                                            len++;
                                            if (i == 0) len1 = len;
                                        }
                                        return new Point(1 + len1 + 1, len - len1 - 1);
                                    case 4:
                                        len = len1 = 0;
                                        for (int i = 0; i <= 2; i++)
                                        {
                                            while (originData.Substring((dataHead + 1 + len) * 3, 2).ToLower() != "2c" && len <= 200) len++;
                                            len++;
                                            if (i == 1) len1 = len;
                                        }
                                        return new Point(1 + len1 + 1, len - len1 - 1);
                                    case 5:
                                        len = 0;
                                        for (int i = 0; i <= 2; i++)
                                        {
                                            while (originData.Substring((dataHead + 1 + len) * 3, 2).ToLower() != "2c" && len <= 200) len++;
                                            len++;
                                        }
                                        return new Point(1 + len + 1, originData.Length / 3 - 4 - dataHead - len - 1);
                                    default: return new Point(1, 0);
                                }
                            }
                            else return new Point(1, 0);
                        case 0xe1: //Boki[131105]:
                            {
                                if (lineNum == 1) return new Point(1, 1);
                                else if (lineNum == 3)
                                {
                                    if (originData.Substring(dataHead * 3, 2).ToLower() == "01")
                                    {
                                        return new Point(2, 1);
                                    }
                                    else
                                        return new Point(2, 0);
                                }
                                return new Point(1, 0);
                            }
                        case 0xe2:
                            if (destPort != 1)
                            {
                                switch (lineNum)
                                {
                                    case 2: return new Point(1, 1);
                                    case 3: return new Point(2, 1);
                                    case 4:
                                        if ((Convert.ToInt32(originData.Substring((dataHead + 2) * 3, 2), 16) & 2) == 2) return new Point(3, 1);
                                        else if ((Convert.ToInt32(originData.Substring((dataHead + 2) * 3, 2), 16) & 1) == 1) return new Point(3, 7);
                                        else return new Point(1, 0);
                                    default: return new Point(1, 0);
                                }
                            }
                            else return new Point(1, 0);
                        case 0xff:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        default: return new Point(1, 0);
                    }
                #endregion
                //-----------------------------------------------0x04------------------------------------------------
                #region 列尾风压
                case 0x04:          //列尾风压
                    switch (command)
                    {
                        case 0x21:
                        case 0x22:
                        case 0x23:
                        case 0x24:
                        case 0x25:
                        case 0x26:
                        case 0x27:
                        case 0x28:
                            if (sourcePort == 5 || sourcePort == 0x26)
                            {
                                if (command >= 0x21 && command <= 0x23)
                                    switch (lineNum)
                                    {
                                        case 3: return new Point(1, 4);
                                        case 4: return new Point(16, 4);
                                        case 5: return new Point(5, 2);
                                        case 6: return new Point(7, 5);
                                        case 7: return new Point(12, 4);
                                        default: return new Point(1, 0);
                                    }
                                else if (command == 0x24 || command == 0x26)
                                    switch (lineNum)
                                    {
                                        case 3: return new Point(1, 4);
                                        case 4: return new Point(14, 4);
                                        case 5: return new Point(5, 5);
                                        case 6: return new Point(10, 4);
                                        default: return new Point(1, 0);
                                    }
                                else if (command == 0x25)
                                    switch (lineNum)
                                    {
                                        case 3: return new Point(1, 4);
                                        case 4: return new Point(18, 4);
                                        case 5: return new Point(9, 5);
                                        case 6: return new Point(14, 4);
                                        case 7: return new Point(5, 4);
                                        default: return new Point(1, 0);
                                    }
                                else if (command == 0x27 && lineNum == 3) return new Point(1, 4);
                                else return new Point(1, 0);
                            }
                            else if (destPort == 5 || destPort == 0x26)
                            {
                                if (command >= 0x21 && command <= 0x25)
                                    switch (lineNum)
                                    {
                                        case 3: return new Point(1, 4);
                                        case 4: return new Point(14, 4);
                                        case 5: return new Point(5, 5);
                                        case 6: return new Point(10, 4);
                                        default: return new Point(1, 0);
                                    }
                                else if (command == 0x26)
                                    switch (lineNum)
                                    {
                                        case 3: return new Point(1, 4);
                                        case 4: return new Point(18, 4);
                                        case 5: return new Point(9, 5);
                                        case 6: return new Point(14, 4);
                                        //case 7: return new Point(5, 4);
                                        default: return new Point(1, 0); ;
                                    }
                                else return new Point(1, 0);
                            }
                            else if (sourcePort == 0x13)
                            {
                                if (command == 0x21 || command == 0x23 || command == 0x27 || command == 0x28)
                                    switch (lineNum)
                                    {
                                        case 2: return new Point(1, 4);
                                        case 3: return new Point(5, 3);
                                        case 4: return new Point(8, 2);
                                        default: return new Point(1, 0);
                                    }
                                else if (command == 0x22 || command == 0x24)
                                    switch (lineNum)
                                    {
                                        case 2: return new Point(1, 4);
                                        case 3: return new Point(5, 3);
                                        default: return new Point(1, 0);
                                    }
                                else return new Point(1, 0);
                            }
                            else if (destPort == 0x13)
                            {
                                if (command >= 0x21 && command <= 0x26)
                                    switch (lineNum)
                                    {
                                        case 2: return new Point(1, 4);
                                        case 3: return new Point(5, 3);
                                        default: return new Point(1, 0);
                                    }
                                else return new Point(1, 0);
                            }
                            else return new Point(1, 0);

                        default: return new Point(1, 0);
                    }
                #endregion
                //--------------------------------------------0x05,0x07-----------------------------------------------
                #region 0x05,0x07
                case 0x05:
                case 0x07:
                    if ((type == 0x05 & command == 0x21) || (type == 0x07 & command == 0x02) || (type == 0x07 & command == 0x03))
                    {
                        //case 0x21:
                        switch (lineNum)
                        {
                            case 2: return new Point(1, 1);
                            case 3: return new Point(2, 1);
                            case 4: return new Point(3, 1);
                            case 5: return new Point(4, 1);
                            case 6: return new Point(6, 1);
                            case 7: return new Point(7, 4);
                            case 8: return new Point(11, 1);
                            case 9: return new Point(12, 1);
                            case 10: return new Point(15, 1);
                            case 11: return new Point(16, 1);
                            case 12: return new Point(28, 1);
                            case 13: return new Point(29, 3);
                            case 14: return new Point(32, 1);

                            case 15: return new Point(33, 1);
                            case 16: return new Point(34, 1);
                            case 17: return new Point(35, 1);
                            case 18: return new Point(36, 4);
                            case 19: return new Point(40, 3);
                            case 20: return new Point(43, 1);
                            case 21: return new Point(44, 1);
                            case 22: return new Point(45, 2);
                            case 23: return new Point(47, 1);
                            case 24: return new Point(48, 3);
                            case 25: return new Point(51, 2);
                            case 26: return new Point(53, 2);
                            case 27: return new Point(55, 1);
                            case 28: return new Point(56, 1);
                            case 29: return new Point(57, 2);
                            case 30: return new Point(59, 1);
                            case 31: return new Point(60, 1);
                            case 32: return new Point(61, 2);
                            case 33: return new Point(63, 2);
                            case 34: return new Point(65, 2);
                            case 35: return new Point(67, 1);
                            case 36: return new Point(68, 2);
                            case 37: return new Point(70, 1);
                            case 38: return new Point(72, 1);

                            case 39: return new Point(116, 2);
                            case 40: return new Point(118, 2);
                            case 41: return new Point(120, 1);
                            case 42: return new Point(121, 5);
                            case 43: return new Point(126, 4);
                            case 44: return new Point(130, 6);
                            default: return new Point(1, 0);
                        }
                    }
                    else return new Point(1, 0);
                #endregion
                //-----------------------------------------------0x06------------------------------------------------
                #region 调度命令
                case 0x06:                     //调度命令协议
                    switch (command)
                    {
                        case 0x20:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 6);
                                case 5: return new Point(11, 7);
                                case 6: return new Point(18, 8);
                                case 7: return new Point(26, 1);
                                case 8: return new Point(27, 6);
                                case 9: return new Point(33, 8);
                                case 10: return new Point(41, 1);
                                case 11: return new Point(47, 1);
                                case 12: return new Point(48, 1);
                                case 13:
                                case 14:
                                case 15:
                                case 16:
                                case 17:
                                case 18:
                                case 19:
                                    return new Point(49, originData.Length / 3 - dataHead - 48 - 4);
                                default: return new Point(1, 0);
                            }
                        case 0x21:
                        case 0x22:
                        case 0x23:
                            switch (lineNum - 2 - (lineNum - 2) / 5 * 5)
                            {
                                case 1: return new Point((1 + (lineNum - 2) / 5 * 14), 1);
                                case 2: return new Point((2 + (lineNum - 2) / 5 * 14), 6);
                                case 3: return new Point((8 + (lineNum - 2) / 5 * 14), 6);
                                case 4: return new Point((14 + (lineNum - 2) / 5 * 14), 1);
                                default: return new Point(1, 0);
                            }
                        case 0x2e:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x51:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                case 5: return new Point(3, 6);
                                case 6: return new Point(9, 7);
                                case 7: return new Point(16, 8);
                                case 8: return new Point(24, 1);
                                case 9: return new Point(25, 6);
                                case 10: return new Point(31, 3);
                                case 11: return new Point(34, 5);
                                case 12: return new Point(39, 4);
                                case 13: return new Point(48, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x53:
                        case 0x61:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 1);
                                case 5: return new Point(3, 6);
                                case 6: return new Point(9, 7);
                                case 7: return new Point(16, 8);
                                case 8: return new Point(24, 1);
                                case 9: return new Point(25, 6);
                                case 10: return new Point(31, 3);
                                case 11: return new Point(34, 5);
                                case 12: return new Point(39, 4);
                                case 13: return new Point(48, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x5f:
                            switch (lineNum)
                            {
                                case 3: return new Point(1, 1);
                                case 4: return new Point(2, 6);
                                case 5: return new Point(8, 6);
                                default: return new Point(1, 0);
                            }
                        default: return new Point(1, 0);
                    }
                #endregion
                //------------------------------------------------0x0b-------------------------------------------------
                #region LBJ数据
                case 0x0b:                             //LBJ数据
                    switch (command)
                    {
                        case 0x00:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                case 3: return new Point(2, 1);
                                case 4: return new Point(3, 1);
                                case 5: return new Point(4, 1);
                                case 6: return new Point(6, 1);
                                case 7: return new Point(7, 4);
                                case 8: return new Point(11, 1);
                                case 9: return new Point(12, 1);
                                case 10: return new Point(15, 1);
                                case 11: return new Point(16, 1);
                                case 12: return new Point(28, 1);
                                case 13: return new Point(29, 3);
                                case 14: return new Point(32, 1);

                                case 15: return new Point(33, 1);
                                case 16: return new Point(34, 1);
                                case 17: return new Point(35, 1);
                                case 18: return new Point(36, 4);
                                case 19: return new Point(40, 3);
                                case 20: return new Point(43, 1);
                                case 21: return new Point(44, 1);
                                case 22: return new Point(45, 2);
                                case 23: return new Point(47, 1);
                                case 24: return new Point(48, 3);
                                case 25: return new Point(51, 2);
                                case 26: return new Point(53, 2);
                                case 27: return new Point(55, 1);
                                case 28: return new Point(56, 1);
                                case 29: return new Point(57, 2);
                                case 30: return new Point(59, 1);
                                case 31: return new Point(60, 1);
                                case 32: return new Point(61, 2);
                                case 33: return new Point(63, 2);
                                case 34: return new Point(65, 2);
                                case 35: return new Point(67, 1);
                                case 36: return new Point(68, 2);
                                case 37: return new Point(70, 1);
                                case 38: return new Point(72, 1);

                                default: return new Point(1, 0);
                            }
                        case 0x01:
                            if (originData.Substring(dataHead * 3, 2) == "01" || originData.Substring(dataHead * 3, 2) == "02")
                                switch (lineNum)
                                {
                                    case 2: return new Point(1, 1);
                                    case 3: return new Point(20, 1);
                                    case 4: return new Point(2, 7);
                                    case 5: return new Point(9, 4);
                                    case 6: return new Point(13, 3);
                                    case 7: return new Point(16, 4);
                                    default: return new Point(1, 0);
                                }
                            else if (originData.Substring(dataHead * 3, 2) == "03" || originData.Substring(dataHead * 3, 2) == "04" || originData.Substring(dataHead * 3, 2) == "05" || originData.Substring(dataHead * 3, 2) == "06")
                            {
                                switch (lineNum)
                                {
                                    case 2: return new Point(1, 1);
                                    case 3: return new Point(2, 3);
                                    case 4: return new Point(5, 4);
                                    default: return new Point(1, 0);
                                }
                            }
                            else if (originData.Substring(dataHead * 3, 2) == "07")
                                switch (lineNum)
                                {
                                    case 3: return new Point(1, 1);
                                    case 4: return new Point(2, 4);
                                    default: return new Point(1, 0);
                                }
                            else if (originData.Substring(dataHead * 3, 2) == "08")
                                switch (lineNum)
                                {
                                    case 3: return new Point(1, 1);
                                    case 4: return new Point(2, 4);
                                    default: return new Point(1, 0);
                                }
                            else return new Point(1, 0);
                        case 0x02:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x04:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                case 3: return new Point(2, 1);
                                case 4: return new Point(3, 1);
                                case 5: return new Point(4, 4);
                                case 6: return new Point(8, 3);
                                case 7: return new Point(11, 2);
                                default: return new Point(1, 0);
                            }
                        case 0x05:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x08:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                case 3: return new Point(1, 1);
                                case 4: return new Point(1, 1);
                                case 5: return new Point(2, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x0a:
                            if (lineNum == 2) return new Point(1, originData.Length / 3 - dataHead - 4);
                            else return new Point(1, 0);
                        case 0x0b:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        case 0x21:
                        case 0x22:
                        case 0x23:
                        case 0x24:
                        case 0x25:
                        case 0x26:
                        case 0x27:
                        case 0x28:                         //没有经过验证!
                            if (sourcePort != 0x13)
                                if (command <= 26)
                                    switch (lineNum)
                                    {
                                        case 2: return new Point(1, 4);
                                        case 3: return new Point(5, 3);
                                        default: return new Point(1, 0);
                                    }
                                else return new Point(1, 0);
                            else
                            {
                                if (command != 0x22 && command != 0x24)
                                {
                                    switch (lineNum)
                                    {
                                        case 2: return new Point(1, 4);
                                        case 3: return new Point(5, 3);
                                        case 4: return new Point(8, 2);
                                        default: return new Point(1, 0);
                                    }
                                }
                                else
                                    switch (lineNum)
                                    {
                                        case 2: return new Point(1, 4);
                                        case 3: return new Point(5, 3);
                                        default: return new Point(1, 0);
                                    }
                            }
                        case 0x48:
                            if (originData.Substring((dataHead + 2) * 3, 2) == "41")
                                switch (lineNum)
                                {
                                    case 3: return new Point(1, 1);
                                    case 4: return new Point(3, 1);
                                    case 5: return new Point(4, 8);
                                    case 6: return new Point(12, 2);
                                    case 7: return new Point(14, 21);
                                    case 8: return new Point(35, 1);
                                    case 9: return new Point(36, 5);
                                    case 10: return new Point(41, 4);
                                    case 11: return new Point(45, 6);
                                    case 12: return new Point(51, 8);
                                    case 14: return new Point(59, 8);
                                    case 15: return new Point(67, 2);
                                    case 16: return new Point(69, 8);
                                    case 18: return new Point(77, 8);
                                    case 19: return new Point(85, 2);
                                    case 20: return new Point(87, 8);
                                    case 22: return new Point(95, 8);
                                    case 23: return new Point(103, 2);
                                    case 24: return new Point(105, 8);
                                    case 26: return new Point(113, 8);
                                    case 27: return new Point(121, 2);
                                    case 28: return new Point(123, 8);
                                    default: return new Point(1, 0);
                                }
                            else        //0x56
                            {
                                switch (lineNum)
                                {
                                    case 3: return new Point(1, 1);
                                    case 4: return new Point(3, 1);
                                    case 6: return new Point(45, 6);
                                    default: return new Point(1, 0);
                                }
                            }
                        case 0x55:
                            len1 = 1;
                            len2 = 1;
                            len3 = 1;
                            while (originData.Substring((dataHead + len1 + 17) * 3, 2).ToLower() != "3b") len1++;
                            while (originData.Substring((dataHead + len1 + len2 + 17) * 3, 2).ToLower() != "3b") len2++;
                            while (originData.Substring((dataHead + len1 + len2 + len3 + 17) * 3, 2).ToLower() != "3b") len3++;
                            switch (lineNum)
                            {

                                case 2: return new Point(1, 1);
                                case 3: return new Point(2, 1);
                                case 4: return new Point(2, 1);
                                case 5: return new Point(2, 1);
                                case 6: return new Point(2, 1);
                                case 7: return new Point(2, 1);
                                case 8: return new Point(2, 1);
                                case 9: return new Point(2, 1);
                                case 10: return new Point(3, 1);
                                case 11: return new Point(4, 1);
                                case 12: return new Point(5, 1);
                                case 13: return new Point(7, 4);
                                case 14: return new Point(11, 4);
                                case 15: return new Point(15, 4);
                                case 16: return new Point(19, len1);
                                case 17: return new Point(19 + len1, len2);
                                case 18: return new Point(19 + len1 + len2, len3);
                                default: return new Point(1, 0);
                            }
                        case 0xe5:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                case 3:
                                    if (Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) == 2 || Convert.ToInt32(originData.Substring(dataHead * 3, 2), 16) == 4) return new Point(1, 0);
                                    else /*1,3,5*/ return new Point(2, 3);
                                default: return new Point(1, 0);
                            }
                        default: return new Point(1, 0);
                    }
                #endregion
                //-----------------------------------------------0x13------------------------------------------------
                #region 新增
                case 0x13:
                    switch (command)
                    {
                        case 0xf1:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                case 3: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        default: return new Point(1, 0);
                    }
                //-----------------------------------------------0xa1------------------------------------------------
                case 0xa1:
                    switch (command)
                    {
                        case 0xfe:
                            switch (lineNum)
                            {
                                case 2: return new Point(1, 1);
                                default: return new Point(1, 0);
                            }
                        default: return new Point(1, 0);
                    }
                #endregion
                default: return new Point(1, 0);
            }
        }
    }
}

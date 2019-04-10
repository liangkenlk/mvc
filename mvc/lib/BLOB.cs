using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.lib
{
    public class BLOB
    {
        public static string GeoByteToText(byte[] bs)
        {
            string r = "";
            string geoType = GetGeoType(bs);
            if (geoType == "point")
            {
                string x = BitConverter.ToDouble(SubBytes(bs, 43, 50), 0).ToString();
                string y =  BitConverter.ToDouble(SubBytes(bs, 51, 58), 0).ToString();
                r = x + "," + y;
            }
            if (geoType == "linestring")
            {
                uint pointCount = System.BitConverter.ToUInt32(SubBytes(bs, 43, 46), 0);
                for (int i = 0; i<pointCount;i++)
                {
                    
                    string x = BitConverter.ToDouble(SubBytes(bs, 46+i*16+1, 46+i*16+8), 0).ToString();
                    string y = BitConverter.ToDouble(SubBytes(bs, 46 + i*16+9, 46 + i * 16+ 16), 0).ToString();
                    r = r+x + "," + y + ";";
                }
                
            }
            if (geoType == "polygon")
            {
                uint pointCount = System.BitConverter.ToUInt32(SubBytes(bs, 47, 50), 0);
                for (int i = 0; i < pointCount; i++)
                {
                    string x = BitConverter.ToDouble(SubBytes(bs, 50 + i * 16 + 1, 50 + i * 16 + 8), 0).ToString();
                    string y = BitConverter.ToDouble(SubBytes(bs, 50 + i * 16 + 9, 50 + i * 16 + 16), 0).ToString();
                    r = r + x + "," + y + ";";
                }
            }
            return r.TrimEnd(';');
        }
        public static string  GetGeoType(byte[] bs)
        {
            uint typeCode = System.BitConverter.ToUInt32(SubBytes(bs, 39, 42),0);
            if (typeCode == 1)
                return "point";
            if (typeCode == 2)
                return "linestring";
            if (typeCode == 3)
                return "polygon";
            return "other";
        }
        private static byte[] SubBytes(byte[] bs,int star, int end)
        {
            byte[] r = new byte[end - star+1];
            for (int i = 0; star+i <= end; i++)
            {
                
                r[i]=bs[star+i];
            }
            return r;
        }
    }
}
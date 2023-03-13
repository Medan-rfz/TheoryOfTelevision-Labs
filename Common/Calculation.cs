using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Drawing;

namespace TheoryOfTelevision
{
    public static class Calculation
    {
        public static List<Complex> DFT(List<double> listIn)
        {
            int len = listIn.Count;

            List<Complex> listSpectr = new List<Complex>();
            Complex sum = new Complex();

            for (int k = 0; k <= (len - 1); k++)
            {
                sum = 0;

                for (int n = 0; n <= (len - 1); n++)
                {
                    Complex exponenta = Complex.Exp(new Complex(0, (-2 * Math.PI * n * k) / len));

                    sum += listIn[n] * exponenta;
                }
                listSpectr.Add(sum / len);
            }

            return listSpectr;
        }

        public static List<Complex> IDFT(List<Complex> listIn)
        {
            int len = listIn.Count;

            List<Complex> listValue = new List<Complex>();
            Complex sum = new Complex();

            for (int n = 0; n <= (len - 1); n++)
            {
                sum = 0;

                for (int k = 0; k <= (len - 1); k++)
                {
                    Complex exponenta = Complex.Exp(new Complex(0, (2 * Math.PI * n * k) / len));

                    sum += listIn[k] * exponenta;
                }
                listValue[n] = sum / len;
            }

            return listValue;
        }

        public static List<double> ShiftCenter(List<double> listIn)
        {
            List<double> temp = new List<double>();

            temp.AddRange(listIn.Skip(listIn.Count / 2));
            temp.AddRange(listIn.Take(listIn.Count / 2));

            return temp;
        }

        public static List<double> GetBrightnessOfLine(Bitmap img, int nbrLine)
        {
            if (img != null)
            {
                int len = img.Width;
                System.Drawing.Color[] ArrColor = new System.Drawing.Color[len];
                List<double> outValue = new List<double>();

                for (int i = 0; i < len; i++)
                {
                    ArrColor[i] = img.GetPixel(i, nbrLine);
                    outValue.Add(ArrColor[i].GetBrightness());
                }

                return outValue;
            }
            else return null;
        }

        public static List<double> GetREDofLine(Bitmap img, int nbrLine)
        {
            if (img != null)
            {
                int len = img.Width;
                System.Drawing.Color[] ArrColor = new System.Drawing.Color[len];
                List<double> outValue = new List<double>();

                for (int i = 0; i < len; i++)
                {
                    ArrColor[i] = img.GetPixel(i, nbrLine);
                    outValue.Add((double)ArrColor[i].R/(double)255);
                }

                return outValue;
            }
            else return null;
        }

        public static List<double> GetBLUEofLine(Bitmap img, int nbrLine)
        {
            if (img != null)
            {
                int len = img.Width;
                System.Drawing.Color[] ArrColor = new System.Drawing.Color[len];
                List<double> outValue = new List<double>();

                for (int i = 0; i < len; i++)
                {
                    ArrColor[i] = img.GetPixel(i, nbrLine);
                    outValue.Add((double)ArrColor[i].B / (double)255);
                }

                return outValue;
            }
            else return null;
        }

        public static List<double> GetGREENofLine(Bitmap img, int nbrLine)
        {
            if (img != null)
            {
                int len = img.Width;
                System.Drawing.Color[] ArrColor = new System.Drawing.Color[len];
                List<double> outValue = new List<double>();

                for (int i = 0; i < len; i++)
                {
                    ArrColor[i] = img.GetPixel(i, nbrLine);
                    outValue.Add((double)ArrColor[i].G / (double)255);
                }

                return outValue;
            }
            else return null;
        }
    }
}

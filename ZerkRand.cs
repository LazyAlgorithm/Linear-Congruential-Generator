using System;
using System.Collections;
using System.Collections.Generic;

namespace Random_Generator
{
    public class ZerkRandom
    {
        /*
         The MIT License (MIT)

         Copyright (c) 2015 LazyAlgorithm

         Permission is hereby granted, free of charge, to any person obtaining a copy
         of this software and associated documentation files (the "Software"), to deal
         in the Software without restriction, including without limitation the rights
         to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
         copies of the Software, and to permit persons to whom the Software is
         furnished to do so, subject to the following conditions:

         The above copyright notice and this permission notice shall be included in all
         copies or substantial portions of the Software.

         THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
         IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
         FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
         AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
         LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
         OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
         SOFTWARE.
         */
        
        private int seed = 0;
        private double lastVal = 0;
        // A, M, and C based on Java's java.util.Random
        private double a = 25214903917;
        private double m = Math.Pow(2, 48);
        private double c = 11;

        public ZerkRandom(int seed)
        {
            this.seed = seed;
            lastVal = seed;
        }

        private List<double> FindFactors(double num)
        {
            List<double> result = new List<double>();

            // Take out the 2s.
            while (num % 2 == 0)
            {
                result.Add(2);
                num /= 2;
            }

            // Take out other primes.
            long factor = 3;
            while (factor * factor <= num)
            {
                if (num % factor == 0)
                {
                    // This is a factor.
                    result.Add(factor);
                    num /= factor;
                }
                else
                {
                    // Go to the next odd number.
                    factor += 2;
                }
            }

            // If num is not 1, then whatever is left is prime.
            if (num > 1) result.Add(num);

            return result;
        }

        public void SetAMC(double a, double m, double c)
        {
            double tmpC = c;
            double tmpM = m;
            // Check if C and M are co-prime as per rule 1;
            while (tmpC != 0 && tmpM != 0)
            {
                if (tmpC > tmpM)
                    tmpC %= tmpM;
                else
                    tmpM %= tmpC;
            }
            if (Math.Max(tmpC, tmpM) != 1)
                throw new Exception("C and M must be relatively prime or co-prime.");

            //Get value of a - 1 for the next two rules
            double aTmp = a - 1;

            //Make sure aTmp is divisable by the prime factors of m
            List<double> primes = FindFactors(m);
            bool isPrime = true;

            foreach (double d in primes)
            {
                if ((aTmp % d) != 0)
                {
                    isPrime = false;
                    return;
                }
            }

            if (!isPrime)
                throw new Exception("A - 1 must be divisible by all the prime factors of M.");

            //If m is a multiple of 4, aTmp must be also.
            bool isMultiple = m % 4 == 0;
            if (isMultiple)
            {
                bool aIsMultiple = aTmp % 4 == 0;
                if (!aIsMultiple)
                    throw new Exception("If M is a multiple of 4, A - 1 must be also. ");
            }

            this.a = a;
            this.m = m;
            this.c = c;
        }

        /// <summary>
        /// Gets gets an array of the bits in the value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>a byte array of bits.</returns>
        private byte[] GetBits(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            BitArray bitArray = new BitArray(bytes);
            List<byte> bits = new List<byte>();
            foreach (bool b in bitArray)
                bits.Add(Convert.ToByte(b));
            return bits.ToArray();
        }

        /// <summary>
        /// Takes a set number of bits, and turns them into a int32.
        /// </summary>
        /// <param name="bits">The bits to start from.</param>
        /// <param name="startingBit">The bit to start from. Should be more than 32 bits away from the end.</param>
        /// <param name="endingBit">The bit to end on. Should be no more than 32 bits from the start.</param>
        /// <returns></returns>
        private int GetValue(byte[] bits, int startingBit, int endingBit)
        {
            string bitString = "";
            for (int i = startingBit; i < endingBit; i++)
                bitString += bits[i];
            return Convert.ToInt32(bitString, 2);
        }

        /// <summary>
        /// Generates a random number using a linear congruential generator.
        /// </summary>
        /// <returns>A double.</returns>
        public double Generate()
        {
            // Standard Linear Congruential Generator equation.
            double value = (a * lastVal + c) % m;
            // This next part just helps the program be more random.
            int rtnInt = GetValue(GetBits(value), 10, 42);
            lastVal = rtnInt;
            return rtnInt;
        }
        /// <summary>
        /// Generates a random number using a linear congruential generator.
        /// </summary>
        /// <returns>A double.</returns>
        public double Generate(bool PosativeOnly)
        {
            // Standard Linear Congruential Generator equation.
            double value = (a * lastVal + c) % m;
            // This next part just helps the program be more random.
            int rtnInt = GetValue(GetBits(value), 10, 42);
            lastVal = rtnInt;
            if (PosativeOnly)
                rtnInt = Math.Abs(rtnInt);
            return rtnInt;
        }
        /// <summary>
        /// Generates a random number using a linear congruential generator.
        /// </summary>
        /// <returns>A double between 0 and 1.</returns>
        public double GenerateDecimal()
        {

            double value = Generate(true);
            lastVal = value;
            value = value % 100;
            return value / 100;
        }
        /// <summary>
        /// Generates a random number using a linear congruential generator.
        /// </summary>
        /// <param name="min">The minimum number the generator will generate.</param>
        /// <param name="max">The highest number the generator will generate.</param>
        /// <returns>A double between the values of min and max.</returns>
        public double Generate(double min, double max)
        {
            double diffRate = max - min;
            double value = 0;
            if (min >= 0)
                value = Generate(true);
            else
                value = Generate();
            lastVal = value;
            return (value % diffRate) + min;
        }

        /// <summary>
        /// Shuffles a givin array randomly using the Fisher-Yates Shuffle.
        /// </summary>
        /// <typeparam name="T">The arrays object type.</typeparam>
        /// <param name="array">The array to shuffle.</param>
        /// <returns>A shuffled version of the supplied array.</returns>
        public T[] ShuffleArray<T>(T[] array)
        {
            T[] rtnArray = new T[array.Length];
            Array.Copy(array, rtnArray, array.Length);
            int n = rtnArray.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + (int)(GenerateDecimal() * (n - i));
                T t = rtnArray[r];
                rtnArray[r] = rtnArray[i];
                rtnArray[i] = t;
            }
            return rtnArray;
        }
        /// <summary>
        /// Shuffles a givin list randomly using the Fisher-Yates Shuffle.
        /// </summary>
        /// <typeparam name="T">The arrays object type.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        /// <returns>A shuffled version of the supplied list.</returns>
        public List<T> ShuffleList<T>(List<T> list)
        {
            List<T> rtnList = new List<T>();
            T[] array = list.ToArray();
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + (int)(GenerateDecimal() * (n - i));
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
            foreach (T t in array)
                rtnList.Add(t);
            return rtnList;
        }

        /// <summary>
        /// Generates an array of values.
        /// </summary>
        /// <param name="length">The length of the return array.</param>
        /// <returns>An array of random double values.</returns>
        public double[] GenerateArray(int length)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < length; i++)
                data.Add(this.Generate());
            return data.ToArray();
        }
        /// <summary>
        /// Generates an array of values.
        /// </summary>
        /// <param name="length">The length of the return array.</param>
        /// <param name="min">The min value the system can generate.</param>
        /// <param name="max">The max value the system can generate.</param>
        /// <returns>An array of random double values.</returns>
        public double[] GenerateArray(int length, int min, int max)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < length; i++)
                data.Add(this.Generate(min, max));
            return data.ToArray();
        }
        /// <summary>
        /// Generates an array of values.
        /// </summary>
        /// <param name="length">The length of the return array.</param>
        /// <returns>An array of random double values between 0 and 1.</returns>
        public double[] GenerateDecimalArray(int length)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < length; i++)
                data.Add(this.GenerateDecimal());
            return data.ToArray();
        }

        /// <summary>
        /// Generates an list of values.
        /// </summary>
        /// <param name="length">The length of the return list.</param>
        /// <returns>An array of random double values.</returns>
        public List<double> GenerateList(int length)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < length; i++)
                data.Add(this.Generate());
            return data;
        }
        /// <summary>
        /// Generates an list of values.
        /// </summary>
        /// <param name="length">The length of the return list.</param>
        /// <param name="min">The min value the system can generate.</param>
        /// <param name="max">The max value the system can generate.</param>
        /// <returns>An list of random double values.</returns>
        public List<double> GenerateList(int length, int min, int max)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < length; i++)
                data.Add(this.Generate(min, max));
            return data;
        }
        /// <summary>
        /// Generates an list of values.
        /// </summary>
        /// <param name="length">The length of the return list.</param>
        /// <returns>An list of random double values between 0 and 1.</returns>
        public List<double> GenerateDecimalList(int length)
        {
            List<double> data = new List<double>();
            for (int i = 0; i < length; i++)
                data.Add(this.GenerateDecimal());
            return data;
        }
    }
}

﻿using System.Diagnostics;
namespace SuperRPC
{
    public class MyService
    {
        public static int StaticCounter { get; set; } = 0;

        public static int Mul(int a, int b)
        {
            return a * b;
        }
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Counter { get; set; } = 0;

        public int Increment()
        {
            Debug.WriteLine("Increment called");
            return ++Counter;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecutionEngine;
using NetTopologySuite.Algorithm;
using Newtonsoft.Json;

namespace ExecutionEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Part1Tester part1Tester = new Part1Tester();
            part1Tester.Run();
        }
    }
}

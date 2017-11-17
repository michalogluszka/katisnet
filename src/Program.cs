using Kattis.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kattis.UltimateSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            //for submission
            //string fileName = String.Empty;

            //for local
            string fileName = "sample.in";

            var problemProcessor = new EmptyProcessor();

            var solver = new KattisSolver(problemProcessor, fileName);
            solver.Solve();            
        }
    }

    class EmptyProcessor : IProblemProcessor
    {
        public void PostData()
        {
        }

        public void ProcessDataItem(Scanner scanner)
        {
            var line = scanner.Next().ToCharArray();
            var sorted = line.GroupBy(p => p);

            Console.WriteLine("...");
            Console.WriteLine(MathExtension.Factorial(100));

            decimal n = MathExtension.Factorial(line.Count());

            decimal k = 1;

            for (int i = 0; i < sorted.Count(); i++)
            {
                k *= MathExtension.Factorial(sorted.ElementAt(i).Count());
            }

            Console.WriteLine(n / k);


        }
    }
}

#region Core

namespace Kattis.Core
{
    class KattisSolver
    {
        private IProblemProcessor _processor;
        private string _fileName;

        public KattisSolver(IProblemProcessor processor, string fileName)
        {
            _processor = processor;
            _fileName = fileName;
        }

        public void Solve()
        {
            Stream stream = null;
            Scanner scanner = new Scanner();

            try
            {

                if (String.IsNullOrEmpty(_fileName))
                {
                    scanner = new Scanner();
                }
                else
                {
                    stream = File.OpenRead(_fileName);
                    scanner = new Scanner(stream);
                }

                do
                {
                    _processor.ProcessDataItem(scanner);
                } while (scanner.HasNext());

                _processor.PostData();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }
    }

    interface IProblemProcessor
    {
        void ProcessDataItem(Scanner scanner);
        void PostData();
    }

    public class NoMoreTokensException : Exception
    {
    }

    public class Tokenizer
    {
        string[] tokens = new string[0];
        private int pos;
        StreamReader reader;

        public Tokenizer(Stream inStream)
        {
            var bs = new BufferedStream(inStream);
            reader = new StreamReader(bs);
        }

        public Tokenizer() : this(Console.OpenStandardInput())
        {
            // Nothing more to do
        }

        private string PeekNext()
        {
            if (pos < 0)
                // pos < 0 indicates that there are no more tokens
                return null;
            if (pos < tokens.Length)
            {
                if (tokens[pos].Length == 0)
                {
                    ++pos;
                    return PeekNext();
                }
                return tokens[pos];
            }
            string line = reader.ReadLine();
            if (line == null)
            {
                // There is no more data to read
                pos = -1;
                return null;
            }
            // Split the line that was read on white space characters
            tokens = line.Split(null);
            pos = 0;
            return PeekNext();
        }

        public bool HasNext()
        {
            return (PeekNext() != null);
        }

        public string Next()
        {
            string next = PeekNext();
            if (next == null)
                throw new NoMoreTokensException();
            ++pos;
            return next;
        }
    }


    public class Scanner : Tokenizer
    {
        public Scanner(Stream stream) : base(stream)
        { }

        public Scanner() : base()
        { }


        public int NextInt()
        {
            return int.Parse(Next());
        }

        public long NextLong()
        {
            return long.Parse(Next());
        }

        public float NextFloat()
        {
            return float.Parse(Next());
        }

        public double NextDouble()
        {
            return double.Parse(Next());
        }
    }


    public class BufferedStdoutWriter : StreamWriter
    {
        public BufferedStdoutWriter() : base(new BufferedStream(Console.OpenStandardOutput()))
        {
        }
    }
}

static class MathExtension
{
    public static decimal Factorial(int x)
    {
        decimal result = 1;
        for(var i = 1; i<=x;i++)
        {
            result *= i;
        }
        return result;
    }
}


#endregion Core
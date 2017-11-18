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
            string fileName = "sample-testcase2.in";

            var problemProcessor = new EmptyProcessor();

            var solver = new KattisSolver(problemProcessor, fileName);
            solver.Solve();
        }
    }

    class Line
    {
        public Point p1;
        public Point p2;
    }

    class Point
    {
        public int x;
        public int y;
    }

    static class Helper
    {
        public static Point CrossPoint(Point p1, Point p2, Point l1, Point l2)
        {
            int t = (p1.x - p2.x) * (l1.y - l2.y) - (p1.y - p2.y) * (l1.x - l2.x);
            int s = (l2.x - p2.x) * (l1.y - l2.y) - (l2.y - p2.y) * (l1.x - l2.x);

            if (t == 0)
                return null;                

            int x = s * p1.x + (1 - s) * p2.x;
            int y = s * p1.y + (1 - s) * p2.y;

            Point point = new Point();
            point.x = x;
            point.y = y;

            return point;
        }
    }

    class CrossLine
    {
        public Line l1;
        public Line l2;
    }

    class Crossing
    {
        public Line l1;
        public Line l2;

        public int id = 0;

        public List<Crossing> connected = new List<Crossing>();
    }



    class EmptyProcessor : IProblemProcessor
    {
        int N = 0;

        List<Line> fences = new List<Line>();

        List<Crossing> graph = new List<Crossing>();

        //Dictionary<Line, Line> graph = new Dictionary<Line, int>();

        int result = 0;


        public int cows = 0;

        public void PostData()
        {
            if (fences.Count == 1)
            {
                Console.WriteLine("0");
                return;
            }
            else
            {
                int crossId = 0;
                for (int i = 0; i < fences.Count; i++)
                {
                    for (int j = i + 1; j < fences.Count; j++)
                    {
                        Point cross = Helper.CrossPoint(fences[i].p1, fences[i].p2, fences[j].p1, fences[j].p2);

                        if (cross != null)
                        {
                            var current = new Crossing() { l1 = fences[i], l2 = fences[j], id=crossId};
                            crossId++;

                            VisitOptimal(current);
                        }
                    }
                }
            }


            Console.WriteLine(cows);
        }

        public void VisitOptimal(Crossing current)
        {
            graph.Add(current);

            if (!currentRoute.Contains(current))
            {
                currentRoute.Add(current);
            }
            else
            {
                currentRoute = new List<Crossing>();
                cows++;
            }

            //all which are connected
            var connected = graph.Where(p => p.l1 == current.l1 || p.l2 == current.l2 || p.l2 == current.l1 || p.l1 == current.l2).ToList();

            foreach (var c in current.connected)
            {
                Pair route = crossingsVisited.FirstOrDefault(p => (p.crossing1 == current && p.crossing2 == c) || p.crossing2 == current && p.crossing1 == c);

                if (route == null)
                {
                    Pair pair = new Pair() { crossing1 = current, crossing2 = c };
                    crossingsVisited.Add(pair);
                    VisitOptimal(c);
                }
            }

        }

        List<Line> linesVisited = new List<Line>();
        List<Pair> crossingsVisited = new List<Pair>();

        List<Crossing> currentRoute = new List<Crossing>();

        public int Visit(Crossing current)
        {
            if(!currentRoute.Contains(current))
            { 
                currentRoute.Add(current);
            }
            else
            {
                currentRoute = new List<Crossing>();
                cows++;
            }

            foreach (var c in current.connected)
            {
                Pair route = crossingsVisited.FirstOrDefault(p => (p.crossing1 == current && p.crossing2 == c) || p.crossing2 == current && p.crossing1 == c);

                if (route==null)
                {
                    Pair pair = new Pair() { crossing1 = current, crossing2 = c };
                    crossingsVisited.Add(pair);



                    Visit(c);

                    
                }
            }
            return 0;
        }

        class Pair
        {
            public Crossing crossing1;
            public Crossing crossing2;
        }
        

        public void AddConnections(Crossing current)
        {
            var connected = graph.Where(p => p.l1 == current.l1 || p.l2 == current.l2 || p.l2 == current.l1 || p.l1 == current.l2).ToList();
            current.connected = connected;
            
            foreach(var c in connected)
            {
                c.connected.Add(current);
            }

            graph.Add(current);

            //var existing = graph.FirstOrDefault(p => p == current);
            //if(existing==null)
            //{
            //    graph.Add(current);
            //    foreach (var c in connected)
            //    {
            //        AddConnections(c);
            //    }
            //}
        }

        public void ProcessDataItem(Scanner scanner)
        {
            N = scanner.NextInt();

            for (int i = 0; i < N; i++)
            {
                Point p1 = new Point();
                Point p2 = new Point();

                p1.x = scanner.NextInt();
                p1.y = scanner.NextInt();
                p2.x = scanner.NextInt();
                p2.y = scanner.NextInt();

                fences.Add(new Line() { p1 = p1, p2 = p2 });
            }
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


#endregion Core
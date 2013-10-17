using System;
using System.Collections.Generic;
using System.Linq;

namespace OEIS
{
    internal class Towers
    {
        private List<long> _ways;

        static void Main(string[] args)
        {
            var t = new Towers();

            for (var i = 0; i <= 50; i++)
            {
                Console.Write(t.Calculate(i) + ", ");
            }

            Console.ReadLine();
        }

        public Towers()
        {
        }

        // first create all partitions
        // an example partition, for n=4, is
        // 1 1 1 1
        // 1 1 2
        // 1 3
        // 2 2
        // 4
        //
        // then each set of towers of the same size gets a configuration
        // for 2 2 2, for instance, there are two possibilities for each
        // tower (a single level with two blocks or two levels with one
        // block each) but the total possibilities is not 2*2*2=8, since
        // the configuration "1/1,2,2" is the same as "2,1/1,2". Instead
        // we want to choose three towers with repetition from two possibilities
        // which is 3+2-1 choose 3, aka 4C3 = 4.
        //
        // multiply all the sets of towers of the same size and sum
        // over partitions for the result
        //
        // for n=4, then,
        //
        // 1 1 2 becomes "1 with multiplicity 2" and "2 with multiplicity 1".
        // There is f(1)=1 way to build a tower of size 1, and
        // f(1)+2-1 choose 2 = 2C2 = 1 way to build 2 towers of size 1.
        //
        // f(2)=2 ways to build a tower of size 2.
        //
        // 1 1 2 has 1*2=2 ways to be built. Sum over each of the 5 partitions of n=4.
        public long Calculate(int n)
        {
            if (n == 0) return 1;

            return Partitions(n).Sum(partition => (from multiple in Multiples(partition)
                let possibilitiesForOneTower = F(multiple[0])
                let numTowers = multiple.Count
                select Choose(possibilitiesForOneTower + numTowers - 1, numTowers)).Aggregate<long, long>(1,
                    (current, ways) => current*ways));
        }

        private long F(int n)
        {
            if (_ways == null)
            {
                _ways = new List<long>() {1, 1, 2};
            }
            if (_ways.Count <= n)
            {
                for (var i = _ways.Count; i <= n; i++)
                {
                    _ways.Add(_ways[i - 1] + _ways[i - 2]);
                }
            }
            return _ways[n];
        }

        private static IEnumerable<List<int>> Multiples(List<int> partition)
        {
            var index = 0;
            while (index < partition.Count)
            {
                var startIndex = index;
                var num = partition[startIndex];
                while (index < partition.Count && partition[index] == num)
                {
                    index++;
                }
                yield return partition.GetRange(startIndex, index - startIndex);
            }
        }

        public static IEnumerable<List<int>> Partitions(int n)
        {
            var a = new List<int>();
            if (n == 0) yield break;
            if (n == 1)
            {
                a.Add(1);
                yield return a;
                yield break;
            }
            for (var i = 0; i < n; i++) a.Add(0);
            var k = 1;
            a[1] = n;
            while (k != 0)
            {
                var x = a[k - 1] + 1;
                var y = a[k] - 1;
                k -= 1;
                while (x <= y)
                {
                    a[k] = x;
                    y -= x;
                    k += 1;
                }
                a[k] = x + y;
                yield return a.GetRange(0, k + 1);
            }
        }
        public static long Choose(long n, long m)
        {
            if (n < m) return 0;
            long r = 1;
            for (long i = 0; i < m; i++)
            {
                r *= n - i;
                r /= i + 1;
            }
            return r;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace OEIS
{
    class Program
    {
        static void Main(string[] args)
        {
            int simulateN = 6;
            int iterations = 10000;
            int maxN = 12;

            JengaTower j = new JengaTower();

            Console.WriteLine("Simulating a random set of Jenga towers with " + simulateN + " blocks...");
            j.simulate(simulateN, iterations);
            Console.ReadLine();

            j.count(maxN);
            Console.ReadLine();
        }
    }
    class JengaTower
    {
        Random rand;
        int maxN;
        public JengaTower()
        {
            rand = new Random();
        }
        public void count(int N)
        {
            maxN = N;

            // populate num1Tower[n], the number of ways to build 1 tower of n blocks
            // for towers which have either 1 or 2 blocks at each level, this is just Fibonacci(n)
            count1Tower();

            // populate numSameTowers[m, n], the number of ways to build m towers each with exactly n blocks
            // this is just num1Tower[n]+m-1 choose m
            countSameTowers();

            // populate numSetTowers[m, n], the number of ways to build m towers with a total of n blocks
            // here is where the interesting work is done
            countSetTowers();

            // populate numTowers[n], the number of ways to build towers with a total of n blocks.
            // this is just a sum over "how many towers?" of numSetTowers
            countTowers();

            for (int n = 0; n <= maxN; n++)
            {
                Console.Write(numTowers[n] + ", ");
            }
            Console.WriteLine();
        }

        // how many distinct ways can we build towers with N blocks?
        int[] numTowers;
        public void countTowers()
        {
            numTowers = new int[maxN + 1];
            numTowers[0] = 1;
            for (int n = 1; n <= maxN; n++)
            {
                // sum of ways to build m towers, m=1..n
                for (int m = 1; m <= n; m++)
                {
                    numTowers[n] += numSetTowers[m, n];
                }
            }
        }

        // how many distinct ways can we build M towers with N blocks?
        int[,] numSetTowers;
        public void countSetTowers()
        {
            numSetTowers = new int[maxN + 1, maxN + 1];

            // there is exactly 1 way to build 0 towers with 0 blocks
            numSetTowers[0, 0] = 1;

            // there are 0 ways to build m towers with < m blocks

            // M towers, N blocks, no tower has more than S blocks in it
            int[, ,] numSetTowersMaxSize = new int[maxN + 1, maxN + 1, maxN + 1];
            
            // there is exactly 1 way to build 0 towers with 0 blocks
            for (int s = 0; s <= maxN; s++)
            {
                numSetTowersMaxSize[0, 0, s] = 1;
            }

            // there are 0 ways to build m towers with < m blocks
            // there are 0 ways to build m towers with n + 1 blocks when no tower has more than n/m blocks in it

            // for each number of total blocks,
            for (int n = 1; n <= maxN; n++)
            {
                // for each number of total towers,
                for (int m = 1; m <= n; m++)
                {
                    // suppose the largest number of blocks in any of the towers is s,
                    // (we have n-m extra blocks, so the largest tower can have at most n-m+1 blocks)
                    for (int s = 1; s <= (n - m + 1); s++)
                    {
                        // for each number d of towers that all have s blocks,
                        for (int d = 1; d * s <= n && d <= m; d++)
                        {
                            // there are w ways to build m towers from n blocks with d of the towers having s blocks
                            // and no tower having more than s blocks
                            int w = numSameTowers[d, s] * numSetTowersMaxSize[m - d, n - d * s, s - 1];
                            
                            // if a tower arrangement has no tower with more than s blocks, then it also
                            // has no tower with more than s+1, s+2, etc. blocks
                            for (int x = s; x <= maxN; x++)
                            {
                                numSetTowersMaxSize[m, n, x] += w;
                            }
                        }
                    }
                    numSetTowers[m, n] = numSetTowersMaxSize[m, n, maxN];
                }
            }
            Console.WriteLine("Number of ways to build M towers with N blocks");
            Console.WriteLine("------------------------------------------------");
            for (int i = 0; i <= maxN; i++)
            {
                Console.Write(i + ":\t");
                for (int j = 0; j <= maxN; j++)
                {
                    Console.Write(numSetTowers[i, j] + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        // how many distinct ways can we build M towers with N blocks in each?
        // numSameTowers[i, j] is how many ways to build i towers with j blocks in each
        // this is just choosing M times with repetition from num1Tower(N) tower choices
        // which is ((num1Tower(N), M)) = (num1Tower(N)+M-1,M)
        int[,] numSameTowers;
        public void countSameTowers()
        {
            numSameTowers = new int[maxN + 1, maxN + 1];
            for (int n = 0; n <= maxN; n++)
            {
                for (int m = 0; m <= maxN; m++)
                {
                    numSameTowers[m, n] = choose(num1Tower[n] + m - 1, m);
                }
            }
        }

        private int choose(int n, int m)
        {
            if (n < m) return 0;
            int r = 1;
            for (int i = 0; i < m; i++)
            {
                r *= n - i;
                r /= i + 1;
            }
            return r;
        }

        // how many distinct ways can we build 1 tower with N blocks in it?
        // if we can use up to k blocks in each level, then picture a tower
        // whose top level has j blocks - the number of such towers is the same
        // as the number of towers with N-j blocks in it.
        int[] num1Tower;
        public void count1Tower()
        {
            int k = 2;
            num1Tower = new int[maxN + 1];
            num1Tower[0] = 1;
            num1Tower[1] = 1;
            num1Tower[2] = 2;
            // assert initial fills are at least as many as k
            for (int i = 3; i <= maxN; i++)
            {
                //num1Tower[i] = num1Tower[i - 1] + num1Tower[i - 2] + num1Tower[i - 3];
                for (int j = 1; j <= k; j++)
                {
                    num1Tower[i] += num1Tower[i - j];
                }
            }
            Console.WriteLine("Number of ways to build 1 tower with N blocks");
            Console.WriteLine("===");
            Console.WriteLine("\t0\t1\t2\t3\t4\t5\t6\t7\t8");
            Console.WriteLine("------------------------------------------------");
            for (int i = 0; i <= maxN; i++) Console.Write("\t" + num1Tower[i]);
            Console.WriteLine("");
            Console.WriteLine("");
        }
        public void simulate(int n, int T)
        {
            HashSet<string> first = new HashSet<string>();
            HashSet<string> second = new HashSet<string>();
            for (int multiply = 1; multiply <= 10; multiply *= 10)
            {
                HashSet<string> found = new HashSet<string>();
                for (int t = 0; t < T * multiply; t++)
                {
                    List<List<int>> towers = new List<List<int>>();
                    int numTowers = 0;
                    int howManyTowers = rand.Next(1, ((n + 1) * n) / 2 + 1);
                    for (int i = 0; i < n; i++)
                    {
                        if (howManyTowers >= n - i)
                        {
                            howManyTowers -= (n - i);
                            towers.Add(new List<int>() { 1 });
                            numTowers++;
                        }
                    }

                    double[] prob = new double[numTowers];
                    double sum = 0;
                    for (int i = 0; i < numTowers; i++)
                    {
                        prob[i] = rand.NextDouble();
                        if (i == 0) prob[i] = 1 - prob[i] * prob[i];
                        sum += prob[i];
                    }
                    for (int i = 0; i < numTowers; i++)
                    {
                        prob[i] /= sum;
                    }

                    for (int i = 0; i < n - numTowers; i++)
                    {
                        double towerToChangeRandom = rand.NextDouble();
                        List<int> towerToChange = towers[0];
                        for (int j = 0; j < numTowers; j++)
                        {
                            if (towerToChangeRandom < prob[j])
                            {
                                towerToChange = towers[j];
                                break;
                            }
                            towerToChangeRandom -= prob[j];
                        }
                        if (towerToChange[towerToChange.Count - 1] == 2 || rand.Next(0, 5) <= 2)
                        {
                            towerToChange.Add(1);
                        }
                        else
                        {
                            towerToChange[towerToChange.Count - 1] = 2;
                        }
                    }
                    found.Add(canonical(towers));
                }
                int total = found.Count;
                printHashSet(found, total);
                if (multiply == 1)
                {
                    first.UnionWith(found);
                }
                else
                {
                    second.UnionWith(found);
                }
            }
            second.SymmetricExceptWith(first);
            printHashSet(second, second.Count);
        }

        private static void printHashSet(HashSet<string> found, int total)
        {
            string[] instances = new string[found.Count];
            found.CopyTo(instances);
            Array.Sort<string>(instances);
            foreach (string instance in instances)
            {
                Console.WriteLine(instance);
            }
            Console.WriteLine(total);
        }
        public string canonical(List<List<int>> towerset)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < towerset.Count; i++)
            {
                if (i != 0) sb.Append(",");
                sb.Append(string.Join<int>("/", towerset[i]));
            }
            string towersstring = sb.ToString();
            string[] towers = towersstring.Split(new char[] { ',' });
            Array.Sort<string>(towers);
            return string.Join<string>(",", towers);
        }
    }
}


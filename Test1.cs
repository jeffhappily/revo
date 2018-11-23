using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


public class Program
{
    //List of genes
    public static List<string> cities;

    public static Population population;
    //List of chromosomes
    public static List<Route> route;

    public static void Main()
    {
        //To initialize the search space together with the distance
        Input.initialize();

        cities = Input.cities.Keys.ToList<string>();
        population = Population.randomize(cities);
        route = population.chromosomes;
        
        int generation = 0;
        bool better = true;

        //Loop through the population 
        while (generation <= 100)
        {
            // if (better)
            //     display(route, generation);

            // better = false;
            route = population.evolve().chromosomes;

            // double oldMax = population.maxFitness;
            // //To check if the new route is better, using fitness score
            // if (population.maxFitness > oldMax)
            //     better = true;

            generation++;
        }
    }

    public static void display(List<Route> p, int gen)
    {
        Route best = population.findBest();
        System.Console.WriteLine("Generation {0}\n" +
            "Best fitness:  {1}\n" +
            "Best distance: {2}\n", gen, best.fitness, best.totalDistance);
    }
}

public class Population
{
    //List of Chromosomes or the Population
    public List<Route> chromosomes = new List<Route>();
    public double maxFitness;
    //public Route firstBest, secondBest;
    List<List<string>> bestCity = new List<List<string>>();

    public Population(List<Route> Temp)
    {
        chromosomes = Temp;
        maxFitness = calcMaxFit();
    }

    public static Population randomize(List<string> cities)
    {
        List<Route> temp = new List<Route>();
        //To create 100 chromosomes
        for (int i = 0; i < Data.population; i++)
            temp.Add(Route.randomize(cities));

        return new Population(temp);
    }

    public double calcMaxFit()
    {
        double max = 0;
        foreach (Route r in chromosomes)
        {
            if (r.fitness > max)
                max = r.fitness;
        }
        return max;
    }

    public Population evolve()
    {
        Population best = elite(Data.elite);
        Population newPopulation = generateNewPopulation(Data.population - Data.elite);

        return new Population(best.chromosomes.Concat(newPopulation.chromosomes).ToList());
    }

    public Population elite(int n)
    {
        List<Route> best = new List<Route>();
        Population temp = new Population(chromosomes);

        for (int i = 0; i < n; i++)
        {
            best.Add(temp.findBest());
            temp = new Population(temp.chromosomes.Except(best).ToList());
        }

        return new Population(best);
    }

    public Route findBest()
    {
        foreach (Route r in chromosomes)
            if (r.fitness == maxFitness)
                return r;

        return null;
    }

    public Population generateNewPopulation(int n)
    {
        List<Route> temp = new List<Route>();

        for (int i = 0; i < n; i++)
        {
            select();
            crossover(bestCity);
        }

        return new Population(temp);
    }

    public void select()
    {
        List<Route> temp = new List<Route>(chromosomes);
        temp.Sort((a, b) => -1 * a.fitness.CompareTo(b.fitness));
        
        bestCity.Add(temp[0].route);
        bestCity.Add(temp[1].route);
    }

    public static void crossover(List<List<string>> best)
    {
        int crossOverPoint = 3;
        
        List<string> tempA = best.First().GetRange(crossOverPoint, 4);
        List<string> tempB = best.Last().GetRange(crossOverPoint, 4);

        int aCount = 0;
        int bCount = 0;

        for (int i = 0; i < best.First().Count; i++)
        {
            if (tempA.Contains(best.First()[i]))
                continue;

            while (tempA.Contains(best.Last()[bCount]))
                bCount++;

            best.First()[i] = best.Last()[bCount];
            bCount++;
        }

        for (int i = 0; i < best.Last().Count; i++)
        {
            if (tempB.Contains(best.Last()[i]))
                continue;

            while (tempB.Contains(best.First()[aCount]))
                aCount++;

            best.Last()[i] = best.First()[aCount];
            aCount++;
        }

        mutate();
    }

    public void mutate()
    {
        Random rand = new Random();

        if (rand.NextDouble() < Data.muRate)
        {
            int posA = rand.Next(0, bestCity[0].Count);
            int posB = rand.Next(posA, best.Count);
            string temp;

            temp = bestCity[0][posA];
            bestCity[0][posA] = bestCity[0][posB];
            bestCity[0][posB] = temp;
        }
    }
}

public class Route
{
    public List<string> route = new List<string>();
    public int totalDistance = 0;
    public double fitness;

    public Route(List<string> OriRoute)
    {
        route = OriRoute;
        calcDistance();
        fitness = calcFitness();
    }

    public static Route randomize(List<string> oriRoute)
    {
        int n = oriRoute.Count;
        Random rand = new Random();

        //Loop through the list of cities and randomly swap the order of cities
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);

            //Swap between the genes
            string value = oriRoute[k];
            oriRoute[k] = oriRoute[n];
            oriRoute[n] = value;
        }

        return new Route(oriRoute);
    }

    public void calcDistance()
    {
        for (int i = 0; i < route.Count; i++)
        {
            int next = i + 1;

            if (route[i] == route.Last())
                next = 0;

            totalDistance += Input.cities[route[i]][route[next]];
        }
    }

    public double calcFitness()
    {
        return (1 / totalDistance) * 10;
    }
}

public class Input
{
    public static Dictionary<string, Dictionary<string, int>> cities = new Dictionary<string, Dictionary<string, int>>();

    public static void initialize()
    {

        StreamReader reader = new StreamReader("Input.csv");
        string[] header = reader.ReadLine().Split(',');
        while (!reader.EndOfStream)
        {
            var distance = new Dictionary<string, int>();
            string[] temp = reader.ReadLine().Split(',');

            int count = 0;
            foreach (string st in temp)
            {
                if (count == 0)
                {
                    count++;
                    continue;
                }
                distance.Add(header[count], Convert.ToInt32(st));
                count++;
            }
            cities.Add(temp[0], distance);
        }
    }
}

public class Data
{
    public const double muRate = 0.02;
    public const int cityNum = 8;
    public const int population = 100;
    public const int elite = 7;
}


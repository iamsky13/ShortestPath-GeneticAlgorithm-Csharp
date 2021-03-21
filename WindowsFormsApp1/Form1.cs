﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShortestPathGenetic
{
    public partial class Form1 : Form
    {
        Graphics graphic;
        Graphics bestPathGraphic;
        Pen pen;
        Pen greenPen;
        Genetic geneticAlgService = new Genetic();
        CancellationTokenSource cancellationTokenSource;

        bool isGraph = true;

        uint[][] Graph = new uint[][]
        {
            new uint[]{0, 2, 3, 0, 0, 0, 0},
            new uint[]{7, 0, 0, 1, 0, 0, 3},
            new uint[]{3, 0, 0, 0, 0, 0, 5},
            new uint[]{0, 0, 0, 0, 0, 2, 2},
            new uint[]{0, 0, 0, 0, 0, 0, 0},
            new uint[]{0, 0, 0, 1, 11, 0, 7},
            new uint[]{0, 0, 0, 0, 2, 7, 0}
        };

        int threshold = 50;
        public Form1()
        {
            InitializeComponent();
            graphic = panel1.CreateGraphics();
            bestPathGraphic = bestPathPanel.CreateGraphics();
            button1.Text = "Start";
        }
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            //initialize total nodes for random nodes
            //this will work only if you set isGraph flag false
            int totalNodes = 10;
            
            int startNodeIndex = 0;
            int DestinationNodeIndex = 5;
            if (isGraph)
            {
                totalNodes = Graph.Length;
            }
            
            if (button1.Text == "Start")
            {
                button1.Text = "Stop";
            }
            await Task.Factory.StartNew(async () =>
            {
                int populationSize = 100;
                
                //generate random coordinates 
                var nodes = geneticAlgService.GenerateNodes(totalNodes, startNodeIndex, DestinationNodeIndex);

                //for making application responsive while loop is running
                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                    try
                    {
                        //assigning best path as max value
                        double minPath = double.MaxValue;

                        //sample population is generated by swapping order index randomly
                        Population[] population = await CalculateSamplePopulation(populationSize, nodes, cancellationTokenSource.Token);
                        
                        //find the best parent among population by comparing path wih minPath
                        Population bestParent1 = await CalculateBestPopulation(population, minPath, isGraph, cancellationTokenSource.Token);
                        NormalizeFitness(population);
                        if (bestParent1.PathLength < minPath && minPath!=0)
                        {
                            minPath = bestParent1.PathLength;
                        }

                        //we use random Mutation to calculate Next Generation Population
                        Population[] nextGenerationPopulation = await geneticAlgService.CalculateNextGenerationPopulationByMutation(population);
                        
                        //find the better population than previous population
                        Population bestParent2 = await CalculateBestPopulation(nextGenerationPopulation, bestParent1.PathLength, isGraph, cancellationTokenSource.Token);
                        
                        //if best population doesn't exist in nextGeneration we use population from 1st generation
                        if (bestParent2.Nodes == null)
                        {
                            bestParent2.Nodes = geneticAlgService.DeepCopyNodes(bestParent1.Nodes);
                            bestParent2.PathLength = bestParent1.PathLength;
                        }
                        if (bestParent2.PathLength < minPath && bestParent2.PathLength != 0)
                        {
                            minPath = bestParent2.PathLength;
                        }

                        NormalizeFitness(nextGenerationPopulation);
                        //we generate three different sets of offspring 
                        for(int i = 0; i < threshold; i++)
                        {
                            //using crossover only
                            Population[] CrossOverPopulation =await geneticAlgService.CalculateNextGenerationPopulationByCrossOver(nextGenerationPopulation);
                            Population bestCrossOverChild = await CalculateBestPopulation(CrossOverPopulation, minPath, isGraph, cancellationTokenSource.Token);
                            NormalizeFitness(CrossOverPopulation);

                            //using mutation only
                            Population[] MutatedPopulation =await geneticAlgService.CalculateNextGenerationPopulationByMutation(nextGenerationPopulation);
                            Population bestMutatedChild = await CalculateBestPopulation(MutatedPopulation, minPath, isGraph, cancellationTokenSource.Token);
                            NormalizeFitness(MutatedPopulation);

                            //using mutation in one population and then applying crossover
                            Population[] MutatedCrossOverPopulation =await geneticAlgService.CalculateNextGenerationPopulationByBothCrossOverAndMutation(nextGenerationPopulation);
                            Population bestMutatedCrossOverChild = await CalculateBestPopulation(MutatedPopulation, minPath, isGraph, cancellationTokenSource.Token);
                            NormalizeFitness(MutatedCrossOverPopulation);

                            bool flag = true;

                            //compare the child from each population and update best path
                            if (bestCrossOverChild.PathLength < minPath && bestCrossOverChild.PathLength != 0)
                            {
                                nextGenerationPopulation = CrossOverPopulation;
                                minPath = bestCrossOverChild.PathLength;
                                flag = false;
                            }
                            if (bestMutatedChild.PathLength < minPath && bestMutatedChild.PathLength != 0)
                            {
                                nextGenerationPopulation = MutatedPopulation;
                                minPath = bestMutatedChild.PathLength;
                                flag = false;
                            }
                            if (bestMutatedCrossOverChild.PathLength < minPath && bestMutatedCrossOverChild.PathLength != 0)
                            {
                                nextGenerationPopulation = MutatedCrossOverPopulation;
                                minPath = bestMutatedCrossOverChild.PathLength;
                                flag = false;
                            }
                            //if none of the child can perform better than current best path we apply 
                            //mutation to one of the child or parent randomly
                            //this will help to overcome local minima problem to some extent
                            if (flag)
                            {
                                var r = new Random();
                                int rand = r.Next(1, 4);
                                Population[] p;
                                if (rand == 1)
                                {
                                    p = CrossOverPopulation;
                                }
                                else if (rand == 2)
                                {
                                    p = MutatedPopulation;
                                }
                                else if(rand ==3)
                                {
                                    p = MutatedCrossOverPopulation;
                                }
                                else
                                {
                                    p = population;
                                }
                                nextGenerationPopulation = await geneticAlgService.CalculateNextGenerationPopulationByMutation(p); ;
                            }

                        }

                        button1.Text="Stop";

                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        cancellationTokenSource = null;
                    }
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = null;
                }

            });

        }



        private async Task<Population> CalculateBestPopulation(Population[] samplePopulations, double minDistance, bool isGraph, CancellationToken token)
        {
            Population bestPopulation = new Population();
            double TotalPathDistance = 0;
            for (int i = 0; i < samplePopulations.Length; i++)
            {
                if (token.IsCancellationRequested)
                {
                    button1.Text = "Start";
                    break;
                }
                //for graph we use distance from adjacency matrix
                if (!isGraph)
                {
                    TotalPathDistance = geneticAlgService.CalculateTotalDistance(samplePopulations[i].Nodes);
                }
                //for randomly generated node we calculate displacement from coordinates
                else
                {
                    TotalPathDistance = geneticAlgService.CalculateGraphTotalDistance(samplePopulations[i].Nodes, Graph);
                }
                samplePopulations[i].PathLength = TotalPathDistance;
                graphic.Clear(Color.White);

                //visualize current path
                await GenerateGraphic(samplePopulations[i]);
                Console.WriteLine(i);

                if (TotalPathDistance < minDistance)
                {
                    minDistance = TotalPathDistance;
                    bestPopulation = samplePopulations[i];

                    //update best path till now
                    bestPathGraphic.Clear(Color.White);
                    await GenerateBestpathGraphic(bestPopulation,isGraph);
                }
                samplePopulations[i].Fitness = 1 / (TotalPathDistance + 1);
            }
            return bestPopulation;
        }

        private void NormalizeFitness(Population[] population)
        {
            double sum = 0;
            for (int i = 0; i < population.Length; i++)
            {
                sum += population[i].Fitness;
            }
            for (int j = 0; j < population.Length; j++)
            {
                population[j].Fitness = population[j].Fitness / sum;
            }
        }

        private async Task<Population[]> CalculateSamplePopulation(int populationSize, Nodes[] nodes, CancellationToken cancellationToken)
        {
            Population[] samplePopulation = new Population[populationSize];
            for (var i = 0; i < populationSize; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    button1.Text = "Start";
                    break;
                }
                Nodes[] copyofNodes = geneticAlgService.DeepCopyNodes(nodes);
                samplePopulation[i] = new Population();
                samplePopulation[i].Nodes = geneticAlgService.ShuffleNodes(copyofNodes, 10 * (i + 1));

            }
            return samplePopulation;
        }



        private async Task GenerateGraphic(Population population)
        {
            await Task.Factory.StartNew(() =>
            {

                pen = new Pen(Color.Red, 5);
                SolidBrush sb = new SolidBrush(Color.Red);
                Point p1 = new Point(0, 0);

                graphic.DrawString(population.PathLength.ToString("0.####"),new Font("Arial",12),Brushes.Black,327, 20);
                for (int i = 0; i < population.Nodes.Length; i++)
                {

                    graphic.DrawEllipse(pen, population.Nodes[i].Coordinates[0], population.Nodes[i].Coordinates[1], 5, 5);
                    graphic.DrawString("V"+population.Nodes[i].NodeId.ToString(), new Font("Arial", 10), Brushes.Black, population.Nodes[i].Coordinates[0] + 5, population.Nodes[i].Coordinates[1] + 5);
                }
                for (int i = 0; i < population.Nodes.Length - 1; i++)
                {
                    Nodes NodeU = Array.Find(population.Nodes, a => a.OrderIndex == i);
                    Nodes NodeV = Array.Find(population.Nodes, a => a.OrderIndex == (i + 1));

                    graphic.DrawLine(pen,
                        new Point(NodeU.Coordinates[0], NodeU.Coordinates[1]),
                         new Point(NodeV.Coordinates[0], NodeV.Coordinates[1]));
                }
            });

        }
        private async Task GenerateBestpathGraphic(Population bestPopulation, bool isGraph)
        {
            await Task.Factory.StartNew(() =>
            {

                pen = new Pen(Color.Red, 5);
                greenPen = new Pen(Color.Green, 5);
                SolidBrush sb = new SolidBrush(Color.Red);
                Point p1 = new Point(0, 0);
                bestPathGraphic.DrawString(bestPopulation.PathLength.ToString("0.####"), new Font("Arial", 12), Brushes.Green, 327, 20);

                for (int i = 0; i < bestPopulation.Nodes.Length; i++)
                {

                    bestPathGraphic.DrawEllipse(pen, bestPopulation.Nodes[i].Coordinates[0], bestPopulation.Nodes[i].Coordinates[1], 5, 5);
                    bestPathGraphic.DrawString("V" + bestPopulation.Nodes[i].NodeId, new Font("Arial", 10), Brushes.Black, bestPopulation.Nodes[i].Coordinates[0] + 5, bestPopulation.Nodes[i].Coordinates[1] + 5);
                }
                for (int i = 0; i < bestPopulation.Nodes.Length - 1; i++)
                {

                    Nodes NodeU = Array.Find(bestPopulation.Nodes, a => a.OrderIndex == i);
                    Nodes NodeV = Array.Find(bestPopulation.Nodes, a => a.OrderIndex == (i + 1));
                    if (isGraph && Graph[NodeU.NodeId][NodeV.NodeId] != 0)
                    {
                        bestPathGraphic.DrawLine(greenPen,
                        new Point(NodeU.Coordinates[0], NodeU.Coordinates[1]),
                         new Point(NodeV.Coordinates[0], NodeV.Coordinates[1]));

                        bestPathGraphic.DrawString(Graph[NodeU.NodeId][NodeV.NodeId].ToString(), new Font("Arial", 10), Brushes.Black, (NodeU.Coordinates[0] + NodeV.Coordinates[0]) / 2, (NodeU.Coordinates[1] + NodeV.Coordinates[1]) / 2);
                    }
                    if (!isGraph)
                    {

                        bestPathGraphic.DrawLine(pen,
                            new Point(NodeU.Coordinates[0], NodeU.Coordinates[1]),
                             new Point(NodeV.Coordinates[0], NodeV.Coordinates[1]));
                    }

                }

            });

        }

    }
}

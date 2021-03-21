using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShortestPathGenetic
{
    public class Nodes
    {
        public int[] Coordinates { get; set; }
        public int OrderIndex { get; set; }
        public int NodeId { get; set; }
        public bool IsStartOrEndNode { get; set; }
    }

    public class Population
    {
        public Nodes[] Nodes { get; set; }
        public Double Fitness { get; set; }
        public double PathLength { get; set; }
    }
    public class Genetic
    {
        Random random = new Random();
        public Nodes[] GenerateNodes(int TotalNodes, int startNodeIndex, int destinationNodeIndex)
        {
            Nodes[] nodes = new Nodes[TotalNodes];
            
            for(int i = 0; i < TotalNodes; i++)
            {
                nodes[i] = new Nodes();
                nodes[i].Coordinates = new int[2] {random.Next(0,430), random.Next(0, 530)};
                nodes[i].OrderIndex = i;
                nodes[i].NodeId = i;
            }
            nodes[startNodeIndex].IsStartOrEndNode = true;
            nodes[destinationNodeIndex].IsStartOrEndNode = true;

            //order index of source should be 0 
            //order index of destination should be the highest
            if (startNodeIndex != 0)
            {
                SwapOrderIndex(nodes, startNodeIndex, 0);
            }

            if(destinationNodeIndex+1 != TotalNodes)
            {
                SwapOrderIndex(nodes, destinationNodeIndex, TotalNodes - 1);
            }
            return nodes;
        }

        public Nodes[] ShuffleNodes(Nodes[] nodes, int shuffleFrequency)
        {
            for (int i = 0; i < shuffleFrequency; i++)
            {
                var firstIndex = random.Next(0, nodes.Length);
                var secondIndex = random.Next(0, nodes.Length);

                //we don't want to shuffle source or destination node
                if(!nodes[firstIndex].IsStartOrEndNode && !nodes[secondIndex].IsStartOrEndNode)
                {
                    nodes = SwapOrderIndex(nodes, firstIndex, secondIndex);
                }
                
            }
            return nodes;
        }

        private Nodes[] SwapOrderIndex(Nodes[] nodes, int firstIndex, int secondIndex)
        {
            var temp = nodes[firstIndex].OrderIndex;
            nodes[firstIndex].OrderIndex = nodes[secondIndex].OrderIndex;
            nodes[secondIndex].OrderIndex = temp;
            return nodes;
        }

        public double CalculateTotalDistance(Nodes[] nodes)
        {
            double PathSum = 0;
            for (int i=0; i < nodes.Length - 1; i++)
            {
                Nodes nodeA = Array.Find(nodes, a => a.OrderIndex == i);
                Nodes nodeB = Array.Find(nodes, a => a.OrderIndex == (i + 1));
                double distance = CalculateDisplacement(nodeA.Coordinates, nodeB.Coordinates);
                PathSum += distance;
            }
            return PathSum;
        }

        public double CalculateGraphTotalDistance(Nodes[] nodes, uint[][] graph)
        {
            double PathSum = 0;

            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Nodes nodeA = Array.Find(nodes, a => a.OrderIndex == i);
                Nodes nodeB = Array.Find(nodes, a => a.OrderIndex == (i + 1));
                double distance = Convert.ToDouble(graph[nodeA.NodeId][nodeB.NodeId]);
                if (distance == 0)
                {
                    distance = 1000;  //penalty if no distance is found, value should be large compared to path in marix
                }
                PathSum += distance;
            }
            return PathSum;
        }
        private double CalculateDisplacement(int[] coordinates1, int[] coordinates2)
        {
            double distX = coordinates1[0] - coordinates2[0];
            double distY = coordinates1[1] - coordinates2[1];
            return Math.Sqrt(distX * distX + distY * distY);
        }
        public async Task<Population[]> CalculateNextGenerationPopulationByMutation(Population[] populations)
        {
            Population[] newPopulations = new Population[populations.Length];
            for (var i = 0; i < populations.Length; i++)
            {
                //select random population based on their fitness
                var tempPopulation = PickOnePopulation(populations);
                newPopulations[i] = new Population();

                MutatePopulation(tempPopulation,10*(i+1));
                newPopulations[i].Nodes = DeepCopyNodes(tempPopulation.Nodes);
            }
            return newPopulations;
        }
        public async Task<Population[]> CalculateNextGenerationPopulationByCrossOver(Population[] populations)
        {
            Population[] newPopulations = new Population[populations.Length];
            for (var i = 0; i < populations.Length; i++)
            {
                //PickOnePopulation will randomly pick one population with higher fitness rate
                var tempPopulation1 = PickOnePopulation(populations);
                var tempPopulation2 = PickOnePopulation(populations);
                newPopulations[i] = new Population();
                var tempPopulation = CrossOver(tempPopulation1, tempPopulation2);
                newPopulations[i].Nodes = DeepCopyNodes(tempPopulation.Nodes);
            }
            return newPopulations;
        }
        public async Task<Population[]> CalculateNextGenerationPopulationByBothCrossOverAndMutation(Population[] populations)
        {
            Population[] newPopulations = new Population[populations.Length];
            for (var i = 0; i < populations.Length; i++)
            {
                var tempPopulation1 = PickOnePopulation(populations);
                var tempPopulation2 = PickOnePopulation(populations);
                MutatePopulation(tempPopulation2, 10 * (i + 1));
                var tempPopulation = CrossOver(tempPopulation1, tempPopulation2);
                newPopulations[i] = new Population();
                newPopulations[i].Nodes = DeepCopyNodes(tempPopulation.Nodes);
            }
            return newPopulations;
        }

        public void MutatePopulation(Population population, int mutationRate)
        {
            ShuffleNodes(population.Nodes, mutationRate); 
        }
        public Population PickOnePopulation(Population[] populations)
        {   int index = 0;
            double r = random.NextDouble();
            while (r > 0 && index<populations.Length-1)
            {
                r = r - populations[index].Fitness;
                index++;
            }
            index--;
            var newPopulation = new Population();
            newPopulation.Nodes= DeepCopyNodes(populations[index].Nodes);
            return (newPopulation);
        }
        public Nodes[] DeepCopyNodes(Nodes[] nodes)
        {
            Nodes[] newNodes = new Nodes[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                newNodes[i] = new Nodes
                {
                    Coordinates = nodes[i].Coordinates,
                    OrderIndex = nodes[i].OrderIndex,
                    NodeId=nodes[i].NodeId,
                    IsStartOrEndNode=nodes[i].IsStartOrEndNode
                };
            }
            return newNodes;
        }

        public Population CrossOver(Population pop1, Population pop2)
        {
            Population NewCrossOverPopulation= new Population();
            int commonNodeIndex = pop1.Nodes.Length - 1;

            //find the index for common node in both population
            for (int i = 1; i < pop1.Nodes.Length - 1; i++)
            {
                var oldNode = Array.Find(pop2.Nodes, n => n.OrderIndex == i);
                if (Array.Exists(pop1.Nodes, n => n.NodeId == oldNode.NodeId))
                {
                    commonNodeIndex = oldNode.OrderIndex;
                    break;

                }
            }
            NewCrossOverPopulation = new Population();
            NewCrossOverPopulation.Nodes= GenerateTempNodeByCrossing(pop1, pop2, commonNodeIndex);
            return NewCrossOverPopulation;

        }

        private Nodes[] GenerateTempNodeByCrossing(Population firstPop, Population SecondPop,int commonNodeIndex)
        {
            Nodes[] tempNode = { };

            //pick nodes from population2 from 0 to common node
            for (int i = 0; i <= commonNodeIndex; i++)
            {
                var oldNode= Array.Find(SecondPop.Nodes, n => n.OrderIndex == i);
                var n1 = new Nodes
                {
                    OrderIndex = oldNode.OrderIndex,
                    Coordinates = oldNode.Coordinates,
                    IsStartOrEndNode = oldNode.IsStartOrEndNode,
                    NodeId = oldNode.NodeId

                };
                tempNode = tempNode.Concat(new Nodes[] { n1 }).ToArray(); 
            }

            //we want this population to have both source and destination node
            tempNode = AddSourceAndDestinationIfNotExist(tempNode, SecondPop);

            //add unique node from population 1 till the no. of node is equal to lenth of population1
            //here we reduce the number of nodes by 1 unit
            for (int i = SecondPop.Nodes.Length-1; tempNode.Length <= firstPop.Nodes.Length - 2; i--)
            {
                var remNode = Array.Find(firstPop.Nodes, n => n.OrderIndex == i);
                if (!Array.Exists(tempNode, n => n.NodeId == remNode.NodeId))
                {
                    var n = new Nodes
                    {
                        OrderIndex = commonNodeIndex+1,
                        Coordinates = remNode.Coordinates,
                        IsStartOrEndNode = remNode.IsStartOrEndNode,
                        NodeId = remNode.NodeId

                    };
                    ++commonNodeIndex;
                    tempNode = tempNode.Concat(new Nodes[] { n }).ToArray();
                }
            }

            //Repair nodes for the lost node
            tempNode = AdjustLostNodeOrderIndex(tempNode);
            return tempNode;
        } 

        private Nodes[] AdjustLostNodeOrderIndex(Nodes[] tempNode)
        {
            int lostOrderIdIndex = -1;
            var adjustedNodes = DeepCopyNodes(tempNode);
            for(int i=0;i<tempNode.Length; i++)
            {
                if ((Array.FindIndex(tempNode, x => x.OrderIndex == i))==-1)
                {
                    lostOrderIdIndex = i;
                    break;
                }
            }
            if(lostOrderIdIndex!= -1)
            {
                for(int i = lostOrderIdIndex + 1; i < tempNode.Length+1; i++)
                {
                    int toBeRepairedNodeIndex = Array.FindIndex(tempNode, x => x.OrderIndex == i);
                    adjustedNodes[toBeRepairedNodeIndex].OrderIndex = adjustedNodes[toBeRepairedNodeIndex].OrderIndex - 1;
                }
            }
            return adjustedNodes;
        }

        private Nodes[] AddSourceAndDestinationIfNotExist(Nodes[] tempNode, Population pop)
        {
            Nodes source = Array.Find(pop.Nodes, x => x.OrderIndex == 0);
            Nodes destination = Array.Find(pop.Nodes, x => x.OrderIndex == (pop.Nodes.Length)-1);
            if (!Array.Exists(tempNode, n => n.NodeId == source.NodeId))
            {
                var n = new Nodes
                {
                    OrderIndex = source.OrderIndex,
                    Coordinates = source.Coordinates,
                    IsStartOrEndNode = source.IsStartOrEndNode,
                    NodeId = source.NodeId
                };
                tempNode = tempNode.Concat(new Nodes[] { n }).ToArray();
            }
            if (!Array.Exists(tempNode, n => n.NodeId == destination.NodeId))
            {
                var n = new Nodes
                {
                    OrderIndex = destination.OrderIndex,
                    Coordinates = destination.Coordinates,
                    IsStartOrEndNode = destination.IsStartOrEndNode,
                    NodeId = destination.NodeId
                };
                tempNode = tempNode.Concat(new Nodes[] { n }).ToArray();
            }
            return tempNode;
        }
    }
}

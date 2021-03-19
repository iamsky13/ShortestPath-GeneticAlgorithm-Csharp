﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShortestPathGenetic
{
    public partial class Form1 : Form
    {
        Graphics graphic;
        Pen pen;
        Pen pen1;
        Genetic geneticAlgService = new Genetic();
        public Form1()
        {
            InitializeComponent();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int populationSize = 10;
            var nodes = geneticAlgService.GenerateNodes(5);
            Nodes[][] population = new Nodes[populationSize][];
            for (var i = 0; i < populationSize; i++)
            {
                Nodes[] copyofNodes = DeepCopyNodes(nodes);
                population[i] = new Nodes[nodes.Length];
                population[i] = geneticAlgService.ShuffleOrderIndex(copyofNodes, 10*(i+1));
            }
            
            GenerateGraphic(nodes);

        }

        private Nodes[] DeepCopyNodes(Nodes[] nodes)
        {
            Nodes[] newNodes = new Nodes[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                newNodes[i] = new Nodes
                {
                    Coordinates = nodes[i].Coordinates,
                    OrderIndex = nodes[i].OrderIndex
                };
            }
            return newNodes;

        }

        private async Task GenerateGraphic(Nodes[] nodes)
        {
            await Task.Factory.StartNew(() =>
            {
                graphic = panel1.CreateGraphics();
                pen = new Pen(Color.Red, 5);
                pen1 = new Pen(Color.White, 5);
                SolidBrush sb = new SolidBrush(Color.Red);
                Point p1 = new Point(0, 0);
                for (int i = 0; i < nodes.Length; i++)
                {
                    graphic.DrawEllipse(pen, nodes[i].Coordinates[0], nodes[i].Coordinates[1], 5, 5);
                }

                //Point p2 = new Point(150, 350);
                //graphic.DrawEllipse(pen, 0, 0, 10, 5);
                //graphic.DrawEllipse(pen, 150, 350, 10, 5);
                //graphic.DrawEllipse(pen, 550, 550, 10, 5);
                //graphic.DrawEllipse(pen, 150, 250, 10, 5);
                //graphic.DrawEllipse(pen, 450, 750, 10, 5);

                //for (int i = 0; i < 1000; i++)
                //{
                //    graphic.DrawLine(pen, p1, p2);
                //    graphic.DrawLine(pen1, p1, p2);
                //    graphic.DrawLine(pen, new Point(150, 350), new Point(550, 550));
                //    graphic.DrawLine(pen1, new Point(150, 350), new Point(550, 550));

                //}
            });
            
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void panel1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}

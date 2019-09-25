﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace WpfApp1
{
    class DrawGraph
    {
        Panel panel;
        Graph graph;
        Brush brush;

        public DrawGraph(Panel panl, Graph gr)
        {
            panel = panl;
            graph = gr;
            brush = Brushes.Red;
        }

        public DrawGraph(Panel panl, Graph gr, Brush br)
        {
            panel = panl;
            graph = gr;
            brush = br;
        }

        public void Clear()
        {
            panel.Children.Clear();
        }
        
        // рисуй все ключи (вершины), а затем только рисуй ребра, проходя по значениям
        public void Draw()
        {
            panel.Children.Clear(); 

            // идем по графу и рисуем
            var data = graph.GetGraph();
            Vertex[] dataKeys = data.Keys.ToArray();

            // рисуем вершины
            foreach (var vertex in dataKeys)
            {
                panel.Children.Add(EllipseFab.GetEllipse(new Point(vertex.X, vertex.Y), new Point(0, 0), brush, vertex.Name));
            }

            // рисуем ребра
            List<Edge> temp = new List<Edge>();
            for (int j = 0; j < dataKeys.Length - 1; j++)
            {
                for (int i = j + 1; i < dataKeys.Length; i++)      // к последнему нет смысла обращаться
                {
                    foreach (var edge in data[dataKeys[j]])
                    {   
                        if (edge.To == dataKeys[i])     //ReferenceEqual    // сравниваем попарно (точно знаем, что from это ключ)
                        {
                            temp.Add(edge);
                        }
                    }

                    // в обратную сторону
                    foreach (var edge in data[dataKeys[i]])
                    {
                        if (edge.To == dataKeys[j])    
                        {
                            temp.Add(edge);
                        }
                    }

                    Point middle = new Point();
                    for (int k = 0; k < temp.Count; k++)
                    {
                        // формируем middle точку
                        if (k == 0)
                        {
                            middle.X = temp[k].To.X - temp[k].From.X;
                            middle.Y = temp[k].To.Y - temp[k].From.Y;
                        }
                        else if (k % 2 != 0)
                        {
                            middle.X = -1 * (temp[k].To.X - temp[k].From.X) + k * 4;
                            middle.Y = temp[k].To.Y - temp[k].From.Y - 20;
                        }
                        else
                        {
                            middle.X = (temp[k].To.X - temp[k].From.X) + (k-1) * 4;
                            middle.Y = temp[k].To.Y - temp[k].From.Y - 20;
                        }

                        panel.Children.Add(BezPathFab.GetPath(new Point(temp[k].From.X, temp[k].From.Y), middle,
                                            new Point(temp[k].To.X, temp[k].To.Y), brush, temp[k].Name, temp[k].Orient));
                    }

                    temp.Clear();
                }
            }

            //собираем петли
            foreach (var key in data.Keys)
            {
                foreach (var edge in data[key])
                {
                    if (edge.From == edge.To)
                    {
                        panel.Children.Add(BezPathFab.GetPath(new Point(edge.From.X, edge.From.Y), new Point(edge.To.X + 50,
                                       edge.To.Y - 50), new Point(edge.To.X, edge.To.Y), brush, edge.Name, false));

                        break;      //петля для данной вершины всегда одна
                    }
                }
            }
        }
    }
}
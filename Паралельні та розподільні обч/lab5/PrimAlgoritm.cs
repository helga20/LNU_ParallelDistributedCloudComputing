using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab5
{
    
    class PGraph
    {

        public int[,] G;
        public int V;
        public Label pr;

        public PGraph(int[,] g, int v, Label pr)
        {
            G = g;
            V = v;
            this.pr = pr;
        }

        public string Prim()
        {
            pr.Text = "ACTIVE";
            pr.ForeColor = System.Drawing.Color.Green;
            Thread.Sleep(200);
            string result = "";
            int INF = 9999999;

            int edge_number = 0; 

            
            bool[] selected = new bool[V];

            for (int i = 0; i<selected.Length; i++)
            {
                selected[i] = false;
            }
            
            selected[0] = true;

            
            result += ("\r\nEdge : Weight");

            while (edge_number < V - 1)
            {
                int min = INF;
                int x = 0; // row 
                int y = 0; // col 

                for (int i = 0; i < V; i++)
                {
                    if (selected[i] == true)
                    {
                        for (int j = 0; j < V; j++)
                        {
                            if (!selected[j] && G[i,j] != 0)
                            {
                                if (min > G[i,j])
                                {
                                    min = G[i,j];
                                    x = i;
                                    y = j;
                                }
                            }
                        }
                    }
                }
                result += $"\r\n({x} - {y} : {G[x,y]})";
                selected[y] = true;
                edge_number++;
                
            }
            pr.Text = "INACTIVE";
            pr.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0);
            return result;
        }

    }
}

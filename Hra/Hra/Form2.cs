using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hra
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            strana.Add(panel1);
            strana.Add(panel2);
            strana.Add(panel3);
            strana.Add(panel4);
            strana.Add(panel5);
            strana.Add(panel6);
            strana.Add(panel7);
            strana.Add(panel9);
            strana.Add(panel10);
            strana.Add(panel11);

        }
        int page = 0;
        List<Panel> strana = new List<Panel>();

        private void button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            
            if(button.Text == "<")
            {
                if (0 < page)
                {
                    strana[page].Visible = false;
                    page--;
                    strana[page].Visible = true;
                }
            }
            else
            {
                if (page < strana.Count - 1)
                {
                    strana[page].Visible = false;
                    page++;
                    strana[page].Visible = true;
                }
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

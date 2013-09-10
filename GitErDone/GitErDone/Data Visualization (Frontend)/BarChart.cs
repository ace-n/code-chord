using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GitErDone.Data_Visualization__Frontend_
{
    public partial class BarChart : UserControl
    {
        public BarChart()
        {
            InitializeComponent();

            // Events
            this.Resize += new EventHandler(resize);
        }

        // Stored values
        private decimal v_minimum;
        private decimal v_maximum;
        public List<decimal> v_values; // Kept public so user can have easy access without the programmer having to write tons of functions

        // Properties
        public decimal minimum
        {
            get { return v_minimum; }
            set { v_minimum = value; }
        }
        public decimal maximum
        {
            get { return v_maximum; }
            set { v_maximum = value; }
        }

        // Draw method
        public void drawChart()
        {
            // Perform calculations
            int barWidth = this.Width / Math.Max(v_values.Count, 1);
            decimal barMax = Math.Max(v_values.Max(), v_maximum);
            decimal barMin = Math.Min(v_values.Min(), v_minimum);

            // Draw chart
            Image img = new Bitmap(this.Width, this.Height);
            Graphics gr = Graphics.FromImage(img);
            for (int i = 0; i < v_values.Count; i++)
            {
                // Get bar height
                int barHeight = (int)((v_values[i] - barMin) / (barMax - barMin));

                // Draw bar
                gr.FillRectangle((i % 2 == 0 ? Brushes.Orange : Brushes.Blue), barWidth * i, barHeight,
                    barWidth, barHeight);                    
            }
            pbx.Image = img;
        }

        // Resizing handler
        public void resize(object sender, EventArgs e)
        {
            pbx.Size = this.Size;
        }
    }
}

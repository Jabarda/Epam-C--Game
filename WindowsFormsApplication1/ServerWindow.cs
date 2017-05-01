using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class ServerWindow : Form
    {
        public Random rnd = new Random();

        public ServerWindow()
        {
            InitializeComponent();
        }

        public void AddLabel(string Name)
        {
            
            Console.WriteLine("Added " + Name);
            Label label1 = new Label();

            // Initialize the controls and their bounds.
            label1.Name = Name;
            label1.Text = Name;
            label1.Location = new System.Drawing.Point(rnd.Next(this.Size.Height), rnd.Next(this.Size.Width));
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            label1.ForeColor = Color.Black;
            label1.Visible = true;
            this.Controls.Add(label1);
        }
            

    }
}

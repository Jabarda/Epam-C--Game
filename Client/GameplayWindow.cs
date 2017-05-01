using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class GameplayWindow : Form
    {
        public GameplayWindow()
        {
            InitializeComponent();
            
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                Client.Move("Left");
                return true;
            }
            if (keyData == Keys.Right)
            {
                
                Client.Move("Right");
                return true;
            }
            if (keyData == Keys.Up)
            {
                
                Client.Move("Up");
                return true;
            }
            if (keyData == Keys.Down)
            {
               
                Client.Move("Down");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        public void AddLabel(string Name, string col, string posx, string posy)
        {

            Console.WriteLine("Added " + Name);
            Label label1 = new Label();

            // Initialize the controls and their bounds.
            label1.Name = Name;
            label1.Text = Name;
            label1.Location = new System.Drawing.Point(int.Parse(posx), int.Parse(posy));
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(int.Parse(col));
            label1.ForeColor = Color.Black;
            label1.Visible = true;
            this.Controls.Add(label1);
        }
    }
}

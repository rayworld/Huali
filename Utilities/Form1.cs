using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ray.Framework.Encrypt;

namespace Utilities
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = EncryptHelper.Decrypt("77052300", textBox2.Text);
            }
            else 
            {
                textBox2.Text = EncryptHelper.Encrypt("77052300", textBox1.Text);
            }
        }
    }
}

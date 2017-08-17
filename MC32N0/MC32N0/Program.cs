using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MC32N0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Form1 frm1 = new Form1();
            Application.Run(frm1);
        }
    }
}
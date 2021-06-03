﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;


namespace ProtokolApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            int idSluzbe;
            try
            {
                idSluzbe = Helper.GetIDSluzbe(Environment.UserName);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Greska sa konekcijom " + exception.ToString());
            }
            
            //if (idSluzbe == 6)
            //{
            //    Application.Run(new KreiranjeUsera());
            //}
            //else
            //{
            //    Application.Run(new RadFormOsnovniProtokol());
            //}
            //  AutoMapperConfiguration.Configure();
            Application.Run(new RadFormOsnovniProtokol());

        }
    }
}

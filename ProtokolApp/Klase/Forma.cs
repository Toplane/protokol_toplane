using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProtokolApp.Klase
{
   public class FormC : Telerik.WinControls.UI.RadForm
   {
        public  protokol p;
        public void Show(protokol Protokol)
        {
            p = Protokol;
            base.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace ProtokolApp
{
    class MyAutoCompleteEditor : RadTextBoxControlEditor
    {
        protected override Telerik.WinControls.RadElement CreateEditorElement()
        {
            return new RadAutoCompleteBoxElement();
        }

        public override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            RadAutoCompleteBoxElement element = this.EditorElement as RadAutoCompleteBoxElement;

            if (element.IsAutoCompleteDropDownOpen)
            {
                return;
            }

            base.OnKeyDown(e);
        }
    }
}

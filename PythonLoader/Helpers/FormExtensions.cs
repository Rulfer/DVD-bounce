using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Python_Loader.Helpers
{
    internal static class FormExtensions 
    {
        internal static void TextInvoke(this TextBox text, string s)
        {
            if (text.InvokeRequired)
            {
                text.Invoke((MethodInvoker)delegate
                {
                    text.Text = s;
                });
            }
            else
            {
                text.Text = s;
            }
        }

        internal static void ShowInvoke(this Control gui)
        {
            if (gui.InvokeRequired)
            {
                gui.Invoke((MethodInvoker)delegate
                {
                    gui.Show();
                });
            }
            else
            {
                gui.Show();
            }
        }

        internal static void HideInvoke(this Control gui)
        {
            if (gui.InvokeRequired)
            {
                gui.Invoke((MethodInvoker)delegate
                {
                    gui.Hide();
                });
            }
            else
            {
                gui.Hide();
            }
        }
    }
}

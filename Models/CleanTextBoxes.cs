using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QR_Checking_winVersion
{
    public class CleanTextBoxes
    {
        public static void Clear(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
                else
                {
                    Clear(child);
                }
            }
        }
    }
}

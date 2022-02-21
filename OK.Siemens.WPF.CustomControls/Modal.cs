using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OK.Siemens.WPF.CustomControls
{
    
    public class Modal : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen", typeof(bool), typeof(Modal), new PropertyMetadata(default(bool)));

        public bool IsOpen
        {
            get { return (bool) GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        
        static Modal()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Modal),
                new FrameworkPropertyMetadata(typeof(Modal)));
        }
    }
}
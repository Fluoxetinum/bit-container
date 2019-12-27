using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitContainer.Presentation.Helpers
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public static readonly DependencyProperty DataProperty = 
            DependencyProperty.Register("Data", typeof(Object), typeof(BindingProxy));

        public Object Data
        {
            get => (Object) GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}

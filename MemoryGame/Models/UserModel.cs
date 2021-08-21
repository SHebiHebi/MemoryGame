using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class UserModel : BindableBase
    {
        private int point;
        public int Point
        {
            get { return point; }
            set { SetProperty(ref point, value); }
        }
    }
}

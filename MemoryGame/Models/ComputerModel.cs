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
    public class ComputerModel : BindableBase
    {
        private int level;
        public int Level
        {
            get { return level; }
            set { SetProperty(ref level, value); }
        }

        private int point;
        public int Point
        {
            get { return point; }
            set { SetProperty(ref point, value); }
        }


        public ObservableCollection<TrumpModel> Storage { get; set; }

    }
}

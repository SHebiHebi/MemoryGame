using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class TrumpModel : BindableBase
    {
        private string number;
        public string Number
        {
            get { return number; }
            set { SetProperty(ref number, value); }
        }

        private string type;
        public string Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private string frontImage;
        public string FrontImage
        {
            get { return frontImage; }
            set { SetProperty(ref frontImage, value); }
        }

        private string backImage;
        public string BackImage
        {
            get { return backImage; }
            set { SetProperty(ref backImage, value); }
        }

        private string nowImage;
        public string NowImage
        {
            get { return nowImage; }
            set { SetProperty(ref nowImage, value); }
        }

        private bool isBack;
        public bool IsBack
        {
            get { return isBack; }
            set { SetProperty(ref isBack, value); }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }


        private bool isHitTestVisible;
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set { SetProperty(ref isHitTestVisible, value); }
        }

        public DelegateCommand<TrumpModel> ClickTrumpCommand { get; set; }

    }
}
